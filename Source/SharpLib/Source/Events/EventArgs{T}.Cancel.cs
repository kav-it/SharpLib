using System.ComponentModel;

namespace SharpLib
{
    public class CancelEventArgs<T>: CancelEventArgs
    {
        #region Свойства

        public T Value { get; private set; }

        #endregion

        #region Конструктор

        public CancelEventArgs(T value)
        {
            Value = value;
        }

        #endregion
    }
}