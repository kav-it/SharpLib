//*****************************************************************************
//
// Имя файла    : 'WaitIndicator.Control.cs'
// Заголовок    : Компонент "Индикатор ожидания"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 28/09/2012
//
//*****************************************************************************

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SharpLib
{

    #region Класс WaitIndicator

    public partial class WaitIndicator : UserControl
    {
        #region Поля

        public static readonly DependencyProperty PercentProperty;

        #endregion

        #region Маршрутизируемые события

        public static readonly RoutedEvent OnPauseEvent;

        public static readonly RoutedEvent OnResumeEvent;

        public static readonly RoutedEvent OnStartEvent;

        public static readonly RoutedEvent OnStopEvent;

        #endregion Маршрутизируемые события

        #region Свойства

        /// <summary>
        /// Текущий процент выполнения (0-100)
        /// <para>Если Percent=-1, проценты не отображаются</para>
        /// </summary>
        public int Percent
        {
            get { return (int)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        #endregion

        #region События

        public event RoutedEventHandler OnPause
        {
            add { AddHandler(OnPauseEvent, value); }
            remove { RemoveHandler(OnPauseEvent, value); }
        }

        public event RoutedEventHandler OnResume
        {
            add { AddHandler(OnResumeEvent, value); }
            remove { RemoveHandler(OnResumeEvent, value); }
        }

        public event RoutedEventHandler OnStart
        {
            add { AddHandler(OnStartEvent, value); }
            remove { RemoveHandler(OnStartEvent, value); }
        }

        public event RoutedEventHandler OnStop
        {
            add { AddHandler(OnStopEvent, value); }
            remove { RemoveHandler(OnStopEvent, value); }
        }

        #endregion

        #region Конструктор

        static WaitIndicator()
        {
            PercentProperty = DependencyProperty.Register("Percent", typeof(int), typeof(WaitIndicator), new PropertyMetadata(-1));
            OnStartEvent = EventManager.RegisterRoutedEvent("OnStart", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WaitIndicator));
            OnStopEvent = EventManager.RegisterRoutedEvent("OnStop", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WaitIndicator));
            OnPauseEvent = EventManager.RegisterRoutedEvent("OnPause", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WaitIndicator));
            OnResumeEvent = EventManager.RegisterRoutedEvent("OnResume", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WaitIndicator));
        }

        public WaitIndicator()
        {
            InitializeComponent();
        }

        #endregion

        #region Методы

        public void Start()
        {
            RaiseEvent(new RoutedEventArgs(WaitIndicator.OnStartEvent, this));
        }

        public void Stop()
        {
            RaiseEvent(new RoutedEventArgs(WaitIndicator.OnStopEvent, this));
        }

        public void Pause()
        {
            RaiseEvent(new RoutedEventArgs(WaitIndicator.OnPauseEvent, this));
        }

        public void Resume()
        {
            RaiseEvent(new RoutedEventArgs(WaitIndicator.OnResumeEvent, this));
        }

        #endregion
    }

    #endregion Класс WaitIndicator

    #region Класс WaitIndicatorPercentConverter

    public class WaitIndicatorPercentConverter : IValueConverter
    {
        #region Методы

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int percent = (int)value;

            if (percent == -1) return "";

            return String.Format("{0} %", percent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс VisibilityConverter
}