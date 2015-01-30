using System;

namespace NAudio.Wave
{
    internal class WaveRecorder : IWaveProvider, IDisposable
    {
        #region Поля

        private readonly IWaveProvider source;

        private WaveFileWriter writer;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        #endregion

        #region Конструктор

        public WaveRecorder(IWaveProvider source, string destination)
        {
            this.source = source;
            writer = new WaveFileWriter(destination, source.WaveFormat);
        }

        #endregion

        #region Методы

        public int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = source.Read(buffer, offset, count);
            writer.Write(buffer, offset, bytesRead);
            return bytesRead;
        }

        public void Dispose()
        {
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }

        #endregion
    }
}