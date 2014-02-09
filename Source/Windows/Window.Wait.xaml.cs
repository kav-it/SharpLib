//*****************************************************************************
//
// Имя файла    : 'Window.Wait.xaml.cs'
// Заголовок    : Окно "Ожидание..."
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 09/10/2012
//
//*****************************************************************************

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SharpLib
{

    #region Класс WindowWait

    public partial class WindowWait : Window
    {
        #region Поля

        private static int _curr;

        private static int _max;

        private static WindowWaitModel _model;

        private static Boolean _splashActive;

        private static DispatcherTimer _timerUpdateToUI;

        #endregion

        #region Свойства

        public static int Percent
        {
            get { return _model.Percent; }
            set { _model.Percent = value; }
        }

        public static int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public static int Curr
        {
            get { return _curr; }
            set
            {
                if (_curr != value)
                {
                    if (value < _max)
                    {
                        _curr = value;
                        Percent = (_curr * 100) / _max;
                    }
                }
            }
        }

        public static String Text
        {
            get { return _model.Text; }
            set { _model.Text = value; }
        }

        #endregion

        #region Конструктор

        static WindowWait()
        {
            _splashActive = false;
            _model = new WindowWaitModel();
        }

        public WindowWait()
        {
            InitializeComponent();

            _timerUpdateToUI = new DispatcherTimer();
        }

        #endregion

        #region Загрузка/Выгрузка окна

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PART_waitIndicator.Start();
            _timerUpdateToUI.Interval = new TimeSpan(0, 0, 0, 0, 50);
            _timerUpdateToUI.Tick += OnTimer;
            _timerUpdateToUI.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _timerUpdateToUI.Stop();
            _timerUpdateToUI.Tick -= OnTimer;
        }

        #endregion Загрузка/Выгрузка окна

        #region Показать/Скрыть окно

        public static void Show(String text)
        {
            if (_splashActive == false)
            {
                _splashActive = true;
                Thread thread = new Thread(() =>
                {
                    WindowWait wndSplash = new WindowWait();
                    wndSplash.DataContext = _model;
                    wndSplash.Title = "Подождите пожалуйста";

                    Text = text;
                    Percent = -1;

                    wndSplash.ShowDialog();

                    wndSplash.Closed += (sender1, e1) => wndSplash.Dispatcher.InvokeShutdown();
                    System.Windows.Threading.Dispatcher.Run();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();

                // Пауза для отображения окна (если вызов был выполнен не из GUI-потока)
                Thread.Sleep(100);
            }
        }

        public new static void Hide()
        {
            _splashActive = false;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            if (_splashActive == false)
                Close();
        }

        #endregion Показать/Скрыть окно
    }

    #endregion Класс WindowWait

    #region Класс WindowWaitModel

    public class WindowWaitModel : ModuleNotifyBase
    {
        #region Поля

        private int _percent;

        private String _text;

        #endregion

        #region Свойства

        public int Percent
        {
            get { return _percent; }
            set
            {
                _percent = value;
                OnPropertyChanged("Percent");
            }
        }

        public String Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        #endregion

        #region Конструктор

        public WindowWaitModel()
        {
            Percent = -1;
            Text = "Ожидание...";
        }

        #endregion
    }

    #endregion Класс WindowWaitModel
}