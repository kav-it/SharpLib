using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpLib.Wpf.Controls
{
    public class TreeListExViewItem : ListViewItem
    {
        #region Поля

        private Point _startPoint;

        private bool _wasDoubleClick;

        private bool _wasSelected;

        #endregion

        #region Свойства

        public TreeListExNode ListNode
        {
            get { return DataContext as TreeListExNode; }
        }

        public TreeListExNodeView ListNodeView { get; internal set; }

        public TreeListEx ParentTreeList { get; internal set; }

        #endregion

        #region Конструктор

        static TreeListExViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListExViewItem),
                new FrameworkPropertyMetadata(typeof(TreeListExViewItem)));
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