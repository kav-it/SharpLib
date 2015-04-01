using System.Windows;

namespace SharpLib.Wpf
{
    public static class FontHelper
    {
        #region Методы

        public static double PointToPixelFontSize(string fontSizeInPoints)
        {
            var converter = new FontSizeConverter();
            var result = converter.ConvertFrom(fontSizeInPoints);

            return result != null ? (double)result : 10;
        }

        #endregion
    }
}