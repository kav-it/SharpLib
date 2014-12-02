using System;
using System.ComponentModel;

using JetBrains.Annotations;

namespace SharpLib.Log
{
    public interface ILogger
    {
        #region Свойства

        string Name { get; }

        bool IsTraceEnabled { get; }

        bool IsDebugEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }

        #endregion

        #region Методы

        bool IsEnabled(LogLevel level);

        void Log(LogEventInfo logEvent);

        void Log(Type wrapperType, LogEventInfo logEvent);

        void Log<T>(LogLevel level, T value);

        void Log<T>(LogLevel level, IFormatProvider formatProvider, T value);

        void Log(LogLevel level, LogMessageGenerator messageFunc);

        [StringFormatMethod("message")]
        void Log(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        void Log(LogLevel level, [Localizable(false)] string message);

        void Log(LogLevel level, [Localizable(false)] string message, params object[] args);

        void Log(LogLevel level, [Localizable(false)] string message, Exception exception);

        [StringFormatMethod("message")]
        void Log<TArgument>(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Log<TArgument>(LogLevel level, [Localizable(false)] string message, TArgument argument);

        void Log<TArgument1, TArgument2>(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Log<TArgument1, TArgument2>(LogLevel level, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        void Log<TArgument1, TArgument2, TArgument3>(LogLevel level,
            IFormatProvider formatProvider,
            [Localizable(false)] string message,
            TArgument1 argument1,
            TArgument2 argument2,
            TArgument3 argument3);

        [StringFormatMethod("message")]
        void Log<TArgument1, TArgument2, TArgument3>(LogLevel level, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        void Trace<T>(T value);

        void Trace<T>(IFormatProvider formatProvider, T value);

        void Trace(LogMessageGenerator messageFunc);

        [StringFormatMethod("message")]
        void Trace(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        void Trace([Localizable(false)] string message);

        [StringFormatMethod("message")]
        void Trace([Localizable(false)] string message, params object[] args);

        void Trace([Localizable(false)] string message, Exception exception);

        [StringFormatMethod("message")]
        void Trace<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Trace<TArgument>([Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Trace<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Trace<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Trace<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        [StringFormatMethod("message")]
        void Trace<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        void Debug<T>(T value);

        void Debug<T>(IFormatProvider formatProvider, T value);

        void Debug(LogMessageGenerator messageFunc);

        [StringFormatMethod("message")]
        void Debug(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        void Debug([Localizable(false)] string message);

        [StringFormatMethod("message")]
        void Debug([Localizable(false)] string message, params object[] args);

        void Debug([Localizable(false)] string message, Exception exception);

        [StringFormatMethod("message")]
        void Debug<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Debug<TArgument>([Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Debug<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Debug<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        [StringFormatMethod("message")]
        void Debug<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        void Info<T>(T value);

        void Info<T>(IFormatProvider formatProvider, T value);

        void Info(LogMessageGenerator messageFunc);

        [StringFormatMethod("message")]
        void Info(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        void Info([Localizable(false)] string message);

        [StringFormatMethod("message")]
        void Info([Localizable(false)] string message, params object[] args);

        void Info([Localizable(false)] string message, Exception exception);

        [StringFormatMethod("message")]
        void Info<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Info<TArgument>([Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Info<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Info<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Info<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        [StringFormatMethod("message")]
        void Info<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        void Warn<T>(T value);

        void Warn<T>(IFormatProvider formatProvider, T value);

        void Warn(LogMessageGenerator messageFunc);

        [StringFormatMethod("message")]
        void Warn(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        void Warn([Localizable(false)] string message);

        [StringFormatMethod("message")]
        void Warn([Localizable(false)] string message, params object[] args);

        void Warn([Localizable(false)] string message, Exception exception);

        [StringFormatMethod("message")]
        void Warn<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Warn<TArgument>([Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Warn<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Warn<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        [StringFormatMethod("message")]
        void Warn<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        void Error<T>(T value);

        void Error<T>(IFormatProvider formatProvider, T value);

        void Error(LogMessageGenerator messageFunc);

        void ErrorException([Localizable(false)] string message, Exception exception);

        [StringFormatMethod("message")]
        void Error(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        void Error([Localizable(false)] string message);

        [StringFormatMethod("message")]
        void Error([Localizable(false)] string message, params object[] args);

        void Error([Localizable(false)] string message, Exception exception);

        [StringFormatMethod("message")]
        void Error<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Error<TArgument>([Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Error<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Error<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Error<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        [StringFormatMethod("message")]
        void Error<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        void Fatal<T>(T value);

        void Fatal<T>(IFormatProvider formatProvider, T value);

        void Fatal(LogMessageGenerator messageFunc);

        [StringFormatMethod("message")]
        void Fatal(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args);

        void Fatal([Localizable(false)] string message);

        [StringFormatMethod("message")]
        void Fatal([Localizable(false)] string message, params object[] args);

        void Fatal([Localizable(false)] string message, Exception exception);

        [StringFormatMethod("message")]
        void Fatal<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Fatal<TArgument>([Localizable(false)] string message, TArgument argument);

        [StringFormatMethod("message")]
        void Fatal<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Fatal<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2);

        [StringFormatMethod("message")]
        void Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        [StringFormatMethod("message")]
        void Fatal<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3);

        #endregion
    }
}