using System;
using System.IO;

using SharpLib.Audio.Wave.SampleProviders;

namespace SharpLib.Audio.Wave
{
    internal class WaveFileWriter : Stream
    {
        #region Поля

        private readonly string filename;

        private readonly WaveFormat format;

        private readonly byte[] value24 = new byte[3];

        private readonly BinaryWriter writer;

        private long dataChunkSize;

        private long dataSizePos;

        private long factSampleCountPos;

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
            set { throw new InvalidOperationException("Repositioning a WaveFileWriter is not supported"); }
        }

        #endregion

        #region Конструктор

        public WaveFileWriter(Stream outStream, WaveFormat format)
        {
            this.outStream = outStream;
            this.format = format;
            writer = new BinaryWriter(outStream, System.Text.Encoding.UTF8);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
            writer.Write(0);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));

            writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
            format.Serialize(writer);

            CreateFactChunk();
            WriteDataChunkHeader();
        }

        public WaveFileWriter(string filename, WaveFormat format)
            : this(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read), format)
        {
            this.filename = filename;
        }

        ~WaveFileWriter()
        {
            System.Diagnostics.Debug.Assert(false, "WaveFileWriter was not disposed");
            Dispose(false);
        }

        #endregion

        #region Методы

        public static void CreateWaveFile16(string filename, ISampleProvider sourceProvider)
        {
            CreateWaveFile(filename, new SampleToWaveProvider16(sourceProvider));
        }

        public static void CreateWaveFile(string filename, IWaveProvider sourceProvider)
        {
            using (var writer = new WaveFileWriter(filename, sourceProvider.WaveFormat))
            {
                long outputLength = 0;
                var buffer = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond * 4];
                while (true)
                {
                    int bytesRead = sourceProvider.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    outputLength += bytesRead;

                    writer.Write(buffer, 0, bytesRead);
                }
            }
        }

        private void WriteDataChunkHeader()
        {
            writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
            dataSizePos = outStream.Position;
            writer.Write(0);
        }

        private void CreateFactChunk()
        {
            if (HasFactChunk())
            {
                writer.Write(System.Text.Encoding.UTF8.GetBytes("fact"));
                writer.Write(4);
                factSampleCountPos = outStream.Position;
                writer.Write(0);
            }
        }

        private bool HasFactChunk()
        {
            return format.Encoding != WaveFormatEncoding.Pcm &&
                   format.BitsPerSample != 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException("Cannot read from a WaveFileWriter");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException("Cannot seek within a WaveFileWriter");
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("Cannot set length of a WaveFileWriter");
        }

        [Obsolete("Use Write instead")]
        public void WriteData(byte[] data, int offset, int count)
        {
            Write(data, offset, count);
        }

        public override void Write(byte[] data, int offset, int count)
        {
            if (outStream.Length + count > UInt32.MaxValue)
            {
                throw new ArgumentException("WAV file too large", "count");
            }
            outStream.Write(data, offset, count);
            dataChunkSize += count;
        }

        public void WriteSample(float sample)
        {
            if (WaveFormat.BitsPerSample == 16)
            {
                writer.Write((Int16)(Int16.MaxValue * sample));
                dataChunkSize += 2;
            }
            else if (WaveFormat.BitsPerSample == 24)
            {
                var value = BitConverter.GetBytes((Int32)(Int32.MaxValue * sample));
                value24[0] = value[1];
                value24[1] = value[2];
                value24[2] = value[3];
                writer.Write(value24);
                dataChunkSize += 3;
            }
            else if (WaveFormat.BitsPerSample == 32 && WaveFormat.Encoding == WaveFormatEncoding.Extensible)
            {
                writer.Write(UInt16.MaxValue * (Int32)sample);
                dataChunkSize += 4;
            }
            else if (WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                writer.Write(sample);
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

        [Obsolete("Use WriteSamples instead")]
        public void WriteData(short[] samples, int offset, int count)
        {
            WriteSamples(samples, offset, count);
        }

        public void WriteSamples(short[] samples, int offset, int count)
        {
            if (WaveFormat.BitsPerSample == 16)
            {
                for (int sample = 0; sample < count; sample++)
                {
                    writer.Write(samples[sample + offset]);
                }
                dataChunkSize += (count * 2);
            }

            else if (WaveFormat.BitsPerSample == 24)
            {
                byte[] value;
                for (int sample = 0; sample < count; sample++)
                {
                    value = BitConverter.GetBytes(UInt16.MaxValue * samples[sample + offset]);
                    value24[0] = value[1];
                    value24[1] = value[2];
                    value24[2] = value[3];
                    writer.Write(value24);
                }
                dataChunkSize += (count * 3);
            }

            else if (WaveFormat.BitsPerSample == 32 && WaveFormat.Encoding == WaveFormatEncoding.Extensible)
            {
                for (int sample = 0; sample < count; sample++)
                {
                    writer.Write(UInt16.MaxValue * samples[sample + offset]);
                }
                dataChunkSize += (count * 4);
            }

            else if (WaveFormat.BitsPerSample == 32 && WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                for (int sample = 0; sample < count; sample++)
                {
                    writer.Write(samples[sample + offset] / (float)(Int16.MaxValue + 1));
                }
                dataChunkSize += (count * 4);
            }
            else
            {
                throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
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
            UpdateRiffChunk(writer);
            UpdateFactChunk(writer);
            UpdateDataChunk(writer);
        }

        private void UpdateDataChunk(BinaryWriter writer)
        {
            writer.Seek((int)dataSizePos, SeekOrigin.Begin);
            writer.Write((UInt32)dataChunkSize);
        }

        private void UpdateRiffChunk(BinaryWriter writer)
        {
            writer.Seek(4, SeekOrigin.Begin);
            writer.Write((UInt32)(outStream.Length - 8));
        }

        private void UpdateFactChunk(BinaryWriter writer)
        {
            if (HasFactChunk())
            {
                int bitsPerSample = (format.BitsPerSample * format.Channels);
                if (bitsPerSample != 0)
                {
                    writer.Seek((int)factSampleCountPos, SeekOrigin.Begin);

                    writer.Write((int)((dataChunkSize * 8) / bitsPerSample));
                }
            }
        }

        #endregion
    }
}