//*****************************************************************************
//
// Имя файла    : 'IntelHex.cs'
// Заголовок    : Формат "IntelHex"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 11/11/2012
//
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Windows;

namespace SharpLib
{
    #region Перечисление IntexHexRecordTyp
    public enum IntexHexRecordTyp: byte
    {
        Unknow              = 0xFF,
        Data                = 0x00,
        EndFile             = 0x01,
        SegmentAddr         = 0x02,
        StartSegmentAddr    = 0x03,
        LinearAddr          = 0x04,
        StartLinearAddr     = 0x05
    }
    #endregion Перечисление IntexHexRecordTyp

    #region Класс IntelHexRecord
    public class IntelHexRecord
    {
        #region Поля
        private Byte[] _buffer;
        private String _text;
        #endregion Поля

        #region Свойства
        public Byte Len
        {
            get { return _buffer[0]; }
        }
        public UInt16 Addr
        {
            get 
            { 
                return _buffer.GetByte16Ex(1, Endianess.Big); 
            }
        }
        public IntexHexRecordTyp Typ
        {
            get 
            { 
                return (IntexHexRecordTyp)_buffer[3]; 
            }
        }
        public Byte[] Data
        {
            get 
            {
                Byte[] value = Mem.Clone(_buffer, 4, Len);

                return value; 
            }
        }
        public Byte Crc
        {
            get 
            { 
                return _buffer[_buffer.Length - 1]; 
            }
        }
        public String Text 
        { 
            get { return _text; }
        }
	    public Byte[] Buffer
	    {
		    get { return _buffer; }
	    }
        public Boolean IsValid
        {
            get 
            { 
                Byte crc_0 = GetCrc(Buffer);
                Byte crc_1 = Crc;

                return (crc_0 == crc_1);
            }
        }
        public Boolean IsSegment
        {
            get 
            {
                return (Typ == IntexHexRecordTyp.LinearAddr || Typ == IntexHexRecordTyp.SegmentAddr);
            }
        }
        public Boolean IsData
        {
            get
            {
                return (Typ == IntexHexRecordTyp.Data);
            }
        }
        public UInt32 AddrSegment
        {
            get
            {
                UInt32 value = 0;

                if (IsSegment)                
                {
                    UInt16 tempAddr = Data.GetByte16Ex(0, Endianess.Big);

                    if (Typ == IntexHexRecordTyp.SegmentAddr)
                        value = (UInt32)(tempAddr << 4);
                    else
                        value = (UInt32)(tempAddr << 16);
                }

                return value;

            }
        }
        #endregion Свойства

        #region Конструктор
        public IntelHexRecord()
        {
            _buffer = null;
            _text   = null;
        }
        public IntelHexRecord(String text): this()
        {
            FromText(text);
        }
        public IntelHexRecord(UInt16 addr, IntexHexRecordTyp typ, Byte[] data)
        {
            ByteList list = new ByteList();

            int len = (data == null) ? 0 : data.Length;

            list.AddByte8((Byte)len);
            list.AddByte16(addr, Endianess.Big);
            list.AddByte8((Byte)typ);
            list.AddBuffer(data);
            list.AddByte8(0xAA);

            _buffer = list.ToBuffer();

            UpdateCrc();
            UpdateText();
        }
        #endregion Конструктор

        #region Методы
        public override String ToString()
        {
            return _text;
        }
        /// <summary>
        /// Расчет контрольной суммы блока
        /// </summary>
        private Byte GetCrc (Byte[] record)
        {
            // Контрольная сумма расчитывается для всех полей, кроме последнего
            // (контрольной суммы)
            //
            // 020000040010EA = ~(0x02 + 0x00 + 0x00 + 0x04 + 0x10) + 1 = 0xEA
            //

            Byte crc = 0;
            for (int i = 0; i < record.Length - 1; i++)
            {
                crc += record[i];
            }

            crc = (Byte)(~crc + 1);

            return crc;
        }
        /// <summary>
        /// Заполнение полей записи на основании строки текста 
        /// Например ':020000040010EA'
        /// </summary>
        private void FromText(String text)
        {
            _text = text;
            // Удаление первого символа ":", поэтому offset = 1
            // :020000040010EA -> 020000040010EA
            text = text.TrimStart(':');
            // Преобразование: 020000040010EA -> 0x02 0x00 0x00 0x04 0x00 0x10 0xEA
            // 0x02      - количество байт данных
            // 0x00 0x00 - адрес
            // 0x04      - тип поля 
            // 0x00 0x10 - данные (зависит от типа записи)
            _buffer = text.ToAsciiBufferEx();
        }
        /// <summary>
        /// Перерасчет текстового представления
        /// </summary>
        private void UpdateText()
        {
            String text = ":" + _buffer.ToAsciiEx("");

            _text = text;
        }
        /// <summary>
        /// Перерасчет контрольной суммы (при изменении данных)
        /// </summary>
        public void UpdateCrc()
        {
            Byte localCrc = GetCrc(Buffer);

            _buffer[_buffer.Length - 1] = localCrc; 
        }

        /// <summary>
        /// Формирование записи "Сегмент (расширенный)"
        /// </summary>
        public static IntelHexRecord SegmentRecord(UInt32 addr)
        {
            // Для расширенного сегмента в поле данных адрес по 64K
            UInt16 addrSegment = (UInt16)(addr >> 16);

            Byte[] data = addrSegment.ToBufferEx(Endianess.Big);

            IntelHexRecord record = new IntelHexRecord(0, IntexHexRecordTyp.LinearAddr, data);

            return record;
        }
        /// <summary>
        /// Формирование записи "Данные"
        /// </summary>
        public static IntelHexRecord DataRecord(UInt16 addr, Byte[] data)
        {
            IntelHexRecord record = new IntelHexRecord(addr, IntexHexRecordTyp.Data, data);

            return record;
        }
        /// <summary>
        /// Формирование записи "Конец файла"
        /// </summary>
        public static IntelHexRecord EndRecord()
        {
            // Для расширенного сегмента в поле данных адрес по 64K
            IntelHexRecord record = new IntelHexRecord(0, IntexHexRecordTyp.EndFile, null);

            return record;
        }
        #endregion Методы
    }
    #endregion Класс IntelHex

    #region Класс IntelHexRegion
    public class IntelHexRegion : IComparable
    {
        #region Константы
        public const int REGION_SIZE = 64 * 1024;
        #endregion Константы

        #region Поля
        private UInt32 _addr;
        private Byte[] _buffer;
        private Byte[] _present;
        #endregion Поля

        #region Свойства
        public UInt32 AddrStart
        {
            get { return _addr; }
            set { _addr = value; }
        }
        public UInt32 AddrEnd
        {
            get { return (UInt32)(AddrStart + REGION_SIZE - 1); }
        }
        public Boolean IsEmpty
        {
            get
            {
                foreach (Byte present in _present)
                {
                    if (present != 0)
                        return false;
                }

                return true;
            }
        }
        #endregion Свойства

        #region Конструктор
        public IntelHexRegion(UInt32 addr)
        {
            _addr   = addr;
            _buffer = new Byte[REGION_SIZE];
            _present = new Byte[REGION_SIZE];
        }
        #endregion Конструктор

        #region Методы
        /// <summary>
        /// Текстовое представление класса (для отладки)
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            String text = String.Format("Addr: {0}", Conv.IntToHex(_addr));

            return text;
        }
        /// <summary>
        /// Реализация сравнения (по начальному адресу)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int IComparable.CompareTo(Object obj)
        {
            IntelHexRegion region = (IntelHexRegion)obj;

            if (this.AddrStart > region.AddrStart)
                return 1;
            if (this.AddrStart < region.AddrStart)
                return -1;

            return 0;
        }
        /// <summary>
        /// Проверка принадлежности адреса региону
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public Boolean IsValidAddr (UInt32 addr)
        {
            if (addr >= AddrStart && addr <= AddrEnd)
                return true;

            return false;
        }
        /// <summary>
        /// Запись блока данных
        /// </summary>
        /// <param name="addr">Полный адрес записи</param>
        /// <param name="data">Блок данных</param>
        /// <returns>Количество записанных данных</returns>
        public int Write (UInt32 addr, Byte[] data)
        {
            if ( IsValidAddr(addr))
            {
                int offsetAddr = (int)(addr % IntelHexRegion.REGION_SIZE);
                // Расчет размера записываемых данных (с учетом возможного перехода на следующий регион)
                int remainSize = IntelHexRegion.REGION_SIZE - offsetAddr;
                int writeSize  = (data.Length > remainSize) ? remainSize : data.Length;

                for (int i = 0; i < writeSize; i++)
                {
                    _buffer[offsetAddr + i]  = data[i];
                    _present[offsetAddr + i] = 1;
                }

                return writeSize;
            }

            return 0;
        }
        /// <summary>
        /// Чтение блока данных
        /// </summary>
        /// <param name="addr">Полный адрес записи</param>
        /// <param name="data">Размер блока данных</param>
        /// <returns>Прочитанный блок</returns>
        public Byte[] Read(UInt32 addr, int size)
        {
            if (IsValidAddr(addr))
            {
                int offsetAddr = (int)(addr % IntelHexRegion.REGION_SIZE);
                // Расчет размера данных (с учетом возможного перехода на следующий регион)
                int remainSize = IntelHexRegion.REGION_SIZE - offsetAddr;
                int readSize   = (size > remainSize) ? remainSize : size;

                Byte[] data = new Byte[readSize];

                for (int i = 0; i < readSize; i++)
                {
                    // Адрес не существует
                    if (_present[offsetAddr + i] == 0) return null;

                    data[i] = _buffer[offsetAddr + i];
                }

                return data;
            }

            return null;
        }
        /// <summary>
        /// Удаление данных из региона
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public int Remove (UInt32 addr, int size)
        {
            if (IsValidAddr(addr))
            {
                int offsetAddr = (int)(addr % IntelHexRegion.REGION_SIZE);
                // Расчет размера данных (с учетом возможного перехода на следующий регион)
                int remainSize = IntelHexRegion.REGION_SIZE - offsetAddr;
                int removeSize = (size > remainSize) ? remainSize : size;

                for (int i = 0; i < removeSize; i++)
                {
                    // Адрес не существует
                    _present[offsetAddr + i] = 0;
                    _buffer[offsetAddr + i] = 0;
                }

                return removeSize;
            }

            return 0;
        }
        /// <summary>
        /// Чтение буфера с заполнением
        /// </summary>
        public Byte[] ReadFill(UInt32 addr, int size, Byte fill)
        {
            if (IsValidAddr(addr))
            {
                int offsetAddr = (int)(addr % IntelHexRegion.REGION_SIZE);
                // Расчет размера данных (с учетом возможного перехода на следующий регион)
                int remainSize = IntelHexRegion.REGION_SIZE - offsetAddr;
                int readSize   = (size > remainSize) ? remainSize : size;

                Byte[] data = Mem.Set(readSize, fill);

                for (int i = 0; i < readSize; i++)
                {
                    if (_present[offsetAddr + i] != 0)
                    {
                        // Данные есть
                        data[i] = _buffer[offsetAddr + i];
                    }
                }

                return data;
            }

            return null;
        }
        /// <summary>
        /// Преобразование региона в текстовое представление
        /// </summary>
        /// <returns></returns>
        public String ToText(int width)
        {
            String result = "";

            // Формирование записи "Регион" (для 0-го адреса описание региона не добавляется)
            if (AddrStart != 0)
            {
                IntelHexRecord recordSegment = IntelHexRecord.SegmentRecord(AddrStart);
                result = recordSegment.Text + "\r\n";
            }

            Byte[]  buffer = new Byte[width];
            int     count  = 0;
            int     addr   = -1;
            Boolean flag   = false; 

            for (int i = 0; i < REGION_SIZE; i++)
            {
                if (_present[i] != 0)
                {
                    // Сохранение адреса начала блока
                    if (addr == -1) addr = i;
                    // Сохранение байта в буфер
                    buffer[count++] = _buffer[i];

                    if (count >= width || (i == (REGION_SIZE - 1)))
                    {
                        // Набрана строка: Формирование записи
                        flag = true;
                    }
                } // end if (байт присутствует)
                else
                {
                    // Нет данных
                    if (addr != -1)
                    {
                        // Если предыдущие данные были -> Формирование записи
                        flag = true;
                    }
                }

                if (flag)
                {
                    // Набрана строка: Формирование записи
                    flag = false;
                    // Выделение данных из буфера (если количество меньше)
                    Byte[] data = Mem.Clone(buffer, 0, count);
                    // Формирование записи
                    IntelHexRecord recordData = IntelHexRecord.DataRecord((UInt16)addr, data);
                    
                    // Добавление текстового представления к результату
                    result += recordData.Text + "\r\n";

                    // Сброс адреса и счетчика байт
                    addr = -1;
                    count = 0;
                }

            } // end for (перебор всех байт в регионе)

            return result;
        }
        /// <summary>
        /// Формирование сообщение об ошибке адреса
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static String ErrorAddrText (UInt32 addr)
        {
            String text = String.Format("Адрес {0} не существует", Conv.IntToHex(addr));

            return text;
        }
        #endregion Методы
    }
    #endregion Класс IntelHexRegion

    #region Класс IntelHex
    public class IntelHex
    {
        #region Константы
        private const int SAVE_HEX_WIDTH = 16;
        #endregion Константы

        #region Поля
        private List<IntelHexRegion> _regions;
        #endregion Поля

        #region Свойства
        public List<IntelHexRegion> Regions
        {
            get { return _regions; }
            set { _regions = value; }
        }
        #endregion Свойства

        #region Конструктор
        public IntelHex()
        {
            _regions = new List<IntelHexRegion>();
        }
        #endregion Конструктор

        #region Методы

        #region Загрузка/Сохранение файла
        public Boolean Load(String filename)
        {
            String error;

            return Load(filename, out error);
        }
        public Boolean Load (String filename, out String error)
        {
            error = "";

            _regions = new List<IntelHexRegion>();

            // Проверка существования файла
            if (Files.IsExists(filename) == false) 
            {
                error = "Файл не существует"; return false;
            }

            // Чтение файла
            String text = Files.ReadText(filename);
            if (text == null)
            {
                error = "Ошибка чтения файла"; return false;
            }

            // Разделение текста на записи (разделитель #13#10)
            String[] lines = text.SplitEx("\r\n");

            if (lines.Length == 0)
            {
                error = "Файл не IntelHex формата"; return false;
            }

            List<IntelHexRecord> records = new List<IntelHexRecord>();

            // Проверка контрольных суммы записей
            for (int i = 0; i < lines.Length; i++)
            {
                String line = lines[i];
                
                IntelHexRecord record = new IntelHexRecord(line);

                if (record.IsValid == false)
                {
                    error = String.Format("Неверный формат. Строка {0}", i + 1); 

                    return false;
                }

                records.Add(record);
            }

            // Добавление записей в регионы (с автоматическим созданием регионов)
            UInt32 baseAddr = 0;
            for (int i = 0; i < records.Count; i++)
            {
                IntelHexRecord record = records[i];

                if (record.IsData)
                {
                    // Запись: данные
                    WriteBuffer(baseAddr + record.Addr, record.Data);
                }
                else if (record.IsSegment)
                {
                    // Запись: сегмент -> установка нового базового адреса
                    baseAddr = record.AddrSegment;
                }
            }

            // Проверка
            if (Regions.Count == 0)
            {
                error = String.Format("Не найдено ни одного региона"); return false;
            }

            // Сортировка регионов по возрастанию адреса
            Regions.Sort();
            
            return true;
        }
        public Boolean Save (String filename, out String error, int width = SAVE_HEX_WIDTH)
        {
            if (Regions.Count == 0)
            {
                error = "Нет регионов с данными"; return false;
            }

            error = "";
            String result = "";

            // Формирование строк для каждого региона
            for (int i = 0; i < Regions.Count; i++)
            {
                IntelHexRegion region = Regions[i];

                String temp = region.ToText(width);

                result += temp;
            }

            // Добавление записи "Конец файла"
            IntelHexRecord record = IntelHexRecord.EndRecord();
            result += record.Text + "\r\n";

            // Сохранение результата в файл
            Files.WriteText(filename, result);

            return true;
        }
        public Boolean SaveBin (String filename, UInt32 addr, int size, Byte fill)
        {
            Byte[] data = ReadBufferFill(addr, size, fill);

            Files.Write(filename, data);

            return true;
        }
        #endregion Загрузка/Сохранение файла

        #region Операции с регионами
        /// <summary>
        /// Создание региона для определенного адреса и добавление его в список
        /// </summary>
        private IntelHexRegion CreateRegion(UInt32 addr)
        {
            UInt32 baseAddr = MathEx.GetAlignAddr(addr, IntelHexRegion.REGION_SIZE);

            IntelHexRegion region = new IntelHexRegion(baseAddr);

            _regions.Add(region);
            _regions.Sort();

            return region;
        }
        /// <summary>
        /// Удаление региона
        /// </summary>
        /// <param name="region"></param>
        private void RemoveRegion(IntelHexRegion region)
        {
            _regions.Remove(region);
            _regions.Sort();
        }
        /// <summary>
        /// Поиск региона, содержащего адрес
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        private IntelHexRegion SearchRegion(UInt32 addr)
        {
            foreach (IntelHexRegion region in _regions)
            {
                if (region.IsValidAddr(addr))
                    return region;
            }

            return null;
        }
        #endregion Операции с регионами

        #region Чтение/Запись/Удаление данных
        /// <summary>
        /// Удаление данных
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="size"></param>
        public void RemoveBuffer (UInt32 addr, int size)
        {
            while (size > 0)
            {
                // Поиск расположения данных в 64к регионах
                IntelHexRegion region = SearchRegion(addr);
                int            removeSize;

                // Регион не существует для текущего адреса
                if (region != null)
                {
                    // Удаление данных из региона
                    removeSize = region.Remove(addr, size);
                    
                    if (removeSize == 0)
                    {
                        // Данных нет для удаления: Переход к следующему региону
                        removeSize = IntelHexRegion.REGION_SIZE;
                    }
                    else
                    {
                        // Данные удалены из региона: Возможно регион тоже нужно удалить
                        // т.к. он может уже не содержать данных
                        if (region.IsEmpty)
                            RemoveRegion(region);
                    }
                }
                else
                {
                    // Регион не существует: Переход к следующему адресу через регион
                    removeSize = IntelHexRegion.REGION_SIZE;
                }

                size -= removeSize;
                addr += (UInt32)removeSize;

            } // end while (поиск адресов)
        }
        /// <summary>
        /// Запись данные по указанному адресу
        /// </summary>
        public void WriteBuffer(UInt32 addr, Byte[] data)
        {
            // Поиск расположения данных в 64к регионах
            IntelHexRegion region = SearchRegion(addr);

            // Регион не существует для текущего адреса
            if (region == null)
            {
                // Создание региона
                region = CreateRegion(addr);
            }

            int totalSize = data.Length;
            int writeSize = region.Write(addr, data);

            if (writeSize != 0 && writeSize < totalSize)
            {
                // Буфер записан не весь: Часть переноситься на другой регион
                data = Mem.Clone(data, writeSize, totalSize - writeSize);
                addr += (UInt32)writeSize;

                // Запись следующего региона
                WriteBuffer(addr, data);
            }
        }
        /// <summary>
        /// Чтение данных по указанному адресу
        /// </summary>
        public Byte[] ReadBuffer(UInt32 addr, int size, out String error)
        {
            error = "";

            // Поиск расположения данных в 64к регионах
            IntelHexRegion region = SearchRegion(addr);

            // Регион не существует для текущего адреса
            if (region == null)
            {
                error = IntelHexRegion.ErrorAddrText(addr); return null;
            } 

            Byte[] data = region.Read(addr, size);

            // Адрес не существует
            if (data == null) return null;

            int readSize = data.Length;

            if (readSize != 0 && readSize < size)
            {
                // Буфер прочитан не весь: Часть переноситься на другой регион
                addr += (UInt32)readSize;
                Byte[] remain = ReadBuffer(addr, size - readSize, out error);
                if (remain == null) return null;

                data = Mem.Concat(data, remain);
            }

            return data;
        }
        /// <summary>
        /// Чтение данных по указанному адресу с заполнением недостающих данных
        /// </summary>
        /// <returns></returns>
        public Byte[] ReadBufferFill (UInt32 addr, int size, Byte fill)
        {
            Byte[] data = null;

            while (size > 0)
            {
                // Поиск расположения данных в 64к регионах
                IntelHexRegion region = SearchRegion(addr);
                Byte[]         block;
                int            readSize;

                // Регион не существует для текущего адреса
                if (region != null)
                {
                    // Чтение данных
                    block    = region.ReadFill(addr, size, fill);
                    readSize = block.Length;
                }
                else
                {
                    readSize = (size > IntelHexRegion.REGION_SIZE) ? IntelHexRegion.REGION_SIZE : size;
                    block = Mem.Set(readSize, fill);
                }

                // Добавление данных к существующему буферу
                data = Mem.Concat(data, block);
                
                addr += (UInt32)readSize;
                size -= readSize;
                
            } // end while (перебор всего размера)

            return data;
        }
        /// <summary>
        /// Чтение байта по указанному адресу в HEX-формате
        /// </summary>
        public Boolean ReadByte8 (UInt32 addr, out Byte value, out String error)
        {
            value = 0;

            Byte[] buffer = ReadBuffer(addr, 1, out error);

            if (buffer == null)
                return false;

            value = buffer[0];

            return true;
        }
        /// <summary>
        /// Чтение 4-х байтного значения по указанному адресу
        /// </summary>
        public Boolean ReadByte16(UInt32 addr, out UInt16 value, Endianess endian, out String error)
        {
            value = 0;

            Byte[] buffer = ReadBuffer(addr, 2, out error);

            if (buffer == null)
                return false;

            value = buffer.GetByte16Ex(0, endian);

            return true;
        }
        /// <summary>
        /// Чтение 4-х байтного значения по указанному адресу
        /// </summary>
        public Boolean ReadByte32(UInt32 addr, out UInt32 value, Endianess endian, out String error)
        {
            value = 0;
            
            Byte[] buffer = ReadBuffer(addr, 4, out error);

            if (buffer == null)
                return false;

            value = buffer.GetByte32Ex(0, endian);

            return true;
        }
        /// <summary>
        /// Запись байта по указанному адресу 
        /// </summary>
        public Boolean WriteByte8(UInt32 addr, Byte value, out String error)
        {
            Byte[] buffer = value.ToBufferEx();

            WriteBuffer(addr, buffer);

            error = "";
            return true;
        }
        /// <summary>
        /// Запись 2-х байтового значения по указанному адресу 
        /// </summary>
        public Boolean WriteByte16(UInt32 addr, UInt16 value, Endianess endian, out String error)
        {
            Byte[] buffer = value.ToBufferEx(endian);

            WriteBuffer(addr, buffer);

            error = "";
            return true;
        }
        /// <summary>
        /// Запись 2-х байтового значения по указанному адресу 
        /// </summary>
        public Boolean WriteByte32(UInt32 addr, UInt32 value, Endianess endian, out String error)
        {
            Byte[] buffer = value.ToBufferEx(endian);

            WriteBuffer(addr, buffer);

            error = "";
            return true;
        }
        #endregion Чтение/Запись/Удаление данных

        #endregion Методы
    }
    #endregion Класс IntelHex
}
