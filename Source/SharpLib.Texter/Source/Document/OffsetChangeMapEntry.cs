using System;

using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Document
{
    [Serializable]
    public struct OffsetChangeMapEntry : IEquatable<OffsetChangeMapEntry>
    {
        #region Поля

        private readonly uint insertionLengthWithMovementFlag;

        private readonly int offset;

        private readonly uint removalLengthWithDeletionFlag;

        #endregion

        #region Свойства

        public int Offset
        {
            get { return offset; }
        }

        public int InsertionLength
        {
            get { return (int)(insertionLengthWithMovementFlag & 0x7fffffff); }
        }

        public int RemovalLength
        {
            get { return (int)(removalLengthWithDeletionFlag & 0x7fffffff); }
        }

        public bool RemovalNeverCausesAnchorDeletion
        {
            get { return (removalLengthWithDeletionFlag & 0x80000000) != 0; }
        }

        public bool DefaultAnchorMovementIsBeforeInsertion
        {
            get { return (insertionLengthWithMovementFlag & 0x80000000) != 0; }
        }

        #endregion

        #region Конструктор

        public OffsetChangeMapEntry(int offset, int removalLength, int insertionLength)
        {
            ThrowUtil.CheckNotNegative(offset, "offset");
            ThrowUtil.CheckNotNegative(removalLength, "removalLength");
            ThrowUtil.CheckNotNegative(insertionLength, "insertionLength");

            this.offset = offset;
            removalLengthWithDeletionFlag = (uint)removalLength;
            insertionLengthWithMovementFlag = (uint)insertionLength;
        }

        public OffsetChangeMapEntry(int offset, int removalLength, int insertionLength, bool removalNeverCausesAnchorDeletion, bool defaultAnchorMovementIsBeforeInsertion)
            : this(offset, removalLength, insertionLength)
        {
            if (removalNeverCausesAnchorDeletion)
            {
                removalLengthWithDeletionFlag |= 0x80000000;
            }
            if (defaultAnchorMovementIsBeforeInsertion)
            {
                insertionLengthWithMovementFlag |= 0x80000000;
            }
        }

        #endregion

        #region Методы

        public int GetNewOffset(int oldOffset, AnchorMovementType movementType = AnchorMovementType.Default)
        {
            int insertionLength = InsertionLength;
            int removalLength = RemovalLength;
            if (!(removalLength == 0 && oldOffset == offset))
            {
                if (oldOffset <= offset)
                {
                    return oldOffset;
                }

                if (oldOffset >= offset + removalLength)
                {
                    return oldOffset + insertionLength - removalLength;
                }
            }

            if (movementType == AnchorMovementType.AfterInsertion)
            {
                return offset + insertionLength;
            }
            if (movementType == AnchorMovementType.BeforeInsertion)
            {
                return offset;
            }
            return DefaultAnchorMovementIsBeforeInsertion ? offset : offset + insertionLength;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return offset + 3559 * (int)insertionLengthWithMovementFlag + 3571 * (int)removalLengthWithDeletionFlag;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is OffsetChangeMapEntry && Equals((OffsetChangeMapEntry)obj);
        }

        public bool Equals(OffsetChangeMapEntry other)
        {
            return offset == other.offset && insertionLengthWithMovementFlag == other.insertionLengthWithMovementFlag && removalLengthWithDeletionFlag == other.removalLengthWithDeletionFlag;
        }

        #endregion

        public static bool operator ==(OffsetChangeMapEntry left, OffsetChangeMapEntry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OffsetChangeMapEntry left, OffsetChangeMapEntry right)
        {
            return !left.Equals(right);
        }
    }
}