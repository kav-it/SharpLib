//*****************************************************************************
//
// Имя файла    : 'TextLink.Control.cs'
// Заголовок    : Это файл предназначен для ...
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 20/06/2012
//
//*****************************************************************************
			
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace SharpLib
{
    #region Класс TextLink
    public partial class TextLink : UserControl
    {
        #region Свойства
        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public SolidColorBrush TextColor
        {
            get { return (SolidColorBrush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }
        #endregion Свойства

        #region Свойства зависимости
        public static DependencyProperty TextProperty;
        public static DependencyProperty TextColorProperty;
        #endregion Свойства зависимости

        #region События
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        #endregion

        #region Маршрутизируемые события
        public static readonly RoutedEvent ClickEvent;
        #endregion Маршрутизируемые события

        #region Конструктор
        static TextLink()
        {
            TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(TextLink), new PropertyMetadata(new PropertyChangedCallback(OnTextPropertyChanged)));
            TextColorProperty = DependencyProperty.Register("TextColor", typeof(SolidColorBrush), typeof(TextLink), new PropertyMetadata(new PropertyChangedCallback(OnTextColorPropertyChanged)));
            ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextLink));
        }
        public TextLink()
        {
            InitializeComponent();
        }
        #endregion Конструктор

        #region Методы
        /// <summary>
        /// Обработчик нажатия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(TextLink.ClickEvent, this));
        }
        private static void OnTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            String text = (String)e.NewValue;
            (sender as TextLink).SetText(text);
        }
        private static void OnTextColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SolidColorBrush brush = (SolidColorBrush)e.NewValue;
            (sender as TextLink).SetBrush(brush);
        }
        private void SetText(String text)
        {
            if (text != "")
                PART_textBox.Text = text;
        }
        private void SetBrush(SolidColorBrush brush)
        {
            PART_textBox.Foreground = brush;
            PART_hyperLink.Foreground = brush;
        }
        #endregion Методы
    }
    #endregion Класс TextLink
}
