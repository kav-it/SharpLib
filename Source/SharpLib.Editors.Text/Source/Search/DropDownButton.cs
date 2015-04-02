using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SharpLib.Notepad.Search
{
    public class DropDownButton : ButtonBase
    {
        #region Поля

        public static readonly DependencyProperty DropDownContentProperty
            = DependencyProperty.Register("DropDownContent", typeof(Popup),
                typeof(DropDownButton), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty IsDropDownContentOpenProperty = IsDropDownContentOpenPropertyKey.DependencyProperty;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        protected static readonly DependencyPropertyKey IsDropDownContentOpenPropertyKey
            = DependencyProperty.RegisterReadOnly("IsDropDownContentOpen", typeof(bool),
                typeof(DropDownButton), new FrameworkPropertyMetadata(false));

        #endregion

        #region Свойства

        public Popup DropDownContent
        {
            get { return (Popup)GetValue(DropDownContentProperty); }
            set { SetValue(DropDownContentProperty, value); }
        }

        public bool IsDropDownContentOpen
        {
            get { return (bool)GetValue(IsDropDownContentOpenProperty); }
            protected set { SetValue(IsDropDownContentOpenPropertyKey, value); }
        }

        #endregion

        #region Конструктор

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        #endregion

        #region Методы

        protected override void OnClick()
        {
            if (DropDownContent != null && !IsDropDownContentOpen)
            {
                DropDownContent.Placement = PlacementMode.Bottom;
                DropDownContent.PlacementTarget = this;
                DropDownContent.IsOpen = true;
                DropDownContent.Closed += DropDownContent_Closed;
                IsDropDownContentOpen = true;
            }
        }

        private void DropDownContent_Closed(object sender, EventArgs e)
        {
            ((Popup)sender).Closed -= DropDownContent_Closed;
            IsDropDownContentOpen = false;
        }

        #endregion
    }
}