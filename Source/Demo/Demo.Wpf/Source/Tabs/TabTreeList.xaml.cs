using System;
using System.Windows;

using SharpLib.Wpf.Controls;
using SharpLib.Wpf.Dragging;

namespace DemoWpf
{
    public partial class TabTreeList : IDragSource, IDragDest
    {
        #region Конструктор

        public TabTreeList()
        {
            InitializeComponent();
        }

        #endregion

        #region Методы

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var root = new TreeListFsNode("Root", 0, DateTime.Now, DateTime.Now);
            var child1 = new TreeListFsNode("Child1", 1, DateTime.Now, DateTime.Now);
            var child2 = new TreeListFsNode("Child2", 1, DateTime.Now, DateTime.Now);
            var child3 = new TreeListFsNode("Child3", 1, DateTime.Now, DateTime.Now);
            var child4 = new TreeListFsNode("Child4", 1, DateTime.Now, DateTime.Now);

            PART_treeListEx.DragDest = this;
            PART_treeListEx.DragSource = this;
            PART_treeListEx.Sorter = TreeListExSorter.Default;
            PART_treeListEx.Root = root;

            root.AddChild(child3);
            root.AddChild(child4);
            root.AddChild(child2);
            root.AddChild(child1);

            root.ExpandAll();
        }

        private void FillTree(TreeListExNode root, int level)
        {
            //Directory.GetDirectories(item.Tag.ToString())
            //foreach (string s in Directory.GetLogicalDrives())
            //{
            //    TreeViewItem item = new TreeViewItem();
            //    item.Header = s;
            //    item.Tag = s;
            //    item.FontWeight = FontWeights.Normal;
            //    item.Items.Add(dummyNode);
            //    item.Expanded += new RoutedEventHandler(folder_Expanded);
            //    foldersItem.Items.Add(item);
            //}
        }

        void IDragSource.StartDrag(IDragInfo dragInfo)
        {
            dragInfo.Data = PART_treeListEx.SelectedItems;
        }

        bool IDragSource.CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        void IDragSource.Dropped(IDropInfo dropInfo)
        {
        }

        void IDragSource.DragCancelled()
        {
        }

        void IDragDest.DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

            if (dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem)
            {
                dropInfo.Effects = DragDropEffects.Link;
            }
        }

        void IDragDest.Drop(IDropInfo dropInfo)
        {
        }

        #endregion

        #region Вложенный класс: TreeListFsNode

        internal class TreeListFsNode : TreeListExNode
        {
            #region Поля

            private readonly string _text;

            #endregion

            #region Свойства

            public int Size { get; private set; }

            public DateTime Created { get; private set; }

            public DateTime Modify { get; private set; }

            public override object Text
            {
                get { return _text; }
            }

            #endregion

            #region Конструктор

            public TreeListFsNode(string text, int size, DateTime created, DateTime modify)
            {
                _text = text;
                Size = size;
                Created = created;
                Modify = modify;
            }

            #endregion
        }

        #endregion
    }
}