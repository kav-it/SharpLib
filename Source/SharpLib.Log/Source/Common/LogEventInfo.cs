using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

using NLog.Common;
using NLog.Internal;
using NLog.Layouts;
using NLog.Time;

namespace NLog
{
    public class LogEventInfo
    {
        #region Поля

        public static readonly DateTime ZeroDate = DateTime.UtcNow;

        private static int globalSequenceId;

        private readonly object layoutCacheLock = new object();

        private IDictionary eventContextAdapter;

        private string formattedMessage;

        private IDictionary<Layout, string> layoutCache;

        private IDictionary<object, object> properties;

        #endregion

        #region Свойства

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID", Justification = "Backwards compatibility")]
        public int SequenceID { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TimeStamp", Justification = "Backwards compatibility.")]
        public DateTime TimeStamp { get; set; }

        public LogLevel Level { get; set; }

        public bool HasStackTrace
        {
            get { return StackTrace != null; }
        }

        public StackFrame UserStackFrame
        {
            get { return (StackTrace != null) ? StackTrace.GetFrame(UserStackFrameNumber) : null; }
        }

        public int UserStackFrameNumber { get; private set; }

        public StackTrace StackTrace { get; private set; }

        public Exception Exception { get; set; }

        public string LoggerName { get; set; }

        [Obsolete("This property should not be used.")]
        public string LoggerShortName
        {
            get
            {
                int lastDot = LoggerName.LastIndexOf('.');
                if (lastDot >= 0)
                {
                    return LoggerName.Substring(lastDot + 1);
                }

                return LoggerName;
            }
        }

        public string Message { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "For backwards compatibility.")]
        public object[] Parameters { get; set; }

        public IFormatProvider FormatProvider { get; set; }

        public string FormattedMessage
        {
            get
            {
                if (formattedMessage == null)
                {
                    CalcFormattedMessage();
                }

                return formattedMessage;
            }
        }

        public IDictionary<object, object> Properties
        {
            get
            {
                if (properties == null)
                {
                    InitEventContext();
                }

                return properties;
            }
        }

        [Obsolete("Use LogEventInfo.Properties instead.", true)]
        public IDictionary Context
        {
            get
            {
                if (eventContextAdapter == null)
                {
                    InitEventContext();
                }

                return eventContextAdapter;
            }
        }

        #endregion

        #region Конструктор

        public LogEventInfo()
        {
        }

        public LogEventInfo(LogLevel level, string loggerName, [Localizable(false)] string message)
            : this(level, loggerName, null, message, null, null)
        {
        }

        public LogEventInfo(LogLevel level, string loggerName, IFormatProvider formatProvider, [Localizable(false)] string message, object[] parameters)
            : this(level, loggerName, formatProvider, message, parameters, null)
        {
        }

        public LogEventInfo(LogLevel level, string loggerName, IFormatProvider formatProvider, [Localizable(false)] string message, object[] parameters, Exception exception)
        {
            TimeStamp = TimeSource.Current.Time;
            Level = level;
            LoggerName = loggerName;
            Message = message;
            Parameters = parameters;
            FormatProvider = formatProvider;
            Exception = exception;
            SequenceID = Interlocked.Increment(ref globalSequenceId);

            if (NeedToPreformatMessage(parameters))
            {
                CalcFormattedMessage();
            }
        }

        #endregion

        #region Методы

        public static LogEventInfo CreateNullEvent()
        {
            return new LogEventInfo(LogLevel.Off, string.Empty, string.Empty);
        }

        public static LogEventInfo Create(LogLevel logLevel, string loggerName, [Localizable(false)] string message)
        {
            return new LogEventInfo(logLevel, loggerName, null, message, null);
        }

        public static LogEventInfo Create(LogLevel logLevel, string loggerName, IFormatProvider formatProvider, [Localizable(false)] string message, object[] parameters)
        {
            return new LogEventInfo(logLevel, loggerName, formatProvider, message, parameters);
        }

        public static LogEventInfo Create(LogLevel logLevel, string loggerName, IFormatProvider formatProvider, object message)
        {
            return new LogEventInfo(logLevel, loggerName, formatProvider, "{0}", new[] { message });
        }

        public static LogEventInfo Create(LogLevel logLevel, string loggerName, [Localizable(false)] string message, Exception exception)
        {
            return new LogEventInfo(logLevel, loggerName, null, message, null, exception);
        }

        public AsyncLogEventInfo WithContinuation(AsyncContinuation asyncContinuation)
        {
            return new AsyncLogEventInfo(this, asyncContinuation);
        }

        public override string ToString()
        {
            return "Log Event: Logger='" + LoggerName + "' Level=" + Level + " Message='" + FormattedMessage + "' SequenceID=" + SequenceID;
        }

        public void SetStackTrace(StackTrace stackTrace, int userStackFrame)
        {
            StackTrace = stackTrace;
            UserStackFrameNumber = userStackFrame;
        }

        internal string AddCachedLayoutValue(Layout layout, string value)
        {
            lock (layoutCacheLock)
            {
                if (layoutCache == null)
                {
                    layoutCache = new Dictionary<Layout, string>();
                }

                layoutCache[layout] = value;
            }

            return value;
        }

        internal bool TryGetCachedLayoutValue(Layout layout, out string value)
        {
            lock (layoutCacheLock)
            {
                if (layoutCache == null)
                {
                    value = null;
                    return false;
                }

                return layoutCache.TryGetValue(layout, out value);
            }
        }

        private static bool NeedToPreformatMessage(object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return false;
            }

            if (parameters.Length > 3)
            {
                return true;
            }

            if (!IsSafeToDeferFormatting(parameters[0]))
            {
                return true;
            }

            if (parameters.Length >= 2)
            {
                if (!IsSafeToDeferFormatting(parameters[1]))
                {
                    return true;
                }
            }

            if (parameters.Length >= 3)
            {
                if (!IsSafeToDeferFormatting(parameters[2]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSafeToDeferFormatting(object value)
        {
            if (value == null)
            {
                return true;
            }

            return value.GetType().IsPrimitive || (value is string);
        }

        private void CalcFormattedMessage()
        {
            if (Parameters == null || Parameters.Length == 0)
            {
                formattedMessage = Message;
            }
            else
            {
                try
                {
                    formattedMessage = string.Format(FormatProvider ?? LogManager.Instance.DefaultCultureInfo(), Message, Parameters);
                }
                catch (Exception exception)
                {
                    formattedMessage = Message;
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    InternalLogger.Warn("Error when formatting a message: {0}", exception);
                }
            }
        }

        private void InitEventContext()
        {
            properties = new Dictionary<object, object>();
            eventContextAdapter = new DictionaryAdapter<object, object>(properties);
        }

        #endregion
    }
}