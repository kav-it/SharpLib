using System.Windows;

namespace SharpLib.Docking.Controls
{
    public class DocumentPaneControlOverlayArea : OverlayArea
    {
        #region Поля

        private LayoutDocumentPaneControl _documentPaneControl;

        #endregion

        #region Конструктор

        internal DocumentPaneControlOverlayArea(IOverlayWindow overlayWindow, LayoutDocumentPaneControl documentPaneControl): base(overlayWindow)
        {
            _documentPaneControl = documentPaneControl;
            SetScreenDetectionArea(new Rect(_documentPaneControl.PointToScreenDPI(new Point()), _documentPaneControl.TransformActualSizeToAncestor()));
        }

        #endregion
    }
}