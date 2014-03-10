// ****************************************************************************
//
// Имя файла    : 'TreeList.cs'
// Заголовок    : Компонент TreeList
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 11/03/2014
//
// ****************************************************************************

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

namespace SharpLib.Controls
{
    public class TreeList : ListView
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

        public static readonly DependencyProperty AllowDropOrderProperty = DependencyProperty.Register("AllowDropOrder", typeof(bool), typeof(TreeList));

        public static readonly DependencyProperty RootProperty = DependencyProperty.Register("Root", typeof(TreeListNode), typeof(TreeList));

        public static readonly DependencyProperty ShowAlternationProperty = DependencyProperty.RegisterAttached("ShowAlternation", typeof(bool), typeof(TreeList), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty ShowLinesProperty = DependencyProperty.Register("ShowLines", typeof(bool), typeof(TreeList), new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty ShowRootExpanderProperty = DependencyProperty.Register("ShowRootExpander", typeof(bool), typeof(TreeList), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty ShowRootProperty = DependencyProperty.Register("ShowRoot", typeof(bool), typeof(TreeList), new FrameworkPropertyMetadata(true));

        private bool _doNotScrollOnExpanding;

        private TreeListFlattener _listFlattener;

        private TreeListNodeView _previewListNodeView;

        private TreeListInsertMarker _treeListInsertMarker;

        // ReSharper disable NotAccessedField.Local
        private DropPlace _previewPlace;
        // ReSharper restore NotAccessedField.Local

        #endregion

        #region Свойства

        public static ResourceKey DefaultItemContainerStyleKey { get; private set; }

        public TreeListNode Root
        {
            get { return (TreeListNode)GetValue(RootProperty); }
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

        #endregion

        #region Конструктор

        static TreeList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeList),
                new FrameworkPropertyMetadata(typeof(TreeList)));

            SelectionModeProperty.OverrideMetadata(typeof(TreeList),
                new FrameworkPropertyMetadata(SelectionMode.Extended));

            AlternationCountProperty.OverrideMetadata(typeof(TreeList),
                new FrameworkPropertyMetadata(2));

            DefaultItemContainerStyleKey =
                new ComponentResourceKey(typeof(TreeList), "DefaultItemContainerStyleKey");

            VirtualizingStackPanel.VirtualizationModeProperty.OverrideMetadata(typeof(TreeList),
                new FrameworkPropertyMetadata(VirtualizationMode.Recycling));

            RegisterCommands();
        }

        public TreeList()
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
                Reload();
        }

        private void Reload()
        {
            if (_listFlattener != null)
                _listFlattener.Stop();
            if (Root != null)
            {
                if (!(ShowRoot && ShowRootExpander))
                    Root.IsExpanded = true;
                _listFlattener = new TreeListFlattener(Root, ShowRoot);
                _listFlattener.CollectionChanged += ListFlattenerCollectionChanged;
                ItemsSource = _listFlattener;
            }
        }

        private void ListFlattenerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Deselect listNodes that are being hidden, if any remain in the tree
            if (e.Action == NotifyCollectionChangedAction.Remove && Items.Count > 0)
            {
                List<TreeListNode> selectedOldItems = null;
                foreach (TreeListNode node in e.OldItems)
                {
                    if (node.IsSelected)
                    {
                        if (selectedOldItems == null)
                            selectedOldItems = new List<TreeListNode>();
                        selectedOldItems.Add(node);
                    }
                }
                if (selectedOldItems != null)
                {
                    var list = SelectedItems.Cast<TreeListNode>().Except(selectedOldItems).ToList();
                    SetSelectedItems(list);
                    if (SelectedItem == null && IsKeyboardFocusWithin)
                    {
                        // if we removed all selected listNodes, then move the focus to the listNode
                        // preceding the first of the old selected listNodes
                        SelectedIndex = Math.Max(0, e.OldStartingIndex - 1);
                        if (SelectedIndex >= 0)
                            FocusNode((TreeListNode)SelectedItem);
                    }
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            TreeListViewItem container = element as TreeListViewItem;
            if (container != null)
            {
                container.ParentTreeList = this;
                // Make sure that the line renderer takes into account the new bound data
                if (container.ListNodeView != null)
                    container.ListNodeView.TreeListLinesRenderer.InvalidateVisual();
            }
        }

        /// <summary>
        /// Handles the listNode expanding event in the tree view.
        /// This method gets called only if the listNode is in the visible region (a TreeListNodeView exists).
        /// </summary>
        internal void HandleExpanding(TreeListNode listNode)
        {
            if (_doNotScrollOnExpanding)
                return;
            TreeListNode lastVisibleChild = listNode;
            while (true)
            {
                TreeListNode tmp = lastVisibleChild.Children.LastOrDefault(c => c.IsVisible);
                if (tmp != null)
                    lastVisibleChild = tmp;
                else
                    break;
            }
            if (lastVisibleChild != listNode)
            {
                // Make the the expanded children are visible; but don't scroll down
                // to much (keep listNode itself visible)
                base.ScrollIntoView(lastVisibleChild);
                // For some reason, this only works properly when delaying it...
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(
                    () => base.ScrollIntoView(listNode)));
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            TreeListViewItem container = e.OriginalSource as TreeListViewItem;
            switch (e.Key)
            {
                case Key.Left:
                    if (container != null && Equals(ItemsControlFromItemContainer(container), this))
                    {
                        if (container.ListNode.IsExpanded)
                            container.ListNode.IsExpanded = false;
                        else if (container.ListNode.Parent != null)
                            FocusNode(container.ListNode.Parent);
                        e.Handled = true;
                    }
                    break;
                case Key.Right:
                    if (container != null && Equals(ItemsControlFromItemContainer(container), this))
                    {
                        if (!container.ListNode.IsExpanded && container.ListNode.ShowExpander)
                            container.ListNode.IsExpanded = true;
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
                                container.ListNode.IsChecked = true;
                            else
                                container.ListNode.IsChecked = !container.ListNode.IsChecked;
                        }
                        else
                            container.ListNode.ActivateItem(e);
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
                        container.ListNode.IsExpanded = true;
                        ExpandRecursively(container.ListNode);
                        e.Handled = true;
                    }
                    break;
            }
            if (!e.Handled)
                base.OnKeyDown(e);
        }

        private void ExpandRecursively(TreeListNode listNode)
        {
            if (listNode.CanExpandRecursively)
            {
                listNode.IsExpanded = true;
                foreach (TreeListNode child in listNode.Children)
                    ExpandRecursively(child);
            }
        }

        /// <summary>
        /// Scrolls the specified listNode in view and sets keyboard focus on it.
        /// </summary>
        public void FocusNode(TreeListNode listNode)
        {
            if (listNode == null)
                throw new ArgumentNullException("listNode");
            ScrollIntoView(listNode);
            // WPF's ScrollIntoView() uses the same if/dispatcher construct, so we call OnFocusItem() after the item was brought into view.
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                OnFocusItem(listNode);
            else
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DispatcherOperationCallback(OnFocusItem), listNode);
        }

        public void ScrollIntoView(TreeListNode listNode)
        {
            if (listNode == null)
                throw new ArgumentNullException("listNode");
            _doNotScrollOnExpanding = true;
            foreach (TreeListNode ancestor in listNode.Ancestors())
                ancestor.IsExpanded = true;
            _doNotScrollOnExpanding = false;
            base.ScrollIntoView(listNode);
        }

        private object OnFocusItem(object item)
        {
            FrameworkElement element = ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
            if (element != null)
                element.Focus();
            return null;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (TreeListNode node in e.RemovedItems)
                node.IsSelected = false;
            foreach (TreeListNode node in e.AddedItems)
                node.IsSelected = true;
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
                    Root.InternalDrop(e, Root.Children.Count);
            }
        }

        internal void HandleDragEnter(TreeListViewItem item, DragEventArgs e)
        {
            HandleDragOver(item, e);
        }

        internal void HandleDragOver(TreeListViewItem item, DragEventArgs e)
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

        internal void HandleDrop(TreeListViewItem item, DragEventArgs e)
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

        internal void HandleDragLeave(TreeListViewItem item, DragEventArgs e)
        {
            HidePreview();
            e.Handled = true;
        }

        private DropTarget GetDropTarget(TreeListViewItem item, DragEventArgs e)
        {
            var dropTargets = BuildDropTargets(item, e);
            var y = e.GetPosition(item).Y;

            return dropTargets.FirstOrDefault(target => target._y >= y);
        }

        private IEnumerable<DropTarget> BuildDropTargets(TreeListViewItem item, DragEventArgs e)
        {
            var result = new List<DropTarget>();
            var node = item.ListNode;

            if (AllowDropOrder)
                TryAddDropTarget(result, item, DropPlace.Before, e);

            TryAddDropTarget(result, item, DropPlace.Inside, e);

            if (AllowDropOrder)
            {
                if (node.IsExpanded && node.Children.Count > 0)
                {
                    var firstChildItem = ItemContainerGenerator.ContainerFromItem(node.Children[0]) as TreeListViewItem;
                    TryAddDropTarget(result, firstChildItem, DropPlace.Before, e);
                }
                else
                    TryAddDropTarget(result, item, DropPlace.After, e);
            }

            var h = item.ActualHeight;
            var y1 = 0.2 * h;
            var y2 = h / 2;
            var y3 = h - y1;

            if (result.Count == 2)
            {
                if (result[0]._place == DropPlace.Inside &&
                    result[1]._place != DropPlace.Inside)
                    result[0]._y = y3;
                else if (result[0]._place != DropPlace.Inside &&
                         result[1]._place == DropPlace.Inside)
                    result[0]._y = y1;
                else
                    result[0]._y = y2;
            }
            else if (result.Count == 3)
            {
                result[0]._y = y1;
                result[1]._y = y3;
            }
            if (result.Count > 0)
                result[result.Count - 1]._y = h;
            return result;
        }

        private void TryAddDropTarget(List<DropTarget> targets, TreeListViewItem item, DropPlace place, DragEventArgs e)
        {
            TreeListNode listNode;
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

        private void GetNodeAndIndex(TreeListViewItem item, DropPlace place, out TreeListNode listNode, out int index)
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

        private void ShowPreview(TreeListViewItem item, DropPlace place)
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
                    var adorner = new TreeListGeneralAdorner(this);
                    _treeListInsertMarker = new TreeListInsertMarker();
                    adorner.Child = _treeListInsertMarker;
                    adornerLayer.Add(adorner);
                }

                _treeListInsertMarker.Visibility = Visibility.Visible;

                var p1 = _previewListNodeView.TransformToVisual(this).Transform(new Point());
                var p = new Point(p1.X + _previewListNodeView.CalculateIndent() + 4.5, p1.Y - 3);

                if (place == DropPlace.After)
                    p.Y += _previewListNodeView.ActualHeight;

                _treeListInsertMarker.Margin = new Thickness(p.X, p.Y, 0, 0);

                TreeListNodeView secondListNodeView = null;
                var index = _listFlattener.IndexOf(item.ListNode);

                if (place == DropPlace.Before)
                {
                    if (index > 0)
                    {
                        var indexItem = (ItemContainerGenerator.ContainerFromIndex(index - 1) as TreeListViewItem);
                        if (indexItem != null) secondListNodeView = indexItem.ListNodeView;
                    }
                }
                else if (index + 1 < _listFlattener.Count)
                {
                    var indexItem = (ItemContainerGenerator.ContainerFromIndex(index + 1) as TreeListViewItem);
                    if (indexItem != null) secondListNodeView = indexItem.ListNodeView;
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
                _previewListNodeView.ClearValue(TreeListNodeView.TextBackgroundProperty);
                _previewListNodeView.ClearValue(ForegroundProperty);
                if (_treeListInsertMarker != null)
                    _treeListInsertMarker.Visibility = Visibility.Collapsed;
                _previewListNodeView = null;
            }
        }

        private static void RegisterCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(TreeList),
                new CommandBinding(ApplicationCommands.Cut, HandleExecuted_Cut, HandleCanExecute_Cut));

            CommandManager.RegisterClassCommandBinding(typeof(TreeList),
                new CommandBinding(ApplicationCommands.Copy, HandleExecuted_Copy, HandleCanExecute_Copy));

            CommandManager.RegisterClassCommandBinding(typeof(TreeList),
                new CommandBinding(ApplicationCommands.Paste, HandleExecuted_Paste, HandleCanExecute_Paste));

            CommandManager.RegisterClassCommandBinding(typeof(TreeList),
                new CommandBinding(ApplicationCommands.Delete, HandleExecuted_Delete, HandleCanExecute_Delete));
        }

        private static void HandleExecuted_Cut(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            TreeList treeList = (TreeList)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            if (nodes.Length > 0)
                nodes[0].Cut(nodes);
        }

        private static void HandleCanExecute_Cut(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeList treeList = (TreeList)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            e.CanExecute = nodes.Length > 0 && nodes[0].CanCut(nodes);
            e.Handled = true;
        }

        private static void HandleExecuted_Copy(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            TreeList treeList = (TreeList)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            if (nodes.Length > 0)
                nodes[0].Copy(nodes);
        }

        private static void HandleCanExecute_Copy(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeList treeList = (TreeList)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            e.CanExecute = nodes.Length > 0 && nodes[0].CanCopy(nodes);
            e.Handled = true;
        }

        private static void HandleExecuted_Paste(object sender, ExecutedRoutedEventArgs e)
        {
            TreeList treeList = (TreeList)sender;
            var data = Clipboard.GetDataObject();
            if (data != null)
            {
                var selectedNode = (treeList.SelectedItem as TreeListNode) ?? treeList.Root;
                if (selectedNode != null)
                    selectedNode.Paste(data);
            }
            e.Handled = true;
        }

        private static void HandleCanExecute_Paste(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeList treeList = (TreeList)sender;
            var data = Clipboard.GetDataObject();
            if (data == null)
                e.CanExecute = false;
            else
            {
                var selectedNode = (treeList.SelectedItem as TreeListNode) ?? treeList.Root;
                e.CanExecute = selectedNode != null && selectedNode.CanPaste(data);
            }
            e.Handled = true;
        }

        private static void HandleExecuted_Delete(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            TreeList treeList = (TreeList)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            if (nodes.Length > 0)
                nodes[0].Delete(nodes);
        }

        private static void HandleCanExecute_Delete(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeList treeList = (TreeList)sender;
            var nodes = treeList.GetTopLevelSelection().ToArray();
            e.CanExecute = nodes.Length > 0 && nodes[0].CanDelete(nodes);
            e.Handled = true;
        }

        /// <summary>
        /// Gets the selected items which do not have any of their ancestors selected.
        /// </summary>
        public IEnumerable<TreeListNode> GetTopLevelSelection()
        {
            var selection = SelectedItems.OfType<TreeListNode>();
            var treeListNodes = selection as IList<TreeListNode> ?? selection.ToList();
            var selectionHash = new HashSet<TreeListNode>(treeListNodes);

            return treeListNodes.Where(item => item.Ancestors().All(a => !selectionHash.Contains(a)));
        }

        #endregion

        #region Вложенный класс: DropTarget

        private class DropTarget
        {
            #region Поля

            public DragDropEffects _effect;

            public int _index;

            public TreeListViewItem _item;

            public TreeListNode _listNode;

            public DropPlace _place;

            public double _y;

            #endregion
        }

        #endregion
    }

    public class TreeListViewItem : ListViewItem
    {
        #region Поля

        private Point _startPoint;

        private bool _wasDoubleClick;

        private bool _wasSelected;

        #endregion

        #region Свойства

        public TreeListNode ListNode
        {
            get { return DataContext as TreeListNode; }
        }

        public TreeListNodeView ListNodeView { get; internal set; }

        public TreeList ParentTreeList { get; internal set; }

        #endregion

        #region Конструктор

        static TreeListViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListViewItem),
                new FrameworkPropertyMetadata(typeof(TreeListViewItem)));
        }

        #endregion

        #region Методы

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    if (ListNode.IsEditable && ParentTreeList != null && ParentTreeList.SelectedItems.Count == 1 && ParentTreeList.SelectedItems[0] == ListNode)
                    {
                        ListNode.IsEditing = true;
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    if (ListNode.IsEditing)
                    {
                        ListNode.IsEditing = false;
                        e.Handled = true;
                    }
                    break;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _wasSelected = IsSelected;
            if (!IsSelected)
                base.OnMouseLeftButtonDown(e);

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                _startPoint = e.GetPosition(null);
                CaptureMouse();

                if (e.ClickCount == 2)
                    _wasDoubleClick = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var currentPoint = e.GetPosition(null);
                if (Math.Abs(currentPoint.X - _startPoint.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPoint.Y - _startPoint.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    var selection = ParentTreeList.GetTopLevelSelection().ToArray();
                    ListNode.StartDrag(this, selection);
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_wasDoubleClick)
            {
                _wasDoubleClick = false;
                ListNode.ActivateItem(e);
                if (!e.Handled)
                {
                    if (!ListNode.IsRoot || ParentTreeList.ShowRootExpander)
                        ListNode.IsExpanded = !ListNode.IsExpanded;
                }
            }

            ReleaseMouseCapture();
            if (_wasSelected)
                base.OnMouseLeftButtonDown(e);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            ParentTreeList.HandleDragEnter(this, e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            ParentTreeList.HandleDragOver(this, e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            ParentTreeList.HandleDrop(this, e);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            ParentTreeList.HandleDragLeave(this, e);
        }

        #endregion
    }
}