﻿using System;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Folding
{
    internal class NewFolding : ISegment
    {
        #region Свойства

        public int StartOffset { get; set; }

        public int EndOffset { get; set; }

        public string Name { get; set; }

        public bool DefaultClosed { get; set; }

        public bool IsDefinition { get; set; }

        int ISegment.Offset
        {
            get { return StartOffset; }
        }

        int ISegment.Length
        {
            get { return EndOffset - StartOffset; }
        }

        #endregion

        #region Конструктор

        public NewFolding()
        {
        }

        public NewFolding(int start, int end)
        {
            if (!(start <= end))
            {
                throw new ArgumentException("'start' must be less than 'end'");
            }
            StartOffset = start;
            EndOffset = end;
            Name = null;
            DefaultClosed = false;
        }

        #endregion
    }
}