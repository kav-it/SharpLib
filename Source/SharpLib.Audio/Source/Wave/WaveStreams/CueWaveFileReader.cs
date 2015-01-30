namespace NAudio.Wave
{
    internal class CueWaveFileReader : WaveFileReader
    {
        #region Поля

        private CueList cues;

        #endregion

        #region Свойства

        public CueList Cues
        {
            get
            {
                if (cues == null)
                {
                    cues = CueList.FromChunks(this);
                }
                return cues;
            }
        }

        #endregion

        #region Конструктор

        public CueWaveFileReader(string fileName)
            : base(fileName)
        {
        }

        #endregion
    }
}