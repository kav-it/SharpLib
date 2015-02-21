using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using SharpLib.Docking;

namespace SharpLib.Docking.Controls
{
    public class LayoutAnchorableControl : Control
    {
        #region Поля

        private static readonly DependencyPropertyKey _layoutItemPropertyKey;

        public static readonly DependencyProperty LayoutItemProperty;

        public static readonly DependencyProperty ModelProperty;

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

        static LayoutAnchorableControl()
        {
            _layoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata((LayoutItem)null));
            ModelProperty = DependencyProperty.Register("Model", typeof(LayoutAnchorable), typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(null, OnModelChanged));
            LayoutItemProperty = _layoutItemPropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorableControl)));
            FocusableProperty.OverrideMetadata(typeof(LayoutAnchorableControl), new FrameworkPropertyMetadata(false));
        }

        #endregion

        #region Методы

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableControl)d).OnModelChanged(e);
        }

        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            SetLayoutItem(Model != null ? Model.Root.Manager.GetLayoutItemFromModel(Model) : null);
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(_layoutItemPropertyKey, value);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (Model != null)
            {
                Model.IsActive = true;
            }

            base.OnGotKeyboardFocus(e);
        }

        #endregion
    }
}