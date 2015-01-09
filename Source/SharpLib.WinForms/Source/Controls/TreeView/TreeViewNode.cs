using System.Windows.Forms;

namespace SharpLib.WinForms.Controls
{
    public class TreeViewNode : TreeNode
    {
        #region Конструктор

        public TreeViewNode(string text, object tag = null) : base(text)
        {
            Tag = tag;
        }

        public TreeViewNode Add(string text, object tag = null)
        {
            var node = new TreeViewNode(text, tag);

            Nodes.Add(node);

            return node;
        }

        #endregion
    }
}