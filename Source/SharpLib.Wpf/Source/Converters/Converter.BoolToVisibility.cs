using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SharpLibWpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region Свойства

        public Visibility TrueValue { get; set; }

        public Visibility FalseValue { get; set; }

        #endregion

        #region Конструктор

        public BoolToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        #endregion

        #region Методы

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as bool?) == true ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility && (Visibility)value == TrueValue;
        }

        #endregion
    }
}