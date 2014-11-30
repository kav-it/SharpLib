
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    internal class AsyncRequestQueue
    {
        #region Поля

        private readonly Queue<AsyncLogEventInfo> logEventInfoQueue = new Queue<AsyncLogEventInfo>();

        #endregion

        #region Свойства

        public int RequestLimit { get; set; }

        public AsyncTargetWrapperOverflowAction OnOverflow { get; set; }

        public int RequestCount
        {
            get { return logEventInfoQueue.Count; }
        }

        #endregion

        #region Конструктор

        public AsyncRequestQueue(int requestLimit, AsyncTargetWrapperOverflowAction overflowAction)
        {
            RequestLimit = requestLimit;
            OnOverflow = overflowAction;
        }

        #endregion

        #region Методы

        public void Enqueue(AsyncLogEventInfo logEventInfo)
        {
            lock (this)
            {
                if (logEventInfoQueue.Count >= RequestLimit)
                {
                    switch (OnOverflow)
                    {
                        case AsyncTargetWrapperOverflowAction.Discard:
                            logEventInfoQueue.Dequeue();
                            break;

                        case AsyncTargetWrapperOverflowAction.Grow:
                            break;

                        case AsyncTargetWrapperOverflowAction.Block:
                            while (logEventInfoQueue.Count >= RequestLimit)
                            {
                                System.Threading.Monitor.Wait(this);
                            }
                            break;
                    }
                }

                logEventInfoQueue.Enqueue(logEventInfo);
            }
        }

        public AsyncLogEventInfo[] DequeueBatch(int count)
        {
            var resultEvents = new List<AsyncLogEventInfo>();

            lock (this)
            {
                for (int i = 0; i < count; ++i)
                {
                    if (logEventInfoQueue.Count <= 0)
                    {
                        break;
                    }

                    resultEvents.Add(logEventInfoQueue.Dequeue());
                }

                if (OnOverflow == AsyncTargetWrapperOverflowAction.Block)
                {
                    System.Threading.Monitor.PulseAll(this);
                }
            }

            return resultEvents.ToArray();
        }

        public void Clear()
        {
            lock (this)
            {
                logEventInfoQueue.Clear();
            }
        }

        #endregion
    }
}
