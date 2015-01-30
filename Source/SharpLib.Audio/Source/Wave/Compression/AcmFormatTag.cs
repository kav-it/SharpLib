namespace SharpLib.Audio.Wave.Compression
{
    internal class AcmFormatTag
    {
        #region Поля

        private AcmFormatTagDetails formatTagDetails;

        #endregion

        #region Свойства

        public int FormatTagIndex
        {
            get { return formatTagDetails.formatTagIndex; }
        }

        public WaveFormatEncoding FormatTag
        {
            get { return (WaveFormatEncoding)formatTagDetails.formatTag; }
        }

        public int FormatSize
        {
            get { return formatTagDetails.formatSize; }
        }

        public AcmDriverDetailsSupportFlags SupportFlags
        {
            get { return formatTagDetails.supportFlags; }
        }

        public int StandardFormatsCount
        {
            get { return formatTagDetails.standardFormatsCount; }
        }

        public string FormatDescription
        {
            get { return formatTagDetails.formatDescription; }
        }

        #endregion

        #region Конструктор

        internal AcmFormatTag(AcmFormatTagDetails formatTagDetails)
        {
            this.formatTagDetails = formatTagDetails;
        }

        #endregion
    }
}