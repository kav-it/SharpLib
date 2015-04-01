namespace SharpLib.Notepad.Document
{
    internal interface IUndoableOperationWithContext : IUndoableOperation
    {
        #region Методы

        void Undo(UndoStack stack);

        void Redo(UndoStack stack);

        #endregion
    }
}