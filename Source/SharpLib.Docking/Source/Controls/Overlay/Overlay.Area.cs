using System.Windows;

namespace SharpLib.Docking.Controls
{
    public abstract class OverlayArea : IOverlayWindowArea
    {
        #region Поля

        private IOverlayWindow _overlayWindow;

        private Rect? _screenDetectionArea;

        #endregion

        #region Свойства

        Rect IOverlayWindowArea.ScreenDetectionArea
        {
            get { return _screenDetectionArea.Value; }
        }

        #endregion

        #region Конструктор

        internal OverlayArea(IOverlayWindow overlayWindow)
        {
            _overlayWindow = overlayWindow;
        }

        #endregion

        #region Методы

        protected void SetScreenDetectionArea(Rect rect)
        {
            _screenDetectionArea = rect;
        }

        #endregion
    }
}