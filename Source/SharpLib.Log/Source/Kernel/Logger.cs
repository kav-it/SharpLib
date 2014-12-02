using System;
using System.ComponentModel;

using JetBrains.Annotations;

namespace SharpLib.Log
{
    internal class Logger : ILogger
    {
        #region Поля

        private readonly Type _loggerType;

        private volatile LoggerConfiguration _configuration;

        private volatile bool _isDebugEnabled;

        private volatile bool _isErrorEnabled;

        private volatile bool _isFatalEnabled;

        private volatile bool _isInfoEnabled;

        private volatile bool _isTraceEnabled;

        private volatile bool _isWarnEnabled;

        #endregion

        #region Свойства

        public string Name { get; private set; }

        public bool IsTraceEnabled
        {
            get { return _isTraceEnabled; }
        }

        public bool IsDebugEnabled
        {
            get { return _isDebugEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return _isInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return _isWarnEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return _isErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return _isFatalEnabled; }
        }

        #endregion

        #region События

        internal event EventHandler<EventArgs> LoggerReconfigured;

        #endregion

        #region Конструктор

        protected internal Logger()
        {
            _loggerType = typeof(Logger);
        }

        #endregion

        #region Методы

        public bool IsEnabled(LogLevel level)
        {
            if (level == null)
            {
                throw new InvalidOperationException("Log level must be defined");
            }

            return GetTargetsForLevel(level) != null;
        }

        void ILogger.Log(LogEventInfo logEvent)
        {
            if (IsEnabled(logEvent.Level))
            {
                WriteToTargets(logEvent);
            }
        }

        void ILogger.Log(Type wrapperType, LogEventInfo logEvent)
        {
            if (IsEnabled(logEvent.Level))
            {
                WriteToTargets(wrapperType, logEvent);
            }
        }

        void ILogger.Log<T>(LogLevel level, T value)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, null, value);
            }
        }

        void ILogger.Log<T>(LogLevel level, IFormatProvider formatProvider, T value)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, value);
            }
        }

        void ILogger.Log(LogLevel level, LogMessageGenerator messageFunc)
        {
            if (IsEnabled(level))
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(level, null, messageFunc());
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Log(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, args);
            }
        }

        void ILogger.Log(LogLevel level, [Localizable(false)] string message)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, null, message);
            }
        }

        void ILogger.Log(LogLevel level, [Localizable(false)] string message, params object[] args)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, args);
            }
        }

        public void Log(LogLevel level, [Localizable(false)] string message, Exception exception)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, exception);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Log<TArgument>(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Log<TArgument>(LogLevel level, [Localizable(false)] string message, TArgument argument)
        {
            if (IsEnabled(level))
            {
                var exceptionCandidate = argument as Exception;
                if (exceptionCandidate != null)
                {
                    Log(level, message, exceptionCandidate);
                    return;
                }

                WriteToTargets(level, message, new object[] { argument });
            }
        }

        void ILogger.Log<TArgument1, TArgument2>(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Log<TArgument1, TArgument2>(LogLevel level, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, new object[] { argument1, argument2 });
            }
        }

        void ILogger.Log<TArgument1, TArgument2, TArgument3>(LogLevel level,
            IFormatProvider formatProvider,
            [Localizable(false)] string message,
            TArgument1 argument1,
            TArgument2 argument2,
            TArgument3 argument3)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Log<TArgument1, TArgument2, TArgument3>(LogLevel level, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, new object[] { argument1, argument2, argument3 });
            }
        }

        void ILogger.Trace<T>(T value)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, null, value);
            }
        }

        void ILogger.Trace<T>(IFormatProvider formatProvider, T value)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, value);
            }
        }

        void ILogger.Trace(LogMessageGenerator messageFunc)
        {
            if (IsTraceEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Trace, null, messageFunc());
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Trace(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, args);
            }
        }

        void ILogger.Trace([Localizable(false)] string message)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, null, message);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Trace([Localizable(false)] string message, params object[] args)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, args);
            }
        }

        public void Trace([Localizable(false)] string message, Exception exception)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, exception);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Trace<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Trace<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsTraceEnabled)
            {
                var exceptionCandidate = argument as Exception;
                if (exceptionCandidate != null)
                {
                    Trace(message, exceptionCandidate);
                    return;
                }

                WriteToTargets(LogLevel.Trace, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Trace<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Trace<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Trace<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Trace<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, new object[] { argument1, argument2, argument3 });
            }
        }

        void ILogger.Debug<T>(T value)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, null, value);
            }
        }

        void ILogger.Debug<T>(IFormatProvider formatProvider, T value)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, value);
            }
        }

        void ILogger.Debug(LogMessageGenerator messageFunc)
        {
            if (IsDebugEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Debug, null, messageFunc());
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Debug(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, args);
            }
        }

        void ILogger.Debug([Localizable(false)] string message)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, null, message);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Debug([Localizable(false)] string message, params object[] args)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, args);
            }
        }

        public void Debug([Localizable(false)] string message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, exception);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Debug<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Debug<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsDebugEnabled)
            {
                var exceptionCandidate = argument as Exception;
                if (exceptionCandidate != null)
                {
                    Debug(message, exceptionCandidate);
                    return;
                }

                WriteToTargets(LogLevel.Debug, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Debug<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Debug<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Debug<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, new object[] { argument1, argument2, argument3 });
            }
        }

        void ILogger.Info<T>(T value)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, null, value);
            }
        }

        void ILogger.Info<T>(IFormatProvider formatProvider, T value)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, value);
            }
        }

        void ILogger.Info(LogMessageGenerator messageFunc)
        {
            if (IsInfoEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Info, null, messageFunc());
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Info(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, args);
            }
        }

        void ILogger.Info([Localizable(false)] string message)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, null, message);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Info([Localizable(false)] string message, params object[] args)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, args);
            }
        }

        public void Info([Localizable(false)] string message, Exception exception)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, exception);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Info<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Info<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsInfoEnabled)
            {
                var exceptionCandidate = argument as Exception;
                if (exceptionCandidate != null)
                {
                    Info(message, exceptionCandidate);
                    return;
                }

                WriteToTargets(LogLevel.Info, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Info<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Info<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Info<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Info<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, new object[] { argument1, argument2, argument3 });
            }
        }

        void ILogger.Warn<T>(T value)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, null, value);
            }
        }

        void ILogger.Warn<T>(IFormatProvider formatProvider, T value)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, value);
            }
        }

        void ILogger.Warn(LogMessageGenerator messageFunc)
        {
            if (IsWarnEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Warn, null, messageFunc());
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Warn(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, args);
            }
        }

        void ILogger.Warn([Localizable(false)] string message)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, null, message);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Warn([Localizable(false)] string message, params object[] args)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, args);
            }
        }

        void ILogger.Warn([Localizable(false)] string message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, exception);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Warn<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        public void Warn<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsWarnEnabled)
            {
                var exceptionCandidate = argument as Exception;
                if (exceptionCandidate != null)
                {
                    Warn(message, exceptionCandidate);
                    return;
                }

                WriteToTargets(LogLevel.Warn, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Warn<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Warn<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Warn<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, new object[] { argument1, argument2, argument3 });
            }
        }

        public void Error<T>(T value)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, null, value);
            }
        }

        void ILogger.Error<T>(IFormatProvider formatProvider, T value)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, value);
            }
        }

        void ILogger.Error(LogMessageGenerator messageFunc)
        {
            if (IsErrorEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Error, null, messageFunc());
            }
        }

        void ILogger.ErrorException([Localizable(false)] string message, Exception exception)
        {
            Error(message, exception);
        }

        [StringFormatMethod("message")]
        void ILogger.Error(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, args);
            }
        }

        void ILogger.Error([Localizable(false)] string message)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, null, message);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Error([Localizable(false)] string message, params object[] args)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, args);
            }
        }

        public void Error([Localizable(false)] string message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, exception);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Error<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Error<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            var exceptionCandidate = argument as Exception;
            if (exceptionCandidate != null)
            {
                Error(message, exceptionCandidate);
                return;
            }

            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Error<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Error<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Error<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Error<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, new object[] { argument1, argument2, argument3 });
            }
        }

        void ILogger.Fatal<T>(T value)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, null, value);
            }
        }

        void ILogger.Fatal<T>(IFormatProvider formatProvider, T value)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, value);
            }
        }

        void ILogger.Fatal(LogMessageGenerator messageFunc)
        {
            if (IsFatalEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Fatal, null, messageFunc());
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Fatal(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, args);
            }
        }

        void ILogger.Fatal([Localizable(false)] string message)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, null, message);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Fatal([Localizable(false)] string message, params object[] args)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, args);
            }
        }

        public void Fatal([Localizable(false)] string message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, exception);
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Fatal<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Fatal<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsFatalEnabled)
            {
                var exceptionCandidate = argument as Exception;
                if (exceptionCandidate != null)
                {
                    Fatal(message, exceptionCandidate);
                    return;
                }

                WriteToTargets(LogLevel.Fatal, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Fatal<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Fatal<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        void ILogger.Fatal<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, new object[] { argument1, argument2, argument3 });
            }
        }

        internal void Swallow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Error(e);
            }
        }

        internal T Swallow<T>(Func<T> func)
        {
            return Swallow(func, default(T));
        }

        internal T Swallow<T>(Func<T> func, T fallback)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Error(e);
                return fallback;
            }
        }

        internal void Initialize(string name, LoggerConfiguration loggerConfiguration)
        {
            Name = name;
            SetConfiguration(loggerConfiguration);
        }

        internal void WriteToTargets(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, object[] args)
        {
            LoggerImpl.Write(_loggerType, GetTargetsForLevel(level), LogEventInfo.Create(level, Name, formatProvider, message, args));
        }

        internal void WriteToTargets(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message)
        {
            var logEvent = LogEventInfo.Create(level, Name, formatProvider, message, null);
            LoggerImpl.Write(_loggerType, GetTargetsForLevel(level), logEvent);
        }

        internal void WriteToTargets<T>(LogLevel level, IFormatProvider formatProvider, T value)
        {
            LoggerImpl.Write(_loggerType, GetTargetsForLevel(level), LogEventInfo.Create(level, Name, formatProvider, value));
        }

        internal void WriteToTargets(LogLevel level, [Localizable(false)] string message, Exception ex)
        {
            LoggerImpl.Write(_loggerType, GetTargetsForLevel(level), LogEventInfo.Create(level, Name, message, ex));
        }

        internal void WriteToTargets(LogLevel level, [Localizable(false)] string message, object[] args)
        {
            WriteToTargets(level, null, message, args);
        }

        internal void WriteToTargets(LogEventInfo logEvent)
        {
            LoggerImpl.Write(_loggerType, GetTargetsForLevel(logEvent.Level), logEvent);
        }

        internal void WriteToTargets(Type wrapperType, LogEventInfo logEvent)
        {
            LoggerImpl.Write(wrapperType, GetTargetsForLevel(logEvent.Level), logEvent);
        }

        internal void SetConfiguration(LoggerConfiguration newConfiguration)
        {
            _configuration = newConfiguration;

            _isTraceEnabled = newConfiguration.IsEnabled(LogLevel.Trace);
            _isDebugEnabled = newConfiguration.IsEnabled(LogLevel.Debug);
            _isInfoEnabled = newConfiguration.IsEnabled(LogLevel.Info);
            _isWarnEnabled = newConfiguration.IsEnabled(LogLevel.Warn);
            _isErrorEnabled = newConfiguration.IsEnabled(LogLevel.Error);
            _isFatalEnabled = newConfiguration.IsEnabled(LogLevel.Fatal);

            var loggerReconfiguredDelegate = LoggerReconfigured;

            if (loggerReconfiguredDelegate != null)
            {
                loggerReconfiguredDelegate(this, new EventArgs());
            }
        }

        private TargetWithFilterChain GetTargetsForLevel(LogLevel level)
        {
            return _configuration.GetTargetsForLevel(level);
        }

        #endregion
    }
}