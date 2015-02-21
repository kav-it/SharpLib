using System.Windows;
using System.Windows.Controls;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    public class LayoutDocumentControl : Control
    {
        #region Поля

        public static readonly DependencyProperty LayoutItemProperty;

        public static readonly DependencyProperty ModelProperty;

        private static readonly DependencyPropertyKey _layoutItemPropertyKey;

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

        static LayoutDocumentControl()
        {
            _layoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(LayoutDocumentControl), new FrameworkPropertyMetadata((LayoutItem)null));
            ModelProperty = DependencyProperty.Register("Model", typeof(LayoutContent), typeof(LayoutDocumentControl), new FrameworkPropertyMetadata(null, OnModelChanged));
            LayoutItemProperty = _layoutItemPropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentControl), new FrameworkPropertyMetadata(typeof(LayoutDocumentControl)));
            FocusableProperty.OverrideMetadata(typeof(LayoutDocumentControl), new FrameworkPropertyMetadata(false));
        }

        #endregion

        #region Методы

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutDocumentControl)d).OnModelChanged(e);
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

        protected override void OnPreviewGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (Model != null)
            {
                Model.IsActive = true;
            }
            base.OnPreviewGotKeyboardFocus(e);
        }

        #endregion
    }
}