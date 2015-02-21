using System.Windows;
using System.Windows.Media;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    internal interface IDropTarget
    {
        #region Свойства

        DropTargetType Type { get; }

        #endregion

        #region Методы

        Geometry GetPreviewPath(OverlayWindow overlayWindow, LayoutFloatingWindow floatingWindow);

        bool HitTest(Point dragPoint);

        void Drop(LayoutFloatingWindow floatingWindow);

        void DragEnter();

        void DragLeave();

        #endregion
    }
}