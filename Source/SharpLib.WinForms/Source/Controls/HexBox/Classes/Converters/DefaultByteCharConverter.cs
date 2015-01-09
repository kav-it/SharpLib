namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Реализация IByteCharConverter по умолчанию
    /// </summary>
    public class DefaultByteCharConverter : IByteCharConverter
    {
        #region Методы

        /// <summary>
        /// Преобразование byte - char 
        /// </summary>
        public virtual char ToChar(byte value)
        {
            // Диапазон отображаемых значений
            // [0x20..0x7E] && [0xC0..0xFF] - char
            // иное - '.'
            if ((value >= 0x20 && value <= 0x7E) || (value >= 0xC0))
            {
                return (char)value;
            }
            
            return '.';
        }

        /// <summary>
        /// Преобразование char - byte
        /// </summary>
        public virtual byte ToByte(char c)
        {
            return (byte)c;
        }

        /// <summary>
        /// Текстовое представление конвертора
        /// </summary>
        public override string ToString()
        {
            return "Windows-1251 (Default)";
        }

        #endregion
    }
}