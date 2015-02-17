using System;

namespace SharpLib
{
    /// <summary>
    /// Порядок байт
    /// </summary>
    [Flags]
    public enum Placement
    {
        /// <summary>
        /// Неопределено
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Слева
        /// </summary>
        Left = (1 << 0),

        /// <summary>
        /// Слева
        /// </summary>
        Right = (1 << 1),

        /// <summary>
        /// Слева
        /// </summary>
        Top = (1 << 2),

        /// <summary>
        /// Слева
        /// </summary>
        Bottom = (1 << 3),
    }
}