using System.Windows;

namespace SharpLib.Docking.Controls
{
    public class OverlayWindowDropTarget : IOverlayWindowDropTarget
    {
        #region Поля

        private readonly Rect _screenDetectionArea;

        private readonly OverlayWindowDropTargetType _type;

        private IOverlayWindowArea _overlayArea;

        #endregion

        #region Свойства

        Rect IOverlayWindowDropTarget.ScreenDetectionArea
        {
            get { return _screenDetectionArea; }
        }

        OverlayWindowDropTargetType IOverlayWindowDropTarget.Type
        {
            get { return _type; }
        }

        #endregion

        #region Конструктор

        internal OverlayWindowDropTarget(IOverlayWindowArea overlayArea, OverlayWindowDropTargetType targetType, FrameworkElement element)
        {
            _overlayArea = overlayArea;
            _type = targetType;
            _screenDetectionArea = new Rect(element.TransformToDeviceDPI(new Point()), element.TransformActualSizeToAncestor());
        }

        #endregion
    }
}