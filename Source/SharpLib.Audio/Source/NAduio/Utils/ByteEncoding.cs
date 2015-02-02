using System.Text;

namespace SharpLib.Audio.Utils
{
    internal class ByteEncoding : Encoding
    {
        #region Поля

        public static readonly ByteEncoding Instance = new ByteEncoding();

        #endregion

        #region Конструктор

        private ByteEncoding()
        {
        }

        #endregion

        #region Методы

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int n = 0; n < charCount; n++)
            {
                bytes[byteIndex + n] = (byte)chars[charIndex + n];
            }
            return charCount;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            for (int n = 0; n < count; n++)
            {
                if (bytes[index + n] == 0)
                {
                    return n;
                }
            }
            return count;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int n = 0; n < byteCount; n++)
            {
                var b = bytes[byteIndex + n];
                if (b == 0)
                {
                    return n;
                }
                chars[charIndex + n] = (char)b;
            }
            return byteCount;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        #endregion
    }
}