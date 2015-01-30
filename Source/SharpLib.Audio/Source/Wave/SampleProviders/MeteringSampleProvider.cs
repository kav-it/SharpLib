using System;

namespace NAudio.Wave.SampleProviders
{
    internal class MeteringSampleProvider : ISampleProvider
    {
        #region Поля

        private readonly StreamVolumeEventArgs args;

        private readonly int channels;

        private readonly float[] maxSamples;

        private readonly ISampleProvider source;

        private int sampleCount;

        #endregion

        #region Свойства

        public int SamplesPerNotification { get; set; }

        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        #endregion

        #region События

        public event EventHandler<StreamVolumeEventArgs> StreamVolume;

        #endregion

        #region Конструктор

        public MeteringSampleProvider(ISampleProvider source) :
            this(source, source.WaveFormat.SampleRate / 10)
        {
        }

        public MeteringSampleProvider(ISampleProvider source, int samplesPerNotification)
        {
            this.source = source;
            channels = source.WaveFormat.Channels;
            maxSamples = new float[channels];
            SamplesPerNotification = samplesPerNotification;
            args = new StreamVolumeEventArgs
            {
                MaxSampleValues = maxSamples
            };
        }

        #endregion

        #region Методы

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);

            if (StreamVolume != null)
            {
                for (int index = 0; index < samplesRead; index += channels)
                {
                    for (int channel = 0; channel < channels; channel++)
                    {
                        float sampleValue = Math.Abs(buffer[offset + index + channel]);
                        maxSamples[channel] = Math.Max(maxSamples[channel], sampleValue);
                    }
                    sampleCount++;
                    if (sampleCount >= SamplesPerNotification)
                    {
                        StreamVolume(this, args);
                        sampleCount = 0;

                        Array.Clear(maxSamples, 0, channels);
                    }
                }
            }
            return samplesRead;
        }

        #endregion
    }

    internal class StreamVolumeEventArgs : EventArgs
    {
        #region Свойства

        public float[] MaxSampleValues { get; set; }

        #endregion
    }
}