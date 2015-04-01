using System;

using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Indentation
{
    public class DefaultIndentationStrategy : IIndentationStrategy
    {
        #region Методы

        public virtual void IndentLine(TextDocument document, DocumentLine line)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }
            var previousLine = line.PreviousLine;
            if (previousLine != null)
            {
                var indentationSegment = TextUtilities.GetWhitespaceAfter(document, previousLine.Offset);
                string indentation = document.GetText(indentationSegment);

                indentationSegment = TextUtilities.GetWhitespaceAfter(document, line.Offset);
                document.Replace(indentationSegment, indentation);
            }
        }

        public virtual void IndentLines(TextDocument document, int beginLine, int endLine)
        {
        }

        #endregion
    }
}