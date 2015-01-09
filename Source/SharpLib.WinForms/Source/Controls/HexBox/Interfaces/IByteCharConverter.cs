namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// ��������� �������������� [byte]-[char], [char]-[byte]
    /// </summary>
    public interface IByteCharConverter
    {
        #region ������

        /// <summary>
        /// ����������� ������� ��� �����
        /// </summary>
        char ToChar(byte b);

        /// <summary>
        /// �������� �������� �������
        /// </summary>
        byte ToByte(char c);

        #endregion
    }
}