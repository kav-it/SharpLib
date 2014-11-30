using System;
using System.Threading;

namespace SharpLib.Log
{
    internal class TimeoutContinuation : IDisposable
    {
        #region Поля

        private AsyncContinuation asyncContinuation;

        private Timer timeoutTimer;

        #endregion

        #region Конструктор

        public TimeoutContinuation(AsyncContinuation asyncContinuation, TimeSpan timeout)
        {
            this.asyncContinuation = asyncContinuation;
            timeoutTimer = new Timer(TimerElapsed, null, timeout, TimeSpan.FromMilliseconds(-1));
        }

        #endregion

        #region Методы

        public void Function(Exception exception)
        {
            try
            {
                StopTimer();

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

        public void Dispose()
        {
            StopTimer();
            GC.SuppressFinalize(this);
        }

        private void StopTimer()
        {
            lock (this)
            {
                if (timeoutTimer != null)
                {
                    timeoutTimer.Dispose();
                    timeoutTimer = null;
                }
            }
        }

        private void TimerElapsed(object state)
        {
            Function(new TimeoutException("Timeout."));
        }

        #endregion
    }
}