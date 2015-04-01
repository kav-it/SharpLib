using System;
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.AvalonEdit.Utils
{
    [Serializable]
    internal class RopeNode<T>
    {
        internal const int NODE_SIZE = 256;

        internal static readonly RopeNode<T> emptyRopeNode = new RopeNode<T>
        {
            isShared = true,
            contents = new T[RopeNode<T>.NODE_SIZE]
        };

        internal RopeNode<T> left;

        internal RopeNode<T> right;

        internal volatile bool isShared;

        internal int length;

        internal byte height;

        internal T[] contents;

        internal int Balance
        {
            get { return right.height - left.height; }
        }

        [Conditional("DATACONSISTENCYTEST")]
        internal void CheckInvariants()
        {
            if (height == 0)
            {
                Debug.Assert(left == null && right == null);
                if (contents == null)
                {
                    Debug.Assert(this is FunctionNode<T>);
                    Debug.Assert(length > 0);
                    Debug.Assert(isShared);
                }
                else
                {
                    Debug.Assert(contents != null && contents.Length == NODE_SIZE);
                    Debug.Assert(length >= 0 && length <= NODE_SIZE);
                }
            }
            else
            {
                Debug.Assert(left != null && right != null);
                Debug.Assert(contents == null);
                Debug.Assert(length == left.length + right.length);
                Debug.Assert(height == 1 + Math.Max(left.height, right.height));
                Debug.Assert(Math.Abs(Balance) <= 1);

                Debug.Assert(length > NODE_SIZE);

                if (isShared)
                {
                    Debug.Assert(left.isShared && right.isShared);
                }
                left.CheckInvariants();
                right.CheckInvariants();
            }
        }

        internal RopeNode<T> Clone()
        {
            if (height == 0)
            {
                if (contents == null)
                {
                    return GetContentNode().Clone();
                }
                var newContents = new T[NODE_SIZE];
                contents.CopyTo(newContents, 0);
                return new RopeNode<T>
                {
                    length = length,
                    contents = newContents
                };
            }
            return new RopeNode<T>
            {
                left = left,
                right = right,
                length = length,
                height = height
            };
        }

        internal RopeNode<T> CloneIfShared()
        {
            if (isShared)
            {
                return Clone();
            }
            return this;
        }

        internal void Publish()
        {
            if (!isShared)
            {
                if (left != null)
                {
                    left.Publish();
                }
                if (right != null)
                {
                    right.Publish();
                }

                isShared = true;
            }
        }

        internal static RopeNode<T> CreateFromArray(T[] arr, int index, int length)
        {
            if (length == 0)
            {
                return emptyRopeNode;
            }
            var node = CreateNodes(length);
            return node.StoreElements(0, arr, index, length);
        }

        internal static RopeNode<T> CreateNodes(int totalLength)
        {
            int leafCount = (totalLength + NODE_SIZE - 1) / NODE_SIZE;
            return CreateNodes(leafCount, totalLength);
        }

        private static RopeNode<T> CreateNodes(int leafCount, int totalLength)
        {
            Debug.Assert(leafCount > 0);
            Debug.Assert(totalLength > 0);
            var result = new RopeNode<T>();
            result.length = totalLength;
            if (leafCount == 1)
            {
                result.contents = new T[NODE_SIZE];
            }
            else
            {
                int rightSide = leafCount / 2;
                int leftSide = leafCount - rightSide;
                int leftLength = leftSide * NODE_SIZE;
                result.left = CreateNodes(leftSide, leftLength);
                result.right = CreateNodes(rightSide, totalLength - leftLength);
                result.height = (byte)(1 + Math.Max(result.left.height, result.right.height));
            }
            return result;
        }

        internal void Rebalance()
        {
            Debug.Assert(!isShared);

            if (left == null)
            {
                return;
            }

            Debug.Assert(length > NODE_SIZE);

            while (Math.Abs(Balance) > 1)
            {
                if (Balance > 1)
                {
                    if (right.Balance < 0)
                    {
                        right = right.CloneIfShared();
                        right.RotateRight();
                    }
                    RotateLeft();

                    left.Rebalance();
                }
                else if (Balance < -1)
                {
                    if (left.Balance > 0)
                    {
                        left = left.CloneIfShared();
                        left.RotateLeft();
                    }
                    RotateRight();

                    right.Rebalance();
                }
            }

            Debug.Assert(Math.Abs(Balance) <= 1);
            height = (byte)(1 + Math.Max(left.height, right.height));
        }

        private void RotateLeft()
        {
            Debug.Assert(!isShared);

            var a = left;
            var b = right.left;
            var c = right.right;

            left = right.isShared ? new RopeNode<T>() : right;
            left.left = a;
            left.right = b;
            left.length = a.length + b.length;
            left.height = (byte)(1 + Math.Max(a.height, b.height));
            right = c;

            left.MergeIfPossible();
        }

        private void RotateRight()
        {
            Debug.Assert(!isShared);

            var a = left.left;
            var b = left.right;
            var c = right;

            right = left.isShared ? new RopeNode<T>() : left;
            right.left = b;
            right.right = c;
            right.length = b.length + c.length;
            right.height = (byte)(1 + Math.Max(b.height, c.height));
            left = a;

            right.MergeIfPossible();
        }

        private void MergeIfPossible()
        {
            Debug.Assert(!isShared);

            if (length <= NODE_SIZE)
            {
                height = 0;
                int lengthOnLeftSide = left.length;
                if (left.isShared)
                {
                    contents = new T[NODE_SIZE];
                    left.CopyTo(0, contents, 0, lengthOnLeftSide);
                }
                else
                {
                    Debug.Assert(left.contents != null);

                    contents = left.contents;
#if DEBUG

                    left.contents = Empty<T>.Array;
#endif
                }
                left = null;
                right.CopyTo(0, contents, lengthOnLeftSide, right.length);
                right = null;
            }
        }

        internal RopeNode<T> StoreElements(int index, T[] array, int arrayIndex, int count)
        {
            var result = CloneIfShared();

            if (result.height == 0)
            {
                Array.Copy(array, arrayIndex, result.contents, index, count);
            }
            else
            {
                if (index + count <= result.left.length)
                {
                    result.left = result.left.StoreElements(index, array, arrayIndex, count);
                }
                else if (index >= left.length)
                {
                    result.right = result.right.StoreElements(index - result.left.length, array, arrayIndex, count);
                }
                else
                {
                    int amountInLeft = result.left.length - index;
                    result.left = result.left.StoreElements(index, array, arrayIndex, amountInLeft);
                    result.right = result.right.StoreElements(0, array, arrayIndex + amountInLeft, count - amountInLeft);
                }
                result.Rebalance();
            }
            return result;
        }

        internal void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (height == 0)
            {
                if (contents == null)
                {
                    GetContentNode().CopyTo(index, array, arrayIndex, count);
                }
                else
                {
                    Array.Copy(contents, index, array, arrayIndex, count);
                }
            }
            else
            {
                if (index + count <= left.length)
                {
                    left.CopyTo(index, array, arrayIndex, count);
                }
                else if (index >= left.length)
                {
                    right.CopyTo(index - left.length, array, arrayIndex, count);
                }
                else
                {
                    int amountInLeft = left.length - index;
                    left.CopyTo(index, array, arrayIndex, amountInLeft);
                    right.CopyTo(0, array, arrayIndex + amountInLeft, count - amountInLeft);
                }
            }
        }

        internal RopeNode<T> SetElement(int offset, T value)
        {
            var result = CloneIfShared();

            if (result.height == 0)
            {
                result.contents[offset] = value;
            }
            else
            {
                if (offset < result.left.length)
                {
                    result.left = result.left.SetElement(offset, value);
                }
                else
                {
                    result.right = result.right.SetElement(offset - result.left.length, value);
                }
                result.Rebalance();
            }
            return result;
        }

        internal static RopeNode<T> Concat(RopeNode<T> left, RopeNode<T> right)
        {
            if (left.length == 0)
            {
                return right;
            }
            if (right.length == 0)
            {
                return left;
            }

            if (left.length + right.length <= NODE_SIZE)
            {
                left = left.CloneIfShared();

                right.CopyTo(0, left.contents, left.length, right.length);
                left.length += right.length;
                return left;
            }
            var concatNode = new RopeNode<T>();
            concatNode.left = left;
            concatNode.right = right;
            concatNode.length = left.length + right.length;
            concatNode.Rebalance();
            return concatNode;
        }

        private RopeNode<T> SplitAfter(int offset)
        {
            Debug.Assert(!isShared && height == 0 && contents != null);
            var newPart = new RopeNode<T>();
            newPart.contents = new T[NODE_SIZE];
            newPart.length = length - offset;
            Array.Copy(contents, offset, newPart.contents, 0, newPart.length);
            length = offset;
            return newPart;
        }

        internal RopeNode<T> Insert(int offset, RopeNode<T> newElements)
        {
            if (offset == 0)
            {
                return Concat(newElements, this);
            }
            if (offset == length)
            {
                return Concat(this, newElements);
            }

            var result = CloneIfShared();
            if (result.height == 0)
            {
                var left = result;
                var right = left.SplitAfter(offset);
                return Concat(Concat(left, newElements), right);
            }
            if (offset < result.left.length)
            {
                result.left = result.left.Insert(offset, newElements);
            }
            else
            {
                result.right = result.right.Insert(offset - result.left.length, newElements);
            }
            result.length += newElements.length;
            result.Rebalance();
            return result;
        }

        internal RopeNode<T> Insert(int offset, T[] array, int arrayIndex, int count)
        {
            Debug.Assert(count > 0);

            RopeNode<T> result;

            if (length + count < RopeNode<char>.NODE_SIZE)
            {
                result = CloneIfShared();

                int lengthAfterOffset = result.length - offset;
                var resultContents = result.contents;
                for (int i = lengthAfterOffset; i >= 0; i--)
                {
                    resultContents[i + offset + count] = resultContents[i + offset];
                }
                Array.Copy(array, arrayIndex, resultContents, offset, count);
                result.length += count;
                return result;
            }
            if (height == 0)
            {
                return Insert(offset, CreateFromArray(array, arrayIndex, count));
            }

            result = CloneIfShared();
            if (offset < result.left.length)
            {
                result.left = result.left.Insert(offset, array, arrayIndex, count);
            }
            else
            {
                result.right = result.right.Insert(offset - result.left.length, array, arrayIndex, count);
            }
            result.length += count;
            result.Rebalance();
            return result;
        }

        internal RopeNode<T> RemoveRange(int index, int count)
        {
            Debug.Assert(count > 0);

            if (index == 0 && count == length)
            {
                return emptyRopeNode;
            }

            int endIndex = index + count;
            var result = CloneIfShared();
            if (result.height == 0)
            {
                int remainingAfterEnd = result.length - endIndex;
                for (int i = 0; i < remainingAfterEnd; i++)
                {
                    result.contents[index + i] = result.contents[endIndex + i];
                }
                result.length -= count;
            }
            else
            {
                if (endIndex <= result.left.length)
                {
                    result.left = result.left.RemoveRange(index, count);
                }
                else if (index >= result.left.length)
                {
                    result.right = result.right.RemoveRange(index - result.left.length, count);
                }
                else
                {
                    int deletionAmountOnLeftSide = result.left.length - index;
                    result.left = result.left.RemoveRange(index, deletionAmountOnLeftSide);
                    result.right = result.right.RemoveRange(0, count - deletionAmountOnLeftSide);
                }

                if (result.left.length == 0)
                {
                    return result.right;
                }
                if (result.right.length == 0)
                {
                    return result.left;
                }

                result.length -= count;
                result.MergeIfPossible();
                result.Rebalance();
            }
            return result;
        }

        #region Debug Output

#if DEBUG
        internal virtual void AppendTreeToString(StringBuilder b, int indent)
        {
            b.AppendLine(ToString());
            indent += 2;
            if (left != null)
            {
                b.Append(' ', indent);
                b.Append("L: ");
                left.AppendTreeToString(b, indent);
            }
            if (right != null)
            {
                b.Append(' ', indent);
                b.Append("R: ");
                right.AppendTreeToString(b, indent);
            }
        }

        public override string ToString()
        {
            if (contents != null)
            {
                var charContents = contents as char[];
                if (charContents != null)
                {
                    return "[Leaf length=" + length + ", isShared=" + isShared + ", text=\"" + new string(charContents, 0, length) + "\"]";
                }
                return "[Leaf length=" + length + ", isShared=" + isShared + "\"]";
            }
            return "[Concat length=" + length + ", isShared=" + isShared + ", height=" + height + ", Balance=" + Balance + "]";
        }

        internal string GetTreeAsString()
        {
            var b = new StringBuilder();
            AppendTreeToString(b, 0);
            return b.ToString();
        }
#endif

        #endregion

        internal virtual RopeNode<T> GetContentNode()
        {
            throw new InvalidOperationException("Called GetContentNode() on non-FunctionNode.");
        }
    }

    internal sealed class FunctionNode<T> : RopeNode<T>
    {
        private Func<Rope<T>> initializer;

        private RopeNode<T> cachedResults;

        public FunctionNode(int length, Func<Rope<T>> initializer)
        {
            Debug.Assert(length > 0);
            Debug.Assert(initializer != null);

            this.length = length;
            this.initializer = initializer;

            isShared = true;
        }

        internal override RopeNode<T> GetContentNode()
        {
            lock (this)
            {
                if (cachedResults == null)
                {
                    if (initializer == null)
                    {
                        throw new InvalidOperationException("Trying to load this node recursively; or: a previous call to a rope initializer failed.");
                    }
                    var initializerCopy = initializer;
                    initializer = null;
                    var resultRope = initializerCopy();
                    if (resultRope == null)
                    {
                        throw new InvalidOperationException("Rope initializer returned null.");
                    }
                    var resultNode = resultRope.root;
                    resultNode.Publish();
                    if (resultNode.length != length)
                    {
                        throw new InvalidOperationException("Rope initializer returned rope with incorrect length.");
                    }
                    if (resultNode.height == 0 && resultNode.contents == null)
                    {
                        cachedResults = resultNode.GetContentNode();
                    }
                    else
                    {
                        cachedResults = resultNode;
                    }
                }
                return cachedResults;
            }
        }

#if DEBUG
        internal override void AppendTreeToString(StringBuilder b, int indent)
        {
            RopeNode<T> resultNode;
            lock (this)
            {
                b.AppendLine(ToString());
                resultNode = cachedResults;
            }
            indent += 2;
            if (resultNode != null)
            {
                b.Append(' ', indent);
                b.Append("C: ");
                resultNode.AppendTreeToString(b, indent);
            }
        }

        public override string ToString()
        {
            return "[FunctionNode length=" + length + " initializerRan=" + (initializer == null) + "]";
        }
#endif
    }
}