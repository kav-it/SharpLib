using System;

using NLog.Config;

namespace NLog.Time
{
    /// <summary>
    /// Defines source of current time.
    /// </summary>
    [NLogConfigurationItem]
    public abstract class TimeSource
    {
        #region Свойства

        /// <summary>
        /// Gets current time.
        /// </summary>
        public abstract DateTime Time { get; }

        /// <summary>
        /// Gets or sets current global time source used in all log events.
        /// </summary>
        /// <remarks>
        /// Default time source is <see cref="FastLocalTimeSource" />.
        /// </remarks>
        public static TimeSource Current { get; set; }

        #endregion

        #region Конструктор

        static TimeSource()
        {
            Current = new FastLocalTimeSource();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var targetAttribute = (TimeSourceAttribute)Attribute.GetCustomAttribute(GetType(), typeof(TimeSourceAttribute));
            if (targetAttribute != null)
            {
                return targetAttribute.Name + " (time source)";
            }

            return GetType().Name;
        }

        #endregion
    }
}