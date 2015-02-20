using System.ComponentModel;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking
{
    public class DocumentClosingEventArgs : CancelEventArgs
    {
        #region Свойства

        public LayoutDocument Document { get; private set; }

        #endregion

        #region Конструктор

        public DocumentClosingEventArgs(LayoutDocument document)
        {
            Document = document;
        }

        #endregion
    }
}