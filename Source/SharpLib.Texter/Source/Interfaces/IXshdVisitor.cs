namespace SharpLib.Texter.Highlighting.Xshd
{
    public interface IXshdVisitor
    {
        #region Методы

        object VisitRuleSet(XshdRuleSet ruleSet);

        object VisitColor(XshdColor color);

        object VisitKeywords(XshdKeywords keywords);

        object VisitSpan(XshdSpan span);

        object VisitImport(XshdImport import);

        object VisitRule(XshdRule rule);

        #endregion
    }
}