namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Реализация IByteCharConverter по умолчанию
    /// </summary>
    public class DefaultByteCharConverter : IByteCharConverter
    {
        #region Методы

        /// <summary>
        /// Returns the character to display for the byte passed across.
        /// </summary>
        public virtual char ToChar(byte b)
        {
            return b > 0x1F && !(b > 0x7E && b < 0xA0) ? (char)b : '.';
        }

        /// <summary>
        /// Returns the byte to use for the character passed across.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public virtual byte ToByte(char c)
        {
            return (byte)c;
        }

        /// <summary>
        /// Returns a description of the byte char provider.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ANSI (Default)";
        }

        #endregion
    }
}