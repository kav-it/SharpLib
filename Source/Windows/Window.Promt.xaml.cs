//*****************************************************************************
//
// Имя файла    : 'Window.Promt.cs'
// Заголовок    : Окно "Ввод текстового значения или выбора из списка вариантов"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 06/07/2012
//
//*****************************************************************************

using System;
using System.Windows;
using System.Windows.Input;

namespace SharpLib
{
    public partial class WindowPromt : Window
    {
        #region Свойства

        public String Text
        {
            get { return PART_textBox.Text; }
            set
            {
                PART_textBox.Text = value;
                PART_textBox.SelectionStart = 0;
                PART_textBox.SelectionLength = PART_textBox.Text.Length;
            }
        }

        #endregion

        #region Конструктор

        public WindowPromt(String title, Boolean isCombo = false)
        {
            InitializeComponent();

            Owner = Program.CurrentWindow;
            ShowInTaskbar = false;
            Title = title;

            if (isCombo == false)
            {
                PART_comboBox.Visibility = Visibility.Collapsed;
                PART_textBox.Focus();
            }
            else
            {
                PART_textBox.Visibility = Visibility.Collapsed;
                PART_comboBox.Focus();
            }
        }

        #endregion

        #region Обработка нажатий

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                DialogResult = true;
        }

        #endregion Обработка нажатий
    }
}