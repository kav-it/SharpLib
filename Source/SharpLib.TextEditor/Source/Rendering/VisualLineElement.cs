using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Rendering
{
    public abstract class VisualLineElement
    {
        #region Свойства

        public int VisualLength { get; private set; }

        public int DocumentLength { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "This property holds the start visual column, use GetVisualColumn to get inner visual columns.")]
        public int VisualColumn { get; internal set; }

        public int RelativeTextOffset { get; internal set; }

        public VisualLineElementTextRunProperties TextRunProperties { get; private set; }

        public Brush BackgroundBrush { get; set; }

        public virtual bool CanSplit
        {
            get { return false; }
        }

        public virtual bool HandlesLineBorders
        {
            get { return false; }
        }

        #endregion

        #region Конструктор

        protected VisualLineElement(int visualLength, int documentLength)
        {
            if (visualLength < 1)
            {
                throw new ArgumentOutOfRangeException("visualLength", visualLength, "Value must be at least 1");
            }
            if (documentLength < 0)
            {
                throw new ArgumentOutOfRangeException("documentLength", documentLength, "Value must be at least 0");
            }
            VisualLength = visualLength;
            DocumentLength = documentLength;
        }

        #endregion

        #region Методы

        internal void SetTextRunProperties(VisualLineElementTextRunProperties p)
        {
            TextRunProperties = p;
        }

        public abstract TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context);

        public virtual TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int visualColumnLimit, ITextRunConstructionContext context)
        {
            return null;
        }

        public virtual void Split(int splitVisualColumn, IList<VisualLineElement> elements, int elementIndex)
        {
            throw new NotSupportedException();
        }

        protected void SplitHelper(VisualLineElement firstPart, VisualLineElement secondPart, int splitVisualColumn, int splitRelativeTextOffset)
        {
            if (firstPart == null)
            {
                throw new ArgumentNullException("firstPart");
            }
            if (secondPart == null)
            {
                throw new ArgumentNullException("secondPart");
            }
            int relativeSplitVisualColumn = splitVisualColumn - VisualColumn;
            int relativeSplitRelativeTextOffset = splitRelativeTextOffset - RelativeTextOffset;

            if (relativeSplitVisualColumn <= 0 || relativeSplitVisualColumn >= VisualLength)
            {
                throw new ArgumentOutOfRangeException("splitVisualColumn", splitVisualColumn, "Value must be between " + (VisualColumn + 1) + " and " + (VisualColumn + VisualLength - 1));
            }
            if (relativeSplitRelativeTextOffset < 0 || relativeSplitRelativeTextOffset > DocumentLength)
            {
                throw new ArgumentOutOfRangeException("splitRelativeTextOffset", splitRelativeTextOffset,
                    "Value must be between " + (RelativeTextOffset) + " and " + (RelativeTextOffset + DocumentLength));
            }
            int oldVisualLength = VisualLength;
            int oldDocumentLength = DocumentLength;
            int oldVisualColumn = VisualColumn;
            int oldRelativeTextOffset = RelativeTextOffset;
            firstPart.VisualColumn = oldVisualColumn;
            secondPart.VisualColumn = oldVisualColumn + relativeSplitVisualColumn;
            firstPart.RelativeTextOffset = oldRelativeTextOffset;
            secondPart.RelativeTextOffset = oldRelativeTextOffset + relativeSplitRelativeTextOffset;
            firstPart.VisualLength = relativeSplitVisualColumn;
            secondPart.VisualLength = oldVisualLength - relativeSplitVisualColumn;
            firstPart.DocumentLength = relativeSplitRelativeTextOffset;
            secondPart.DocumentLength = oldDocumentLength - relativeSplitRelativeTextOffset;
            if (firstPart.TextRunProperties == null)
            {
                firstPart.TextRunProperties = TextRunProperties.Clone();
            }
            if (secondPart.TextRunProperties == null)
            {
                secondPart.TextRunProperties = TextRunProperties.Clone();
            }
        }

        public virtual int GetVisualColumn(int relativeTextOffset)
        {
            if (relativeTextOffset >= RelativeTextOffset + DocumentLength)
            {
                return VisualColumn + VisualLength;
            }
            return VisualColumn;
        }

        public virtual int GetRelativeOffset(int visualColumn)
        {
            if (visualColumn >= VisualColumn + VisualLength)
            {
                return RelativeTextOffset + DocumentLength;
            }
            return RelativeTextOffset;
        }

        public virtual int GetNextCaretPosition(int visualColumn, LogicalDirection direction, CaretPositioningMode mode)
        {
            int stop1 = VisualColumn;
            int stop2 = VisualColumn + VisualLength;
            if (direction == LogicalDirection.Backward)
            {
                if (visualColumn > stop2 && mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol)
                {
                    return stop2;
                }
                if (visualColumn > stop1)
                {
                    return stop1;
                }
            }
            else
            {
                if (visualColumn < stop1)
                {
                    return stop1;
                }
                if (visualColumn < stop2 && mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol)
                {
                    return stop2;
                }
            }
            return -1;
        }

        public virtual bool IsWhitespace(int visualColumn)
        {
            return false;
        }

        protected internal virtual void OnQueryCursor(QueryCursorEventArgs e)
        {
        }

        protected internal virtual void OnMouseDown(MouseButtonEventArgs e)
        {
        }

        protected internal virtual void OnMouseUp(MouseButtonEventArgs e)
        {
        }

        #endregion
    }
}