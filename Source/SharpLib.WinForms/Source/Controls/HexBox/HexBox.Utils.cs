using System.Globalization;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Вспомогательный класс работы с Border элемента
    /// </summary>
    internal class HexBoxUtils
    {
        internal static string ConvertByteToHex(byte b)
        {
            return string.Format("{0:X2}", b);
        }
    }
}