using System.Collections.Generic;
using System.Windows;

namespace SharpLib.Docking.Controls
{
    internal interface IOverlayWindowHost
    {
        #region Свойства

        DockingManager Manager { get; }

        #endregion

        #region Методы

        bool HitTest(Point dragPoint);

        IOverlayWindow ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow);

        void HideOverlayWindow();

        IEnumerable<IDropArea> GetDropAreas(LayoutFloatingWindowControl draggingWindow);

        #endregion
    }
}