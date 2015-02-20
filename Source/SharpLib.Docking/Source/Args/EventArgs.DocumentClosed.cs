using System;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking
{
    public class DocumentClosedEventArgs : EventArgs
    {
        #region Свойства

        public LayoutDocument Document { get; private set; }

        #endregion

        #region Конструктор

        public DocumentClosedEventArgs(LayoutDocument document)
        {
            Document = document;
        }

        #endregion
    }
}