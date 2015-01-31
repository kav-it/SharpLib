using System;

namespace SharpLib
{
    /// <summary>
    /// Менеджер таймеров 
    /// </summary>
    /// <remarks>
    /// Для WPF-приложений создается DispatherTimer, лля остальных SharpLib.Timer
    /// </remarks>
    public interface ITimer
    {
        /// <summary>
        /// Период таймера (мс)
        /// </summary>
        int Interval { get; }

        /// <summary>
        /// Пользовательское полей
        /// </summary>
        object Tag { get; set; }

        /// <summary>
        /// Запуск таймера (перезапуск)
        /// </summary>
        void Start();

        /// <summary>
        /// Остановка таймера
        /// </summary>
        void Stop();
    }
}