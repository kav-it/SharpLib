namespace SharpLib.Log
{
    internal class SimpleStringReader
    {
        #region Поля

        private readonly string text;

        #endregion

        #region Свойства

        internal int Position { get; set; }

        internal string Text
        {
            get { return text; }
        }

        #endregion

        #region Конструктор

        public SimpleStringReader(string text)
        {
            this.text = text;
            Position = 0;
        }

        #endregion

        #region Методы

        internal int Peek()
        {
            if (Position < text.Length)
            {
                return text[Position];
            }

            return -1;
        }

        internal int Read()
        {
            if (Position < text.Length)
            {
                return text[Position++];
            }

            return -1;
        }

        internal string Substring(int p0, int p1)
        {
            return text.Substring(p0, p1 - p0);
        }

        #endregion
    }
}