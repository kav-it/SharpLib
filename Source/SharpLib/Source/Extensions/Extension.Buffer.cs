using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpLib
{
    public static class ExtensionBuffer
    {
        #region Методы

        public static string ToStringEx(this byte[] buffer, string fill = null)
        {
            var text = Encoding.Default.GetString(buffer);
            var textLocal = string.Empty;

            foreach (char t in text)
            {
                if (Char.IsControl(t))
                {
                    if (fill == null)
                    {
                        break;
                    }
                    textLocal += fill;
                }
                else
                {
                    textLocal += t;
                }
            }
            return textLocal;
        }

        public static MemoryStream ToMemoryStreamEx(this byte[] buffer)
        {
            MemoryStream stream = new MemoryStream(buffer.Length);

            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static string ToAsciiEx(this byte[] buffer, string delimeter = " ")
        {
            string value = BitConverter.ToString(buffer);
            value = value.Replace("-", delimeter);

            return value;
        }

        public static List<Byte[]> SplitByEx(this byte[] value, int chunkSize)
        {
            List<Byte[]> list = new List<Byte[]>();

            if (value != null && chunkSize > 0)
            {
                int valueLength = value.Length;

                for (int i = 0; i < valueLength; i += chunkSize)
                {
                    if (i + chunkSize > valueLength)
                    {
                        chunkSize = valueLength - i;
                    }

                    byte[] temp = Mem.Clone(value, i, chunkSize);

                    list.Add(temp);
                }
            }

            return list;
        }

        public static byte GetByte8Ex(this byte[] buf, int offset)
        {
            byte value = buf[offset];

            return value;
        }

        public static UInt16 GetByte16Ex(this byte[] buf, int offset, Endianess endian)
        {
            UInt16 value;

            if (endian == Endianess.Little)
            {
                value = (UInt16)
                    (
                        (buf[offset + 0] << 0) +
                        (buf[offset + 1] << 8)
                        );
            }
            else
            {
                value = (UInt16)
                    (
                        (buf[offset + 0] << 8) +
                        (buf[offset + 1] << 0)
                        );
            }

            return value;
        }

        public static UInt32 GetByte32Ex(this byte[] buf, int offset, Endianess endian)
        {
            UInt32 value;

            if (endian == Endianess.Little)
            {
                value = (
                    ((UInt32)buf[offset + 0] << 0) +
                    ((UInt32)buf[offset + 1] << 8) +
                    ((UInt32)buf[offset + 2] << 16) +
                    ((UInt32)buf[offset + 3] << 24)
                    );
            }
            else
            {
                value = (
                    ((UInt32)buf[offset + 0] << 24) +
                    ((UInt32)buf[offset + 1] << 16) +
                    ((UInt32)buf[offset + 2] << 8) +
                    ((UInt32)buf[offset + 3] << 0)
                    );
            }
            return value;
        }

        public static UInt64 GetByte64Ex(this byte[] buf, int offset, Endianess endian)
        {
            UInt64 value;

            if (endian == Endianess.Little)
            {
                value = (
                    ((UInt64)buf[offset + 0] << 0) +
                    ((UInt64)buf[offset + 1] << 8) +
                    ((UInt64)buf[offset + 2] << 16) +
                    ((UInt64)buf[offset + 3] << 24) +
                    ((UInt64)buf[offset + 4] << 32) +
                    ((UInt64)buf[offset + 5] << 40) +
                    ((UInt64)buf[offset + 6] << 48) +
                    ((UInt64)buf[offset + 7] << 56)
                    );
            }
            else
            {
                value = (
                    ((UInt64)buf[offset + 0] << 56) +
                    ((UInt64)buf[offset + 1] << 48) +
                    ((UInt64)buf[offset + 2] << 40) +
                    ((UInt64)buf[offset + 3] << 32) +
                    ((UInt64)buf[offset + 4] << 24) +
                    ((UInt64)buf[offset + 5] << 16) +
                    ((UInt64)buf[offset + 6] << 8) +
                    ((UInt64)buf[offset + 7] << 0)
                    );
            }
            return value;
        }

        public static void SetByte8Ex(this byte[] buf, int offset, byte value)
        {
            buf[offset] = value;
        }

        public static void SetByte16Ex(this byte[] buf, int offset, UInt16 value, Endianess endian)
        {
            byte[] src = new byte[2];

            if (endian == Endianess.Little)
            {
                src[0] = (Byte)(value >> 0);
                src[1] = (Byte)(value >> 8);
            }
            else
            {
                src[0] = (Byte)(value >> 8);
                src[1] = (Byte)(value >> 0);
            }

            Mem.Copy(buf, offset, src, 0, 2);
        }

        public static void SetByte32Ex(this byte[] buf, int offset, UInt32 value, Endianess endian)
        {
            byte[] src = new byte[4];

            if (endian == Endianess.Little)
            {
                src[0] = (Byte)(value >> 0);
                src[1] = (Byte)(value >> 8);
                src[2] = (Byte)(value >> 16);
                src[3] = (Byte)(value >> 24);
            }
            else
            {
                src[0] = (Byte)(value >> 24);
                src[1] = (Byte)(value >> 16);
                src[2] = (Byte)(value >> 8);
                src[3] = (Byte)(value >> 0);
            }

            Mem.Copy(buf, offset, src, 0, 4);
        }

        public static void SetByte64Ex(this byte[] buf, int offset, UInt64 value, Endianess endian)
        {
            byte[] src = new byte[8];

            if (endian == Endianess.Little)
            {
                src[0] = (Byte)(value >> 0);
                src[1] = (Byte)(value >> 8);
                src[2] = (Byte)(value >> 16);
                src[3] = (Byte)(value >> 24);
                src[4] = (Byte)(value >> 32);
                src[5] = (Byte)(value >> 40);
                src[6] = (Byte)(value >> 48);
                src[7] = (Byte)(value >> 56);
            }
            else
            {
                src[0] = (Byte)(value >> 56);
                src[1] = (Byte)(value >> 48);
                src[2] = (Byte)(value >> 40);
                src[3] = (Byte)(value >> 32);
                src[4] = (Byte)(value >> 24);
                src[5] = (Byte)(value >> 16);
                src[6] = (Byte)(value >> 8);
                src[7] = (Byte)(value >> 0);
            }
            Mem.Copy(buf, offset, src, 0, 8);
        }

        public static void SetFloatEx(this byte[] buf, int offset, float value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            Mem.Copy(buf, offset, temp, 0, 4);
        }

        public static void SetDoubleEx(this byte[] buf, int offset, Double value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            Mem.Copy(buf, offset, temp, 0, 8);
        }

        public static byte[] ResizeEx(this byte[] buffer, int addSize)
        {
            var plus = (addSize > 0);
            int size = Math.Abs(addSize);
            byte[] result = new byte[buffer.Length + size];

            Mem.Copy(result, plus ? size : 0, buffer);

            return result;
        }

        public static float GetFloatEx(this byte[] buf, int offset)
        {
            float value = BitConverter.ToSingle(buf, offset);

            return value;
        }

        public static Double GetDoubleEx(this byte[] buf, int offset)
        {
            Double value = BitConverter.ToDouble(buf, offset);

            return value;
        }

        public static byte[] AddEx(this byte[] buf, byte[] addBuf)
        {
            return Mem.Concat(buf, addBuf);
        }

        public static byte[] CloneEx(this byte[] buf, int offset, int size)
        {
            return Mem.Clone(buf, offset, size);
        }

        public static int SearchEx(this byte[] buf, byte[] value)
        {
            if (value == null || buf.Length == 0 || value.Length == 0)
            {
                return -1;
            }

            int count = 0;
            int sizeBuf = buf.Length;
            int sizeValue = value.Length;
            int indexBuf = 0;
            int indexValue = 0;

            for (; sizeBuf > 0; --sizeBuf)
            {
                byte b1 = buf[indexBuf++];
                byte b2 = value[indexValue];

                if (b1 == b2)
                {
                    if (++indexValue == sizeValue)
                    {
                        return indexBuf;
                    }
                    count++;
                }
                else
                {
                    if (count > 0)
                    {
                        indexValue -= count;
                        count = 0;
                    }
                }
            }

            return -1;
        }

        public static Boolean IsValid(this byte[] value)
        {
            return (value != null && value.Length > 0);
        }

        #endregion
    }
}