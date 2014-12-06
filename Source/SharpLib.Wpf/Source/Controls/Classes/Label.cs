using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class Label : Control
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

        static Label()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Label), new FrameworkPropertyMetadata(typeof(Label)));

            // Установка вертикального выравнивания текста по умолчанию
            VerticalAlignmentProperty.OverrideMetadata(typeof(Label), new FrameworkPropertyMetadata(VerticalAlignment.Center));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Label), new PropertyMetadata("Label"));
        }

        #endregion
    }
}