using System;

namespace ICSharpCode.AvalonEdit.Document
{
    [Serializable]
    public class DocumentChangeEventArgs : TextChangeEventArgs
    {
        #region Поля

        private volatile OffsetChangeMap offsetChangeMap;

        #endregion

        #region Свойства

        public OffsetChangeMap OffsetChangeMap
        {
            get
            {
                var map = offsetChangeMap;
                if (map == null)
                {
                    map = OffsetChangeMap.FromSingleElement(CreateSingleChangeMapEntry());
                    offsetChangeMap = map;
                }
                return map;
            }
        }

        internal OffsetChangeMap OffsetChangeMapOrNull
        {
            get { return offsetChangeMap; }
        }

        #endregion

        #region Конструктор

        public DocumentChangeEventArgs(int offset, string removedText, string insertedText)
            : this(offset, removedText, insertedText, null)
        {
        }

        public DocumentChangeEventArgs(int offset, string removedText, string insertedText, OffsetChangeMap offsetChangeMap)
            : base(offset, removedText, insertedText)
        {
            SetOffsetChangeMap(offsetChangeMap);
        }

        public DocumentChangeEventArgs(int offset, ITextSource removedText, ITextSource insertedText, OffsetChangeMap offsetChangeMap)
            : base(offset, removedText, insertedText)
        {
            SetOffsetChangeMap(offsetChangeMap);
        }

        #endregion

        #region Методы

        internal OffsetChangeMapEntry CreateSingleChangeMapEntry()
        {
            return new OffsetChangeMapEntry(Offset, RemovalLength, InsertionLength);
        }

        public override int GetNewOffset(int offset, AnchorMovementType movementType = AnchorMovementType.Default)
        {
            if (offsetChangeMap != null)
            {
                return offsetChangeMap.GetNewOffset(offset, movementType);
            }
            return CreateSingleChangeMapEntry().GetNewOffset(offset, movementType);
        }

        private void SetOffsetChangeMap(OffsetChangeMap offsetChangeMap)
        {
            if (offsetChangeMap != null)
            {
                if (!offsetChangeMap.IsFrozen)
                {
                    throw new ArgumentException("The OffsetChangeMap must be frozen before it can be used in DocumentChangeEventArgs");
                }
                if (!offsetChangeMap.IsValidForDocumentChange(Offset, RemovalLength, InsertionLength))
                {
                    throw new ArgumentException("OffsetChangeMap is not valid for this document change", "offsetChangeMap");
                }
                this.offsetChangeMap = offsetChangeMap;
            }
        }

        public override TextChangeEventArgs Invert()
        {
            var map = OffsetChangeMapOrNull;
            if (map != null)
            {
                map = map.Invert();
                map.Freeze();
            }
            return new DocumentChangeEventArgs(Offset, InsertedText, RemovedText, map);
        }

        #endregion
    }
}