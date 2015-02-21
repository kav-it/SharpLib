using System;
using System.Windows.Data;

using SharpLib.Docking;

namespace SharpLib.Docking.Converters
{
    [ValueConversion(typeof(AnchorSide), typeof(double))]
    public class AnchorSideToAngleConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var side = (AnchorSide)value;
            if (side == AnchorSide.Left || side == AnchorSide.Right)
            {
                return 90.0;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}