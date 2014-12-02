using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpLib.Log
{
    [LogConfigurationItem]
    public abstract class Target : ISupportsInitialize, IDisposable
    {
        #region Поля

        private readonly object _lockObject;

        private List<Layout> _allLayouts;

        private Exception _initializeException;

        protected Target()
        {
            _lockObject = new object();
        }

        #endregion

        #region Свойства

        public string Name { get; set; }

        protected object SyncRoot
        {
            get { return _lockObject; }
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
                    foreach (Layout l in _allLayouts)
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
                    return;
                }

                if (_initializeException != null)
                {
                    return;
                }

                try
                {
                    Write(logEvent.LogEvent.WithContinuation());
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }
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
                    return;
                }

                if (_initializeException != null)
                {
                    return;
                }

                var wrappedEvents = new AsyncLogEventInfo[logEvents.Length];
                for (int i = 0; i < logEvents.Length; ++i)
                {
                    wrappedEvents[i] = logEvents[i].LogEvent.WithContinuation();
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
                        _initializeException = null;
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        _initializeException = exception;
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

                if (!IsInitialized)
                {
                    return;
                }
                IsInitialized = false;

                try
                {
                    if (_initializeException == null)
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

        internal void WriteAsyncLogEvents(AsyncLogEventInfo[] logEventInfos, AsyncContinuation continuation)
        {
            if (logEventInfos.Length == 0)
            {
                continuation(null);
            }
            else
            {
                var wrappedLogEventInfos = new AsyncLogEventInfo[logEventInfos.Length];
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
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }
            }
        }

        protected virtual void Write(AsyncLogEventInfo[] logEvents)
        {
            foreach (AsyncLogEventInfo evt in logEvents)
            {
                Write(evt);
            }
        }

        private Exception CreateInitException()
        {
            return new Exception("Target " + this + " failed to initialize.", _initializeException);
        }

        private void GetAllLayouts()
        {
            _allLayouts = new List<Layout>(ObjectGraphScanner.FindReachableObjects<Layout>(this));
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