using System;
using System.Text;

namespace SharpLib
{
    /// <summary>
    /// Методы расширения для Byte
    /// </summary>
    public static class ExtensionByte
    {
        /// <summary>
        /// Преобразование к char
        /// </summary>
        public static Char ToCharEx(this byte self)
        {
            var buffer = new[]{ self };

            var enc = new ASCIIEncoding();
            var text = enc.GetString(buffer);

            return text[0];
        }
    }
}