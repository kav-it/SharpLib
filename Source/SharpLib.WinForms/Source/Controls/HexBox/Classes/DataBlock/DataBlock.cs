namespace SharpLib.WinForms.Controls
{
    internal abstract class DataBlock
    {
        #region Поля

        internal DataMap _map;

        internal DataBlock _nextBlock;

        internal DataBlock _previousBlock;

        #endregion

        #region Свойства

        public abstract long Length { get; }

        public DataMap Map
        {
            get { return _map; }
        }

        public DataBlock NextBlock
        {
            get { return _nextBlock; }
        }

        public DataBlock PreviousBlock
        {
            get { return _previousBlock; }
        }

        #endregion

        #region Методы

        public abstract void RemoveBytes(long position, long count);

        #endregion
    }
}