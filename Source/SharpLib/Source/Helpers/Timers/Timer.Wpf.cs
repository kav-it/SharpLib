using System;
using System.Reflection;

namespace SharpLib
{
    /// <summary>
    /// Таймер для WPF приложений (на базе DispatcherTimer)
    /// </summary>
    internal sealed class TimerWpf : ITimer
    {
        #region Поля

        /// <summary>
        /// Блокировка объекта
        /// </summary>
        private readonly object _instanceLock;

        /// <summary>
        /// Таймер отсчета времени
        /// </summary>
        private readonly object _dispatherTimer;

        /// <summary>
        /// Метод запуска таймера
        /// </summary>
        private readonly MethodInfo _methodStart;

        /// <summary>
        /// Метод остановки таймера
        /// </summary>
        private readonly MethodInfo _methodStop;

        #endregion

        #region Свойства

        /// <summary>
        /// Вспомогательное поле
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Период тика таймера (в мс)
        /// </summary>
        public int Interval { get; private set; }

        #endregion

        #region Конструктор

        internal TimerWpf(int interval, EventHandler tickHandler, Type dispatcherType)
        {
            _instanceLock = new object();
            Interval = interval;
            _dispatherTimer = Reflector.CreateObject(dispatcherType);
            _methodStart = dispatcherType.GetMethod("Start");
            _methodStop = dispatcherType.GetMethod("Stop");

            // Установка свойства "Interval"
            var propInterval = dispatcherType.GetProperty("Interval");
            var span = new TimeSpan(0, 0, 0, 0, interval);
            propInterval.SetValue(_dispatherTimer, span, null);

            // Добавление обработчика события "Tick"
            var eventTick = dispatcherType.GetEvent("Tick");
            eventTick.AddEventHandler(_dispatherTimer, tickHandler);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Запуск таймер
        /// </summary>
        public void Start()
        {
            lock (_instanceLock)
            {
                _methodStart.Invoke(_dispatherTimer, null);
            }
        }

        /// <summary>
        /// Остановка таймера
        /// </summary>
        public void Stop()
        {
            lock (_instanceLock)
            {
                _methodStop.Invoke(_dispatherTimer, null);
            }
        }

        #endregion
    }
}