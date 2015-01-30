using System;
using System.IO;

namespace NAudio.Wave
{
    internal abstract class WaveStream : Stream, IWaveProvider
    {
        #region Ñגמיסעגא

        public abstract WaveFormat WaveFormat { get; }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public virtual int BlockAlign
        {
            get { return WaveFormat.BlockAlign; }
        }

        public virtual TimeSpan CurrentTime
        {
            get { return TimeSpan.FromSeconds((double)Position / WaveFormat.AverageBytesPerSecond); }
            set { Position = (long)(value.TotalSeconds * WaveFormat.AverageBytesPerSecond); }
        }

        public virtual TimeSpan TotalTime
        {
            get { return TimeSpan.FromSeconds((double)Length / WaveFormat.AverageBytesPerSecond); }
        }

        #endregion

        #region Ìועמהû

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
            {
                Position = offset;
            }
            else if (origin == SeekOrigin.Current)
            {
                Position += offset;
            }
            else
            {
                Position = Length + offset;
            }
            return Position;
        }

        public override void SetLength(long length)
        {
            throw new NotSupportedException("Can't set length of a WaveFormatString");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Can't write to a WaveFormatString");
        }

        public void Skip(int seconds)
        {
            long newPosition = Position + WaveFormat.AverageBytesPerSecond * seconds;
            if (newPosition > Length)
            {
                Position = Length;
            }
            else if (newPosition < 0)
            {
                Position = 0;
            }
            else
            {
                Position = newPosition;
            }
        }

        public virtual bool HasData(int count)
        {
            return Position < Length;
        }

        #endregion
    }
}