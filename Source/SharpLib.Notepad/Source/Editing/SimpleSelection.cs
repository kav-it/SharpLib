using System;
using System.Collections.Generic;

using SharpLib.Notepad.Document;
using SharpLib.Notepad.Utils;

#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#endif

namespace SharpLib.Notepad.Editing
{
    internal sealed class SimpleSelection : Selection
    {
        #region Поля

        private readonly TextViewPosition end;

        private readonly int endOffset;

        private readonly TextViewPosition start;

        private readonly int startOffset;

        #endregion

        #region Свойства

        public override IEnumerable<SelectionSegment> Segments
        {
            get { return ExtensionMethods.Sequence(new SelectionSegment(startOffset, start.VisualColumn, endOffset, end.VisualColumn)); }
        }

        public override ISegment SurroundingSegment
        {
            get { return new SelectionSegment(startOffset, endOffset); }
        }

        public override TextViewPosition StartPosition
        {
            get { return start; }
        }

        public override TextViewPosition EndPosition
        {
            get { return end; }
        }

        public override bool IsEmpty
        {
            get { return startOffset == endOffset && start.VisualColumn == end.VisualColumn; }
        }

        public override int Length
        {
            get { return Math.Abs(endOffset - startOffset); }
        }

        #endregion

        #region Конструктор

        internal SimpleSelection(TextArea textArea, TextViewPosition start, TextViewPosition end)
            : base(textArea)
        {
            this.start = start;
            this.end = end;
            startOffset = textArea.Document.GetOffset(start.Location);
            endOffset = textArea.Document.GetOffset(end.Location);
        }

        #endregion

        #region Методы

        public override void ReplaceSelectionWithText(string newText)
        {
            if (newText == null)
            {
                throw new ArgumentNullException("newText");
            }
            using (textArea.Document.RunUpdate())
            {
                var segmentsToDelete = textArea.GetDeletableSegments(SurroundingSegment);
                for (int i = segmentsToDelete.Length - 1; i >= 0; i--)
                {
                    if (i == segmentsToDelete.Length - 1)
                    {
                        if (segmentsToDelete[i].Offset == SurroundingSegment.Offset && segmentsToDelete[i].Length == SurroundingSegment.Length)
                        {
                            newText = AddSpacesIfRequired(newText, start, end);
                        }
                        if (string.IsNullOrEmpty(newText))
                        {
                            if (start.CompareTo(end) <= 0)
                            {
                                textArea.Caret.Position = start;
                            }
                            else
                            {
                                textArea.Caret.Position = end;
                            }
                        }
                        else
                        {
                            textArea.Caret.Offset = segmentsToDelete[i].EndOffset;
                        }
                        textArea.Document.Replace(segmentsToDelete[i], newText);
                    }
                    else
                    {
                        textArea.Document.Remove(segmentsToDelete[i]);
                    }
                }
                if (segmentsToDelete.Length != 0)
                {
                    textArea.ClearSelection();
                }
            }
        }

        public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            int newStartOffset, newEndOffset;
            if (startOffset <= endOffset)
            {
                newStartOffset = e.GetNewOffset(startOffset, AnchorMovementType.Default);
                newEndOffset = Math.Max(newStartOffset, e.GetNewOffset(endOffset, AnchorMovementType.BeforeInsertion));
            }
            else
            {
                newEndOffset = e.GetNewOffset(endOffset, AnchorMovementType.Default);
                newStartOffset = Math.Max(newEndOffset, e.GetNewOffset(startOffset, AnchorMovementType.BeforeInsertion));
            }
            return Selection.Create(
                textArea,
                new TextViewPosition(textArea.Document.GetLocation(newStartOffset), start.VisualColumn),
                new TextViewPosition(textArea.Document.GetLocation(newEndOffset), end.VisualColumn)
                );
        }

        public override Selection SetEndpoint(TextViewPosition endPosition)
        {
            return Create(textArea, start, endPosition);
        }

        public override Selection StartSelectionOrSetEndpoint(TextViewPosition startPosition, TextViewPosition endPosition)
        {
            var document = textArea.Document;
            if (document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            return Create(textArea, start, endPosition);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return startOffset * 27811 + endOffset + textArea.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as SimpleSelection;
            if (other == null)
            {
                return false;
            }
            return start.Equals(other.start) && end.Equals(other.end)
                   && startOffset == other.startOffset && endOffset == other.endOffset
                   && textArea == other.textArea;
        }

        public override string ToString()
        {
            return "[SimpleSelection Start=" + start + " End=" + end + "]";
        }

        #endregion
    }
}