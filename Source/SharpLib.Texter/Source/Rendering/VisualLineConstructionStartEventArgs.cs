using System;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Rendering
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