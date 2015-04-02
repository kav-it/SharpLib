using System;
using System.Collections.Generic;

namespace SharpLib.Texter.Utils
{
    internal sealed class DelayedEvents
    {
        #region Поля

        private readonly Queue<EventCall> eventCalls = new Queue<EventCall>();

        #endregion

        #region Методы

        public void DelayedRaise(EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null)
            {
                eventCalls.Enqueue(new EventCall(handler, sender, e));
            }
        }

        public void RaiseEvents()
        {
            while (eventCalls.Count > 0)
            {
                eventCalls.Dequeue().Call();
            }
        }

        #endregion

        #region Вложенный класс: EventCall

        private struct EventCall
        {
            #region Поля

            private readonly EventArgs e;

            private readonly EventHandler handler;

            private readonly object sender;

            #endregion

            #region Конструктор

            public EventCall(EventHandler handler, object sender, EventArgs e)
            {
                this.handler = handler;
                this.sender = sender;
                this.e = e;
            }

            #endregion

            #region Методы

            public void Call()
            {
                handler(sender, e);
            }

            #endregion
        }

        #endregion
    }
}