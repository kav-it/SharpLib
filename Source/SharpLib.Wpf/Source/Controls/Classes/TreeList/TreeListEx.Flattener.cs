using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace SharpLib.Wpf.Controls
{
    internal sealed class TreeListExFlattener : IList, INotifyCollectionChanged
    {
        #region ����

        private readonly bool _includeRoot;

        private readonly object _syncRoot = new object();

        /// <summary>
        /// The root listNode of the flat list tree.
        /// Tjis is not necessarily the root of the model!
        /// </summary>
        internal TreeListExNode _root;

        #endregion

        #region ��������

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException();
                return TreeListExNode.GetNodeByVisibleIndex(_root, _includeRoot ? index : index + 1);
            }
            set { throw new NotSupportedException(); }
        }

        public int Count
        {
            get { return _includeRoot ? _root.GetTotalListLength() : _root.GetTotalListLength() - 1; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return _syncRoot; }
        }

        #endregion

        #region �������

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region �����������

        public TreeListExFlattener(TreeListExNode modelRoot, bool includeRoot)
        {
            _root = modelRoot;
            while (_root._listParent != null)
                _root = _root._listParent;
            _root._treeListFlattener = this;
            _includeRoot = includeRoot;
        }

        #endregion

        #region ������

        public void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        public void NodesInserted(int index, IEnumerable<TreeListExNode> nodes)
        {
            if (!_includeRoot) index--;
            foreach (TreeListExNode node in nodes)
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, node, index++));
        }

        public void NodesRemoved(int index, IEnumerable<TreeListExNode> nodes)
        {
            if (!_includeRoot) index--;
            foreach (TreeListExNode node in nodes)
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, node, index));
        }

        public void Stop()
        {
            Debug.Assert(_root._treeListFlattener == this);
            _root._treeListFlattener = null;
        }

        public int IndexOf(object item)
        {
            TreeListExNode listNode = item as TreeListExNode;
            if (listNode != null && listNode.IsVisible && listNode.GetListRoot() == _root)
            {
                if (_includeRoot)
                    return TreeListExNode.GetVisibleIndexForNode(listNode);

                return TreeListExNode.GetVisibleIndexForNode(listNode) - 1;
            }

            return -1;
        }

        void IList.Insert(int index, object item)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        int IList.Add(object item)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(object item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            foreach (object item in this)
                array.SetValue(item, arrayIndex++);
        }

        void IList.Remove(object item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        #endregion
    }
}