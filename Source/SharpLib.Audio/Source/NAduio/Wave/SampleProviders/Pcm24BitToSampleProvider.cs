namespace SharpLib.Audio.Wave.SampleProviders
{
    internal class Pcm24BitToSampleProvider : SampleProviderConverterBase
    {
        #region Конструктор

        public Pcm24BitToSampleProvider(IWaveProvider source)
            : base(source)
        {
        }

        #endregion

        #region Методы

        public override int Read(float[] buffer, int offset, int count)
        {
            int sourceBytesRequired = count * 3;
            EnsureSourceBuffer(sourceBytesRequired);
            int bytesRead = source.Read(sourceBuffer, 0, sourceBytesRequired);
            int outIndex = offset;
            for (int n = 0; n < bytesRead; n += 3)
            {
                buffer[outIndex++] = (((sbyte)sourceBuffer[n + 2] << 16) | (sourceBuffer[n + 1] << 8) | sourceBuffer[n]) / 8388608f;
            }
            return bytesRead / 3;
        }

        #endregion
    }
}