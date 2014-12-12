using System;

namespace SharpLib
{
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
                    if ((crc16 & 0x8000) != 0)
                    {
                        crc16 = (UInt16)((crc16 << 1) ^ 0x1021);
                    }
                    else
                    {
                        crc16 = (UInt16)((crc16 << 1));
                    }
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
}