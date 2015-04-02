namespace SharpLib.Texter.Document
{
    public interface IUndoableOperation
    {
        #region Методы

        void Undo();

        void Redo();

        #endregion
    }
}