using System;

namespace NAudio.Wave.SampleProviders
{
    internal class Pcm16BitToSampleProvider : SampleProviderConverterBase
    {
        #region Конструктор

        public Pcm16BitToSampleProvider(IWaveProvider source)
            : base(source)
        {
        }

        #endregion

        #region Методы

        public override int Read(float[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count * 2;
            EnsureSourceBuffer(sourceBytesRequired);
            int bytesRead = source.Read(sourceBuffer, 0, sourceBytesRequired);
            int outIndex = offset;
            for (int n = 0; n < bytesRead; n += 2)
            {
                buffer[outIndex++] = BitConverter.ToInt16(sourceBuffer, n) / 32768f;
            }
            return bytesRead / 2;
        }

        #endregion
    }
}