using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpLib.Texter.Document
{
    internal sealed class LineManager
    {
        #region Поля

        private readonly TextDocument document;

        private readonly DocumentLineTree documentLineTree;

        private ILineTracker[] lineTrackers;

        #endregion

        #region Конструктор

        public LineManager(DocumentLineTree documentLineTree, TextDocument document)
        {
            this.document = document;
            this.documentLineTree = documentLineTree;
            UpdateListOfLineTrackers();

            Rebuild();
        }

        #endregion

        #region Методы

        internal void UpdateListOfLineTrackers()
        {
            lineTrackers = document.LineTrackers.ToArray();
        }

        public void Rebuild()
        {
            var ls = documentLineTree.GetByNumber(1);

            for (var line = ls.NextLine; line != null; line = line.NextLine)
            {
                line.isDeleted = true;
                line.parent = line.left = line.right = null;
            }

            ls.ResetLine();
            var ds = NewLineFinder.NextNewLine(document, 0);
            var lines = new List<DocumentLine>();
            int lastDelimiterEnd = 0;
            while (ds != SimpleSegment.Invalid)
            {
                ls.TotalLength = ds.Offset + ds.Length - lastDelimiterEnd;
                ls.DelimiterLength = ds.Length;
                lastDelimiterEnd = ds.Offset + ds.Length;
                lines.Add(ls);

                ls = new DocumentLine(document);
                ds = NewLineFinder.NextNewLine(document, lastDelimiterEnd);
            }
            ls.TotalLength = document.TextLength - lastDelimiterEnd;
            lines.Add(ls);
            documentLineTree.RebuildTree(lines);
            foreach (ILineTracker lineTracker in lineTrackers)
            {
                lineTracker.RebuildDocument();
            }
        }

        public void Remove(int offset, int length)
        {
            Debug.Assert(length >= 0);
            if (length == 0)
            {
                return;
            }
            var startLine = documentLineTree.GetByOffset(offset);
            int startLineOffset = startLine.Offset;

            Debug.Assert(offset < startLineOffset + startLine.TotalLength);
            if (offset > startLineOffset + startLine.Length)
            {
                Debug.Assert(startLine.DelimiterLength == 2);

                SetLineLength(startLine, startLine.TotalLength - 1);

                Remove(offset, length - 1);
                return;
            }

            if (offset + length < startLineOffset + startLine.TotalLength)
            {
                SetLineLength(startLine, startLine.TotalLength - length);
                return;
            }

            int charactersRemovedInStartLine = startLineOffset + startLine.TotalLength - offset;
            Debug.Assert(charactersRemovedInStartLine > 0);

            var endLine = documentLineTree.GetByOffset(offset + length);
            if (endLine == startLine)
            {
                SetLineLength(startLine, startLine.TotalLength - length);
                return;
            }
            int endLineOffset = endLine.Offset;
            int charactersLeftInEndLine = endLineOffset + endLine.TotalLength - (offset + length);

            var tmp = startLine.NextLine;
            DocumentLine lineToRemove;
            do
            {
                lineToRemove = tmp;
                tmp = tmp.NextLine;
                RemoveLine(lineToRemove);
            } while (lineToRemove != endLine);

            SetLineLength(startLine, startLine.TotalLength - charactersRemovedInStartLine + charactersLeftInEndLine);
        }

        private void RemoveLine(DocumentLine lineToRemove)
        {
            foreach (ILineTracker lt in lineTrackers)
            {
                lt.BeforeRemoveLine(lineToRemove);
            }
            documentLineTree.RemoveLine(lineToRemove);
        }

        public void Insert(int offset, ITextSource text)
        {
            var line = documentLineTree.GetByOffset(offset);
            int lineOffset = line.Offset;

            Debug.Assert(offset <= lineOffset + line.TotalLength);
            if (offset > lineOffset + line.Length)
            {
                Debug.Assert(line.DelimiterLength == 2);

                SetLineLength(line, line.TotalLength - 1);

                line = InsertLineAfter(line, 1);
                line = SetLineLength(line, 1);
            }

            var ds = NewLineFinder.NextNewLine(text, 0);
            if (ds == SimpleSegment.Invalid)
            {
                SetLineLength(line, line.TotalLength + text.TextLength);
                return;
            }

            int lastDelimiterEnd = 0;
            while (ds != SimpleSegment.Invalid)
            {
                int lineBreakOffset = offset + ds.Offset + ds.Length;
                lineOffset = line.Offset;
                int lengthAfterInsertionPos = lineOffset + line.TotalLength - (offset + lastDelimiterEnd);
                line = SetLineLength(line, lineBreakOffset - lineOffset);
                var newLine = InsertLineAfter(line, lengthAfterInsertionPos);
                newLine = SetLineLength(newLine, lengthAfterInsertionPos);

                line = newLine;
                lastDelimiterEnd = ds.Offset + ds.Length;

                ds = NewLineFinder.NextNewLine(text, lastDelimiterEnd);
            }

            if (lastDelimiterEnd != text.TextLength)
            {
                SetLineLength(line, line.TotalLength + text.TextLength - lastDelimiterEnd);
            }
        }

        private DocumentLine InsertLineAfter(DocumentLine line, int length)
        {
            var newLine = documentLineTree.InsertLineAfter(line, length);
            foreach (ILineTracker lt in lineTrackers)
            {
                lt.LineInserted(line, newLine);
            }
            return newLine;
        }

        private DocumentLine SetLineLength(DocumentLine line, int newTotalLength)
        {
            int delta = newTotalLength - line.TotalLength;
            if (delta != 0)
            {
                foreach (ILineTracker lt in lineTrackers)
                {
                    lt.SetLineLength(line, newTotalLength);
                }
                line.TotalLength = newTotalLength;
                DocumentLineTree.UpdateAfterChildrenChange(line);
            }

            if (newTotalLength == 0)
            {
                line.DelimiterLength = 0;
            }
            else
            {
                int lineOffset = line.Offset;
                char lastChar = document.GetCharAt(lineOffset + newTotalLength - 1);
                if (lastChar == '\r')
                {
                    line.DelimiterLength = 1;
                }
                else if (lastChar == '\n')
                {
                    if (newTotalLength >= 2 && document.GetCharAt(lineOffset + newTotalLength - 2) == '\r')
                    {
                        line.DelimiterLength = 2;
                    }
                    else if (newTotalLength == 1 && lineOffset > 0 && document.GetCharAt(lineOffset - 1) == '\r')
                    {
                        var previousLine = line.PreviousLine;
                        RemoveLine(line);
                        return SetLineLength(previousLine, previousLine.TotalLength + 1);
                    }
                    else
                    {
                        line.DelimiterLength = 1;
                    }
                }
                else
                {
                    line.DelimiterLength = 0;
                }
            }
            return line;
        }

        public void ChangeComplete(DocumentChangeEventArgs e)
        {
            foreach (ILineTracker lt in lineTrackers)
            {
                lt.ChangeComplete(e);
            }
        }

        #endregion
    }
}