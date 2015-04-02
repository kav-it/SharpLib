using System.Collections.Generic;
using System.Diagnostics;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Rendering
{
    internal sealed class HeightTreeNode
    {
        internal readonly DocumentLine documentLine;

        internal HeightTreeLineNode lineNode;

        internal HeightTreeNode left, right, parent;

        internal bool color;

        internal HeightTreeNode()
        {
        }

        internal HeightTreeNode(DocumentLine documentLine, double height)
        {
            this.documentLine = documentLine;
            totalCount = 1;
            lineNode = new HeightTreeLineNode(height);
            totalHeight = height;
        }

        internal HeightTreeNode LeftMost
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

        internal HeightTreeNode RightMost
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

        internal HeightTreeNode Successor
        {
            get
            {
                if (right != null)
                {
                    return right.LeftMost;
                }
                var node = this;
                HeightTreeNode oldNode;
                do
                {
                    oldNode = node;
                    node = node.parent;
                } while (node != null && node.right == oldNode);
                return node;
            }
        }

        internal int totalCount;

        internal double totalHeight;

        internal List<CollapsedLineSection> collapsedSections;

        internal bool IsDirectlyCollapsed
        {
            get { return collapsedSections != null; }
        }

        internal void AddDirectlyCollapsed(CollapsedLineSection section)
        {
            if (collapsedSections == null)
            {
                collapsedSections = new List<CollapsedLineSection>();
                totalHeight = 0;
            }
            Debug.Assert(!collapsedSections.Contains(section));
            collapsedSections.Add(section);
        }

        internal void RemoveDirectlyCollapsed(CollapsedLineSection section)
        {
            Debug.Assert(collapsedSections.Contains(section));
            collapsedSections.Remove(section);
            if (collapsedSections.Count == 0)
            {
                collapsedSections = null;
                totalHeight = lineNode.TotalHeight;
                if (left != null)
                {
                    totalHeight += left.totalHeight;
                }
                if (right != null)
                {
                    totalHeight += right.totalHeight;
                }
            }
        }

#if DEBUG
        public override string ToString()
        {
            return "[HeightTreeNode "
                   + documentLine.LineNumber + " CS=" + GetCollapsedSections(collapsedSections)
                   + " Line.CS=" + GetCollapsedSections(lineNode.collapsedSections)
                   + " Line.Height=" + lineNode.height
                   + " TotalHeight=" + totalHeight
                   + "]";
        }

        private static string GetCollapsedSections(List<CollapsedLineSection> list)
        {
            if (list == null)
            {
                return "{}";
            }
            return "{" +
                   string.Join(",",
                       list.ConvertAll(cs => cs.ID).ToArray())
                   + "}";
        }
#endif
    }
}