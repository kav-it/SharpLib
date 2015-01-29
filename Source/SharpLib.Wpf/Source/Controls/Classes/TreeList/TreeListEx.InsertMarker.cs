using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class TreeListExInsertMarker : Control
    {
        #region Конструктор

        static TreeListExInsertMarker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListExInsertMarker), new FrameworkPropertyMetadata(typeof(TreeListExInsertMarker)));
        }

        #endregion
    }
}