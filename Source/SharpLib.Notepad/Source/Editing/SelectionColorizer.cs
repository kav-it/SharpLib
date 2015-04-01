using System;

using SharpLib.Notepad.Rendering;

#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#endif

namespace SharpLib.Notepad.Editing
{
    internal sealed class SelectionColorizer : ColorizingTransformer
    {
        #region Поля

        private readonly TextArea textArea;

        #endregion

        #region Конструктор

        public SelectionColorizer(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            this.textArea = textArea;
        }

        #endregion

        #region Методы

        protected override void Colorize(ITextRunConstructionContext context)
        {
            if (textArea.SelectionForeground == null)
            {
                return;
            }

            int lineStartOffset = context.VisualLine.FirstDocumentLine.Offset;
            int lineEndOffset = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;

            foreach (SelectionSegment segment in textArea.Selection.Segments)
            {
                int segmentStart = segment.StartOffset;
                int segmentEnd = segment.EndOffset;
                if (segmentEnd <= lineStartOffset)
                {
                    continue;
                }
                if (segmentStart >= lineEndOffset)
                {
                    continue;
                }
                int startColumn;
                if (segmentStart < lineStartOffset)
                {
                    startColumn = 0;
                }
                else
                {
                    startColumn = context.VisualLine.ValidateVisualColumn(segment.StartOffset, segment.StartVisualColumn, textArea.Selection.EnableVirtualSpace);
                }

                int endColumn;
                if (segmentEnd > lineEndOffset)
                {
                    endColumn = textArea.Selection.EnableVirtualSpace ? int.MaxValue : context.VisualLine.VisualLengthWithEndOfLineMarker;
                }
                else
                {
                    endColumn = context.VisualLine.ValidateVisualColumn(segment.EndOffset, segment.EndVisualColumn, textArea.Selection.EnableVirtualSpace);
                }

                ChangeVisualElements(
                    startColumn, endColumn,
                    element => { element.TextRunProperties.SetForegroundBrush(textArea.SelectionForeground); });
            }
        }

        #endregion
    }
}