using System;

namespace SharpLib.Audio.Dmo
{
    internal class MediaObjectSizeInfo
    {
        #region Свойства

        public int Size { get; private set; }

        public int MaxLookahead { get; private set; }

        public int Alignment { get; private set; }

        #endregion

        #region Конструктор

        public MediaObjectSizeInfo(int size, int maxLookahead, int alignment)
        {
            Size = size;
            MaxLookahead = maxLookahead;
            Alignment = alignment;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return String.Format("Size: {0}, Alignment {1}, MaxLookahead {2}",
                Size, Alignment, MaxLookahead);
        }

        #endregion
    }
}