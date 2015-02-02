using System;
using System.Collections.Generic;
using System.IO;

using SharpLib.Audio.FileFormats.Wav;

namespace SharpLib.Audio.Wave
{
    internal class WaveFileReader : WaveStream
    {
        #region Поля

        private readonly List<RiffChunk> _chunks;

        private readonly long _dataChunkLength;

        private readonly long _dataPosition;

        private readonly object _lockObject;

        private readonly bool _ownInput;

        private readonly WaveFormat _waveFormat;

        private Stream _waveStream;

        #endregion

        #region Свойства

        public List<RiffChunk> ExtraChunks
        {
            get { return _chunks; }
        }

        public override WaveFormat WaveFormat
        {
            get { return _waveFormat; }
        }

        public override long Length
        {
            get { return _dataChunkLength; }
        }

        public long SampleCount
        {
            get
            {
                if (_waveFormat.Encoding == WaveFormatEncoding.Pcm ||
                    _waveFormat.Encoding == WaveFormatEncoding.Extensible ||
                    _waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                {
                    return _dataChunkLength / BlockAlign;
                }

                throw new InvalidOperationException("Sample count is calculated only for the standard encodings");
            }
        }

        public override long Position
        {
            get { return _waveStream.Position - _dataPosition; }
            set
            {
                lock (_lockObject)
                {
                    value = Math.Min(value, Length);

                    value -= (value % _waveFormat.BlockAlign);
                    _waveStream.Position = value + _dataPosition;
                }
            }
        }

        #endregion

        #region Конструктор

        public WaveFileReader(String waveFile) :
            this(File.OpenRead(waveFile))
        {
            _ownInput = true;
        }

        public WaveFileReader(Stream inputStream)
        {
            _lockObject = new object();
            _chunks = new List<RiffChunk>();
            _waveStream = inputStream;
            var chunkReader = new WaveFileChunkReader();
            chunkReader.ReadWaveHeader(inputStream);
            _waveFormat = chunkReader.WaveFormat;
            _dataPosition = chunkReader.DataChunkPosition;
            _dataChunkLength = chunkReader.DataChunkLength;
            _chunks = chunkReader.RiffChunks;
            Position = 0;
        }

        #endregion

        #region Методы

        public byte[] GetChunkData(RiffChunk chunk)
        {
            long oldPosition = _waveStream.Position;
            _waveStream.Position = chunk.StreamPosition;
            byte[] data = new byte[chunk.Length];
            _waveStream.Read(data, 0, data.Length);
            _waveStream.Position = oldPosition;
            return data;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_waveStream != null)
                {
                    if (_ownInput)
                    {
                        _waveStream.Close();
                    }
                    _waveStream = null;
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "WaveFileReader was not disposed");
            }

            base.Dispose(disposing);
        }

        public override int Read(byte[] array, int offset, int count)
        {
            if (count % _waveFormat.BlockAlign != 0)
            {
                throw new ArgumentException(String.Format("Must read complete blocks: requested {0}, block align is {1}", count, WaveFormat.BlockAlign));
            }
            lock (_lockObject)
            {
                if (Position + count > _dataChunkLength)
                {
                    count = (int)(_dataChunkLength - Position);
                }
                return _waveStream.Read(array, offset, count);
            }
        }

        public float[] ReadNextSampleFrame()
        {
            switch (_waveFormat.Encoding)
            {
                case WaveFormatEncoding.Pcm:
                case WaveFormatEncoding.IeeeFloat:
                case WaveFormatEncoding.Extensible:
                    break;
                default:
                    throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
            }
            var sampleFrame = new float[_waveFormat.Channels];
            int bytesToRead = _waveFormat.Channels * (_waveFormat.BitsPerSample / 8);
            byte[] raw = new byte[bytesToRead];
            int bytesRead = Read(raw, 0, bytesToRead);
            if (bytesRead == 0)
            {
                return null;
            }
            if (bytesRead < bytesToRead)
            {
                throw new InvalidDataException("Unexpected end of file");
            }
            int offset = 0;
            for (int channel = 0; channel < _waveFormat.Channels; channel++)
            {
                if (_waveFormat.BitsPerSample == 16)
                {
                    sampleFrame[channel] = BitConverter.ToInt16(raw, offset) / 32768f;
                    offset += 2;
                }
                else if (_waveFormat.BitsPerSample == 24)
                {
                    sampleFrame[channel] = (((sbyte)raw[offset + 2] << 16) | (raw[offset + 1] << 8) | raw[offset]) / 8388608f;
                    offset += 3;
                }
                else if (_waveFormat.BitsPerSample == 32 && _waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                {
                    sampleFrame[channel] = BitConverter.ToSingle(raw, offset);
                    offset += 4;
                }
                else if (_waveFormat.BitsPerSample == 32)
                {
                    sampleFrame[channel] = BitConverter.ToInt32(raw, offset) / (Int32.MaxValue + 1f);
                    offset += 4;
                }
                else
                {
                    throw new InvalidOperationException("Unsupported bit depth");
                }
            }
            return sampleFrame;
        }

        [Obsolete("Use ReadNextSampleFrame instead (this version does not support stereo properly)")]
        public bool TryReadFloat(out float sampleValue)
        {
            var sf = ReadNextSampleFrame();
            sampleValue = sf != null ? sf[0] : 0;
            return sf != null;
        }

        #endregion
    }
}