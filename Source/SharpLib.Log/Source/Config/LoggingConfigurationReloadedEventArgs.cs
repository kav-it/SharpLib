using System;

#if !SILVERLIGHT

namespace NLog.Config
{
    public class LoggingConfigurationReloadedEventArgs : EventArgs
    {
        #region Свойства

        public bool Succeeded { get; private set; }

        public Exception Exception { get; private set; }

        #endregion

        #region Конструктор

        internal LoggingConfigurationReloadedEventArgs(bool succeeded, Exception exception)
        {
            Succeeded = succeeded;
            Exception = exception;
        }

        #endregion
    }
}

#endif