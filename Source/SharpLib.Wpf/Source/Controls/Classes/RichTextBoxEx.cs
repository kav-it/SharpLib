using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace SharpLib.Wpf.Controls
{
    [ContentProperty("Text")]
    public class RichTextBoxEx : RichTextBox
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

        static RichTextBoxEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichTextBoxEx), new FrameworkPropertyMetadata(typeof(RichTextBoxEx)));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RichTextBoxEx), new PropertyMetadata(string.Empty, OnTextPropertyChangedCallback));
        }

        public RichTextBoxEx()
        {
            // Условие для выключения переноса строк
            Document.PageWidth = 2000;
            Document.Blocks.Clear();
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        #endregion

        #region Методы

        private static void OnTextPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as RichTextBoxEx;

            if (control != null)
            {
                control.SetText(control.Document, (string)args.NewValue);
            }
        }

        public void SetText(FlowDocument document, string text)
        {
            Document.Blocks.Clear();
            Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        #endregion
    }
}