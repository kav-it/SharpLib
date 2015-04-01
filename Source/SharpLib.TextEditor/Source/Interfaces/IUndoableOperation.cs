namespace ICSharpCode.AvalonEdit.Document
{
    public interface IUndoableOperation
    {
        #region Методы

        void Undo();

        void Redo();

        #endregion
    }
}