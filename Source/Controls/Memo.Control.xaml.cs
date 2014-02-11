// ****************************************************************************
//
// Имя файла    : 'Memo.Control.cs'
// Заголовок    : Компонент "Memo" (для отображения логи, процессов выполнения)
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 11/01/2013
//
// ****************************************************************************

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpLib
{

    #region Класс Memo

    public partial class Memo : UserControl
    {
        #region Поля

        private MemoContextMenu _contextMenu;

        private ScrollViewer _richTextBoxScrollViewer;

        #endregion

        #region Свойства

        public TimeStampFormat TimeStampFormat { get; set; }

        public Boolean AutoScroll { get; set; }

        public String Text
        {
            get { return GetText(); }
            set { SetText(value); }
        }

        public Boolean IsReadOnly
        {
            get { return PART_richTextBox.IsReadOnly; }
            set { PART_richTextBox.IsReadOnly = value; }
        }

        #endregion

        #region Конструктор

        public Memo()
        {
            InitializeComponent();

            // Инициализация переменных
            TimeStampFormat = TimeStampFormat.TimeMSec;
            // Настройка контекстного меню
            _contextMenu = new MemoContextMenu(this);
            AutoScroll = true;
            _richTextBoxScrollViewer = null;

            // Условие для выключения переноса строк
            PART_flowDoc.PageWidth = 2000;

            // Настройка после инициализации
            PART_richTextBox.ApplyTemplate();
            PART_richTextBox.Loaded += OnRichBoxLoaded;
            PART_richTextBox.MouseRightButtonUp += OnRichBoxMouseRightButtonUp;
        }

        #endregion

        #region Обработчики событий

        private void OnRichBoxLoaded(object sender, RoutedEventArgs e)
        {
            _richTextBoxScrollViewer = (ScrollViewer)VisualTree.FindDown(typeof(ScrollViewer), "PART_ContentHost", PART_richTextBox);
        }

        private void OnRichBoxMouseRightButtonUp(Object sender, MouseButtonEventArgs e)
        {
            _contextMenu.IsOpen = true;
        }

        #endregion Обработчики событий

        #region Вспомогательные методы

        private String GetText()
        {
            String text = new TextRange(PART_flowDoc.ContentStart, PART_flowDoc.ContentEnd).Text;

            return text;
        }

        private void SetText(String value)
        {
            PART_flowDoc.Blocks.Clear();
            PART_flowDoc.Blocks.Add(new Paragraph(new Run(value)));
        }

        private String AddTimeStamp(String text)
        {
            if (TimeStampFormat != TimeStampFormat.None)
                text = String.Format("[{0}] {1}", Time.NowToStr(TimeStampFormat), text);

            return text;
        }

        private void ScrollToEnd()
        {
            if (_richTextBoxScrollViewer != null)
                _richTextBoxScrollViewer.ScrollToEnd();
        }

        private void AddLineSafety(String text, Brush color)
        {
            Run run = new Run(text);
            run.Foreground = color;

            Paragraph paragraph = new Paragraph(run);
            paragraph.Margin = new Thickness(0);

            PART_flowDoc.Blocks.Add(paragraph);

            if (AutoScroll)
                ScrollToEnd();
        }

        public void AddTextSafety(String text, Brush color)
        {
            TextRange textRange = new TextRange(PART_flowDoc.ContentEnd, PART_flowDoc.ContentEnd);
            textRange.Text = text;
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }

        #endregion Вспомогательные методы

        #region Основные методы

        public void SelectAll()
        {
            PART_richTextBox.SelectAll();
        }

        public void Clear()
        {
            PART_flowDoc.Blocks.Clear();
        }

        public void AddLine(String text)
        {
            AddLine(text, Brushes.Black);
        }

        public void AddLineGreen(String text)
        {
            AddLine(text, Brushes.Green);
        }

        public void AddLineRed(String text)
        {
            AddLine(text, Brushes.Red);
        }

        public void AddLine(String text, Brush color)
        {
            text = AddTimeStamp(text);

            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke((Action)(() => { AddLineSafety(text, color); }));
        }

        public new void AddText(String text)
        {
            AddText(text, Brushes.Black);
        }

        public void AddTextRed(String text)
        {
            AddText(text, Brushes.Red);
        }

        public void AddText(String text, Brush color)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.BeginInvoke((Action)(() => { AddTextSafety(text, color); }));
        }

        #endregion Основные методы
    }

    #endregion Класс Memo

    #region Класс MemoContextMenu

    public class MemoContextMenu : ContextMenuBase
    {
        #region Поля

        private Memo _memo;

        #endregion

        #region Конструктор

        public MemoContextMenu(Memo memo)
        {
            _memo = memo;

            MenuItemBase itemClear = new MenuItemBase("Очистить", OnClear, null);
            MenuItemBase itemSelectAll = new MenuItemBase("Выделить все", OnSelectAll, null);
            MenuItemBase itemAutoScroll = new MenuItemBase("Автоскролл", OnAutoScroll, null);
            itemAutoScroll.IsCheckable = true;
            itemAutoScroll.IsChecked = true;

            Add(itemClear);
            Add(itemSelectAll);
            AddSeparator();
            Add(itemAutoScroll);
        }

        #endregion

        #region Методы

        private void OnClear(Object sender, RoutedEventArgs e)
        {
            _memo.Clear();
        }

        private void OnSelectAll(Object sender, RoutedEventArgs e)
        {
            _memo.SelectAll();
        }

        private void OnAutoScroll(Object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (item != null)
                _memo.AutoScroll = item.IsChecked;
        }

        #endregion
    }

    #endregion Класс MemoContextMenu
}