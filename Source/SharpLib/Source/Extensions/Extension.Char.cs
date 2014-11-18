using System.Globalization;
using System.Text;

namespace SharpLib
{
    public static class ExtensionChar
    {
        #region Методы

        public static int ToIntEx(this char value)
        {
            int result = (int)char.GetNumericValue(value);

            return result;
        }

        public static int ToDigitEx(this char value)
        {
            int result = value.ToIntEx() & 0x0F;

            return result;
        }

        public static bool IsDigit(this char value)
        {
            bool result = char.IsDigit(value);

            return result;
        }

        public static bool IsWhiteSpace(this char value)
        {
            bool result = char.IsWhiteSpace(value);

            return result;
        }

        public static bool IsLetter(this char value)
        {
            bool result = char.IsLetter(value);

            return result;
        }

        public static bool IsSymbol(this char value)
        {
            string pattern = @"`-=\~!@#$%^&*()_+|[];',./{}:""<>?";

            int index = pattern.IndexOf(value);
            bool result = (index != -1);

            return result;
        }

        public static bool IsNull(this char value)
        {
            bool result = (value == 0x0000);

            return result;
        }

        public static byte ToAsciiByteEx(this char ch)
        {
            var encoding = new ASCIIEncoding();

            var text = ch.ToString(CultureInfo.InvariantCulture);

            var value = encoding.GetBytes(text);

            return value[0];
        }

        #endregion
    }
}