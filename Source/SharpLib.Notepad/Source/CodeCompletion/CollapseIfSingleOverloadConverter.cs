using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SharpLib.Notepad.CodeCompletion
{
    internal sealed class CollapseIfSingleOverloadConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value < 2) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}