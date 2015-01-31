using System;

namespace SharpLib
{
    /// <summary>
    /// Еще один таймер :) помимо существующих 3-х.Введен, для возможности смены реализации
    /// не взирая на веяния моды Microsoft
    /// </summary>
    /// <remarks>
    /// По чему не
    /// 1. System.Timers.Timer    = это Component, который не нужен
    /// 2. System.Threading.Timer = нужен workaraund, "куцый"
    /// 3. DispatcherTimer        = требует WPF (WindowsBase)
    /// </remarks>
    internal sealed class Timer : ITimer, IDisposable
    {
        #region Поля

        /// <summary>
        /// Блокировка объекта
        /// </summary>
        private readonly object _instanceLock;

        /// <summary>
        /// Событие "Тик таймера"
        /// </summary>
        private readonly EventHandler _tick;

        /// <summary>
        /// Таймер отсчета времени
        /// </summary>
        private System.Threading.Timer _threadTimer;

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

        internal Timer(int interval, EventHandler tickHandler)
        {
            _instanceLock = new object();
            Interval = interval;
            _tick = tickHandler;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обработка тика таймера
        /// </summary>
        private void OnTimerTick(object state)
        {
            Diag.WriteLine("OnTimerTick");
            Restart();
            RaiseTickEvent();
        }

        /// <summary>
        /// Запуск таймер
        /// </summary>
        public void Start()
        {
            lock (_instanceLock)
            {
                if (_threadTimer == null)
                {
                    _threadTimer = new System.Threading.Timer(OnTimerTick);
                }
                Restart();
            }
        }

        /// <summary>
        /// Остановка таймера
        /// </summary>
        public void Stop()
        {
            lock (_instanceLock)
            {
                if (_threadTimer != null)
                {
                    _threadTimer.Dispose();
                    _threadTimer = null;
                }
            }
        }

        /// <summary>
        /// Перезапуск таймера
        /// </summary>
        private void Restart()
        {
            lock (_instanceLock)
            {
                _threadTimer.Change(Interval, 0);
            }
        }

        /// <summary>
        /// Генерация события
        /// </summary>
        private void RaiseTickEvent()
        {
            if (_tick != null)
            {
                _tick(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Удаление объекта
        /// </summary>
        public void Dispose()
        {
            if (_threadTimer != null)
            {
                _threadTimer.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}