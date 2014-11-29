using System;
using System.ComponentModel;

using JetBrains.Annotations;

using NLog.Internal;

namespace NLog
{
    public class Logger
    {
        private readonly Type _loggerType;

        private volatile LoggerConfiguration _configuration;

        private volatile bool _isTraceEnabled;

        private volatile bool _isDebugEnabled;

        private volatile bool _isInfoEnabled;

        private volatile bool _isWarnEnabled;

        private volatile bool _isErrorEnabled;

        private volatile bool _isFatalEnabled;

        protected internal Logger()
        {
            _loggerType = typeof(Logger);
        }

        public event EventHandler<EventArgs> LoggerReconfigured;

        public string Name { get; private set; }

        public LogFactory Factory { get; private set; }

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

        public bool IsEnabled(LogLevel level)
        {
            if (level == null)
            {
                throw new InvalidOperationException("Log level must be defined");
            }

            return GetTargetsForLevel(level) != null;
        }

        public void Log(LogEventInfo logEvent)
        {
            if (IsEnabled(logEvent.Level))
            {
                WriteToTargets(logEvent);
            }
        }

        public void Log(Type wrapperType, LogEventInfo logEvent)
        {
            if (IsEnabled(logEvent.Level))
            {
                WriteToTargets(wrapperType, logEvent);
            }
        }

        #region Log() overloads

        public void Log<T>(LogLevel level, T value)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, null, value);
            }
        }

        public void Log<T>(LogLevel level, IFormatProvider formatProvider, T value)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, value);
            }
        }

        public void Log(LogLevel level, LogMessageGenerator messageFunc)
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
        public void Log(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, args);
            }
        }

        public void Log(LogLevel level, [Localizable(false)] string message)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, null, message);
            }
        }

        public void Log(LogLevel level, [Localizable(false)] string message, params object[] args)
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
        public void Log<TArgument>(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        public void Log<TArgument>(LogLevel level, [Localizable(false)] string message, TArgument argument)
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

        public void Log<TArgument1, TArgument2>(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Log<TArgument1, TArgument2>(LogLevel level, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, new object[] { argument1, argument2 });
            }
        }

        public void Log<TArgument1, TArgument2, TArgument3>(LogLevel level,
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
        public void Log<TArgument1, TArgument2, TArgument3>(LogLevel level, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, new object[] { argument1, argument2, argument3 });
            }
        }

        #endregion

        #region Trace() overloads

        public void Trace<T>(T value)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, null, value);
            }
        }

        public void Trace<T>(IFormatProvider formatProvider, T value)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, value);
            }
        }

        public void Trace(LogMessageGenerator messageFunc)
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
        public void Trace(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, args);
            }
        }

        public void Trace([Localizable(false)] string message)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, null, message);
            }
        }

        [StringFormatMethod("message")]
        public void Trace([Localizable(false)] string message, params object[] args)
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
        public void Trace<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        public void Trace<TArgument>([Localizable(false)] string message, TArgument argument)
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
        public void Trace<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Trace<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Trace<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        public void Trace<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, new object[] { argument1, argument2, argument3 });
            }
        }

        #endregion

        #region Debug() overloads

        public void Debug<T>(T value)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, null, value);
            }
        }

        public void Debug<T>(IFormatProvider formatProvider, T value)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, value);
            }
        }

        public void Debug(LogMessageGenerator messageFunc)
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
        public void Debug(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, args);
            }
        }

        public void Debug([Localizable(false)] string message)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, null, message);
            }
        }

        [StringFormatMethod("message")]
        public void Debug([Localizable(false)] string message, params object[] args)
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
        public void Debug<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        public void Debug<TArgument>([Localizable(false)] string message, TArgument argument)
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
        public void Debug<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Debug<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        public void Debug<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, new object[] { argument1, argument2, argument3 });
            }
        }

        #endregion

        #region Info() overloads

        public void Info<T>(T value)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, null, value);
            }
        }

        public void Info<T>(IFormatProvider formatProvider, T value)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, value);
            }
        }

        public void Info(LogMessageGenerator messageFunc)
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
        public void Info(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, args);
            }
        }

        public void Info([Localizable(false)] string message)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, null, message);
            }
        }

        [StringFormatMethod("message")]
        public void Info([Localizable(false)] string message, params object[] args)
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
        public void Info<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        public void Info<TArgument>([Localizable(false)] string message, TArgument argument)
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
        public void Info<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Info<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Info<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        public void Info<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, new object[] { argument1, argument2, argument3 });
            }
        }

        #endregion

        #region Warn() overloads

        public void Warn<T>(T value)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, null, value);
            }
        }

        public void Warn<T>(IFormatProvider formatProvider, T value)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, value);
            }
        }

        public void Warn(LogMessageGenerator messageFunc)
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
        public void Warn(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, args);
            }
        }

        public void Warn([Localizable(false)] string message)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, null, message);
            }
        }

        [StringFormatMethod("message")]
        public void Warn([Localizable(false)] string message, params object[] args)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, args);
            }
        }

        public void Warn([Localizable(false)] string message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, exception);
            }
        }

        [StringFormatMethod("message")]
        public void Warn<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
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
        public void Warn<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Warn<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        public void Warn<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, new object[] { argument1, argument2, argument3 });
            }
        }

        #endregion

        #region Error() overloads

        public void Error<T>(T value)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, null, value);
            }
        }

        public void Error<T>(IFormatProvider formatProvider, T value)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, value);
            }
        }

        public void Error(LogMessageGenerator messageFunc)
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

        public void ErrorException([Localizable(false)] string message, Exception exception)
        {
            Error(message, exception);
        }

        [StringFormatMethod("message")]
        public void Error(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, args);
            }
        }

        public void Error([Localizable(false)] string message)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, null, message);
            }
        }

        [StringFormatMethod("message")]
        public void Error([Localizable(false)] string message, params object[] args)
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
        public void Error<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        public void Error<TArgument>([Localizable(false)] string message, TArgument argument)
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
        public void Error<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Error<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Error<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        public void Error<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, new object[] { argument1, argument2, argument3 });
            }
        }

        #endregion

        #region Fatal() overloads

        public void Fatal<T>(T value)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, null, value);
            }
        }

        public void Fatal<T>(IFormatProvider formatProvider, T value)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, value);
            }
        }

        public void Fatal(LogMessageGenerator messageFunc)
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
        public void Fatal(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, args);
            }
        }

        public void Fatal([Localizable(false)] string message)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, null, message);
            }
        }

        [StringFormatMethod("message")]
        public void Fatal([Localizable(false)] string message, params object[] args)
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
        public void Fatal<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[] { argument });
            }
        }

        [StringFormatMethod("message")]
        public void Fatal<TArgument>([Localizable(false)] string message, TArgument argument)
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
        public void Fatal<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Fatal<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, new object[] { argument1, argument2 });
            }
        }

        [StringFormatMethod("message")]
        public void Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[] { argument1, argument2, argument3 });
            }
        }

        [StringFormatMethod("message")]
        public void Fatal<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, new object[] { argument1, argument2, argument3 });
            }
        }

        #endregion

        public void Swallow(Action action)
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

        public T Swallow<T>(Func<T> func)
        {
            return Swallow(func, default(T));
        }

        public T Swallow<T>(Func<T> func, T fallback)
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

#if ASYNC_SUPPORTED
    
    
    
    
        public async Task SwallowAsync(Func<Task> asyncAction)
        {
	        try
	        {
		        await asyncAction();
	        }
	        catch (Exception e)
	        {
		        Error(e);
	        }
        }

        
        
        
        
        
        
        
        public async Task<T> SwallowAsync<T>(Func<Task<T>> asyncFunc)
        {
	        return await SwallowAsync(asyncFunc, default(T));
        }

        
        
        
        
        
        
        
        
        public async Task<T> SwallowAsync<T>(Func<Task<T>> asyncFunc, T fallback)
        {
            try
            {
                return await asyncFunc();
            }
            catch (Exception e)
            {
                Error(e);
                return fallback;
            }
        }
#endif

        internal void Initialize(string name, LoggerConfiguration loggerConfiguration, LogFactory factory)
        {
            Name = name;
            Factory = factory;
            SetConfiguration(loggerConfiguration);
        }

        internal void WriteToTargets(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, object[] args)
        {
            LoggerImpl.Write(_loggerType, GetTargetsForLevel(level), LogEventInfo.Create(level, Name, formatProvider, message, args), Factory);
        }

        internal void WriteToTargets(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message)
        {
            var logEvent = LogEventInfo.Create(level, Name, formatProvider, message, null);
            LoggerImpl.Write(_loggerType, GetTargetsForLevel(level), logEvent, Factory);
        }

        internal void WriteToTargets<T>(LogLevel level, IFormatProvider formatProvider, T value)
        {
            LoggerImpl.Write(_loggerType, GetTargetsForLevel(level), LogEventInfo.Create(level, Name, formatProvider, value), Factory);
        }

        internal void WriteToTargets(LogLevel level, [Localizable(false)] string message, Exception ex)
        {
            LoggerImpl.Write(_loggerType, GetTargetsForLevel(level), LogEventInfo.Create(level, Name, message, ex), Factory);
        }

        internal void WriteToTargets(LogLevel level, [Localizable(false)] string message, object[] args)
        {
            WriteToTargets(level, null, message, args);
        }

        internal void WriteToTargets(LogEventInfo logEvent)
        {
            LoggerImpl.Write(_loggerType, GetTargetsForLevel(logEvent.Level), logEvent, Factory);
        }

        internal void WriteToTargets(Type wrapperType, LogEventInfo logEvent)
        {
            LoggerImpl.Write(wrapperType, GetTargetsForLevel(logEvent.Level), logEvent, Factory);
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
    }
}