using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace SharpLib.Wpf.Controls
{
    public sealed class TreeListExNodeCollection : IList<TreeListExNode>, INotifyCollectionChanged
    {
        #region Поля

        private readonly TreeListExNode _parent;

        private bool _isRaisingEvent;

        private List<TreeListExNode> _list = new List<TreeListExNode>();

        #endregion

        #region Свойства

        public TreeListExNode this[int index]
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

        bool ICollection<TreeListExNode>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region События

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Конструктор

        public TreeListExNodeCollection(TreeListExNode parent)
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

        private void ThrowIfValueIsNullOrHasParent(TreeListExNode listNode)
        {
            if (listNode == null)
                throw new ArgumentNullException("listNode");
            if (listNode._modelParent != null)
                throw new ArgumentException("The listNode already has a parent", "listNode");
        }

        public int IndexOf(TreeListExNode listNode)
        {
            if (listNode == null || listNode._modelParent != _parent)
                return -1;

            return _list.IndexOf(listNode);
        }

        public void Insert(int index, TreeListExNode listNode)
        {
            ThrowOnReentrancy();
            ThrowIfValueIsNullOrHasParent(listNode);
            _list.Insert(index, listNode);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, listNode, index));
        }

        public void InsertRange(int index, IEnumerable<TreeListExNode> nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");
            ThrowOnReentrancy();
            List<TreeListExNode> newNodes = nodes.ToList();
            if (newNodes.Count == 0)
                return;
            foreach (TreeListExNode node in newNodes)
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

        public void Add(TreeListExNode listNode)
        {
            ThrowOnReentrancy();
            ThrowIfValueIsNullOrHasParent(listNode);
            _list.Add(listNode);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, listNode, _list.Count - 1));
        }

        public void AddRange(IEnumerable<TreeListExNode> nodes)
        {
            InsertRange(Count, nodes);
        }

        public void Clear()
        {
            ThrowOnReentrancy();
            var oldList = _list;
            _list = new List<TreeListExNode>();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldList, 0));
        }

        public bool Contains(TreeListExNode listNode)
        {
            return IndexOf(listNode) >= 0;
        }

        public void CopyTo(TreeListExNode[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(TreeListExNode item)
        {
            int pos = IndexOf(item);
            if (pos >= 0)
            {
                RemoveAt(pos);
                return true;
            }

            return false;
        }

        public IEnumerator<TreeListExNode> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void RemoveAll(Predicate<TreeListExNode> match)
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
}