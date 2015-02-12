using System;
using System.Globalization;

namespace SharpLib
{
    public static class ExtensionInteger
    {
        #region Методы

        public static string ToStringEx(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringEx(this Byte value, int radix = 10)
        {
            if (radix == 10)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
            return String.Format("{0:X2}", value);
        }

        public static string ToStringEx(this UInt16 value, int radix = 10)
        {
            if (radix == 10)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
            return String.Format("{0:X4}", value);
        }

        public static string ToStringEx(this UInt32 value, int radix = 10)
        {
            if (radix == 10)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
            return String.Format("{0:X8}", value);
        }

        public static string ToStringEx(this UInt64 value, int radix = 10)
        {
            if (radix == 10)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }

            return String.Format("{0:X16}", value);
        }

        public static byte[] ToBufferEx(this Byte value)
        {
            byte[] buffer = { value };

            return buffer;
        }

        public static byte[] ToBufferEx(this UInt16 value, Endianess endian)
        {
            byte[] buffer = new byte[2];

            Mem.PutByte16(buffer, 0, value, endian);

            return buffer;
        }

        public static byte[] ToBufferEx(this UInt32 value, Endianess endian)
        {
            byte[] buffer = new byte[4];

            Mem.PutByte32(buffer, 0, value, endian);

            return buffer;
        }

        public static byte[] ToBufferEx(this UInt64 value, Endianess endian)
        {
            byte[] buffer = new byte[8];

            Mem.PutByte64(buffer, 0, value, endian);

            return buffer;
        }

        public static byte[] ToBufferEx(this int value, Endianess endian)
        {
            if (value < 0)
            {
                return new byte[0];
            }
            if (value < (0xFF + 1))
            {
                return ((Byte)value).ToBufferEx();
            }
            if (value < (0xFFFF + 1))
            {
                return ((UInt16)value).ToBufferEx(endian);
            }

            return ((UInt32)value).ToBufferEx(endian);
        }

        public static UInt16 SwitchOrderEx(this UInt16 value)
        {
            UInt16 result = (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);

            return result;
        }

        public static UInt32 SwitchOrderEx(this UInt32 value)
        {
            UInt32 result =
                (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;

            return result;
        }

        public static UInt64 SwitchOrderEx(this UInt64 value)
        {
            UInt64 result =
                (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;

            return result;
        }

        /// <summary>
        /// Проверка, установлен ли бит
        /// </summary>
        public static bool IsBitEx(this int self, int bitIndex)
        {
            return ((self & (1 << bitIndex)) != 0);
        }

        /// <summary>
        /// Установка бита в нужное состояние
        /// </summary>
        public static int SetBitEx(this int self, int bitIndex, bool state)
        {
            if (state)
            {
                self = (self + (1 << bitIndex));
            }
            else
            {
                self = (self & ~(1 << bitIndex));
            }

            return self;
        }

        #endregion
    }
}