namespace NLog.Common
{
    public struct AsyncLogEventInfo
    {
        #region Свойства

        public LogEventInfo LogEvent { get; private set; }

        public AsyncContinuation Continuation { get; internal set; }

        #endregion

        #region Конструктор

        public AsyncLogEventInfo(LogEventInfo logEvent, AsyncContinuation continuation)
            : this()
        {
            LogEvent = logEvent;
            Continuation = continuation;
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
            return LogEvent.GetHashCode() ^ Continuation.GetHashCode();
        }

        #endregion

        public static bool operator ==(AsyncLogEventInfo eventInfo1, AsyncLogEventInfo eventInfo2)
        {
            return ReferenceEquals(eventInfo1.Continuation, eventInfo2.Continuation)
                   && ReferenceEquals(eventInfo1.LogEvent, eventInfo2.LogEvent);
        }

        public static bool operator !=(AsyncLogEventInfo eventInfo1, AsyncLogEventInfo eventInfo2)
        {
            return !ReferenceEquals(eventInfo1.Continuation, eventInfo2.Continuation)
                   || !ReferenceEquals(eventInfo1.LogEvent, eventInfo2.LogEvent);
        }
    }
}