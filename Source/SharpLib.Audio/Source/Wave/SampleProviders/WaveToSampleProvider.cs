using System;

namespace NAudio.Wave.SampleProviders
{
    internal class WaveToSampleProvider : SampleProviderConverterBase
    {
        #region Конструктор

        public WaveToSampleProvider(IWaveProvider source)
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
            int bytesNeeded = count * 4;
            EnsureSourceBuffer(bytesNeeded);
            int bytesRead = source.Read(sourceBuffer, 0, bytesNeeded);
            int samplesRead = bytesRead / 4;
            int outputIndex = offset;
            for (int n = 0; n < bytesRead; n += 4)
            {
                buffer[outputIndex++] = BitConverter.ToSingle(sourceBuffer, n);
            }
            return samplesRead;
        }

        #endregion
    }
}