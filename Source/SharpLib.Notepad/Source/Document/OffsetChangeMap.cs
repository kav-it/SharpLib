using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using SharpLib.Notepad.Utils;

#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#endif

namespace SharpLib.Notepad.Document
{
    [Serializable]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "It's a mapping old offsets -> new offsets")]
    public sealed class OffsetChangeMap : Collection<OffsetChangeMapEntry>
    {
        #region Поля

        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "The Empty instance is immutable")]
        public static readonly OffsetChangeMap Empty = new OffsetChangeMap(Empty<OffsetChangeMapEntry>.Array, true);

        private bool isFrozen;

        #endregion

        #region Свойства

        public bool IsFrozen
        {
            get { return isFrozen; }
        }

        #endregion

        #region Конструктор

        public OffsetChangeMap()
        {
        }

        internal OffsetChangeMap(int capacity)
            : base(new List<OffsetChangeMapEntry>(capacity))
        {
        }

        private OffsetChangeMap(IList<OffsetChangeMapEntry> entries, bool isFrozen)
            : base(entries)
        {
            this.isFrozen = isFrozen;
        }

        #endregion

        #region Методы

        public static OffsetChangeMap FromSingleElement(OffsetChangeMapEntry entry)
        {
            return new OffsetChangeMap(new[] { entry }, true);
        }

        public int GetNewOffset(int offset, AnchorMovementType movementType = AnchorMovementType.Default)
        {
            var items = Items;
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                offset = items[i].GetNewOffset(offset, movementType);
            }
            return offset;
        }

        public bool IsValidForDocumentChange(int offset, int removalLength, int insertionLength)
        {
            int endOffset = offset + removalLength;
            foreach (OffsetChangeMapEntry entry in this)
            {
                if (entry.Offset < offset || entry.Offset + entry.RemovalLength > endOffset)
                {
                    return false;
                }
                endOffset += entry.InsertionLength - entry.RemovalLength;
            }

            return endOffset == offset + insertionLength;
        }

        public OffsetChangeMap Invert()
        {
            if (this == Empty)
            {
                return this;
            }
            var newMap = new OffsetChangeMap(Count);
            for (int i = Count - 1; i >= 0; i--)
            {
                var entry = this[i];

                newMap.Add(new OffsetChangeMapEntry(entry.Offset, entry.InsertionLength, entry.RemovalLength));
            }
            return newMap;
        }

        protected override void ClearItems()
        {
            CheckFrozen();
            base.ClearItems();
        }

        protected override void InsertItem(int index, OffsetChangeMapEntry item)
        {
            CheckFrozen();
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            CheckFrozen();
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, OffsetChangeMapEntry item)
        {
            CheckFrozen();
            base.SetItem(index, item);
        }

        private void CheckFrozen()
        {
            if (isFrozen)
            {
                throw new InvalidOperationException("This instance is frozen and cannot be modified.");
            }
        }

        public void Freeze()
        {
            isFrozen = true;
        }

        #endregion
    }
}