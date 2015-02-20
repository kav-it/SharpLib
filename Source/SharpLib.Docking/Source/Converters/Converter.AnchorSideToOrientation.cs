using System;
using System.Windows.Controls;
using System.Windows.Data;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Converters
{
    [ValueConversion(typeof(AnchorSide), typeof(Orientation))]
    public class AnchorSideToOrientationConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var side = (AnchorSide)value;
            if (side == AnchorSide.Left || side == AnchorSide.Right)
            {
                return Orientation.Vertical;
            }

            return Orientation.Horizontal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}