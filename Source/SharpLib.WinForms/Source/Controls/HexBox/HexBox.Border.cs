namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Вспомогательный класс работы с Border элемента
    /// </summary>
    internal class HexBoxBorder
    {
        #region Свойства

        internal int Left { get; set; }

        internal int Top { get; set; }

        internal int Right { get; set; }

        internal int Bottom { get; set; }

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        internal HexBoxBorder(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Установка всех значений
        /// </summary>
        internal void SetValues(int value)
        {
            Left = Top = Right = Bottom = value;
        }
    }
}