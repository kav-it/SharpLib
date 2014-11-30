using System;

namespace SharpLib.Log
{
    [LogConfigurationItem]
    public abstract class TimeSource
    {
        #region Свойства

        public abstract DateTime Time { get; }

        public static TimeSource Current { get; set; }

        #endregion

        #region Конструктор

        static TimeSource()
        {
            Current = new FastLocalTimeSource();
        }

        #endregion

        #region Методы

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