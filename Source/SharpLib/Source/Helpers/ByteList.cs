using System;
using System.Collections.Generic;
using System.Text;

namespace SharpLib
{
    public class ByteList
    {
        #region Поля

        private readonly List<Byte> _list;

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

        public ByteList(IEnumerable<byte> buffer)
            : this()
        {
            _list.AddRange(buffer);
        }

        public ByteList(int capacity)
            : this(new Byte[capacity])
        {
        }

        #endregion

        #region Методы

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
            {
                _list.AddRange(buffer);
            }
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
            if (arraySize == 0)
            {
                arraySize = value.Length + 1;
            }

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
            if (arraySize == 0)
            {
                arraySize = value.Length + 1;
            }

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
            {
                size = Count - Offset;
            }

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
                        includeNullCh = true;
                        break;
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
            var buffer = GetBuffer(size);
            var value = buffer.ToStringEx();

            if (includeNullCh)
            {
                Offset++;
            }

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

        public Byte[] ToBuffer()
        {
            Byte[] buffer = _list.ToArray();

            return buffer;
        }

        #endregion
    }
}