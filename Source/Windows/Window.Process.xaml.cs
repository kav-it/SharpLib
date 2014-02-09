// ****************************************************************************
//
// Имя файла    : 'Window.Process.cs'
// Заголовок    : Окно "Процесс"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 09/11/2012
//
// ****************************************************************************

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SharpLib
{

    #region Перечисление WindowProcessMessageTyp

    public enum WindowProcessMessageTyp
    {
        Error,

        Warning,

        Info,

        Blank
    }

    #endregion Перечисление

    #region Делегат WindowProcessCallback

    public delegate void WindowProcessCallback(WindowProcess window);

    #endregion Делегат WindowProcessCallback

    #region Класс WindowProcessMessage

    public class WindowProcessMessage
    {
        #region Свойства

        public WindowProcessMessageTyp Typ { get; set; }

        public DateTime Stamp { get; set; }

        public String Text { get; set; }

        public String Desc { get; set; }

        #endregion

        #region Конструктор

        public WindowProcessMessage(WindowProcessMessageTyp typ, String text, String desc)
        {
            Typ = typ;
            Stamp = DateTime.Now;
            Text = text;
            Desc = desc;
        }

        public WindowProcessMessage() : this(WindowProcessMessageTyp.Error, null, null)
        {
        }

        #endregion
    }

    #endregion Класс WindowProcessMessage

    #region Класс WindowProcess

    public partial class WindowProcess : Window
    {
        #region Поля

        private ObservableCollection<WindowProcessMessage> _messages;

        private WindowProcessCallback _onBreak;

        private WindowProcessCallback _onRun;

        private ModuleThreadBase _thread;

        #endregion

        #region Свойства

        private Boolean IsExecute
        {
            get { return (_thread.IsStop == false); }
            set
            {
                if (value == false)
                {
                    _thread.StopThread();
                    PART_buttonFinish.Content = "Закрыть";
                }
            }
        }

        public ObservableCollection<WindowProcessMessage> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        #endregion

        #region Конструктор

        public WindowProcess()
        {
            InitializeComponent();

            Owner = Program.CurrentWindow;

            _messages = new ObservableCollection<WindowProcessMessage>();

            PART_listView.ItemsSource = _messages;
        }

        #endregion

        #region Отображение окна

        public void ShowModal(WindowProcessCallback onRun, WindowProcessCallback onBreak)
        {
            _onRun = onRun;
            _onBreak = onBreak;

            ShowDialog();
        }

        #endregion Отображение окна

        #region Обработка запуска/завершения

        /// <summary>
        ////Окно загружено
        /// </summary>
        private void WindowLoaded(Object sender, RoutedEventArgs e)
        {
            // Создание потока
            _thread = new ModuleThreadBase("WindowProcess", OnThread);
            _thread.Finished += OnThreadFinished;
            _thread.StartThread();
        }

        /// <summary>
        /// Окно закрывается
        /// </summary>
        private void WindowClosed(Object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Основной цикл поток а выполнения
        /// </summary>
        private void OnThread()
        {
            if (_onRun != null)
                _onRun(this);
        }

        /// <summary>
        /// Поток закончил свое выполнение
        /// </summary>
        private void OnThreadFinished(object sender, object data)
        {
            IsExecute = false;
        }

        /// <summary>
        /// Обработка кнопки "Отмена/Закрыть"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFinishClick(object sender, RoutedEventArgs e)
        {
            if (IsExecute)
            {
                if (_onBreak != null)
                    _onBreak(this);

                IsExecute = false;

                AddWarning("Прервано пользователем");

                return;
            }

            Close();
        }

        #endregion Обработка запуска/завершения

        #region Добавление сообщений

        public void AddMessage(WindowProcessMessage message)
        {
            Application.Current.Dispatcher.BeginInvoke(
                                                       (Action)(() => { Messages.Add(message); })
                );
        }

        public void AddError(String text, String desc = null)
        {
            WindowProcessMessage message = new WindowProcessMessage(WindowProcessMessageTyp.Error, text, desc);

            AddMessage(message);
        }

        public void AddWarning(String text, String desc = null)
        {
            WindowProcessMessage message = new WindowProcessMessage(WindowProcessMessageTyp.Warning, text, desc);

            AddMessage(message);
        }

        public void AddInfo(String text, String desc = null)
        {
            WindowProcessMessage message = new WindowProcessMessage(WindowProcessMessageTyp.Info, text, desc);

            AddMessage(message);
        }

        public void AddBlank(String text, String desc = null)
        {
            WindowProcessMessage message = new WindowProcessMessage(WindowProcessMessageTyp.Blank, text, desc);

            AddMessage(message);
        }

        public Boolean AddQuestion(String text, String desc = null)
        {
            Boolean result = WindowMessageBox.ShowQuestion(text);

            WindowProcessMessage message = new WindowProcessMessage(WindowProcessMessageTyp.Blank, text, desc);
            AddMessage(message);

            text = result ? "Да" : "Нет";
            AddMessage(new WindowProcessMessage(WindowProcessMessageTyp.Blank, text, null));

            return result;
        }

        #endregion Добавление сообщений
    }

    #endregion Класс WindowProcess

    #region Класс WindowProcessMessageTypConverter

    public class WindowProcessMessageTypConverter : IValueConverter
    {
        #region Поля

        private ImageSource _sourceError;

        private ImageSource _sourceInfo;

        private ImageSource _sourceWarning;

        #endregion

        #region Конструктор

        public WindowProcessMessageTypConverter()
        {
            _sourceError = GuiImages.IconError.Source;
            _sourceWarning = GuiImages.IconWarning.Source;
            _sourceInfo = GuiImages.IconInformation.Source;
        }

        #endregion

        #region Методы

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            WindowProcessMessageTyp typ = (WindowProcessMessageTyp)value;

            switch (typ)
            {
                case WindowProcessMessageTyp.Warning:
                    return _sourceWarning;
                case WindowProcessMessageTyp.Info:
                    return _sourceInfo;
                case WindowProcessMessageTyp.Error:
                    return _sourceError;
            }

            return null;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс WindowProcessMessageTypConverter
}