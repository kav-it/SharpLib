// ****************************************************************************
//
// Имя файла    : 'Math.cs'
// Заголовок    : Модуль выполнения математических операций
// Автор        : Крыцкий А.В./Тихомиров В.С.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;

namespace SharpLib
{

    #region Реализация CRC-8

    /// <summary>
    /// Реализация контрольной суммы CRC8
    /// </summary>
    public class Crc8
    {
        #region Поля

        /// <summary>
        /// Таблица контрольной суммы (reversed, 8-bit, poly=0x07)
        /// </summary>
        private static Byte[] _tableCrc8 =
            {
                0x00, 0x5E, 0xBC, 0xE2, 0x61, 0x3F, 0xDD, 0x83, 0xC2, 0x9C, 0x7E, 0x20, 0xA3, 0xFD, 0x1F, 0x41,
                0x9D, 0xC3, 0x21, 0x7F, 0xFC, 0xA2, 0x40, 0x1E, 0x5F, 0x01, 0xE3, 0xBD, 0x3E, 0x60, 0x82, 0xDC,
                0x23, 0x7D, 0x9F, 0xC1, 0x42, 0x1C, 0xFE, 0xA0, 0xE1, 0xBF, 0x5D, 0x03, 0x80, 0xDE, 0x3C, 0x62,
                0xBE, 0xE0, 0x02, 0x5C, 0xDF, 0x81, 0x63, 0x3D, 0x7C, 0x22, 0xC0, 0x9E, 0x1D, 0x43, 0xA1, 0xFF,
                0x46, 0x18, 0xFA, 0xA4, 0x27, 0x79, 0x9B, 0xC5, 0x84, 0xDA, 0x38, 0x66, 0xE5, 0xBB, 0x59, 0x07,
                0xDB, 0x85, 0x67, 0x39, 0xBA, 0xE4, 0x06, 0x58, 0x19, 0x47, 0xA5, 0xFB, 0x78, 0x26, 0xC4, 0x9A,
                0x65, 0x3B, 0xD9, 0x87, 0x04, 0x5A, 0xB8, 0xE6, 0xA7, 0xF9, 0x1B, 0x45, 0xC6, 0x98, 0x7A, 0x24,
                0xF8, 0xA6, 0x44, 0x1A, 0x99, 0xC7, 0x25, 0x7B, 0x3A, 0x64, 0x86, 0xD8, 0x5B, 0x05, 0xE7, 0xB9,
                0x8C, 0xD2, 0x30, 0x6E, 0xED, 0xB3, 0x51, 0x0F, 0x4E, 0x10, 0xF2, 0xAC, 0x2F, 0x71, 0x93, 0xCD,
                0x11, 0x4F, 0xAD, 0xF3, 0x70, 0x2E, 0xCC, 0x92, 0xD3, 0x8D, 0x6F, 0x31, 0xB2, 0xEC, 0x0E, 0x50,
                0xAF, 0xF1, 0x13, 0x4D, 0xCE, 0x90, 0x72, 0x2C, 0x6D, 0x33, 0xD1, 0x8F, 0x0C, 0x52, 0xB0, 0xEE,
                0x32, 0x6C, 0x8E, 0xD0, 0x53, 0x0D, 0xEF, 0xB1, 0xF0, 0xAE, 0x4C, 0x12, 0x91, 0xCF, 0x2D, 0x73,
                0xCA, 0x94, 0x76, 0x28, 0xAB, 0xF5, 0x17, 0x49, 0x08, 0x56, 0xB4, 0xEA, 0x69, 0x37, 0xD5, 0x8B,
                0x57, 0x09, 0xEB, 0xB5, 0x36, 0x68, 0x8A, 0xD4, 0x95, 0xCB, 0x29, 0x77, 0xF4, 0xAA, 0x48, 0x16,
                0xE9, 0xB7, 0x55, 0x0B, 0x88, 0xD6, 0x34, 0x6A, 0x2B, 0x75, 0x97, 0xC9, 0x4A, 0x14, 0xF6, 0xA8,
                0x74, 0x2A, 0xC8, 0x96, 0x15, 0x4B, 0xA9, 0xF7, 0xB6, 0xE8, 0x0A, 0x54, 0xD7, 0x89, 0x6B, 0x35
            };

        #endregion

        #region Методы

        /// <summary>
        /// Расчет контрольной суммы
        /// </summary>
        /// <param name="arr">Массив данных</param>
        /// <param name="index">Индекс начала расчета</param>
        /// <param name="size">Количество байт в расчете</param>
        /// <returns>Контрольная сумма GSM 07.10</returns>
        public static Byte Get(Byte[] arr, int index, int size)
        {
            Byte crc8 = 0;

            for (int i = 0; i < size; i++)
                crc8 = _tableCrc8[crc8 ^ arr[index + i]];
            return crc8;
        }

        /// <summary>
        /// Расчет контрольной суммы
        /// </summary>
        /// <param name="arr">Массив данных</param>
        public static Byte Get(Byte[] arr)
        {
            return Get(arr, 0, arr.Length);
        }

        #endregion
    }

    #endregion Реализация CRC-8

    #region Реализация CRC-16

    /// <summary>
    /// Реализация контрольной суммы CRC16
    /// </summary>
    public class Crc16
    {
        #region Поля

        /// <summary>
        /// Таблица контрольной суммы (reversed, 8-bit, poly=0x07)
        /// </summary>
        private static UInt16[] _tableCrc16 =
            {
                0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
                0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef,
                0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6,
                0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de,
                0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485,
                0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d,
                0x3653, 0x2672, 0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4,
                0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc,
                0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823,
                0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b,
                0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12,
                0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a,
                0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41,
                0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49,
                0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70,
                0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78,
                0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f,
                0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004, 0x4025, 0x7046, 0x6067,
                0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e,
                0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256,
                0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d,
                0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
                0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c,
                0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634,
                0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab,
                0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3,
                0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a,
                0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92,
                0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9,
                0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1,
                0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8,
                0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0
            };

        #endregion

        #region Методы

        /// <summary>
        /// Расчет контрольной суммы
        /// </summary>
        /// <param name="arr">Массив данных</param>
        /// <param name="index">Индекс начала расчета</param>
        /// <param name="size">Количество байт в расчете</param>
        /// <returns>Контрольная сумма GSM 07.10</returns>
        public static UInt16 Get(Byte[] arr, int index, int size)
        {
            UInt16 crc16 = 0;

            for (int i = 0; i < size; i++)
                crc16 = (UInt16)(_tableCrc16[(crc16 >> 8) & 0xFF] ^ (crc16 << 8) ^ arr[index + i]);

            return crc16;
        }

        /// <summary>
        /// Расчет контрольной суммы
        /// </summary>
        /// <param name="arr">Массив данных</param>
        public static UInt16 Get(Byte[] arr)
        {
            return Get(arr, 0, arr.Length);
        }

        /// <summary>
        /// Расчет контрольной суммы
        /// </summary>
        /// <param name="arr">Блок данных</param>
        /// <param name="index">Индекс начала расчета</param>
        /// <param name="size">Размер блока для расчетов</param>
        /// <param name="fill">true - дополнять блок данных, расчитанной суммой</param>
        /// <returns>Результирующая сумма</returns>
        public static Boolean Check(Byte[] arr, int index, int size, Boolean fill)
        {
            UInt16 crc16 = Get(arr, index, size);

            if (fill)
                Mem.PutByte16(arr, index + size, crc16, Endianess.Little);
            else
            {
                UInt16 temp = arr.GetByte16Ex(index + size, Endianess.Little);

                return (temp == crc16);
            }

            return false;
        }

        #endregion
    }

    #endregion Реализация CRC-16

    #region Реализация CRC-Xmodem

    public class CrcXmodem
    {
        #region Методы

        /// <summary>
        /// Расчет контрольной суммы
        /// </summary>
        /// <param name="arr">Массив данных</param>
        /// <param name="index">Индекс начала расчета</param>
        /// <param name="size">Количество байт в расчете</param>
        /// <returns>Контрольная сумма GSM 07.10</returns>
        public static UInt16 Get(Byte[] arr, int index, int size)
        {
            UInt16 crc16 = 0;

            for (; size > 0; --size)
            {
                crc16 = (UInt16)(crc16 ^ (arr[index++] << 8));

                for (uint i = 0; i < 8; i++)
                {
                    if ((crc16 & 0x8000) != 0) crc16 = (UInt16)((crc16 << 1) ^ 0x1021);
                    else crc16 = (UInt16)((crc16 << 1));
                }
            }

            return crc16;
        }

        /// <summary>
        /// Расчет контрольной суммы
        /// </summary>
        /// <param name="arr">Массив данных</param>
        public static UInt16 Get(Byte[] arr)
        {
            return Get(arr, 0, arr.Length);
        }

        #endregion
    }

    #endregion Реализация CRC-Xmodem

    #region Класс Rand

    /// <summary>
    /// Реализация генерации случайных чисел
    /// </summary>
    public class Rand
    {
        #region Поля

        public static readonly Random Value;

        private static Random _rand;

        #endregion

        #region Конструктор

        static Rand()
        {
            Value = new Random();
            _rand = new Random(DateTime.Now.Ticks.GetHashCode());
        }

        #endregion

        #region Методы

        public static UInt32 Get(UInt32 min, UInt32 max)
        {
            int value = _rand.Next((int)min, (int)max);

            return (UInt32)value;
        }

        public static UInt32 Get(UInt32 max)
        {
            return Get(0, max);
        }

        public static UInt32 Get()
        {
            return Get(0, int.MaxValue);
        }

        public static int GetInt()
        {
            return (int)Get(0, int.MaxValue);
        }

        public static int GetInt(int max)
        {
            return (int)Get(0, (UInt32)max);
        }

        public static Byte[] GetBuffer(int size)
        {
            Byte[] buffer = new Byte[size];

            _rand.NextBytes(buffer);

            return buffer;
        }

        #endregion
    }

    #endregion Класс Rand

    #region Класс MathEx

    public class MathEx
    {
        #region Методы

        /// <summary>
        /// Расчет адреса, выровненного по границе блока
        /// </summary>
        /// <param name="addr">Невыровненный адрес</param>
        /// <param name="blockSize">Размер блока</param>
        /// <returns>Выровенный адрес</returns>
        public static UInt32 GetAlignAddr(UInt32 addr, int blockSize)
        {
            // Пример:
            // Невыровненный адрес: 5
            // Размер блока       : 4
            // Выровненный адрес  : 4

            UInt32 alignAddr = (UInt32)(addr / blockSize);

            alignAddr = (UInt32)(alignAddr * blockSize);

            return alignAddr;
        }

        #endregion
    }

    #endregion Класс MathEx
}