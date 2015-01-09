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
        string ToChar(byte b);

        /// <summary>
        /// Отображение массива в виде текста
        /// </summary>
        string ToText(byte[] bytes);

        /// <summary>
        /// Преобразование string - byte[]
        /// </summary>
        byte[] ToBuffer(string text);

        /// <summary>
        /// Байтовое значение символа
        /// </summary>
        byte ToByte(char c);

        #endregion
    }
}