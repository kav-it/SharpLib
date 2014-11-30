
using System;

namespace SharpLib.Log
{
    internal sealed class ConditionOrExpression : ConditionExpression
    {
        #region Поля

        private static readonly object boxedFalse = false;

        private static readonly object boxedTrue = true;

        #endregion

        #region Свойства

        public ConditionExpression LeftExpression { get; private set; }

        public ConditionExpression RightExpression { get; private set; }

        #endregion

        #region Конструктор

        public ConditionOrExpression(ConditionExpression left, ConditionExpression right)
        {
            LeftExpression = left;
            RightExpression = right;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return "(" + LeftExpression + " or " + RightExpression + ")";
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            var bval1 = (bool)LeftExpression.Evaluate(context);
            if (bval1)
            {
                return boxedTrue;
            }

            var bval2 = (bool)RightExpression.Evaluate(context);
            if (bval2)
            {
                return boxedTrue;
            }

            return boxedFalse;
        }

        #endregion
    }
}
