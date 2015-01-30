namespace NAudio.Wave.Compression
{
    internal class AcmFormat
    {
        #region Поля

        private readonly AcmFormatDetails formatDetails;

        private readonly WaveFormat waveFormat;

        #endregion

        #region Свойства

        public int FormatIndex
        {
            get { return formatDetails.formatIndex; }
        }

        public WaveFormatEncoding FormatTag
        {
            get { return (WaveFormatEncoding)formatDetails.formatTag; }
        }

        public AcmDriverDetailsSupportFlags SupportFlags
        {
            get { return formatDetails.supportFlags; }
        }

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public int WaveFormatByteSize
        {
            get { return formatDetails.waveFormatByteSize; }
        }

        public string FormatDescription
        {
            get { return formatDetails.formatDescription; }
        }

        #endregion

        #region Конструктор

        internal AcmFormat(AcmFormatDetails formatDetails)
        {
            this.formatDetails = formatDetails;
            waveFormat = WaveFormat.MarshalFromPtr(formatDetails.waveFormatPointer);
        }

        #endregion
    }
}