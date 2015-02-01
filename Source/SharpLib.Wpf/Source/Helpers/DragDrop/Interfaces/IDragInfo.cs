using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SharpLib.Wpf.Dragging
{
    public interface IDragInfo
    {
        #region Свойства

        object Data { get; set; }

        Point DragStartPosition { get; }

        Point PositionInDraggedItem { get; }

        DragDropEffects Effects { get; set; }

        MouseButton MouseButton { get; }

        IEnumerable SourceCollection { get; }

        int SourceIndex { get; }

        object SourceItem { get; }

        IEnumerable SourceItems { get; }

        CollectionViewGroup SourceGroup { get; }

        UIElement VisualSource { get; }

        UIElement VisualSourceItem { get; }

        FlowDirection VisualSourceFlowDirection { get; }

        IDataObject DataObject { get; set; }

        #endregion
    }
}