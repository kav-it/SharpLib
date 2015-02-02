namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class Pcm32BitToSampleProvider : SampleProviderConverterBase
    {
        #region Конструктор

        public Pcm32BitToSampleProvider(IWaveProvider source)
            : base(source)
        {
        }

        #endregion

        #region Методы

        public override int Read(float[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count * 4;
            EnsureSourceBuffer(sourceBytesRequired);
            int bytesRead = source.Read(sourceBuffer, 0, sourceBytesRequired);
            int outIndex = offset;
            for (int n = 0; n < bytesRead; n += 4)
            {
                buffer[outIndex++] = (((sbyte)sourceBuffer[n + 3] << 24 |
                                       sourceBuffer[n + 2] << 16) |
                                      (sourceBuffer[n + 1] << 8) |
                                      sourceBuffer[n]) / 2147483648f;
            }
            return bytesRead / 4;
        }

        #endregion
    }
}