using System;
using System.Windows.Data;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Converters
{
    public class ActivateCommandLayoutItemFromLayoutModelConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var layoutModel = value as LayoutContent;
            if (layoutModel == null)
            {
                return null;
            }
            if (layoutModel.Root == null)
            {
                return null;
            }
            if (layoutModel.Root.Manager == null)
            {
                return null;
            }

            var layoutItemModel = layoutModel.Root.Manager.GetLayoutItemFromModel(layoutModel);
            if (layoutItemModel == null)
            {
                return Binding.DoNothing;
            }

            return layoutItemModel.ActivateCommand;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}