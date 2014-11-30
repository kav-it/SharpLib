using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace SharpLib.Log
{
    [LogConfigurationItem]
    public class LoggingRule
    {
        #region Перечисления

        internal enum MatchMode
        {
            All,

            None,

            Equals,

            StartsWith,

            EndsWith,

            Contains,
        }

        #endregion

        #region Поля

        private readonly bool[] logLevels = new bool[LogLevel.MaxLevel.Ordinal + 1];

        private string loggerNameMatchArgument;

        private MatchMode loggerNameMatchMode;

        private string loggerNamePattern;

        #endregion

        #region Свойства

        public IList<Target> Targets { get; private set; }

        public IList<LoggingRule> ChildRules { get; private set; }

        public IList<Filter> Filters { get; private set; }

        public bool Final { get; set; }

        public string LoggerNamePattern
        {
            get { return loggerNamePattern; }

            set
            {
                loggerNamePattern = value;
                int firstPos = loggerNamePattern.IndexOf('*');
                int lastPos = loggerNamePattern.LastIndexOf('*');

                if (firstPos < 0)
                {
                    loggerNameMatchMode = MatchMode.Equals;
                    loggerNameMatchArgument = value;
                    return;
                }

                if (firstPos == lastPos)
                {
                    string before = LoggerNamePattern.Substring(0, firstPos);
                    string after = LoggerNamePattern.Substring(firstPos + 1);

                    if (before.Length > 0)
                    {
                        loggerNameMatchMode = MatchMode.StartsWith;
                        loggerNameMatchArgument = before;
                        return;
                    }

                    if (after.Length > 0)
                    {
                        loggerNameMatchMode = MatchMode.EndsWith;
                        loggerNameMatchArgument = after;
                        return;
                    }

                    return;
                }

                if (firstPos == 0 && lastPos == LoggerNamePattern.Length - 1)
                {
                    string text = LoggerNamePattern.Substring(1, LoggerNamePattern.Length - 2);
                    loggerNameMatchMode = MatchMode.Contains;
                    loggerNameMatchArgument = text;
                    return;
                }

                loggerNameMatchMode = MatchMode.None;
                loggerNameMatchArgument = string.Empty;
            }
        }

        public ReadOnlyCollection<LogLevel> Levels
        {
            get
            {
                var levels = new List<LogLevel>();

                for (int i = LogLevel.MinLevel.Ordinal; i <= LogLevel.MaxLevel.Ordinal; ++i)
                {
                    if (logLevels[i])
                    {
                        levels.Add(LogLevel.FromOrdinal(i));
                    }
                }

                return levels.AsReadOnly();
            }
        }

        #endregion

        #region Конструктор

        public LoggingRule()
        {
            Filters = new List<Filter>();
            ChildRules = new List<LoggingRule>();
            Targets = new List<Target>();
        }

        public LoggingRule(string loggerNamePattern, LogLevel minLevel, Target target)
        {
            Filters = new List<Filter>();
            ChildRules = new List<LoggingRule>();
            Targets = new List<Target>();
            LoggerNamePattern = loggerNamePattern;
            Targets.Add(target);
            for (int i = minLevel.Ordinal; i <= LogLevel.MaxLevel.Ordinal; ++i)
            {
                EnableLoggingForLevel(LogLevel.FromOrdinal(i));
            }
        }

        public LoggingRule(string loggerNamePattern, Target target)
        {
            Filters = new List<Filter>();
            ChildRules = new List<LoggingRule>();
            Targets = new List<Target>();
            LoggerNamePattern = loggerNamePattern;
            Targets.Add(target);
        }

        #endregion

        #region Методы

        public void EnableLoggingForLevel(LogLevel level)
        {
            logLevels[level.Ordinal] = true;
        }

        public void DisableLoggingForLevel(LogLevel level)
        {
            logLevels[level.Ordinal] = false;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(CultureInfo.InvariantCulture, "logNamePattern: ({0}:{1})", loggerNameMatchArgument, loggerNameMatchMode);
            sb.Append(" levels: [ ");
            for (int i = 0; i < logLevels.Length; ++i)
            {
                if (logLevels[0])
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0} ", LogLevel.FromOrdinal(i));
                }
            }

            sb.Append("] appendTo: [ ");
            foreach (Target app in Targets)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0} ", app.Name);
            }

            sb.Append("]");
            return sb.ToString();
        }

        public bool IsLoggingEnabledForLevel(LogLevel level)
        {
            return logLevels[level.Ordinal];
        }

        public bool NameMatches(string loggerName)
        {
            switch (loggerNameMatchMode)
            {
                case MatchMode.All:
                    return true;

                default:
                case MatchMode.None:
                    return false;

                case MatchMode.Equals:
                    return loggerName.Equals(loggerNameMatchArgument, StringComparison.Ordinal);

                case MatchMode.StartsWith:
                    return loggerName.StartsWith(loggerNameMatchArgument, StringComparison.Ordinal);

                case MatchMode.EndsWith:
                    return loggerName.EndsWith(loggerNameMatchArgument, StringComparison.Ordinal);

                case MatchMode.Contains:
                    return loggerName.IndexOf(loggerNameMatchArgument, StringComparison.Ordinal) >= 0;
            }
        }

        #endregion
    }
}