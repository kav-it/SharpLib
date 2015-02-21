using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    public class AnchorablePaneTitle : Control
    {
        #region Поля

        private static readonly DependencyPropertyKey _layoutItemPropertyKey;

        public static readonly DependencyProperty LayoutItemProperty;

        public static readonly DependencyProperty ModelProperty;

        private bool _isMouseDown;

        #endregion

        #region Свойства

        public LayoutAnchorable Model
        {
            get { return (LayoutAnchorable)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public LayoutItem LayoutItem
        {
            get { return (LayoutItem)GetValue(LayoutItemProperty); }
        }

        #endregion

        #region Конструктор

        static AnchorablePaneTitle()
        {
            _layoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(AnchorablePaneTitle),new FrameworkPropertyMetadata((LayoutItem)null));
            ModelProperty = DependencyProperty.Register("Model", typeof(LayoutAnchorable), typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(null, _OnModelChanged));
            LayoutItemProperty = _layoutItemPropertyKey.DependencyProperty;

            IsHitTestVisibleProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(true));
            FocusableProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(false));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(typeof(AnchorablePaneTitle)));
        }

        #endregion

        #region Методы

        private static void _OnModelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((AnchorablePaneTitle)sender).OnModelChanged(e);
        }

        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Model != null)
            {
                SetLayoutItem(Model.Root.Manager.GetLayoutItemFromModel(Model));
            }
            else
            {
                SetLayoutItem(null);
            }
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(_layoutItemPropertyKey, value);
        }

        private void OnHide()
        {
            Model.Hide();
        }

        private void OnToggleAutoHide()
        {
            Model.ToggleAutoHide();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                _isMouseDown = false;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                var pane = this.FindVisualAncestor<LayoutAnchorablePaneControl>();
                if (pane != null)
                {
                    var paneModel = pane.Model as LayoutAnchorablePane;
                    var manager = paneModel.Root.Manager;

                    manager.StartDraggingFloatingWindowForPane(paneModel);
                }
            }

            _isMouseDown = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!e.Handled)
            {
                bool attachFloatingWindow = false;
                var parentFloatingWindow = Model.FindParent<LayoutAnchorableFloatingWindow>();
                if (parentFloatingWindow != null)
                {
                    attachFloatingWindow = parentFloatingWindow.Descendents().OfType<LayoutAnchorablePane>().Count() == 1;
                }

                if (attachFloatingWindow)
                {
                    var floatingWndControl = Model.Root.Manager.FloatingWindows.Single(fwc => fwc.Model == parentFloatingWindow);
                    floatingWndControl.AttachDrag(false);
                }
                else
                {
                    _isMouseDown = true;
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            base.OnMouseLeftButtonUp(e);

            if (Model != null)
            {
                Model.IsActive = true;
            }
        }

        #endregion
    }
}