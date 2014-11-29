using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NLog.Internal;

namespace NLog.Common
{
    public static class AsyncHelpers
    {
        #region Методы

        public static void ForEachItemSequentially<T>(IEnumerable<T> items, AsyncContinuation asyncContinuation, AsynchronousAction<T> action)
        {
            action = ExceptionGuard(action);
            AsyncContinuation invokeNext = null;
            IEnumerator<T> enumerator = items.GetEnumerator();

            invokeNext = ex =>
            {
                if (ex != null)
                {
                    asyncContinuation(ex);
                    return;
                }

                if (!enumerator.MoveNext())
                {
                    asyncContinuation(null);
                    return;
                }

                action(enumerator.Current, PreventMultipleCalls(invokeNext));
            };

            invokeNext(null);
        }

        public static void Repeat(int repeatCount, AsyncContinuation asyncContinuation, AsynchronousAction action)
        {
            action = ExceptionGuard(action);
            AsyncContinuation invokeNext = null;
            int remaining = repeatCount;

            invokeNext = ex =>
            {
                if (ex != null)
                {
                    asyncContinuation(ex);
                    return;
                }

                if (remaining-- <= 0)
                {
                    asyncContinuation(null);
                    return;
                }

                action(PreventMultipleCalls(invokeNext));
            };

            invokeNext(null);
        }

        public static AsyncContinuation PrecededBy(AsyncContinuation asyncContinuation, AsynchronousAction action)
        {
            action = ExceptionGuard(action);

            AsyncContinuation continuation =
                ex =>
                {
                    if (ex != null)
                    {
                        asyncContinuation(ex);
                        return;
                    }

                    action(PreventMultipleCalls(asyncContinuation));
                };

            return continuation;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Continuation will be disposed of elsewhere.")]
        public static AsyncContinuation WithTimeout(AsyncContinuation asyncContinuation, TimeSpan timeout)
        {
            return new TimeoutContinuation(asyncContinuation, timeout).Function;
        }

        public static void ForEachItemInParallel<T>(IEnumerable<T> values, AsyncContinuation asyncContinuation, AsynchronousAction<T> action)
        {
            action = ExceptionGuard(action);

            var items = new List<T>(values);
            int remaining = items.Count;
            var exceptions = new List<Exception>();

            InternalLogger.Trace("ForEachItemInParallel() {0} items", items.Count);

            if (remaining == 0)
            {
                asyncContinuation(null);
                return;
            }

            AsyncContinuation continuation =
                ex =>
                {
                    InternalLogger.Trace("Continuation invoked: {0}", ex);
                    int r;

                    if (ex != null)
                    {
                        lock (exceptions)
                        {
                            exceptions.Add(ex);
                        }
                    }

                    r = Interlocked.Decrement(ref remaining);
                    InternalLogger.Trace("Parallel task completed. {0} items remaining", r);
                    if (r == 0)
                    {
                        asyncContinuation(GetCombinedException(exceptions));
                    }
                };

            foreach (T item in items)
            {
                T itemCopy = item;

                ThreadPool.QueueUserWorkItem(s => action(itemCopy, PreventMultipleCalls(continuation)));
            }
        }

        public static void RunSynchronously(AsynchronousAction action)
        {
            var ev = new ManualResetEvent(false);
            Exception lastException = null;

            action(PreventMultipleCalls(ex =>
            {
                lastException = ex;
                ev.Set();
            }));
            ev.WaitOne();
            if (lastException != null)
            {
                throw new NLogRuntimeException("Asynchronous exception has occurred.", lastException);
            }
        }

        public static AsyncContinuation PreventMultipleCalls(AsyncContinuation asyncContinuation)
        {
            if (asyncContinuation.Target is SingleCallContinuation)
            {
                return asyncContinuation;
            }

            return new SingleCallContinuation(asyncContinuation).Function;
        }

        public static Exception GetCombinedException(IList<Exception> exceptions)
        {
            if (exceptions.Count == 0)
            {
                return null;
            }

            if (exceptions.Count == 1)
            {
                return exceptions[0];
            }

            var sb = new StringBuilder();
            string separator = string.Empty;
            string newline = EnvironmentHelper.NewLine;
            foreach (var ex in exceptions)
            {
                sb.Append(separator);
                sb.Append(ex);
                sb.Append(newline);
                separator = newline;
            }

            return new NLogRuntimeException("Got multiple exceptions:\r\n" + sb);
        }

        private static AsynchronousAction ExceptionGuard(AsynchronousAction action)
        {
            return cont =>
            {
                try
                {
                    action(cont);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    cont(exception);
                }
            };
        }

        private static AsynchronousAction<T> ExceptionGuard<T>(AsynchronousAction<T> action)
        {
            return (T argument, AsyncContinuation cont) =>
            {
                try
                {
                    action(argument, cont);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    cont(exception);
                }
            };
        }

        #endregion
    }
}