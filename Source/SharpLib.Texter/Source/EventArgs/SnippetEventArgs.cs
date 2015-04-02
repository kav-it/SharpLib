using System;

namespace SharpLib.Texter.Snippets
{
    public class SnippetEventArgs : EventArgs
    {
        #region Свойства

        public DeactivateReason Reason { get; private set; }

        #endregion

        #region Конструктор

        public SnippetEventArgs(DeactivateReason reason)
        {
            Reason = reason;
        }

        #endregion
    }
}