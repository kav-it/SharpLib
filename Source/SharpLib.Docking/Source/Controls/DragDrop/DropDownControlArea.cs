using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SharpLib.Docking.Controls
{
    public class DropDownControlArea : UserControl
    {
        #region Поля

        public static readonly DependencyProperty DropDownContextMenuDataContextProperty;

        public static readonly DependencyProperty DropDownContextMenuProperty;

        #endregion

        #region Свойства

        public ContextMenu DropDownContextMenu
        {
            get { return (ContextMenu)GetValue(DropDownContextMenuProperty); }
            set { SetValue(DropDownContextMenuProperty, value); }
        }

        public object DropDownContextMenuDataContext
        {
            get { return GetValue(DropDownContextMenuDataContextProperty); }
            set { SetValue(DropDownContextMenuDataContextProperty, value); }
        }

        #endregion

        #region Конструктор

        static DropDownControlArea()
        {
            DropDownContextMenuDataContextProperty = DependencyProperty.Register("DropDownContextMenuDataContext", typeof(object), typeof(DropDownControlArea),
                new FrameworkPropertyMetadata((object)null));
            DropDownContextMenuProperty = DependencyProperty.Register("DropDownContextMenu", typeof(ContextMenu), typeof(DropDownControlArea), new FrameworkPropertyMetadata((ContextMenu)null));
        }

        #endregion

        #region Методы

        protected override void OnPreviewMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);

            if (!e.Handled)
            {
                if (DropDownContextMenu != null)
                {
                    DropDownContextMenu.PlacementTarget = null;
                    DropDownContextMenu.Placement = PlacementMode.MousePoint;
                    DropDownContextMenu.DataContext = DropDownContextMenuDataContext;
                    DropDownContextMenu.IsOpen = true;
                }
            }
        }

        #endregion
    }
}