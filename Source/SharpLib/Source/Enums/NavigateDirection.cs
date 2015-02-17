using System;

namespace SharpLib
{
    /// <summary>
    /// Направление навигации
    /// </summary>
    [Flags]
    public enum NavigateDirection
    {
        /// <summary>
        /// Неопределено
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Вперед
        /// </summary>
        Forward = (1 << 0),

        /// <summary>
        /// Назад
        /// </summary>
        Backward = (1 << 1),

        /// <summary>
        /// Вверх
        /// </summary>
        Up = (1 << 2),

        /// <summary>
        /// Вниз
        /// </summary>
        Down = (1 << 3),
    }
}