using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SharpLib.Wpf.Dragging
{
    public interface IDropInfo
    {
        #region Свойства

        object Data { get; }

        IDragInfo DragInfo { get; }

        Point DropPosition { get; }

        Type DropTargetAdorner { get; set; }

        DragDropEffects Effects { get; set; }

        int InsertIndex { get; }

        int UnfilteredInsertIndex { get; }

        IEnumerable TargetCollection { get; }

        object TargetItem { get; }

        CollectionViewGroup TargetGroup { get; }

        UIElement VisualTarget { get; }

        UIElement VisualTargetItem { get; }

        Orientation VisualTargetOrientation { get; }

        FlowDirection VisualTargetFlowDirection { get; }

        string DestinationText { get; set; }

        RelativeInsertPosition InsertPosition { get; }

        DragDropKeyStates KeyStates { get; }

        bool NotHandled { get; set; }

        bool IsSameDragDropContextAsSource { get; }

        #endregion
    }
}