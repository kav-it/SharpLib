using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    public class LayoutAnchorableTabItem : Control
    {
        #region Поля

        public static readonly DependencyProperty LayoutItemProperty;

        public static readonly DependencyProperty ModelProperty;

        private static readonly DependencyPropertyKey _layoutItemPropertyKey;

        private static LayoutAnchorableTabItem _draggingItem;

        private bool _isMouseDown;

        #endregion

        #region Свойства

        public LayoutContent Model
        {
            get { return (LayoutContent)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public LayoutItem LayoutItem
        {
            get { return (LayoutItem)GetValue(LayoutItemProperty); }
        }

        #endregion

        #region Конструктор

        static LayoutAnchorableTabItem()
        {
            _layoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(LayoutAnchorableTabItem), new FrameworkPropertyMetadata((LayoutItem)null));
            ModelProperty = DependencyProperty.Register("Model", typeof(LayoutContent), typeof(LayoutAnchorableTabItem), new FrameworkPropertyMetadata(null, OnModelChanged));
            LayoutItemProperty = _layoutItemPropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorableTabItem), new FrameworkPropertyMetadata(typeof(LayoutAnchorableTabItem)));
        }

        #endregion

        #region Методы

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableTabItem)d).OnModelChanged(e);
        }

        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            SetLayoutItem(Model != null ? Model.Root.Manager.GetLayoutItemFromModel(Model) : null);
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(_layoutItemPropertyKey, value);
        }

        internal static bool IsDraggingItem()
        {
            return _draggingItem != null;
        }

        internal static LayoutAnchorableTabItem GetDraggingItem()
        {
            return _draggingItem;
        }

        internal static void ResetDraggingItem()
        {
            _draggingItem = null;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _isMouseDown = true;
            _draggingItem = this;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                _isMouseDown = false;
                _draggingItem = null;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isMouseDown = false;

            base.OnMouseLeftButtonUp(e);

            Model.IsActive = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                _draggingItem = this;
            }

            _isMouseDown = false;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (_draggingItem == null || _draggingItem == this || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            var model = Model;
            var container = model.Parent;
            var containerPane = model.Parent as ILayoutPane;
            var childrenList = container.Children.ToList();
            if (containerPane != null)
            {
                containerPane.MoveChild(childrenList.IndexOf(_draggingItem.Model), childrenList.IndexOf(model));
            }
        }

        #endregion
    }
}