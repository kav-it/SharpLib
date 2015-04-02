namespace SharpLib.Notepad.Document
{
    public interface IUndoableOperation
    {
        #region Методы

        void Undo();

        void Redo();

        #endregion
    }
}