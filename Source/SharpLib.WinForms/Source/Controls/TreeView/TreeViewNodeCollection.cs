using System.Collections;
using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    public class TreeViewNodeCollection : IEnumerable
    {
        #region Поля

        private readonly TreeView _treeView;

        #endregion

        #region Конструктор

        internal TreeViewNodeCollection(TreeView treeView)
        {
            _treeView = treeView;
        }

        #endregion

        #region Методы

        public IEnumerator GetEnumerator()
        {
            return _treeView.Nodes.GetEnumerator();
        }

        /// <summary>
        /// Добавление элемента
        /// </summary>
        public void Add(TreeViewNode node)
        {
            _treeView.Nodes.Add(node);
        }

        /// <summary>
        /// Добавление элемента (с автосозданием)
        /// </summary>
        public TreeViewNode Add(string text, object tag = null)
        {
            var node = new TreeViewNode(text, tag);

            Add(node);

            return node;
        }

        #endregion
    }
}