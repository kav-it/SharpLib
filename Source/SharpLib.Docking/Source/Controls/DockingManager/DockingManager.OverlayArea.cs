using System.Windows;

namespace SharpLib.Docking.Controls
{
    public class DockingManagerOverlayArea : OverlayArea
    {
        #region Поля

        private DockingManager _manager;

        #endregion

        #region Конструктор

        internal DockingManagerOverlayArea(IOverlayWindow overlayWindow, DockingManager manager)
            : base(overlayWindow)
        {
            _manager = manager;

            SetScreenDetectionArea(new Rect(_manager.PointToScreenDPI(new Point()), _manager.TransformActualSizeToAncestor()));
        }

        #endregion
    }
}