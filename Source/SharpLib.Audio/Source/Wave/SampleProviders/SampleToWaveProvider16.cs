using System;

using SharpLib.Audio.Utils;

namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class SampleToWaveProvider16 : IWaveProvider
    {
        #region Поля

        private readonly ISampleProvider sourceProvider;

        private readonly WaveFormat waveFormat;

        private float[] sourceBuffer;

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

        public SampleToWaveProvider16(ISampleProvider sourceProvider)
        {
            if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Input source provider must be IEEE float", "sourceProvider");
            }
            if (sourceProvider.WaveFormat.BitsPerSample != 32)
            {
                throw new ArgumentException("Input source provider must be 32 bit", "sourceProvider");
            }

            waveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 16, sourceProvider.WaveFormat.Channels);

            this.sourceProvider = sourceProvider;
            volume = 1.0f;
        }

        #endregion

        #region Методы

        public int Read(byte[] destBuffer, int offset, int numBytes)
        {
            int samplesRequired = numBytes / 2;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, samplesRequired);
            int sourceSamples = sourceProvider.Read(sourceBuffer, 0, samplesRequired);
            WaveBuffer destWaveBuffer = new WaveBuffer(destBuffer);

            int destOffset = offset / 2;
            for (int sample = 0; sample < sourceSamples; sample++)
            {
                float sample32 = sourceBuffer[sample] * volume;

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