using System;

using SharpLib.Audio.Wave.Asio;

namespace SharpLib.Audio.Wave
{
    internal class AsioAudioAvailableEventArgs : EventArgs
    {
        #region Свойства

        public IntPtr[] InputBuffers { get; private set; }

        public IntPtr[] OutputBuffers { get; private set; }

        public bool WrittenToOutputBuffers { get; set; }

        public int SamplesPerBuffer { get; private set; }

        public AsioSampleType AsioSampleType { get; private set; }

        #endregion

        #region Конструктор

        public AsioAudioAvailableEventArgs(IntPtr[] inputBuffers, IntPtr[] outputBuffers, int samplesPerBuffer, AsioSampleType asioSampleType)
        {
            InputBuffers = inputBuffers;
            OutputBuffers = outputBuffers;
            SamplesPerBuffer = samplesPerBuffer;
            AsioSampleType = asioSampleType;
        }

        #endregion

        #region Методы

        public int GetAsInterleavedSamples(float[] samples)
        {
            int channels = InputBuffers.Length;
            if (samples.Length < SamplesPerBuffer * channels)
            {
                throw new ArgumentException("Buffer not big enough");
            }
            int index = 0;
            unsafe
            {
                if (AsioSampleType == AsioSampleType.Int32LSB)
                {
                    for (int n = 0; n < SamplesPerBuffer; n++)
                    {
                        for (int ch = 0; ch < channels; ch++)
                        {
                            samples[index++] = *((int*)InputBuffers[ch] + n) / (float)Int32.MaxValue;
                        }
                    }
                }
                else if (AsioSampleType == AsioSampleType.Int16LSB)
                {
                    for (int n = 0; n < SamplesPerBuffer; n++)
                    {
                        for (int ch = 0; ch < channels; ch++)
                        {
                            samples[index++] = *((short*)InputBuffers[ch] + n) / (float)Int16.MaxValue;
                        }
                    }
                }
                else if (AsioSampleType == AsioSampleType.Int24LSB)
                {
                    for (int n = 0; n < SamplesPerBuffer; n++)
                    {
                        for (int ch = 0; ch < channels; ch++)
                        {
                            byte* pSample = ((byte*)InputBuffers[ch] + n * 3);

                            int sample = pSample[0] | (pSample[1] << 8) | ((sbyte)pSample[2] << 16);
                            samples[index++] = sample / 8388608.0f;
                        }
                    }
                }
                else if (AsioSampleType == AsioSampleType.Float32LSB)
                {
                    for (int n = 0; n < SamplesPerBuffer; n++)
                    {
                        for (int ch = 0; ch < channels; ch++)
                        {
                            samples[index++] = *((float*)InputBuffers[ch] + n);
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException(String.Format("ASIO Sample Type {0} not supported", AsioSampleType));
                }
            }
            return SamplesPerBuffer * channels;
        }

        [Obsolete("Better performance if you use the overload that takes an array, and reuse the same one")]
        public float[] GetAsInterleavedSamples()
        {
            int channels = InputBuffers.Length;
            var samples = new float[SamplesPerBuffer * channels];
            GetAsInterleavedSamples(samples);
            return samples;
        }

        #endregion
    }
}