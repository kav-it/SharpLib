namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class VolumeSampleProvider : ISampleProvider
    {
        #region Поля

        private readonly ISampleProvider _source;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return _source.WaveFormat; }
        }

        public float Volume { get; set; }

        #endregion

        #region Конструктор

        public VolumeSampleProvider(ISampleProvider source)
        {
            this._source = source;
            Volume = 1.0f;
        }

        #endregion

        #region Методы

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = _source.Read(buffer, offset, sampleCount);
            if (Volume != 1f)
            {
                for (int n = 0; n < sampleCount; n++)
                {
                    buffer[offset + n] *= Volume;
                }
            }
            return samplesRead;
        }

        #endregion
    }
}