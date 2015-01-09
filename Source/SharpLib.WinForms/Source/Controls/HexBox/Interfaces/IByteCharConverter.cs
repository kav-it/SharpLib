namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Интерфейс преобразование [byte]-[char], [char]-[byte]
    /// </summary>
    public interface IByteCharConverter
    {
        #region Методы

        /// <summary>
        /// Отображение символа для байта
        /// </summary>
        char ToChar(byte b);

        /// <summary>
        /// Байтовое значение символа
        /// </summary>
        byte ToByte(char c);

        #endregion
    }
}