namespace NLog.Conditions
{
    internal sealed class ConditionLoggerNameExpression : ConditionExpression
    {
        #region ������

        public override string ToString()
        {
            return "logger";
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            return context.LoggerName;
        }

        #endregion
    }
}