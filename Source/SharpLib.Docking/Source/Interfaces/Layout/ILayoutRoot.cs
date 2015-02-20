using System.Collections.ObjectModel;

namespace SharpLib.Docking.Layout
{
    public interface ILayoutRoot
    {
        #region Свойства

        DockingManager Manager { get; }

        LayoutPanel RootPanel { get; }

        LayoutAnchorSide TopSide { get; }

        LayoutAnchorSide LeftSide { get; }

        LayoutAnchorSide RightSide { get; }

        LayoutAnchorSide BottomSide { get; }

        LayoutContent ActiveContent { get; set; }

        ObservableCollection<LayoutFloatingWindow> FloatingWindows { get; }

        ObservableCollection<LayoutAnchorable> Hidden { get; }

        #endregion

        #region Методы

        void CollectGarbage();

        #endregion
    }
}