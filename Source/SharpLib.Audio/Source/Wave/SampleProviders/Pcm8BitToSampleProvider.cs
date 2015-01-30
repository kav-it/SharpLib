namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class Pcm8BitToSampleProvider : SampleProviderConverterBase
    {
        #region Конструктор

        public Pcm8BitToSampleProvider(IWaveProvider source) :
            base(source)
        {
        }

        #endregion

        #region Методы

        public override int Read(float[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count;
            EnsureSourceBuffer(sourceBytesRequired);
            int bytesRead = source.Read(sourceBuffer, 0, sourceBytesRequired);
            int outIndex = offset;
            for (int n = 0; n < bytesRead; n++)
            {
                buffer[outIndex++] = sourceBuffer[n] / 128f - 1.0f;
            }
            return bytesRead;
        }

        #endregion
    }
}