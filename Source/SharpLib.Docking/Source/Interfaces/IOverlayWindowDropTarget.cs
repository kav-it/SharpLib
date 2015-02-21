using System.Windows;

namespace SharpLib.Docking.Controls
{
    internal interface IOverlayWindowDropTarget
    {
        #region Свойства

        Rect ScreenDetectionArea { get; }

        OverlayWindowDropTargetType Type { get; }

        #endregion
    }
}