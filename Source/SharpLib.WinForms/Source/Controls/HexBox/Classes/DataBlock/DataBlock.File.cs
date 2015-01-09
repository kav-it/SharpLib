using System;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// ���� ������ = ����
    /// </summary>
    internal sealed class FileDataBlock : DataBlock
    {
        #region ����

        private long _length;

        #endregion

        #region ��������

        public long FileOffset { get; private set; }

        public override long Length
        {
            get { return _length; }
        }

        #endregion

        #region �����������

        public FileDataBlock(long fileOffset, long length)
        {
            FileOffset = fileOffset;
            _length = length;
        }

        #endregion

        #region ������

        public void SetFileOffset(long value)
        {
            FileOffset = value;
        }

        public void RemoveBytesFromEnd(long count)
        {
            if (count > _length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            _length -= count;
        }

        public void RemoveBytesFromStart(long count)
        {
            if (count > _length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            FileOffset += count;
            _length -= count;
        }

        public override void RemoveBytes(long position, long count)
        {
            if (position > _length)
            {
                throw new ArgumentOutOfRangeException("position");
            }

            if (position + count > _length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            long prefixLength = position;
            long prefixFileOffset = FileOffset;

            long suffixLength = _length - count - prefixLength;
            long suffixFileOffset = FileOffset + position + count;

            if (prefixLength > 0 && suffixLength > 0)
            {
                FileOffset = prefixFileOffset;
                _length = prefixLength;
                _map.AddAfter(this, new FileDataBlock(suffixFileOffset, suffixLength));
                return;
            }

            if (prefixLength > 0)
            {
                FileOffset = prefixFileOffset;
                _length = prefixLength;
            }
            else
            {
                FileOffset = suffixFileOffset;
                _length = suffixLength;
            }
        }

        #endregion
    }
}