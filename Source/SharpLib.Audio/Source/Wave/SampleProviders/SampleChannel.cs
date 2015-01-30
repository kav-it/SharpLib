using System;

namespace NAudio.Wave.SampleProviders
{
    internal class SampleChannel : ISampleProvider
    {
        #region Поля

        private readonly MeteringSampleProvider preVolumeMeter;

        private readonly VolumeSampleProvider volumeProvider;

        private readonly WaveFormat waveFormat;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public float Volume
        {
            get { return volumeProvider.Volume; }
            set { volumeProvider.Volume = value; }
        }

        #endregion

        #region События

        public event EventHandler<StreamVolumeEventArgs> PreVolumeMeter
        {
            add { preVolumeMeter.StreamVolume += value; }
            remove { preVolumeMeter.StreamVolume -= value; }
        }

        #endregion

        #region Конструктор

        public SampleChannel(IWaveProvider waveProvider)
            : this(waveProvider, false)
        {
        }

        public SampleChannel(IWaveProvider waveProvider, bool forceStereo)
        {
            ISampleProvider sampleProvider = SampleProviderConverters.ConvertWaveProviderIntoSampleProvider(waveProvider);
            if (sampleProvider.WaveFormat.Channels == 1 && forceStereo)
            {
                sampleProvider = new MonoToStereoSampleProvider(sampleProvider);
            }
            waveFormat = sampleProvider.WaveFormat;

            preVolumeMeter = new MeteringSampleProvider(sampleProvider);
            volumeProvider = new VolumeSampleProvider(preVolumeMeter);
        }

        #endregion

        #region Методы

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            return volumeProvider.Read(buffer, offset, sampleCount);
        }

        #endregion
    }
}