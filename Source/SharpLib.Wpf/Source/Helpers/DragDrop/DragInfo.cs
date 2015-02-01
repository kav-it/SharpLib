using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using SharpLib.Wpf.Dragging.Utilities;

namespace SharpLib.Wpf.Dragging
{
    public class DragInfo : IDragInfo
    {
        #region Свойства

        public object Data { get; set; }

        public Point DragStartPosition { get; private set; }

        public Point PositionInDraggedItem { get; private set; }

        public DragDropEffects Effects { get; set; }

        public MouseButton MouseButton { get; private set; }

        public IEnumerable SourceCollection { get; private set; }

        public int SourceIndex { get; private set; }

        public object SourceItem { get; private set; }

        public IEnumerable SourceItems { get; private set; }

        public CollectionViewGroup SourceGroup { get; private set; }

        public UIElement VisualSource { get; private set; }

        public UIElement VisualSourceItem { get; private set; }

        public FlowDirection VisualSourceFlowDirection { get; private set; }

        public IDataObject DataObject { get; set; }

        #endregion

        #region Конструктор

        public DragInfo(object sender, MouseButtonEventArgs e)
        {
            DragStartPosition = e.GetPosition((IInputElement)sender);
            Effects = DragDropEffects.None;
            MouseButton = e.ChangedButton;
            VisualSource = sender as UIElement;

            if (sender is ItemsControl)
            {
                var itemsControl = (ItemsControl)sender;

                SourceGroup = itemsControl.FindGroup(DragStartPosition);
                VisualSourceFlowDirection = itemsControl.GetItemsPanelFlowDirection();

                var sourceItem = e.OriginalSource as UIElement;
                if (sourceItem == null && e.OriginalSource is FrameworkContentElement)
                {
                    sourceItem = ((FrameworkContentElement)e.OriginalSource).Parent as UIElement;
                }
                UIElement item = null;
                if (sourceItem != null)
                {
                    item = itemsControl.GetItemContainer(sourceItem);
                }

                if (item == null)
                {
                    item = DragDrop.GetDragDirectlySelectedOnly(VisualSource) 
                        ? itemsControl.GetItemContainerAt(e.GetPosition(itemsControl)) 
                        : itemsControl.GetItemContainerAt(e.GetPosition(itemsControl), itemsControl.GetItemsPanelOrientation());
                }

                if (item != null)
                {
                    PositionInDraggedItem = e.GetPosition(item);

                    var itemParent = ItemsControl.ItemsControlFromItemContainer(item);

                    if (itemParent != null)
                    {
                        SourceCollection = itemParent.ItemsSource ?? itemParent.Items;
                        SourceIndex = itemParent.ItemContainerGenerator.IndexFromContainer(item);
                        SourceItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
                    }
                    else
                    {
                        SourceIndex = -1;
                    }
                    SourceItems = itemsControl.GetSelectedItems();

                    if (SourceItems.Cast<object>().Count() <= 1)
                    {
                        SourceItems = Enumerable.Repeat(SourceItem, 1);
                    }

                    VisualSourceItem = item;
                }
                else
                {
                    SourceCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                }
            }
            else
            {
                if (sender is UIElement)
                {
                    PositionInDraggedItem = e.GetPosition((UIElement)sender);
                }
            }

            if (SourceItems == null)
            {
                SourceItems = Enumerable.Empty<object>();
            }
        }

        #endregion
    }
}