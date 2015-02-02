using System;

namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class SampleToWaveProvider : IWaveProvider
    {
        #region Поля

        private readonly ISampleProvider source;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        #endregion

        #region Конструктор

        public SampleToWaveProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Must be already floating point");
            }
            this.source = source;
        }

        #endregion

        #region Методы

        public int Read(byte[] buffer, int offset, int count)
        {
            int samplesNeeded = count / 4;
            WaveBuffer wb = new WaveBuffer(buffer);
            int samplesRead = source.Read(wb.FloatBuffer, offset / 4, samplesNeeded);
            return samplesRead * 4;
        }

        #endregion
    }
}