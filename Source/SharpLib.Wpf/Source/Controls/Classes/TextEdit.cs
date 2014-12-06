using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class TextEdit : TextBox
    {
        #region Поля

        public static readonly DependencyProperty WatermarkTextProperty;

        public static readonly DependencyProperty PasswordCharProperty;

        #endregion

        #region Свойства

        [Category("SharpLib")]
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }

        [Category("SharpLib")]
        public char PasswordChar
        {
            get { return (char)GetValue(PasswordCharProperty); }
            set { SetValue(PasswordCharProperty, value); }
        }

        #endregion

        #region Конструктор

        static TextEdit()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEdit), new FrameworkPropertyMetadata(typeof(TextEdit)));

            // Установка вертикального выравнивания текста по умолчанию
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(TextEdit), new FrameworkPropertyMetadata(VerticalAlignment.Center));

            WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(TextEdit), new PropertyMetadata(null));
            PasswordCharProperty = DependencyProperty.Register("PasswordChar", typeof(char), typeof(TextEdit), new PropertyMetadata(null));
        }

        #endregion
    }
}