using System.Windows;

namespace SharpLib.Docking
{
    internal interface ILayoutPositionableElement : ILayoutElement, ILayoutElementForFloatingWindow
    {
        #region Свойства

        GridLength DockWidth { get; set; }

        GridLength DockHeight { get; set; }

        double DockMinWidth { get; set; }

        double DockMinHeight { get; set; }

        bool IsVisible { get; }

        #endregion
    }
}