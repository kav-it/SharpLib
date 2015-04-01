using System;
using System.Windows.Documents;

namespace SharpLib.Notepad.Snippets
{
    [Serializable]
    public abstract class SnippetElement
    {
        #region Методы

        public abstract void Insert(InsertionContext context);

        public virtual Inline ToTextRun()
        {
            return null;
        }

        #endregion
    }
}