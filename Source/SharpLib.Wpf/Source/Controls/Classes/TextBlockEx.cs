using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class TextBlockEx : Control
    {
        #region Поля

        public static readonly DependencyProperty TextProperty;

        #endregion

        #region Свойства

        [Category("SharpLib")]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region Конструктор

        static TextBlockEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBlockEx), new FrameworkPropertyMetadata(typeof(TextBlockEx)));

            // Установка вертикального выравнивания текста по умолчанию
            VerticalAlignmentProperty.OverrideMetadata(typeof(TextBlockEx), new FrameworkPropertyMetadata(VerticalAlignment.Center));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextBlockEx), new PropertyMetadata("TextBlockEx"));
        }

        #endregion
    }
}