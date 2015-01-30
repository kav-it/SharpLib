using System;
using System.Collections.Generic;
using System.IO;

using SharpLib.Audio.Utils;

namespace SharpLib.Audio.Wave
{
    internal class AiffFileReader : WaveStream
    {
        #region Поля

        private readonly List<AiffChunk> chunks = new List<AiffChunk>();

        private readonly int dataChunkLength;

        private readonly long dataPosition;

        private readonly object lockObject = new object();

        private readonly bool ownInput;

        private readonly WaveFormat waveFormat;

        private Stream waveStream;

        #endregion

        #region Свойства

        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public override long Length
        {
            get { return dataChunkLength; }
        }

        public long SampleCount
        {
            get
            {
                if (waveFormat.Encoding == WaveFormatEncoding.Pcm ||
                    waveFormat.Encoding == WaveFormatEncoding.Extensible ||
                    waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                {
                    return dataChunkLength / BlockAlign;
                }
                throw new FormatException("Sample count is calculated only for the standard encodings");
            }
        }

        public override long Position
        {
            get { return waveStream.Position - dataPosition; }
            set
            {
                lock (lockObject)
                {
                    value = Math.Min(value, Length);

                    value -= (value % waveFormat.BlockAlign);
                    waveStream.Position = value + dataPosition;
                }
            }
        }

        #endregion

        #region Конструктор

        public AiffFileReader(String aiffFile) :
            this(File.OpenRead(aiffFile))
        {
            ownInput = true;
        }

        public AiffFileReader(Stream inputStream)
        {
            waveStream = inputStream;
            ReadAiffHeader(waveStream, out waveFormat, out dataPosition, out dataChunkLength, chunks);
            Position = 0;
        }

        #endregion

        #region Методы

        public static void ReadAiffHeader(Stream stream, out WaveFormat format, out long dataChunkPosition, out int dataChunkLength, List<AiffChunk> chunks)
        {
            dataChunkPosition = -1;
            format = null;
            BinaryReader br = new BinaryReader(stream);

            if (ReadChunkName(br) != "FORM")
            {
                throw new FormatException("Not an AIFF file - no FORM header.");
            }
            uint fileSize = ConvertInt(br.ReadBytes(4));
            string formType = ReadChunkName(br);
            if (formType != "AIFC" && formType != "AIFF")
            {
                throw new FormatException("Not an AIFF file - no AIFF/AIFC header.");
            }

            dataChunkLength = 0;

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                AiffChunk nextChunk = ReadChunkHeader(br);
                if (nextChunk.ChunkName == "COMM")
                {
                    short numChannels = ConvertShort(br.ReadBytes(2));
                    uint numSampleFrames = ConvertInt(br.ReadBytes(4));
                    short sampleSize = ConvertShort(br.ReadBytes(2));
                    double sampleRate = IEEE.ConvertFromIeeeExtended(br.ReadBytes(10));

                    format = new WaveFormat((int)sampleRate, sampleSize, numChannels);

                    if (nextChunk.ChunkLength > 18 && formType == "AIFC")
                    {
                        string compress = new string(br.ReadChars(4)).ToLower();
                        if (compress != "none")
                        {
                            throw new FormatException("Compressed AIFC is not supported.");
                        }
                        br.ReadBytes((int)nextChunk.ChunkLength - 22);
                    }
                    else
                    {
                        br.ReadBytes((int)nextChunk.ChunkLength - 18);
                    }
                }
                else if (nextChunk.ChunkName == "SSND")
                {
                    uint offset = ConvertInt(br.ReadBytes(4));
                    uint blockSize = ConvertInt(br.ReadBytes(4));
                    dataChunkPosition = nextChunk.ChunkStart + 16 + offset;
                    dataChunkLength = (int)nextChunk.ChunkLength - 8;

                    br.ReadBytes((int)nextChunk.ChunkLength - 8);
                }
                else
                {
                    if (chunks != null)
                    {
                        chunks.Add(nextChunk);
                    }
                    br.ReadBytes((int)nextChunk.ChunkLength);
                }

                if (nextChunk.ChunkName == "\0\0\0\0")
                {
                    break;
                }
            }

            if (format == null)
            {
                throw new FormatException("Invalid AIFF file - No COMM chunk found.");
            }
            if (dataChunkPosition == -1)
            {
                throw new FormatException("Invalid AIFF file - No SSND chunk found.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (waveStream != null)
                {
                    if (ownInput)
                    {
                        waveStream.Close();
                    }
                    waveStream = null;
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "AiffFileReader was not disposed");
            }

            base.Dispose(disposing);
        }

        public override int Read(byte[] array, int offset, int count)
        {
            if (count % waveFormat.BlockAlign != 0)
            {
                throw new ArgumentException(String.Format("Must read complete blocks: requested {0}, block align is {1}", count, WaveFormat.BlockAlign));
            }
            lock (lockObject)
            {
                if (Position + count > dataChunkLength)
                {
                    count = dataChunkLength - (int)Position;
                }

                byte[] buffer = new byte[count];
                int length = waveStream.Read(buffer, offset, count);

                int bytesPerSample = WaveFormat.BitsPerSample / 8;
                for (int i = 0; i < length; i += bytesPerSample)
                {
                    if (WaveFormat.BitsPerSample == 8)
                    {
                        array[i] = buffer[i];
                    }
                    else if (WaveFormat.BitsPerSample == 16)
                    {
                        array[i + 0] = buffer[i + 1];
                        array[i + 1] = buffer[i];
                    }
                    else if (WaveFormat.BitsPerSample == 24)
                    {
                        array[i + 0] = buffer[i + 2];
                        array[i + 1] = buffer[i + 1];
                        array[i + 2] = buffer[i + 0];
                    }
                    else if (WaveFormat.BitsPerSample == 32)
                    {
                        array[i + 0] = buffer[i + 3];
                        array[i + 1] = buffer[i + 2];
                        array[i + 2] = buffer[i + 1];
                        array[i + 3] = buffer[i + 0];
                    }
                    else
                    {
                        throw new FormatException("Unsupported PCM format.");
                    }
                }

                return length;
            }
        }

        private static uint ConvertInt(byte[] buffer)
        {
            if (buffer.Length != 4)
            {
                throw new Exception("Incorrect length for long.");
            }
            return (uint)((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]);
        }

        private static short ConvertShort(byte[] buffer)
        {
            if (buffer.Length != 2)
            {
                throw new Exception("Incorrect length for int.");
            }
            return (short)((buffer[0] << 8) | buffer[1]);
        }

        private static AiffChunk ReadChunkHeader(BinaryReader br)
        {
            var chunk = new AiffChunk((uint)br.BaseStream.Position, ReadChunkName(br), ConvertInt(br.ReadBytes(4)));
            return chunk;
        }

        private static string ReadChunkName(BinaryReader br)
        {
            return new string(br.ReadChars(4));
        }

        #endregion

        #region Вложенный класс: AiffChunk

        public struct AiffChunk
        {
            #region Поля

            public uint ChunkLength;

            public string ChunkName;

            public uint ChunkStart;

            #endregion

            #region Конструктор

            public AiffChunk(uint start, string name, uint length)
            {
                ChunkStart = start;
                ChunkName = name;
                ChunkLength = length + (uint)(length % 2 == 1 ? 1 : 0);
            }

            #endregion
        }

        #endregion
    }
}