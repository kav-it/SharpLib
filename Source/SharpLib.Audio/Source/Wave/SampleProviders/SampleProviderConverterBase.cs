using SharpLib.Audio.Utils;

namespace SharpLib.Audio.Wave.SampleProviders
{
    internal abstract class SampleProviderConverterBase : ISampleProvider
    {
        #region Поля

        private readonly WaveFormat waveFormat;

        protected IWaveProvider source;

        protected byte[] sourceBuffer;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        #endregion

        #region Конструктор

        public SampleProviderConverterBase(IWaveProvider source)
        {
            this.source = source;
            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, source.WaveFormat.Channels);
        }

        #endregion

        #region Методы

        public abstract int Read(float[] buffer, int offset, int count);

        protected void EnsureSourceBuffer(int sourceBytesRequired)
        {
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);
        }

        #endregion
    }
}