using System;
using System.Collections.Generic;

using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Highlighting
{
    [Serializable]
    public class HighlightingRuleSet
    {
        #region Свойства

        public string Name { get; set; }

        public IList<HighlightingSpan> Spans { get; private set; }

        public IList<HighlightingRule> Rules { get; private set; }

        #endregion

        #region Конструктор

        public HighlightingRuleSet()
        {
            Spans = new NullSafeCollection<HighlightingSpan>();
            Rules = new NullSafeCollection<HighlightingRule>();
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return "[" + GetType().Name + " " + Name + "]";
        }

        #endregion
    }
}