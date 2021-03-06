﻿using SharpLib.Texter.Document;

namespace SharpLib.Texter.Highlighting
{
    public class HighlightedSection : ISegment
    {
        #region Свойства

        public int Offset { get; set; }

        public int Length { get; set; }

        int ISegment.EndOffset
        {
            get { return Offset + Length; }
        }

        public HighlightingColor Color { get; set; }

        #endregion

        #region Методы

        public override string ToString()
        {
            return string.Format("[HighlightedSection ({0}-{1})={2}]", Offset, Offset + Length, Color);
        }

        #endregion
    }
}