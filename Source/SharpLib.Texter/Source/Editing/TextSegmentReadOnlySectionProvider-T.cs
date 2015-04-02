using System;
using System.Collections.Generic;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Editing
{
    public class TextSegmentReadOnlySectionProvider<T> : IReadOnlySectionProvider where T : TextSegment
    {
        #region Поля

        private readonly TextSegmentCollection<T> segments;

        #endregion

        #region Свойства

        public TextSegmentCollection<T> Segments
        {
            get { return segments; }
        }

        #endregion

        #region Конструктор

        public TextSegmentReadOnlySectionProvider(TextDocument textDocument)
        {
            segments = new TextSegmentCollection<T>(textDocument);
        }

        public TextSegmentReadOnlySectionProvider(TextSegmentCollection<T> segments)
        {
            if (segments == null)
            {
                throw new ArgumentNullException("segments");
            }
            this.segments = segments;
        }

        #endregion

        #region Методы

        public virtual bool CanInsert(int offset)
        {
            foreach (TextSegment segment in segments.FindSegmentsContaining(offset))
            {
                if (segment.StartOffset < offset && offset < segment.EndOffset)
                {
                    return false;
                }
            }
            return true;
        }

        public virtual IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException("segment");
            }

            if (segment.Length == 0 && CanInsert(segment.Offset))
            {
                yield return segment;
                yield break;
            }

            int readonlyUntil = segment.Offset;
            foreach (TextSegment ts in segments.FindOverlappingSegments(segment))
            {
                int start = ts.StartOffset;
                int end = start + ts.Length;
                if (start > readonlyUntil)
                {
                    yield return new SimpleSegment(readonlyUntil, start - readonlyUntil);
                }
                if (end > readonlyUntil)
                {
                    readonlyUntil = end;
                }
            }
            int endOffset = segment.EndOffset;
            if (readonlyUntil < endOffset)
            {
                yield return new SimpleSegment(readonlyUntil, endOffset - readonlyUntil);
            }
        }

        #endregion
    }
}