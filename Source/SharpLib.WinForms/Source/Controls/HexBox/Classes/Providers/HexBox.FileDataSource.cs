using System;
using System.Collections.Generic;
using System.IO;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// ��������� ������ ��� ������� ������ (����� 100 ��)
    /// </summary>
    public class HexBoxFileDataSource : IHexBoxDataSource, IDisposable
    {
        #region ����

        /// <summary>
        /// true: ������ ������
        /// </summary>
        private readonly bool _readOnly;

        /// <summary>
        /// ��������� ���� ���������
        /// </summary>
        private readonly Dictionary<long, byte> _writes;

        /// <summary>
        /// ����� ������ �����
        /// </summary>
        private FileStream _fileStream;

        #endregion

        #region ��������

        /// <summary>
        /// ��� �����
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// ������ ������
        /// </summary>
        public long Length
        {
            get { return _fileStream.Length; }
        }

        /// <summary>
        /// true: ������ ��������
        /// </summary>
        public bool IsChanged
        {
            get { return (_writes.Count > 0); }
        }

        /// <summary>
        /// true: ��������� ������ ����
        /// </summary>
        public bool IsCanWriteByte
        {
            get { return !_readOnly; }
        }

        /// <summary>
        /// true: ��������� ������� ����
        /// </summary>
        public bool IsCanInsertBytes
        {
            get { return false; }
        }

        /// <summary>
        /// true: ��������� �������� ����
        /// </summary>
        public bool IsCanDeleteBytes
        {
            get { return false; }
        }

        #endregion

        #region �������

        /// <summary>
        /// ������� "�������� ������"
        /// </summary>
        public event EventHandler Changed;

        #endregion

        #region �����������

        /// <summary>
        /// �����������
        /// </summary>
        public HexBoxFileDataSource(string fileName)
        {
            _writes = new Dictionary<long, byte>();

            FileName = fileName;

            try
            {
                // ������� ������� ���� � ������ "������"
                _fileStream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            }
            catch
            {
                // �������� ����� � ������ "Read" � "Shared"
                _fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _readOnly = true;
            }
        }

        /// <summary>
        /// ������ Dispose (�� ������������ unmanaged)
        /// </summary>
        ~HexBoxFileDataSource()
        {
            Dispose();
        }

        #endregion

        #region ������

        /// <summary>
        /// ��������� ������� "Changed"
        /// </summary>
        private void RaiseChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        /// <summary>
        /// ���������� ���������
        /// </summary>
        public void ApplyChanges()
        {
            if (_readOnly)
            {
                throw new Exception("File is in read-only mode.");
            }

            if (IsChanged == false)
            {
                return;
            }

            // ���������� ������ � ����
            foreach (var entry in _writes)
            {
                var index = entry.Key;
                var value = entry.Value;

                _fileStream.Position = index;
                _fileStream.Write(new[] { value }, 0, 1);
            }

            _writes.Clear();
        }

        /// <summary>
        /// ������ ���������
        /// </summary>
        public void RejectChanges()
        {
            _writes.Clear();
        }

        /// <summary>
        /// ������ ����� �� �����
        /// </summary>
        public byte ReadByte(long index)
        {
            if (_writes.ContainsKey(index))
            {
                return _writes[index];
            }

            _fileStream.Position = index;

            byte res = (byte)_fileStream.ReadByte();

            return res;
        }

        /// <summary>
        /// ������ �����
        /// </summary>
        public void WriteByte(long index, byte value)
        {
            if (_writes.ContainsKey(index))
            {
                _writes[index] = value;
            }
            else
            {
                _writes.Add(index, value);
            }

            RaiseChanged(EventArgs.Empty);
        }

        /// <summary>
        /// �������� ����� (�� ��������������)
        /// </summary>
        public void DeleteBytes(long index, long length)
        {
            throw new NotSupportedException("HexBoxFileDataSource.DeleteBytes");
        }

        /// <summary>
        /// ������� ����� (�� ��������������)
        /// </summary>
        public void InsertBytes(long index, byte[] bs)
        {
            throw new NotSupportedException("HexBoxFileDataSource.InsertBytes");
        }

        /// <summary>
        /// Releases the file handle used by the HexBoxFileDataSource.
        /// </summary>
        public void Dispose()
        {
            if (_fileStream != null)
            {
                FileName = null;

                _fileStream.Close();
                _fileStream = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}