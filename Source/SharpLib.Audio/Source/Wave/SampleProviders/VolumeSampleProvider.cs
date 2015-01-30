namespace NAudio.Wave.SampleProviders
{
    internal class VolumeSampleProvider : ISampleProvider
    {
        #region Поля

        private readonly ISampleProvider source;

        private float volume;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        #endregion

        #region Конструктор

        public VolumeSampleProvider(ISampleProvider source)
        {
            this.source = source;
            volume = 1.0f;
        }

        #endregion

        #region Методы

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);
            if (volume != 1f)
            {
                for (int n = 0; n < sampleCount; n++)
                {
                    buffer[offset + n] *= volume;
                }
            }
            return samplesRead;
        }

        #endregion
    }
}