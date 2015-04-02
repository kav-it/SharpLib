using System;
using System.ComponentModel;
using System.Globalization;

namespace SharpLib.Notepad.Document
{
    [Serializable]
    [TypeConverter(typeof(TextLocationConverter))]
    public struct TextLocation : IComparable<TextLocation>, IEquatable<TextLocation>
    {
        #region Поля

        public static readonly TextLocation Empty = new TextLocation(0, 0);

        private readonly int column;

        private readonly int line;

        #endregion

        #region Свойства

        public int Line
        {
            get { return line; }
        }

        public int Column
        {
            get { return column; }
        }

        public bool IsEmpty
        {
            get { return column <= 0 && line <= 0; }
        }

        #endregion

        #region Конструктор

        public TextLocation(int line, int column)
        {
            this.line = line;
            this.column = column;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "(Line {1}, Col {0})", column, line);
        }

        public override int GetHashCode()
        {
            return unchecked(191 * column.GetHashCode() ^ line.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TextLocation))
            {
                return false;
            }
            return (TextLocation)obj == this;
        }

        public bool Equals(TextLocation other)
        {
            return this == other;
        }

        public int CompareTo(TextLocation other)
        {
            if (this == other)
            {
                return 0;
            }
            if (this < other)
            {
                return -1;
            }
            return 1;
        }

        #endregion

        public static bool operator ==(TextLocation left, TextLocation right)
        {
            return left.column == right.column && left.line == right.line;
        }

        public static bool operator !=(TextLocation left, TextLocation right)
        {
            return left.column != right.column || left.line != right.line;
        }

        public static bool operator <(TextLocation left, TextLocation right)
        {
            if (left.line < right.line)
            {
                return true;
            }
            if (left.line == right.line)
            {
                return left.column < right.column;
            }
            return false;
        }

        public static bool operator >(TextLocation left, TextLocation right)
        {
            if (left.line > right.line)
            {
                return true;
            }
            if (left.line == right.line)
            {
                return left.column > right.column;
            }
            return false;
        }

        public static bool operator <=(TextLocation left, TextLocation right)
        {
            return !(left > right);
        }

        public static bool operator >=(TextLocation left, TextLocation right)
        {
            return !(left < right);
        }
    }
}