namespace SharpLib.Notepad.Document
{
    using LineNode = DocumentLine;

    partial class DocumentLine
    {
        #region Поля

        internal bool color;

        internal DocumentLine left;

        internal int nodeTotalCount;

        internal int nodeTotalLength;

        internal DocumentLine parent;

        internal DocumentLine right;

        #endregion

        #region Свойства

        internal LineNode LeftMost
        {
            get
            {
                var node = this;
                while (node.left != null)
                {
                    node = node.left;
                }
                return node;
            }
        }

        internal LineNode RightMost
        {
            get
            {
                var node = this;
                while (node.right != null)
                {
                    node = node.right;
                }
                return node;
            }
        }

        #endregion

        #region Методы

        internal void ResetLine()
        {
            totalLength = delimiterLength = 0;
            isDeleted = color = false;
            left = right = parent = null;
        }

        internal LineNode InitLineNode()
        {
            nodeTotalCount = 1;
            nodeTotalLength = TotalLength;
            return this;
        }

        #endregion
    }
}