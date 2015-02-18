using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    public abstract class TreeListExNode : INotifyPropertyChanged
    {
        #region Поля

        private bool _canExpandRecursively;

        private byte _height;

        private bool? _isChecked;

        private bool _isEditing;

        private bool _isExpanded;

        private bool _isHidden;

        private bool _isSelected;

        private bool _isVisible;

        private bool _lazyLoading;

        internal TreeListExNode _listParent;

        private TreeListExNodeCollection _modelChildren;

        internal TreeListExNode _modelParent;

        private TreeListExNode _right;

        private TreeListExNode _left;

        private int _totalListLength;

        internal TreeListExFlattener _treeListFlattener;

        /// <summary>
        /// Класс сортировка дерева (если указан)
        /// </summary>
        internal IComparer<TreeListExNode> Sorter
        {
            get
            {
                var listRoot = GetListRoot();
                var sorter = listRoot._treeListFlattener._tree.Sorter;

                return sorter;
            }
        }

        #endregion

        #region Свойства

        private int Balance
        {
            get { return Height(_right) - Height(_left); }
        }

        public TreeListExNodeCollection Children
        {
            get { return _modelChildren ?? (_modelChildren = new TreeListExNodeCollection(this)); }
        }

        public TreeListExNode Parent
        {
            get { return _modelParent; }
        }

        public abstract object Text { get; }

        public virtual Brush Foreground
        {
            get { return SystemColors.WindowTextBrush; }
        }

        public virtual object Icon
        {
            get { return null; }
        }

        public virtual object ToolTip
        {
            get { return null; }
        }

        public int Level
        {
            get { return Parent != null ? Parent.Level + 1 : 0; }
        }

        public bool IsRoot
        {
            get { return Parent == null; }
        }

        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                if (_isHidden != value)
                {
                    _isHidden = value;
                    if (_modelParent != null)
                    {
                        UpdateIsVisible(_modelParent._isVisible && _modelParent._isExpanded, true);
                    }
                    RaisePropertyChanged("IsHidden");
                    if (Parent != null)
                    {
                        Parent.RaisePropertyChanged("ShowExpander");
                    }
                }
            }
        }

        /// <summary>
        /// Return true when this listNode is not hidden and when all parent listNodes are expanded and not hidden.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        public virtual object ExpandedIcon
        {
            get { return Icon; }
        }

        public virtual bool ShowExpander
        {
            get { return LazyLoading || Children.Any(c => !c._isHidden); }
        }

        /// <summary>
        /// Смена состояния "Expanded/Collapsed"
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    if (_isExpanded)
                    {
                        // Разворачинвание родительского узла (если он свернут)
                        ExpandParent(this);
                        // 
                        EnsureLazyChildren();
                        OnExpanding();
                    }
                    else
                    {
                        OnCollapsing();
                    }
                    UpdateChildIsVisible(true);
                    RaisePropertyChanged("IsExpanded");
                }
            }
        }

        /// <summary>
        /// Состоянеие "Collapsed"
        /// </summary>
        public bool IsCollapsed
        {
            get { return IsExpanded == false; }
        }

        public bool LazyLoading
        {
            get { return _lazyLoading; }
            set
            {
                _lazyLoading = value;
                if (_lazyLoading)
                {
                    IsExpanded = false;
                    if (_canExpandRecursively)
                    {
                        _canExpandRecursively = false;
                        RaisePropertyChanged("CanExpandRecursively");
                    }
                }
                RaisePropertyChanged("LazyLoading");
                RaisePropertyChanged("ShowExpander");
            }
        }

        public virtual bool CanExpandRecursively
        {
            get { return _canExpandRecursively; }
        }

        public virtual bool ShowIcon
        {
            get { return Icon != null; }
        }

        public virtual bool IsEditable
        {
            get { return false; }
        }

        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    RaisePropertyChanged("IsEditing");
                }
            }
        }

        public virtual bool IsCheckable
        {
            get { return false; }
        }

        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true); }
        }

        public virtual bool IsCut
        {
            get { return false; }
        }

        public bool IsLast
        {
            get
            {
                return Parent == null || Parent.Children[Parent.Children.Count - 1] == this;
            }
        }

        public object Model
        {
            get { return GetModel(); }
        }

        #endregion

        #region События

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Конструктор

        protected TreeListExNode()
        {
            _totalListLength = -1;
            _isVisible = true;
            _height = 1;
            _canExpandRecursively = true;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Рекурсивное разворачивание родительских узлов
        /// </summary>
        private void ExpandParent(TreeListExNode node)
        {
            if (node != null)
            {
                var parent = node.Parent;
                if (parent != null && parent.IsExpanded == false)
                {
                    node.Parent.IsExpanded = true;
                }
            }
        }

        /// <summary>
        /// "Высота элемента" (бинарное дерево)
        /// </summary>
        private static int Height(TreeListExNode listNode)
        {
            return listNode != null ? listNode._height : 0;
        }

        internal TreeListExNode GetListRoot()
        {
            TreeListExNode listNode = this;
            while (listNode._listParent != null)
            {
                listNode = listNode._listParent;
            }
            return listNode;
        }

        /// <summary>
        /// Чтение корневого элемента всего дерева
        /// </summary>
        internal TreeListExNode GetRoot()
        {
            var item = this;

            while (item.Parent != null)
            {
                item = item.Parent;
            }

            return item;
        }

        private void UpdateIsVisible(bool parentIsVisible, bool updateFlattener)
        {
            bool newIsVisible = parentIsVisible && !_isHidden;
            if (_isVisible != newIsVisible)
            {
                _isVisible = newIsVisible;

                // invalidate the augmented data
                TreeListExNode listNode = this;
                while (listNode != null && listNode._totalListLength >= 0)
                {
                    listNode._totalListLength = -1;
                    listNode = listNode._listParent;
                }
                // Remember the removed listNodes:
                List<TreeListExNode> removedNodes = null;
                if (updateFlattener && !newIsVisible)
                {
                    removedNodes = VisibleDescendantsAndSelf().ToList();
                }
                // also update the model children:
                UpdateChildIsVisible(false);

                // Validate our invariants:
                if (updateFlattener)
                {
                    CheckRootInvariants();
                }

                // Tell the flattener about the removed listNodes:
                if (removedNodes != null)
                {
                    var flattener = GetListRoot()._treeListFlattener;
                    if (flattener != null)
                    {
                        flattener.NodesRemoved(GetVisibleIndexForNode(this), removedNodes);
                        foreach (var n in removedNodes)
                        {
                            n.OnIsVisibleChanged();
                        }
                    }
                }
                // Tell the flattener about the new listNodes:
                if (updateFlattener && newIsVisible)
                {
                    var flattener = GetListRoot()._treeListFlattener;
                    if (flattener != null)
                    {
                        flattener.NodesInserted(GetVisibleIndexForNode(this), VisibleDescendantsAndSelf());
                        foreach (var n in VisibleDescendantsAndSelf())
                        {
                            n.OnIsVisibleChanged();
                        }
                    }
                }
            }
        }

        protected virtual void OnIsVisibleChanged()
        {
        }

        private void UpdateChildIsVisible(bool updateFlattener)
        {
            if (_modelChildren != null && _modelChildren.Count > 0)
            {
                bool showChildren = _isVisible && _isExpanded;
                foreach (TreeListExNode child in _modelChildren)
                {
                    child.UpdateIsVisible(showChildren, updateFlattener);
                }
            }
        }

        public virtual void ActivateItem(RoutedEventArgs e)
        {
        }

        public override string ToString()
        {
            object text = Text;
            return text != null ? text.ToString() : string.Empty;
        }

        protected internal virtual void OnChildrenChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (TreeListExNode node in e.OldItems)
                {
                    Debug.Assert(node._modelParent == this);
                    node._modelParent = null;
                    TreeListExNode removeEnd = node;
                    while (removeEnd._modelChildren != null && removeEnd._modelChildren.Count > 0)
                    {
                        removeEnd = removeEnd._modelChildren.Last();
                    }

                    List<TreeListExNode> removedNodes = null;
                    int visibleIndexOfRemoval = 0;
                    if (node._isVisible)
                    {
                        visibleIndexOfRemoval = GetVisibleIndexForNode(node);
                        removedNodes = node.VisibleDescendantsAndSelf().ToList();
                    }

                    RemoveNodes(node, removeEnd);

                    if (removedNodes != null)
                    {
                        var flattener = GetListRoot()._treeListFlattener;
                        if (flattener != null)
                        {
                            flattener.NodesRemoved(visibleIndexOfRemoval, removedNodes);
                        }
                    }
                }
            }
            if (e.NewItems != null)
            {
                TreeListExNode insertionPos = e.NewStartingIndex == 0 ? null : _modelChildren[e.NewStartingIndex - 1];

                foreach (TreeListExNode node in e.NewItems)
                {
                    Debug.Assert(node._modelParent == null);
                    node._modelParent = this;
                    node.UpdateIsVisible(_isVisible && _isExpanded, false);

                    while (insertionPos != null && insertionPos._modelChildren != null && insertionPos._modelChildren.Count > 0)
                    {
                        insertionPos = insertionPos._modelChildren.Last();
                    }
                    InsertNodeAfter(insertionPos ?? this, node);

                    insertionPos = node;
                    if (node._isVisible)
                    {
                        var flattener = GetListRoot()._treeListFlattener;
                        if (flattener != null)
                        {
                            flattener.NodesInserted(GetVisibleIndexForNode(node), node.VisibleDescendantsAndSelf());
                        }
                    }
                }
            }

            RaisePropertyChanged("ShowExpander");
            RaiseIsLastChangedIfNeeded(e);
        }

        protected virtual void OnExpanding()
        {
        }

        protected virtual void OnCollapsing()
        {
        }

        protected virtual void LoadChildren()
        {
            throw new NotSupportedException(GetType().Name + " does not support lazy loading");
        }

        public void EnsureLazyChildren()
        {
            if (LazyLoading)
            {
                LazyLoading = false;
                LoadChildren();
            }
        }

        public IEnumerable<TreeListExNode> Descendants()
        {
            return TreeListExTraversal.PreOrder(Children, n => n.Children);
        }

        public IEnumerable<TreeListExNode> DescendantsAndSelf()
        {
            return TreeListExTraversal.PreOrder(this, n => n.Children);
        }

        internal IEnumerable<TreeListExNode> VisibleDescendants()
        {
            return TreeListExTraversal.PreOrder(Children.Where(c => c._isVisible), n => n.Children.Where(c => c._isVisible));
        }

        internal IEnumerable<TreeListExNode> VisibleDescendantsAndSelf()
        {
            return TreeListExTraversal.PreOrder(this, n => n.Children.Where(c => c._isVisible));
        }

        public IEnumerable<TreeListExNode> Ancestors()
        {
            for (TreeListExNode n = Parent; n != null; n = n.Parent)
            {
                yield return n;
            }
        }

        public IEnumerable<TreeListExNode> AncestorsAndSelf()
        {
            for (TreeListExNode n = this; n != null; n = n.Parent)
            {
                yield return n;
            }
        }

        public virtual string LoadEditText()
        {
            return null;
        }

        public virtual bool SaveEditText(string value)
        {
            return true;
        }

        private void SetIsChecked(bool? value, bool update)
        {
            if (_isChecked != value)
            {
                _isChecked = value;

                if (update)
                {
                    if (IsChecked != null)
                    {
                        foreach (var child in Descendants())
                        {
                            if (child.IsCheckable)
                            {
                                child.SetIsChecked(IsChecked, false);
                            }
                        }
                    }

                    foreach (var parent in Ancestors())
                    {
                        if (parent.IsCheckable)
                        {
                            if (!parent.TryValueForIsChecked(true))
                            {
                                if (!parent.TryValueForIsChecked(false))
                                {
                                    parent.SetIsChecked(null, false);
                                }
                            }
                        }
                    }
                }

                RaisePropertyChanged("IsChecked");
            }
        }

        private bool TryValueForIsChecked(bool? value)
        {
            if (Children.Where(n => n.IsCheckable).All(n => n.IsChecked == value))
            {
                SetIsChecked(value, false);
                return true;
            }
            return false;
        }

        public virtual bool CanDelete(TreeListExNode[] listNodes)
        {
            return false;
        }

        public virtual void DeleteWithoutConfirmation(TreeListExNode[] listNodes)
        {
            throw new NotSupportedException(GetType().Name + " does not support deletion");
        }

        public virtual bool CanCut(TreeListExNode[] listNodes)
        {
            return CanCopy(listNodes) && CanDelete(listNodes);
        }

        public virtual void Cut(TreeListExNode[] listNodes)
        {
            var data = GetDataObject(listNodes);
            if (data != null)
            {
                Clipboard.SetDataObject(data, true);
                DeleteWithoutConfirmation(listNodes);
            }
        }

        public virtual bool CanCopy(TreeListExNode[] listNodes)
        {
            return false;
        }

        public virtual void Copy(TreeListExNode[] listNodes)
        {
            var data = GetDataObject(listNodes);
            if (data != null)
            {
                Clipboard.SetDataObject(data, true);
            }
        }

        protected virtual IDataObject GetDataObject(TreeListExNode[] listNodes)
        {
            return null;
        }

        public virtual bool CanPaste(IDataObject data)
        {
            return false;
        }

        public virtual void Paste(IDataObject data)
        {
            throw new NotSupportedException(GetType().Name + " does not support copy/paste");
        }

        public virtual void StartDrag(DependencyObject dragSource, TreeListExNode[] listNodes)
        {
            // The default drag implementation works by reusing the copy infrastructure.
            // Derived classes should override this method
            var data = GetDataObject(listNodes);
            if (data == null)
            {
                return;
            }
            DragDropEffects effects = DragDropEffects.Copy;
            if (CanDelete(listNodes))
            {
                effects |= DragDropEffects.Move;
            }
            DragDropEffects result = DragDrop.DoDragDrop(dragSource, data, effects);
            if (result == DragDropEffects.Move)
            {
                DeleteWithoutConfirmation(listNodes);
            }
        }

        public virtual DragDropEffects GetDropEffect(DragEventArgs e, int index)
        {
            // Since the default drag implementation uses Copy(),
            // we'll use Paste() in our default drop implementation.
            if (CanPaste(e.Data))
            {
                // If Ctrl is pressed -> copy
                // If moving is not allowed -> copy
                // Otherwise: move
                if ((e.KeyStates & DragDropKeyStates.ControlKey) != 0 || (e.AllowedEffects & DragDropEffects.Move) == 0)
                {
                    return DragDropEffects.Copy;
                }
                return DragDropEffects.Move;
            }
            return DragDropEffects.None;
        }

        internal void InternalDrop(DragEventArgs e, int index)
        {
            if (LazyLoading)
            {
                EnsureLazyChildren();
                index = Children.Count;
            }

            Drop(e, index);
        }

        public virtual void Drop(DragEventArgs e, int index)
        {
            // Since the default drag implementation uses Copy(),
            // we'll use Paste() in our default drop implementation.
            Paste(e.Data);
        }

        private void RaiseIsLastChangedIfNeeded(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex == Children.Count - 1)
                    {
                        if (Children.Count > 1)
                        {
                            Children[Children.Count - 2].RaisePropertyChanged("IsLast");
                        }
                        Children[Children.Count - 1].RaisePropertyChanged("IsLast");
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex == Children.Count)
                    {
                        if (Children.Count > 0)
                        {
                            Children[Children.Count - 1].RaisePropertyChanged("IsLast");
                        }
                    }
                    break;
            }
        }

        public void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        protected virtual object GetModel()
        {
            return null;
        }

        [Conditional("DEBUG")]
        private void CheckRootInvariants()
        {
            GetListRoot().CheckInvariants();
        }

        [Conditional("DATACONSISTENCYCHECK")]
        private void CheckInvariants()
        {
            Debug.Assert(_left == null || _left._listParent == this);
            Debug.Assert(_right == null || _right._listParent == this);
            Debug.Assert(_height == 1 + Math.Max(Height(_left), Height(_right)));
            Debug.Assert(Math.Abs(Balance) <= 1);
            Debug.Assert(_totalListLength == -1 || _totalListLength == (_left != null ? _left._totalListLength : 0) + (_isVisible ? 1 : 0) + (_right != null ? _right._totalListLength : 0));
            if (_left != null)
            {
                _left.CheckInvariants();
            }
            if (_right != null)
            {
                _right.CheckInvariants();
            }
        }

        [Conditional("DEBUG")]
        // ReSharper disable UnusedMember.Local
        private static void DumpTree(TreeListExNode listNode)
            // ReSharper restore UnusedMember.Local
        {
            listNode.GetListRoot().DumpTree();
        }

        [Conditional("DEBUG")]
        private void DumpTree()
        {
            Debug.Indent();
            if (_left != null)
            {
                _left.DumpTree();
            }
            Debug.Unindent();
            Debug.WriteLine("{0}, totalListLength={1}, height={2}, Balance={3}, isVisible={4}", ToString(), _totalListLength, _height, Balance, _isVisible);
            Debug.Indent();
            if (_right != null)
            {
                _right.DumpTree();
            }
            Debug.Unindent();
        }

        internal static TreeListExNode GetNodeByVisibleIndex(TreeListExNode root, int index)
        {
            root.GetTotalListLength(); // ensure all list lengths are calculated
            Debug.Assert(index >= 0);
            Debug.Assert(index < root._totalListLength);
            TreeListExNode listNode = root;
            while (true)
            {
                if (listNode._left != null && index < listNode._left._totalListLength)
                {
                    listNode = listNode._left;
                }
                else
                {
                    if (listNode._left != null)
                    {
                        index -= listNode._left._totalListLength;
                    }
                    if (listNode._isVisible)
                    {
                        if (index == 0)
                        {
                            return listNode;
                        }
                        index--;
                    }
                    listNode = listNode._right;
                }
            }
        }

        internal static int GetVisibleIndexForNode(TreeListExNode listNode)
        {
            int index = listNode._left != null ? listNode._left.GetTotalListLength() : 0;
            while (listNode._listParent != null)
            {
                if (listNode == listNode._listParent._right)
                {
                    if (listNode._listParent._left != null)
                    {
                        index += listNode._listParent._left.GetTotalListLength();
                    }
                    if (listNode._listParent._isVisible)
                    {
                        index++;
                    }
                }
                listNode = listNode._listParent;
            }
            return index;
        }

        /// <summary>
        /// Баланс поддерева и пересчет "высоты"
        /// This method assumes that the children of this listNode are already balanced and have an up-to-date 'height' value.
        /// </summary>
        /// <returns>The new root listNode</returns>
        private static TreeListExNode Rebalance(TreeListExNode listNode)
        {
            Debug.Assert(listNode._left == null || Math.Abs(listNode._left.Balance) <= 1);
            Debug.Assert(listNode._right == null || Math.Abs(listNode._right.Balance) <= 1);
            // Keep looping until it's balanced. Not sure if this is stricly required; this is based on
            // the Rope code where listNode merging made this necessary.
            while (Math.Abs(listNode.Balance) > 1)
            {
                // AVL balancing
                // note: because we don't care about the identity of concat listNodes, this works a little different than usual
                // tree rotations: in our implementation, the "this" listNode will stay at the top, only its children are rearranged
                if (listNode.Balance > 1)
                {
                    if (listNode._right != null && listNode._right.Balance < 0)
                    {
                        listNode._right = listNode._right.RotateRight();
                    }
                    listNode = listNode.RotateLeft();
                    // If 'listNode' was unbalanced by more than 2, we've shifted some of the inbalance to the left listNode; so rebalance that.
                    listNode._left = Rebalance(listNode._left);
                }
                else if (listNode.Balance < -1)
                {
                    if (listNode._left != null && listNode._left.Balance > 0)
                    {
                        listNode._left = listNode._left.RotateLeft();
                    }
                    listNode = listNode.RotateRight();
                    // If 'listNode' was unbalanced by more than 2, we've shifted some of the inbalance to the right listNode; so rebalance that.
                    listNode._right = Rebalance(listNode._right);
                }
            }
            Debug.Assert(Math.Abs(listNode.Balance) <= 1);
            listNode._height = (byte)(1 + Math.Max(Height(listNode._left), Height(listNode._right)));
            listNode._totalListLength = -1; // mark for recalculation
            // since balancing checks the whole tree up to the root, the whole path will get marked as invalid
            return listNode;
        }

        internal int GetTotalListLength()
        {
            if (_totalListLength >= 0)
            {
                return _totalListLength;
            }
            int length = (_isVisible ? 1 : 0);
            if (_left != null)
            {
                length += _left.GetTotalListLength();
            }
            if (_right != null)
            {
                length += _right.GetTotalListLength();
            }
            return _totalListLength = length;
        }

        private TreeListExNode RotateLeft()
        {
            /* Rotate tree to the left
			 * 
			 *       this               right
			 *       /  \               /  \
			 *      A   right   ===>  this  C
			 *           / \          / \
			 *          B   C        A   B
			 */
            TreeListExNode b = _right._left;
            TreeListExNode newTop = _right;

            if (b != null)
            {
                b._listParent = this;
            }
            _right = b;
            newTop._left = this;
            newTop._listParent = _listParent;
            _listParent = newTop;
            // rebalance the 'this' listNode - this is necessary in some bulk insertion cases:
            newTop._left = Rebalance(this);
            return newTop;
        }

        private TreeListExNode RotateRight()
        {
            /* Rotate tree to the right
			 * 
			 *       this             left
			 *       /  \             /  \
			 *     left  C   ===>    A   this
			 *     / \                   /  \
			 *    A   B                 B    C
			 */
            TreeListExNode b = _left._right;
            TreeListExNode newTop = _left;

            if (b != null)
            {
                b._listParent = this;
            }
            _left = b;
            newTop._right = this;
            newTop._listParent = _listParent;
            _listParent = newTop;
            newTop._right = Rebalance(this);
            return newTop;
        }

        private static void RebalanceUntilRoot(TreeListExNode node)
        {
            while (node._listParent != null)
            {
                if (node == node._listParent._left)
                {
                    node = node._listParent._left = Rebalance(node);
                }
                else
                {
                    Debug.Assert(node == node._listParent._right);
                    node = node._listParent._right = Rebalance(node);
                }
                node = node._listParent;
            }
            TreeListExNode newRoot = Rebalance(node);
            if (newRoot != node && node._treeListFlattener != null)
            {
                Debug.Assert(newRoot._treeListFlattener == null);
                newRoot._treeListFlattener = node._treeListFlattener;
                node._treeListFlattener = null;
                newRoot._treeListFlattener._root = newRoot;
            }
            Debug.Assert(newRoot._listParent == null);
            newRoot.CheckInvariants();
        }

        private static void InsertNodeAfter(TreeListExNode pos, TreeListExNode newListNode)
        {
            // newListNode might be the model root of a whole subtree, so go to the list root of that subtree:
            newListNode = newListNode.GetListRoot();
            if (pos._right == null)
            {
                pos._right = newListNode;
                newListNode._listParent = pos;
            }
            else
            {
                // insert before pos.right's leftmost:
                pos = pos._right;
                while (pos._left != null)
                {
                    pos = pos._left;
                }
                Debug.Assert(pos._left == null);
                pos._left = newListNode;
                newListNode._listParent = pos;
            }
            RebalanceUntilRoot(pos);
        }

        private void RemoveNodes(TreeListExNode start, TreeListExNode end)
        {
            // Removes all listNodes from start to end (inclusive)
            // All removed listNodes will be reorganized in a separate tree, do not delete
            // regions that don't belong together in the tree model!

            List<TreeListExNode> removedSubtrees = new List<TreeListExNode>();
            TreeListExNode oldPos;
            TreeListExNode pos = start;
            do
            {
                // recalculate the endAncestors every time, because the tree might have been rebalanced
                HashSet<TreeListExNode> endAncestors = new HashSet<TreeListExNode>();
                for (TreeListExNode tmp = end; tmp != null; tmp = tmp._listParent)
                {
                    endAncestors.Add(tmp);
                }

                removedSubtrees.Add(pos);
                if (!endAncestors.Contains(pos))
                {
                    // we can remove pos' right subtree in a single step:
                    if (pos._right != null)
                    {
                        removedSubtrees.Add(pos._right);
                        pos._right._listParent = null;
                        pos._right = null;
                    }
                }
                TreeListExNode succ = pos.Successor();
                DeleteNodeInternal(pos); // this will also rebalance out the deletion of the right subtree

                oldPos = pos;
                pos = succ;
            } while (oldPos != end);

            // merge back together the removed subtrees:
            TreeListExNode removed = removedSubtrees[0];
            for (int i = 1; i < removedSubtrees.Count; i++)
            {
                removed = ConcatTrees(removed, removedSubtrees[i]);
            }
        }

        private static TreeListExNode ConcatTrees(TreeListExNode first, TreeListExNode second)
        {
            TreeListExNode tmp = first;
            while (tmp._right != null)
            {
                tmp = tmp._right;
            }
            InsertNodeAfter(tmp, second);
            return tmp.GetListRoot();
        }

        private TreeListExNode Successor()
        {
            if (_right != null)
            {
                TreeListExNode listNode = _right;
                while (listNode._left != null)
                {
                    listNode = listNode._left;
                }
                return listNode;
            }
            else
            {
                TreeListExNode listNode = this;
                TreeListExNode oldListNode;
                do
                {
                    oldListNode = listNode;
                    listNode = listNode._listParent;
                    // loop while we are on the way up from the right part
                } while (listNode != null && listNode._right == oldListNode);
                return listNode;
            }
        }

        private void DeleteNodeInternal(TreeListExNode listNode)
        {
            TreeListExNode balancingListNode;
            if (listNode._left == null)
            {
                balancingListNode = listNode._listParent;
                listNode.ReplaceWith(listNode._right);
                listNode._right = null;
            }
            else if (listNode._right == null)
            {
                balancingListNode = listNode._listParent;
                listNode.ReplaceWith(listNode._left);
                listNode._left = null;
            }
            else
            {
                TreeListExNode tmp = listNode._right;
                while (tmp._left != null)
                {
                    tmp = tmp._left;
                }
                // First replace tmp with tmp.right
                balancingListNode = tmp._listParent;
                tmp.ReplaceWith(tmp._right);
                tmp._right = null;
                Debug.Assert(tmp._left == null);
                Debug.Assert(tmp._listParent == null);
                // Now move listNode's children to tmp:
                tmp._left = listNode._left;
                listNode._left = null;
                tmp._right = listNode._right;
                listNode._right = null;
                if (tmp._left != null)
                {
                    tmp._left._listParent = tmp;
                }
                if (tmp._right != null)
                {
                    tmp._right._listParent = tmp;
                }
                // Then replace listNode with tmp
                listNode.ReplaceWith(tmp);
                if (balancingListNode == listNode)
                {
                    balancingListNode = tmp;
                }
            }
            Debug.Assert(listNode._listParent == null);
            Debug.Assert(listNode._left == null);
            Debug.Assert(listNode._right == null);
            listNode._height = 1;
            listNode._totalListLength = -1;
            if (balancingListNode != null)
            {
                RebalanceUntilRoot(balancingListNode);
            }
        }

        private void ReplaceWith(TreeListExNode listNode)
        {
            if (_listParent != null)
            {
                if (_listParent._left == this)
                {
                    _listParent._left = listNode;
                }
                else
                {
                    Debug.Assert(_listParent._right == this);
                    _listParent._right = listNode;
                }
                if (listNode != null)
                {
                    listNode._listParent = _listParent;
                }
                _listParent = null;
            }
            else
            {
                // this was a root listNode
                Debug.Assert(listNode != null); // cannot delete the only listNode in the tree
                listNode._listParent = null;
                if (_treeListFlattener != null)
                {
                    Debug.Assert(listNode._treeListFlattener == null);
                    listNode._treeListFlattener = _treeListFlattener;
                    _treeListFlattener = null;
                    listNode._treeListFlattener._root = listNode;
                }
            }
        }

        /// <summary>
        /// Развернуть все (рекурсивно)
        /// </summary>
        private void ExpandRecursively(TreeListExNode listNode)
        {
            if (listNode.CanExpandRecursively)
            {
                listNode.IsExpanded = true;
                foreach (var child in listNode.Children)
                {
                    ExpandRecursively(child);
                }
            }
        }

        /// <summary>
        /// Свернуть все (рекурсивно)
        /// </summary>
        private void CollapseRecursively(TreeListExNode listNode)
        {
            if (listNode.CanExpandRecursively)
            {
                listNode.IsExpanded = false;
                foreach (var child in listNode.Children)
                {
                    CollapseRecursively(child);
                }
            }
        }

        /// <summary>
        /// Разввернуть узел
        /// </summary>
        public void Expand()
        {
            IsExpanded = true;
        }

        /// <summary>
        /// Свернуть узел
        /// </summary>
        public void Collapse()
        {
            IsExpanded = false;
        }

        /// <summary>
        /// Свернуть все элементы
        /// </summary>
        public void ExpandAll()
        {
            ExpandRecursively(this);
        }

        /// <summary>
        /// Развернуть все элементы
        /// </summary>
        public void CollapseAll()
        {
            CollapseRecursively(this);
        }

        /// <summary>
        /// Добавление элемента
        /// </summary>
        public void AddChild(TreeListExNode node)
        {
            var sorter = Sorter;
            if (sorter != null)
            {
                var tempList = new List<TreeListExNode>(Children);
                var index = tempList.BinarySearch(node, sorter);
                if (index < 0) index = ~index;
                Children.Insert(index, node);
            }
            else
            {
                Children.Add(node);    
            }
        }

        /// <summary>
        /// Удаление дочернего элемента
        /// </summary>
        public void DeleteChild(TreeListExNode node)
        {
            Children.Remove(node);
        }

        /// <summary>
        /// Удаление дочерних элементов 
        /// </summary>
        public void DeleteChilds(IEnumerable<TreeListExNode> nodes)
        {
            foreach (var node in nodes)
            {
                Children.Remove(node);
            }
        }

        /// <summary>
        /// Удаление указанного элемента из дерева
        /// </summary>
        public void DeleteNode(TreeListExNode node)
        {
            var parernt = node.Parent;

            if (parernt != null)
            {
                parernt.DeleteChild(node);
            }
        }

        /// <summary>
        /// Удаление указанных элементов из дерева
        /// </summary>
        public void DeleteNodes(IEnumerable<TreeListExNode> nodes)
        {
            foreach (var node in nodes)
            {
                DeleteNode(node);
            }
        }

        /// <summary>
        /// Сортировка элеметов
        /// </summary>
        public void Sort()
        {
            var sorter = Sorter;
            if (sorter != null)
            {
                var tempList = new List<TreeListExNode>(Children);
                tempList.Sort(sorter);

                Children.RemoveRange(0, Children.Count);
                Children.AddRange(tempList);
            }
        }

        /// <summary>
        /// Вызывается для обновление текстового представления
        /// </summary>
        public void UpdateText()
        {
            RaisePropertyChanged("Text");
        }

        #endregion
    }
}