namespace NLog.Conditions
{
    internal sealed class ConditionLevelExpression : ConditionExpression
    {
        #region ������

        public override string ToString()
        {
            return "level";
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            return context.Level;
        }

        #endregion
    }
}