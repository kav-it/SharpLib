namespace SharpLib.Json
{
    internal struct StringReference
    {
        #region Поля

        private readonly char[] _chars;

        private readonly int _length;

        private readonly int _startIndex;

        #endregion

        #region Свойства

        public char[] Chars
        {
            get { return _chars; }
        }

        public int StartIndex
        {
            get { return _startIndex; }
        }

        public int Length
        {
            get { return _length; }
        }

        #endregion

        #region Конструктор

        public StringReference(char[] chars, int startIndex, int length)
        {
            _chars = chars;
            _startIndex = startIndex;
            _length = length;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return new string(_chars, _startIndex, _length);
        }

        #endregion
    }
}