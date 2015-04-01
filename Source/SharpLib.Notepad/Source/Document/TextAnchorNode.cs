using System;

namespace SharpLib.Notepad.Document
{
    internal sealed class TextAnchorNode : WeakReference
    {
        #region Поля

        internal bool color;

        internal TextAnchorNode left;

        internal int length;

        internal TextAnchorNode parent;

        internal TextAnchorNode right;

        internal int totalLength;

        #endregion

        #region Свойства

        internal TextAnchorNode LeftMost
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

        internal TextAnchorNode RightMost
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

        internal TextAnchorNode Successor
        {
            get
            {
                if (right != null)
                {
                    return right.LeftMost;
                }
                var node = this;
                TextAnchorNode oldNode;
                do
                {
                    oldNode = node;
                    node = node.parent;
                } while (node != null && node.right == oldNode);
                return node;
            }
        }

        internal TextAnchorNode Predecessor
        {
            get
            {
                if (left != null)
                {
                    return left.RightMost;
                }
                var node = this;
                TextAnchorNode oldNode;
                do
                {
                    oldNode = node;
                    node = node.parent;
                } while (node != null && node.left == oldNode);
                return node;
            }
        }

        #endregion

        #region Конструктор

        public TextAnchorNode(TextAnchor anchor)
            : base(anchor)
        {
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return "[TextAnchorNode Length=" + length + " TotalLength=" + totalLength + " Target=" + Target + "]";
        }

        #endregion
    }
}