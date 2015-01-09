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
        string ToChar(byte b);

        /// <summary>
        /// ����������� ������� � ���� ������
        /// </summary>
        string ToText(byte[] bytes);

        /// <summary>
        /// �������������� string - byte[]
        /// </summary>
        byte[] ToBuffer(string text);

        /// <summary>
        /// �������� �������� �������
        /// </summary>
        byte ToByte(char c);

        #endregion
    }
}