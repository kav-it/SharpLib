using System;
using System.Globalization;

namespace NLog.Conditions
{
    internal sealed class ConditionLiteralExpression : ConditionExpression
    {
        #region ��������

        public object LiteralValue { get; private set; }

        #endregion

        #region �����������

        public ConditionLiteralExpression(object literalValue)
        {
            LiteralValue = literalValue;
        }

        #endregion

        #region ������

        public override string ToString()
        {
            if (LiteralValue == null)
            {
                return "null";
            }

            return Convert.ToString(LiteralValue, CultureInfo.InvariantCulture);
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            return LiteralValue;
        }

        #endregion
    }
}