using System;
using System.Windows.Data;

using SharpLib.Docking.Controls;
using SharpLib.Docking;

namespace SharpLib.Docking.Converters
{
    public class HideCommandLayoutItemFromLayoutModelConverter : IValueConverter
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

            var layoutItemModel = layoutModel.Root.Manager.GetLayoutItemFromModel(layoutModel) as LayoutAnchorableItem;
            if (layoutItemModel == null)
            {
                return Binding.DoNothing;
            }

            return layoutItemModel.HideCommand;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}