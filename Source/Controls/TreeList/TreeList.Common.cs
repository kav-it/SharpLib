// ****************************************************************************
//
// Имя файла    : 'TreeList.Common.cs'
// Заголовок    : Общие классы компонента TreeList
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 11/03/2014
//
// ****************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace SharpLib.Controls
{
    public class TreeListCollapsedConverter : MarkupExtension, IValueConverter
    {
        #region Поля

        public static TreeListCollapsedConverter _instance = new TreeListCollapsedConverter();

        #endregion

        #region Методы

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal static class TreeListExtensionMethods
    {
        #region Методы

        public static T FindAncestor<T>(this DependencyObject d) where T : class
        {
            return AncestorsAndSelf(d).OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject d)
        {
            while (d != null)
            {
                yield return d;
                d = VisualTreeHelper.GetParent(d);
            }
        }

        public static void AddOnce(this IList list, object item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        #endregion
    }

    internal sealed class TreeListFlattener : IList, INotifyCollectionChanged
    {
        #region Поля

        private readonly bool _includeRoot;

        private readonly object _syncRoot = new object();

        /// <summary>
        /// The root listNode of the flat list tree.
        /// Tjis is not necessarily the root of the model!
        /// </summary>
        internal TreeListNode _root;

        #endregion

        #region Свойства

        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException();
                return TreeListNode.GetNodeByVisibleIndex(_root, _includeRoot ? index : index + 1);
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

        #region События

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Конструктор

        public TreeListFlattener(TreeListNode modelRoot, bool includeRoot)
        {
            _root = modelRoot;
            while (_root._listParent != null)
                _root = _root._listParent;
            _root._treeListFlattener = this;
            _includeRoot = includeRoot;
        }

        #endregion

        #region Методы

        public void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        public void NodesInserted(int index, IEnumerable<TreeListNode> nodes)
        {
            if (!_includeRoot) index--;
            foreach (TreeListNode node in nodes)
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, node, index++));
        }

        public void NodesRemoved(int index, IEnumerable<TreeListNode> nodes)
        {
            if (!_includeRoot) index--;
            foreach (TreeListNode node in nodes)
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, node, index));
        }

        public void Stop()
        {
            Debug.Assert(_root._treeListFlattener == this);
            _root._treeListFlattener = null;
        }

        public int IndexOf(object item)
        {
            TreeListNode listNode = item as TreeListNode;
            if (listNode != null && listNode.IsVisible && listNode.GetListRoot() == _root)
            {
                if (_includeRoot)
                    return TreeListNode.GetVisibleIndexForNode(listNode);

                return TreeListNode.GetVisibleIndexForNode(listNode) - 1;
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

    public class TreeListGridView : GridView
    {
        #region Свойства

        public static ResourceKey ItemContainerStyleKey { get; private set; }

        protected override object ItemContainerDefaultStyleKey
        {
            get { return ItemContainerStyleKey; }
        }

        #endregion

        #region Конструктор

        static TreeListGridView()
        {
            ItemContainerStyleKey = new ComponentResourceKey(typeof(TreeList), "GridViewItemContainerStyleKey");
        }

        #endregion
    }

    public class TreeListInsertMarker : Control
    {
        #region Конструктор

        static TreeListInsertMarker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListInsertMarker), new FrameworkPropertyMetadata(typeof(TreeListInsertMarker)));
        }

        #endregion
    }

    internal class TreeListLinesRenderer : FrameworkElement
    {
        #region Поля

        private static readonly Pen _pen;

        #endregion

        #region Свойства

        private TreeListNodeView ListNodeView
        {
            get { return TemplatedParent as TreeListNodeView; }
        }

        #endregion

        #region Конструктор

        static TreeListLinesRenderer()
        {
            _pen = new Pen(Brushes.LightGray, 1);
            _pen.Freeze();
        }

        #endregion

        #region Методы

        protected override void OnRender(DrawingContext dc)
        {
            var indent = ListNodeView.CalculateIndent();
            var p = new Point(indent + 4.5, 0);

            if (!ListNodeView.ListNode.IsRoot || ListNodeView.ParentTreeList.ShowRootExpander)
                dc.DrawLine(_pen, new Point(p.X, ActualHeight / 2), new Point(p.X + 10, ActualHeight / 2));

            if (ListNodeView.ListNode.IsRoot) return;

            dc.DrawLine(_pen, p, ListNodeView.ListNode.IsLast ? new Point(p.X, ActualHeight / 2) : new Point(p.X, ActualHeight));

            var current = ListNodeView.ListNode;
            while (true)
            {
                p.X -= 19;
                current = current.Parent;
                if (p.X < 0) break;
                if (!current.IsLast)
                    dc.DrawLine(_pen, p, new Point(p.X, ActualHeight));
            }
        }

        #endregion
    }

    internal static class TreeListTraversal
    {
        #region Методы

        /// <summary>
        /// Converts a tree data structure into a flat list by traversing it in pre-order.
        /// </summary>
        /// <param name="root">The root element of the tree.</param>
        /// <param name="recursion">The function that gets the children of an element.</param>
        /// <returns>Iterator that enumerates the tree structure in pre-order.</returns>
        public static IEnumerable<T> PreOrder<T>(T root, Func<T, IEnumerable<T>> recursion)
        {
            return PreOrder(new[] {root}, recursion);
        }

        /// <summary>
        /// Converts a tree data structure into a flat list by traversing it in pre-order.
        /// </summary>
        /// <param name="input">The root elements of the forest.</param>
        /// <param name="recursion">The function that gets the children of an element.</param>
        /// <returns>Iterator that enumerates the tree structure in pre-order.</returns>
        public static IEnumerable<T> PreOrder<T>(IEnumerable<T> input, Func<T, IEnumerable<T>> recursion)
        {
            Stack<IEnumerator<T>> stack = new Stack<IEnumerator<T>>();
            try
            {
                stack.Push(input.GetEnumerator());
                while (stack.Count > 0)
                {
                    while (stack.Peek().MoveNext())
                    {
                        T element = stack.Peek().Current;
                        yield return element;
                        IEnumerable<T> children = recursion(element);
                        if (children != null)
                            stack.Push(children.GetEnumerator());
                    }
                    stack.Pop().Dispose();
                }
            }
            finally
            {
                while (stack.Count > 0)
                    stack.Pop().Dispose();
            }
        }

        #endregion
    }

    public class TreeListGeneralAdorner : Adorner
    {
        #region Поля

        private FrameworkElement _child;

        #endregion

        #region Свойства

        public FrameworkElement Child
        {
            get { return _child; }
            set
            {
                if (!Equals(_child, value))
                {
                    RemoveVisualChild(_child);
                    RemoveLogicalChild(_child);
                    _child = value;
                    AddLogicalChild(value);
                    AddVisualChild(value);
                    InvalidateMeasure();
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get { return _child == null ? 0 : 1; }
        }

        #endregion

        #region Конструктор

        public TreeListGeneralAdorner(UIElement target) : base(target)
        {
        }

        #endregion

        #region Методы

        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (_child != null)
            {
                _child.Measure(constraint);
                return _child.DesiredSize;
            }
            return new Size();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_child != null)
            {
                _child.Arrange(new Rect(finalSize));
                return finalSize;
            }
            return new Size();
        }

        #endregion
    }

    internal class TreeListEditTextBox : TextBox
    {
        #region Поля

        private bool _commiting;

        #endregion

        #region Свойства

        public TreeListViewItem Item { get; set; }

        public TreeListNode ListNode
        {
            get { return Item.ListNode; }
        }

        #endregion

        #region Конструктор

        static TreeListEditTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListEditTextBox),
                new FrameworkPropertyMetadata(typeof(TreeListEditTextBox)));
        }

        public TreeListEditTextBox()
        {
            Loaded += delegate { Init(); };
        }

        #endregion

        #region Методы

        private void Init()
        {
            Text = ListNode.LoadEditText();
            Focus();
            SelectAll();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Commit();
            else if (e.Key == Key.Escape)
                ListNode.IsEditing = false;
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (ListNode.IsEditing)
                Commit();
        }

        private void Commit()
        {
            if (!_commiting)
            {
                _commiting = true;

                ListNode.IsEditing = false;
                if (!ListNode.SaveEditText(Text))
                    Item.Focus();
                ListNode.RaisePropertyChanged("Text");

                _commiting = false;
            }
        }

        #endregion
    }
}