// ****************************************************************************
//
// Имя файла    : 'Window.History.xaml.cs'
// Заголовок    : Окно "История версий"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 05/06/2012
//
// ****************************************************************************

using System;
using System.Windows;

namespace SharpLib
{
    public partial class WindowHistory : Window
    {
        #region Константы

#if SIBER
        private const String HISTORY_PATH = "Source/history_sib.txt";
#else
        private const String HISTORY_PATH = "Source/history.txt";
#endif

        #endregion Константы

        #region Свойства

        /// <summary>
        /// История приложения (загружается в HistoryWindow)
        /// </summary>
        public static String History
        {
            get
            {
                String text = ResourcesWpf.LoadText(HISTORY_PATH);

                return text;
            }
        }

        #endregion Свойства

        #region Конструктор

        public WindowHistory()
        {
            InitializeComponent();

            Owner = Program.CurrentWindow;
            ShowInTaskbar = false;

            textBox1.Text = History;
        }

        #endregion Конструктор
    }
}