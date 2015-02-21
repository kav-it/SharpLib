using System.Windows;

namespace SharpLib.Docking.Controls
{
    internal abstract class DropTargetBase : DependencyObject
    {
        #region Поля

        public static readonly DependencyProperty IsDraggingOverProperty;

        #endregion

        #region Конструктор

        static DropTargetBase()
        {
            IsDraggingOverProperty = DependencyProperty.RegisterAttached("IsDraggingOver", typeof(bool), typeof(DropTargetBase), new FrameworkPropertyMetadata(false));
        }

        #endregion

        #region Методы

        public static bool GetIsDraggingOver(DependencyObject d)
        {
            return (bool)d.GetValue(IsDraggingOverProperty);
        }

        public static void SetIsDraggingOver(DependencyObject d, bool value)
        {
            d.SetValue(IsDraggingOverProperty, value);
        }

        #endregion
    }
}