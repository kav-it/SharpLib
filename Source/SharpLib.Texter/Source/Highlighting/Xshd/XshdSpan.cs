using System;

namespace SharpLib.Texter.Highlighting.Xshd
{
    [Serializable]
    public class XshdSpan : XshdElement
    {
        #region Свойства

        public string BeginRegex { get; set; }

        public XshdRegexType BeginRegexType { get; set; }

        public string EndRegex { get; set; }

        public XshdRegexType EndRegexType { get; set; }

        public bool Multiline { get; set; }

        public XshdReference<XshdRuleSet> RuleSetReference { get; set; }

        public XshdReference<XshdColor> SpanColorReference { get; set; }

        public XshdReference<XshdColor> BeginColorReference { get; set; }

        public XshdReference<XshdColor> EndColorReference { get; set; }

        #endregion

        #region Методы

        public override object AcceptVisitor(IXshdVisitor visitor)
        {
            return visitor.VisitSpan(this);
        }

        #endregion
    }
}