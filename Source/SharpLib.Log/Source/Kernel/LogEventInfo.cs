using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace SharpLib.Log
{
    public class LogEventInfo
    {
        #region Поля

        public static readonly DateTime ZeroDate = DateTime.UtcNow;

        private static int _globalSequenceId;

        private readonly object _layoutCacheLock;

        private string _formattedMessage;

        private IDictionary<Layout, string> _layoutCache;

        private IDictionary<object, object> _properties;

        #endregion

        #region Свойства

        public int SequenceId { get; private set; }

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

        public string Message { get; set; }

        public object[] Parameters { get; set; }

        public IFormatProvider FormatProvider { get; set; }

        public string FormattedMessage
        {
            get
            {
                if (_formattedMessage == null)
                {
                    CalcFormattedMessage();
                }

                return _formattedMessage;
            }
        }

        public IDictionary<object, object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    InitEventContext();
                }

                return _properties;
            }
        }

        #endregion

        #region Конструктор

        public LogEventInfo()
        {
            _layoutCacheLock = new object();
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
            _layoutCacheLock = new object();
            TimeStamp = TimeSource.Current.Time;
            Level = level;
            LoggerName = loggerName;
            Message = message;
            Parameters = parameters;
            FormatProvider = formatProvider;
            Exception = exception;
            SequenceId = Interlocked.Increment(ref _globalSequenceId);

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

        public AsyncLogEventInfo WithContinuation()
        {
            return new AsyncLogEventInfo(this);
        }

        public override string ToString()
        {
            return "Log Event: Logger='" + LoggerName + "' Level=" + Level + " Message='" + FormattedMessage + "' SequenceID=" + SequenceId;
        }

        public void SetStackTrace(StackTrace stackTrace, int userStackFrame)
        {
            StackTrace = stackTrace;
            UserStackFrameNumber = userStackFrame;
        }

        internal string AddCachedLayoutValue(Layout layout, string value)
        {
            lock (_layoutCacheLock)
            {
                if (_layoutCache == null)
                {
                    _layoutCache = new Dictionary<Layout, string>();
                }

                _layoutCache[layout] = value;
            }

            return value;
        }

        internal bool TryGetCachedLayoutValue(Layout layout, out string value)
        {
            lock (_layoutCacheLock)
            {
                if (_layoutCache == null)
                {
                    value = null;
                    return false;
                }

                return _layoutCache.TryGetValue(layout, out value);
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
                _formattedMessage = Message;
            }
            else
            {
                try
                {
                    _formattedMessage = string.Format(FormatProvider ?? CultureInfo.CurrentCulture, Message, Parameters);
                }
                catch (Exception exception)
                {
                    _formattedMessage = Message;
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }
                }
            }
        }

        private void InitEventContext()
        {
            _properties = new Dictionary<object, object>();
        }

        #endregion
    }
}