using System;
using System.Text;

namespace NAudio.Utils
{
    internal static class ByteArrayExtensions
    {
        #region Методы

        public static bool IsEntirelyNull(byte[] buffer)
        {
            foreach (byte b in buffer)
            {
                if (b != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static string DescribeAsHex(byte[] buffer, string separator, int bytesPerLine)
        {
            StringBuilder sb = new StringBuilder();
            int n = 0;
            foreach (byte b in buffer)
            {
                sb.AppendFormat("{0:X2}{1}", b, separator);
                if (++n % bytesPerLine == 0)
                {
                    sb.Append("\r\n");
                }
            }
            sb.Append("\r\n");
            return sb.ToString();
        }

        public static string DecodeAsString(byte[] buffer, int offset, int length, Encoding encoding)
        {
            for (int n = 0; n < length; n++)
            {
                if (buffer[offset + n] == 0)
                {
                    length = n;
                }
            }
            return encoding.GetString(buffer, offset, length);
        }

        public static byte[] Concat(params byte[][] byteArrays)
        {
            int size = 0;
            foreach (byte[] btArray in byteArrays)
            {
                size += btArray.Length;
            }

            if (size <= 0)
            {
                return new byte[0];
            }

            byte[] result = new byte[size];
            int idx = 0;
            foreach (byte[] btArray in byteArrays)
            {
                Array.Copy(btArray, 0, result, idx, btArray.Length);
                idx += btArray.Length;
            }

            return result;
        }

        #endregion
    }
}