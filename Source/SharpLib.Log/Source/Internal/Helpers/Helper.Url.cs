using System.Text;

namespace SharpLib.Log
{
    internal static class UrlHelper
    {
        #region Поля

        private static readonly string _hexChars = "0123456789abcdef";

        private static readonly string _safeUrlPunctuation = ".()*-_!'";

        #endregion

        #region Методы

        internal static string UrlEncode(string str, bool spaceAsPlus)
        {
            var result = new StringBuilder(str.Length + 20);

            foreach (char ch in str)
            {
                if (ch == ' ' && spaceAsPlus)
                {
                    result.Append('+');
                }
                else if (IsSafeUrlCharacter(ch))
                {
                    result.Append(ch);
                }
                else if (ch < 256)
                {
                    result.Append('%');
                    result.Append(_hexChars[(ch >> 4) & 0xF]);
                    result.Append(_hexChars[(ch >> 0) & 0xF]);
                }
                else
                {
                    result.Append('%');
                    result.Append('u');
                    result.Append(_hexChars[(ch >> 12) & 0xF]);
                    result.Append(_hexChars[(ch >> 8) & 0xF]);
                    result.Append(_hexChars[(ch >> 4) & 0xF]);
                    result.Append(_hexChars[(ch >> 0) & 0xF]);
                }
            }

            return result.ToString();
        }

        private static bool IsSafeUrlCharacter(char ch)
        {
            if (ch >= 'a' && ch <= 'z')
            {
                return true;
            }

            if (ch >= 'A' && ch <= 'Z')
            {
                return true;
            }

            if (ch >= '0' && ch <= '9')
            {
                return true;
            }

            return _safeUrlPunctuation.IndexOf(ch) >= 0;
        }

        #endregion
    }
}