using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Rendering
{
    public class VisualLineText : VisualLineElement
    {
        #region Поля

        private readonly VisualLine parentVisualLine;

        #endregion

        #region Свойства

        public VisualLine ParentVisualLine
        {
            get { return parentVisualLine; }
        }

        public override bool CanSplit
        {
            get { return true; }
        }

        #endregion

        #region Конструктор

        public VisualLineText(VisualLine parentVisualLine, int length)
            : base(length, length)
        {
            if (parentVisualLine == null)
            {
                throw new ArgumentNullException("parentVisualLine");
            }
            this.parentVisualLine = parentVisualLine;
        }

        #endregion

        #region Методы

        protected virtual VisualLineText CreateInstance(int length)
        {
            return new VisualLineText(parentVisualLine, length);
        }

        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            int relativeOffset = startVisualColumn - VisualColumn;
            var text = context.GetText(context.VisualLine.FirstDocumentLine.Offset + RelativeTextOffset + relativeOffset, DocumentLength - relativeOffset);
            return new TextCharacters(text.Text, text.Offset, text.Count, TextRunProperties);
        }

        public override bool IsWhitespace(int visualColumn)
        {
            int offset = visualColumn - VisualColumn + parentVisualLine.FirstDocumentLine.Offset + RelativeTextOffset;
            return char.IsWhiteSpace(parentVisualLine.Document.GetCharAt(offset));
        }

        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int visualColumnLimit, ITextRunConstructionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            int relativeOffset = visualColumnLimit - VisualColumn;
            var text = context.GetText(context.VisualLine.FirstDocumentLine.Offset + RelativeTextOffset, relativeOffset);
            var range = new CharacterBufferRange(text.Text, text.Offset, text.Count);
            return new TextSpan<CultureSpecificCharacterBufferRange>(range.Length, new CultureSpecificCharacterBufferRange(TextRunProperties.CultureInfo, range));
        }

        public override void Split(int splitVisualColumn, IList<VisualLineElement> elements, int elementIndex)
        {
            if (splitVisualColumn <= VisualColumn || splitVisualColumn >= VisualColumn + VisualLength)
            {
                throw new ArgumentOutOfRangeException("splitVisualColumn", splitVisualColumn, "Value must be between " + (VisualColumn + 1) + " and " + (VisualColumn + VisualLength - 1));
            }
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }
            if (elements[elementIndex] != this)
            {
                throw new ArgumentException("Invalid elementIndex - couldn't find this element at the index");
            }
            int relativeSplitPos = splitVisualColumn - VisualColumn;
            var splitPart = CreateInstance(DocumentLength - relativeSplitPos);
            SplitHelper(this, splitPart, splitVisualColumn, relativeSplitPos + RelativeTextOffset);
            elements.Insert(elementIndex + 1, splitPart);
        }

        public override int GetRelativeOffset(int visualColumn)
        {
            return RelativeTextOffset + visualColumn - VisualColumn;
        }

        public override int GetVisualColumn(int relativeTextOffset)
        {
            return VisualColumn + relativeTextOffset - RelativeTextOffset;
        }

        public override int GetNextCaretPosition(int visualColumn, LogicalDirection direction, CaretPositioningMode mode)
        {
            int textOffset = parentVisualLine.StartOffset + RelativeTextOffset;
            int pos = TextUtilities.GetNextCaretPosition(parentVisualLine.Document, textOffset + visualColumn - VisualColumn, direction, mode);
            if (pos < textOffset || pos > textOffset + DocumentLength)
            {
                return -1;
            }
            return VisualColumn + pos - textOffset;
        }

        #endregion
    }
}