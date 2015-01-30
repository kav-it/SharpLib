using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NAudio.Wave
{
    internal class Mp3Index
    {
        #region Свойства

        public long FilePosition { get; set; }

        public long SamplePosition { get; set; }

        public int SampleCount { get; set; }

        public int ByteCount { get; set; }

        #endregion
    }

    internal class Mp3FileReader : WaveStream
    {
        #region Делегаты

        public delegate IMp3FrameDecompressor FrameDecompressorBuilder(WaveFormat mp3Format);

        #endregion

        #region Поля

        private readonly int _bytesPerSample;

        private readonly long _dataStartPosition;

        private readonly byte[] _decompressBuffer;

        private readonly byte[] _id3V1Tag;

        private readonly Id3v2Tag _id3V2Tag;

        private readonly long _mp3DataLength;

        private readonly bool _ownInputStream;

        private readonly object _repositionLock;

        private readonly WaveFormat _waveFormat;

        private readonly XingHeader _xingHeader;

        private int _decompressBufferOffset;

        private int _decompressLeftovers;

        private IMp3FrameDecompressor _decompressor;

        private Stream _mp3Stream;

        private bool _repositionedFlag;

        private List<Mp3Index> _tableOfContents;

        private int _tocIndex;

        private long _totalSamples;

        #endregion

        #region Свойства

        public Mp3WaveFormat Mp3WaveFormat { get; private set; }

        public Id3v2Tag Id3V2Tag
        {
            get { return _id3V2Tag; }
        }

        public byte[] Id3V1Tag
        {
            get { return _id3V1Tag; }
        }

        public override long Length
        {
            get { return _totalSamples * _bytesPerSample; }
        }

        public override WaveFormat WaveFormat
        {
            get { return _waveFormat; }
        }

        public override long Position
        {
            get
            {
                if (_tocIndex >= _tableOfContents.Count)
                {
                    return Length;
                }
                return (_tableOfContents[_tocIndex].SamplePosition * _bytesPerSample) + _decompressBufferOffset;
            }
            set
            {
                lock (_repositionLock)
                {
                    value = Math.Max(Math.Min(value, Length), 0);
                    var samplePosition = value / _bytesPerSample;
                    Mp3Index mp3Index = null;
                    for (int index = 0; index < _tableOfContents.Count; index++)
                    {
                        if (_tableOfContents[index].SamplePosition >= samplePosition)
                        {
                            mp3Index = _tableOfContents[index];
                            _tocIndex = index;
                            break;
                        }
                    }
                    if (mp3Index != null)
                    {
                        _mp3Stream.Position = mp3Index.FilePosition;
                    }
                    else
                    {
                        _mp3Stream.Position = _mp3DataLength + _dataStartPosition;
                    }
                    _decompressBufferOffset = 0;
                    _decompressLeftovers = 0;
                    _repositionedFlag = true;
                }
            }
        }

        public XingHeader XingHeader
        {
            get { return _xingHeader; }
        }

        #endregion

        #region Конструктор

        public Mp3FileReader(string mp3FileName)
            : this(File.OpenRead(mp3FileName))
        {
            _ownInputStream = true;
        }

        public Mp3FileReader(string mp3FileName, FrameDecompressorBuilder frameDecompressorBuilder)
            : this(File.OpenRead(mp3FileName), frameDecompressorBuilder)
        {
            _ownInputStream = true;
        }

        public Mp3FileReader(Stream inputStream)
            : this(inputStream, CreateAcmFrameDecompressor)
        {
        }

        public Mp3FileReader(Stream inputStream, FrameDecompressorBuilder frameDecompressorBuilder)
        {
            _repositionLock = new object();
            _mp3Stream = inputStream;
            _id3V2Tag = Id3v2Tag.ReadTag(_mp3Stream);

            _dataStartPosition = _mp3Stream.Position;
            var firstFrame = Mp3Frame.LoadFromStream(_mp3Stream);
            double bitRate = firstFrame.BitRate;
            _xingHeader = XingHeader.LoadXingHeader(firstFrame);

            if (_xingHeader != null)
            {
                _dataStartPosition = _mp3Stream.Position;
            }

            var secondFrame = Mp3Frame.LoadFromStream(_mp3Stream);
            if (secondFrame != null &&
                (secondFrame.SampleRate != firstFrame.SampleRate ||
                 secondFrame.ChannelMode != firstFrame.ChannelMode))
            {
                _dataStartPosition = secondFrame.FileOffset;

                firstFrame = secondFrame;
            }

            _mp3DataLength = _mp3Stream.Length - _dataStartPosition;

            _mp3Stream.Position = _mp3Stream.Length - 128;
            byte[] tag = new byte[128];
            _mp3Stream.Read(tag, 0, 128);
            if (tag[0] == 'T' && tag[1] == 'A' && tag[2] == 'G')
            {
                _id3V1Tag = tag;
                _mp3DataLength -= 128;
            }

            _mp3Stream.Position = _dataStartPosition;

            Mp3WaveFormat = new Mp3WaveFormat(firstFrame.SampleRate, firstFrame.ChannelMode == ChannelMode.Mono ? 1 : 2, firstFrame.FrameLength, (int)bitRate);

            CreateTableOfContents();
            _tocIndex = 0;

            bitRate = (_mp3DataLength * 8.0 / TotalSeconds());

            _mp3Stream.Position = _dataStartPosition;

            Mp3WaveFormat = new Mp3WaveFormat(firstFrame.SampleRate, firstFrame.ChannelMode == ChannelMode.Mono ? 1 : 2, firstFrame.FrameLength, (int)bitRate);
            _decompressor = frameDecompressorBuilder(Mp3WaveFormat);
            _waveFormat = _decompressor.OutputFormat;
            _bytesPerSample = (_decompressor.OutputFormat.BitsPerSample) / 8 * _decompressor.OutputFormat.Channels;

            _decompressBuffer = new byte[1152 * _bytesPerSample * 2];
        }

        #endregion

        #region Методы

        public static IMp3FrameDecompressor CreateAcmFrameDecompressor(WaveFormat mp3Format)
        {
            return new AcmMp3FrameDecompressor(mp3Format);
        }

        private void CreateTableOfContents()
        {
            try
            {
                _tableOfContents = new List<Mp3Index>((int)(_mp3DataLength / 400));
                Mp3Frame frame;
                do
                {
                    var index = new Mp3Index();
                    index.FilePosition = _mp3Stream.Position;
                    index.SamplePosition = _totalSamples;
                    frame = ReadNextFrame(false);
                    if (frame != null)
                    {
                        ValidateFrameFormat(frame);

                        _totalSamples += frame.SampleCount;
                        index.SampleCount = frame.SampleCount;
                        index.ByteCount = (int)(_mp3Stream.Position - index.FilePosition);
                        _tableOfContents.Add(index);
                    }
                } while (frame != null);
            }
            catch (EndOfStreamException)
            {
            }
        }

        private void ValidateFrameFormat(Mp3Frame frame)
        {
            if (frame.SampleRate != Mp3WaveFormat.SampleRate)
            {
                string message =
                    String.Format(
                        "Got a frame at sample rate {0}, in an MP3 with sample rate {1}. Mp3FileReader does not support sample rate changes.",
                        frame.SampleRate, Mp3WaveFormat.SampleRate);
                throw new InvalidOperationException(message);
            }
            int channels = frame.ChannelMode == ChannelMode.Mono ? 1 : 2;
            if (channels != Mp3WaveFormat.Channels)
            {
                string message =
                    String.Format(
                        "Got a frame with channel mode {0}, in an MP3 with {1} channels. Mp3FileReader does not support changes to channel count.",
                        frame.ChannelMode, Mp3WaveFormat.Channels);
                throw new InvalidOperationException(message);
            }
        }

        private double TotalSeconds()
        {
            return (double)_totalSamples / Mp3WaveFormat.SampleRate;
        }

        public Mp3Frame ReadNextFrame()
        {
            return ReadNextFrame(true);
        }

        private Mp3Frame ReadNextFrame(bool readData)
        {
            Mp3Frame frame = null;
            try
            {
                frame = Mp3Frame.LoadFromStream(_mp3Stream, readData);
                if (frame != null)
                {
                    _tocIndex++;
                }
            }
            catch (EndOfStreamException)
            {
            }
            return frame;
        }

        public override int Read(byte[] sampleBuffer, int offset, int numBytes)
        {
            int bytesRead = 0;
            lock (_repositionLock)
            {
                if (_decompressLeftovers != 0)
                {
                    int toCopy = Math.Min(_decompressLeftovers, numBytes);
                    Array.Copy(_decompressBuffer, _decompressBufferOffset, sampleBuffer, offset, toCopy);
                    _decompressLeftovers -= toCopy;
                    if (_decompressLeftovers == 0)
                    {
                        _decompressBufferOffset = 0;
                    }
                    else
                    {
                        _decompressBufferOffset += toCopy;
                    }
                    bytesRead += toCopy;
                    offset += toCopy;
                }

                while (bytesRead < numBytes)
                {
                    Mp3Frame frame = ReadNextFrame();
                    if (frame != null)
                    {
                        if (_repositionedFlag)
                        {
                            _decompressor.Reset();
                            _repositionedFlag = false;
                        }
                        int decompressed = _decompressor.DecompressFrame(frame, _decompressBuffer, 0);

                        int toCopy = Math.Min(decompressed, numBytes - bytesRead);
                        Array.Copy(_decompressBuffer, 0, sampleBuffer, offset, toCopy);
                        if (toCopy < decompressed)
                        {
                            _decompressBufferOffset = toCopy;
                            _decompressLeftovers = decompressed - toCopy;
                        }
                        else
                        {
                            _decompressBufferOffset = 0;
                            _decompressLeftovers = 0;
                        }
                        offset += toCopy;
                        bytesRead += toCopy;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            Debug.Assert(bytesRead <= numBytes, "MP3 File Reader read too much");
            return bytesRead;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_mp3Stream != null)
                {
                    if (_ownInputStream)
                    {
                        _mp3Stream.Dispose();
                    }
                    _mp3Stream = null;
                }
                if (_decompressor != null)
                {
                    _decompressor.Dispose();
                    _decompressor = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}