using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace SharpLib.Wpf.Controls
{
    /// <summary>
    /// Коллекция элементов "Node"
    /// </summary>
    public sealed class TreeListExNodeCollection : IEnumerable<TreeListExNode>, INotifyCollectionChanged
    {
        #region Поля

        private readonly TreeListExNode _parent;

        private bool _isRaisingEvent;

        private List<TreeListExNode> _list = new List<TreeListExNode>();

        #endregion

        #region Свойства

        /// <summary>
        /// Индексатор
        /// </summary>
        public TreeListExNode this[int index]
        {
            get { return _list[index]; }
            internal set
            {
                ThrowOnReentrancy();
                var oldItem = _list[index];
                if (oldItem == value)
                {
                    return;
                }
                ThrowIfValueIsNullOrHasParent(value);
                _list[index] = value;
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
            }
        }

        /// <summary>
        /// Количество элементов
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        #endregion

        #region События

        /// <summary>
        /// Событие "Коллекция изменена"
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Конструктор

        internal TreeListExNodeCollection(TreeListExNode parent)
        {
            _parent = parent;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Генерация события "Коллекция изменена"
        /// </summary>
        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Debug.Assert(!_isRaisingEvent);
            _isRaisingEvent = true;
            try
            {
                _parent.OnChildrenChanged(e);
                if (CollectionChanged != null)
                {
                    CollectionChanged(this, e);
                }
            }
            finally
            {
                _isRaisingEvent = false;
            }
        }

        /// <summary>
        /// Проверка на повторную входимость при вызовых методов
        /// </summary>
        private void ThrowOnReentrancy()
        {
            if (_isRaisingEvent)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Проверка, имеет ли элемент родителя
        /// </summary>
        private void ThrowIfValueIsNullOrHasParent(TreeListExNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("listNode");
            }
            if (node._modelParent != null)
            {
                throw new ArgumentException("The listNode already has a parent", "listNode");
            }
        }

        public int IndexOf(TreeListExNode node)
        {
            if (node == null || node._modelParent != _parent)
            {
                return -1;
            }

            return _list.IndexOf(node);
        }

        public void Insert(int index, TreeListExNode node)
        {
            ThrowOnReentrancy();
            ThrowIfValueIsNullOrHasParent(node);
            _list.Insert(index, node);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, node, index));
        }

        public void InsertRange(int index, IEnumerable<TreeListExNode> nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }
            ThrowOnReentrancy();
            var newNodes = nodes.ToList();
            if (newNodes.Count == 0)
            {
                return;
            }
            foreach (TreeListExNode node in newNodes)
            {
                ThrowIfValueIsNullOrHasParent(node);
            }
            _list.InsertRange(index, newNodes);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newNodes, index));
        }

        public void RemoveAt(int index)
        {
            ThrowOnReentrancy();
            var oldItem = _list[index];
            _list.RemoveAt(index);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
        }

        public void RemoveRange(int index, int count)
        {
            ThrowOnReentrancy();
            if (count == 0)
            {
                return;
            }
            var oldItems = _list.GetRange(index, count);
            _list.RemoveRange(index, count);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, index));
        }

        internal void Add(TreeListExNode listNode)
        {
            ThrowOnReentrancy();
            ThrowIfValueIsNullOrHasParent(listNode);
            _list.Add(listNode);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, listNode, _list.Count - 1));
        }

        internal void AddRange(IEnumerable<TreeListExNode> nodes)
        {
            InsertRange(Count, nodes);
        }

        public void Clear()
        {
            ThrowOnReentrancy();
            var oldList = _list;
            _list = new List<TreeListExNode>();
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldList, 0));
        }

        public bool Contains(TreeListExNode listNode)
        {
            return IndexOf(listNode) >= 0;
        }

        internal void CopyTo(TreeListExNode[] array, int arrayIndex)
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

        internal void RemoveAll(Predicate<TreeListExNode> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
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
                    {
                        firstToRemove = i + 1;
                    }
                    Debug.Assert(firstToRemove == i + 1);
                }
            }
            if (firstToRemove < _list.Count)
            {
                RemoveRange(firstToRemove, _list.Count - firstToRemove);
            }
        }

        #endregion
    }
}