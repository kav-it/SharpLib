using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class TextBoxEx : TextBox
    {
        #region Поля

        public static readonly DependencyProperty WatermarkTextProperty;

        #endregion

        #region Свойства

        [Category("SharpLib")]
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }

        #endregion

        #region Конструктор

        static TextBoxEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxEx), new FrameworkPropertyMetadata(typeof(TextBoxEx)));

            // Установка вертикального выравнивания текста по умолчанию
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(TextBoxEx), new FrameworkPropertyMetadata(VerticalAlignment.Center));

            WatermarkTextProperty = DependencyProperty.Register("WatermarkText", typeof(string), typeof(TextBoxEx), new PropertyMetadata(null));
        }

        #endregion

        #region Методы

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion
    }
}