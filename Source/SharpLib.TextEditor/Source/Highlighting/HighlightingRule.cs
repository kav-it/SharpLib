﻿using System;
using System.Text.RegularExpressions;

namespace ICSharpCode.AvalonEdit.Highlighting
{
    [Serializable]
    public class HighlightingRule
    {
        #region Свойства

        public Regex Regex { get; set; }

        public HighlightingColor Color { get; set; }

        #endregion

        #region Методы

        public override string ToString()
        {
            return "[" + GetType().Name + " " + Regex + "]";
        }

        #endregion
    }
}