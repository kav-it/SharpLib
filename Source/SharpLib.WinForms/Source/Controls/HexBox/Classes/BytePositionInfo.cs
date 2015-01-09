namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Позиция в HexBox
    /// </summary>
    internal class BytePositionInfo
    {
        #region Поля

        #endregion

        #region Свойства

        /// <summary>
        /// Позиция символа
        /// </summary>
        public int CharacterPosition { get; private set; }

        /// <summary>
        /// Индекс байта в массиве
        /// </summary>
        public long Index { get; private set; }

        #endregion

        #region Конструктор

        public BytePositionInfo(long index, int characterPosition)
        {
            Index = index;
            CharacterPosition = characterPosition;
        }

        #endregion
    }
}