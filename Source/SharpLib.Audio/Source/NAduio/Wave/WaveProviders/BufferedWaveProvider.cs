using System;

using SharpLib.Audio.Utils;

namespace SharpLib.Audio.Wave
{
    internal class BufferedWaveProvider : IWaveProvider
    {
        #region Поля

        private readonly WaveFormat waveFormat;

        private CircularBuffer circularBuffer;

        #endregion

        #region Свойства

        public int BufferLength { get; set; }

        public TimeSpan BufferDuration
        {
            get { return TimeSpan.FromSeconds((double)BufferLength / WaveFormat.AverageBytesPerSecond); }
            set { BufferLength = (int)(value.TotalSeconds * WaveFormat.AverageBytesPerSecond); }
        }

        public bool DiscardOnBufferOverflow { get; set; }

        public int BufferedBytes
        {
            get { return circularBuffer == null ? 0 : circularBuffer.Count; }
        }

        public TimeSpan BufferedDuration
        {
            get { return TimeSpan.FromSeconds((double)BufferedBytes / WaveFormat.AverageBytesPerSecond); }
        }

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        #endregion

        #region Конструктор

        public BufferedWaveProvider(WaveFormat waveFormat)
        {
            this.waveFormat = waveFormat;
            BufferLength = waveFormat.AverageBytesPerSecond * 5;
        }

        #endregion

        #region Методы

        public void AddSamples(byte[] buffer, int offset, int count)
        {
            if (circularBuffer == null)
            {
                circularBuffer = new CircularBuffer(BufferLength);
            }

            var written = circularBuffer.Write(buffer, offset, count);
            if (written < count && !DiscardOnBufferOverflow)
            {
                throw new InvalidOperationException("Buffer full");
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int read = 0;
            if (circularBuffer != null)
            {
                read = circularBuffer.Read(buffer, offset, count);
            }
            if (read < count)
            {
                Array.Clear(buffer, offset + read, count - read);
            }
            return count;
        }

        public void ClearBuffer()
        {
            if (circularBuffer != null)
            {
                circularBuffer.Reset();
            }
        }

        #endregion
    }
}