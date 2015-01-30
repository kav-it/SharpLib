using System;

namespace NAudio.Wave.SampleProviders
{
    internal class WaveToSampleProvider64 : SampleProviderConverterBase
    {
        #region Конструктор

        public WaveToSampleProvider64(IWaveProvider source)
            : base(source)
        {
            if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
            {
                throw new ArgumentException("Must be already floating point");
            }
        }

        #endregion

        #region Методы

        public override int Read(float[] buffer, int offset, int count)
        {
            int bytesNeeded = count * 8;
            EnsureSourceBuffer(bytesNeeded);
            int bytesRead = source.Read(sourceBuffer, 0, bytesNeeded);
            int samplesRead = bytesRead / 8;
            int outputIndex = offset;
            for (int n = 0; n < bytesRead; n += 8)
            {
                long sample64 = BitConverter.ToInt64(sourceBuffer, n);
                buffer[outputIndex++] = (float)BitConverter.Int64BitsToDouble(sample64);
            }
            return samplesRead;
        }

        #endregion
    }
}