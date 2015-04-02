using System;
using System.Threading;

namespace SharpLib.Texter.Utils
{
    internal sealed class CallbackOnDispose : IDisposable
    {
        #region Поля

        private Action _action;

        #endregion

        #region Конструктор

        public CallbackOnDispose(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            this._action = action;
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            var a = Interlocked.Exchange(ref _action, null);
            if (a != null)
            {
                a();
            }
        }

        #endregion
    }
}