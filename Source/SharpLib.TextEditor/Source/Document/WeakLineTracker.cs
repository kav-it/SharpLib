using System;

namespace ICSharpCode.AvalonEdit.Document
{
    public sealed class WeakLineTracker : ILineTracker
    {
        #region Поля

        private readonly WeakReference targetObject;

        private TextDocument textDocument;

        #endregion

        #region Конструктор

        private WeakLineTracker(TextDocument textDocument, ILineTracker targetTracker)
        {
            this.textDocument = textDocument;
            targetObject = new WeakReference(targetTracker);
        }

        #endregion

        #region Методы

        public static WeakLineTracker Register(TextDocument textDocument, ILineTracker targetTracker)
        {
            if (textDocument == null)
            {
                throw new ArgumentNullException("textDocument");
            }
            if (targetTracker == null)
            {
                throw new ArgumentNullException("targetTracker");
            }
            var wlt = new WeakLineTracker(textDocument, targetTracker);
            textDocument.LineTrackers.Add(wlt);
            return wlt;
        }

        public void Deregister()
        {
            if (textDocument != null)
            {
                textDocument.LineTrackers.Remove(this);
                textDocument = null;
            }
        }

        void ILineTracker.BeforeRemoveLine(DocumentLine line)
        {
            var targetTracker = targetObject.Target as ILineTracker;
            if (targetTracker != null)
            {
                targetTracker.BeforeRemoveLine(line);
            }
            else
            {
                Deregister();
            }
        }

        void ILineTracker.SetLineLength(DocumentLine line, int newTotalLength)
        {
            var targetTracker = targetObject.Target as ILineTracker;
            if (targetTracker != null)
            {
                targetTracker.SetLineLength(line, newTotalLength);
            }
            else
            {
                Deregister();
            }
        }

        void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
        {
            var targetTracker = targetObject.Target as ILineTracker;
            if (targetTracker != null)
            {
                targetTracker.LineInserted(insertionPos, newLine);
            }
            else
            {
                Deregister();
            }
        }

        void ILineTracker.RebuildDocument()
        {
            var targetTracker = targetObject.Target as ILineTracker;
            if (targetTracker != null)
            {
                targetTracker.RebuildDocument();
            }
            else
            {
                Deregister();
            }
        }

        void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
        {
            var targetTracker = targetObject.Target as ILineTracker;
            if (targetTracker != null)
            {
                targetTracker.ChangeComplete(e);
            }
            else
            {
                Deregister();
            }
        }

        #endregion
    }
}