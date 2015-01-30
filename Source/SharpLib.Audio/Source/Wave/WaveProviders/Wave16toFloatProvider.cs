using System;

using NAudio.Utils;

namespace NAudio.Wave
{
    internal class Wave16ToFloatProvider : IWaveProvider
    {
        #region Поля

        private readonly IWaveProvider sourceProvider;

        private readonly WaveFormat waveFormat;

        private byte[] sourceBuffer;

        private volatile float volume;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        #endregion

        #region Конструктор

        public Wave16ToFloatProvider(IWaveProvider sourceProvider)
        {
            if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
            {
                throw new ArgumentException("Only PCM supported");
            }
            if (sourceProvider.WaveFormat.BitsPerSample != 16)
            {
                throw new ArgumentException("Only 16 bit audio supported");
            }

            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sourceProvider.WaveFormat.SampleRate, sourceProvider.WaveFormat.Channels);

            this.sourceProvider = sourceProvider;
            volume = 1.0f;
        }

        #endregion

        #region Методы

        public int Read(byte[] destBuffer, int offset, int numBytes)
        {
            int sourceBytesRequired = numBytes / 2;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);
            int sourceBytesRead = sourceProvider.Read(sourceBuffer, offset, sourceBytesRequired);
            WaveBuffer sourceWaveBuffer = new WaveBuffer(sourceBuffer);
            WaveBuffer destWaveBuffer = new WaveBuffer(destBuffer);

            int sourceSamples = sourceBytesRead / 2;
            int destOffset = offset / 4;
            for (int sample = 0; sample < sourceSamples; sample++)
            {
                destWaveBuffer.FloatBuffer[destOffset++] = (sourceWaveBuffer.ShortBuffer[sample] / 32768f) * volume;
            }

            return sourceSamples * 4;
        }

        #endregion
    }
}