namespace NAudio.Wave
{
    internal interface IWaveProvider
    {
        #region Свойства

        WaveFormat WaveFormat { get; }

        #endregion

        #region Методы

        int Read(byte[] buffer, int offset, int count);

        #endregion
    }
}