using System.Windows;

namespace SharpLib.Docking.Controls
{
    public class AnchorablePaneControlOverlayArea : OverlayArea
    {
        #region Поля

        private LayoutAnchorablePaneControl _anchorablePaneControl;

        #endregion

        #region Конструктор

        internal AnchorablePaneControlOverlayArea(IOverlayWindow overlayWindow, LayoutAnchorablePaneControl anchorablePaneControl)
            : base(overlayWindow)
        {
            _anchorablePaneControl = anchorablePaneControl;
            SetScreenDetectionArea(new Rect(_anchorablePaneControl.PointToScreenDPI(new Point()), _anchorablePaneControl.TransformActualSizeToAncestor()));
        }

        #endregion
    }
}