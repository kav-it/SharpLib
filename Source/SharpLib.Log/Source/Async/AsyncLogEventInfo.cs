
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    public struct AsyncLogEventInfo
    {
        #region Свойства

        public LogEventInfo LogEvent { get; private set; }

        #endregion

        #region Конструктор

        public AsyncLogEventInfo(LogEventInfo logEvent)
            : this()
        {
            LogEvent = logEvent;
        }

        #endregion

        #region Методы

        public override bool Equals(object obj)
        {
            var other = (AsyncLogEventInfo)obj;
            return this == other;
        }

        public override int GetHashCode()
        {
            return LogEvent.GetHashCode();
        }

        #endregion

        public static bool operator ==(AsyncLogEventInfo eventInfo1, AsyncLogEventInfo eventInfo2)
        {
            return ReferenceEquals(eventInfo1.LogEvent, eventInfo2.LogEvent);
        }

        public static bool operator !=(AsyncLogEventInfo eventInfo1, AsyncLogEventInfo eventInfo2)
        {
            return !ReferenceEquals(eventInfo1.LogEvent, eventInfo2.LogEvent);
        }
    }
}
