using System;

namespace NAudio.Wave
{
    internal class StoppedEventArgs : EventArgs
    {
        #region Поля

        private readonly Exception exception;

        #endregion

        #region Свойства

        public Exception Exception
        {
            get { return exception; }
        }

        #endregion

        #region Конструктор

        public StoppedEventArgs(Exception exception = null)
        {
            this.exception = exception;
        }

        #endregion
    }
}