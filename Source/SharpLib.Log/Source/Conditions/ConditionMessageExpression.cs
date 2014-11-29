namespace NLog.Conditions
{
    internal sealed class ConditionMessageExpression : ConditionExpression
    {
        #region Ìועמהû

        public override string ToString()
        {
            return "message";
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            return context.FormattedMessage;
        }

        #endregion
    }
}