using System;
using System.Diagnostics;
using System.Globalization;

#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#endif

namespace SharpLib.Notepad.Document
{
    public sealed partial class DocumentLine : IDocumentLine
    {
        #region Constructor

#if DEBUG

        private readonly TextDocument document;
#endif

        internal bool isDeleted;

        internal DocumentLine(TextDocument document)
        {
#if DEBUG
            Debug.Assert(document != null);
            this.document = document;
#endif
        }

        [Conditional("DEBUG")]
        private void DebugVerifyAccess()
        {
#if DEBUG
            document.DebugVerifyAccess();
#endif
        }

        #endregion

        #region Events

        #endregion

        #region Properties stored in tree

        public bool IsDeleted
        {
            get
            {
                DebugVerifyAccess();
                return isDeleted;
            }
        }

        public int LineNumber
        {
            get
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException();
                }
                return DocumentLineTree.GetIndexFromNode(this) + 1;
            }
        }

        public int Offset
        {
            get
            {
                if (IsDeleted)
                {
                    throw new InvalidOperationException();
                }
                return DocumentLineTree.GetOffsetFromNode(this);
            }
        }

        public int EndOffset
        {
            get { return Offset + Length; }
        }

        #endregion

        #region Length

        private int totalLength;

        private byte delimiterLength;

        public int Length
        {
            get
            {
                DebugVerifyAccess();
                return totalLength - delimiterLength;
            }
        }

        public int TotalLength
        {
            get
            {
                DebugVerifyAccess();
                return totalLength;
            }
            internal set { totalLength = value; }
        }

        public int DelimiterLength
        {
            get
            {
                DebugVerifyAccess();
                return delimiterLength;
            }
            internal set
            {
                Debug.Assert(value >= 0 && value <= 2);
                delimiterLength = (byte)value;
            }
        }

        #endregion

        #region Previous / Next Line

        public DocumentLine NextLine
        {
            get
            {
                DebugVerifyAccess();

                if (right != null)
                {
                    return right.LeftMost;
                }
                var node = this;
                DocumentLine oldNode;
                do
                {
                    oldNode = node;
                    node = node.parent;
                } while (node != null && node.right == oldNode);
                return node;
            }
        }

        public DocumentLine PreviousLine
        {
            get
            {
                DebugVerifyAccess();

                if (left != null)
                {
                    return left.RightMost;
                }
                var node = this;
                DocumentLine oldNode;
                do
                {
                    oldNode = node;
                    node = node.parent;
                } while (node != null && node.left == oldNode);
                return node;
            }
        }

        IDocumentLine IDocumentLine.NextLine
        {
            get { return NextLine; }
        }

        IDocumentLine IDocumentLine.PreviousLine
        {
            get { return PreviousLine; }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            if (IsDeleted)
            {
                return "[DocumentLine deleted]";
            }
            return string.Format(
                CultureInfo.InvariantCulture,
                "[DocumentLine Number={0} Offset={1} Length={2}]", LineNumber, Offset, Length);
        }

        #endregion
    }
}