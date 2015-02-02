using System;

namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class NotifyingSampleProvider : ISampleProvider, ISampleNotifier
    {
        #region Поля

        private readonly int channels;

        private readonly SampleEventArgs sampleArgs = new SampleEventArgs(0, 0);

        private readonly ISampleProvider source;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }

        #endregion

        #region События

        public event EventHandler<SampleEventArgs> Sample;

        #endregion

        #region Конструктор

        public NotifyingSampleProvider(ISampleProvider source)
        {
            this.source = source;
            channels = WaveFormat.Channels;
        }

        #endregion

        #region Методы

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);
            if (Sample != null)
            {
                for (int n = 0; n < samplesRead; n += channels)
                {
                    sampleArgs.Left = buffer[offset + n];
                    sampleArgs.Right = channels > 1 ? buffer[offset + n + 1] : sampleArgs.Left;
                    Sample(this, sampleArgs);
                }
            }
            return samplesRead;
        }

        #endregion
    }
}