using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

using NLog.Internal;
using NLog.Time;

namespace NLog.Common
{
    public static class InternalLogger
    {
        #region Поля

        private static readonly object _lockObject = new object();

        #endregion

        #region Свойства

        public static LogLevel LogLevel { get; set; }

        public static bool LogToConsole { get; set; }

        public static bool LogToConsoleError { get; set; }

        public static string LogFile { get; set; }

        public static TextWriter LogWriter { get; set; }

        public static bool IncludeTimestamp { get; set; }

        public static bool IsTraceEnabled
        {
            get { return LogLevel.Trace >= LogLevel; }
        }

        public static bool IsDebugEnabled
        {
            get { return LogLevel.Debug >= LogLevel; }
        }

        public static bool IsInfoEnabled
        {
            get { return LogLevel.Info >= LogLevel; }
        }

        public static bool IsWarnEnabled
        {
            get { return LogLevel.Warn >= LogLevel; }
        }

        public static bool IsErrorEnabled
        {
            get { return LogLevel.Error >= LogLevel; }
        }

        public static bool IsFatalEnabled
        {
            get { return LogLevel.Fatal >= LogLevel; }
        }

        #endregion

        #region Конструктор

        static InternalLogger()
        {
            LogToConsole = GetSetting("nlog.internalLogToConsole", "NLOG_INTERNAL_LOG_TO_CONSOLE", false);
            LogToConsoleError = GetSetting("nlog.internalLogToConsoleError", "NLOG_INTERNAL_LOG_TO_CONSOLE_ERROR", false);
            LogLevel = GetSetting("nlog.internalLogLevel", "NLOG_INTERNAL_LOG_LEVEL", LogLevel.Info);
            LogFile = GetSetting("nlog.internalLogFile", "NLOG_INTERNAL_LOG_FILE", string.Empty);
            Info("NLog internal logger initialized.");

            IncludeTimestamp = true;
        }

        #endregion

        #region Методы

        public static void Log(LogLevel level, string message, params object[] args)
        {
            Write(level, message, args);
        }

        public static void Log(LogLevel level, [Localizable(false)] string message)
        {
            Write(level, message, null);
        }

        public static void Trace([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Trace, message, args);
        }

        public static void Trace([Localizable(false)] string message)
        {
            Write(LogLevel.Trace, message, null);
        }

        public static void Debug([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Debug, message, args);
        }

        public static void Debug([Localizable(false)] string message)
        {
            Write(LogLevel.Debug, message, null);
        }

        public static void Info([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Info, message, args);
        }

        public static void Info([Localizable(false)] string message)
        {
            Write(LogLevel.Info, message, null);
        }

        public static void Warn([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Warn, message, args);
        }

        public static void Warn([Localizable(false)] string message)
        {
            Write(LogLevel.Warn, message, null);
        }

        public static void Error([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Error, message, args);
        }

        public static void Error([Localizable(false)] string message)
        {
            Write(LogLevel.Error, message, null);
        }

        public static void Fatal([Localizable(false)] string message, params object[] args)
        {
            Write(LogLevel.Fatal, message, args);
        }

        public static void Fatal([Localizable(false)] string message)
        {
            Write(LogLevel.Fatal, message, null);
        }

        private static void Write(LogLevel level, string message, object[] args)
        {
            if (level < LogLevel)
            {
                return;
            }

            if (string.IsNullOrEmpty(LogFile) && !LogToConsole && !LogToConsoleError && LogWriter == null)
            {
                return;
            }

            try
            {
                string formattedMessage = message;
                if (args != null)
                {
                    formattedMessage = string.Format(CultureInfo.InvariantCulture, message, args);
                }

                var builder = new StringBuilder(message.Length + 32);
                if (IncludeTimestamp)
                {
                    builder.Append(TimeSource.Current.Time.ToString("yyyy-MM-dd HH:mm:ss.ffff", CultureInfo.InvariantCulture));
                    builder.Append(" ");
                }

                builder.Append(level);
                builder.Append(" ");
                builder.Append(formattedMessage);
                string msg = builder.ToString();

                var logFile = LogFile;
                if (!string.IsNullOrEmpty(logFile))
                {
                    using (var textWriter = File.AppendText(logFile))
                    {
                        textWriter.WriteLine(msg);
                    }
                }

                var writer = LogWriter;
                if (writer != null)
                {
                    lock (_lockObject)
                    {
                        writer.WriteLine(msg);
                    }
                }

                if (LogToConsole)
                {
                    Console.WriteLine(msg);
                }

                if (LogToConsoleError)
                {
                    Console.Error.WriteLine(msg);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }
            }
        }

        private static string GetSettingString(string configName, string envName)
        {
            string settingValue = null;
            try
            {
                settingValue = Environment.GetEnvironmentVariable(envName);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }
            }

            return settingValue;
        }

        private static LogLevel GetSetting(string configName, string envName, LogLevel defaultValue)
        {
            string value = GetSettingString(configName, envName);
            if (value == null)
            {
                return defaultValue;
            }

            try
            {
                return LogLevel.FromString(value);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                return defaultValue;
            }
        }

        private static T GetSetting<T>(string configName, string envName, T defaultValue)
        {
            string value = GetSettingString(configName, envName);
            if (value == null)
            {
                return defaultValue;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                return defaultValue;
            }
        }

        #endregion
    }
}