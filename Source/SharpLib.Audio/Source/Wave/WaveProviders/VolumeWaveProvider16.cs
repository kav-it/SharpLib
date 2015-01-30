using System;

namespace NAudio.Wave
{
    internal class VolumeWaveProvider16 : IWaveProvider
    {
        #region Поля

        private readonly IWaveProvider sourceProvider;

        private float volume;

        #endregion

        #region Свойства

        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public WaveFormat WaveFormat
        {
            get { return sourceProvider.WaveFormat; }
        }

        #endregion

        #region Конструктор

        public VolumeWaveProvider16(IWaveProvider sourceProvider)
        {
            Volume = 1.0f;
            this.sourceProvider = sourceProvider;
            if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
            {
                throw new ArgumentException("Expecting PCM input");
            }
            if (sourceProvider.WaveFormat.BitsPerSample != 16)
            {
                throw new ArgumentException("Expecting 16 bit");
            }
        }

        #endregion

        #region Методы

        public int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = sourceProvider.Read(buffer, offset, count);
            if (volume == 0.0f)
            {
                for (int n = 0; n < bytesRead; n++)
                {
                    buffer[offset++] = 0;
                }
            }
            else if (volume != 1.0f)
            {
                for (int n = 0; n < bytesRead; n += 2)
                {
                    short sample = (short)((buffer[offset + 1] << 8) | buffer[offset]);
                    var newSample = sample * volume;
                    sample = (short)newSample;

                    if (Volume > 1.0f)
                    {
                        if (newSample > Int16.MaxValue)
                        {
                            sample = Int16.MaxValue;
                        }
                        else if (newSample < Int16.MinValue)
                        {
                            sample = Int16.MinValue;
                        }
                    }

                    buffer[offset++] = (byte)(sample & 0xFF);
                    buffer[offset++] = (byte)(sample >> 8);
                }
            }
            return bytesRead;
        }

        #endregion
    }
}