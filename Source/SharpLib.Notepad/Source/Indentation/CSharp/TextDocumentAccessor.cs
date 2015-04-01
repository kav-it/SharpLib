using System;

using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Indentation.CSharp
{

    #region TextDocumentAccessor

    public sealed class TextDocumentAccessor : IDocumentAccessor
    {
        #region Поля

        private readonly TextDocument doc;

        private readonly int maxLine;

        private readonly int minLine;

        private DocumentLine line;

        private bool lineDirty;

        private int num;

        private string text;

        #endregion

        #region Свойства

        public bool IsReadOnly
        {
            get { return num < minLine; }
        }

        public int LineNumber
        {
            get { return num; }
        }

        public string Text
        {
            get { return text; }
            set
            {
                if (num < minLine)
                {
                    return;
                }
                text = value;
                lineDirty = true;
            }
        }

        #endregion

        #region Конструктор

        public TextDocumentAccessor(TextDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            doc = document;
            minLine = 1;
            maxLine = doc.LineCount;
        }

        public TextDocumentAccessor(TextDocument document, int minLine, int maxLine)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            doc = document;
            this.minLine = minLine;
            this.maxLine = maxLine;
        }

        #endregion

        #region Методы

        public bool MoveNext()
        {
            if (lineDirty)
            {
                doc.Replace(line, text);
                lineDirty = false;
            }
            ++num;
            if (num > maxLine)
            {
                return false;
            }
            line = doc.GetLineByNumber(num);
            text = doc.GetText(line);
            return true;
        }

        #endregion
    }

    #endregion
}