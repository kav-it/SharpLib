using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Indentation
{
    public interface IIndentationStrategy
    {
        #region Методы

        void IndentLine(TextDocument document, DocumentLine line);

        void IndentLines(TextDocument document, int beginLine, int endLine);

        #endregion
    }
}