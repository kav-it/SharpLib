using System;
using System.Diagnostics;
using System.Globalization;

#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#endif

namespace ICSharpCode.AvalonEdit.Document
{
    internal struct SimpleSegment : IEquatable<SimpleSegment>, ISegment
    {
        #region Поля

        public static readonly SimpleSegment Invalid = new SimpleSegment(-1, -1);

        public readonly int Length;

        public readonly int Offset;

        #endregion

        #region Свойства

        int ISegment.Offset
        {
            get { return Offset; }
        }

        int ISegment.Length
        {
            get { return Length; }
        }

        public int EndOffset
        {
            get { return Offset + Length; }
        }

        #endregion

        #region Конструктор

        public SimpleSegment(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }

        public SimpleSegment(ISegment segment)
        {
            Debug.Assert(segment != null);
            Offset = segment.Offset;
            Length = segment.Length;
        }

        #endregion

        #region Методы

        public static SimpleSegment GetOverlap(ISegment segment1, ISegment segment2)
        {
            int start = Math.Max(segment1.Offset, segment2.Offset);
            int end = Math.Min(segment1.EndOffset, segment2.EndOffset);
            if (end < start)
            {
                return SimpleSegment.Invalid;
            }
            return new SimpleSegment(start, end - start);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Offset + 10301 * Length;
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is SimpleSegment) && Equals((SimpleSegment)obj);
        }

        public bool Equals(SimpleSegment other)
        {
            return Offset == other.Offset && Length == other.Length;
        }

        public override string ToString()
        {
            return "[Offset=" + Offset.ToString(CultureInfo.InvariantCulture) + ", Length=" + Length.ToString(CultureInfo.InvariantCulture) + "]";
        }

        #endregion

        public static bool operator ==(SimpleSegment left, SimpleSegment right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SimpleSegment left, SimpleSegment right)
        {
            return !left.Equals(right);
        }
    }
}