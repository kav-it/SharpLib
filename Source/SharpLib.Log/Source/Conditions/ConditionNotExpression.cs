namespace NLog.Conditions
{
    internal sealed class ConditionNotExpression : ConditionExpression
    {
        #region ��������

        public ConditionExpression Expression { get; private set; }

        #endregion

        #region �����������

        public ConditionNotExpression(ConditionExpression expression)
        {
            Expression = expression;
        }

        #endregion

        #region ������

        public override string ToString()
        {
            return "(not " + Expression + ")";
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            return !(bool)Expression.Evaluate(context);
        }

        #endregion
    }
}