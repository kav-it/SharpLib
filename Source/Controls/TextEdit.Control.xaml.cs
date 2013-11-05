//*****************************************************************************
//
// Имя файла    : 'TextEdit.Control.cs'
// Заголовок    : Элемент TextBox с фильтрацией содержимого
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 26/06/2012
//
//*****************************************************************************
			
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;

namespace SharpLib
{
    #region Типы фильтров элемента "TextEdit"
    public enum TextEditFilter
    {
        None                = 0,
        Digits              = 1,
        Letters             = 2,
        DigitsAndLetter     = 3,
        Hex                 = 4,
        Mask                = 5,
        Password            = 6
    }
    #endregion Типы фильтров элемента "TextEdit"

    #region Делегат TextEditChangedEventHandler
    public delegate void TextEditChangedEventHandler(object sender, TextEditChangeEventArg e);
    #endregion Делегат TextEditChangedEventHandler

    #region Класс TextEdit
    public partial class TextEdit : UserControl
    {
        #region Поля
        #endregion Поля

        #region Свойства
        [Browsable(true), Category("Common"), Description("Текстовое значение")]
        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        [Browsable(true), Category("Common"), Description("Подпись курсивом, когда текст отсутсвует")]
        public String Watermark
        {
            get { return (String)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
        public TextEditFilter Filter
        {
            get { return (TextEditFilter)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }
        public int MaxLength
        {
            get { return PART_textBox.MaxLength; }
            set { PART_textBox.MaxLength = value; }
        }
        public CharacterCasing CharacterCasing
        {
            get { return PART_textBox.CharacterCasing; }
            set { PART_textBox.CharacterCasing = value; }
        }
        public Boolean IsEnabledFlat
        {
            get
            {
                return (PART_textBox.Style != null);
            }
            set
            {
                if (value == false)
                    PART_textBox.Style = null;
                else
                    PART_textBox.Style = (Style)this.FindResource("textBoxFlatStyle");
            }
        }
        public String PasswordText
        {
            get { return PART_password.Password; }
        }
        public Boolean IsBlank
        {
            get { return (Boolean)GetValue(IsBlankProperty); }
            set { SetValue(IsBlankProperty, value); }
        }
        public TextAlignment TextAlignment
        {
            get { return PART_textBox.TextAlignment;  }
            set { PART_textBox.TextAlignment = value; }
        }
        #endregion Свойства

        #region Свойства зависимости
        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty WatermarkProperty;
        public static readonly DependencyProperty FilterProperty;
        public static readonly DependencyProperty IsBlankProperty;
        #endregion Свойства зависимости

        #region События
        public event TextEditChangedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }
        #endregion События

        #region Маршрутизируемые события
        public static readonly RoutedEvent TextChangedEvent;
        #endregion Маршрутизируемые события

        #region Конструктор
        static TextEdit()
        {
            TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(TextEdit));
            WatermarkProperty = DependencyProperty.Register("Watermark", typeof(String), typeof(TextEdit));
            FilterProperty = DependencyProperty.Register("Filter", typeof(TextEditFilter), typeof(TextEdit),
                new PropertyMetadata(TextEditFilter.None));
            IsBlankProperty = DependencyProperty.Register("IsBlank", typeof(Boolean), typeof(TextEdit), new PropertyMetadata(true));
            TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged", RoutingStrategy.Bubble, typeof(TextEditChangedEventHandler), typeof(TextEdit));
        }
        public TextEdit()
        {
            InitializeComponent();

            // Установка обработчиков 
            // Ввод текста
            PART_textBox.PreviewTextInput += new TextCompositionEventHandler(TextBoxPreviewTextInput);
            // Копирование "Paste"
            PART_textBox.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(TextBoxPasting));
            PART_textBox.TextChanged += new TextChangedEventHandler(PART_textBox_TextChanged);
            // Обработка изменения поля Password
            PART_password.PasswordChanged += new RoutedEventHandler(PART_password_PasswordChanged);
        }

        #endregion Конструктор

        #region Методы
        /// <summary>
        /// Расчет состояния "IsBlank"
        /// </summary>
        private void OnChangeText()
        {
            String text;

            if (Filter == TextEditFilter.Password)
                text = PasswordText;
            else
                text = Text;

            IsBlank = String.IsNullOrEmpty(text);

            // Генерация события "Смена текста"
            RaiseEvent(new TextEditChangeEventArg(TextEdit.TextChangedEvent, text));
        }
        /// <summary>
        /// Обработка события "Изменение пароля"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            OnChangeText();
        }
        /// <summary>
        /// Обработка события "Изменение текста"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PART_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnChangeText();
        }
        /// <summary>
        /// Обработка события "Ввод текста"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxPreviewTextInput(Object sender, TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            e.Handled = FilterPreviewTextInput(e.Text);
        }
        /// <summary>
        /// Обработка события "'Paste' текста"
        /// </summary>
        private void TextBoxPasting(Object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));

                if (FilterPreviewTextInput(text) == true)
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        /// <summary>
        /// Фильтрация вводимого текста
        /// </summary>
        /// <param name="text">Анализируемый текст</param>
        /// <returns>true - фильтр сработал, строка НЕ соответствует формату</returns>
        private Boolean FilterPreviewTextInput(String text)
        {
            // Фильтр не установлен
            if (Filter == TextEditFilter.None) return false;

            foreach (Char ch in text)
            {
                switch (Filter)
                {
                    case TextEditFilter.Digits:
                        if (Char.IsDigit(ch) == false)
                        {
                            return true;
                        }
                        break;

                    case TextEditFilter.Letters:
                        if (Char.IsLetter(ch) == false)
                        {
                            return true;
                        }
                        break;

                    case TextEditFilter.DigitsAndLetter:
                        if (Char.IsLetterOrDigit(ch) == false)
                        {
                            return true;
                        }
                        break;

                    case TextEditFilter.Hex:
                        {
                            if (Char.IsDigit(ch) == false)
                            {
                                Char c = Char.ToLower(ch);

                                if ((c < 'a') || (c > 'f'))
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                } // end switch (анализ символа)
            } // end for (перебор всех символов в строке)

            // Фильтр не сработал: Строка соответствует формату
            return false;
        }
        /// <summary>
        /// Установка фокуса в поле ввода
        /// </summary>
        /// <returns></returns>
        public new Boolean Focus()
        {
            return PART_textBox.Focus();
        }
        #endregion Методы
    }
    #endregion Класс TextEdit

    #region Класс PasswordVisibleConverter
    public class PasswordVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TextEditFilter filter = (TextEditFilter)value;
            String param;

            if (parameter == null)
                param = "0";
            else
                param = (String)parameter;

            if (param == "0")
            {
                if (filter == TextEditFilter.Password)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
            else 
            {
                if (filter == TextEditFilter.Password)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    #endregion Класс PasswordVisibleConverter

    #region Класс HeightMinusConverter
    public class HeightMinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((double)value >= 6)
                return (double)value - 6;
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    #endregion Класс HeightMinusConverter

    #region Класс TextEditChangeEventArg
    public class TextEditChangeEventArg: RoutedEventArgs
    {
        #region Поля
        public String Text { get; set; }
        #endregion Поля

        #region Конструктор
        public TextEditChangeEventArg(RoutedEvent id, String text): base(id)
        {
            Text = text;
        }
        #endregion Конструктор
    }
    #endregion Класс TextEditChangeEventArg
}
