using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Document
{
    public sealed class TextSegmentCollection<T> : ICollection<T>, ISegmentTree, IWeakEventListener where T : TextSegment
    {
        private int count;

        private TextSegment root;

        private readonly bool isConnectedToDocument;

        #region Constructor

        public TextSegmentCollection()
        {
        }

        public TextSegmentCollection(TextDocument textDocument)
        {
            if (textDocument == null)
            {
                throw new ArgumentNullException("textDocument");
            }

            textDocument.VerifyAccess();
            isConnectedToDocument = true;
            TextDocumentWeakEventManager.Changed.AddListener(textDocument, this);
        }

        #endregion

        #region OnDocumentChanged / UpdateOffsets

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(TextDocumentWeakEventManager.Changed))
            {
                OnDocumentChanged((DocumentChangeEventArgs)e);
                return true;
            }
            return false;
        }

        public void UpdateOffsets(DocumentChangeEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (isConnectedToDocument)
            {
                throw new InvalidOperationException("This TextSegmentCollection will automatically update offsets; do not call UpdateOffsets manually!");
            }
            OnDocumentChanged(e);
            CheckProperties();
        }

        private void OnDocumentChanged(DocumentChangeEventArgs e)
        {
            var map = e.OffsetChangeMapOrNull;
            if (map != null)
            {
                foreach (OffsetChangeMapEntry entry in map)
                {
                    UpdateOffsetsInternal(entry);
                }
            }
            else
            {
                UpdateOffsetsInternal(e.CreateSingleChangeMapEntry());
            }
        }

        public void UpdateOffsets(OffsetChangeMapEntry change)
        {
            if (isConnectedToDocument)
            {
                throw new InvalidOperationException("This TextSegmentCollection will automatically update offsets; do not call UpdateOffsets manually!");
            }
            UpdateOffsetsInternal(change);
            CheckProperties();
        }

        #endregion

        #region UpdateOffsets (implementation)

        private void UpdateOffsetsInternal(OffsetChangeMapEntry change)
        {
            if (change.RemovalLength == 0)
            {
                InsertText(change.Offset, change.InsertionLength);
            }
            else
            {
                ReplaceText(change);
            }
        }

        private void InsertText(int offset, int length)
        {
            if (length == 0)
            {
                return;
            }

            foreach (TextSegment segment in FindSegmentsContaining(offset))
            {
                if (segment.StartOffset < offset && offset < segment.EndOffset)
                {
                    segment.Length += length;
                }
            }

            TextSegment node = FindFirstSegmentWithStartAfter(offset);
            if (node != null)
            {
                node.nodeLength += length;
                UpdateAugmentedData(node);
            }
        }

        private void ReplaceText(OffsetChangeMapEntry change)
        {
            Debug.Assert(change.RemovalLength > 0);
            int offset = change.Offset;
            foreach (TextSegment segment in FindOverlappingSegments(offset, change.RemovalLength))
            {
                if (segment.StartOffset <= offset)
                {
                    if (segment.EndOffset >= offset + change.RemovalLength)
                    {
                        segment.Length += change.InsertionLength - change.RemovalLength;
                    }
                    else
                    {
                        segment.Length = offset - segment.StartOffset;
                    }
                }
                else
                {
                    int remainingLength = segment.EndOffset - (offset + change.RemovalLength);
                    RemoveSegment(segment);
                    segment.StartOffset = offset + change.RemovalLength;
                    segment.Length = Math.Max(0, remainingLength);
                    AddSegment(segment);
                }
            }

            TextSegment node = FindFirstSegmentWithStartAfter(offset + 1);
            if (node != null)
            {
                Debug.Assert(node.nodeLength >= change.RemovalLength);
                node.nodeLength += change.InsertionLength - change.RemovalLength;
                UpdateAugmentedData(node);
            }
        }

        #endregion

        #region Add

        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.ownerTree != null)
            {
                throw new ArgumentException("The segment is already added to a SegmentCollection.");
            }
            AddSegment(item);
        }

        void ISegmentTree.Add(TextSegment s)
        {
            AddSegment(s);
        }

        private void AddSegment(TextSegment node)
        {
            int insertionOffset = node.StartOffset;
            node.distanceToMaxEnd = node.segmentLength;
            if (root == null)
            {
                root = node;
                node.totalNodeLength = node.nodeLength;
            }
            else if (insertionOffset >= root.totalNodeLength)
            {
                node.nodeLength = node.totalNodeLength = insertionOffset - root.totalNodeLength;
                InsertAsRight(root.RightMost, node);
            }
            else
            {
                var n = FindNode(ref insertionOffset);
                Debug.Assert(insertionOffset < n.nodeLength);

                node.totalNodeLength = node.nodeLength = insertionOffset;
                n.nodeLength -= insertionOffset;
                InsertBefore(n, node);
            }
            node.ownerTree = this;
            count++;
            CheckProperties();
        }

        private void InsertBefore(TextSegment node, TextSegment newNode)
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

        #region GetNextSegment / GetPreviousSegment

        public T GetNextSegment(T segment)
        {
            if (!Contains(segment))
            {
                throw new ArgumentException("segment is not inside the segment tree");
            }
            return (T)segment.Successor;
        }

        public T GetPreviousSegment(T segment)
        {
            if (!Contains(segment))
            {
                throw new ArgumentException("segment is not inside the segment tree");
            }
            return (T)segment.Predecessor;
        }

        #endregion

        #region FirstSegment/LastSegment

        public T FirstSegment
        {
            get { return root == null ? null : (T)root.LeftMost; }
        }

        public T LastSegment
        {
            get { return root == null ? null : (T)root.RightMost; }
        }

        #endregion

        #region FindFirstSegmentWithStartAfter

        public T FindFirstSegmentWithStartAfter(int startOffset)
        {
            if (root == null)
            {
                return null;
            }
            if (startOffset <= 0)
            {
                return (T)root.LeftMost;
            }
            var s = FindNode(ref startOffset);

            while (startOffset == 0)
            {
                var p = (s == null) ? root.RightMost : s.Predecessor;

                Debug.Assert(p != null);
                startOffset += p.nodeLength;
                s = p;
            }
            return (T)s;
        }

        private TextSegment FindNode(ref int offset)
        {
            var n = root;
            while (true)
            {
                if (n.left != null)
                {
                    if (offset < n.left.totalNodeLength)
                    {
                        n = n.left;
                        continue;
                    }
                    offset -= n.left.totalNodeLength;
                }
                if (offset < n.nodeLength)
                {
                    return n;
                }
                offset -= n.nodeLength;
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

        #region FindOverlappingSegments

        public ReadOnlyCollection<T> FindSegmentsContaining(int offset)
        {
            return FindOverlappingSegments(offset, 0);
        }

        public ReadOnlyCollection<T> FindOverlappingSegments(ISegment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException("segment");
            }
            return FindOverlappingSegments(segment.Offset, segment.Length);
        }

        public ReadOnlyCollection<T> FindOverlappingSegments(int offset, int length)
        {
            ThrowUtil.CheckNotNegative(length, "length");
            var results = new List<T>();
            if (root != null)
            {
                FindOverlappingSegments(results, root, offset, offset + length);
            }
            return results.AsReadOnly();
        }

        private void FindOverlappingSegments(List<T> results, TextSegment node, int low, int high)
        {
            if (high < 0)
            {
                return;
            }

            int nodeLow = low - node.nodeLength;
            int nodeHigh = high - node.nodeLength;
            if (node.left != null)
            {
                nodeLow -= node.left.totalNodeLength;
                nodeHigh -= node.left.totalNodeLength;
            }

            if (node.distanceToMaxEnd < nodeLow)
            {
                return;
            }

            if (node.left != null)
            {
                FindOverlappingSegments(results, node.left, low, high);
            }

            if (nodeHigh < 0)
            {
                return;
            }

            if (nodeLow <= node.segmentLength)
            {
                results.Add((T)node);
            }

            if (node.right != null)
            {
                FindOverlappingSegments(results, node.right, nodeLow, nodeHigh);
            }
        }

        #endregion

        #region UpdateAugmentedData

        private void UpdateAugmentedData(TextSegment node)
        {
            int totalLength = node.nodeLength;
            int distanceToMaxEnd = node.segmentLength;
            if (node.left != null)
            {
                totalLength += node.left.totalNodeLength;

                int leftDTME = node.left.distanceToMaxEnd;

                if (node.left.right != null)
                {
                    leftDTME -= node.left.right.totalNodeLength;
                }
                leftDTME -= node.nodeLength;
                if (leftDTME > distanceToMaxEnd)
                {
                    distanceToMaxEnd = leftDTME;
                }
            }
            if (node.right != null)
            {
                totalLength += node.right.totalNodeLength;

                int rightDTME = node.right.distanceToMaxEnd;

                rightDTME += node.right.nodeLength;
                if (node.right.left != null)
                {
                    rightDTME += node.right.left.totalNodeLength;
                }
                if (rightDTME > distanceToMaxEnd)
                {
                    distanceToMaxEnd = rightDTME;
                }
            }
            if (node.totalNodeLength != totalLength
                || node.distanceToMaxEnd != distanceToMaxEnd)
            {
                node.totalNodeLength = totalLength;
                node.distanceToMaxEnd = distanceToMaxEnd;
                if (node.parent != null)
                {
                    UpdateAugmentedData(node.parent);
                }
            }
        }

        void ISegmentTree.UpdateAugmentedData(TextSegment node)
        {
            UpdateAugmentedData(node);
        }

        #endregion

        #region Remove

        public bool Remove(T item)
        {
            if (!Contains(item))
            {
                return false;
            }
            RemoveSegment(item);
            return true;
        }

        void ISegmentTree.Remove(TextSegment s)
        {
            RemoveSegment(s);
        }

        private void RemoveSegment(TextSegment s)
        {
            int oldOffset = s.StartOffset;
            var successor = s.Successor;
            if (successor != null)
            {
                successor.nodeLength += s.nodeLength;
            }
            RemoveNode(s);
            if (successor != null)
            {
                UpdateAugmentedData(successor);
            }
            Disconnect(s, oldOffset);
            CheckProperties();
        }

        private void Disconnect(TextSegment s, int offset)
        {
            s.left = s.right = s.parent = null;
            s.ownerTree = null;
            s.nodeLength = offset;
            count--;
        }

        public void Clear()
        {
            var segments = this.ToArray();
            root = null;
            int offset = 0;
            foreach (TextSegment s in segments)
            {
                offset += s.nodeLength;
                Disconnect(s, offset);
            }
            CheckProperties();
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

            int expectedCount = 0;

            using (var en = GetEnumerator())
            {
                while (en.MoveNext())
                {
                    expectedCount++;
                }
            }
            Debug.Assert(count == expectedCount);
#endif
        }

#if DEBUG
        private void CheckProperties(TextSegment node)
        {
            int totalLength = node.nodeLength;
            int distanceToMaxEnd = node.segmentLength;
            if (node.left != null)
            {
                CheckProperties(node.left);
                totalLength += node.left.totalNodeLength;
                distanceToMaxEnd = Math.Max(distanceToMaxEnd,
                    node.left.distanceToMaxEnd + node.left.StartOffset - node.StartOffset);
            }
            if (node.right != null)
            {
                CheckProperties(node.right);
                totalLength += node.right.totalNodeLength;
                distanceToMaxEnd = Math.Max(distanceToMaxEnd,
                    node.right.distanceToMaxEnd + node.right.StartOffset - node.StartOffset);
            }
            Debug.Assert(node.totalNodeLength == totalLength);
            Debug.Assert(node.distanceToMaxEnd == distanceToMaxEnd);
        }

        private void CheckNodeProperties(TextSegment node, TextSegment parentNode, bool parentColor, int blackCount, ref int expectedBlackCount)
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

        private static void AppendTreeToString(TextSegment node, StringBuilder b, int indent)
        {
            if (node.color == RED)
            {
                b.Append("RED   ");
            }
            else
            {
                b.Append("BLACK ");
            }
            b.AppendLine(node + node.ToDebugString());
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

        internal string GetTreeAsString()
        {
#if DEBUG
            var b = new StringBuilder();
            if (root != null)
            {
                AppendTreeToString(root, b, 0);
            }
            return b.ToString();
#else
			return "Not available in release build.";
#endif
        }

        #endregion

        #region Red/Black Tree

        internal const bool RED = true;

        internal const bool BLACK = false;

        private void InsertAsLeft(TextSegment parentNode, TextSegment newNode)
        {
            Debug.Assert(parentNode.left == null);
            parentNode.left = newNode;
            newNode.parent = parentNode;
            newNode.color = RED;
            UpdateAugmentedData(parentNode);
            FixTreeOnInsert(newNode);
        }

        private void InsertAsRight(TextSegment parentNode, TextSegment newNode)
        {
            Debug.Assert(parentNode.right == null);
            parentNode.right = newNode;
            newNode.parent = parentNode;
            newNode.color = RED;
            UpdateAugmentedData(parentNode);
            FixTreeOnInsert(newNode);
        }

        private void FixTreeOnInsert(TextSegment node)
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

        private void RemoveNode(TextSegment removedNode)
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

        private void FixTreeOnDelete(TextSegment node, TextSegment parentNode)
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

        private void ReplaceNode(TextSegment replacedNode, TextSegment newNode)
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

        private void RotateLeft(TextSegment p)
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

        private void RotateRight(TextSegment p)
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

        private static TextSegment Sibling(TextSegment node)
        {
            if (node == node.parent.left)
            {
                return node.parent.right;
            }
            return node.parent.left;
        }

        private static TextSegment Sibling(TextSegment node, TextSegment parentNode)
        {
            Debug.Assert(node == null || node.parent == parentNode);
            if (node == parentNode.left)
            {
                return parentNode.right;
            }
            return parentNode.left;
        }

        private static bool GetColor(TextSegment node)
        {
            return node != null ? node.color : BLACK;
        }

        #endregion

        #region ICollection<T> implementation

        public int Count
        {
            get { return count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public bool Contains(T item)
        {
            return item != null && item.ownerTree == this;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Length < Count)
            {
                throw new ArgumentException("The array is too small", "array");
            }
            if (arrayIndex < 0 || arrayIndex + count > array.Length)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Value must be between 0 and " + (array.Length - count));
            }
            foreach (T s in this)
            {
                array[arrayIndex++] = s;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (root != null)
            {
                var current = root.LeftMost;
                while (current != null)
                {
                    yield return (T)current;

                    current = current.Successor;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}