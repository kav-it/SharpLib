namespace NLog.Conditions
{
    internal sealed class ConditionAndExpression : ConditionExpression
    {
        #region ����

        private static readonly object boxedFalse = false;

        private static readonly object boxedTrue = true;

        #endregion

        #region ��������

        public ConditionExpression Left { get; private set; }

        public ConditionExpression Right { get; private set; }

        #endregion

        #region �����������

        public ConditionAndExpression(ConditionExpression left, ConditionExpression right)
        {
            Left = left;
            Right = right;
        }

        #endregion

        #region ������

        public override string ToString()
        {
            return "(" + Left + " and " + Right + ")";
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            var bval1 = (bool)Left.Evaluate(context);
            if (!bval1)
            {
                return boxedFalse;
            }

            var bval2 = (bool)Right.Evaluate(context);
            if (!bval2)
            {
                return boxedFalse;
            }

            return boxedTrue;
        }

        #endregion
    }
}