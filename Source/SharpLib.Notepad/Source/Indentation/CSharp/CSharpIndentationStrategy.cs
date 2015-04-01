using System;

using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Indentation.CSharp
{
    public class CSharpIndentationStrategy : DefaultIndentationStrategy
    {
        #region Поля

        private string indentationString = "\t";

        #endregion

        #region Свойства

        public string IndentationString
        {
            get { return indentationString; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Indentation string must not be null or empty");
                }
                indentationString = value;
            }
        }

        #endregion

        #region Конструктор

        public CSharpIndentationStrategy()
        {
        }

        public CSharpIndentationStrategy(TextEditorOptions options)
        {
            IndentationString = options.IndentationString;
        }

        #endregion

        #region Методы

        public void Indent(IDocumentAccessor document, bool keepEmptyLines)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            var settings = new IndentationSettings();
            settings.IndentString = IndentationString;
            settings.LeaveEmptyLines = keepEmptyLines;

            var r = new IndentationReformatter();
            r.Reformat(document, settings);
        }

        public override void IndentLine(TextDocument document, DocumentLine line)
        {
            int lineNr = line.LineNumber;
            var acc = new TextDocumentAccessor(document, lineNr, lineNr);
            Indent(acc, false);

            string t = acc.Text;
            if (t.Length == 0)
            {
                base.IndentLine(document, line);
            }
        }

        public override void IndentLines(TextDocument document, int beginLine, int endLine)
        {
            Indent(new TextDocumentAccessor(document, beginLine, endLine), true);
        }

        #endregion
    }
}