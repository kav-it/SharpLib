using System;

using SharpLib.Audio.Utils;

namespace SharpLib.Audio.Wave
{
    internal class MonoToStereoProvider16 : IWaveProvider
    {
        #region Поля

        private readonly WaveFormat outputFormat;

        private readonly IWaveProvider sourceProvider;

        private byte[] sourceBuffer;

        #endregion

        #region Свойства

        public float LeftVolume { get; set; }

        public float RightVolume { get; set; }

        public WaveFormat WaveFormat
        {
            get { return outputFormat; }
        }

        #endregion

        #region Конструктор

        public MonoToStereoProvider16(IWaveProvider sourceProvider)
        {
            if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
            {
                throw new ArgumentException("Source must be PCM");
            }
            if (sourceProvider.WaveFormat.Channels != 1)
            {
                throw new ArgumentException("Source must be Mono");
            }
            if (sourceProvider.WaveFormat.BitsPerSample != 16)
            {
                throw new ArgumentException("Source must be 16 bit");
            }
            this.sourceProvider = sourceProvider;
            outputFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 2);
            RightVolume = 1.0f;
            LeftVolume = 1.0f;
        }

        #endregion

        #region Методы

        public int Read(byte[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count / 2;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);
            WaveBuffer sourceWaveBuffer = new WaveBuffer(sourceBuffer);
            WaveBuffer destWaveBuffer = new WaveBuffer(buffer);

            int sourceBytesRead = sourceProvider.Read(sourceBuffer, 0, sourceBytesRequired);
            int samplesRead = sourceBytesRead / 2;
            int destOffset = offset / 2;
            for (int sample = 0; sample < samplesRead; sample++)
            {
                short sampleVal = sourceWaveBuffer.ShortBuffer[sample];
                destWaveBuffer.ShortBuffer[destOffset++] = (short)(LeftVolume * sampleVal);
                destWaveBuffer.ShortBuffer[destOffset++] = (short)(RightVolume * sampleVal);
            }
            return samplesRead * 4;
        }

        #endregion
    }
}