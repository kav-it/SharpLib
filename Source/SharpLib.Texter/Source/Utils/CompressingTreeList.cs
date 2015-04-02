using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SharpLib.Texter.Utils
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "It's an IList<T> implementation")]
    public sealed class CompressingTreeList<T> : IList<T>
    {
        #region Node definition

        private sealed class Node
        {
            #region Поля

            internal bool color;

            internal int count;

            internal Node left;

            internal Node parent;

            internal Node right;

            internal int totalCount;

            internal T value;

            #endregion

            #region Свойства

            internal Node LeftMost
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

            internal Node RightMost
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

            internal Node Predecessor
            {
                get
                {
                    if (left != null)
                    {
                        return left.RightMost;
                    }
                    var node = this;
                    Node oldNode;
                    do
                    {
                        oldNode = node;
                        node = node.parent;
                    } while (node != null && node.left == oldNode);
                    return node;
                }
            }

            internal Node Successor
            {
                get
                {
                    if (right != null)
                    {
                        return right.LeftMost;
                    }
                    var node = this;
                    Node oldNode;
                    do
                    {
                        oldNode = node;
                        node = node.parent;
                    } while (node != null && node.right == oldNode);
                    return node;
                }
            }

            #endregion

            #region Конструктор

            public Node(T value, int count)
            {
                this.value = value;
                this.count = count;
                totalCount = count;
            }

            #endregion

            #region Методы

            public override string ToString()
            {
                return "[TotalCount=" + totalCount + " Count=" + count + " Value=" + value + "]";
            }

            #endregion
        }

        #endregion

        #region Fields and Constructor

        private readonly Func<T, T, bool> comparisonFunc;

        private Node root;

        public CompressingTreeList(IEqualityComparer<T> equalityComparer)
        {
            if (equalityComparer == null)
            {
                throw new ArgumentNullException("equalityComparer");
            }
            comparisonFunc = equalityComparer.Equals;
        }

        public CompressingTreeList(Func<T, T, bool> comparisonFunc)
        {
            if (comparisonFunc == null)
            {
                throw new ArgumentNullException("comparisonFunc");
            }
            this.comparisonFunc = comparisonFunc;
        }

        #endregion

        #region InsertRange

        public void InsertRange(int index, int count, T item)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + Count);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", count, "Value must not be negative");
            }
            if (count == 0)
            {
                return;
            }
            unchecked
            {
                if (Count + count < 0)
                {
                    throw new OverflowException("Cannot insert elements: total number of elements must not exceed int.MaxValue.");
                }
            }

            if (root == null)
            {
                root = new Node(item, count);
            }
            else
            {
                var n = GetNode(ref index);

                if (comparisonFunc(n.value, item))
                {
                    n.count += count;
                    UpdateAugmentedData(n);
                }
                else if (index == n.count)
                {
                    Debug.Assert(n == root.RightMost);
                    InsertAsRight(n, new Node(item, count));
                }
                else if (index == 0)
                {
                    var p = n.Predecessor;
                    if (p != null && comparisonFunc(p.value, item))
                    {
                        p.count += count;
                        UpdateAugmentedData(p);
                    }
                    else
                    {
                        InsertBefore(n, new Node(item, count));
                    }
                }
                else
                {
                    Debug.Assert(index > 0 && index < n.count);

                    n.count -= index;
                    InsertBefore(n, new Node(n.value, index));

                    InsertBefore(n, new Node(item, count));
                    UpdateAugmentedData(n);
                }
            }
            CheckProperties();
        }

        private void InsertBefore(Node node, Node newNode)
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

        #region RemoveRange

        public void RemoveRange(int index, int count)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + Count);
            }
            if (count < 0 || index + count > Count)
            {
                throw new ArgumentOutOfRangeException("count", count, "0 <= length, index(" + index + ")+count <= " + Count);
            }
            if (count == 0)
            {
                return;
            }

            var n = GetNode(ref index);
            if (index + count < n.count)
            {
                n.count -= count;
                UpdateAugmentedData(n);
            }
            else
            {
                Node firstNodeBeforeDeletedRange;
                if (index > 0)
                {
                    count -= (n.count - index);
                    n.count = index;
                    UpdateAugmentedData(n);
                    firstNodeBeforeDeletedRange = n;
                    n = n.Successor;
                }
                else
                {
                    Debug.Assert(index == 0);
                    firstNodeBeforeDeletedRange = n.Predecessor;
                }
                while (n != null && count >= n.count)
                {
                    count -= n.count;
                    var s = n.Successor;
                    RemoveNode(n);
                    n = s;
                }
                if (count > 0)
                {
                    Debug.Assert(n != null && count < n.count);
                    n.count -= count;
                    UpdateAugmentedData(n);
                }
                if (n != null)
                {
                    Debug.Assert(n.Predecessor == firstNodeBeforeDeletedRange);
                    if (firstNodeBeforeDeletedRange != null && comparisonFunc(firstNodeBeforeDeletedRange.value, n.value))
                    {
                        firstNodeBeforeDeletedRange.count += n.count;
                        RemoveNode(n);
                        UpdateAugmentedData(firstNodeBeforeDeletedRange);
                    }
                }
            }

            CheckProperties();
        }

        #endregion

        #region SetRange

        public void SetRange(int index, int count, T item)
        {
            RemoveRange(index, count);
            InsertRange(index, count, item);
        }

        #endregion

        #region GetNode

        private Node GetNode(ref int index)
        {
            var node = root;
            while (true)
            {
                if (node.left != null && index < node.left.totalCount)
                {
                    node = node.left;
                }
                else
                {
                    if (node.left != null)
                    {
                        index -= node.left.totalCount;
                    }
                    if (index < node.count || node.right == null)
                    {
                        return node;
                    }
                    index -= node.count;
                    node = node.right;
                }
            }
        }

        #endregion

        #region UpdateAugmentedData

        private void UpdateAugmentedData(Node node)
        {
            int totalCount = node.count;
            if (node.left != null)
            {
                totalCount += node.left.totalCount;
            }
            if (node.right != null)
            {
                totalCount += node.right.totalCount;
            }
            if (node.totalCount != totalCount)
            {
                node.totalCount = totalCount;
                if (node.parent != null)
                {
                    UpdateAugmentedData(node.parent);
                }
            }
        }

        #endregion

        #region IList<T> implementation

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + (Count - 1));
                }
                return GetNode(ref index).value;
            }
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public int Count
        {
            get
            {
                if (root != null)
                {
                    return root.totalCount;
                }
                return 0;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(T item)
        {
            int index = 0;
            if (root != null)
            {
                var n = root.LeftMost;
                while (n != null)
                {
                    if (comparisonFunc(n.value, item))
                    {
                        return index;
                    }
                    index += n.count;
                    n = n.Successor;
                }
            }
            Debug.Assert(index == Count);
            return -1;
        }

        public int GetStartOfRun(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + (Count - 1));
            }
            int indexInRun = index;
            GetNode(ref indexInRun);
            return index - indexInRun;
        }

        public int GetEndOfRun(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + (Count - 1));
            }
            int indexInRun = index;
            int runLength = GetNode(ref indexInRun).count;
            return index - indexInRun + runLength;
        }

        [Obsolete("This method may be confusing as it returns only the remaining run length after index. " +
                  "Use GetStartOfRun/GetEndOfRun instead.")]
        public int GetRunLength(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index", index, "Value must be between 0 and " + (Count - 1));
            }
            return GetNode(ref index).count - index;
        }

        public void Transform(Func<T, T> converter)
        {
            if (root == null)
            {
                return;
            }
            Node prevNode = null;
            for (var n = root.LeftMost; n != null; n = n.Successor)
            {
                n.value = converter(n.value);
                if (prevNode != null && comparisonFunc(prevNode.value, n.value))
                {
                    n.count += prevNode.count;
                    UpdateAugmentedData(n);
                    RemoveNode(prevNode);
                }
                prevNode = n;
            }
            CheckProperties();
        }

        public void TransformRange(int index, int length, Func<T, T> converter)
        {
            if (root == null)
            {
                return;
            }
            int endIndex = index + length;
            int pos = index;
            while (pos < endIndex)
            {
                int endPos = Math.Min(endIndex, GetEndOfRun(pos));
                var oldValue = this[pos];
                var newValue = converter(oldValue);
                SetRange(pos, endPos - pos, newValue);
                pos = endPos;
            }
        }

        public void Insert(int index, T item)
        {
            InsertRange(index, 1, item);
        }

        public void RemoveAt(int index)
        {
            RemoveRange(index, 1);
        }

        public void Add(T item)
        {
            InsertRange(Count, 1, item);
        }

        public void Clear()
        {
            root = null;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
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
            if (arrayIndex < 0 || arrayIndex + Count > array.Length)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Value must be between 0 and " + (array.Length - Count));
            }
            foreach (T v in this)
            {
                array[arrayIndex++] = v;
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            if (root != null)
            {
                var n = root.LeftMost;
                while (n != null)
                {
                    for (int i = 0; i < n.count; i++)
                    {
                        yield return n.value;
                    }
                    n = n.Successor;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Red/Black Tree

        internal const bool RED = true;

        internal const bool BLACK = false;

        private void InsertAsLeft(Node parentNode, Node newNode)
        {
            Debug.Assert(parentNode.left == null);
            parentNode.left = newNode;
            newNode.parent = parentNode;
            newNode.color = RED;
            UpdateAugmentedData(parentNode);
            FixTreeOnInsert(newNode);
        }

        private void InsertAsRight(Node parentNode, Node newNode)
        {
            Debug.Assert(parentNode.right == null);
            parentNode.right = newNode;
            newNode.parent = parentNode;
            newNode.color = RED;
            UpdateAugmentedData(parentNode);
            FixTreeOnInsert(newNode);
        }

        private void FixTreeOnInsert(Node node)
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

        private void RemoveNode(Node removedNode)
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

        private void FixTreeOnDelete(Node node, Node parentNode)
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

        private void ReplaceNode(Node replacedNode, Node newNode)
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

        private void RotateLeft(Node p)
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

        private void RotateRight(Node p)
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

        private static Node Sibling(Node node)
        {
            if (node == node.parent.left)
            {
                return node.parent.right;
            }
            return node.parent.left;
        }

        private static Node Sibling(Node node, Node parentNode)
        {
            Debug.Assert(node == null || node.parent == parentNode);
            if (node == parentNode.left)
            {
                return parentNode.right;
            }
            return parentNode.left;
        }

        private static bool GetColor(Node node)
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

                var p = root.LeftMost;
                var n = p.Successor;
                while (n != null)
                {
                    Debug.Assert(!comparisonFunc(p.value, n.value));
                    p = n;
                    n = p.Successor;
                }
            }
#endif
        }

#if DEBUG
        private void CheckProperties(Node node)
        {
            Debug.Assert(node.count > 0);
            int totalCount = node.count;
            if (node.left != null)
            {
                CheckProperties(node.left);
                totalCount += node.left.totalCount;
            }
            if (node.right != null)
            {
                CheckProperties(node.right);
                totalCount += node.right.totalCount;
            }
            Debug.Assert(node.totalCount == totalCount);
        }

        private void CheckNodeProperties(Node node, Node parentNode, bool parentColor, int blackCount, ref int expectedBlackCount)
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

        internal string GetTreeAsString()
        {
#if DEBUG
            if (root == null)
            {
                return "<empty tree>";
            }
            var b = new StringBuilder();
            AppendTreeToString(root, b, 0);
            return b.ToString();
#else
			return "Not available in release build.";
#endif
        }

#if DEBUG
        private static void AppendTreeToString(Node node, StringBuilder b, int indent)
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