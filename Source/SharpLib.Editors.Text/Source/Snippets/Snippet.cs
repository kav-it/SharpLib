using System;

using SharpLib.Notepad.Document;
using SharpLib.Notepad.Editing;

namespace SharpLib.Notepad.Snippets
{
    [Serializable]
    public class Snippet : SnippetContainerElement
    {
        #region Методы

        public void Insert(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }

            var selection = textArea.Selection.SurroundingSegment;
            int insertionPosition = textArea.Caret.Offset;

            if (selection != null)
            {
                insertionPosition = selection.Offset + TextUtilities.GetWhitespaceAfter(textArea.Document, selection.Offset).Length;
            }

            var context = new InsertionContext(textArea, insertionPosition);

            using (context.Document.RunUpdate())
            {
                if (selection != null)
                {
                    textArea.Document.Remove(insertionPosition, selection.EndOffset - insertionPosition);
                }
                Insert(context);
                context.RaiseInsertionCompleted(EventArgs.Empty);
            }
        }

        #endregion
    }
}