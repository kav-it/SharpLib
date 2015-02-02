using System;

using SharpLib.Audio.Utils;

namespace SharpLib.Audio.Wave
{
    internal class BlockAlignReductionStream : WaveStream
    {
        #region Поля

        private readonly CircularBuffer circularBuffer;

        private readonly object lockObject = new object();

        private long bufferStartPosition;

        private long position;

        private byte[] sourceBuffer;

        private WaveStream sourceStream;

        #endregion

        #region Свойства

        public override int BlockAlign
        {
            get { return (WaveFormat.BitsPerSample / 8) * WaveFormat.Channels; }
        }

        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        public override long Length
        {
            get { return sourceStream.Length; }
        }

        public override long Position
        {
            get { return position; }
            set
            {
                lock (lockObject)
                {
                    if (position != value)
                    {
                        if (position % BlockAlign != 0)
                        {
                            throw new ArgumentException("Position must be block aligned");
                        }
                        long sourcePosition = value - (value % sourceStream.BlockAlign);
                        if (sourceStream.Position != sourcePosition)
                        {
                            sourceStream.Position = sourcePosition;
                            circularBuffer.Reset();
                            bufferStartPosition = sourceStream.Position;
                        }
                        position = value;
                    }
                }
            }
        }

        private long BufferEndPosition
        {
            get { return bufferStartPosition + circularBuffer.Count; }
        }

        #endregion

        #region Конструктор

        public BlockAlignReductionStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            circularBuffer = new CircularBuffer(sourceStream.WaveFormat.AverageBytesPerSecond * 4);
        }

        #endregion

        #region Методы

        private byte[] GetSourceBuffer(int size)
        {
            if (sourceBuffer == null || sourceBuffer.Length < size)
            {
                sourceBuffer = new byte[size * 2];
            }
            return sourceBuffer;
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
                System.Diagnostics.Debug.Assert(false, "BlockAlignReductionStream was not Disposed");
            }
            base.Dispose(disposing);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (lockObject)
            {
                while (BufferEndPosition < position + count)
                {
                    int sourceReadCount = count;
                    if (sourceReadCount % sourceStream.BlockAlign != 0)
                    {
                        sourceReadCount = (count + sourceStream.BlockAlign) - (count % sourceStream.BlockAlign);
                    }

                    int sourceRead = sourceStream.Read(GetSourceBuffer(sourceReadCount), 0, sourceReadCount);
                    circularBuffer.Write(GetSourceBuffer(sourceReadCount), 0, sourceRead);
                    if (sourceRead == 0)
                    {
                        break;
                    }
                }

                if (bufferStartPosition < position)
                {
                    circularBuffer.Advance((int)(position - bufferStartPosition));
                    bufferStartPosition = position;
                }

                int bytesRead = circularBuffer.Read(buffer, offset, count);
                position += bytesRead;

                bufferStartPosition = position;

                return bytesRead;
            }
        }

        #endregion
    }
}