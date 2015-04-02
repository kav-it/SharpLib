using System;
using System.Diagnostics;

using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Document
{
    internal sealed class UndoOperationGroup : IUndoableOperationWithContext
    {
        #region Поля

        private readonly IUndoableOperation[] undolist;

        #endregion

        #region Конструктор

        public UndoOperationGroup(Deque<IUndoableOperation> stack, int numops)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack");
            }

            Debug.Assert(numops > 0, "UndoOperationGroup : numops should be > 0");
            Debug.Assert(numops <= stack.Count);

            undolist = new IUndoableOperation[numops];
            for (int i = 0; i < numops; ++i)
            {
                undolist[i] = stack.PopBack();
            }
        }

        #endregion

        #region Методы

        public void Undo()
        {
            for (int i = 0; i < undolist.Length; ++i)
            {
                undolist[i].Undo();
            }
        }

        public void Undo(UndoStack stack)
        {
            for (int i = 0; i < undolist.Length; ++i)
            {
                stack.RunUndo(undolist[i]);
            }
        }

        public void Redo()
        {
            for (int i = undolist.Length - 1; i >= 0; --i)
            {
                undolist[i].Redo();
            }
        }

        public void Redo(UndoStack stack)
        {
            for (int i = undolist.Length - 1; i >= 0; --i)
            {
                stack.RunRedo(undolist[i]);
            }
        }

        #endregion
    }
}