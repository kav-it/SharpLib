using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// �������� ������ ��� ��������� ������ ������ (����� 10 ��������)
    /// </summary>
    public class HexBoxBufferProvider : IByteProvider
    {
        #region ����

        /// <summary>
        /// ��������� ����
        /// </summary>
        private readonly List<byte> _bytes;

        #endregion

        #region ��������

        /// <summary>
        /// ��������� ����
        /// </summary>
        public ReadOnlyCollection<Byte> Bytes
        {
            get { return _bytes.AsReadOnly(); }
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        public long Length
        {
            get { return _bytes.Count; }
        }

        /// <summary>
        /// true: ������ ��������
        /// </summary>
        public bool IsChanged { get; private set; }

        /// <summary>
        /// true: ��������� ������ ����
        /// </summary>
        public bool IsCanWriteByte
        {
            get { return true; }
        }

        /// <summary>
        /// true: ��������� ������� ����
        /// </summary>
        public bool IsCanInsertBytes
        {
            get { return true; }
        }

        /// <summary>
        /// true: ��������� �������� ����
        /// </summary>
        public bool IsCanDeleteBytes
        {
            get { return true; }
        }

        #endregion

        #region �������

        /// <summary>
        /// ������� "�������� ������"
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// ������� "�������� ������ ������"
        /// </summary>
        public event EventHandler LengthChanged;

        #endregion

        #region �����������

        /// <summary>
        /// �����������
        /// </summary>
        public HexBoxBufferProvider(IEnumerable<byte> bytes)
        {
            _bytes = bytes.ToList();
        }

        #endregion

        #region ������

        /// <summary>
        /// ��������� ������� "Changed"
        /// </summary>
        private void RaiseChanged(EventArgs e)
        {
            IsChanged = true;

            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        /// <summary>
        /// ��������� ������� "LengthChanged"
        /// </summary>
        private void RaiseLengthChanged(EventArgs e)
        {
            if (LengthChanged != null)
            {
                LengthChanged(this, e);
            }
        }

        /// <summary>
        /// ���������� ��������� (����� ����� Modify)
        /// </summary>
        public void ApplyChanges()
        {
            IsChanged = false;
        }

        /// <summary>
        /// ������ ����� �� ���������� ��������
        /// </summary>
        public byte ReadByte(long index)
        {
            return _bytes[(int)index];
        }

        /// <summary>
        /// ������ ����� �� ��������� ��������
        /// </summary>
        public void WriteByte(long index, byte value)
        {
            _bytes[(int)index] = value;
            RaiseChanged(EventArgs.Empty);
        }

        /// <summary>
        /// �������� ���� �� ���������
        /// </summary>
        public void DeleteBytes(long index, long length)
        {
            int internalIndex = (int)Math.Max(0, index);
            int internalLength = (int)Math.Min((int)Length, length);
            _bytes.RemoveRange(internalIndex, internalLength);

            RaiseLengthChanged(EventArgs.Empty);
            RaiseChanged(EventArgs.Empty);
        }

        /// <summary>
        /// ������� ���� � ���������
        /// </summary>
        public void InsertBytes(long index, byte[] bs)
        {
            _bytes.InsertRange((int)index, bs);

            RaiseLengthChanged(EventArgs.Empty);
            RaiseChanged(EventArgs.Empty);
        }

        #endregion
    }
}