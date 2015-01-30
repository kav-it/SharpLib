using System.IO;

namespace NAudio.Wave
{
    internal class RawSourceWaveStream : WaveStream
    {
        #region Поля

        private readonly Stream sourceStream;

        private readonly WaveFormat waveFormat;

        #endregion

        #region Свойства

        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public override long Length
        {
            get { return sourceStream.Length; }
        }

        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        #endregion

        #region Конструктор

        public RawSourceWaveStream(Stream sourceStream, WaveFormat waveFormat)
        {
            this.sourceStream = sourceStream;
            this.waveFormat = waveFormat;
        }

        #endregion

        #region Методы

        public override int Read(byte[] buffer, int offset, int count)
        {
            return sourceStream.Read(buffer, offset, count);
        }

        #endregion
    }
}