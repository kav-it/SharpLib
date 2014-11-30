
using System;
using System.Globalization;

namespace SharpLib.Log
{
    internal sealed class ConditionLiteralExpression : ConditionExpression
    {
        #region Свойства

        public object LiteralValue { get; private set; }

        #endregion

        #region Конструктор

        public ConditionLiteralExpression(object literalValue)
        {
            LiteralValue = literalValue;
        }

        #endregion

        #region Методы

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
