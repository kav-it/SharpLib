using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;

using NLog.Time;

namespace NLog
{
    public class NLogTraceListener : TraceListener
    {
        #region Поля

        private static readonly Assembly systemAssembly = typeof(Trace).Assembly;

        private bool attributesLoaded;

        private bool autoLoggerName;

        private LogLevel defaultLogLevel = LogLevel.Debug;

        private bool disableFlush;

        private LogLevel forceLogLevel;

        private LogFactory logFactory;

        #endregion

        #region Свойства

        public LogFactory LogFactory
        {
            get
            {
                InitAttributes();
                return logFactory;
            }

            set
            {
                attributesLoaded = true;
                logFactory = value;
            }
        }

        public LogLevel DefaultLogLevel
        {
            get
            {
                InitAttributes();
                return defaultLogLevel;
            }

            set
            {
                attributesLoaded = true;
                defaultLogLevel = value;
            }
        }

        public LogLevel ForceLogLevel
        {
            get
            {
                InitAttributes();
                return forceLogLevel;
            }

            set
            {
                attributesLoaded = true;
                forceLogLevel = value;
            }
        }

        public bool DisableFlush
        {
            get
            {
                InitAttributes();
                return disableFlush;
            }

            set
            {
                attributesLoaded = true;
                disableFlush = value;
            }
        }

        public override bool IsThreadSafe
        {
            get { return true; }
        }

        public bool AutoLoggerName
        {
            get
            {
                InitAttributes();
                return autoLoggerName;
            }

            set
            {
                attributesLoaded = true;
                autoLoggerName = value;
            }
        }

        #endregion

        #region Методы

        public override void Write(string message)
        {
            ProcessLogEventInfo(DefaultLogLevel, null, message, null, null, TraceEventType.Resume, null);
        }

        public override void WriteLine(string message)
        {
            ProcessLogEventInfo(DefaultLogLevel, null, message, null, null, TraceEventType.Resume, null);
        }

        public override void Close()
        {
        }

        public override void Fail(string message)
        {
            ProcessLogEventInfo(LogLevel.Error, null, message, null, null, TraceEventType.Error, null);
        }

        public override void Fail(string message, string detailMessage)
        {
            ProcessLogEventInfo(LogLevel.Error, null, message + " " + detailMessage, null, null, TraceEventType.Error, null);
        }

        public override void Flush()
        {
            if (!DisableFlush)
            {
                if (LogFactory != null)
                {
                    LogFactory.Flush();
                }
                else
                {
                    LogManager.Flush();
                }
            }
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            TraceData(eventCache, source, eventType, id, new[] { data });
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < data.Length; ++i)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append("{");
                sb.Append(i);
                sb.Append("}");
            }

            ProcessLogEventInfo(TranslateLogLevel(eventType), source, sb.ToString(), data, id, eventType, null);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            ProcessLogEventInfo(TranslateLogLevel(eventType), source, string.Empty, null, id, eventType, null);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            ProcessLogEventInfo(TranslateLogLevel(eventType), source, format, args, id, eventType, null);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            ProcessLogEventInfo(TranslateLogLevel(eventType), source, message, null, id, eventType, null);
        }

        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            ProcessLogEventInfo(LogLevel.Debug, source, message, null, id, TraceEventType.Transfer, relatedActivityId);
        }

        protected override string[] GetSupportedAttributes()
        {
            return new[] { "defaultLogLevel", "autoLoggerName", "forceLogLevel", "disableFlush" };
        }

        private static LogLevel TranslateLogLevel(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Verbose:
                    return LogLevel.Trace;

                case TraceEventType.Information:
                    return LogLevel.Info;

                case TraceEventType.Warning:
                    return LogLevel.Warn;

                case TraceEventType.Error:
                    return LogLevel.Error;

                case TraceEventType.Critical:
                    return LogLevel.Fatal;

                default:
                    return LogLevel.Debug;
            }
        }

        protected virtual void ProcessLogEventInfo(LogLevel logLevel,
            string loggerName,
            [Localizable(false)] string message,
            object[] arguments,
            int? eventId,
            TraceEventType? eventType,
            Guid? relatedActiviyId)
        {
            var ev = new LogEventInfo();

            ev.LoggerName = (loggerName ?? Name) ?? string.Empty;

            if (AutoLoggerName)
            {
                var stack = new StackTrace();
                int userFrameIndex = -1;
                MethodBase userMethod = null;

                for (int i = 0; i < stack.FrameCount; ++i)
                {
                    var frame = stack.GetFrame(i);
                    var method = frame.GetMethod();

                    if (method.DeclaringType == GetType())
                    {
                        continue;
                    }

                    if (method.DeclaringType.Assembly == systemAssembly)
                    {
                        continue;
                    }

                    userFrameIndex = i;
                    userMethod = method;
                    break;
                }

                if (userFrameIndex >= 0)
                {
                    ev.SetStackTrace(stack, userFrameIndex);
                    if (userMethod.DeclaringType != null)
                    {
                        ev.LoggerName = userMethod.DeclaringType.FullName;
                    }
                }
            }

            if (eventType.HasValue)
            {
                ev.Properties.Add("EventType", eventType.Value);
            }

            if (relatedActiviyId.HasValue)
            {
                ev.Properties.Add("RelatedActivityID", relatedActiviyId.Value);
            }

            ev.TimeStamp = TimeSource.Current.Time;
            ev.Message = message;
            ev.Parameters = arguments;
            ev.Level = forceLogLevel ?? logLevel;

            if (eventId.HasValue)
            {
                ev.Properties.Add("EventID", eventId.Value);
            }

            Logger logger;
            if (LogFactory != null)
            {
                logger = LogFactory.GetLogger(ev.LoggerName);
            }
            else
            {
                logger = LogManager.GetLogger(ev.LoggerName);
            }

            logger.Log(ev);
        }

        private void InitAttributes()
        {
            if (!attributesLoaded)
            {
                attributesLoaded = true;
                foreach (DictionaryEntry de in Attributes)
                {
                    var key = (string)de.Key;
                    var value = (string)de.Value;

                    switch (key.ToUpperInvariant())
                    {
                        case "DEFAULTLOGLEVEL":
                            defaultLogLevel = LogLevel.FromString(value);
                            break;

                        case "FORCELOGLEVEL":
                            forceLogLevel = LogLevel.FromString(value);
                            break;

                        case "AUTOLOGGERNAME":
                            AutoLoggerName = XmlConvert.ToBoolean(value);
                            break;

                        case "DISABLEFLUSH":
                            disableFlush = Boolean.Parse(value);
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
