using System;
using System.Linq;

using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Rendering
{
    public abstract class DocumentColorizingTransformer : ColorizingTransformer
    {
        #region Поля

        private DocumentLine currentDocumentLine;

        private int currentDocumentLineEndOffset;

        private int currentDocumentLineStartOffset;

        private int firstLineStart;

        #endregion

        #region Свойства

        protected ITextRunConstructionContext CurrentContext { get; private set; }

        #endregion

        #region Методы

        protected override void Colorize(ITextRunConstructionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            CurrentContext = context;

            currentDocumentLine = context.VisualLine.FirstDocumentLine;
            firstLineStart = currentDocumentLineStartOffset = currentDocumentLine.Offset;
            currentDocumentLineEndOffset = currentDocumentLineStartOffset + currentDocumentLine.Length;
            int currentDocumentLineTotalEndOffset = currentDocumentLineStartOffset + currentDocumentLine.TotalLength;

            if (context.VisualLine.FirstDocumentLine == context.VisualLine.LastDocumentLine)
            {
                ColorizeLine(currentDocumentLine);
            }
            else
            {
                ColorizeLine(currentDocumentLine);

                foreach (VisualLineElement e in context.VisualLine.Elements.ToArray())
                {
                    int elementOffset = firstLineStart + e.RelativeTextOffset;
                    if (elementOffset >= currentDocumentLineTotalEndOffset)
                    {
                        currentDocumentLine = context.Document.GetLineByOffset(elementOffset);
                        currentDocumentLineStartOffset = currentDocumentLine.Offset;
                        currentDocumentLineEndOffset = currentDocumentLineStartOffset + currentDocumentLine.Length;
                        currentDocumentLineTotalEndOffset = currentDocumentLineStartOffset + currentDocumentLine.TotalLength;
                        ColorizeLine(currentDocumentLine);
                    }
                }
            }
            currentDocumentLine = null;
            CurrentContext = null;
        }

        protected abstract void ColorizeLine(DocumentLine line);

        protected void ChangeLinePart(int startOffset, int endOffset, Action<VisualLineElement> action)
        {
            if (startOffset < currentDocumentLineStartOffset || startOffset > currentDocumentLineEndOffset)
            {
                throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between " + currentDocumentLineStartOffset + " and " + currentDocumentLineEndOffset);
            }
            if (endOffset < startOffset || endOffset > currentDocumentLineEndOffset)
            {
                throw new ArgumentOutOfRangeException("endOffset", endOffset, "Value must be between " + startOffset + " and " + currentDocumentLineEndOffset);
            }
            var vl = CurrentContext.VisualLine;
            int visualStart = vl.GetVisualColumn(startOffset - firstLineStart);
            int visualEnd = vl.GetVisualColumn(endOffset - firstLineStart);
            if (visualStart < visualEnd)
            {
                ChangeVisualElements(visualStart, visualEnd, action);
            }
        }

        #endregion
    }
}