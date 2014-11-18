﻿namespace SharpLib
{
    public class ChangingEventArgs<T>
    {
        #region Свойства

        public T OldValue { get; private set; }

        public T NewValue { get; private set; }

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