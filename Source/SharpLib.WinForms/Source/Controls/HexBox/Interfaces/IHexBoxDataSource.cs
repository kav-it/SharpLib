using System;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// ����� ������� � ������� ���� � ���������� HexBox
    /// </summary>
    public interface IHexBoxDataSource
    {
        #region ��������

        /// <summary>
        /// ����� ������ ������
        /// </summary>
        long Length { get; }

        /// <summary>
        /// true: ������ ��������
        /// </summary>
        bool IsChanged { get; }

        /// <summary>
        /// true: ��������� ������ ����
        /// </summary>
        bool IsCanWriteByte { get; }

        /// <summary>
        /// true: ��������� ������� ����
        /// </summary>
        bool IsCanInsertBytes { get; }

        /// <summary>
        /// true: ��������� �������� ����
        /// </summary>
        bool IsCanDeleteBytes { get; }

        #endregion

        #region �������

        /// <summary>
        /// ������� "������ ��������"
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        /// ������� "������ ������ �������"
        /// </summary>
        event EventHandler LengthChanged;

        #endregion

        #region ������

        /// <summary>
        /// ������ �����
        /// </summary>
        byte ReadByte(long index);

        /// <summary>
        /// ������ �����
        /// </summary>
        void WriteByte(long index, byte value);

        /// <summary>
        /// ������� ����� (���������� �����, ���� ������ ������ �������)
        /// </summary>
        void InsertBytes(long index, byte[] values);

        /// <summary>
        /// �������� ����
        /// </summary>
        void DeleteBytes(long index, long length);

        /// <summary>
        /// ���������� ���������
        /// </summary>
        void ApplyChanges();

        #endregion
    }
}