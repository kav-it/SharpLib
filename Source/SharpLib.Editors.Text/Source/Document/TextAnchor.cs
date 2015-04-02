using System;

using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Document
{
    public sealed class TextAnchor : ITextAnchor
    {
        #region Поля

        private readonly TextDocument document;

        internal TextAnchorNode node;

        #endregion

        #region Свойства

        public TextDocument Document
        {
            get { return document; }
        }

        public AnchorMovementType MovementType { get; set; }

        public bool SurviveDeletion { get; set; }

        public bool IsDeleted
        {
            get
            {
                document.DebugVerifyAccess();
                return node == null;
            }
        }

        public int Offset
        {
            get
            {
                document.DebugVerifyAccess();

                var n = node;
                if (n == null)
                {
                    throw new InvalidOperationException();
                }

                int offset = n.length;
                if (n.left != null)
                {
                    offset += n.left.totalLength;
                }
                while (n.parent != null)
                {
                    if (n == n.parent.right)
                    {
                        if (n.parent.left != null)
                        {
                            offset += n.parent.left.totalLength;
                        }
                        offset += n.parent.length;
                    }
                    n = n.parent;
                }
                return offset;
            }
        }

        public int Line
        {
            get { return document.GetLineByOffset(Offset).LineNumber; }
        }

        public int Column
        {
            get
            {
                int offset = Offset;
                return offset - document.GetLineByOffset(offset).Offset + 1;
            }
        }

        public TextLocation Location
        {
            get { return document.GetLocation(Offset); }
        }

        #endregion

        #region События

        public event EventHandler Deleted;

        #endregion

        #region Конструктор

        internal TextAnchor(TextDocument document)
        {
            this.document = document;
        }

        #endregion

        #region Методы

        internal void OnDeleted(DelayedEvents delayedEvents)
        {
            node = null;
            delayedEvents.DelayedRaise(Deleted, this, EventArgs.Empty);
        }

        public override string ToString()
        {
            return "[TextAnchor Offset=" + Offset + "]";
        }

        #endregion
    }
}