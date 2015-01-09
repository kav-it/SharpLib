using System.ComponentModel;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    /// <summary>
    /// Элемент TreeView
    /// </summary>
    public partial class TreeViewControl : UserControl
    {
        #region Поля

        private readonly TreeViewNodeCollection _nodes;

        #endregion

        #region Свойства

        /// <summary>
        /// Элементы дерева
        /// </summary>
        [Browsable(false)]
        public TreeViewNodeCollection Nodes
        {
            get { return _nodes; }
        }

        /// <summary>
        /// Выделенный элемент
        /// </summary>
        public TreeViewNode SelectedItem
        {
            get { return (TreeViewNode)treeView1.SelectedNode; }
        }

        #endregion

        #region Конструктор

        public TreeViewControl()
        {
            InitializeComponent();

            _nodes = new TreeViewNodeCollection(treeView1);
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обработка Selection по Right-click
        /// </summary>
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
            }
        }

        public void ExpandAll()
        {
            treeView1.ExpandAll();
        }

        public void CollapseAll()
        {
            treeView1.CollapseAll();
        }

        #endregion
    }
}