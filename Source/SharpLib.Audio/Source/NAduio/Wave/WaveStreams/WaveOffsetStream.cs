using System;

namespace SharpLib.Audio.Wave
{
    internal class WaveOffsetStream : WaveStream
    {
        #region Поля

        private readonly int bytesPerSample;

        private readonly object lockObject = new object();

        private long audioStartPosition;

        private long length;

        private long position;

        private TimeSpan sourceLength;

        private long sourceLengthBytes;

        private TimeSpan sourceOffset;

        private long sourceOffsetBytes;

        private WaveStream sourceStream;

        private TimeSpan startTime;

        #endregion

        #region Свойства

        public TimeSpan StartTime
        {
            get { return startTime; }
            set
            {
                lock (lockObject)
                {
                    startTime = value;
                    audioStartPosition = (long)(startTime.TotalSeconds * sourceStream.WaveFormat.SampleRate) * bytesPerSample;

                    length = audioStartPosition + sourceLengthBytes;
                    Position = Position;
                }
            }
        }

        public TimeSpan SourceOffset
        {
            get { return sourceOffset; }
            set
            {
                lock (lockObject)
                {
                    sourceOffset = value;
                    sourceOffsetBytes = (long)(sourceOffset.TotalSeconds * sourceStream.WaveFormat.SampleRate) * bytesPerSample;

                    Position = Position;
                }
            }
        }

        public TimeSpan SourceLength
        {
            get { return sourceLength; }
            set
            {
                lock (lockObject)
                {
                    sourceLength = value;
                    sourceLengthBytes = (long)(sourceLength.TotalSeconds * sourceStream.WaveFormat.SampleRate) * bytesPerSample;

                    length = audioStartPosition + sourceLengthBytes;
                    Position = Position;
                }
            }
        }

        public override int BlockAlign
        {
            get { return sourceStream.BlockAlign; }
        }

        public override long Length
        {
            get { return length; }
        }

        public override long Position
        {
            get { return position; }
            set
            {
                lock (lockObject)
                {
                    value -= (value % BlockAlign);
                    if (value < audioStartPosition)
                    {
                        sourceStream.Position = sourceOffsetBytes;
                    }
                    else
                    {
                        sourceStream.Position = sourceOffsetBytes + (value - audioStartPosition);
                    }
                    position = value;
                }
            }
        }

        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        #endregion

        #region Конструктор

        public WaveOffsetStream(WaveStream sourceStream, TimeSpan startTime, TimeSpan sourceOffset, TimeSpan sourceLength)
        {
            if (sourceStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
            {
                throw new ArgumentException("Only PCM supported");
            }

            this.sourceStream = sourceStream;
            position = 0;
            bytesPerSample = (sourceStream.WaveFormat.BitsPerSample / 8) * sourceStream.WaveFormat.Channels;
            StartTime = startTime;
            SourceOffset = sourceOffset;
            SourceLength = sourceLength;
        }

        public WaveOffsetStream(WaveStream sourceStream)
            : this(sourceStream, TimeSpan.Zero, TimeSpan.Zero, sourceStream.TotalTime)
        {
        }

        #endregion

        #region Методы

        public override int Read(byte[] destBuffer, int offset, int numBytes)
        {
            lock (lockObject)
            {
                int bytesWritten = 0;

                if (position < audioStartPosition)
                {
                    bytesWritten = (int)Math.Min(numBytes, audioStartPosition - position);
                    for (int n = 0; n < bytesWritten; n++)
                    {
                        destBuffer[n + offset] = 0;
                    }
                }
                if (bytesWritten < numBytes)
                {
                    int sourceBytesRequired = (int)Math.Min(
                        numBytes - bytesWritten,
                        sourceLengthBytes + sourceOffsetBytes - sourceStream.Position);
                    int read = sourceStream.Read(destBuffer, bytesWritten + offset, sourceBytesRequired);
                    bytesWritten += read;
                }

                for (int n = bytesWritten; n < numBytes; n++)
                {
                    destBuffer[offset + n] = 0;
                }
                position += numBytes;
                return numBytes;
            }
        }

        public override bool HasData(int count)
        {
            if (position + count < audioStartPosition)
            {
                return false;
            }
            if (position >= length)
            {
                return false;
            }

            return sourceStream.HasData(count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (sourceStream != null)
                {
                    sourceStream.Dispose();
                    sourceStream = null;
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "WaveOffsetStream was not Disposed");
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}