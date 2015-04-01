using System;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Rendering
{
    public class VisualLineConstructionStartEventArgs : EventArgs
    {
        #region Свойства

        public DocumentLine FirstLineInView { get; private set; }

        #endregion

        #region Конструктор

        public VisualLineConstructionStartEventArgs(DocumentLine firstLineInView)
        {
            if (firstLineInView == null)
            {
                throw new ArgumentNullException("firstLineInView");
            }
            FirstLineInView = firstLineInView;
        }

        #endregion
    }
}