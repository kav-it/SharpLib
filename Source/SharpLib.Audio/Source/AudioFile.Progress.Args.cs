using System;

namespace SharpLib.Audio
{
    /// <summary>
    /// Аргумент события "Progress"
    /// </summary>
    public class AudioFileProgressArgs : EventArgs
    {
        #region Свойства

        /// <summary>
        /// Текущее время воспроизведения
        /// </summary>
        public TimeSpan Current { get; private set; }

        /// <summary>
        /// Общее время воспроизведения
        /// </summary>
        public TimeSpan Total { get; private set; }

        #endregion

        #region Конструктор

        public AudioFileProgressArgs(TimeSpan current, TimeSpan total)
        {
            Current = current;
            Total = total;
        }

        #endregion
    }
}