using System;

namespace SharpLib.Notepad.Highlighting.Xshd
{
    [Serializable]
    public class XshdRule : XshdElement
    {
        #region Свойства

        public string Regex { get; set; }

        public XshdRegexType RegexType { get; set; }

        public XshdReference<XshdColor> ColorReference { get; set; }

        #endregion

        #region Методы

        public override object AcceptVisitor(IXshdVisitor visitor)
        {
            return visitor.VisitRule(this);
        }

        #endregion
    }
}