using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SharpLib.Texter.Search
{
    public class DropDownButton : ButtonBase
    {
        #region Поля

        public static readonly DependencyProperty DropDownContentProperty;

        public static readonly DependencyProperty IsDropDownContentOpenProperty;

        protected static readonly DependencyPropertyKey IsDropDownContentOpenPropertyKey;

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
            DropDownContentProperty = DependencyProperty.Register("DropDownContent", typeof(Popup), typeof(DropDownButton), new FrameworkPropertyMetadata(null));
            IsDropDownContentOpenPropertyKey = DependencyProperty.RegisterReadOnly("IsDropDownContentOpen", typeof(bool), typeof(DropDownButton), new FrameworkPropertyMetadata(false));
            IsDropDownContentOpenProperty = IsDropDownContentOpenPropertyKey.DependencyProperty;

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