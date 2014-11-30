
using System;

namespace SharpLib.Log
{
    internal sealed class ConditionNotExpression : ConditionExpression
    {
        #region Свойства

        public ConditionExpression Expression { get; private set; }

        #endregion

        #region Конструктор

        public ConditionNotExpression(ConditionExpression expression)
        {
            Expression = expression;
        }

        #endregion

        #region Методы

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
