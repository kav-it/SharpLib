using System;

namespace SharpLib
{
    /// <summary>
    /// Менеджер таймеров 
    /// </summary>
    /// <remarks>
    /// Для WPF-приложений создается DispatherTimer, лля остальных SharpLib.Timer
    /// </remarks>
    public static class Timers
    {
        /// <summary>
        /// Найденный тип таймера WPF
        /// </summary>
        private static Type _dispatcherType;

        /// <summary>
        /// Признак выполненного поиска (при дальнейшем создании таймеров)
        /// </summary>
        private static bool _searchCompleted;

        /// <summary>
        /// Блокировка
        /// </summary>
        private static readonly object _locker;

        static Timers()
        {
            _locker = new object();
        }

        /// <summary>
        /// Создание таймера
        /// </summary>
        public static ITimer Create(int interval, EventHandler tickHandler)
        {
            lock (_locker)
            {
                if (_searchCompleted == false)
                {
                    _dispatcherType = Reflector.SearchType("WindowsBase", "System.Windows.Threading.DispatcherTimer");
                    _searchCompleted = true;
                }

                if (_dispatcherType != null)
                {
                    return new TimerWpf(interval, tickHandler, _dispatcherType);
                }

                return new Timer(interval, tickHandler);
            }
        }
    }
}