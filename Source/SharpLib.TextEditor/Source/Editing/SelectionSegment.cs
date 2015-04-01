using System;
#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#else
using ICSharpCode.AvalonEdit.Document;

#endif

namespace ICSharpCode.AvalonEdit.Editing
{
    public class SelectionSegment : ISegment
    {
        #region Поля

        private readonly int endOffset;

        private readonly int endVC;

        private readonly int startOffset;

        private readonly int startVC;

        #endregion

        #region Свойства

        public int StartOffset
        {
            get { return startOffset; }
        }

        public int EndOffset
        {
            get { return endOffset; }
        }

        public int StartVisualColumn
        {
            get { return startVC; }
        }

        public int EndVisualColumn
        {
            get { return endVC; }
        }

        int ISegment.Offset
        {
            get { return startOffset; }
        }

        public int Length
        {
            get { return endOffset - startOffset; }
        }

        #endregion

        #region Конструктор

        public SelectionSegment(int startOffset, int endOffset)
        {
            this.startOffset = Math.Min(startOffset, endOffset);
            this.endOffset = Math.Max(startOffset, endOffset);
            startVC = endVC = -1;
        }

        public SelectionSegment(int startOffset, int startVC, int endOffset, int endVC)
        {
            if (startOffset < endOffset || (startOffset == endOffset && startVC <= endVC))
            {
                this.startOffset = startOffset;
                this.startVC = startVC;
                this.endOffset = endOffset;
                this.endVC = endVC;
            }
            else
            {
                this.startOffset = endOffset;
                this.startVC = endVC;
                this.endOffset = startOffset;
                this.endVC = startVC;
            }
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return string.Format("[SelectionSegment StartOffset={0}, EndOffset={1}, StartVC={2}, EndVC={3}]", startOffset, endOffset, startVC, endVC);
        }

        #endregion
    }
}