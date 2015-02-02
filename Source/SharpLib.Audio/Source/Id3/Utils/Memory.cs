using System;

namespace Id3Lib
{
    internal static class Memory
    {
        #region Ìועמהû

        public static bool Compare(byte[] b1, byte[] b2)
        {
            if (b1 == null)
            {
                throw new ArgumentNullException("b1");
            }
            if (b2 == null)
            {
                throw new ArgumentNullException("b2");
            }

            if (b1.Length != b2.Length)
            {
                return false;
            }
            for (int n = 0; n < b1.Length; n++)
            {
                if (b1[n] != b2[n])
                {
                    return false;
                }
            }
            return true;
        }

        public static byte[] Extract(byte[] src, int srcIndex, int count)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (src == null || srcIndex < 0 || count < 0)
            {
                throw new InvalidOperationException();
            }

            if (src.Length - srcIndex < count)
            {
                throw new InvalidOperationException();
            }

            byte[] dst = new byte[count];
            Array.Copy(src, srcIndex, dst, 0, count);
            return dst;
        }

        public static int FindByte(byte[] src, byte val, int index)
        {
            int size = src.Length;

            if (index > size)
            {
                throw new InvalidOperationException();
            }

            for (int n = index; n < size; n++)
            {
                if (src[n] == val)
                {
                    return n - index;
                }
            }
            return -1;
        }

        public static int FindShort(byte[] src, short val, int index)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            int size = src.Length;
            if (index > size)
            {
                throw new InvalidOperationException();
            }

            for (int n = index; n < size; n += 2)
            {
                if (BitConverter.ToInt16(src, n) == val)
                {
                    return n - index;
                }
            }
            return -1;
        }

        public static void Clear(byte[] dst, int begin, int end)
        {
            if (dst == null)
            {
                throw new ArgumentNullException("dst");
            }
            if (begin > end || begin > dst.Length || end > dst.Length)
            {
                throw new InvalidOperationException();
            }

            Array.Clear(dst, begin, end - begin);
        }

        public static ulong ToInt64(byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Length > 8)
            {
                throw new InvalidOperationException("The count is to large to be stored");
            }

            return BitConverter.ToUInt64(value, 0);
        }

        public static byte[] GetBytes(ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        #endregion
    }
}