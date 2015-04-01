using System;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd
{
    [Serializable]
    public class XshdImport : XshdElement
    {
        #region Свойства

        public XshdReference<XshdRuleSet> RuleSetReference { get; set; }

        #endregion

        #region Методы

        public override object AcceptVisitor(IXshdVisitor visitor)
        {
            return visitor.VisitImport(this);
        }

        #endregion
    }
}