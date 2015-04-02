using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Document
{
    internal sealed class TextAnchorTree
    {
        private readonly TextDocument document;

        private readonly List<TextAnchorNode> nodesToDelete = new List<TextAnchorNode>();

        private TextAnchorNode root;

        public TextAnchorTree(TextDocument document)
        {
            this.document = document;
        }

        [Conditional("DEBUG")]
        private static void Log(string text)
        {
            Debug.WriteLine("TextAnchorTree: " + text);
        }

        #region Insert Text

        private void InsertText(int offset, int length, bool defaultAnchorMovementIsBeforeInsertion)
        {
            if (length == 0 || root == null || offset > root.totalLength)
            {
                return;
            }

            if (offset == root.totalLength)
            {
                PerformInsertText(FindActualBeginNode(root.RightMost), null, length, defaultAnchorMovementIsBeforeInsertion);
            }
            else
            {
                var endNode = FindNode(ref offset);
                Debug.Assert(endNode.length > 0);

                if (offset > 0)
                {
                    endNode.length += length;
                    UpdateAugmentedData(endNode);
                }
                else
                {
                    PerformInsertText(FindActualBeginNode(endNode.Predecessor), endNode, length, defaultAnchorMovementIsBeforeInsertion);
                }
            }
            DeleteMarkedNodes();
        }

        private TextAnchorNode FindActualBeginNode(TextAnchorNode node)
        {
            while (node != null && node.length == 0)
            {
                node = node.Predecessor;
            }
            if (node == null)
            {
                node = root.LeftMost;
            }
            return node;
        }

        private void PerformInsertText(TextAnchorNode beginNode, TextAnchorNode endNode, int length, bool defaultAnchorMovementIsBeforeInsertion)
        {
            Debug.Assert(beginNode != null);

            var beforeInsert = new List<TextAnchorNode>();

            var temp = beginNode;
            while (temp != endNode)
            {
                var anchor = (TextAnchor)temp.Target;
                if (anchor == null)
                {
                    MarkNodeForDelete(temp);
                }
                else if (defaultAnchorMovementIsBeforeInsertion
                    ? anchor.MovementType != AnchorMovementType.AfterInsertion
                    : anchor.MovementType == AnchorMovementType.BeforeInsertion)
                {
                    beforeInsert.Add(temp);
                }
                temp = temp.Successor;
            }

            temp = beginNode;
            foreach (TextAnchorNode node in beforeInsert)
            {
                SwapAnchors(node, temp);
                temp = temp.Successor;
            }

            if (temp == null)
            {
                Debug.Assert(endNode == null);
            }
            else
            {
                temp.length += length;
                UpdateAugmentedData(temp);
            }
        }

        private void SwapAnchors(TextAnchorNode n1, TextAnchorNode n2)
        {
            if (n1 != n2)
            {
                var anchor1 = (TextAnchor)n1.Target;
                var anchor2 = (TextAnchor)n2.Target;
                if (anchor1 == null && anchor2 == null)
                {
                    return;
                }
                n1.Target = anchor2;
                n2.Target = anchor1;
                if (anchor1 == null)
                {
                    nodesToDelete.Remove(n1);
                    MarkNodeForDelete(n2);
                    anchor2.node = n1;
                }
                else if (anchor2 == null)
                {
                    nodesToDelete.Remove(n2);
                    MarkNodeForDelete(n1);
                    anchor1.node = n2;
                }
                else
                {
                    anchor1.node = n2;
                    anchor2.node = n1;
                }
            }
        }

        #endregion

        #region Remove or Replace text

        public void HandleTextChange(OffsetChangeMapEntry entry, DelayedEvents delayedEvents)
        {
            if (entry.RemovalLength == 0)
            {
                InsertText(entry.Offset, entry.InsertionLength, entry.DefaultAnchorMovementIsBeforeInsertion);
                return;
            }

            int offset = entry.Offset;
            int remainingRemovalLength = entry.RemovalLength;

            if (root == null || offset >= root.totalLength)
            {
                return;
            }
            var node = FindNode(ref offset);
            TextAnchorNode firstDeletionSurvivor = null;

            while (node != null && offset + remainingRemovalLength > node.length)
            {
                var anchor = (TextAnchor)node.Target;
                if (anchor != null && (anchor.SurviveDeletion || entry.RemovalNeverCausesAnchorDeletion))
                {
                    if (firstDeletionSurvivor == null)
                    {
                        firstDeletionSurvivor = node;
                    }

                    remainingRemovalLength -= node.length - offset;
                    node.length = offset;
                    offset = 0;
                    UpdateAugmentedData(node);
                    node = node.Successor;
                }
                else
                {
                    var s = node.Successor;
                    remainingRemovalLength -= node.length;
                    RemoveNode(node);

                    nodesToDelete.Remove(node);
                    if (anchor != null)
                    {
                        anchor.OnDeleted(delayedEvents);
                    }
                    node = s;
                }
            }

            if (node != null)
            {
                node.length -= remainingRemovalLength;
                Debug.Assert(node.length >= 0);
            }
            if (entry.InsertionLength > 0)
            {
                if (firstDeletionSurvivor != null)
                {
                    PerformInsertText(firstDeletionSurvivor, node, entry.InsertionLength, entry.DefaultAnchorMovementIsBeforeInsertion);
                }
                else if (node != null)
                {
                    node.length += entry.InsertionLength;
                }
            }
            if (node != null)
            {
                UpdateAugmentedData(node);
            }
            DeleteMarkedNodes();
        }

        #endregion

        #region Node removal when TextAnchor was GC'ed

        private void MarkNodeForDelete(TextAnchorNode node)
        {
            if (!nodesToDelete.Contains(node))
            {
                nodesToDelete.Add(node);
            }
        }

        private void DeleteMarkedNodes()
        {
            CheckProperties();
            while (nodesToDelete.Count > 0)
            {
                int pos = nodesToDelete.Count - 1;
                var n = nodesToDelete[pos];

                var s = n.Successor;
                if (s != null)
                {
                    s.length += n.length;
                }
                RemoveNode(n);
                if (s != null)
                {
                    UpdateAugmentedData(s);
                }
                nodesToDelete.RemoveAt(pos);
                CheckProperties();
            }
            CheckProperties();
        }

        #endregion

        #region FindNode

        private TextAnchorNode FindNode(ref int offset)
        {
            var n = root;
            while (true)
            {
                if (n.left != null)
                {
                    if (offset < n.left.totalLength)
                    {
                        n = n.left;
                        continue;
                    }
                    offset -= n.left.totalLength;
                }
                if (!n.IsAlive)
                {
                    MarkNodeForDelete(n);
                }
                if (offset < n.length)
                {
                    return n;
                }
                offset -= n.length;
                if (n.right != null)
                {
                    n = n.right;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region UpdateAugmentedData

        private void UpdateAugmentedData(TextAnchorNode n)
        {
            if (!n.IsAlive)
            {
                MarkNodeForDelete(n);
            }

            int totalLength = n.length;
            if (n.left != null)
            {
                totalLength += n.left.totalLength;
            }
            if (n.right != null)
            {
                totalLength += n.right.totalLength;
            }
            if (n.totalLength != totalLength)
            {
                n.totalLength = totalLength;
                if (n.parent != null)
                {
                    UpdateAugmentedData(n.parent);
                }
            }
        }

        #endregion

        #region CreateAnchor

        public TextAnchor CreateAnchor(int offset)
        {
            Log("CreateAnchor(" + offset + ")");
            var anchor = new TextAnchor(document);
            anchor.node = new TextAnchorNode(anchor);
            if (root == null)
            {
                root = anchor.node;
                root.totalLength = root.length = offset;
            }
            else if (offset >= root.totalLength)
            {
                anchor.node.totalLength = anchor.node.length = offset - root.totalLength;
                InsertAsRight(root.RightMost, anchor.node);
            }
            else
            {
                var n = FindNode(ref offset);
                Debug.Assert(offset < n.length);

                anchor.node.totalLength = anchor.node.length = offset;
                n.length -= offset;
                InsertBefore(n, anchor.node);
            }
            DeleteMarkedNodes();
            return anchor;
        }

        private void InsertBefore(TextAnchorNode node, TextAnchorNode newNode)
        {
            if (node.left == null)
            {
                InsertAsLeft(node, newNode);
            }
            else
            {
                InsertAsRight(node.left.RightMost, newNode);
            }
        }

        #endregion

        #region Red/Black Tree

        internal const bool RED = true;

        internal const bool BLACK = false;

        private void InsertAsLeft(TextAnchorNode parentNode, TextAnchorNode newNode)
        {
            Debug.Assert(parentNode.left == null);
            parentNode.left = newNode;
            newNode.parent = parentNode;
            newNode.color = RED;
            UpdateAugmentedData(parentNode);
            FixTreeOnInsert(newNode);
        }

        private void InsertAsRight(TextAnchorNode parentNode, TextAnchorNode newNode)
        {
            Debug.Assert(parentNode.right == null);
            parentNode.right = newNode;
            newNode.parent = parentNode;
            newNode.color = RED;
            UpdateAugmentedData(parentNode);
            FixTreeOnInsert(newNode);
        }

        private void FixTreeOnInsert(TextAnchorNode node)
        {
            Debug.Assert(node != null);
            Debug.Assert(node.color == RED);
            Debug.Assert(node.left == null || node.left.color == BLACK);
            Debug.Assert(node.right == null || node.right.color == BLACK);

            var parentNode = node.parent;
            if (parentNode == null)
            {
                node.color = BLACK;
                return;
            }
            if (parentNode.color == BLACK)
            {
                return;
            }

            var grandparentNode = parentNode.parent;
            var uncleNode = Sibling(parentNode);
            if (uncleNode != null && uncleNode.color == RED)
            {
                parentNode.color = BLACK;
                uncleNode.color = BLACK;
                grandparentNode.color = RED;
                FixTreeOnInsert(grandparentNode);
                return;
            }

            if (node == parentNode.right && parentNode == grandparentNode.left)
            {
                RotateLeft(parentNode);
                node = node.left;
            }
            else if (node == parentNode.left && parentNode == grandparentNode.right)
            {
                RotateRight(parentNode);
                node = node.right;
            }

            parentNode = node.parent;
            grandparentNode = parentNode.parent;

            parentNode.color = BLACK;
            grandparentNode.color = RED;

            if (node == parentNode.left && parentNode == grandparentNode.left)
            {
                RotateRight(grandparentNode);
            }
            else
            {
                Debug.Assert(node == parentNode.right && parentNode == grandparentNode.right);
                RotateLeft(grandparentNode);
            }
        }

        private void RemoveNode(TextAnchorNode removedNode)
        {
            if (removedNode.left != null && removedNode.right != null)
            {
                var leftMost = removedNode.right.LeftMost;
                RemoveNode(leftMost);

                ReplaceNode(removedNode, leftMost);
                leftMost.left = removedNode.left;
                if (leftMost.left != null)
                {
                    leftMost.left.parent = leftMost;
                }
                leftMost.right = removedNode.right;
                if (leftMost.right != null)
                {
                    leftMost.right.parent = leftMost;
                }
                leftMost.color = removedNode.color;

                UpdateAugmentedData(leftMost);
                if (leftMost.parent != null)
                {
                    UpdateAugmentedData(leftMost.parent);
                }
                return;
            }

            var parentNode = removedNode.parent;
            var childNode = removedNode.left ?? removedNode.right;
            ReplaceNode(removedNode, childNode);
            if (parentNode != null)
            {
                UpdateAugmentedData(parentNode);
            }
            if (removedNode.color == BLACK)
            {
                if (childNode != null && childNode.color == RED)
                {
                    childNode.color = BLACK;
                }
                else
                {
                    FixTreeOnDelete(childNode, parentNode);
                }
            }
        }

        private void FixTreeOnDelete(TextAnchorNode node, TextAnchorNode parentNode)
        {
            Debug.Assert(node == null || node.parent == parentNode);
            if (parentNode == null)
            {
                return;
            }

            var sibling = Sibling(node, parentNode);
            if (sibling.color == RED)
            {
                parentNode.color = RED;
                sibling.color = BLACK;
                if (node == parentNode.left)
                {
                    RotateLeft(parentNode);
                }
                else
                {
                    RotateRight(parentNode);
                }

                sibling = Sibling(node, parentNode);
            }

            if (parentNode.color == BLACK
                && sibling.color == BLACK
                && GetColor(sibling.left) == BLACK
                && GetColor(sibling.right) == BLACK)
            {
                sibling.color = RED;
                FixTreeOnDelete(parentNode, parentNode.parent);
                return;
            }

            if (parentNode.color == RED
                && sibling.color == BLACK
                && GetColor(sibling.left) == BLACK
                && GetColor(sibling.right) == BLACK)
            {
                sibling.color = RED;
                parentNode.color = BLACK;
                return;
            }

            if (node == parentNode.left &&
                sibling.color == BLACK &&
                GetColor(sibling.left) == RED &&
                GetColor(sibling.right) == BLACK)
            {
                sibling.color = RED;
                sibling.left.color = BLACK;
                RotateRight(sibling);
            }
            else if (node == parentNode.right &&
                     sibling.color == BLACK &&
                     GetColor(sibling.right) == RED &&
                     GetColor(sibling.left) == BLACK)
            {
                sibling.color = RED;
                sibling.right.color = BLACK;
                RotateLeft(sibling);
            }
            sibling = Sibling(node, parentNode);

            sibling.color = parentNode.color;
            parentNode.color = BLACK;
            if (node == parentNode.left)
            {
                if (sibling.right != null)
                {
                    Debug.Assert(sibling.right.color == RED);
                    sibling.right.color = BLACK;
                }
                RotateLeft(parentNode);
            }
            else
            {
                if (sibling.left != null)
                {
                    Debug.Assert(sibling.left.color == RED);
                    sibling.left.color = BLACK;
                }
                RotateRight(parentNode);
            }
        }

        private void ReplaceNode(TextAnchorNode replacedNode, TextAnchorNode newNode)
        {
            if (replacedNode.parent == null)
            {
                Debug.Assert(replacedNode == root);
                root = newNode;
            }
            else
            {
                if (replacedNode.parent.left == replacedNode)
                {
                    replacedNode.parent.left = newNode;
                }
                else
                {
                    replacedNode.parent.right = newNode;
                }
            }
            if (newNode != null)
            {
                newNode.parent = replacedNode.parent;
            }
            replacedNode.parent = null;
        }

        private void RotateLeft(TextAnchorNode p)
        {
            var q = p.right;
            Debug.Assert(q != null);
            Debug.Assert(q.parent == p);

            ReplaceNode(p, q);

            p.right = q.left;
            if (p.right != null)
            {
                p.right.parent = p;
            }

            q.left = p;
            p.parent = q;
            UpdateAugmentedData(p);
            UpdateAugmentedData(q);
        }

        private void RotateRight(TextAnchorNode p)
        {
            var q = p.left;
            Debug.Assert(q != null);
            Debug.Assert(q.parent == p);

            ReplaceNode(p, q);

            p.left = q.right;
            if (p.left != null)
            {
                p.left.parent = p;
            }

            q.right = p;
            p.parent = q;
            UpdateAugmentedData(p);
            UpdateAugmentedData(q);
        }

        private static TextAnchorNode Sibling(TextAnchorNode node)
        {
            if (node == node.parent.left)
            {
                return node.parent.right;
            }
            return node.parent.left;
        }

        private static TextAnchorNode Sibling(TextAnchorNode node, TextAnchorNode parentNode)
        {
            Debug.Assert(node == null || node.parent == parentNode);
            if (node == parentNode.left)
            {
                return parentNode.right;
            }
            return parentNode.left;
        }

        private static bool GetColor(TextAnchorNode node)
        {
            return node != null ? node.color : BLACK;
        }

        #endregion

        #region CheckProperties

        [Conditional("DATACONSISTENCYTEST")]
        internal void CheckProperties()
        {
#if DEBUG
            if (root != null)
            {
                CheckProperties(root);

                int blackCount = -1;
                CheckNodeProperties(root, null, RED, 0, ref blackCount);
            }
#endif
        }

#if DEBUG
        private void CheckProperties(TextAnchorNode node)
        {
            int totalLength = node.length;
            if (node.left != null)
            {
                CheckProperties(node.left);
                totalLength += node.left.totalLength;
            }
            if (node.right != null)
            {
                CheckProperties(node.right);
                totalLength += node.right.totalLength;
            }
            Debug.Assert(node.totalLength == totalLength);
        }

        private void CheckNodeProperties(TextAnchorNode node, TextAnchorNode parentNode, bool parentColor, int blackCount, ref int expectedBlackCount)
        {
            if (node == null)
            {
                return;
            }

            Debug.Assert(node.parent == parentNode);

            if (parentColor == RED)
            {
                Debug.Assert(node.color == BLACK);
            }
            if (node.color == BLACK)
            {
                blackCount++;
            }
            if (node.left == null && node.right == null)
            {
                if (expectedBlackCount == -1)
                {
                    expectedBlackCount = blackCount;
                }
                else
                {
                    Debug.Assert(expectedBlackCount == blackCount);
                }
            }
            CheckNodeProperties(node.left, node, node.color, blackCount, ref expectedBlackCount);
            CheckNodeProperties(node.right, node, node.color, blackCount, ref expectedBlackCount);
        }
#endif

        #endregion

        #region GetTreeAsString

#if DEBUG
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public string GetTreeAsString()
        {
            if (root == null)
            {
                return "<empty tree>";
            }
            var b = new StringBuilder();
            AppendTreeToString(root, b, 0);
            return b.ToString();
        }

        private static void AppendTreeToString(TextAnchorNode node, StringBuilder b, int indent)
        {
            if (node.color == RED)
            {
                b.Append("RED   ");
            }
            else
            {
                b.Append("BLACK ");
            }
            b.AppendLine(node.ToString());
            indent += 2;
            if (node.left != null)
            {
                b.Append(' ', indent);
                b.Append("L: ");
                AppendTreeToString(node.left, b, indent);
            }
            if (node.right != null)
            {
                b.Append(' ', indent);
                b.Append("R: ");
                AppendTreeToString(node.right, b, indent);
            }
        }
#endif

        #endregion
    }
}