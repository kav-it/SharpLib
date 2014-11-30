using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace SharpLib.Log
{
    [Target("AsyncWrapper", IsWrapper = true)]
    public class AsyncTargetWrapper : WrapperTargetBase
    {
        #region Поля

        private readonly object continuationQueueLock = new object();

        private readonly Queue<AsyncContinuation> flushAllContinuations = new Queue<AsyncContinuation>();

        private readonly object lockObject = new object();

        private Timer lazyWriterTimer;

        #endregion

        #region Свойства

        [DefaultValue(100)]
        public int BatchSize { get; set; }

        [DefaultValue(50)]
        public int TimeToSleepBetweenBatches { get; set; }

        [DefaultValue("Discard")]
        public AsyncTargetWrapperOverflowAction OverflowAction
        {
            get { return RequestQueue.OnOverflow; }
            set { RequestQueue.OnOverflow = value; }
        }

        [DefaultValue(10000)]
        public int QueueLimit
        {
            get { return RequestQueue.RequestLimit; }
            set { RequestQueue.RequestLimit = value; }
        }

        internal AsyncRequestQueue RequestQueue { get; private set; }

        #endregion

        #region Конструктор

        public AsyncTargetWrapper()
            : this(null)
        {
        }

        public AsyncTargetWrapper(Target wrappedTarget)
            : this(wrappedTarget, 10000, AsyncTargetWrapperOverflowAction.Discard)
        {
        }

        public AsyncTargetWrapper(Target wrappedTarget, int queueLimit, AsyncTargetWrapperOverflowAction overflowAction)
        {
            RequestQueue = new AsyncRequestQueue(10000, AsyncTargetWrapperOverflowAction.Discard);
            TimeToSleepBetweenBatches = 50;
            BatchSize = 100;
            WrappedTarget = wrappedTarget;
            QueueLimit = queueLimit;
            OverflowAction = overflowAction;
        }

        #endregion

        #region Методы

        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            lock (continuationQueueLock)
            {
                flushAllContinuations.Enqueue(asyncContinuation);
            }
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            RequestQueue.Clear();
            lazyWriterTimer = new Timer(ProcessPendingEvents, null, Timeout.Infinite, Timeout.Infinite);
            StartLazyWriterTimer();
        }

        protected override void CloseTarget()
        {
            StopLazyWriterThread();
            if (RequestQueue.RequestCount > 0)
            {
                ProcessPendingEvents(null);
            }

            base.CloseTarget();
        }

        protected virtual void StartLazyWriterTimer()
        {
            lock (lockObject)
            {
                if (lazyWriterTimer != null)
                {
                    lazyWriterTimer.Change(TimeToSleepBetweenBatches, Timeout.Infinite);
                }
            }
        }

        protected virtual void StopLazyWriterThread()
        {
            lock (lockObject)
            {
                if (lazyWriterTimer != null)
                {
                    lazyWriterTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    lazyWriterTimer = null;
                }
            }
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            MergeEventProperties(logEvent.LogEvent);
            PrecalculateVolatileLayouts(logEvent.LogEvent);
            RequestQueue.Enqueue(logEvent);
        }

        private void ProcessPendingEvents(object state)
        {
            AsyncContinuation[] continuations;
            lock (continuationQueueLock)
            {
                continuations = flushAllContinuations.Count > 0
                    ? flushAllContinuations.ToArray()
                    : new AsyncContinuation[] { null };
                flushAllContinuations.Clear();
            }

            try
            {
                foreach (var continuation in continuations)
                {
                    int count = BatchSize;
                    if (continuation != null)
                    {
                        count = RequestQueue.RequestCount;
                    }

                    if (RequestQueue.RequestCount == 0)
                    {
                        if (continuation != null)
                        {
                            continuation(null);
                        }
                    }

                    AsyncLogEventInfo[] logEventInfos = RequestQueue.DequeueBatch(count);

                    if (continuation != null)
                    {
                        WrappedTarget.WriteAsyncLogEvents(logEventInfos, ex => WrappedTarget.Flush(continuation));
                    }
                    else
                    {
                        WrappedTarget.WriteAsyncLogEvents(logEventInfos);
                    }
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }
            }
            finally
            {
                StartLazyWriterTimer();
            }
        }

        #endregion
    }
}