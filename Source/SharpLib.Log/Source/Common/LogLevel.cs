using System;

using NLog.Internal;

namespace NLog
{
    public sealed class LogLevel : IComparable
    {
        #region Поля

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
        public static readonly LogLevel Debug = new LogLevel("Debug", 1);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
        public static readonly LogLevel Error = new LogLevel("Error", 4);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
        public static readonly LogLevel Fatal = new LogLevel("Fatal", 5);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
        public static readonly LogLevel Info = new LogLevel("Info", 2);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
        public static readonly LogLevel Off = new LogLevel("Off", 6);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
        public static readonly LogLevel Trace = new LogLevel("Trace", 0);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
        public static readonly LogLevel Warn = new LogLevel("Warn", 3);

        private readonly string name;

        private readonly int ordinal;

        #endregion

        #region Свойства

        public string Name
        {
            get { return name; }
        }

        internal static LogLevel MaxLevel
        {
            get { return Fatal; }
        }

        internal static LogLevel MinLevel
        {
            get { return Trace; }
        }

        public int Ordinal
        {
            get { return ordinal; }
        }

        #endregion

        #region Конструктор

        private LogLevel()
        {
        }

        private LogLevel(string name, int ordinal)
        {
            this.name = name;
            this.ordinal = ordinal;
        }

        #endregion

        #region Методы

        public static LogLevel FromOrdinal(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return Trace;
                case 1:
                    return Debug;
                case 2:
                    return Info;
                case 3:
                    return Warn;
                case 4:
                    return Error;
                case 5:
                    return Fatal;
                case 6:
                    return Off;

                default:
                    throw new ArgumentException("Invalid ordinal.");
            }
        }

        public static LogLevel FromString(string levelName)
        {
            if (levelName == null)
            {
                throw new ArgumentNullException("levelName");
            }

            if (levelName.Equals("Trace", StringComparison.OrdinalIgnoreCase))
            {
                return Trace;
            }

            if (levelName.Equals("Debug", StringComparison.OrdinalIgnoreCase))
            {
                return Debug;
            }

            if (levelName.Equals("Info", StringComparison.OrdinalIgnoreCase))
            {
                return Info;
            }

            if (levelName.Equals("Warn", StringComparison.OrdinalIgnoreCase))
            {
                return Warn;
            }

            if (levelName.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                return Error;
            }

            if (levelName.Equals("Fatal", StringComparison.OrdinalIgnoreCase))
            {
                return Fatal;
            }

            if (levelName.Equals("Off", StringComparison.OrdinalIgnoreCase))
            {
                return Off;
            }

            throw new ArgumentException("Unknown log level: " + levelName);
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
            ParameterUtils.AssertNotNull(level1, "level1");
            ParameterUtils.AssertNotNull(level2, "level2");

            return level1.Ordinal > level2.Ordinal;
        }

        public static bool operator >=(LogLevel level1, LogLevel level2)
        {
            ParameterUtils.AssertNotNull(level1, "level1");
            ParameterUtils.AssertNotNull(level2, "level2");

            return level1.Ordinal >= level2.Ordinal;
        }

        public static bool operator <(LogLevel level1, LogLevel level2)
        {
            ParameterUtils.AssertNotNull(level1, "level1");
            ParameterUtils.AssertNotNull(level2, "level2");

            return level1.Ordinal < level2.Ordinal;
        }

        public static bool operator <=(LogLevel level1, LogLevel level2)
        {
            ParameterUtils.AssertNotNull(level1, "level1");
            ParameterUtils.AssertNotNull(level2, "level2");

            return level1.Ordinal <= level2.Ordinal;
        }
    }
}