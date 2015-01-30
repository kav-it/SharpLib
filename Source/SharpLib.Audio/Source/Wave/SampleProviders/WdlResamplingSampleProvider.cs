using NAudio.Dsp;

namespace NAudio.Wave.SampleProviders
{
    internal class WdlResamplingSampleProvider : ISampleProvider
    {
        #region Поля

        private readonly int channels;

        private readonly WaveFormat outFormat;

        private readonly WdlResampler resampler;

        private readonly ISampleProvider source;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return outFormat; }
        }

        #endregion

        #region Конструктор

        public WdlResamplingSampleProvider(ISampleProvider source, int newSampleRate)
        {
            channels = source.WaveFormat.Channels;
            outFormat = WaveFormat.CreateIeeeFloatWaveFormat(newSampleRate, channels);
            this.source = source;

            resampler = new WdlResampler();
            resampler.SetMode(true, 2, false);
            resampler.SetFilterParms();
            resampler.SetFeedMode(false);
            resampler.SetRates(source.WaveFormat.SampleRate, newSampleRate);
        }

        #endregion

        #region Методы

        public int Read(float[] buffer, int offset, int count)
        {
            float[] inBuffer;
            int inBufferOffset;
            int framesRequested = count / channels;
            int inNeeded = resampler.ResamplePrepare(framesRequested, outFormat.Channels, out inBuffer, out inBufferOffset);
            int inAvailable = source.Read(inBuffer, inBufferOffset, inNeeded * channels) / channels;
            int outAvailable = resampler.ResampleOut(buffer, offset, inAvailable, framesRequested, channels);
            return outAvailable * channels;
        }

        #endregion
    }
}