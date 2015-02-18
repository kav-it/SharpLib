using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

using SharpLib.Wpf.Dragging;

namespace SharpLib.Wpf.Controls
{
    public class TreeListEx : ListView
    {
        #region Поля

        public static readonly DependencyProperty AllowDropOrderProperty;

        public static readonly DependencyProperty RootProperty;

        public static readonly DependencyProperty ShowAlternationProperty;

        public static readonly DependencyProperty ShowLinesProperty;

        public static readonly DependencyProperty ShowRootExpanderProperty;

        public static readonly DependencyProperty ShowRootProperty;

        private TreeListExFlattener _listFlattener;

        #endregion

        #region Свойства

        public static ResourceKey DefaultItemContainerStyleKey { get; private set; }

        public TreeListExNode Root
        {
            get { return (TreeListExNode)GetValue(RootProperty); }
            set { SetValue(RootProperty, value); }
        }

        public bool ShowRoot
        {
            get { return (bool)GetValue(ShowRootProperty); }
            set { SetValue(ShowRootProperty, value); }
        }

        public bool ShowRootExpander
        {
            get { return (bool)GetValue(ShowRootExpanderProperty); }
            set { SetValue(ShowRootExpanderProperty, value); }
        }

        public bool AllowDropOrder
        {
            get { return (bool)GetValue(AllowDropOrderProperty); }
            set { SetValue(AllowDropOrderProperty, value); }
        }

        public bool ShowLines
        {
            get { return (bool)GetValue(ShowLinesProperty); }
            set { SetValue(ShowLinesProperty, value); }
        }

        /// <summary>
        /// Сортировка
        /// </summary>
        public TreeListExSorter Sorter { get; set; }

        /// <summary>
        /// Элемент является источником операций DragDrop
        /// </summary>
        public IDragSource DragSource
        {
            get { return Dragging.DragDrop.GetDragHandler(this); }
            set
            {

                Dragging.DragDrop.SetDragHandler(this, value);
                Dragging.DragDrop.SetIsDragSource(this, value != null);
            }
        }
        /// <summary>
        /// Элемент является получателем операций DragDrop
        /// </summary>
        public IDragDest DragDest
        {
            get { return Dragging.DragDrop.GetDropHandler(this); }
            set
            {
                Dragging.DragDrop.SetDropHandler(this, value);
                Dragging.DragDrop.SetIsDropTarget(this, value != null);
            }
        }

        #endregion

        #region Конструктор

        static TreeListEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListEx), new FrameworkPropertyMetadata(typeof(TreeListEx)));
            SelectionModeProperty.OverrideMetadata(typeof(TreeListEx), new FrameworkPropertyMetadata(SelectionMode.Extended));
            AlternationCountProperty.OverrideMetadata(typeof(TreeListEx), new FrameworkPropertyMetadata(2));
            DefaultItemContainerStyleKey = new ComponentResourceKey(typeof(TreeListEx), "DefaultItemContainerStyleKey");
            VirtualizingStackPanel.VirtualizationModeProperty.OverrideMetadata(typeof(TreeListEx), new FrameworkPropertyMetadata(VirtualizationMode.Recycling));
            AllowDropOrderProperty = DependencyProperty.Register("AllowDropOrder", typeof(bool), typeof(TreeListEx));
            RootProperty = DependencyProperty.Register("Root", typeof(TreeListExNode), typeof(TreeListEx));
            ShowAlternationProperty = DependencyProperty.RegisterAttached("ShowAlternation", typeof(bool), typeof(TreeListEx),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
            ShowLinesProperty = DependencyProperty.Register("ShowLines", typeof(bool), typeof(TreeListEx), new FrameworkPropertyMetadata(true));
            ShowRootExpanderProperty = DependencyProperty.Register("ShowRootExpander", typeof(bool), typeof(TreeListEx), new FrameworkPropertyMetadata(true));
            ShowRootProperty = DependencyProperty.Register("ShowRoot", typeof(bool), typeof(TreeListEx), new FrameworkPropertyMetadata(true));
        }

        public TreeListEx()
        {
            SetResourceReference(ItemContainerStyleProperty, DefaultItemContainerStyleKey);
        }

        #endregion

        #region Методы

        public static bool GetShowAlternation(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowAlternationProperty);
        }

        public static void SetShowAlternation(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowAlternationProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RootProperty ||
                e.Property == ShowRootProperty ||
                e.Property == ShowRootExpanderProperty)
            {
                Reload();
            }
        }

        private void Reload()
        {
            if (_listFlattener != null)
            {
                _listFlattener.Stop();
            }
            if (Root != null)
            {
                if (!(ShowRoot && ShowRootExpander))
                {
                    Root.IsExpanded = true;
                }
                _listFlattener = new TreeListExFlattener(this, Root, ShowRoot);
                _listFlattener.CollectionChanged += ListFlattenerCollectionChanged;
                ItemsSource = _listFlattener;
            }
        }

        private void ListFlattenerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Deselect listNodes that are being hidden, if any remain in the tree
            if (e.Action == NotifyCollectionChangedAction.Remove && Items.Count > 0)
            {
                List<TreeListExNode> selectedOldItems = null;
                foreach (TreeListExNode node in e.OldItems)
                {
                    if (node.IsSelected)
                    {
                        if (selectedOldItems == null)
                        {
                            selectedOldItems = new List<TreeListExNode>();
                        }
                        selectedOldItems.Add(node);
                    }
                }
                if (selectedOldItems != null)
                {
                    var list = SelectedItems.Cast<TreeListExNode>().Except(selectedOldItems).ToList();
                    SetSelectedItems(list);
                    if (SelectedItem == null && IsKeyboardFocusWithin)
                    {
                        // if we removed all selected listNodes, then move the focus to the listNode
                        // preceding the first of the old selected listNodes
                        SelectedIndex = Math.Max(0, e.OldStartingIndex - 1);
                        if (SelectedIndex >= 0)
                        {
                            FocusNode((TreeListExNode)SelectedItem);
                        }
                    }
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListExViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListExViewItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            TreeListExViewItem container = element as TreeListExViewItem;
            if (container != null)
            {
                container.ParentTreeList = this;
                // Make sure that the line renderer takes into account the new bound data
                if (container.ListNodeView != null)
                {
                    container.ListNodeView.TreeListLinesRenderer.InvalidateVisual();
                }
            }
        }

        /// <summary>
        /// Handles the listNode expanding event in the tree view.
        /// This method gets called only if the listNode is in the visible region (a TreeListExNodeView exists).
        /// </summary>
        internal void HandleExpanding(TreeListExNode listNode)
        {
            // 20150214 - Откючение автоскролл при Expand
            // if (_doNotScrollOnExpanding)
            // {
            //    return;
            // }

            //TreeListExNode lastVisibleChild = listNode;
            //while (true)
            //{
            //    TreeListExNode tmp = lastVisibleChild.Children.LastOrDefault(c => c.IsVisible);
            //    if (tmp != null)
            //    {
            //        lastVisibleChild = tmp;
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
            //if (lastVisibleChild != listNode)
            //{
            //    // Make the the expanded children are visible; but don't scroll down
            //    // to much (keep listNode itself visible)
            //    base.ScrollIntoView(lastVisibleChild);
            //    // For some reason, this only works properly when delaying it...
            //    Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => base.ScrollIntoView(listNode)));
            //}
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            TreeListExViewItem container = e.OriginalSource as TreeListExViewItem;
            switch (e.Key)
            {
                case Key.Left:
                    if (container != null && Equals(ItemsControlFromItemContainer(container), this))
                    {
                        if (container.ListNode.IsExpanded)
                        {
                            container.ListNode.IsExpanded = false;
                        }
                        else if (container.ListNode.Parent != null)
                        {
                            FocusNode(container.ListNode.Parent);
                        }
                        e.Handled = true;
                    }
                    break;
                case Key.Right:
                    if (container != null && Equals(ItemsControlFromItemContainer(container), this))
                    {
                        if (!container.ListNode.IsExpanded && container.ListNode.ShowExpander)
                        {
                            container.ListNode.IsExpanded = true;
                        }
                        else if (container.ListNode.Children.Count > 0)
                        {
                            // jump to first child:
                            container.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        }
                        e.Handled = true;
                    }
                    break;
                case Key.Return:
                    if (container != null && Keyboard.Modifiers == ModifierKeys.None && SelectedItems.Count == 1 && SelectedItem == container.ListNode)
                    {
                        e.Handled = true;
                        container.ListNode.ActivateItem(e);
                    }
                    break;
                case Key.Space:
                    if (container != null && Keyboard.Modifiers == ModifierKeys.None && SelectedItems.Count == 1 && SelectedItem == container.ListNode)
                    {
                        e.Handled = true;
                        if (container.ListNode.IsCheckable)
                        {
                            if (container.ListNode.IsChecked == null) // If partially selected, we want to select everything
                            {
                                container.ListNode.IsChecked = true;
                            }
                            else
                            {
                                container.ListNode.IsChecked = !container.ListNode.IsChecked;
                            }
                        }
                        else
                        {
                            container.ListNode.ActivateItem(e);
                        }
                    }
                    break;
                case Key.Add:
                    if (container != null && Equals(ItemsControlFromItemContainer(container), this))
                    {
                        container.ListNode.IsExpanded = true;
                        e.Handled = true;
                    }
                    break;
                case Key.Subtract:
                    if (container != null && Equals(ItemsControlFromItemContainer(container), this))
                    {
                        container.ListNode.IsExpanded = false;
                        e.Handled = true;
                    }
                    break;
                case Key.Multiply:
                    if (container != null && Equals(ItemsControlFromItemContainer(container), this))
                    {
                        container.ListNode.ExpandAll();
                        e.Handled = true;
                    }
                    break;
            }
            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }

        /// <summary>
        /// Очистка дерева
        /// </summary>
        public void Clear()
        {
            ItemsSource = null;
        }

        /// <summary>
        /// Выделение (фокусом) элементы (со скроллом)
        /// </summary>
        public void FocusNode(TreeListExNode listNode)
        {
            if (listNode == null)
            {
                throw new ArgumentNullException("listNode");
            }
            ScrollIntoView(listNode);
            // WPF's ScrollIntoView() uses the same if/dispatcher construct, so we call OnFocusItem() after the item was brought into view.
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                OnFocusItem(listNode);
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DispatcherOperationCallback(OnFocusItem), listNode);
            }
        }

        public void ScrollIntoView(TreeListExNode listNode)
        {
            if (listNode == null)
            {
                throw new ArgumentNullException("listNode");
            }
            // _doNotScrollOnExpanding = true;
            foreach (TreeListExNode ancestor in listNode.Ancestors())
            {
                ancestor.IsExpanded = true;
            }
            // _doNotScrollOnExpanding = false;
            base.ScrollIntoView(listNode);
        }

        private object OnFocusItem(object item)
        {
            FrameworkElement element = ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
            if (element != null)
            {
                element.Focus();
            }
            return null;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (TreeListExNode node in e.RemovedItems)
            {
                node.IsSelected = false;
            }
            foreach (TreeListExNode node in e.AddedItems)
            {
                node.IsSelected = true;
            }
            base.OnSelectionChanged(e);
        }

        public IEnumerable<TreeListExNode> GetTopLevelSelection()
        {
            var selection = SelectedItems.OfType<TreeListExNode>();
            var treeListNodes = selection as IList<TreeListExNode> ?? selection.ToList();
            var selectionHash = new HashSet<TreeListExNode>(treeListNodes);

            return treeListNodes.Where(item => item.Ancestors().All(a => !selectionHash.Contains(a)));
        }

        /// <summary>
        /// Свернуть все элементы
        /// </summary>
        public void ExpandAll()
        {
            if (Root != null)
            {
                Root.ExpandAll();
            }
        }

        /// <summary>
        /// Развернуть все элементы
        /// </summary>
        public void CollapseAll()
        {
            if (Root != null)
            {
                Root.CollapseAll();
            }
        }

        /// <summary>
        /// Выделение элементов
        /// </summary>
        public void SetSelectedItems(IEnumerable<TreeListExNode> items)
        {
            base.SetSelectedItems(items);
        }

        #endregion
    }
}