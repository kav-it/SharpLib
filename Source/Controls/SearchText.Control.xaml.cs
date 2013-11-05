//*****************************************************************************
//
// Имя файла    : 'SearchText.Control.cs'
// Заголовок    : Элемент TextBox с функцией поиска
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 28/09/2012
//
//*****************************************************************************
			
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;

namespace SharpLib
{
    #region Класс SearchText
    public partial class SearchText : UserControl
    {
        #region Конструктор
        public SearchText()
        {
            InitializeComponent();

            PART_textEdit.Watermark     = "Введите текст...";
            PART_textEdit.IsEnabledFlat = true;
        }
        #endregion Конструктор

        #region Методы
        /// <summary>
        /// Обработка нажатия Esc
        /// </summary>
        private void PART_textEdit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;

                PART_textEdit.Text = "";
            }
        }
        /// <summary>
        /// Очистка поля по нажатию на "Отмена"
        /// </summary>
        private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            PART_textEdit.Text = "";
        }
        #endregion Методы
    }
    #endregion Класс SearchText

    #region Класс SearchTextConverter
    public class SearchTextConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String text = (String)value;
            String param;

            if (parameter == null)
                param = "0";
            else
                param = (String)parameter;

            if (param == "0")
            {
                if (text != null && text != "")
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
            else
            {
                if (text != null && text != "")
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    #endregion Класс SearchTextConverter
}
