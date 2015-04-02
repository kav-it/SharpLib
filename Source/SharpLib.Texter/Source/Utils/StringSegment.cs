using System;

namespace SharpLib.Texter.Utils
{
    public struct StringSegment : IEquatable<StringSegment>
    {
        #region Поля

        private readonly int count;

        private readonly int offset;

        private readonly string text;

        #endregion

        #region Свойства

        public string Text
        {
            get { return text; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public int Count
        {
            get { return count; }
        }

        #endregion

        #region Конструктор

        public StringSegment(string text, int offset, int count)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (offset < 0 || offset > text.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (offset + count > text.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            this.text = text;
            this.offset = offset;
            this.count = count;
        }

        public StringSegment(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            this.text = text;
            offset = 0;
            count = text.Length;
        }

        #endregion

        #region Методы

        public override bool Equals(object obj)
        {
            if (obj is StringSegment)
            {
                return Equals((StringSegment)obj);
            }
            return false;
        }

        public bool Equals(StringSegment other)
        {
            return object.ReferenceEquals(text, other.text) && offset == other.offset && count == other.count;
        }

        public override int GetHashCode()
        {
            return text.GetHashCode() ^ offset ^ count;
        }

        #endregion

        public static bool operator ==(StringSegment left, StringSegment right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringSegment left, StringSegment right)
        {
            return !left.Equals(right);
        }
    }
}