using System;
using System.Globalization;
using System.Windows.Data;

namespace SharpLib.Wpf.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class StringToBoolConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "True".Equals(value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "True".Equals(value.ToString(), StringComparison.OrdinalIgnoreCase) ? "True" : "False";
        }

        #endregion
    }
}