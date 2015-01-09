using System;
using System.Collections.Generic;

namespace SharpLib
{
    public static class Mem
    {
        #region Методы

        public static void Copy(byte[] dest, int destOffset, byte[] src, int srcOffset, int size)
        {
            if (size > 0 && dest != null && src != null && destOffset < dest.Length && srcOffset < src.Length)
            {
                size = Math.Min(dest.Length, size);
                size = Math.Min(src.Length, size);
                size = Math.Min(Math.Abs(dest.Length - destOffset), size);
                size = Math.Min(Math.Abs(src.Length - srcOffset), size);

                Array.Copy(src, srcOffset, dest, destOffset, size);
            }
        }

        public static void Copy(byte[] dest, byte[] src, int size)
        {
            Copy(dest, 0, src, 0, size);
        }

        public static void Copy(byte[] dest, byte[] src)
        {
            if (src != null)
            {
                Copy(dest, 0, src, 0, src.Length);
            }
        }

        public static void Copy(byte[] dest, int offset, byte[] src)
        {
            if (src != null)
            {
                Copy(dest, offset, src, 0, src.Length);
            }
        }

        public static void PutByte8(byte[] dest, int offset, Byte value)
        {
            dest[offset] = value;
        }

        public static void PutByte16(byte[] dest, int offset, UInt16 value, Endianess endian)
        {
            byte[] src = new Byte[2];

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

            Copy(dest, offset, src, 0, 2);
        }

        public static void PutByte32(byte[] dest, int offset, UInt32 value, Endianess endian)
        {
            byte[] src = new Byte[4];

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

            Copy(dest, offset, src, 0, 4);
        }

        public static void PutByte64(byte[] dest, int offset, UInt64 value, Endianess endian)
        {
            byte[] src = new Byte[8];

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
            Copy(dest, offset, src, 0, 8);
        }

        public static void PutFloat(byte[] buf, int offset, float value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            Copy(buf, offset, temp, 0, 4);
        }

        public static void PutDouble(byte[] buf, int offset, double value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            Copy(buf, offset, temp, 0, 8);
        }

        public static byte[] GetBuffer(byte[] buf, int offset, int size)
        {
            byte[] value = new Byte[size];

            Copy(value, 0, buf, offset, size);

            return value;
        }

        public static float GetFloat(byte[] buf, int offset)
        {
            float value = BitConverter.ToSingle(buf, offset);

            return value;
        }

        public static double GetDouble(byte[] buf, int offset)
        {
            double value = BitConverter.ToDouble(buf, offset);

            return value;
        }

        public static SByte GetSByte8(byte[] buf, int offset)
        {
            return (SByte)buf.GetByte8Ex(offset);
        }

        public static Int16 GetSByte16(byte[] buf, int offset, Endianess endian)
        {
            return (Int16)buf.GetByte16Ex(offset, endian);
        }

        public static Int32 GetSByte32(byte[] buf, int offset, Endianess endian)
        {
            return (Int32)buf.GetByte32Ex(offset, endian);
        }

        public static Int64 GetSByte64(byte[] buf, int offset, Endianess endian)
        {
            return (Int64)buf.GetByte64Ex(offset, endian);
        }

        public static Byte PopByte8(byte[] buf, ref int offset)
        {
            Byte value = buf.GetByte8Ex(offset);
            offset += 1;

            return value;
        }

        public static UInt16 PopByte16(byte[] buf, ref int offset, Endianess endian)
        {
            UInt16 value = buf.GetByte16Ex(offset, endian);
            offset += 2;

            return value;
        }

        public static UInt32 PopByte32(byte[] buf, ref int offset, Endianess endian)
        {
            UInt32 value = buf.GetByte32Ex(offset, endian);
            offset += 4;

            return value;
        }

        public static UInt64 PopByte64(byte[] buf, ref int offset, Endianess endian)
        {
            UInt64 value = buf.GetByte64Ex(offset, endian);
            offset += 8;

            return value;
        }

        public static byte[] PopBuffer(byte[] buf, ref int offset, int size)
        {
            byte[] value = GetBuffer(buf, offset, size);
            offset += size;

            return value;
        }

        public static SByte PopSByte8(byte[] buf, ref int offset)
        {
            return (SByte)PopByte8(buf, ref offset);
        }

        public static Int16 PopSByte16(byte[] buf, ref int offset, Endianess endian)
        {
            return (Int16)PopByte16(buf, ref offset, endian);
        }

        public static Int32 PopSByte32(byte[] buf, ref int offset, Endianess endian)
        {
            return (Int32)PopByte32(buf, ref offset, endian);
        }

        public static Int64 PopSByte64(byte[] buf, ref int offset, Endianess endian)
        {
            return (Int64)PopByte64(buf, ref offset, endian);
        }

        public static void PushByte8(byte[] buf, ref int offset, Byte value)
        {
            PutByte8(buf, offset, value);
            offset += 1;
        }

        public static void PushByte16(byte[] buf, ref int offset, UInt16 value, Endianess endian)
        {
            PutByte16(buf, offset, value, endian);
            offset += 2;
        }

        public static void PushByte32(byte[] buf, ref int offset, UInt32 value, Endianess endian)
        {
            PutByte32(buf, offset, value, endian);
            offset += 4;
        }

        public static void PushByte64(byte[] buf, ref int offset, UInt64 value, Endianess endian)
        {
            PutByte64(buf, offset, value, endian);
            offset += 8;
        }

        public static void PushSByte8(byte[] buf, ref int offset, SByte value)
        {
            PutByte8(buf, offset, (Byte)value);
            offset += 1;
        }

        public static void PushSByte16(byte[] buf, ref int offset, Int16 value, Endianess endian)
        {
            PutByte16(buf, offset, (UInt16)value, endian);
            offset += 2;
        }

        public static void PushSByte32(byte[] buf, ref int offset, Int32 value, Endianess endian)
        {
            PutByte32(buf, offset, (UInt32)value, endian);
            offset += 4;
        }

        public static void PushSByte64(byte[] buf, ref int offset, Int64 value, Endianess endian)
        {
            PutByte64(buf, offset, (UInt64)value, endian);
            offset += 8;
        }

        public static bool Compare(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1 == buffer2)
            {
                return true;
            }

            if (buffer2 == null)
            {
                return false;
            }

            if (buffer1.Length != buffer2.Length)
            {
                return false;
            }

            int index = 0;
            int length = buffer1.Length;
            while (index < length)
            {
                if (buffer1[index] != buffer2[index])
                {
                    return false;
                }

                index++;
            }

            return true;
        }

        public static byte[] Set(int size, Byte value)
        {
            byte[] buffer = new Byte[size];
            for (int i = 0; i < size; i++)
            {
                buffer[i] = value;
            }

            return buffer;
        }

        public static byte[] Clone(byte[] buffer)
        {
            return Clone(buffer, 0, buffer.Length);
        }

        public static byte[] Clone(byte[] buffer, int offset, int size)
        {
            if (buffer != null)
            {
                if (size > 0)
                {
                    if (offset < buffer.Length)
                    {
                        size = Math.Min(buffer.Length - offset, size);

                        byte[] result = new Byte[size];

                        Array.Copy(buffer, offset, result, 0, size);

                        return result;
                    }
                }
                else
                {
                    byte[] result = new Byte[0];

                    return result;
                }
            }

            return null;
        }

        public static byte[] Reverse(byte[] buffer)
        {
            byte[] reverseByffer = new Byte[buffer.Length];

            Array.Copy(buffer, reverseByffer, buffer.Length);

            Array.Reverse(reverseByffer);

            return reverseByffer;
        }

        /// <summary>
        /// Увеличение размера массива (для уменьшения используйте Clone)
        /// </summary>
        /// <param name="buffer">Исходный массив</param>
        /// <param name="addSize">
        /// <para>Положительное число - массив дополняется слева</para>
        /// <para>Отрицательное число - массив дополняется справа</para>
        /// </param>
        /// <returns></returns>
        public static byte[] Resize(byte[] buffer, int addSize)
        {
            bool plus = (addSize > 0);
            int size = Math.Abs(addSize);
            byte[] result = new Byte[buffer.Length + size];

            Copy(result, plus ? size : 0, buffer);

            return result;
        }

        /// <summary>
        /// Сложение массивов
        /// </summary>
        public static byte[] Concat(byte[] buffer1, byte[] buffer2)
        {
            List<Byte> list = new List<Byte>();
            if (buffer1 != null)
            {
                list.AddRange(buffer1);
            }
            if (buffer2 != null)
            {
                list.AddRange(buffer2);
            }

            byte[] result = list.ToArray();

            return result;
        }

        /// <summary>
        /// Создание буфера указанного размера, заполненного указанным значением
        /// </summary>
        public static byte[] Fill(int size, int value)
        {
            var bytes = new byte[size];

            for (int i = 0; i < size; i++)
            {
                bytes[i] = (byte)value;
            }

            return bytes;
        }

        #endregion
    }
}