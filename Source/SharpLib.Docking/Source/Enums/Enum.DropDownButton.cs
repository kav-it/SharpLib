using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SharpLib.Docking.Controls
{
    public class DropDownButton : ToggleButton
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

        static DropDownButton()
        {
            DropDownContextMenuDataContextProperty = DependencyProperty.Register("DropDownContextMenuDataContext", typeof(object), typeof(DropDownButton), new FrameworkPropertyMetadata((object)null));
            DropDownContextMenuProperty = DependencyProperty.Register("DropDownContextMenu", typeof(ContextMenu), typeof(DropDownButton),
                new FrameworkPropertyMetadata(null, OnDropDownContextMenuChanged));
        }

        public DropDownButton()
        {
            Unloaded += DropDownButton_Unloaded;
        }

        #endregion

        #region Методы

        private static void OnDropDownContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DropDownButton)d).OnDropDownContextMenuChanged(e);
        }

        protected virtual void OnDropDownContextMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldContextMenu = e.OldValue as ContextMenu;
            if (oldContextMenu != null && IsChecked.GetValueOrDefault())
            {
                oldContextMenu.Closed -= OnContextMenuClosed;
            }
        }

        protected override void OnClick()
        {
            if (DropDownContextMenu != null)
            {
                DropDownContextMenu.PlacementTarget = this;
                DropDownContextMenu.Placement = PlacementMode.Bottom;
                DropDownContextMenu.DataContext = DropDownContextMenuDataContext;
                DropDownContextMenu.Closed += OnContextMenuClosed;
                DropDownContextMenu.IsOpen = true;
            }

            base.OnClick();
        }

        private void OnContextMenuClosed(object sender, RoutedEventArgs e)
        {
            var ctxMenu = sender as ContextMenu;
            if (ctxMenu != null)
            {
                ctxMenu.Closed -= OnContextMenuClosed;
            }
            IsChecked = false;
        }

        private void DropDownButton_Unloaded(object sender, RoutedEventArgs e)
        {
            DropDownContextMenu = null;
        }

        #endregion
    }
}