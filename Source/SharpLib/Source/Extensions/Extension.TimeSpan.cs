using System;
using System.Text;

namespace SharpLib
{
    /// <summary>
    /// Методы расширения для класса 'TimeSpan'
    /// </summary>
    public static class ExtensionTimeSpan
    {
        #region Методы

        /// <summary>
        /// Добавление строки с форматированием
        /// </summary>
        public static string ToStringMinEx(this TimeSpan self, bool showHours = false)
        {
            return string.Format("{0:00}:{1:00}", self.Minutes, self.Seconds);
        }

        #endregion
    }
}