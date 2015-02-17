using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace SharpLib.Wpf.Controls
{
    public class TreeListEx : ListView
    {
        #region Перечисления

        private enum DropPlace
        {
            Before,

            Inside,

            After
        }

        #endregion

        #region Поля

        public static readonly DependencyProperty AllowDropOrderProperty;

        public static readonly DependencyProperty RootProperty;

        public static readonly DependencyProperty ShowAlternationProperty;

        public static readonly DependencyProperty ShowLinesProperty;

        public static readonly DependencyProperty ShowRootExpanderProperty;

        public static readonly DependencyProperty ShowRootProperty;

        // private bool _doNotScrollOnExpanding;

        private TreeListExFlattener _listFlattener;

        private TreeListExNodeView _previewListNodeView;

        // ReSharper disable NotAccessedField.Local
        private DropPlace _previewPlace;

        // ReSharper restore NotAccessedField.Local

        private TreeListExInsertMarker _treeListInsertMarker;

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

            CommandManager.RegisterClassCommandBinding(typeof(TreeListEx), new CommandBinding(ApplicationCommands.Cut, HandleExecuted_Cut, HandleCanExecute_Cut));
            CommandManager.RegisterClassCommandBinding(typeof(TreeListEx), new CommandBinding(ApplicationCommands.Copy, HandleExecuted_Copy, HandleCanExecute_Copy));
            CommandManager.RegisterClassCommandBinding(typeof(TreeListEx), new CommandBinding(ApplicationCommands.Paste, HandleExecuted_Paste, HandleCanExecute_Paste));
            CommandManager.RegisterClassCommandBinding(typeof(TreeListEx), new CommandBinding(ApplicationCommands.Delete, HandleExecuted_Delete, HandleCanExecute_Delete));
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
        /// Scrolls the specified listNode in view and sets keyboard focus on it.
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

        protected override void OnDragEnter(DragEventArgs e)
        {
            OnDragOver(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (Root != null && !ShowRoot)
            {
                e.Handled = true;
                e.Effects = Root.GetDropEffect(e, Root.Children.Count);
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (Root != null && !ShowRoot)
            {
                e.Handled = true;
                e.Effects = Root.GetDropEffect(e, Root.Children.Count);
                if (e.Effects != DragDropEffects.None)
                {
                    Root.InternalDrop(e, Root.Children.Count);
                }
            }
        }

        internal void HandleDragEnter(TreeListExViewItem item, DragEventArgs e)
        {
            HandleDragOver(item, e);
        }

        internal void HandleDragOver(TreeListExViewItem item, DragEventArgs e)
        {
            HidePreview();
            e.Effects = DragDropEffects.None;

            var target = GetDropTarget(item, e);
            if (target != null)
            {
                e.Handled = true;
                e.Effects = target._effect;
                ShowPreview(target._item, target._place);
            }
        }

        internal void HandleDrop(TreeListExViewItem item, DragEventArgs e)
        {
            try
            {
                HidePreview();

                var target = GetDropTarget(item, e);
                if (target != null)
                {
                    e.Handled = true;
                    e.Effects = target._effect;
                    target._listNode.InternalDrop(e, target._index);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        internal void HandleDragLeave(TreeListExViewItem item, DragEventArgs e)
        {
            HidePreview();
            e.Handled = true;
        }

        private DropTarget GetDropTarget(TreeListExViewItem item, DragEventArgs e)
        {
            var dropTargets = BuildDropTargets(item, e);
            var y = e.GetPosition(item).Y;

            return dropTargets.FirstOrDefault(target => target._y >= y);
        }

        private IEnumerable<DropTarget> BuildDropTargets(TreeListExViewItem item, DragEventArgs e)
        {
            var result = new List<DropTarget>();
            var node = item.ListNode;

            if (AllowDropOrder)
            {
                TryAddDropTarget(result, item, DropPlace.Before, e);
            }

            TryAddDropTarget(result, item, DropPlace.Inside, e);

            if (AllowDropOrder)
            {
                if (node.IsExpanded && node.Children.Count > 0)
                {
                    var firstChildItem = ItemContainerGenerator.ContainerFromItem(node.Children[0]) as TreeListExViewItem;
                    TryAddDropTarget(result, firstChildItem, DropPlace.Before, e);
                }
                else
                {
                    TryAddDropTarget(result, item, DropPlace.After, e);
                }
            }

            var h = item.ActualHeight;
            var y1 = 0.2 * h;
            var y2 = h / 2;
            var y3 = h - y1;

            if (result.Count == 2)
            {
                if (result[0]._place == DropPlace.Inside &&
                    result[1]._place != DropPlace.Inside)
                {
                    result[0]._y = y3;
                }
                else if (result[0]._place != DropPlace.Inside &&
                         result[1]._place == DropPlace.Inside)
                {
                    result[0]._y = y1;
                }
                else
                {
                    result[0]._y = y2;
                }
            }
            else if (result.Count == 3)
            {
                result[0]._y = y1;
                result[1]._y = y3;
            }
            if (result.Count > 0)
            {
                result[result.Count - 1]._y = h;
            }
            return result;
        }

        private void TryAddDropTarget(List<DropTarget> targets, TreeListExViewItem item, DropPlace place, DragEventArgs e)
        {
            TreeListExNode listNode;
            int index;

            GetNodeAndIndex(item, place, out listNode, out index);

            if (listNode != null)
            {
                var effect = listNode.GetDropEffect(e, index);
                if (effect != DragDropEffects.None)
                {
                    DropTarget target = new DropTarget
                    {
                        _item = item,
                        _place = place,
                        _listNode = listNode,
                        _index = index,
                        _effect = effect
                    };
                    targets.Add(target);
                }
            }
        }

        private void GetNodeAndIndex(TreeListExViewItem item, DropPlace place, out TreeListExNode listNode, out int index)
        {
            listNode = null;
            index = 0;

            if (place == DropPlace.Inside)
            {
                listNode = item.ListNode;
                index = listNode.Children.Count;
            }
            else if (place == DropPlace.Before)
            {
                if (item.ListNode.Parent != null)
                {
                    listNode = item.ListNode.Parent;
                    index = listNode.Children.IndexOf(item.ListNode);
                }
            }
            else
            {
                if (item.ListNode.Parent != null)
                {
                    listNode = item.ListNode.Parent;
                    index = listNode.Children.IndexOf(item.ListNode) + 1;
                }
            }
        }

        private void ShowPreview(TreeListExViewItem item, DropPlace place)
        {
            _previewListNodeView = item.ListNodeView;
            _previewPlace = place;

            if (place == DropPlace.Inside)
            {
                _previewListNodeView.TextBackground = SystemColors.HighlightBrush;
                _previewListNodeView.Foreground = SystemColors.HighlightTextBrush;
            }
            else
            {
                if (_treeListInsertMarker == null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    var adorner = new TreeListExGeneralAdorner(this);
                    _treeListInsertMarker = new TreeListExInsertMarker();
                    adorner.Child = _treeListInsertMarker;
                    adornerLayer.Add(adorner);
                }

                _treeListInsertMarker.Visibility = Visibility.Visible;

                var p1 = _previewListNodeView.TransformToVisual(this).Transform(new Point());
                var p = new Point(p1.X + _previewListNodeView.CalculateIndent() + 4.5, p1.Y - 3);

                if (place == DropPlace.After)
                {
                    p.Y += _previewListNodeView.ActualHeight;
                }

                _treeListInsertMarker.Margin = new Thickness(p.X, p.Y, 0, 0);

                TreeListExNodeView secondListNodeView = null;
                var index = _listFlattener.IndexOf(item.ListNode);

                if (place == DropPlace.Before)
                {
                    if (index > 0)
                    {
                        var indexItem = (ItemContainerGenerator.ContainerFromIndex(index - 1) as TreeListExViewItem);
                        if (indexItem != null)
                        {
                            secondListNodeView = indexItem.ListNodeView;
                        }
                    }
                }
                else if (index + 1 < _listFlattener.Count)
                {
                    var indexItem = (ItemContainerGenerator.ContainerFromIndex(index + 1) as TreeListExViewItem);
                    if (indexItem != null)
                    {
                        secondListNodeView = indexItem.ListNodeView;
                    }
                }

                var w = p1.X + _previewListNodeView.ActualWidth - p.X;

                if (secondListNodeView != null)
                {
                    var p2 = secondListNodeView.TransformToVisual(this).Transform(new Point());
                    w = Math.Max(w, p2.X + secondListNodeView.ActualWidth - p.X);
                }

                _treeListInsertMarker.Width = w + 10;
            }
        }

        private void HidePreview()
        {
            if (_previewListNodeView != null)
            {
                _previewListNodeView.ClearValue(TreeListExNodeView.TextBackgroundProperty);
                _previewListNodeView.ClearValue(ForegroundProperty);
                if (_treeListInsertMarker != null)
                {
                    _treeListInsertMarker.Visibility = Visibility.Collapsed;
                }
                _previewListNodeView = null;
            }
        }

        private static void HandleExecuted_Cut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            TreeListEx treeList = (TreeListEx)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            if (nodes.Length > 0)
            {
                nodes[0].Cut(nodes);
            }
        }

        private static void HandleCanExecute_Cut(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeListEx treeList = (TreeListEx)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            e.CanExecute = nodes.Length > 0 && nodes[0].CanCut(nodes);
            e.Handled = true;
        }

        private static void HandleExecuted_Copy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            TreeListEx treeList = (TreeListEx)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            if (nodes.Length > 0)
            {
                nodes[0].Copy(nodes);
            }
        }

        private static void HandleCanExecute_Copy(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeListEx treeList = (TreeListEx)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            e.CanExecute = nodes.Length > 0 && nodes[0].CanCopy(nodes);
            e.Handled = true;
        }

        private static void HandleExecuted_Paste(object sender, ExecutedRoutedEventArgs e)
        {
            TreeListEx treeList = (TreeListEx)sender;
            var data = Clipboard.GetDataObject();
            if (data != null)
            {
                var selectedNode = (treeList.SelectedItem as TreeListExNode) ?? treeList.Root;
                if (selectedNode != null)
                {
                    selectedNode.Paste(data);
                }
            }
            e.Handled = true;
        }

        private static void HandleCanExecute_Paste(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeListEx treeList = (TreeListEx)sender;
            var data = Clipboard.GetDataObject();
            if (data == null)
            {
                e.CanExecute = false;
            }
            else
            {
                var selectedNode = (treeList.SelectedItem as TreeListExNode) ?? treeList.Root;
                e.CanExecute = selectedNode != null && selectedNode.CanPaste(data);
            }
            e.Handled = true;
        }

        private static void HandleExecuted_Delete(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            var treeList = (TreeListEx)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            if (nodes.Length > 0)
            {
                nodes[0].DeleteChilds(nodes);
            }
        }

        private static void HandleCanExecute_Delete(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeListEx treeList = (TreeListEx)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            e.CanExecute = nodes.Length > 0 && nodes[0].CanDelete(nodes);
            e.Handled = true;
        }

        /// <summary>
        /// Gets the selected items which do not have any of their ancestors selected.
        /// </summary>
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

        #endregion

        #region Вложенный класс: DropTarget

        private class DropTarget
        {
            #region Поля

            public DragDropEffects _effect;

            public int _index;

            public TreeListExViewItem _item;

            public TreeListExNode _listNode;

            public DropPlace _place;

            public double _y;

            #endregion
        }

        #endregion
    }
}