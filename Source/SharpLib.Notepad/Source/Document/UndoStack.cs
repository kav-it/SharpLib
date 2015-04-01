using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Document
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public sealed class UndoStack : INotifyPropertyChanged
    {
        #region Константы

        internal const int StateListen = 0;

        internal const int StatePlayback = 1;

        internal const int StatePlaybackModifyDocument = 2;

        #endregion

        #region Поля

        private readonly Deque<IUndoableOperation> redostack = new Deque<IUndoableOperation>();

        private readonly Deque<IUndoableOperation> undostack = new Deque<IUndoableOperation>();

        private int actionCountInUndoGroup;

        private List<TextDocument> affectedDocuments;

        private bool allowContinue;

        private int elementsOnUndoUntilOriginalFile;

        private bool isOriginalFile = true;

        private object lastGroupDescriptor;

        private int optionalActionCount;

        private int sizeLimit = int.MaxValue;

        internal int state = StateListen;

        private int undoGroupDepth;

        #endregion

        #region Свойства

        public bool IsOriginalFile
        {
            get { return isOriginalFile; }
        }

        public bool AcceptChanges
        {
            get { return state == StateListen; }
        }

        public bool CanUndo
        {
            get { return undostack.Count > 0; }
        }

        public bool CanRedo
        {
            get { return redostack.Count > 0; }
        }

        public int SizeLimit
        {
            get { return sizeLimit; }
            set
            {
                if (value < 0)
                {
                    ThrowUtil.CheckNotNegative(value, "value");
                }
                if (sizeLimit != value)
                {
                    sizeLimit = value;
                    NotifyPropertyChanged("SizeLimit");
                    if (undoGroupDepth == 0)
                    {
                        EnforceSizeLimit();
                    }
                }
            }
        }

        public object LastGroupDescriptor
        {
            get { return lastGroupDescriptor; }
        }

        #endregion

        #region События

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Методы

        private void RecalcIsOriginalFile()
        {
            bool newIsOriginalFile = (elementsOnUndoUntilOriginalFile == 0);
            if (newIsOriginalFile != isOriginalFile)
            {
                isOriginalFile = newIsOriginalFile;
                NotifyPropertyChanged("IsOriginalFile");
            }
        }

        public void MarkAsOriginalFile()
        {
            elementsOnUndoUntilOriginalFile = 0;
            RecalcIsOriginalFile();
        }

        public void DiscardOriginalFileMarker()
        {
            elementsOnUndoUntilOriginalFile = int.MinValue;
            RecalcIsOriginalFile();
        }

        private void FileModified(int newElementsOnUndoStack)
        {
            if (elementsOnUndoUntilOriginalFile == int.MinValue)
            {
                return;
            }

            elementsOnUndoUntilOriginalFile += newElementsOnUndoStack;
            if (elementsOnUndoUntilOriginalFile > undostack.Count)
            {
                elementsOnUndoUntilOriginalFile = int.MinValue;
            }
        }

        private void EnforceSizeLimit()
        {
            Debug.Assert(undoGroupDepth == 0);
            while (undostack.Count > sizeLimit)
            {
                undostack.PopFront();
            }
            while (redostack.Count > sizeLimit)
            {
                redostack.PopFront();
            }
        }

        public void StartUndoGroup()
        {
            StartUndoGroup(null);
        }

        public void StartUndoGroup(object groupDescriptor)
        {
            if (undoGroupDepth == 0)
            {
                actionCountInUndoGroup = 0;
                optionalActionCount = 0;
                lastGroupDescriptor = groupDescriptor;
            }
            undoGroupDepth++;
        }

        public void StartContinuedUndoGroup(object groupDescriptor = null)
        {
            if (undoGroupDepth == 0)
            {
                actionCountInUndoGroup = (allowContinue && undostack.Count > 0) ? 1 : 0;
                optionalActionCount = 0;
                lastGroupDescriptor = groupDescriptor;
            }
            undoGroupDepth++;
        }

        public void EndUndoGroup()
        {
            if (undoGroupDepth == 0)
            {
                throw new InvalidOperationException("There are no open undo groups");
            }
            undoGroupDepth--;

            if (undoGroupDepth == 0)
            {
                Debug.Assert(state == StateListen || actionCountInUndoGroup == 0);
                allowContinue = true;
                if (actionCountInUndoGroup == optionalActionCount)
                {
                    for (int i = 0; i < optionalActionCount; i++)
                    {
                        undostack.PopBack();
                    }
                    allowContinue = false;
                }
                else if (actionCountInUndoGroup > 1)
                {
                    undostack.PushBack(new UndoOperationGroup(undostack, actionCountInUndoGroup));
                    FileModified(-actionCountInUndoGroup + 1 + optionalActionCount);
                }

                EnforceSizeLimit();
                RecalcIsOriginalFile();
            }
        }

        private void ThrowIfUndoGroupOpen()
        {
            if (undoGroupDepth != 0)
            {
                undoGroupDepth = 0;
                throw new InvalidOperationException("No undo group should be open at this point");
            }
            if (state != StateListen)
            {
                throw new InvalidOperationException("This method cannot be called while an undo operation is being performed");
            }
        }

        internal void RegisterAffectedDocument(TextDocument document)
        {
            if (affectedDocuments == null)
            {
                affectedDocuments = new List<TextDocument>();
            }
            if (!affectedDocuments.Contains(document))
            {
                affectedDocuments.Add(document);
                document.BeginUpdate();
            }
        }

        private void CallEndUpdateOnAffectedDocuments()
        {
            if (affectedDocuments != null)
            {
                foreach (TextDocument doc in affectedDocuments)
                {
                    doc.EndUpdate();
                }
                affectedDocuments = null;
            }
        }

        public void Undo()
        {
            ThrowIfUndoGroupOpen();
            if (undostack.Count > 0)
            {
                lastGroupDescriptor = null;
                allowContinue = false;

                var uedit = undostack.PopBack();
                redostack.PushBack(uedit);
                state = StatePlayback;
                try
                {
                    RunUndo(uedit);
                }
                finally
                {
                    state = StateListen;
                    FileModified(-1);
                    CallEndUpdateOnAffectedDocuments();
                }
                RecalcIsOriginalFile();
                if (undostack.Count == 0)
                {
                    NotifyPropertyChanged("CanUndo");
                }
                if (redostack.Count == 1)
                {
                    NotifyPropertyChanged("CanRedo");
                }
            }
        }

        internal void RunUndo(IUndoableOperation op)
        {
            var opWithCtx = op as IUndoableOperationWithContext;
            if (opWithCtx != null)
            {
                opWithCtx.Undo(this);
            }
            else
            {
                op.Undo();
            }
        }

        public void Redo()
        {
            ThrowIfUndoGroupOpen();
            if (redostack.Count > 0)
            {
                lastGroupDescriptor = null;
                allowContinue = false;
                var uedit = redostack.PopBack();
                undostack.PushBack(uedit);
                state = StatePlayback;
                try
                {
                    RunRedo(uedit);
                }
                finally
                {
                    state = StateListen;
                    FileModified(1);
                    CallEndUpdateOnAffectedDocuments();
                }
                RecalcIsOriginalFile();
                if (redostack.Count == 0)
                {
                    NotifyPropertyChanged("CanRedo");
                }
                if (undostack.Count == 1)
                {
                    NotifyPropertyChanged("CanUndo");
                }
            }
        }

        internal void RunRedo(IUndoableOperation op)
        {
            var opWithCtx = op as IUndoableOperationWithContext;
            if (opWithCtx != null)
            {
                opWithCtx.Redo(this);
            }
            else
            {
                op.Redo();
            }
        }

        public void Push(IUndoableOperation operation)
        {
            Push(operation, false);
        }

        public void PushOptional(IUndoableOperation operation)
        {
            if (undoGroupDepth == 0)
            {
                throw new InvalidOperationException("Cannot use PushOptional outside of undo group");
            }
            Push(operation, true);
        }

        private void Push(IUndoableOperation operation, bool isOptional)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            if (state == StateListen && sizeLimit > 0)
            {
                bool wasEmpty = undostack.Count == 0;

                bool needsUndoGroup = undoGroupDepth == 0;
                if (needsUndoGroup)
                {
                    StartUndoGroup();
                }
                undostack.PushBack(operation);
                actionCountInUndoGroup++;
                if (isOptional)
                {
                    optionalActionCount++;
                }
                else
                {
                    FileModified(1);
                }
                if (needsUndoGroup)
                {
                    EndUndoGroup();
                }
                if (wasEmpty)
                {
                    NotifyPropertyChanged("CanUndo");
                }
                ClearRedoStack();
            }
        }

        public void ClearRedoStack()
        {
            if (redostack.Count != 0)
            {
                redostack.Clear();
                NotifyPropertyChanged("CanRedo");

                if (elementsOnUndoUntilOriginalFile < 0)
                {
                    elementsOnUndoUntilOriginalFile = int.MinValue;
                }
            }
        }

        public void ClearAll()
        {
            ThrowIfUndoGroupOpen();
            actionCountInUndoGroup = 0;
            optionalActionCount = 0;
            if (undostack.Count != 0)
            {
                lastGroupDescriptor = null;
                allowContinue = false;
                undostack.Clear();
                NotifyPropertyChanged("CanUndo");
            }
            ClearRedoStack();
        }

        internal void Push(TextDocument document, DocumentChangeEventArgs e)
        {
            if (state == StatePlayback)
            {
                throw new InvalidOperationException("Document changes during undo/redo operations are not allowed.");
            }
            if (state == StatePlaybackModifyDocument)
            {
                state = StatePlayback;
            }
            else
            {
                Push(new DocumentChangeOperation(document, e));
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}