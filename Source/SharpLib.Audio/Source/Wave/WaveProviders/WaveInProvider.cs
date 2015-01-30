namespace NAudio.Wave
{
    internal class WaveInProvider : IWaveProvider
    {
        #region Поля

        private readonly BufferedWaveProvider bufferedWaveProvider;

        private readonly IWaveIn waveIn;

        #endregion

        #region Свойства

        public WaveFormat WaveFormat
        {
            get { return waveIn.WaveFormat; }
        }

        #endregion

        #region Конструктор

        public WaveInProvider(IWaveIn waveIn)
        {
            this.waveIn = waveIn;
            waveIn.DataAvailable += waveIn_DataAvailable;
            bufferedWaveProvider = new BufferedWaveProvider(WaveFormat);
        }

        #endregion

        #region Методы

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return bufferedWaveProvider.Read(buffer, 0, count);
        }

        #endregion
    }
}