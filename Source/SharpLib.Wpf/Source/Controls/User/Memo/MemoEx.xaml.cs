using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    public partial class MemoEx
    {
        #region Поля

        private readonly MemoExContextMenu _contextMenu;

        private ScrollViewer _richTextBoxScrollViewer;

        #endregion

        #region Свойства

        [Category("SharpLib")]
        [DefaultValue(TimeStampConstants.FORMAT_LOG_SHORT)]
        [Description("Формат вывода штампа времени")]
        public string TimeStampFormat { get; set; }

        [Category("SharpLib")]
        [DefaultValue(true)]
        [Description("Автоматическая прокрутка вниз при добавлении записи")]
        public Boolean AutoScroll { get; set; }

        [Category("SharpLib")]
        [DefaultValue("")]
        [Description("Весь текст элемента")]
        public string Text
        {
            get { return GetText(); }
            set { SetText(value); }
        }

        [Category("SharpLib")]
        [DefaultValue(false)]
        [Description("Разрешение редактирования")]
        public Boolean IsReadOnly
        {
            get { return PART_richTextBox.IsReadOnly; }
            set { PART_richTextBox.IsReadOnly = value; }
        }

        #endregion

        #region Конструктор

        public MemoEx()
        {
            InitializeComponent();

            TimeStampFormat = TimeStampConstants.FORMAT_LOG_SHORT;
            IsReadOnly = true;
            AutoScroll = true;

            _contextMenu = new MemoExContextMenu(this);
            _richTextBoxScrollViewer = null;

            PART_flowDoc.PageWidth = 2000;
            PART_richTextBox.ApplyTemplate();
            PART_richTextBox.Loaded += OnRichBoxLoaded;
            PART_richTextBox.MouseRightButtonUp += OnRichBoxMouseRightButtonUp;
        }

        #endregion

        #region Методы

        private void OnRichBoxLoaded(object sender, RoutedEventArgs e)
        {
            _richTextBoxScrollViewer = (ScrollViewer)VisualTree.FindDown(typeof(ScrollViewer), "PART_ContentHost", PART_richTextBox);
        }

        private void OnRichBoxMouseRightButtonUp(Object sender, MouseButtonEventArgs e)
        {
            _contextMenu.IsOpen = true;
        }

        private string GetText()
        {
            string text = new TextRange(PART_flowDoc.ContentStart, PART_flowDoc.ContentEnd).Text;

            return text;
        }

        private void SetText(string value)
        {
            PART_flowDoc.Blocks.Clear();

            if (value.IsValid())
            {
                PART_flowDoc.Blocks.Add(new Paragraph(new Run(value)));                
            }
        }

        private string AddTimeStamp(string text)
        {
            if (TimeStampFormat.IsValid())
            {
                var time = DateTime.Now.ToString(TimeStampFormat);
                text = string.Format("[{0}] {1}", time, text);
            }

            return text;
        }

        private void ScrollToEnd()
        {
            if (_richTextBoxScrollViewer != null)
            {
                _richTextBoxScrollViewer.ScrollToEnd();
            }
        }

        private void AddLineSafety(string text, Brush color)
        {
            Run run = new Run(text);
            run.Foreground = color;

            Paragraph paragraph = new Paragraph(run);
            paragraph.Margin = new Thickness(0);

            PART_flowDoc.Blocks.Add(paragraph);

            if (AutoScroll)
            {
                ScrollToEnd();
            }
        }

        public void AddTextSafety(string text, Brush color)
        {
            TextRange textRange = new TextRange(PART_flowDoc.ContentEnd, PART_flowDoc.ContentEnd);
            textRange.Text = text;
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }

        public void SelectAll()
        {
            PART_richTextBox.SelectAll();
        }

        public void Clear()
        {
            PART_flowDoc.Blocks.Clear();
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
            text = AddTimeStamp(text);

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() => AddLineSafety(text, color)));
            }
        }

        public new void AddText(string text)
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
                Application.Current.Dispatcher.BeginInvoke((Action)(() => AddTextSafety(text, color)));
            }
        }

        #endregion
    }
}