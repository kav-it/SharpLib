namespace Id3Lib.Frames
{
    internal class FrameUnknown : FrameBase
    {
        #region Поля

        private byte[] _data;

        #endregion

        #region Конструктор

        internal FrameUnknown(string frameId)
            : base(frameId)
        {
        }

        #endregion

        #region Методы

        public override void Parse(byte[] frame)
        {
            _data = frame;
        }

        public override byte[] Make()
        {
            return _data;
        }

        public override string ToString()
        {
            return "Unknown ID3 frameId";
        }

        #endregion
    }
}