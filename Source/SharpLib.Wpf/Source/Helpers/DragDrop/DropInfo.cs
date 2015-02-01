using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using SharpLib.Wpf.Dragging.Utilities;

namespace SharpLib.Wpf.Dragging
{
    public class DropInfo : IDropInfo
    {
        #region Поля

        private readonly ItemsControl _itemParent;

        private UIElement item;

        #endregion

        #region Свойства

        public object Data { get; private set; }

        public IDragInfo DragInfo { get; private set; }

        public Point DropPosition { get; private set; }

        public Type DropTargetAdorner { get; set; }

        public DragDropEffects Effects { get; set; }

        public int InsertIndex { get; private set; }

        public int UnfilteredInsertIndex
        {
            get
            {
                var insertIndex = InsertIndex;
                if (_itemParent != null)
                {
                    var itemSourceAsList = _itemParent.ItemsSource.ToList();
                    if (itemSourceAsList != null && _itemParent.Items != null && _itemParent.Items.Count != itemSourceAsList.Count)
                    {
                        if (insertIndex >= 0 && insertIndex < _itemParent.Items.Count)
                        {
                            var indexOf = itemSourceAsList.IndexOf(_itemParent.Items[insertIndex]);
                            if (indexOf >= 0)
                            {
                                return indexOf;
                            }
                        }
                    }
                }
                return insertIndex;
            }
        }

        public IEnumerable TargetCollection { get; private set; }

        public object TargetItem { get; private set; }

        public CollectionViewGroup TargetGroup { get; private set; }

        public UIElement VisualTarget { get; private set; }

        public UIElement VisualTargetItem { get; private set; }

        public Orientation VisualTargetOrientation { get; private set; }

        public FlowDirection VisualTargetFlowDirection { get; private set; }

        public string DestinationText { get; set; }

        public RelativeInsertPosition InsertPosition { get; private set; }

        public DragDropKeyStates KeyStates { get; private set; }

        public bool NotHandled { get; set; }

        public bool IsSameDragDropContextAsSource
        {
            get
            {
                if (DragInfo == null || DragInfo.VisualSource == null)
                {
                    return true;
                }

                if (VisualTarget == null)
                {
                    return true;
                }

                var sourceContext = DragInfo.VisualSource.GetHashCode();
                var targetContext = VisualTarget.GetHashCode();

                return sourceContext == targetContext;
            }
        }

        #endregion

        #region Конструктор

        public DropInfo(object sender, DragEventArgs e, DragInfo dragInfo)
        {
            var dataFormat = DragDrop.DataFormat.Name;
            Data = (e.Data.GetDataPresent(dataFormat)) ? e.Data.GetData(dataFormat) : e.Data;
            DragInfo = dragInfo;
            KeyStates = e.KeyStates;

            VisualTarget = sender as UIElement;

            if (!(VisualTarget is ItemsControl))
            {
                var itemsControl = VisualTarget.GetVisualAncestor<ItemsControl>();
                if (itemsControl != null)
                {
                    if (DragDrop.GetIsDropTarget(itemsControl))
                    {
                        VisualTarget = itemsControl;
                    }
                }
            }

            DropPosition = VisualTarget != null ? e.GetPosition(VisualTarget) : new Point();

            if (VisualTarget is ItemsControl)
            {
                var itemsControl = (ItemsControl)VisualTarget;
                item = itemsControl.GetItemContainerAt(DropPosition);
                var directlyOverItem = item != null;

                TargetGroup = itemsControl.FindGroup(DropPosition);
                VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();
                VisualTargetFlowDirection = itemsControl.GetItemsPanelFlowDirection();

                if (item == null)
                {
                    item = itemsControl.GetItemContainerAt(DropPosition, VisualTargetOrientation);
                    directlyOverItem = false;
                }

                if (item != null)
                {
                    _itemParent = ItemsControl.ItemsControlFromItemContainer(item);

                    InsertIndex = _itemParent.ItemContainerGenerator.IndexFromContainer(item);
                    TargetCollection = _itemParent.ItemsSource ?? _itemParent.Items;

                    if (directlyOverItem || item is TreeViewItem)
                    {
                        TargetItem = _itemParent.ItemContainerGenerator.ItemFromContainer(item);
                        VisualTargetItem = item;
                    }

                    var itemRenderSize = item.RenderSize;

                    if (VisualTargetOrientation == Orientation.Vertical)
                    {
                        var currentYPos = e.GetPosition(item).Y;
                        var targetHeight = itemRenderSize.Height;

                        if (currentYPos > targetHeight / 2)
                        {
                            InsertIndex++;
                            InsertPosition = RelativeInsertPosition.AfterTargetItem;
                        }
                        else
                        {
                            InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                        }

                        if (currentYPos > targetHeight * 0.25 && currentYPos < targetHeight * 0.75)
                        {
                            InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }
                    }
                    else
                    {
                        var currentXPos = e.GetPosition(item).X;
                        var targetWidth = itemRenderSize.Width;

                        if ((VisualTargetFlowDirection == FlowDirection.RightToLeft && currentXPos < targetWidth / 2)
                            || (VisualTargetFlowDirection == FlowDirection.LeftToRight && currentXPos > targetWidth / 2))
                        {
                            InsertIndex++;
                            InsertPosition = RelativeInsertPosition.AfterTargetItem;
                        }
                        else
                        {
                            InsertPosition = RelativeInsertPosition.BeforeTargetItem;
                        }

                        if (currentXPos > targetWidth * 0.25 && currentXPos < targetWidth * 0.75)
                        {
                            InsertPosition |= RelativeInsertPosition.TargetItemCenter;
                        }
                    }
                }
                else
                {
                    TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                    InsertIndex = itemsControl.Items.Count;
                }
            }
        }

        #endregion
    }

    [Flags]
    public enum RelativeInsertPosition
    {
        BeforeTargetItem = 0,

        AfterTargetItem = 1,

        TargetItemCenter = 2
    }
}