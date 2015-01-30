namespace SharpLib.Audio.Wave
{
    internal interface ISampleProvider
    {
        #region Свойства

        WaveFormat WaveFormat { get; }

        #endregion

        #region Методы

        int Read(float[] buffer, int offset, int count);

        #endregion
    }
}