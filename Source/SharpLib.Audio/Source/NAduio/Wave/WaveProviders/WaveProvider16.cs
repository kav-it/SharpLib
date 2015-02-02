namespace SharpLib.Audio.Wave
{
    internal abstract class WaveProvider16 : IWaveProvider
    {
        #region Поля

        private WaveFormat waveFormat;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        #endregion

        #region Конструктор

        public WaveProvider16()
            : this(44100, 1)
        {
        }

        public WaveProvider16(int sampleRate, int channels)
        {
            SetWaveFormat(sampleRate, channels);
        }

        #endregion

        #region Методы

        public void SetWaveFormat(int sampleRate, int channels)
        {
            waveFormat = new WaveFormat(sampleRate, 16, channels);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            WaveBuffer waveBuffer = new WaveBuffer(buffer);
            int samplesRequired = count / 2;
            int samplesRead = Read(waveBuffer.ShortBuffer, offset / 2, samplesRequired);
            return samplesRead * 2;
        }

        public abstract int Read(short[] buffer, int offset, int sampleCount);

        #endregion
    }
}