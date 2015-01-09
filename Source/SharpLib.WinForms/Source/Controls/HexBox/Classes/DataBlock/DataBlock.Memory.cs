using System;

namespace SharpLib.WinForms.Controls
{
    internal sealed class MemoryDataBlock : DataBlock
    {
        #region ����

        #endregion

        #region ��������

        public override long Length
        {
            get { return Data.LongLength; }
        }

        public byte[] Data { get; private set; }

        #endregion

        #region �����������

        public MemoryDataBlock(byte data)
        {
            Data = new[] { data };
        }

        public MemoryDataBlock(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            Data = (byte[])data.Clone();
        }

        #endregion

        #region ������

        public void AddByteToEnd(byte value)
        {
            byte[] newData = new byte[Data.LongLength + 1];
            Data.CopyTo(newData, 0);
            newData[newData.LongLength - 1] = value;
            Data = newData;
        }

        public void AddByteToStart(byte value)
        {
            byte[] newData = new byte[Data.LongLength + 1];
            newData[0] = value;
            Data.CopyTo(newData, 1);
            Data = newData;
        }

        public void InsertBytes(long position, byte[] data)
        {
            byte[] newData = new byte[Data.LongLength + data.LongLength];
            if (position > 0)
            {
                Array.Copy(Data, 0, newData, 0, position);
            }
            Array.Copy(data, 0, newData, position, data.LongLength);
            if (position < Data.LongLength)
            {
                Array.Copy(Data, position, newData, position + data.LongLength, Data.LongLength - position);
            }
            Data = newData;
        }

        public override void RemoveBytes(long position, long count)
        {
            byte[] newData = new byte[Data.LongLength - count];

            if (position > 0)
            {
                Array.Copy(Data, 0, newData, 0, position);
            }
            if (position + count < Data.LongLength)
            {
                Array.Copy(Data, position + count, newData, position, newData.LongLength - position);
            }

            Data = newData;
        }

        #endregion
    }
}