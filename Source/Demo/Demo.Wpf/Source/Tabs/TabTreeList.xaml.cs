using System;
using System.IO;
using System.Windows;

using SharpLib.Wpf.Controls;

namespace DemoWpf
{
    public partial class TabTreeList
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
            var child2 = new TreeListFsNode("Child1", 1, DateTime.Now, DateTime.Now);

            // FillTree(root, 0);
            root.Children.Add(child1);
            root.Children.Add(child2);

            PART_treeListEx.Root = root;
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