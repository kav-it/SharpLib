// ****************************************************************************
//
// Имя файла    : 'TreeList.Node.cs'
// Заголовок    : Узел компонента TreeList
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 11/03/2014
//
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SharpLib.Controls
{
    public class TreeListNode : INotifyPropertyChanged
    {
        #region Поля

        private bool _canExpandRecursively = true;

        private byte _height = 1;

        private bool? _isChecked;

        private bool _isEditing;

        private bool _isExpanded;

        private bool _isHidden;

        private bool _isSelected;

        private bool _isVisible = true;

        private bool _lazyLoading;

        private TreeListNode _left;

        internal TreeListNode _listParent;

        private TreeListNodeCollection _modelChildren;

        internal TreeListNode _modelParent;

        private TreeListNode _right;

        private int _totalListLength = -1;

        internal TreeListFlattener _treeListFlattener;

        #endregion

        #region Свойства

        private int Balance
        {
            get { return Height(_right) - Height(_left); }
        }

        public TreeListNodeCollection Children
        {
            get { return _modelChildren ?? (_modelChildren = new TreeListNodeCollection(this)); }
        }

        public TreeListNode Parent
        {
            get { return _modelParent; }
        }

        public virtual object Text
        {
            get { return null; }
        }

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
                        UpdateIsVisible(_modelParent._isVisible && _modelParent._isExpanded, true);
                    RaisePropertyChanged("IsHidden");
                    if (Parent != null)
                        Parent.RaisePropertyChanged("ShowExpander");
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
                        EnsureLazyChildren();
                        OnExpanding();
                    }
                    else
                        OnCollapsing();
                    UpdateChildIsVisible(true);
                    RaisePropertyChanged("IsExpanded");
                }
            }
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
                return Parent == null ||
                       Parent.Children[Parent.Children.Count - 1] == this;
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

        #region Методы

        private static int Height(TreeListNode listNode)
        {
            return listNode != null ? listNode._height : 0;
        }

        internal TreeListNode GetListRoot()
        {
            TreeListNode listNode = this;
            while (listNode._listParent != null)
                listNode = listNode._listParent;
            return listNode;
        }

        private void UpdateIsVisible(bool parentIsVisible, bool updateFlattener)
        {
            bool newIsVisible = parentIsVisible && !_isHidden;
            if (_isVisible != newIsVisible)
            {
                _isVisible = newIsVisible;

                // invalidate the augmented data
                TreeListNode listNode = this;
                while (listNode != null && listNode._totalListLength >= 0)
                {
                    listNode._totalListLength = -1;
                    listNode = listNode._listParent;
                }
                // Remember the removed listNodes:
                List<TreeListNode> removedNodes = null;
                if (updateFlattener && !newIsVisible)
                    removedNodes = VisibleDescendantsAndSelf().ToList();
                // also update the model children:
                UpdateChildIsVisible(false);

                // Validate our invariants:
                if (updateFlattener)
                    CheckRootInvariants();

                // Tell the flattener about the removed listNodes:
                if (removedNodes != null)
                {
                    var flattener = GetListRoot()._treeListFlattener;
                    if (flattener != null)
                    {
                        flattener.NodesRemoved(GetVisibleIndexForNode(this), removedNodes);
                        foreach (var n in removedNodes)
                            n.OnIsVisibleChanged();
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
                            n.OnIsVisibleChanged();
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
                foreach (TreeListNode child in _modelChildren)
                    child.UpdateIsVisible(showChildren, updateFlattener);
            }
        }

        public virtual void ActivateItem(RoutedEventArgs e)
        {
        }

        public override string ToString()
        {
            // used for keyboard navigation
            object text = Text;
            return text != null ? text.ToString() : string.Empty;
        }

        protected internal virtual void OnChildrenChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (TreeListNode node in e.OldItems)
                {
                    Debug.Assert(node._modelParent == this);
                    node._modelParent = null;
                    Debug.WriteLine(String.Format("Removing {0} from {1}", node, this));
                    TreeListNode removeEnd = node;
                    while (removeEnd._modelChildren != null && removeEnd._modelChildren.Count > 0)
                        removeEnd = removeEnd._modelChildren.Last();

                    List<TreeListNode> removedNodes = null;
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
                            flattener.NodesRemoved(visibleIndexOfRemoval, removedNodes);
                    }
                }
            }
            if (e.NewItems != null)
            {
                TreeListNode insertionPos = e.NewStartingIndex == 0 ? null : _modelChildren[e.NewStartingIndex - 1];

                foreach (TreeListNode node in e.NewItems)
                {
                    Debug.Assert(node._modelParent == null);
                    node._modelParent = this;
                    node.UpdateIsVisible(_isVisible && _isExpanded, false);
                    //Debug.WriteLine("Inserting {0} after {1}", listNode, insertionPos);

                    while (insertionPos != null && insertionPos._modelChildren != null && insertionPos._modelChildren.Count > 0)
                        insertionPos = insertionPos._modelChildren.Last();
                    InsertNodeAfter(insertionPos ?? this, node);

                    insertionPos = node;
                    if (node._isVisible)
                    {
                        var flattener = GetListRoot()._treeListFlattener;
                        if (flattener != null)
                            flattener.NodesInserted(GetVisibleIndexForNode(node), node.VisibleDescendantsAndSelf());
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

        public IEnumerable<TreeListNode> Descendants()
        {
            return TreeListTraversal.PreOrder(Children, n => n.Children);
        }

        public IEnumerable<TreeListNode> DescendantsAndSelf()
        {
            return TreeListTraversal.PreOrder(this, n => n.Children);
        }

        internal IEnumerable<TreeListNode> VisibleDescendants()
        {
            return TreeListTraversal.PreOrder(Children.Where(c => c._isVisible), n => n.Children.Where(c => c._isVisible));
        }

        internal IEnumerable<TreeListNode> VisibleDescendantsAndSelf()
        {
            return TreeListTraversal.PreOrder(this, n => n.Children.Where(c => c._isVisible));
        }

        public IEnumerable<TreeListNode> Ancestors()
        {
            for (TreeListNode n = Parent; n != null; n = n.Parent)
                yield return n;
        }

        public IEnumerable<TreeListNode> AncestorsAndSelf()
        {
            for (TreeListNode n = this; n != null; n = n.Parent)
                yield return n;
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
                                child.SetIsChecked(IsChecked, false);
                        }
                    }

                    foreach (var parent in Ancestors())
                    {
                        if (parent.IsCheckable)
                        {
                            if (!parent.TryValueForIsChecked(true))
                            {
                                if (!parent.TryValueForIsChecked(false))
                                    parent.SetIsChecked(null, false);
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

        public virtual bool CanDelete(TreeListNode[] listNodes)
        {
            return false;
        }

        public virtual void Delete(TreeListNode[] listNodes)
        {
            throw new NotSupportedException(GetType().Name + " does not support deletion");
        }

        public virtual void DeleteWithoutConfirmation(TreeListNode[] listNodes)
        {
            throw new NotSupportedException(GetType().Name + " does not support deletion");
        }

        public virtual bool CanCut(TreeListNode[] listNodes)
        {
            return CanCopy(listNodes) && CanDelete(listNodes);
        }

        public virtual void Cut(TreeListNode[] listNodes)
        {
            var data = GetDataObject(listNodes);
            if (data != null)
            {
                Clipboard.SetDataObject(data, true);
                DeleteWithoutConfirmation(listNodes);
            }
        }

        public virtual bool CanCopy(TreeListNode[] listNodes)
        {
            return false;
        }

        public virtual void Copy(TreeListNode[] listNodes)
        {
            var data = GetDataObject(listNodes);
            if (data != null)
                Clipboard.SetDataObject(data, copy: true);
        }

        protected virtual IDataObject GetDataObject(TreeListNode[] listNodes)
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

        public virtual void StartDrag(DependencyObject dragSource, TreeListNode[] listNodes)
        {
            // The default drag implementation works by reusing the copy infrastructure.
            // Derived classes should override this method
            var data = GetDataObject(listNodes);
            if (data == null)
                return;
            DragDropEffects effects = DragDropEffects.Copy;
            if (CanDelete(listNodes))
                effects |= DragDropEffects.Move;
            DragDropEffects result = DragDrop.DoDragDrop(dragSource, data, effects);
            if (result == DragDropEffects.Move)
                DeleteWithoutConfirmation(listNodes);
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
                    return DragDropEffects.Copy;
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
                            Children[Children.Count - 2].RaisePropertyChanged("IsLast");
                        Children[Children.Count - 1].RaisePropertyChanged("IsLast");
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex == Children.Count)
                    {
                        if (Children.Count > 0)
                            Children[Children.Count - 1].RaisePropertyChanged("IsLast");
                    }
                    break;
            }
        }

        public void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
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
            if (_left != null) _left.CheckInvariants();
            if (_right != null) _right.CheckInvariants();
        }

        [Conditional("DEBUG")]
        // ReSharper disable UnusedMember.Local
        private static void DumpTree(TreeListNode listNode)
            // ReSharper restore UnusedMember.Local
        {
            listNode.GetListRoot().DumpTree();
        }

        [Conditional("DEBUG")]
        private void DumpTree()
        {
            Debug.Indent();
            if (_left != null)
                _left.DumpTree();
            Debug.Unindent();
            Debug.WriteLine(String.Format("{0}, totalListLength={1}, height={2}, Balance={3}, isVisible={4}", ToString(), _totalListLength, _height, Balance, _isVisible));
            Debug.Indent();
            if (_right != null)
                _right.DumpTree();
            Debug.Unindent();
        }

        internal static TreeListNode GetNodeByVisibleIndex(TreeListNode root, int index)
        {
            root.GetTotalListLength(); // ensure all list lengths are calculated
            Debug.Assert(index >= 0);
            Debug.Assert(index < root._totalListLength);
            TreeListNode listNode = root;
            while (true)
            {
                if (listNode._left != null && index < listNode._left._totalListLength)
                    listNode = listNode._left;
                else
                {
                    if (listNode._left != null)
                        index -= listNode._left._totalListLength;
                    if (listNode._isVisible)
                    {
                        if (index == 0)
                            return listNode;
                        index--;
                    }
                    listNode = listNode._right;
                }
            }
        }

        internal static int GetVisibleIndexForNode(TreeListNode listNode)
        {
            int index = listNode._left != null ? listNode._left.GetTotalListLength() : 0;
            while (listNode._listParent != null)
            {
                if (listNode == listNode._listParent._right)
                {
                    if (listNode._listParent._left != null)
                        index += listNode._listParent._left.GetTotalListLength();
                    if (listNode._listParent._isVisible)
                        index++;
                }
                listNode = listNode._listParent;
            }
            return index;
        }

        /// <summary>
        /// Balances the subtree rooted in <paramref name="listNode"/> and recomputes the 'height' field.
        /// This method assumes that the children of this listNode are already balanced and have an up-to-date 'height' value.
        /// </summary>
        /// <returns>The new root listNode</returns>
        private static TreeListNode Rebalance(TreeListNode listNode)
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
                        listNode._right = listNode._right.RotateRight();
                    listNode = listNode.RotateLeft();
                    // If 'listNode' was unbalanced by more than 2, we've shifted some of the inbalance to the left listNode; so rebalance that.
                    listNode._left = Rebalance(listNode._left);
                }
                else if (listNode.Balance < -1)
                {
                    if (listNode._left != null && listNode._left.Balance > 0)
                        listNode._left = listNode._left.RotateLeft();
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
                return _totalListLength;
            int length = (_isVisible ? 1 : 0);
            if (_left != null)
                length += _left.GetTotalListLength();
            if (_right != null)
                length += _right.GetTotalListLength();
            return _totalListLength = length;
        }

        private TreeListNode RotateLeft()
        {
            /* Rotate tree to the left
			 * 
			 *       this               right
			 *       /  \               /  \
			 *      A   right   ===>  this  C
			 *           / \          / \
			 *          B   C        A   B
			 */
            TreeListNode b = _right._left;
            TreeListNode newTop = _right;

            if (b != null) b._listParent = this;
            _right = b;
            newTop._left = this;
            newTop._listParent = _listParent;
            _listParent = newTop;
            // rebalance the 'this' listNode - this is necessary in some bulk insertion cases:
            newTop._left = Rebalance(this);
            return newTop;
        }

        private TreeListNode RotateRight()
        {
            /* Rotate tree to the right
			 * 
			 *       this             left
			 *       /  \             /  \
			 *     left  C   ===>    A   this
			 *     / \                   /  \
			 *    A   B                 B    C
			 */
            TreeListNode b = _left._right;
            TreeListNode newTop = _left;

            if (b != null) b._listParent = this;
            _left = b;
            newTop._right = this;
            newTop._listParent = _listParent;
            _listParent = newTop;
            newTop._right = Rebalance(this);
            return newTop;
        }

        private static void RebalanceUntilRoot(TreeListNode pos)
        {
            while (pos._listParent != null)
            {
                if (pos == pos._listParent._left)
                    pos = pos._listParent._left = Rebalance(pos);
                else
                {
                    Debug.Assert(pos == pos._listParent._right);
                    pos = pos._listParent._right = Rebalance(pos);
                }
                pos = pos._listParent;
            }
            TreeListNode newRoot = Rebalance(pos);
            if (newRoot != pos && pos._treeListFlattener != null)
            {
                Debug.Assert(newRoot._treeListFlattener == null);
                newRoot._treeListFlattener = pos._treeListFlattener;
                pos._treeListFlattener = null;
                newRoot._treeListFlattener._root = newRoot;
            }
            Debug.Assert(newRoot._listParent == null);
            newRoot.CheckInvariants();
        }

        private static void InsertNodeAfter(TreeListNode pos, TreeListNode newListNode)
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
                    pos = pos._left;
                Debug.Assert(pos._left == null);
                pos._left = newListNode;
                newListNode._listParent = pos;
            }
            RebalanceUntilRoot(pos);
        }

        private void RemoveNodes(TreeListNode start, TreeListNode end)
        {
            // Removes all listNodes from start to end (inclusive)
            // All removed listNodes will be reorganized in a separate tree, do not delete
            // regions that don't belong together in the tree model!

            List<TreeListNode> removedSubtrees = new List<TreeListNode>();
            TreeListNode oldPos;
            TreeListNode pos = start;
            do
            {
                // recalculate the endAncestors every time, because the tree might have been rebalanced
                HashSet<TreeListNode> endAncestors = new HashSet<TreeListNode>();
                for (TreeListNode tmp = end; tmp != null; tmp = tmp._listParent)
                    endAncestors.Add(tmp);

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
                TreeListNode succ = pos.Successor();
                DeleteNode(pos); // this will also rebalance out the deletion of the right subtree

                oldPos = pos;
                pos = succ;
            } while (oldPos != end);

            // merge back together the removed subtrees:
            TreeListNode removed = removedSubtrees[0];
            for (int i = 1; i < removedSubtrees.Count; i++)
                removed = ConcatTrees(removed, removedSubtrees[i]);
        }

        private static TreeListNode ConcatTrees(TreeListNode first, TreeListNode second)
        {
            TreeListNode tmp = first;
            while (tmp._right != null)
                tmp = tmp._right;
            InsertNodeAfter(tmp, second);
            return tmp.GetListRoot();
        }

        private TreeListNode Successor()
        {
            if (_right != null)
            {
                TreeListNode listNode = _right;
                while (listNode._left != null)
                    listNode = listNode._left;
                return listNode;
            }
            else
            {
                TreeListNode listNode = this;
                TreeListNode oldListNode;
                do
                {
                    oldListNode = listNode;
                    listNode = listNode._listParent;
                    // loop while we are on the way up from the right part
                } while (listNode != null && listNode._right == oldListNode);
                return listNode;
            }
        }

        private static void DeleteNode(TreeListNode listNode)
        {
            TreeListNode balancingListNode;
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
                TreeListNode tmp = listNode._right;
                while (tmp._left != null)
                    tmp = tmp._left;
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
                if (tmp._left != null) tmp._left._listParent = tmp;
                if (tmp._right != null) tmp._right._listParent = tmp;
                // Then replace listNode with tmp
                listNode.ReplaceWith(tmp);
                if (balancingListNode == listNode)
                    balancingListNode = tmp;
            }
            Debug.Assert(listNode._listParent == null);
            Debug.Assert(listNode._left == null);
            Debug.Assert(listNode._right == null);
            listNode._height = 1;
            listNode._totalListLength = -1;
            if (balancingListNode != null)
                RebalanceUntilRoot(balancingListNode);
        }

        private void ReplaceWith(TreeListNode listNode)
        {
            if (_listParent != null)
            {
                if (_listParent._left == this)
                    _listParent._left = listNode;
                else
                {
                    Debug.Assert(_listParent._right == this);
                    _listParent._right = listNode;
                }
                if (listNode != null)
                    listNode._listParent = _listParent;
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

        #endregion
    }

    public sealed class TreeListNodeCollection : IList<TreeListNode>, INotifyCollectionChanged
    {
        #region Поля

        private readonly TreeListNode _parent;

        private bool _isRaisingEvent;

        private List<TreeListNode> _list = new List<TreeListNode>();

        #endregion

        #region Свойства

        public TreeListNode this[int index]
        {
            get { return _list[index]; }
            set
            {
                ThrowOnReentrancy();
                var oldItem = _list[index];
                if (oldItem == value)
                    return;
                ThrowIfValueIsNullOrHasParent(value);
                _list[index] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
            }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        bool ICollection<TreeListNode>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region События

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Конструктор

        public TreeListNodeCollection(TreeListNode parent)
        {
            _parent = parent;
        }

        #endregion

        #region Методы

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Debug.Assert(!_isRaisingEvent);
            _isRaisingEvent = true;
            try
            {
                _parent.OnChildrenChanged(e);
                if (CollectionChanged != null)
                    CollectionChanged(this, e);
            }
            finally
            {
                _isRaisingEvent = false;
            }
        }

        private void ThrowOnReentrancy()
        {
            if (_isRaisingEvent)
                throw new InvalidOperationException();
        }

        private void ThrowIfValueIsNullOrHasParent(TreeListNode listNode)
        {
            if (listNode == null)
                throw new ArgumentNullException("listNode");
            if (listNode._modelParent != null)
                throw new ArgumentException("The listNode already has a parent", "listNode");
        }

        public int IndexOf(TreeListNode listNode)
        {
            if (listNode == null || listNode._modelParent != _parent)
                return -1;

            return _list.IndexOf(listNode);
        }

        public void Insert(int index, TreeListNode listNode)
        {
            ThrowOnReentrancy();
            ThrowIfValueIsNullOrHasParent(listNode);
            _list.Insert(index, listNode);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, listNode, index));
        }

        public void InsertRange(int index, IEnumerable<TreeListNode> nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");
            ThrowOnReentrancy();
            List<TreeListNode> newNodes = nodes.ToList();
            if (newNodes.Count == 0)
                return;
            foreach (TreeListNode node in newNodes)
                ThrowIfValueIsNullOrHasParent(node);
            _list.InsertRange(index, newNodes);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newNodes, index));
        }

        public void RemoveAt(int index)
        {
            ThrowOnReentrancy();
            var oldItem = _list[index];
            _list.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
        }

        public void RemoveRange(int index, int count)
        {
            ThrowOnReentrancy();
            if (count == 0)
                return;
            var oldItems = _list.GetRange(index, count);
            _list.RemoveRange(index, count);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, index));
        }

        public void Add(TreeListNode listNode)
        {
            ThrowOnReentrancy();
            ThrowIfValueIsNullOrHasParent(listNode);
            _list.Add(listNode);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, listNode, _list.Count - 1));
        }

        public void AddRange(IEnumerable<TreeListNode> nodes)
        {
            InsertRange(Count, nodes);
        }

        public void Clear()
        {
            ThrowOnReentrancy();
            var oldList = _list;
            _list = new List<TreeListNode>();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldList, 0));
        }

        public bool Contains(TreeListNode listNode)
        {
            return IndexOf(listNode) >= 0;
        }

        public void CopyTo(TreeListNode[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(TreeListNode item)
        {
            int pos = IndexOf(item);
            if (pos >= 0)
            {
                RemoveAt(pos);
                return true;
            }

            return false;
        }

        public IEnumerator<TreeListNode> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void RemoveAll(Predicate<TreeListNode> match)
        {
            if (match == null)
                throw new ArgumentNullException("match");
            ThrowOnReentrancy();
            int firstToRemove = 0;
            for (int i = 0; i < _list.Count; i++)
            {
                bool removeNode;
                _isRaisingEvent = true;
                try
                {
                    removeNode = match(_list[i]);
                }
                finally
                {
                    _isRaisingEvent = false;
                }
                if (!removeNode)
                {
                    if (firstToRemove < i)
                    {
                        RemoveRange(firstToRemove, i - firstToRemove);
                        i = firstToRemove - 1;
                    }
                    else
                        firstToRemove = i + 1;
                    Debug.Assert(firstToRemove == i + 1);
                }
            }
            if (firstToRemove < _list.Count)
                RemoveRange(firstToRemove, _list.Count - firstToRemove);
        }

        #endregion
    }

    public class TreeListNodeView : Control
    {
        #region Поля

        public static readonly DependencyProperty CellEditorProperty = DependencyProperty.Register("CellEditor", typeof(Control), typeof(TreeListNodeView), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty TextBackgroundProperty = DependencyProperty.Register("TextBackground", typeof(Brush), typeof(TreeListNodeView));

        #endregion

        #region Свойства

        public Brush TextBackground
        {
            get { return (Brush)GetValue(TextBackgroundProperty); }
            set { SetValue(TextBackgroundProperty, value); }
        }

        public TreeListNode ListNode
        {
            get { return DataContext as TreeListNode; }
        }

        public TreeListViewItem ParentItem { get; private set; }

        public Control CellEditor
        {
            get { return (Control)GetValue(CellEditorProperty); }
            set { SetValue(CellEditorProperty, value); }
        }

        public TreeList ParentTreeList
        {
            get { return ParentItem.ParentTreeList; }
        }

        internal TreeListLinesRenderer TreeListLinesRenderer { get; private set; }

        #endregion

        #region Конструктор

        static TreeListNodeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListNodeView),
                new FrameworkPropertyMetadata(typeof(TreeListNodeView)));
        }

        #endregion

        #region Методы

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TreeListLinesRenderer = Template.FindName("linesRenderer", this) as TreeListLinesRenderer;
            UpdateTemplate();
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            ParentItem = this.FindAncestor<TreeListViewItem>();
            ParentItem.ListNodeView = this;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == DataContextProperty)
                UpdateDataContext(e.OldValue as TreeListNode, e.NewValue as TreeListNode);
        }

        private void UpdateDataContext(TreeListNode oldListNode, TreeListNode newListNode)
        {
            if (newListNode != null)
            {
                newListNode.PropertyChanged += Node_PropertyChanged;
                if (Template != null)
                    UpdateTemplate();
            }
            if (oldListNode != null)
                oldListNode.PropertyChanged -= Node_PropertyChanged;
        }

        private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEditing")
                OnIsEditingChanged();
            else if (e.PropertyName == "IsLast")
            {
                if (ParentTreeList.ShowLines)
                {
                    foreach (var child in ListNode.VisibleDescendantsAndSelf())
                    {
                        var container = ParentTreeList.ItemContainerGenerator.ContainerFromItem(child) as TreeListViewItem;
                        if (container != null)
                            container.ListNodeView.TreeListLinesRenderer.InvalidateVisual();
                    }
                }
            }
            else if (e.PropertyName == "IsExpanded")
            {
                if (ListNode.IsExpanded)
                    ParentTreeList.HandleExpanding(ListNode);
            }
        }

        private void OnIsEditingChanged()
        {
            var textEditorContainer = Template.FindName("textEditorContainer", this) as Border;
            if (textEditorContainer == null) return;

            if (ListNode.IsEditing)
                textEditorContainer.Child = CellEditor ?? new TreeListEditTextBox {Item = ParentItem};
            else textEditorContainer.Child = null;
        }

        private void UpdateTemplate()
        {
            var spacer = Template.FindName("spacer", this) as FrameworkElement;
            if (spacer == null) return;

            spacer.Width = CalculateIndent();

            var expander = Template.FindName("expander", this) as ToggleButton;
            if (expander == null) return;

            if (ParentTreeList.Root == ListNode && !ParentTreeList.ShowRootExpander)
                expander.Visibility = Visibility.Collapsed;
            else
                expander.ClearValue(VisibilityProperty);
        }

        internal double CalculateIndent()
        {
            var result = 19 * ListNode.Level;
            if (ParentTreeList.ShowRoot)
            {
                if (!ParentTreeList.ShowRootExpander)
                {
                    if (ParentTreeList.Root != ListNode)
                        result -= 15;
                }
            }
            else
                result -= 19;
            if (result < 0)
            {
                Debug.WriteLine("Negative indent level detected for listNode " + ListNode);
                result = 0;
            }
            return result;
        }

        #endregion
    }
}