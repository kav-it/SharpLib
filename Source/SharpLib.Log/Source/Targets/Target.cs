using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpLib.Log
{
    [LogConfigurationItem]
    public abstract class Target : ISupportsInitialize, IDisposable
    {
        #region Поля

        private readonly object lockObject = new object();

        private List<Layout> allLayouts;

        private Exception initializeException;

        #endregion

        #region Свойства

        public string Name { get; set; }

        protected object SyncRoot
        {
            get { return lockObject; }
        }

        protected LoggingConfiguration LoggingConfiguration { get; private set; }

        protected bool IsInitialized { get; private set; }

        #endregion

        #region Методы

        void ISupportsInitialize.Initialize(LoggingConfiguration configuration)
        {
            Initialize(configuration);
        }

        void ISupportsInitialize.Close()
        {
            Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Flush(AsyncContinuation asyncContinuation)
        {
            if (asyncContinuation == null)
            {
                throw new ArgumentNullException("asyncContinuation");
            }

            lock (SyncRoot)
            {
                if (!IsInitialized)
                {
                    asyncContinuation(null);
                    return;
                }

                asyncContinuation = AsyncHelpers.PreventMultipleCalls(asyncContinuation);

                try
                {
                    FlushAsync(asyncContinuation);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    asyncContinuation(exception);
                }
            }
        }

        public void PrecalculateVolatileLayouts(LogEventInfo logEvent)
        {
            lock (SyncRoot)
            {
                if (IsInitialized)
                {
                    foreach (Layout l in allLayouts)
                    {
                        l.Precalculate(logEvent);
                    }
                }
            }
        }

        public override string ToString()
        {
            var targetAttribute = (TargetAttribute)Attribute.GetCustomAttribute(GetType(), typeof(TargetAttribute));
            if (targetAttribute != null)
            {
                return targetAttribute.Name + " Target[" + (Name ?? "(unnamed)") + "]";
            }

            return GetType().Name;
        }

        public void WriteAsyncLogEvent(AsyncLogEventInfo logEvent)
        {
            lock (SyncRoot)
            {
                if (!IsInitialized)
                {
                    logEvent.Continuation(null);
                    return;
                }

                if (initializeException != null)
                {
                    logEvent.Continuation(CreateInitException());
                    return;
                }

                var wrappedContinuation = AsyncHelpers.PreventMultipleCalls(logEvent.Continuation);

                try
                {
                    Write(logEvent.LogEvent.WithContinuation(wrappedContinuation));
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    if (LogManager.Instance.ThrowExceptions)
                    {
                        throw;
                    }

                    wrappedContinuation(exception);
                }
            }
        }

        public void WriteAsyncLogEvents(params AsyncLogEventInfo[] logEvents)
        {
            if (logEvents == null || logEvents.Length == 0)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (!IsInitialized)
                {
                    foreach (var ev in logEvents)
                    {
                        ev.Continuation(null);
                    }

                    return;
                }

                if (initializeException != null)
                {
                    foreach (var ev in logEvents)
                    {
                        ev.Continuation(CreateInitException());
                    }

                    return;
                }

                var wrappedEvents = new AsyncLogEventInfo[logEvents.Length];
                for (int i = 0; i < logEvents.Length; ++i)
                {
                    wrappedEvents[i] = logEvents[i].LogEvent.WithContinuation(AsyncHelpers.PreventMultipleCalls(logEvents[i].Continuation));
                }

                try
                {
                    Write(wrappedEvents);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    foreach (var ev in wrappedEvents)
                    {
                        ev.Continuation(exception);
                    }
                }
            }
        }

        internal void Initialize(LoggingConfiguration configuration)
        {
            lock (SyncRoot)
            {
                LoggingConfiguration = configuration;

                if (!IsInitialized)
                {
                    PropertyHelper.CheckRequiredParameters(this);
                    IsInitialized = true;
                    try
                    {
                        InitializeTarget();
                        initializeException = null;
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        initializeException = exception;
                        throw;
                    }
                }
            }
        }

        internal void Close()
        {
            lock (SyncRoot)
            {
                LoggingConfiguration = null;

                if (IsInitialized)
                {
                    IsInitialized = false;

                    try
                    {
                        if (initializeException == null)
                        {
                            CloseTarget();
                        }
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        throw;
                    }
                }
            }
        }

        internal void WriteAsyncLogEvents(AsyncLogEventInfo[] logEventInfos, AsyncContinuation continuation)
        {
            if (logEventInfos.Length == 0)
            {
                continuation(null);
            }
            else
            {
                var wrappedLogEventInfos = new AsyncLogEventInfo[logEventInfos.Length];
                int remaining = logEventInfos.Length;
                for (int i = 0; i < logEventInfos.Length; ++i)
                {
                    AsyncContinuation originalContinuation = logEventInfos[i].Continuation;
                    AsyncContinuation wrappedContinuation = ex =>
                    {
                        originalContinuation(ex);
                        if (0 == Interlocked.Decrement(ref remaining))
                        {
                            continuation(null);
                        }
                    };

                    wrappedLogEventInfos[i] = logEventInfos[i].LogEvent.WithContinuation(wrappedContinuation);
                }

                WriteAsyncLogEvents(wrappedLogEventInfos);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseTarget();
            }
        }

        protected virtual void InitializeTarget()
        {
            GetAllLayouts();
        }

        protected virtual void CloseTarget()
        {
        }

        protected virtual void FlushAsync(AsyncContinuation asyncContinuation)
        {
            asyncContinuation(null);
        }

        protected virtual void Write(LogEventInfo logEvent)
        {
        }

        protected virtual void Write(AsyncLogEventInfo logEvent)
        {
            try
            {
                MergeEventProperties(logEvent.LogEvent);

                Write(logEvent.LogEvent);
                logEvent.Continuation(null);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                logEvent.Continuation(exception);
            }
        }

        protected virtual void Write(AsyncLogEventInfo[] logEvents)
        {
            for (int i = 0; i < logEvents.Length; ++i)
            {
                Write(logEvents[i]);
            }
        }

        private Exception CreateInitException()
        {
            return new Exception("Target " + this + " failed to initialize.", initializeException);
        }

        private void GetAllLayouts()
        {
            allLayouts = new List<Layout>(ObjectGraphScanner.FindReachableObjects<Layout>(this));
        }

        protected void MergeEventProperties(LogEventInfo logEvent)
        {
            if (logEvent.Parameters == null)
            {
                return;
            }

            foreach (var item in logEvent.Parameters)
            {
                var logEventParameter = item as LogEventInfo;
                if (logEventParameter != null)
                {
                    foreach (var key in logEventParameter.Properties.Keys)
                    {
                        logEvent.Properties.Add(key, logEventParameter.Properties[key]);
                    }
                    logEventParameter.Properties.Clear();
                }
            }
        }

        #endregion
    }
}