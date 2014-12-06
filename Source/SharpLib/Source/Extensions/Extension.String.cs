using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace SharpLib
{
    public static class ExtensionString
    {
        #region Методы

        public static string RegexEx(this string value, string pattern)
        {
            // Пример   : '1234 (567) 890'
            // Pattern  : '4 ({0}) 8'
            // Результат: 567

            string patternConst = "{0}";

            int index = pattern.SearchEx(patternConst);
            if (index == -1)
            {
                return "";
            }

            string textLeft = pattern.Substring(0, index - patternConst.Length);
            string textRight = pattern.Substring(index);

            if (textLeft.IsNotValid() && textRight.IsNotValid())
            {
                return "";
            }

            int indexLeft = -1;
            int indexRight = -1;

            if (textLeft.IsNotValid())
            {
                indexLeft = 0;
            }
            else if (textRight.IsNotValid())
            {
                indexRight = value.Length;
            }

            if (indexLeft == -1)
            {
                indexLeft = value.SearchEx(textLeft);
            }
            if (indexRight == -1)
            {
                indexRight = value.SearchEx(textRight, (indexLeft == -1) ? 0 : indexLeft);
            }

            if (indexLeft == -1 || indexRight == -1)
            {
                return "";
            }
            if (indexLeft >= indexRight)
            {
                return "";
            }

            string result =
                value.Substring(indexLeft, indexRight - indexLeft - textRight.Length);

            return result;
        }

        public static string SubstringEx(this string value, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            string result = value.Substring(startIndex, length);

            return result;
        }

        public static string RemoveEx(this string value, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            string result = value.Remove(startIndex, length);

            return result;
        }

        public static int SearchEx(this string value, string substring, int offset)
        {
            int result = -1;
            int index = value.IndexOf(substring, offset, StringComparison.Ordinal);

            if (index >= 0)
            {
                result = index + substring.Length;
            }

            return result;
        }

        public static int SearchEx(this string value, string substring)
        {
            return SearchEx(value, substring, 0);
        }

        public static string ReplaceEx(this string text, string value, int startIndex, int endIndex)
        {
            if (value.IsNotValid())
            {
                return text;
            }
            if (startIndex >= endIndex)
            {
                return text;
            }
            if (startIndex < 0 || endIndex >= text.Length)
            {
                return text;
            }

            string textBefore = text.Substring(0, startIndex);
            string textAfter = text.Substring(endIndex, text.Length - endIndex);

            string result = textBefore + value + textAfter;

            return result;
        }

        public static int GetIntEx(this string value, int offset)
        {
            int result = 0;

            for (int i = offset; i < value.Length; i++)
            {
                Char ch = value[i];

                if (ch.IsDigit() == false)
                {
                    break;
                }

                result *= 10;
                result += ch.ToDigitEx();
            }

            return result;
        }

        public static int ToIntEx(this string value)
        {
            int result;

            int.TryParse(value, out result);

            return result;
        }

        public static int ToIntFromHexEx(this string value)
        {
            int result;

            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                result = Int32.Parse(value, NumberStyles.HexNumber);
            }
            else
            {
                result = ToIntEx(value);
            }

            return result;
        }

        public static Byte ToByteEx(this string value)
        {
            return (Byte)value.ToUInt32Ex();
        }

        public static UInt16 ToUInt16Ex(this string value)
        {
            return (UInt16)value.ToUInt32Ex();
        }

        public static UInt32 ToUInt32Ex(this string value)
        {
            if (value == null)
            {
                return 0;
            }
            if (value == "")
            {
                return 0;
            }
            UInt32 result = 0;
            try
            {
                result = Convert.ToUInt32(value);
            }
            catch
            {
            }
            return result;
        }

        public static Double ToDoubleEx(this string value)
        {
            if (value == "")
            {
                return 0;
            }

            value = value.Replace(',', '.');

            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        public static Char ToCharEx(this string value)
        {
            if (value.IsValid())
            {
                return value[0];
            }

            return '\0';
        }

        public static IPAddress ToIpEx(this string value)
        {
            value = value.ToLower();

            if (value == "localhost")
            {
                value = "127.0.0.1";
            }

            return IPAddress.Parse(value);
        }

        public static Byte[] ToBufferEx(this string text)
        {
            Byte[] buffer = Encoding.Default.GetBytes(text);

            return buffer;
        }

        public static Byte[] ToBufferUtf8Ex(this string text)
        {
            Byte[] buffer = Encoding.UTF8.GetBytes(text);

            return buffer;
        }

        public static Byte[] ToBufferEx(this string value, int offset, int size)
        {
            if (offset > 0 && (offset + size) < value.Length)
            {
                value = value.Substring(offset, size);

                return ToBufferEx(value);
            }

            return null;
        }

        public static string CleanAscii(this string text)
        {
            Char newCh = '.';
            string res = "";

            foreach (char t in text)
            {
                var ch = t;
                int code = ch;

                if ((code < 0x20) || ((code > 0x7E) && (code < 0xC0)))
                {
                    ch = newCh;
                }

                res = res + ch;
            }

            return res;
        }

        public static string[] SplitEx(this string value, string delimeter)
        {
            string[] result = value.Split(new[] { delimeter }, StringSplitOptions.RemoveEmptyEntries);

            return result;
        }

        public static string[] SplitByEx(this string value, int chunkSize)
        {
            List<String> list = new List<String>();

            if (value.IsValid() && chunkSize > 0)
            {
                int valueLength = value.Length;

                for (int i = 0; i < valueLength; i += chunkSize)
                {
                    if (i + chunkSize > valueLength)
                    {
                        chunkSize = valueLength - i;
                    }

                    string temp = value.Substring(i, chunkSize);

                    list.Add(temp);
                }
            }

            return list.ToArray();
        }

        public static Boolean IsValid(this string value)
        {
            Boolean result = (String.IsNullOrEmpty(value) == false);

            return result;
        }

        public static Boolean IsNotValid(this string value)
        {
            return (value.IsValid() == false);
        }

        public static string Remove(this string text, string subString)
        {
            text = text.Replace(subString, "");

            return text;
        }

        public static string TrimEx(this string text, string subString)
        {
            text = text.TrimStartEx(subString);
            text = text.TrimEndEx(subString);

            return text;
        }

        public static string TrimStartEx(this string text, string subString)
        {
            if (text.StartsWith(subString))
            {
                text = text.Remove(0, subString.Length);
            }

            return text;
        }

        public static string TrimEndEx(this string text, string subString)
        {
            if (text.EndsWith(subString))
            {
                text = text.Remove(text.Length - subString.Length);
            }

            return text;
        }

        public static Byte GetByte8(this string text, int offset)
        {
            if (offset < text.Length)
            {
                Byte[] buffer = text.ToBufferEx(offset, 1);

                if (buffer != null)
                {
                    return buffer[0];
                }
            }

            return 0x00;
        }

        public static UInt16 GetByte16(this string text, int offset, Endianess endian)
        {
            if (offset < text.Length)
            {
                Byte[] buffer = text.ToBufferEx(offset, 2);

                if (buffer != null)
                {
                    UInt16 result = buffer.GetByte16Ex(offset, endian);

                    return result;
                }
            }

            return 0x0000;
        }

        public static UInt32 GetByte32(this string text, int offset, Endianess endian)
        {
            if (offset < text.Length)
            {
                Byte[] buffer = text.ToBufferEx(offset, 4);

                if (buffer != null)
                {
                    UInt32 result = buffer.GetByte32Ex(offset, endian);

                    return result;
                }
            }

            return 0x00000000;
        }

        public static UInt64 GetByte64(this string text, int offset, Endianess endian)
        {
            if (offset < text.Length)
            {
                Byte[] buffer = text.ToBufferEx(offset, 8);

                if (buffer != null)
                {
                    UInt64 result = buffer.GetByte64Ex(offset, endian);

                    return result;
                }
            }

            return 0x0000000000000000;
        }

        public static Stream ToStreamEx(this string text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(text);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        public static DateTime ToDateTimeEx(this string text, string pattern)
        {
            var result = DateTime.ParseExact(text, pattern, CultureInfo.InvariantCulture);

            return result;
        }

        public static Byte ToAsciiByteEx(this string text)
        {
            Byte result = 0x00;

            if (text != null && text.Length > 1)
            {
                Char chA = text[0];
                Char chB = text[1];

                Byte a = chA.ToAsciiByteEx();
                Byte b = chB.ToAsciiByteEx();

                if (Char.IsDigit(chA))
                {
                    a &= 0x0F;
                }
                else if (Char.IsUpper(chA))
                {
                    a -= 0x37;
                }
                else
                {
                    a -= 0x57;
                }

                if (Char.IsDigit(chB))
                {
                    b &= 0x0F;
                }
                else if (Char.IsUpper(chB))
                {
                    b -= 0x37;
                }
                else
                {
                    b -= 0x57;
                }

                result = (Byte)((a << 4) | b);
            }

            return result;
        }

        public static Byte[] ToAsciiBufferEx(this string text)
        {
            if (text != null && text.Length > 1)
            {
                int countIn = text.Length / 2;
                Byte[] result = new Byte[countIn];

                int offset = 0;
                while (offset < countIn)
                {
                    string temp = text.Substring(offset * 2, 2);
                    Byte b = ToAsciiByteEx(temp);

                    result[offset] = b;

                    offset++;
                }

                return result;
            }

            return null;
        }

        public static Byte[] ToAsciiBufferEx(this string text, string delimeter)
        {
            text = text.Replace(delimeter, string.Empty);

            return ToAsciiBufferEx(text);
        }

        public static string TabsToSpacesEx(this string value, int numSpaces)
        {
            string spaces = new string(' ', numSpaces);
            string result = value.Replace("\t", spaces);

            return result;
        }

        public static string ExpandRightEx(this string value, int width, string ch = " ")
        {
            Char c = ch[0];
            string result = value.PadRight(width, c);

            return result;
        }

        public static Boolean ContainsEx(this string source, string text, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            int index = source.IndexOf(text, comp);

            return (index >= 0);
        }

        public static bool EqualEx(this string value1, string value2)
        {
            return value1.Equals(value2, StringComparison.Ordinal);
        }

        #endregion
    }
}