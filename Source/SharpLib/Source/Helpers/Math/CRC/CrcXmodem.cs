using System;

namespace SharpLib
{
    public class CrcXmodem
    {
        #region ������

        /// <summary>
        /// ������ ����������� �����
        /// </summary>
        /// <param name="arr">������ ������</param>
        /// <param name="index">������ ������ �������</param>
        /// <param name="size">���������� ���� � �������</param>
        /// <returns>����������� ����� GSM 07.10</returns>
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
        /// ������ ����������� �����
        /// </summary>
        /// <param name="arr">������ ������</param>
        public static UInt16 Get(Byte[] arr)
        {
            return Get(arr, 0, arr.Length);
        }

        #endregion
    }
}