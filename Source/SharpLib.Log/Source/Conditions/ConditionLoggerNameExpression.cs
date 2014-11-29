namespace NLog.Conditions
{
    internal sealed class ConditionLoggerNameExpression : ConditionExpression
    {
        #region Ìועמהû

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