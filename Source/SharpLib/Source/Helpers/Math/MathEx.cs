using System;

namespace SharpLib
{
    public class MathEx
    {
        #region ������

        /// <summary>
        /// ������ ������, ������������ �� ������� �����
        /// </summary>
        /// <param name="addr">������������� �����</param>
        /// <param name="blockSize">������ �����</param>
        /// <returns>���������� �����</returns>
        public static UInt32 GetAlignAddr(UInt32 addr, int blockSize)
        {
            // ������:
            // ������������� �����: 5
            // ������ �����       : 4
            // ����������� �����  : 4

            UInt32 alignAddr = (UInt32)(addr / blockSize);

            alignAddr = (UInt32)(alignAddr * blockSize);

            return alignAddr;
        }

        #endregion
    }
}