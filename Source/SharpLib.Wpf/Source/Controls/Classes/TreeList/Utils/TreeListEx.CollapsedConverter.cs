using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace SharpLib.Wpf.Controls
{
    public class TreeListExCollapsedConverter : MarkupExtension, IValueConverter
    {
        #region Поля

        public static TreeListExCollapsedConverter _instance = new TreeListExCollapsedConverter();

        #endregion

        #region Методы

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}