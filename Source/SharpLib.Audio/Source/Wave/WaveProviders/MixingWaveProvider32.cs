using System;
using System.Collections.Generic;

namespace SharpLib.Audio.Wave
{
    internal class MixingWaveProvider32 : IWaveProvider
    {
        #region Поля

        private readonly int bytesPerSample;

        private readonly List<IWaveProvider> inputs;

        private WaveFormat waveFormat;

        #endregion

        #region Свойства

        public int InputCount
        {
            get { return inputs.Count; }
        }

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        #endregion

        #region Конструктор

        public MixingWaveProvider32()
        {
            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            bytesPerSample = 4;
            inputs = new List<IWaveProvider>();
        }

        public MixingWaveProvider32(IEnumerable<IWaveProvider> inputs)
            : this()
        {
            foreach (var input in inputs)
            {
                AddInputStream(input);
            }
        }

        #endregion

        #region Методы

        public void AddInputStream(IWaveProvider waveProvider)
        {
            if (waveProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Must be IEEE floating point", "waveProvider.WaveFormat");
            }
            if (waveProvider.WaveFormat.BitsPerSample != 32)
            {
                throw new ArgumentException("Only 32 bit audio currently supported", "waveProvider.WaveFormat");
            }

            if (inputs.Count == 0)
            {
                int sampleRate = waveProvider.WaveFormat.SampleRate;
                int channels = waveProvider.WaveFormat.Channels;
                waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            }
            else
            {
                if (!waveProvider.WaveFormat.Equals(waveFormat))
                {
                    throw new ArgumentException("All incoming channels must have the same format", "waveProvider.WaveFormat");
                }
            }

            lock (inputs)
            {
                inputs.Add(waveProvider);
            }
        }

        public void RemoveInputStream(IWaveProvider waveProvider)
        {
            lock (inputs)
            {
                inputs.Remove(waveProvider);
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (count % bytesPerSample != 0)
            {
                throw new ArgumentException("Must read an whole number of samples", "count");
            }

            Array.Clear(buffer, offset, count);
            int bytesRead = 0;

            byte[] readBuffer = new byte[count];
            lock (inputs)
            {
                foreach (var input in inputs)
                {
                    int readFromThisStream = input.Read(readBuffer, 0, count);

                    bytesRead = Math.Max(bytesRead, readFromThisStream);
                    if (readFromThisStream > 0)
                    {
                        Sum32BitAudio(buffer, offset, readBuffer, readFromThisStream);
                    }
                }
            }
            return bytesRead;
        }

        private static unsafe void Sum32BitAudio(byte[] destBuffer, int offset, byte[] sourceBuffer, int bytesRead)
        {
            fixed (byte* pDestBuffer = &destBuffer[offset],
                pSourceBuffer = &sourceBuffer[0])
            {
                float* pfDestBuffer = (float*)pDestBuffer;
                float* pfReadBuffer = (float*)pSourceBuffer;
                int samplesRead = bytesRead / 4;
                for (int n = 0; n < samplesRead; n++)
                {
                    pfDestBuffer[n] += pfReadBuffer[n];
                }
            }
        }

        #endregion
    }
}