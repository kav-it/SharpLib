using System;

namespace SharpLib.Log
{
    public class LoggerConfigModule
    {
        #region Свойства

        public String Name { get; set; }

        public LogLevel Level { get; set; }

        #endregion

        #region Конструктор

        public LoggerConfigModule()
        {
            Name = string.Empty;
            Level = LogLevel.Off;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, Level);
        }

        #endregion
    }
}