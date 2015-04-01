using System;
using System.Text.RegularExpressions;

namespace SharpLib.Notepad.Highlighting
{
    [Serializable]
    public class HighlightingSpan
    {
        #region Свойства

        public Regex StartExpression { get; set; }

        public Regex EndExpression { get; set; }

        public HighlightingRuleSet RuleSet { get; set; }

        public HighlightingColor StartColor { get; set; }

        public HighlightingColor SpanColor { get; set; }

        public HighlightingColor EndColor { get; set; }

        public bool SpanColorIncludesStart { get; set; }

        public bool SpanColorIncludesEnd { get; set; }

        #endregion

        #region Методы

        public override string ToString()
        {
            return "[" + GetType().Name + " Start=" + StartExpression + ", End=" + EndExpression + "]";
        }

        #endregion
    }
}