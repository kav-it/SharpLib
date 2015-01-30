using System;

using NAudio.Utils;

namespace NAudio.Wave
{
    internal class WaveFloatTo16Provider : IWaveProvider
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

        public WaveFloatTo16Provider(IWaveProvider sourceProvider)
        {
            if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Input wave provider must be IEEE float", "sourceProvider");
            }
            if (sourceProvider.WaveFormat.BitsPerSample != 32)
            {
                throw new ArgumentException("Input wave provider must be 32 bit", "sourceProvider");
            }

            waveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 16, sourceProvider.WaveFormat.Channels);

            this.sourceProvider = sourceProvider;
            volume = 1.0f;
        }

        #endregion

        #region Методы

        public int Read(byte[] destBuffer, int offset, int numBytes)
        {
            int sourceBytesRequired = numBytes * 2;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);
            int sourceBytesRead = sourceProvider.Read(sourceBuffer, 0, sourceBytesRequired);
            WaveBuffer sourceWaveBuffer = new WaveBuffer(sourceBuffer);
            WaveBuffer destWaveBuffer = new WaveBuffer(destBuffer);

            int sourceSamples = sourceBytesRead / 4;
            int destOffset = offset / 2;
            for (int sample = 0; sample < sourceSamples; sample++)
            {
                float sample32 = sourceWaveBuffer.FloatBuffer[sample] * volume;

                if (sample32 > 1.0f)
                {
                    sample32 = 1.0f;
                }
                if (sample32 < -1.0f)
                {
                    sample32 = -1.0f;
                }
                destWaveBuffer.ShortBuffer[destOffset++] = (short)(sample32 * 32767);
            }

            return sourceSamples * 2;
        }

        #endregion
    }
}