using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    public class TreeListExNodeView : Control
    {
        #region Поля

        public static readonly DependencyProperty CellEditorProperty = DependencyProperty.Register("CellEditor", typeof(Control), typeof(TreeListExNodeView), new FrameworkPropertyMetadata());

        public static readonly DependencyProperty TextBackgroundProperty = DependencyProperty.Register("TextBackground", typeof(Brush), typeof(TreeListExNodeView));

        #endregion

        #region Свойства

        public Brush TextBackground
        {
            get { return (Brush)GetValue(TextBackgroundProperty); }
            set { SetValue(TextBackgroundProperty, value); }
        }

        public TreeListExNode ListNode
        {
            get { return DataContext as TreeListExNode; }
        }

        public TreeListExViewItem ParentItem { get; private set; }

        public Control CellEditor
        {
            get { return (Control)GetValue(CellEditorProperty); }
            set { SetValue(CellEditorProperty, value); }
        }

        public TreeListEx ParentTreeList
        {
            get { return ParentItem.ParentTreeList; }
        }

        internal TreeListExLinesRenderer TreeListLinesRenderer { get; private set; }

        #endregion

        #region Конструктор

        static TreeListExNodeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListExNodeView),
                new FrameworkPropertyMetadata(typeof(TreeListExNodeView)));
        }

        #endregion

        #region Методы

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TreeListLinesRenderer = Template.FindName("linesRenderer", this) as TreeListExLinesRenderer;
            UpdateTemplate();
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            ParentItem = this.FindAncestor<TreeListExViewItem>();
            ParentItem.ListNodeView = this;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == DataContextProperty)
                UpdateDataContext(e.OldValue as TreeListExNode, e.NewValue as TreeListExNode);
        }

        private void UpdateDataContext(TreeListExNode oldListNode, TreeListExNode newListNode)
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
                        var container = ParentTreeList.ItemContainerGenerator.ContainerFromItem(child) as TreeListExViewItem;
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
                textEditorContainer.Child = CellEditor ?? new TreeListExEditTextBox {Item = ParentItem};
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