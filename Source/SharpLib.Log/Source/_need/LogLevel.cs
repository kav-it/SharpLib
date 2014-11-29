using System;
using System.Collections.Generic;
using System.Linq;

namespace NLog
{
    public sealed class LogLevel : IComparable
    {
        #region Поля

        private static readonly List<LogLevel> _levels;

        #endregion

        #region Свойства

        public static LogLevel Debug { get; private set; }

        public static LogLevel Error { get; private set; }

        public static LogLevel Fatal { get; private set; }

        public static LogLevel Info { get; private set; }

        public static LogLevel Off { get; private set; }

        public static LogLevel Trace { get; private set; }

        public static LogLevel Warn { get; private set; }

        public string Name { get; private set; }

        internal static LogLevel MaxLevel
        {
            get { return Fatal; }
        }

        internal static LogLevel MinLevel
        {
            get { return Trace; }
        }

        public int Ordinal { get; private set; }

        #endregion

        #region Конструктор

        static LogLevel()
        {
            Debug = new LogLevel("Debug", 1);
            Trace = new LogLevel("Trace", 0);
            Info = new LogLevel("Info", 2);
            Warn = new LogLevel("Warn", 3);
            Error = new LogLevel("Error", 4);
            Fatal = new LogLevel("Fatal", 5);
            Off = new LogLevel("Off", 6);

            _levels = new List<LogLevel>
            {
                Off,
                Trace,
                Info,
                Warn,
                Error,
                Fatal
            };
        }

        private LogLevel(string name, int ordinal)
        {
            Name = name;
            Ordinal = ordinal;
        }

        #endregion

        #region Методы

        public static LogLevel FromOrdinal(int ordinal)
        {
            return _levels.Single(x => x.Ordinal == ordinal);
        }

        public static LogLevel FromString(string levelName)
        {
            return _levels.Single(x => x.Name.Equals(levelName, StringComparison.OrdinalIgnoreCase));
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Ordinal;
        }

        public override bool Equals(object obj)
        {
            LogLevel other = obj as LogLevel;
            if ((object)other == null)
            {
                return false;
            }

            return Ordinal == other.Ordinal;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            LogLevel level = (LogLevel)obj;
            return Ordinal - level.Ordinal;
        }

        #endregion

        public static bool operator ==(LogLevel level1, LogLevel level2)
        {
            if (ReferenceEquals(level1, null))
            {
                return ReferenceEquals(level2, null);
            }

            if (ReferenceEquals(level2, null))
            {
                return false;
            }

            return level1.Ordinal == level2.Ordinal;
        }

        public static bool operator !=(LogLevel level1, LogLevel level2)
        {
            if (ReferenceEquals(level1, null))
            {
                return !ReferenceEquals(level2, null);
            }

            if (ReferenceEquals(level2, null))
            {
                return true;
            }

            return level1.Ordinal != level2.Ordinal;
        }

        public static bool operator >(LogLevel level1, LogLevel level2)
        {
            return level1.Ordinal > level2.Ordinal;
        }

        public static bool operator >=(LogLevel level1, LogLevel level2)
        {
            return level1.Ordinal >= level2.Ordinal;
        }

        public static bool operator <(LogLevel level1, LogLevel level2)
        {
            return level1.Ordinal < level2.Ordinal;
        }

        public static bool operator <=(LogLevel level1, LogLevel level2)
        {
            return level1.Ordinal <= level2.Ordinal;
        }
    }
}