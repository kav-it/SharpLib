using System;

namespace SharpLib
{
    public class ChangingEventArgs<T>: EventArgs
    {
        #region Свойства

        public T OldValue { get; private set; }

        public T NewValue { get; set; }

        #endregion

        #region Конструктор

        public ChangingEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion
    }
}