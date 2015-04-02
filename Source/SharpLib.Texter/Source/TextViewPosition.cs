using System;
using System.Globalization;

using SharpLib.Texter.Document;

namespace SharpLib.Texter
{
    public struct TextViewPosition : IEquatable<TextViewPosition>, IComparable<TextViewPosition>
    {
        #region Поля

        private int _column;

        private bool _isAtEndOfLine;

        private int _line;

        private int _visualColumn;

        #endregion

        #region Свойства

        public TextLocation Location
        {
            get { return new TextLocation(_line, _column); }
            set
            {
                _line = value.Line;
                _column = value.Column;
            }
        }

        public int Line
        {
            get { return _line; }
            set { _line = value; }
        }

        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }

        public int VisualColumn
        {
            get { return _visualColumn; }
            set { _visualColumn = value; }
        }

        public bool IsAtEndOfLine
        {
            get { return _isAtEndOfLine; }
            set { _isAtEndOfLine = value; }
        }

        #endregion

        #region Конструктор

        public TextViewPosition(int line, int column, int visualColumn)
        {
            _line = line;
            _column = column;
            _visualColumn = visualColumn;
            _isAtEndOfLine = false;
        }

        public TextViewPosition(int line, int column)
            : this(line, column, -1)
        {
        }

        public TextViewPosition(TextLocation location, int visualColumn)
        {
            _line = location.Line;
            _column = location.Column;
            _visualColumn = visualColumn;
            _isAtEndOfLine = false;
        }

        public TextViewPosition(TextLocation location)
            : this(location, -1)
        {
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[TextViewPosition Line={0} Column={1} VisualColumn={2} IsAtEndOfLine={3}]",
                _line, _column, _visualColumn, _isAtEndOfLine);
        }

        public override bool Equals(object obj)
        {
            if (obj is TextViewPosition)
            {
                return Equals((TextViewPosition)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = _isAtEndOfLine ? 115817 : 0;
            unchecked
            {
                hashCode += 1000000007 * Line.GetHashCode();
                hashCode += 1000000009 * Column.GetHashCode();
                hashCode += 1000000021 * VisualColumn.GetHashCode();
            }
            return hashCode;
        }

        public bool Equals(TextViewPosition other)
        {
            return Line == other.Line && Column == other.Column && VisualColumn == other.VisualColumn && IsAtEndOfLine == other.IsAtEndOfLine;
        }

        public int CompareTo(TextViewPosition other)
        {
            int r = Location.CompareTo(other.Location);
            if (r != 0)
            {
                return r;
            }
            r = _visualColumn.CompareTo(other._visualColumn);
            if (r != 0)
            {
                return r;
            }
            if (_isAtEndOfLine && !other._isAtEndOfLine)
            {
                return -1;
            }
            if (!_isAtEndOfLine && other._isAtEndOfLine)
            {
                return 1;
            }
            return 0;
        }

        #endregion

        public static bool operator ==(TextViewPosition left, TextViewPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextViewPosition left, TextViewPosition right)
        {
            return !(left.Equals(right));
        }
    }
}