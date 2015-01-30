using System;

namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class MonoToStereoSampleProvider : ISampleProvider
    {
        #region Поля

        private readonly ISampleProvider source;

        private readonly WaveFormat waveFormat;

        private float[] sourceBuffer;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        #endregion

        #region Конструктор

        public MonoToStereoSampleProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Channels != 1)
            {
                throw new ArgumentException("Source must be mono");
            }
            this.source = source;
            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(source.WaveFormat.SampleRate, 2);
        }

        #endregion

        #region Методы

        public int Read(float[] buffer, int offset, int count)
        {
            int sourceSamplesRequired = count / 2;
            int outIndex = offset;
            EnsureSourceBuffer(sourceSamplesRequired);
            int sourceSamplesRead = source.Read(sourceBuffer, 0, sourceSamplesRequired);
            for (int n = 0; n < sourceSamplesRead; n++)
            {
                buffer[outIndex++] = sourceBuffer[n];
                buffer[outIndex++] = sourceBuffer[n];
            }
            return sourceSamplesRead * 2;
        }

        private void EnsureSourceBuffer(int count)
        {
            if (sourceBuffer == null || sourceBuffer.Length < count)
            {
                sourceBuffer = new float[count];
            }
        }

        #endregion
    }
}