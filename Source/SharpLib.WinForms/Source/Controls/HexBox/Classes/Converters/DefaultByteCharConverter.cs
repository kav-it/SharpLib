using System.Text;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Реализация IByteCharConverter по умолчанию
    /// </summary>
    public class DefaultByteCharConverter : IByteCharConverter
    {
        #region Методы

        /// <summary>
        /// Преобразование byte - string 
        /// </summary>
        public virtual string ToChar(byte value)
        {
            return ToText(new[] { value });
        }

        /// <summary>
        /// Преобразование byte[] - string
        /// </summary>
        public string ToText(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];

                if ((b < 0x20) || ((b > 0x7E) && (b < 0xC0)))
                {
                    // Символ '.'
                    bytes[i] = 0x2E;
                }
            }

            var result = ExtensionEncoding.Windows1251.GetString(bytes);

            return result;
        }

        /// <summary>
        /// Преобразование string - byte[]
        /// </summary>
        public byte[] ToBuffer(string text)
        {
            return ExtensionEncoding.Windows1251.GetBytes(text);
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