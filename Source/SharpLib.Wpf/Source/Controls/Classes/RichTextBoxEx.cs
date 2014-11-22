using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    [ContentProperty("Text")]
    public class RichTextBoxEx : RichTextBox
    {
        #region Поля

        public static readonly DependencyProperty TextProperty;

        private ScrollViewer _scrollViewer;

        #endregion

        #region Свойства

        [Category("SharpLib")]
        [DefaultValue(true)]
        public bool AutoScroll { get; set; }

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
            AutoScroll = true;

            Loaded += OnLoaded;
        }

        #endregion

        #region Методы

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _scrollViewer = (ScrollViewer)VisualTree.FindDown(typeof(ScrollViewer), "PART_ContentHost", this);
        }

        private static void OnTextPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as RichTextBoxEx;

            if (control != null)
            {
                control.SetText(control.Document, (string)args.NewValue);
            }
        }

        private void SetText(FlowDocument document, string text)
        {
            document.Blocks.Clear();
            document.Blocks.Add(new Paragraph(new Run(text)));
        }

        public void Clear()
        {
            Document.Blocks.Clear();
        }

        //private string AddTimeStamp(string text)
        //{
        //    if (TimeStampFormat != TimeStampFormat.None)
        //        text = string.Format("[{0}] {1}", Time.NowToStr(TimeStampFormat), text);

        //    return text;
        //}

        //private void ScrollToEnd()
        //{
        //    if (_scrollViewer != null)
        //        _scrollViewer.ScrollToEnd();
        //}

        private void AddLineUnsafe(string text, Brush color)
        {
            Run run = new Run(text);
            run.Foreground = color;

            Paragraph paragraph = new Paragraph(run);
            paragraph.Margin = new Thickness(0);

            Document.Blocks.Add(paragraph);

            if (AutoScroll)
            {
                ScrollToEnd();
            }
        }

        public void AddTextUnsafe(string text, Brush color)
        {
            TextRange textRange = new TextRange(Document.ContentEnd, Document.ContentEnd);
            textRange.Text = text;
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }

        public void AddLine(string text)
        {
            AddLine(text, Brushes.Black);
        }

        public void AddLineGreen(string text)
        {
            AddLine(text, Brushes.Green);
        }

        public void AddLineRed(string text)
        {
            AddLine(text, Brushes.Red);
        }

        public void AddLine(string text, Brush color)
        {
            // text = AddTimeStamp(text);

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() => AddLineUnsafe(text, color)));
            }
        }

        public void AddText(string text)
        {
            AddText(text, Brushes.Black);
        }

        public void AddTextRed(string text)
        {
            AddText(text, Brushes.Red);
        }

        public void AddText(string text, Brush color)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() => AddTextUnsafe(text, color)));
            }
        }

        #endregion
    }
}