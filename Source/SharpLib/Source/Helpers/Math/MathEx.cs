using System;

namespace SharpLib
{
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
}