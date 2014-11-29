using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace NLog.Config
{
    public sealed class InstallationContext : IDisposable
    {
        private static readonly Dictionary<LogLevel, ConsoleColor> logLevel2ConsoleColor = new Dictionary<LogLevel, ConsoleColor>
        {
            { LogLevel.Trace, ConsoleColor.DarkGray },
            { LogLevel.Debug, ConsoleColor.Gray },
            { LogLevel.Info, ConsoleColor.White },
            { LogLevel.Warn, ConsoleColor.Yellow },
            { LogLevel.Error, ConsoleColor.Red },
            { LogLevel.Fatal, ConsoleColor.DarkRed },
        };

        public InstallationContext()
            : this(TextWriter.Null)
        {
        }

        public InstallationContext(TextWriter logOutput)
        {
            LogOutput = logOutput;
            Parameters = new Dictionary<string, string>();
            LogLevel = LogLevel.Info;
        }

        public LogLevel LogLevel { get; set; }

        public bool IgnoreFailures { get; set; }

        public IDictionary<string, string> Parameters { get; private set; }

        public TextWriter LogOutput { get; set; }

        public void Trace([Localizable(false)] string message, params object[] arguments)
        {
            Log(LogLevel.Trace, message, arguments);
        }

        public void Debug([Localizable(false)] string message, params object[] arguments)
        {
            Log(LogLevel.Debug, message, arguments);
        }

        public void Info([Localizable(false)] string message, params object[] arguments)
        {
            Log(LogLevel.Info, message, arguments);
        }

        public void Warning([Localizable(false)] string message, params object[] arguments)
        {
            Log(LogLevel.Warn, message, arguments);
        }

        public void Error([Localizable(false)] string message, params object[] arguments)
        {
            Log(LogLevel.Error, message, arguments);
        }

        public void Dispose()
        {
            if (LogOutput != null)
            {
                LogOutput.Close();
                LogOutput = null;
            }
        }

        public LogEventInfo CreateLogEvent()
        {
            var eventInfo = LogEventInfo.CreateNullEvent();

            foreach (var kvp in Parameters)
            {
                eventInfo.Properties.Add(kvp.Key, kvp.Value);
            }

            return eventInfo;
        }

        private void Log(LogLevel logLevel, [Localizable(false)] string message, object[] arguments)
        {
            if (logLevel >= LogLevel)
            {
                if (arguments != null && arguments.Length > 0)
                {
                    message = string.Format(CultureInfo.InvariantCulture, message, arguments);
                }

                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = logLevel2ConsoleColor[logLevel];

                try
                {
                    LogOutput.WriteLine(message);
                }
                finally
                {
                    Console.ForegroundColor = oldColor;
                }
            }
        }
    }
}