using System;
using System.IO;

using NAudio.Utils;

namespace NAudio.Wave
{
    internal class AiffFileWriter : Stream
    {
        #region Поля

        private readonly string filename;

        private readonly WaveFormat format;

        private readonly byte[] value24 = new byte[3];

        private readonly BinaryWriter writer;

        private long commSampleCountPos;

        private int dataChunkSize = 8;

        private long dataSizePos;

        private Stream outStream;

        #endregion

        #region Свойства

        public string Filename
        {
            get { return filename; }
        }

        public override long Length
        {
            get { return dataChunkSize; }
        }

        public WaveFormat WaveFormat
        {
            get { return format; }
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Position
        {
            get { return dataChunkSize; }
            set { throw new InvalidOperationException("Repositioning an AiffFileWriter is not supported"); }
        }

        #endregion

        #region Конструктор

        public AiffFileWriter(Stream outStream, WaveFormat format)
        {
            this.outStream = outStream;
            this.format = format;
            writer = new BinaryWriter(outStream, System.Text.Encoding.UTF8);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("FORM"));
            writer.Write(0);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("AIFF"));

            CreateCommChunk();
            WriteSsndChunkHeader();
        }

        public AiffFileWriter(string filename, WaveFormat format)
            : this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read), format)
        {
            this.filename = filename;
        }

        ~AiffFileWriter()
        {
            System.Diagnostics.Debug.Assert(false, "AiffFileWriter was not disposed");
            Dispose(false);
        }

        #endregion

        #region Методы

        public static void CreateAiffFile(string filename, WaveStream sourceProvider)
        {
            using (var writer = new AiffFileWriter(filename, sourceProvider.WaveFormat))
            {
                byte[] buffer = new byte[16384];

                while (sourceProvider.Position < sourceProvider.Length)
                {
                    int count = Math.Min((int)(sourceProvider.Length - sourceProvider.Position), buffer.Length);
                    int bytesRead = sourceProvider.Read(buffer, 0, count);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    writer.Write(buffer, 0, bytesRead);
                }
            }
        }

        private void WriteSsndChunkHeader()
        {
            writer.Write(System.Text.Encoding.UTF8.GetBytes("SSND"));
            dataSizePos = outStream.Position;
            writer.Write(0);
            writer.Write(0);
            writer.Write(SwapEndian(format.BlockAlign));
        }

        private byte[] SwapEndian(short n)
        {
            return new[] { (byte)(n >> 8), (byte)(n & 0xff) };
        }

        private byte[] SwapEndian(int n)
        {
            return new[] { (byte)((n >> 24) & 0xff), (byte)((n >> 16) & 0xff), (byte)((n >> 8) & 0xff), (byte)(n & 0xff) };
        }

        private void CreateCommChunk()
        {
            writer.Write(System.Text.Encoding.UTF8.GetBytes("COMM"));
            writer.Write(SwapEndian(18));
            writer.Write(SwapEndian((short)format.Channels));
            commSampleCountPos = outStream.Position;
            ;
            writer.Write(0);
            writer.Write(SwapEndian((short)format.BitsPerSample));
            writer.Write(IEEE.ConvertToIeeeExtended(format.SampleRate));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException("Cannot read from an AiffFileWriter");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException("Cannot seek within an AiffFileWriter");
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("Cannot set length of an AiffFileWriter");
        }

        public override void Write(byte[] data, int offset, int count)
        {
            byte[] swappedData = new byte[data.Length];

            int align = format.BitsPerSample / 8;

            for (int i = 0; i < data.Length; i++)
            {
                int pos = (int)Math.Floor((double)i / align) * align + (align - (i % align) - 1);
                swappedData[i] = data[pos];
            }

            outStream.Write(swappedData, offset, count);
            dataChunkSize += count;
        }

        public void WriteSample(float sample)
        {
            if (WaveFormat.BitsPerSample == 16)
            {
                writer.Write(SwapEndian((Int16)(Int16.MaxValue * sample)));
                dataChunkSize += 2;
            }
            else if (WaveFormat.BitsPerSample == 24)
            {
                var value = BitConverter.GetBytes((Int32)(Int32.MaxValue * sample));
                value24[2] = value[1];
                value24[1] = value[2];
                value24[0] = value[3];
                writer.Write(value24);
                dataChunkSize += 3;
            }
            else if (WaveFormat.BitsPerSample == 32 && WaveFormat.Encoding == NAudio.Wave.WaveFormatEncoding.Extensible)
            {
                writer.Write(SwapEndian(UInt16.MaxValue * (Int32)sample));
                dataChunkSize += 4;
            }
            else
            {
                throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
            }
        }

        public void WriteSamples(float[] samples, int offset, int count)
        {
            for (int n = 0; n < count; n++)
            {
                WriteSample(samples[offset + n]);
            }
        }

        public void WriteSamples(short[] samples, int offset, int count)
        {
            if (WaveFormat.BitsPerSample == 16)
            {
                for (int sample = 0; sample < count; sample++)
                {
                    writer.Write(SwapEndian(samples[sample + offset]));
                }
                dataChunkSize += (count * 2);
            }

            else if (WaveFormat.BitsPerSample == 24)
            {
                byte[] value;
                for (int sample = 0; sample < count; sample++)
                {
                    value = BitConverter.GetBytes(UInt16.MaxValue * samples[sample + offset]);
                    value24[2] = value[1];
                    value24[1] = value[2];
                    value24[0] = value[3];
                    writer.Write(value24);
                }
                dataChunkSize += (count * 3);
            }

            else if (WaveFormat.BitsPerSample == 32 && WaveFormat.Encoding == WaveFormatEncoding.Extensible)
            {
                for (int sample = 0; sample < count; sample++)
                {
                    writer.Write(SwapEndian(UInt16.MaxValue * samples[sample + offset]));
                }
                dataChunkSize += (count * 4);
            }
            else
            {
                throw new InvalidOperationException("Only 16, 24 or 32 bit PCM audio data supported");
            }
        }

        public override void Flush()
        {
            writer.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (outStream != null)
                {
                    try
                    {
                        UpdateHeader(writer);
                    }
                    finally
                    {
                        outStream.Close();
                        outStream = null;
                    }
                }
            }
        }

        protected virtual void UpdateHeader(BinaryWriter writer)
        {
            Flush();
            writer.Seek(4, SeekOrigin.Begin);
            writer.Write(SwapEndian((int)(outStream.Length - 8)));
            UpdateCommChunk(writer);
            UpdateSsndChunk(writer);
        }

        private void UpdateCommChunk(BinaryWriter writer)
        {
            writer.Seek((int)commSampleCountPos, SeekOrigin.Begin);
            writer.Write(SwapEndian(dataChunkSize * 8 / format.BitsPerSample / format.Channels));
        }

        private void UpdateSsndChunk(BinaryWriter writer)
        {
            writer.Seek((int)dataSizePos, SeekOrigin.Begin);
            writer.Write(SwapEndian(dataChunkSize));
        }

        #endregion
    }
}