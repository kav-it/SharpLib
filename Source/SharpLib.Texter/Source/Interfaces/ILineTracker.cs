namespace SharpLib.Texter.Document
{
    public interface ILineTracker
    {
        #region Методы

        void BeforeRemoveLine(DocumentLine line);

        void SetLineLength(DocumentLine line, int newTotalLength);

        void LineInserted(DocumentLine insertionPos, DocumentLine newLine);

        void RebuildDocument();

        void ChangeComplete(DocumentChangeEventArgs e);

        #endregion
    }
}