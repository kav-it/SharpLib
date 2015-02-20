using System;
using System.Windows;
using System.Windows.Data;

namespace SharpLib.Docking.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && targetType == typeof(Visibility))
            {
                bool val = (bool)value;
                if (val)
                {
                    return Visibility.Visible;
                }
                if (parameter is Visibility)
                {
                    return parameter;
                }
                return Visibility.Collapsed;
            }
            if (value == null)
            {
                if (parameter is Visibility)
                {
                    return parameter;
                }
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility && targetType == typeof(bool))
            {
                var val = (Visibility)value;
                if (val == Visibility.Visible)
                {
                    return true;
                }
                return false;
            }
            throw new ArgumentException("Invalid argument/return type. Expected argument: Visibility and return type: bool");
        }

        #endregion
    }
}