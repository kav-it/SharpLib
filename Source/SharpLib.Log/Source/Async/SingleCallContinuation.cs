
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    internal class SingleCallContinuation
    {
        #region Поля

        private AsyncContinuation asyncContinuation;

        #endregion

        #region Конструктор

        public SingleCallContinuation(AsyncContinuation asyncContinuation)
        {
            this.asyncContinuation = asyncContinuation;
        }

        #endregion

        #region Методы

        public void Function(Exception exception)
        {
            try
            {
                var cont = Interlocked.Exchange(ref asyncContinuation, null);
                if (cont != null)
                {
                    cont(exception);
                }
            }
            catch (Exception ex)
            {
                if (ex.MustBeRethrown())
                {
                    throw;
                }
            }
        }

        #endregion
    }
}
