// ****************************************************************************
//
// Имя файла    : 'Window.About.xaml.cs'
// Заголовок    : Окно "О программе"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 05/06/2012
//
// ****************************************************************************

using System.Windows;

namespace SharpLib
{
    public partial class WindowAbout : Window
    {
        #region Конструктор

        public WindowAbout()
        {
            InitializeComponent();

            Owner = Program.CurrentWindow;
            ShowInTaskbar = false;

            PART_image.Source = Program.Icon;
            textBlockApp.Text = ProgramBase.Title;
            textBlockVer.Text = "Версия: " + ProgramBase.Version;
            textBlockTime.Text = "Дата: " + ProgramBase.Version.DateTimeText;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обработка "Закрыть"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Обработка "Подробнее"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailClick(object sender, RoutedEventArgs e)
        {
            WindowHistory window = new WindowHistory();
            window.ShowDialog();
        }

        #endregion
    }
}