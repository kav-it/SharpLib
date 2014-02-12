// ****************************************************************************
//
// Имя файла    : 'Conv.cs'
// Заголовок    : Модуль преобразования типов
// Автор        : Тихомиров В.С./Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpLib
{

    #region Перечисление Endianess

    /// <summary>
    /// Порядок байт
    /// </summary>
    public enum Endianess
    {
        /// <summary>
        /// Порядок от младшего к старшему (0x0100 => 1) 
        /// </summary>
        Little = 0,

        /// <summary>
        /// Порядок от старшего к младшему (0x0100 => 256) 
        /// </summary>
        Big = 1
    }

    #endregion Перечисление Endianess

    #region Перечисление TimeStampFormat

    [Flags]
    public enum TimeStampFormat
    {
        None = (0 << 0),

        DateTimeMsec = (1 << 0),

        DateTime = (1 << 1),

        Time = (1 << 2),

        Date = (1 << 3),

        TimeMSec = (1 << 4),

        YYYYMMDD = (1 << 5),

        AsText = (1 << 6),

        Backup = (1 << 7),
    }

    #endregion Перечисление TimeStampFormat

    #region Конвертор типов

    public static class Conv
    {
        #region Методы

        public static Boolean GetBit(int value, int bitIndex)
        {
            Boolean result = ((value & (1 << bitIndex)) != 0);

            return result;
        }

        public static Byte SetBit(Byte value, int bitIndex, Boolean state)
        {
            if (state)
                value = (Byte)(value + (1 << bitIndex));
            else
                value = (Byte)(value & ~(1 << bitIndex));

            return value;
        }

        public static Byte GetLow(UInt16 value)
        {
            return (Byte)(value >> 0);
        }

        public static Byte GetHigh(UInt16 value)
        {
            return (Byte)(value >> 8);
        }

        public static UInt16 SetLow(UInt16 value, Byte newValue)
        {
            return (UInt16)((value & 0xFF00) + (newValue << 0));
        }

        public static UInt16 SetHigh(UInt16 value, Byte newValue)
        {
            return (UInt16)((value & 0x00FF) + (newValue << 8));
        }

        public static Byte GetLowTetr(Byte value)
        {
            return (Byte)(value & 0x0F);
        }

        public static Byte GetHighTetr(Byte value)
        {
            return (Byte)(value >> 4);
        }

        public static Byte SetLowTetr(Byte value, Byte newValue)
        {
            return (Byte)((value & 0xF0) + (newValue & 0x0F));
        }

        public static Byte SetHighTetr(Byte value, Byte newValue)
        {
            return (Byte)((value & 0x0F) + (newValue << 4));
        }

        public static Boolean IsHex(Char ch)
        {
            if (ch >= '0' && ch <= '9') return true;
            if (ch == 'A' || ch == 'a') return true;
            if (ch == 'B' || ch == 'b') return true;
            if (ch == 'C' || ch == 'c') return true;
            if (ch == 'D' || ch == 'd') return true;
            if (ch == 'E' || ch == 'e') return true;
            if (ch == 'F' || ch == 'f') return true;

            return false;
        }

        public static Byte[] StructToBuffer(Object obj)
        {
            int size = Marshal.SizeOf(obj);
            Byte[] buffer = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, buffer, 0, size);
            Marshal.FreeHGlobal(ptr);

            return buffer;
        }

        public static Object BufferToStruct(Byte[] buffer, Type typObject)
        {
            int size = Marshal.SizeOf(typObject);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(buffer, 0, ptr, size);

            Object obj = Marshal.PtrToStructure(ptr, typObject);

            Marshal.FreeHGlobal(ptr);

            return obj;
        }

        public static String Utf8To1251(String source)
        {
            Encoding win1251 = Encoding.GetEncoding("windows-1251");
            Encoding utf8 = Encoding.UTF8;
            Byte[] srcBytes = win1251.GetBytes(source);
            Byte[] dstBytes = Encoding.Convert(utf8, win1251, srcBytes);

            return win1251.GetString(dstBytes);
        }

        public static String Utf8From1251(String source)
        {
            Encoding win1251 = Encoding.GetEncoding("windows-1251");
            Encoding utf8 = Encoding.UTF8;
            Byte[] srcBytes = utf8.GetBytes(source);
            Byte[] dstBytes = Encoding.Convert(win1251, utf8, srcBytes);

            return utf8.GetString(dstBytes);
        }

        public static String IntToIntDb(int value)
        {
            if (value == 0) return "NULL";

            return Convert.ToString(value);
        }

        public static String DoubleToStr(Double value)
        {
            return value.ToString();
        }

        public static String IntToHex(int value)
        {
            String message = String.Format("0x{0:X8}", value);

            return message;
        }

        public static String IntToHex(UInt16 value)
        {
            String message = String.Format("0x{0:X4}", value);

            return message;
        }

        public static String IntToHex(UInt32 value)
        {
            String message = String.Format("0x{0:X8}", value);

            return message;
        }

        public static String DecimalToStrDb(Decimal value)
        {
            String str = Convert.ToString(value);

            return str.Replace(",", ".");
        }

        public static String BufferToString(Byte[] buffer, String fill = null)
        {
            String text = Encoding.Default.GetString(buffer);
            String textLocal = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsControl(text[i]))
                {
                    if (fill == null)
                        break;
                    textLocal += fill;
                }
                else
                    textLocal += text[i];
            }
            return textLocal;
        }

        public static String CharsToString(Char[] value)
        {
            String text = "";

            if (value != null)
            {
                foreach (Char ch in value)
                {
                    if (Char.IsControl(ch))
                        break;

                    text += ch;
                }
            }

            return text;
        }

        public static Char[] StringToChars(String text, int size)
        {
            Char[] result = new Char[size];

            for (int i = 0; i < text.Length; i++)
                result[i] = text[i];

            return result;
        }

        public static Char ByteToChar(Byte value)
        {
            Byte[] buffer =
                {
                    1
                };
            buffer[0] = value;

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

            String text = enc.GetString(buffer);

            return text[0];
        }

        public static Char KeyToChar(Key value)
        {
            return NativeMethods.GetCharFromKey(value);
        }

        public static String KeyToString(Key value)
        {
            return "" + KeyToChar(value);
        }

        public static String FileSizeToString(Int64 size, int divider = 1000)
        {
            String[] sizeSuffixes =
                {
                    "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
                };
            const String formatTemplate = "{0}{1:0.#} {2}";

            String text;

            if (size == 0)
                text = String.Format(formatTemplate, null, 0, sizeSuffixes[0]);
            else
            {
                Double absSize = Math.Abs((double)size);
                Double fpPower = Math.Log(absSize, divider);
                int intPower = (int)fpPower;
                int iUnit = (intPower >= sizeSuffixes.Length) ? sizeSuffixes.Length - 1 : intPower;
                Double normSize = absSize / Math.Pow(divider, iUnit);

                text = String.Format(
                                     formatTemplate,
                                     (size < 0) ? "-" : null,
                                     normSize,
                                     sizeSuffixes[iUnit]);
            }

            text = text.Replace(',', '.');

            return text;
        }

        public static UInt32 StringToSize(String text)
        {
            text = text.ToUpper();

            Boolean kilo = (text.IndexOf('K') > 0);
            Boolean mega = (text.IndexOf('M') > 0);

            text = text.Replace("K", "");
            text = text.Replace("M", "");

            UInt32 size = Convert.ToUInt32(text);

            if (kilo) size *= 1024;
            else if (mega) size *= 1024 * 1024;

            return size;
        }

        public static void RaiseNotImp()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion Конвертор типов

    #region Класс ExtensionBuffer

    public static class ExtensionBuffer
    {
        #region Методы

        public static String ToStringEx(this Byte[] buffer)
        {
            String text = Encoding.Default.GetString(buffer);

            return text;
        }

        public static MemoryStream ToMemoryStreamEx(this Byte[] buffer)
        {
            MemoryStream stream = new MemoryStream(buffer.Length);

            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static String ToAsciiEx(this Byte[] buffer, String delimeter = " ")
        {
            String value = BitConverter.ToString(buffer);
            value = value.Replace("-", delimeter);

            return value;
        }

        public static List<Byte[]> SplitByEx(this Byte[] value, int chunkSize)
        {
            List<Byte[]> list = new List<Byte[]>();

            if (value != null && chunkSize > 0)
            {
                int valueLength = value.Length;

                for (int i = 0; i < valueLength; i += chunkSize)
                {
                    if (i + chunkSize > valueLength)
                        chunkSize = valueLength - i;

                    Byte[] temp = Mem.Clone(value, i, chunkSize);

                    list.Add(temp);
                }
            }

            return list;
        }

        public static Byte GetByte8Ex(this Byte[] buf, int offset)
        {
            Byte value = buf[offset];

            return value;
        }

        public static UInt16 GetByte16Ex(this Byte[] buf, int offset, Endianess endian)
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

        public static UInt32 GetByte32Ex(this Byte[] buf, int offset, Endianess endian)
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

        public static UInt64 GetByte64Ex(this Byte[] buf, int offset, Endianess endian)
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

        public static void SetByte8Ex(this Byte[] buf, int offset, Byte value)
        {
            buf[offset] = value;
        }

        public static void SetByte16Ex(this Byte[] buf, int offset, UInt16 value, Endianess endian)
        {
            Byte[] src = new Byte[2];

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

        public static void SetByte32Ex(this Byte[] buf, int offset, UInt32 value, Endianess endian)
        {
            Byte[] src = new Byte[4];

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

        public static void SetByte64Ex(this Byte[] buf, int offset, UInt64 value, Endianess endian)
        {
            Byte[] src = new Byte[8];

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

        public static void SetFloatEx(this Byte[] buf, int offset, float value)
        {
            Byte[] temp = BitConverter.GetBytes(value);

            Mem.Copy(buf, offset, temp, 0, 4);
        }

        public static void SetDoubleEx(this Byte[] buf, int offset, Double value)
        {
            Byte[] temp = BitConverter.GetBytes(value);

            Mem.Copy(buf, offset, temp, 0, 8);
        }

        public static Byte[] ResizeEx(this Byte[] buffer, int addSize)
        {
            Boolean plus = (addSize > 0);
            int size = Math.Abs(addSize);
            Byte[] result = new Byte[buffer.Length + size];

            if (plus)
                Mem.Copy(result, size, buffer);
            else
                Mem.Copy(result, 0, buffer);

            return result;
        }

        public static float GetFloatEx(this Byte[] buf, int offset)
        {
            float value = BitConverter.ToSingle(buf, offset);

            return value;
        }

        public static Double GetDoubleEx(this Byte[] buf, int offset)
        {
            Double value = BitConverter.ToDouble(buf, offset);

            return value;
        }

        public static Byte[] AddEx(this Byte[] buf, Byte[] addBuf)
        {
            return Mem.Concat(buf, addBuf);
        }

        public static Byte[] CloneEx(this Byte[] buf, int offset, int size)
        {
            return Mem.Clone(buf, offset, size);
        }

        public static int SearchEx(this Byte[] buf, Byte[] value)
        {
            if (value == null || buf.Length == 0 || value.Length == 0) return -1;

            int count = 0;
            int sizeBuf = buf.Length;
            int sizeValue = value.Length;
            int indexBuf = 0;
            int indexValue = 0;

            for (; sizeBuf > 0; --sizeBuf)
            {
                Byte b1 = buf[indexBuf++];
                Byte b2 = value[indexValue];

                if (b1 == b2)
                {
                    if (++indexValue == sizeValue) return indexBuf;
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

        public static Boolean IsValid(this Byte[] value)
        {
            return (value != null && value.Length > 0);
        }

        #endregion
    }

    #endregion Класс ExtensionBuffer

    #region Класс ExtensionDateTime

    public static class ExtensionDateTime
    {
        #region Методы

        public static String ToStringEx(this DateTime stamp, TimeStampFormat format = TimeStampFormat.DateTimeMsec)
        {
            if (format == TimeStampFormat.YYYYMMDD)
                return stamp.ToString("yyyyMMdd");
            if (format == TimeStampFormat.Backup)
                return stamp.ToString("yyyyMMddHHmmss");
            if (format == TimeStampFormat.AsText)
                return stamp.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);

            Boolean date = false;
            Boolean time = false;
            Boolean msec = false;

            if (format == TimeStampFormat.DateTimeMsec || format == TimeStampFormat.DateTime ||
                format == TimeStampFormat.Date)
                date = true;
            if (format != TimeStampFormat.Date) time = true;
            if (format == TimeStampFormat.DateTimeMsec || format == TimeStampFormat.TimeMSec)
                msec = true;

            String dStr = "";
            String tStr = "";
            String mStr = "";

            if (date)
            {
                dStr = stamp.ToString(@"dd\/MM\/yy");
                if (time) dStr += " ";
            }
            if (time)
                tStr = stamp.ToString(@"HH:mm:ss");
            if (msec)
                mStr = stamp.ToString(@".fff");

            return (dStr + tStr + mStr);
        }

        public static Boolean IsWeekendEx(this DateTime stamp)
        {
            Boolean result = (stamp.DayOfWeek == DayOfWeek.Saturday || stamp.DayOfWeek == DayOfWeek.Sunday);

            return result;
        }

        #endregion
    }

    #endregion Класс ExtensionDateTime

    #region Класс ExtensionEnum

    public static class ExtensionEnum
    {
        #region Методы

        public static String ToStringEx(this Enum value)
        {
            FieldInfo info = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

            String text = (attributes.Length > 0) ? attributes[0].Description : value.ToString();

            return text;
        }

        public static List<Object> GetValuesEx(Type typValue)
        {
            List<Object> list = new List<Object>();

            if (typValue.IsEnum)
            {
                foreach (Object obj in Enum.GetValues(typValue))
                    list.Add(obj);
            }

            return list;
        }

        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        #endregion
    }

    #endregion Класс ExtensionEnum

    #region Класс ExtensionObject

    public static class ExtensionObject
    {
        #region Методы

        public static Byte ToByteEx(this Object value)
        {
            return Convert.ToByte(value);
        }

        public static UInt16 ToUInt16Ex(this Object value)
        {
            return Convert.ToUInt16(value);
        }

        public static UInt32 ToUInt32Ex(this Object value)
        {
            return Convert.ToUInt32(value);
        }

        public static UInt64 ToUInt64Ex(this Object value)
        {
            return Convert.ToUInt64(value);
        }

        public static Int64 ToInt64Ex(this Object value)
        {
            return Convert.ToInt64(value);
        }

        public static DateTime ToDateTimeEx(this Object value)
        {
            return Convert.ToDateTime(value);
        }

        public static int ToIntEx(this Object value)
        {
            int result = (int)value;

            return result;
        }

        #endregion
    }

    #endregion Класс ExtensionObject

    #region Класс ExtensionInteger

    public static class ExtensionInteger
    {
        #region Методы

        public static String ToStringEx(this int value)
        {
            return value.ToString();
        }

        public static String ToStringEx(this Byte value, int radix = 10)
        {
            if (radix == 10) return value.ToString();
            return String.Format("0x{0:X2}", value);
        }

        public static String ToStringEx(this UInt16 value, int radix = 10)
        {
            if (radix == 10) return value.ToString();
            return String.Format("0x{0:X4}", value);
        }

        public static String ToStringEx(this UInt32 value, int radix = 10)
        {
            if (radix == 10) return value.ToString();
            return String.Format("0x{0:X8}", value);
        }

        public static String ToStringEx(this UInt64 value, int radix = 10)
        {
            if (radix == 10) return value.ToString();
            return String.Format("0x{0:X16}", value);
        }

        public static Byte[] ToBufferEx(this Byte value)
        {
            Byte[] buffer = new[]
                {
                    value
                };

            return buffer;
        }

        public static Byte[] ToBufferEx(this UInt16 value, Endianess endian)
        {
            Byte[] buffer = new Byte[2];

            Mem.PutByte16(buffer, 0, value, endian);

            return buffer;
        }

        public static Byte[] ToBufferEx(this UInt32 value, Endianess endian)
        {
            Byte[] buffer = new Byte[4];

            Mem.PutByte32(buffer, 0, value, endian);

            return buffer;
        }

        public static Byte[] ToBufferEx(this UInt64 value, Endianess endian)
        {
            Byte[] buffer = new Byte[8];

            Mem.PutByte64(buffer, 0, value, endian);

            return buffer;
        }

        public static Byte[] ToBufferEx(this int value, Endianess endian)
        {
            if (value < 0) return new Byte[0];
            if (value < (0xFF + 1)) return ((Byte)value).ToBufferEx();
            if (value < (0xFFFF + 1)) return ((UInt16)value).ToBufferEx(endian);

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

        #endregion
    }

    #endregion Класс ExtensionInteger

    #region Класс ExtensionDouble

    public static class ExtensionDouble
    {
        #region Методы

        public static Byte[] ToBufferEx(this Double value)
        {
            Byte[] buffer = BitConverter.GetBytes(value);

            return buffer;
        }

        public static Byte[] ToBufferEx(this float value)
        {
            Byte[] buffer = BitConverter.GetBytes(value);

            return buffer;
        }

        #endregion
    }

    #endregion Класс ExtensionDouble

    #region Класс ExtensionStream

    public static class ExtensionStream
    {
        #region Методы

        public static String ToStringEx(this Stream stream)
        {
            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream);
                String text = reader.ReadToEnd();

                return text;
            }

            return null;
        }

        public static Byte[] ToByfferEx(this Stream stream)
        {
            Byte[] buffer = new Byte[(int)stream.Length];

            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            return buffer;
        }

        public static MemoryStream ToMemoryStreamEx(this Stream stream)
        {
            if (stream != null)
            {
                Byte[] buffer = stream.ToByfferEx();
                MemoryStream memStream = buffer.ToMemoryStreamEx();

                return memStream;
            }

            return null;
        }

        #endregion
    }

    #endregion ExtensionStream

    #region Класс ExtensionChar

    public static class ExtensionChar
    {
        #region Методы

        public static int ToIntEx(this Char value)
        {
            int result = (int)Char.GetNumericValue(value);

            return result;
        }

        public static int ToDigitEx(this Char value)
        {
            int result = value.ToIntEx() & 0x0F;

            return result;
        }

        public static Boolean IsDigit(this Char value)
        {
            Boolean result = Char.IsDigit(value);

            return result;
        }

        public static Boolean IsWhiteSpace(this Char value)
        {
            Boolean result = Char.IsWhiteSpace(value);

            return result;
        }

        public static Boolean IsLetter(this Char value)
        {
            Boolean result = Char.IsLetter(value);

            return result;
        }

        public static Boolean IsSymbol(this Char value)
        {
            String pattern = @"`-=\~!@#$%^&*()_+|[];',./{}:""<>?";

            int index = pattern.IndexOf(value);
            Boolean result = (index != -1);

            return result;
        }

        public static Boolean IsNull(this Char value)
        {
            Boolean result = (value == 0x0000);

            return result;
        }

        public static Byte ToAsciiByteEx(this Char ch)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            String text = ch.ToString();

            Byte[] value = encoding.GetBytes(text);

            return value[0];
        }

        #endregion
    }

    #endregion Класс ExtensionChar

    #region Класс ExtensionString

    public static class ExtensionString
    {
        #region Методы

        public static String RegexEx(this String value, String pattern)
        {
            // Пример   : '1234 (567) 890'
            // Pattern  : '4 ({0}) 8'
            // Результат: 567

            String PATTERN_CONST = "{0}";

            int index = pattern.SearchEx(PATTERN_CONST);
            if (index == -1) return "";

            String textLeft = pattern.Substring(0, index - PATTERN_CONST.Length);
            String textRight = pattern.Substring(index);

            if (textLeft.IsNotValid() && textRight.IsNotValid())
                return "";

            int indexLeft = -1;
            int indexRight = -1;

            if (textLeft.IsNotValid()) indexLeft = 0;
            else if (textRight.IsNotValid()) indexRight = value.Length;

            if (indexLeft == -1) indexLeft = value.SearchEx(textLeft);
            if (indexRight == -1) indexRight = value.SearchEx(textRight, (indexLeft == -1) ? 0 : indexLeft);

            if (indexLeft == -1 || indexRight == -1)
                return "";
            if (indexLeft >= indexRight)
                return "";

            String result =
                value.Substring(indexLeft, indexRight - indexLeft - textRight.Length);

            return result;
        }

        public static String SubstringEx(this String value, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            String result = value.Substring(startIndex, length);

            return result;
        }

        public static String RemoveEx(this String value, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            String result = value.Remove(startIndex, length);

            return result;
        }

        public static int SearchEx(this String value, String substring, int offset)
        {
            int result = -1;
            int index = value.IndexOf(substring, offset);

            if (index >= 0)
                result = index + substring.Length;

            return result;
        }

        public static int SearchEx(this String value, String substring)
        {
            return SearchEx(value, substring, 0);
        }

        public static String ReplaceEx(this String text, String value, int startIndex, int endIndex)
        {
            if (value.IsNotValid()) return text;
            if (startIndex >= endIndex) return text;
            if (startIndex < 0 || endIndex >= text.Length) return text;

            String textBefore = text.Substring(0, startIndex);
            String textAfter = text.Substring(endIndex, text.Length - endIndex);

            String result = textBefore + value + textAfter;

            return result;
        }

        public static int GetIntEx(this String value, int offset)
        {
            int result = 0;

            for (int i = offset; i < value.Length; i++)
            {
                Char ch = value[i];

                if (ch.IsDigit() == false)
                    break;

                result *= 10;
                result += ch.ToDigitEx();
            }

            return result;
        }

        public static int ToIntEx(this String value)
        {
            int result;

            int.TryParse(value, out result);

            return result;
        }

        public static int ToIntFromHexEx(this String value)
        {
            int result;

            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                result = Int32.Parse(value, NumberStyles.HexNumber);
            }
            else
                result = ToIntEx(value);

            return result;
        }

        public static Byte ToByteEx(this String value)
        {
            return (Byte)value.ToUInt32Ex();
        }

        public static UInt16 ToUInt16Ex(this String value)
        {
            return (UInt16)value.ToUInt32Ex();
        }

        public static UInt32 ToUInt32Ex(this String value)
        {
            if (value == null) return 0;
            if (value == "") return 0;
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

        public static Double ToDoubleEx(this String value)
        {
            if (value == "") return 0;

            value = value.Replace(',', '.');

            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        public static Char ToCharEx(this String value)
        {
            if (value.IsValid())
                return value[0];

            return '\0';
        }

        public static IPAddress ToIpEx(this String value)
        {
            value = value.ToLower();

            if (value == "localhost") value = "127.0.0.1";

            return IPAddress.Parse(value);
        }

        public static Byte[] ToBufferEx(this String text)
        {
            Byte[] buffer = Encoding.Default.GetBytes(text);

            return buffer;
        }

        public static Byte[] ToBufferUtf8Ex(this String text)
        {
            Byte[] buffer = Encoding.UTF8.GetBytes(text);

            return buffer;
        }

        public static Byte[] ToBufferEx(this String value, int offset, int size)
        {
            if (offset > 0 && (offset + size) < value.Length)
            {
                value = value.Substring(offset, size);

                return ToBufferEx(value);
            }

            return null;
        }

        public static String CleanAscii(this String text)
        {
            Char newCh = '.';
            String res = "";

            for (int i = 0; i < text.Length; i++)
            {
                Char ch = text[i];
                int code = ch;

                if ((code < 0x20) || ((code > 0x7E) && (code < 0xC0)))
                    ch = newCh;

                res = res + ch;
            }

            return res;
        }

        public static String[] SplitEx(this String value, String delimeter)
        {
            String[] result = value.Split(
                                          new[]
                                              {
                                                  delimeter
                                              }, StringSplitOptions.RemoveEmptyEntries);

            return result;
        }

        public static String[] SplitByEx(this String value, int chunkSize)
        {
            List<String> list = new List<String>();

            if (value.IsValid() && chunkSize > 0)
            {
                int valueLength = value.Length;

                for (int i = 0; i < valueLength; i += chunkSize)
                {
                    if (i + chunkSize > valueLength)
                        chunkSize = valueLength - i;

                    String temp = value.Substring(i, chunkSize);

                    list.Add(temp);
                }
            }

            return list.ToArray();
        }

        public static Boolean IsValid(this String value)
        {
            Boolean result = (String.IsNullOrEmpty(value) == false);

            return result;
        }

        public static Boolean IsNotValid(this String value)
        {
            return (value.IsValid() == false);
        }

        public static String Remove(this String text, String subString)
        {
            text = text.Replace(subString, "");

            return text;
        }

        public static String TrimStart(this String text, String subString)
        {
            if (text.StartsWith(subString))
                text = text.Remove(0, subString.Length);

            return text;
        }

        public static String TrimEnd(this String text, String subString)
        {
            if (text.EndsWith(subString))
                text = text.Remove(text.Length - subString.Length);

            return text;
        }

        public static Byte GetByte8(this String text, int offset)
        {
            if (offset < text.Length)
            {
                Byte[] buffer = text.ToBufferEx(offset, 1);

                if (buffer != null)
                    return buffer[0];
            }

            return 0x00;
        }

        public static UInt16 GetByte16(this String text, int offset, Endianess endian)
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

        public static UInt32 GetByte32(this String text, int offset, Endianess endian)
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

        public static UInt64 GetByte64(this String text, int offset, Endianess endian)
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

        public static Stream ToStreamEx(this String text)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        public static DateTime ToDateTimeEx(this String text, TimeStampFormat format)
        {
            String pattern;

            switch (format)
            {
                case TimeStampFormat.YYYYMMDD:
                    pattern = "yyyyMMdd";
                    break;
                default:
                    throw new NotImplementedException("Формат даты не реализован");
            }

            DateTime result = DateTime.ParseExact(text, pattern, CultureInfo.InvariantCulture);

            return result;
        }

        public static Byte ToAsciiByteEx(this String text)
        {
            Byte result = 0x00;

            if (text != null && text.Length > 1)
            {
                Char chA = text[0];
                Char chB = text[1];

                Byte a = chA.ToAsciiByteEx();
                Byte b = chB.ToAsciiByteEx();

                if (Char.IsDigit(chA)) a &= 0x0F;
                else if (Char.IsUpper(chA)) a -= 0x37;
                else a -= 0x57;

                if (Char.IsDigit(chB)) b &= 0x0F;
                else if (Char.IsUpper(chB)) b -= 0x37;
                else b -= 0x57;

                result = (Byte)((a << 4) | b);
            }

            return result;
        }

        public static Byte[] ToAsciiBufferEx(this String text)
        {
            if (text != null && text.Length > 1)
            {
                int countIn = text.Length / 2;
                Byte[] result = new Byte[countIn];

                int offset = 0;
                while (offset < countIn)
                {
                    String temp = text.Substring(offset * 2, 2);
                    Byte b = ToAsciiByteEx(temp);

                    result[offset] = b;

                    offset++;
                }

                return result;
            }

            return null;
        }

        public static Byte[] ToAsciiBufferEx(this String text, String delimeter)
        {
            text = text.Replace(delimeter, "");

            return ToAsciiBufferEx(text);
        }

        public static String TabsToSpacesEx(this String value, int numSpaces)
        {
            String spaces = new String(' ', numSpaces);
            String result = value.Replace("\t", spaces);

            return result;
        }

        public static String ExpandRightEx(this String value, int width, String ch = " ")
        {
            Char c = ch[0];
            String result = value.PadRight(width, c);

            return result;
        }

        public static Boolean ContainsEx(this String source, String text, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            int index = source.IndexOf(text, comp);

            return (index >= 0);
        }

        #endregion
    }

    #endregion Класс ExtensionString

    #region Класс ExtensionStringArray

    public static class ExtensionStringArray
    {
        #region Методы

        public static void SortEx(this String[] value, Boolean descending = false)
        {
            Array.Sort(value);
            if (descending)
                Array.Reverse(value);
        }

        #endregion
    }

    #endregion ExtensionStringArray

    #region Класс ExtensionIpAddress

    public static class ExtensionIpAddress
    {
        #region Методы

        public static String ToStringEx(this IPAddress value)
        {
            return value.ToString();
        }

        #endregion
    }

    #endregion Класс ExtensionIpAddress

    #region Класс ExtensionDataRow

    public static class ExtensionDataRow
    {
        #region Методы

        public static int ToIntEx(this DataRow value, String field)
        {
            Object obj = value[field];
            int result = 0;

            if (obj != null)
                result = obj.ToIntEx();

            return result;
        }

        public static String ToStringEx(this DataRow value, String field)
        {
            Object obj = value[field];
            String result = "";

            if (obj != null)
                result = obj.ToString();

            return result;
        }

        public static DateTime ToDateTimeEx(this DataRow value, String field)
        {
            Object obj = value[field];
            DateTime result = DateTime.MinValue;

            if (obj != null)
                result = obj.ToDateTimeEx();

            return result;
        }

        public static Byte[] ToBufferEx(this DataRow value, String field)
        {
            Object obj = value[field];
            Byte[] result = new Byte[0];

            if (obj != null)
                result = (Byte[])obj;

            return result;
        }

        #endregion
    }

    #endregion Класс ExtensionDataRow

    #region Класс ExtensionList

    public static class ExstensionList
    {
    }

    #endregion Класс ExtensionList

    #region Класс ExstensionLinq

    public static class ExstensionLinq
    {
        #region Методы

        public static IEnumerable<T> DistinctByEx<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }

        #endregion
    }

    #endregion Класс ExstensionLinq

    #region Класс ExtensionEncoding

    public static class ExtensionEncoding
    {
        #region Поля

        private static Encoding _utf8;

        #endregion

        #region Свойства

        public static Encoding Utf8
        {
            get { return _utf8 ?? (_utf8 = new UTF8Encoding(false)); }
        }

        #endregion
    }

    #endregion Класс ExtensionEncoding

    #region Класс ExtensionTimeSpan
    public static class ExtensionTimeSpan
    {
        public static String ToStringEx(this TimeSpan value)
        {
            String text = String.Format("{0:00}:{1:00}:{2:00}", value.Hours, value.Minutes, value.Seconds);

            int days = (int)value.TotalDays;
            if (days > 0)
                text = String.Format("{0}д {1}", days, text);

            return text;    
        }
    }
    #endregion Класс ExtensionTimeSpan

    #region Работа с данными "Блоки данных/памяти"

    public static class Mem
    {
        #region Перемещение данных

        public static void Copy(Byte[] dest, int destOffset, Byte[] src, int srcOffset, int size)
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

        public static void Copy(Byte[] dest, Byte[] src, int size)
        {
            Copy(dest, 0, src, 0, size);
        }

        public static void Copy(Byte[] dest, Byte[] src)
        {
            if (src != null)
                Copy(dest, 0, src, 0, src.Length);
        }

        public static void Copy(Byte[] dest, int offset, Byte[] src)
        {
            if (src != null)
                Copy(dest, offset, src, 0, src.Length);
        }

        public static unsafe void CopyUnsafe(Byte[] dest, Byte* src, int size)
        {
            for (int i = 0; i < size; i++)
                dest[i] = *src++;
        }

        public static unsafe void CopyUnsafe(Byte* dest, Byte[] src, int size)
        {
            for (int i = 0; i < size; i++)
                *dest++ = src[i];
        }

        public static unsafe void CopyUnsafe(Byte* dest, String src, int maxSize)
        {
            Byte[] arr = src.ToBufferEx();
            int size = arr.Length;

            // Если максимальное значение равно 0, то оно не анализируется
            // Иначе резервируется последний символ 0
            if (maxSize == 0) maxSize = int.MaxValue;
            else maxSize--;

            // Расчет размера копирование (сколько можно и сколько есть)
            size = Math.Min(size, maxSize);

            // Копирование блока
            for (int i = 0; i < size; i++)
                *dest++ = arr[i];

            // Заполнение последнего символа завершающим 0
            dest[0] = 0;
        }

        public static unsafe void CopyUnsafe(ref String dest, Byte* src)
        {
            dest = "";
            // Копирование блока
            while (*src != 0)
                dest += Conv.ByteToChar(*src++);
        }

        public static unsafe void CopyUnsafe(Byte* dest, Byte* src, int size)
        {
            for (; size > 0; --size)
                *dest++ = *src++;
        }

        #endregion Перемещение данных

        #region Чтение/Сохранение данных в массив

        public static void PutByte8(Byte[] dest, int offset, Byte value)
        {
            dest[offset] = value;
        }

        public static void PutByte16(Byte[] dest, int offset, UInt16 value, Endianess endian)
        {
            Byte[] src = new Byte[2];

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

        public static void PutByte32(Byte[] dest, int offset, UInt32 value, Endianess endian)
        {
            Byte[] src = new Byte[4];

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

        public static void PutByte64(Byte[] dest, int offset, UInt64 value, Endianess endian)
        {
            Byte[] src = new Byte[8];

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

        public static void PutFloat(Byte[] buf, int offset, float value)
        {
            Byte[] temp = BitConverter.GetBytes(value);

            Copy(buf, offset, temp, 0, 4);
        }

        public static void PutDouble(Byte[] buf, int offset, Double value)
        {
            Byte[] temp = BitConverter.GetBytes(value);

            Copy(buf, offset, temp, 0, 8);
        }

        public static Byte[] GetBuffer(Byte[] buf, int offset, int size)
        {
            Byte[] value = new Byte[size];

            Mem.Copy(value, 0, buf, offset, size);

            return value;
        }

        public static float GetFloat(Byte[] buf, int offset)
        {
            float value = BitConverter.ToSingle(buf, offset);

            return value;
        }

        public static Double GetDouble(Byte[] buf, int offset)
        {
            Double value = BitConverter.ToDouble(buf, offset);

            return value;
        }

        public static SByte GetSByte8(Byte[] buf, int offset)
        {
            return (SByte)buf.GetByte8Ex(offset);
        }

        public static Int16 GetSByte16(Byte[] buf, int offset, Endianess endian)
        {
            return (Int16)buf.GetByte16Ex(offset, endian);
        }

        public static Int32 GetSByte32(Byte[] buf, int offset, Endianess endian)
        {
            return (Int32)buf.GetByte32Ex(offset, endian);
        }

        public static Int64 GetSByte64(Byte[] buf, int offset, Endianess endian)
        {
            return (Int64)buf.GetByte64Ex(offset, endian);
        }

        #endregion Чтение/Сохранение данных в массив

        #region Извлечение данных из массива со сдвигом смещения

        public static Byte PopByte8(Byte[] buf, ref int offset)
        {
            Byte value = buf.GetByte8Ex(offset);
            offset += 1;

            return value;
        }

        public static UInt16 PopByte16(Byte[] buf, ref int offset, Endianess endian)
        {
            UInt16 value = buf.GetByte16Ex(offset, endian);
            offset += 2;

            return value;
        }

        public static UInt32 PopByte32(Byte[] buf, ref int offset, Endianess endian)
        {
            UInt32 value = buf.GetByte32Ex(offset, endian);
            offset += 4;

            return value;
        }

        public static UInt64 PopByte64(Byte[] buf, ref int offset, Endianess endian)
        {
            UInt64 value = buf.GetByte64Ex(offset, endian);
            offset += 8;

            return value;
        }

        public static Byte[] PopBuffer(Byte[] buf, ref int offset, int size)
        {
            Byte[] value = GetBuffer(buf, offset, size);
            offset += size;

            return value;
        }

        public static SByte PopSByte8(Byte[] buf, ref int offset)
        {
            return (SByte)PopByte8(buf, ref offset);
        }

        public static Int16 PopSByte16(Byte[] buf, ref int offset, Endianess endian)
        {
            return (Int16)PopByte16(buf, ref offset, endian);
        }

        public static Int32 PopSByte32(Byte[] buf, ref int offset, Endianess endian)
        {
            return (Int32)PopByte32(buf, ref offset, endian);
        }

        public static Int64 PopSByte64(Byte[] buf, ref int offset, Endianess endian)
        {
            return (Int64)PopByte64(buf, ref offset, endian);
        }

        public static void PushByte8(Byte[] buf, ref int offset, Byte value)
        {
            PutByte8(buf, offset, value);
            offset += 1;
        }

        public static void PushByte16(Byte[] buf, ref int offset, UInt16 value, Endianess endian)
        {
            PutByte16(buf, offset, value, endian);
            offset += 2;
        }

        public static void PushByte32(Byte[] buf, ref int offset, UInt32 value, Endianess endian)
        {
            PutByte32(buf, offset, value, endian);
            offset += 4;
        }

        public static void PushByte64(Byte[] buf, ref int offset, UInt64 value, Endianess endian)
        {
            PutByte64(buf, offset, value, endian);
            offset += 8;
        }

        public static void PushSByte8(Byte[] buf, ref int offset, SByte value)
        {
            PutByte8(buf, offset, (Byte)value);
            offset += 1;
        }

        public static void PushSByte16(Byte[] buf, ref int offset, Int16 value, Endianess endian)
        {
            PutByte16(buf, offset, (UInt16)value, endian);
            offset += 2;
        }

        public static void PushSByte32(Byte[] buf, ref int offset, Int32 value, Endianess endian)
        {
            PutByte32(buf, offset, (UInt32)value, endian);
            offset += 4;
        }

        public static void PushSByte64(Byte[] buf, ref int offset, Int64 value, Endianess endian)
        {
            PutByte64(buf, offset, (UInt64)value, endian);
            offset += 8;
        }

        #endregion Извлечение данных из массива со сдвигом смещения

        #region Операции над массивами

        public static Boolean Compare(Byte[] buffer1, Byte[] buffer2)
        {
            if (buffer1 == buffer2)
                return true;

            if (buffer2 == null)
                return false;

            if (buffer1.Length != buffer2.Length)
                return false;

            int index = 0;
            int length = buffer1.Length;
            while (index < length)
            {
                if (buffer1[index] != buffer2[index])
                    return false;

                index++;
            }

            return true;
        }

        public static Byte[] Set(int size, Byte value)
        {
            Byte[] buffer = new Byte[size];
            for (int i = 0; i < size; i++)
                buffer[i] = value;

            return buffer;
        }

        public static Byte[] Clone(Byte[] buffer)
        {
            return Clone(buffer, 0, buffer.Length);
        }

        public static Byte[] Clone(Byte[] buffer, int offset, int size)
        {
            if (buffer != null)
            {
                if (size > 0)
                {
                    if (offset < buffer.Length)
                    {
                        size = Math.Min(buffer.Length - offset, size);

                        Byte[] result = new Byte[size];

                        Array.Copy(buffer, offset, result, 0, size);

                        return result;
                    }
                }
                else
                {
                    Byte[] result = new Byte[0];

                    return result;
                }
            }

            return null;
        }

        public static Byte[] Reverse(Byte[] buffer)
        {
            Byte[] reverseByffer = new Byte[buffer.Length];

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
        public static Byte[] Resize(Byte[] buffer, int addSize)
        {
            Boolean plus = (addSize > 0);
            int size = Math.Abs(addSize);
            Byte[] result = new Byte[buffer.Length + size];

            if (plus)
                Mem.Copy(result, size, buffer);
            else
                Mem.Copy(result, 0, buffer);

            return result;
        }

        /// <summary>
        /// Сложение массивов
        /// </summary>
        public static Byte[] Concat(Byte[] buffer_1, Byte[] buffer_2)
        {
            List<Byte> list = new List<Byte>();
            if (buffer_1 != null)
                list.AddRange(buffer_1);
            if (buffer_2 != null)
                list.AddRange(buffer_2);

            Byte[] result = list.ToArray();

            return result;
        }

        #endregion Операции над массивами
    }

    #endregion Работа с данными "Блоки данных/памяти"

    #region Работа с данными "Дата/Время"

    public static class Time
    {
        #region Методы

        public static TimeSpan StrToTimeSpan(String value)
        {
            TimeSpan result = TimeSpan.Parse(value);

            return result;
        }

        public static String NowToStr()
        {
            return NowToStr(TimeStampFormat.DateTimeMsec);
        }

        public static String NowToStr(TimeStampFormat format)
        {
            DateTime dt = DateTime.Now;

            return dt.ToStringEx(format);
        }

        public static int DeltaMs(DateTime before, DateTime after)
        {
            TimeSpan span = after - before;

            int dt = (int)span.TotalMilliseconds;

            return dt;
        }

        public static int DeltaMin(DateTime before, DateTime after)
        {
            TimeSpan span = after - before;

            int dt = (int)span.TotalMinutes;

            return dt;
        }

        public static String DeltaToStr(DateTime before, DateTime after)
        {
            TimeSpan elapsed = after - before;

            String text = String.Format("{0:00}:{1:00}:{2:00}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds);

            int days = (int)elapsed.TotalDays;
            if (days > 0)
                text = String.Format("{0}д {1}", days, text);

            return text;
        }

        #endregion
    }

    #endregion Работа с данными "Дата/Время"

    #region Вспомогательный класс "Байтовый массив в виде списка"

    public class ByteList
    {
        #region Поля

        private List<Byte> _list;

        #endregion

        #region Свойства

        /// <summary>
        /// Количество байт в массиве
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Байтовый массив
        /// </summary>
        public Byte[] Buffer
        {
            get { return ToBuffer(); }
        }

        /// <summary>
        /// Внутреннее смещение в массиве 
        /// (используется в операциях GetXXX)
        /// </summary>
        public int Offset { get; set; }

        #endregion

        #region Конструктор

        public ByteList()
        {
            _list = new List<Byte>();
        }

        public ByteList(Byte[] buffer) : this()
        {
            _list.AddRange(buffer);
        }

        public ByteList(int capacity) : this(new Byte[capacity])
        {
        }

        #endregion

        #region Добавление данных

        public void AddByte8(Byte value)
        {
            _list.Add(value);
        }

        public void AddByte16(UInt16 value, Endianess endian = Endianess.Little)
        {
            Byte[] buffer = new Byte[2];
            Mem.PutByte16(buffer, 0, value, endian);

            _list.AddRange(buffer);
        }

        public void AddByte32(UInt32 value, Endianess endian = Endianess.Little)
        {
            Byte[] buffer = new Byte[4];
            Mem.PutByte32(buffer, 0, value, endian);

            _list.AddRange(buffer);
        }

        public void AddByte64(UInt64 value, Endianess endian = Endianess.Little)
        {
            Byte[] buffer = new Byte[8];
            Mem.PutByte64(buffer, 0, value, endian);

            _list.AddRange(buffer);
        }

        public void AddBuffer(Byte[] buffer)
        {
            if (buffer != null)
                _list.AddRange(buffer);
        }

        public void AddBuffer(Byte[] buffer, int offset, int size)
        {
            if (buffer != null)
            {
                buffer = Mem.Clone(buffer, offset, size);
                _list.AddRange(buffer);
            }
        }

        public void AddString(String value, int arraySize = 0)
        {
            // Длина не передана: размещение всей строки
            if (arraySize == 0) arraySize = value.Length + 1;

            Byte[] bufStr = value.ToBufferEx();
            Byte[] bufOut = new Byte[arraySize];
            // Резервирование места под последний байт для создания
            // нуль-терминированной строки
            int size = Math.Min(bufStr.Length, arraySize - 1);
            // Формирование данных в выходной буфере
            Mem.Copy(bufOut, 0, bufStr, 0, size);
            // Добавление данных в список
            AddBuffer(bufOut);
        }

        public void AddStringEncodingDefault(String value, int arraySize)
        {
            // Длина не передана: размещение всей строки
            if (arraySize == 0) arraySize = value.Length + 1;

            Byte[] bufStr = Encoding.Default.GetBytes(value);
            Byte[] bufOut = new Byte[arraySize];
            // Резервирование места под последний байт для создания
            // нуль-терминированной строки
            int size = Math.Min(bufStr.Length, arraySize - 1);
            // Формирование данных в выходной буфере
            Mem.Copy(bufOut, 0, bufStr, 0, size);
            // Добавление данных в список
            AddBuffer(bufOut);
        }

        public void AddFloat(float value)
        {
            Byte[] temp = BitConverter.GetBytes(value);

            _list.AddRange(temp);
        }

        public void AddDouble(Double value)
        {
            Byte[] temp = BitConverter.GetBytes(value);

            _list.AddRange(temp);
        }

        public void AddInt(int value, Endianess endian = Endianess.Little)
        {
            AddByte32((UInt32)value, endian);
        }

        public void AddLong(long value, Endianess endian = Endianess.Little)
        {
            AddByte64((UInt64)value, endian);
        }

        public void AddDateTime(DateTime value)
        {
            AddLong(value.Ticks);
        }

        #endregion Добавление данных

        #region Извлечение данных

        private UInt64 GetByteCustom(int sizeValue, Endianess endian)
        {
            UInt64 value = 0;

            if ((Offset + sizeValue) <= Count)
            {
                Byte[] buffer = new Byte[sizeValue];
                _list.CopyTo(Offset, buffer, 0, sizeValue);

                switch (sizeValue)
                {
                    case 1:
                        value = buffer.GetByte8Ex(0);
                        break;
                    case 2:
                        value = buffer.GetByte16Ex(0, endian);
                        break;
                    case 4:
                        value = buffer.GetByte32Ex(0, endian);
                        break;
                    case 8:
                        value = buffer.GetByte64Ex(0, endian);
                        break;
                    default:
                        value = 0;
                        break;
                }

                Offset += sizeValue;
            }

            return value;
        }

        public Byte GetByte8()
        {
            Byte value = (Byte)GetByteCustom(1, Endianess.Little);

            return value;
        }

        public UInt16 GetByte16(Endianess endian = Endianess.Little)
        {
            UInt16 value = (UInt16)GetByteCustom(2, endian);

            return value;
        }

        public UInt32 GetByte32(Endianess endian = Endianess.Little)
        {
            UInt32 value = (UInt32)GetByteCustom(4, endian);

            return value;
        }

        public UInt64 GetByte64(Endianess endian = Endianess.Little)
        {
            UInt64 value = GetByteCustom(8, endian);

            return value;
        }

        public Byte[] GetBuffer(int size)
        {
            if (size == 0)
                size = Count - Offset;

            Byte[] buffer = new Byte[size];

            if ((Offset + size) <= Count)
            {
                _list.CopyTo(Offset, buffer, 0, size);

                Offset += size;
            }

            return buffer;
        }

        public String GetString(int size = 0)
        {
            Boolean includeNullCh = false;

            if (size == 0)
            {
                // Строка определяется по байту '\0'
                int offset = Offset;
                while (offset < Count)
                {
                    if (_list[offset++] == 0)
                    {
                        includeNullCh = true; break;
                    }

                    size++;
                }

                // Данных в строке не обнаружено
                if (size == 0)
                {
                    Offset++;
                    return "";
                }
            }

            // Указан желаемый размер строки
            Byte[] buffer = GetBuffer(size);
            String value = Conv.BufferToString(buffer);

            if (includeNullCh)
                Offset++;

            return value;
        }

        public float GetFloat()
        {
            Byte[] buffer = GetBuffer(4);
            float value = BitConverter.ToSingle(buffer, 0);

            return value;
        }

        public Double GetDouble()
        {
            Byte[] buffer = GetBuffer(8);
            Double value = BitConverter.ToDouble(buffer, 0);

            return value;
        }

        public int GetInt(Endianess endian = Endianess.Little)
        {
            return (int)GetByte32(endian);
        }

        public long GetLong(Endianess endian = Endianess.Little)
        {
            return (long)GetByte64(endian);
        }

        public DateTime GetDateTime()
        {
            long ticks = GetLong();

            return new DateTime(ticks);
        }

        #endregion Извлечение данных

        #region Преобразование данных

        public Byte[] ToBuffer()
        {
            Byte[] buffer = _list.ToArray();

            return buffer;
        }

        #endregion Преобразование данных
    }

    #endregion Вспомогательный класс "Байтовый массив в виде списка"

    #region Класс VisualTree

    public static class VisualTree
    {
        #region Поиск вверх по визуальному дереву

        public static T FindAncestor<T>(Object element) where T : DependencyObject
        {
            if (element is DependencyObject)
            {
                DependencyObject current = element as DependencyObject;

                current = VisualTreeHelper.GetParent(current);

                while (current != null)
                {
                    if (current is T)
                        return (T)current;
                    current = VisualTreeHelper.GetParent(current);
                }
            }
            return null;
        }

        public static T FindAncestor<T>(DependencyObject current, T lookupItem) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T && current == lookupItem)
                    return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        public static T FindAncestor<T>(DependencyObject current, string parentName) where T : DependencyObject
        {
            while (current != null)
            {
                if (!string.IsNullOrEmpty(parentName))
                {
                    var frameworkElement = current as FrameworkElement;
                    if (current is T && frameworkElement != null && frameworkElement.Name == parentName)
                        return (T)current;
                }
                else if (current is T)
                    return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        #endregion Поиск вверх по визуальному дереву

        #region Поиск вниз по визуальному дереву

        private static DependencyObject FindDownRecurse(DependencyObject parent, Type searchType, String searchName)
        {
            if (parent == null) return null;
            DependencyObject foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                Type childType = child.GetType();

                if (childType != searchType)
                {
                    // Типы не совпадают: Продолжение поиска
                    foundChild = FindDownRecurse(child, searchType, searchName);

                    // Элемент найден
                    if (foundChild != null) break;
                }
                else if (searchName.IsValid())
                {
                    var frameworkElement = child as FrameworkElement;
                    // Имя не указано для поиска
                    if (frameworkElement != null && frameworkElement.Name == searchName)
                    {
                        // Совпадение имени: Элемент найден
                        foundChild = child;
                        break;
                    }
                    else
                    {
                        // Продолжение поиска
                        foundChild = FindDownRecurse(child, searchType, searchName);

                        // Элемент не найден
                        if (foundChild != null) break;
                    }
                }
                else
                {
                    // Элемент найден
                    foundChild = child;
                    break;
                }
            }

            return foundChild;
        }

        public static DependencyObject FindDown(Type searchType, String searchName, DependencyObject root = null)
        {
            if (root == null)
                root = Application.Current.MainWindow;

            DependencyObject result = FindDownRecurse(root, searchType, searchName);

            return result;
        }

        #endregion Поиск вниз по визуальному дереву

        #region Вывод визуального дерева

        private static void PrintVisualRecurse(DependencyObject current, ref int ident, ref String result)
        {
            if (current == null) return;

            String name = "";

            if (current is FrameworkElement)
                name = (current as FrameworkElement).Name;

            for (int i = 0; i < ident; i++)
                result += "    ";

            result += String.Format("'{0}' ({1})\r\n", name, current.GetType());

            // Увеличение отступа
            ++ident;
            // ===========================
            //
            // Обход дочерних элементов
            int childrenCount = VisualTreeHelper.GetChildrenCount(current);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(current, i);
                PrintVisualRecurse(child, ref ident, ref result);
            }
            //
            // ==========================
            // Уменьшение отступа
            --ident;
        }

        public static void PrintVisual(DependencyObject root = null)
        {
            int ident = 0;

            if (root == null)
                root = Application.Current.MainWindow;

            String result = String.Format("\r\n----------- Print visual tree '{0}' -----------\r\n", root);

            PrintVisualRecurse(root, ref ident, ref result);
        }

        private static void PrintLogicalRecurse(Object current, ref int ident, ref String result)
        {
            if (current == null) return;

            String name = "";

            if (current is FrameworkElement)
                name = (current as FrameworkElement).Name;

            for (int i = 0; i < ident; i++)
                result += "    ";

            result += String.Format("'{0}' ({1})\r\n", name, current.GetType());

            ++ident;

            // ==================================
            // 
            if (current is FrameworkElement)
            {
                IEnumerable children = LogicalTreeHelper.GetChildren(current as FrameworkElement);
                foreach (Object child in children)
                    PrintLogicalRecurse(child, ref ident, ref result);
            }
            // 
            // =================================

            --ident;
        }

        public static void PrintLogical(Object root = null)
        {
            int ident = 0;

            if (root == null)
                root = Application.Current.MainWindow;

            String result = String.Format("\r\n----------- Print logical tree '{0}' -----------\r\n", root);

            PrintLogicalRecurse(root, ref ident, ref result);
        }

        #endregion Вывод визуального дерева
    }

    #endregion Класс VisualTree
}