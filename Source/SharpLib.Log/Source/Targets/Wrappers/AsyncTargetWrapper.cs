// 
// Copyright (c) 2004-2011 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

using NLog.Common;
using NLog.Internal;

namespace NLog.Targets.Wrappers
{
    /// <summary>
    /// Provides asynchronous, buffered execution of target writes.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/AsyncWrapper_target">Documentation on NLog Wiki</seealso>
    /// <remarks>
    ///     <p>
    /// Asynchronous target wrapper allows the logger code to execute more quickly, by queueing
    /// messages and processing them in a separate thread. You should wrap targets
    /// that spend a non-trivial amount of time in their Write() method with asynchronous
    /// target to speed up logging.
    ///     </p>
    ///     <p>
    /// Because asynchronous logging is quite a common scenario, NLog supports a
    /// shorthand notation for wrapping all targets with AsyncWrapper. Just add async="true" to
    /// the &lt;targets/&gt; element in the configuration file.
    ///     </p>
    ///     <code lang="XML">
    /// <![CDATA[
    /// <targets async="true">
    ///    ... your targets go here ...
    /// </targets>
    /// ]]></code>
    /// </remarks>
    /// <example>
    ///     <p>
    /// To set up the target in the <a href="config.html">configuration file</a>,
    /// use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/AsyncWrapper/NLog.config" />
    ///     <p>
    /// The above examples assume just one target and a single rule. See below for
    /// a programmatic configuration that's equivalent to the above config file:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/AsyncWrapper/Wrapping File/Example.cs" />
    /// </example>
    [Target("AsyncWrapper", IsWrapper = true)]
    public class AsyncTargetWrapper : WrapperTargetBase
    {
        #region ����

        private readonly object continuationQueueLock = new object();

        private readonly Queue<AsyncContinuation> flushAllContinuations = new Queue<AsyncContinuation>();

        private readonly object lockObject = new object();

        private Timer lazyWriterTimer;

        #endregion

        #region ��������

        /// <summary>
        /// Gets or sets the number of log events that should be processed in a batch
        /// by the lazy writer thread.
        /// </summary>
        /// <docgen category='Buffering Options' order='100' />
        [DefaultValue(100)]
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the time in milliseconds to sleep between batches.
        /// </summary>
        /// <docgen category='Buffering Options' order='100' />
        [DefaultValue(50)]
        public int TimeToSleepBetweenBatches { get; set; }

        /// <summary>
        /// Gets or sets the action to be taken when the lazy writer thread request queue count
        /// exceeds the set limit.
        /// </summary>
        /// <docgen category='Buffering Options' order='100' />
        [DefaultValue("Discard")]
        public AsyncTargetWrapperOverflowAction OverflowAction
        {
            get { return RequestQueue.OnOverflow; }
            set { RequestQueue.OnOverflow = value; }
        }

        /// <summary>
        /// Gets or sets the limit on the number of requests in the lazy writer thread request queue.
        /// </summary>
        /// <docgen category='Buffering Options' order='100' />
        [DefaultValue(10000)]
        public int QueueLimit
        {
            get { return RequestQueue.RequestLimit; }
            set { RequestQueue.RequestLimit = value; }
        }

        /// <summary>
        /// Gets the queue of lazy writer thread requests.
        /// </summary>
        internal AsyncRequestQueue RequestQueue { get; private set; }

        #endregion

        #region �����������

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTargetWrapper" /> class.
        /// </summary>
        public AsyncTargetWrapper()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        public AsyncTargetWrapper(Target wrappedTarget)
            : this(wrappedTarget, 10000, AsyncTargetWrapperOverflowAction.Discard)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="queueLimit">Maximum number of requests in the queue.</param>
        /// <param name="overflowAction">The action to be taken when the queue overflows.</param>
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

        #region ������

        /// <summary>
        /// Waits for the lazy writer thread to finish writing messages.
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            lock (continuationQueueLock)
            {
                flushAllContinuations.Enqueue(asyncContinuation);
            }
        }

        /// <summary>
        /// Initializes the target by starting the lazy writer timer.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            RequestQueue.Clear();
            lazyWriterTimer = new Timer(ProcessPendingEvents, null, Timeout.Infinite, Timeout.Infinite);
            StartLazyWriterTimer();
        }

        /// <summary>
        /// Shuts down the lazy writer timer.
        /// </summary>
        protected override void CloseTarget()
        {
            StopLazyWriterThread();
            if (RequestQueue.RequestCount > 0)
            {
                ProcessPendingEvents(null);
            }

            base.CloseTarget();
        }

        /// <summary>
        /// Starts the lazy writer thread which periodically writes
        /// queued log messages.
        /// </summary>
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

        /// <summary>
        /// Starts the lazy writer thread.
        /// </summary>
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

        /// <summary>
        /// Adds the log event to asynchronous queue to be processed by
        /// the lazy writer thread.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <remarks>
        /// The <see cref="Target.PrecalculateVolatileLayouts" /> is called
        /// to ensure that the log event can be processed in another thread.
        /// </remarks>
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
                        InternalLogger.Trace("Flushing {0} events.", count);
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
                        // write all events, then flush, then call the continuation
                        WrappedTarget.WriteAsyncLogEvents(logEventInfos, ex => WrappedTarget.Flush(continuation));
                    }
                    else
                    {
                        // just write all events
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

                InternalLogger.Error("Error in lazy writer timer procedure: {0}", exception);
            }
            finally
            {
                StartLazyWriterTimer();
            }
        }

        #endregion
    }
}