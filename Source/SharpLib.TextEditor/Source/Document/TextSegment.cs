using System;
using System.Diagnostics;

#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#endif

namespace ICSharpCode.AvalonEdit.Document
{
    public class TextSegment : ISegment
    {
        internal ISegmentTree ownerTree;

        internal TextSegment left, right, parent;

        internal bool color;

        internal int nodeLength;

        internal int totalNodeLength;

        internal int segmentLength;

        internal int distanceToMaxEnd;

        int ISegment.Offset
        {
            get { return StartOffset; }
        }

        protected bool IsConnectedToCollection
        {
            get { return ownerTree != null; }
        }

        public int StartOffset
        {
            get
            {
                Debug.Assert(!(ownerTree == null && parent != null));
                Debug.Assert(!(ownerTree == null && left != null));

                var n = this;
                int offset = n.nodeLength;
                if (n.left != null)
                {
                    offset += n.left.totalNodeLength;
                }
                while (n.parent != null)
                {
                    if (n == n.parent.right)
                    {
                        if (n.parent.left != null)
                        {
                            offset += n.parent.left.totalNodeLength;
                        }
                        offset += n.parent.nodeLength;
                    }
                    n = n.parent;
                }
                return offset;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Offset must not be negative");
                }
                if (StartOffset != value)
                {
                    var ownerTree = this.ownerTree;
                    if (ownerTree != null)
                    {
                        ownerTree.Remove(this);
                        nodeLength = value;
                        ownerTree.Add(this);
                    }
                    else
                    {
                        nodeLength = value;
                    }
                    OnSegmentChanged();
                }
            }
        }

        public int EndOffset
        {
            get { return StartOffset + Length; }
            set
            {
                int newLength = value - StartOffset;
                if (newLength < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "EndOffset must be greater or equal to StartOffset");
                }
                Length = newLength;
            }
        }

        public int Length
        {
            get { return segmentLength; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Length must not be negative");
                }
                if (segmentLength != value)
                {
                    segmentLength = value;
                    if (ownerTree != null)
                    {
                        ownerTree.UpdateAugmentedData(this);
                    }
                    OnSegmentChanged();
                }
            }
        }

        protected virtual void OnSegmentChanged()
        {
        }

        internal TextSegment LeftMost
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

        internal TextSegment RightMost
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

        internal TextSegment Successor
        {
            get
            {
                if (right != null)
                {
                    return right.LeftMost;
                }
                var node = this;
                TextSegment oldNode;
                do
                {
                    oldNode = node;
                    node = node.parent;
                } while (node != null && node.right == oldNode);
                return node;
            }
        }

        internal TextSegment Predecessor
        {
            get
            {
                if (left != null)
                {
                    return left.RightMost;
                }
                var node = this;
                TextSegment oldNode;
                do
                {
                    oldNode = node;
                    node = node.parent;
                } while (node != null && node.left == oldNode);
                return node;
            }
        }

#if DEBUG
        internal string ToDebugString()
        {
            return "[nodeLength=" + nodeLength + " totalNodeLength=" + totalNodeLength
                   + " distanceToMaxEnd=" + distanceToMaxEnd + " MaxEndOffset=" + (StartOffset + distanceToMaxEnd) + "]";
        }
#endif

        public override string ToString()
        {
            return "[" + GetType().Name + " Offset=" + StartOffset + " Length=" + Length + " EndOffset=" + EndOffset + "]";
        }
    }
}