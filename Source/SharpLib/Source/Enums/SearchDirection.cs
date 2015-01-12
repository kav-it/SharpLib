using System;

namespace SharpLib
{
    /// <summary>
    /// Направление поиска
    /// </summary>
    [Flags]
    public enum SearchDirection
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
    }
}