using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Indentation
{
    public interface IIndentationStrategy
    {
        #region Методы

        void IndentLine(TextDocument document, DocumentLine line);

        void IndentLines(TextDocument document, int beginLine, int endLine);

        #endregion
    }
}