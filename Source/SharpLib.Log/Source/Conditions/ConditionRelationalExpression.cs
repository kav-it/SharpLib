using System;
using System.Globalization;

namespace NLog.Conditions
{
    internal sealed class ConditionRelationalExpression : ConditionExpression
    {
        #region Свойства

        public ConditionExpression LeftExpression { get; private set; }

        public ConditionExpression RightExpression { get; private set; }

        public ConditionRelationalOperator RelationalOperator { get; private set; }

        #endregion

        #region Конструктор

        public ConditionRelationalExpression(ConditionExpression leftExpression, ConditionExpression rightExpression, ConditionRelationalOperator relationalOperator)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
            RelationalOperator = relationalOperator;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return "(" + LeftExpression + " " + GetOperatorString() + " " + RightExpression + ")";
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            object v1 = LeftExpression.Evaluate(context);
            object v2 = RightExpression.Evaluate(context);

            return Compare(v1, v2, RelationalOperator);
        }

        private static object Compare(object leftValue, object rightValue, ConditionRelationalOperator relationalOperator)
        {
            StringComparer comparer = StringComparer.InvariantCulture;
            PromoteTypes(ref leftValue, ref rightValue);
            switch (relationalOperator)
            {
                case ConditionRelationalOperator.Equal:
                    return comparer.Compare(leftValue, rightValue) == 0;

                case ConditionRelationalOperator.NotEqual:
                    return comparer.Compare(leftValue, rightValue) != 0;

                case ConditionRelationalOperator.Greater:
                    return comparer.Compare(leftValue, rightValue) > 0;

                case ConditionRelationalOperator.GreaterOrEqual:
                    return comparer.Compare(leftValue, rightValue) >= 0;

                case ConditionRelationalOperator.LessOrEqual:
                    return comparer.Compare(leftValue, rightValue) <= 0;

                case ConditionRelationalOperator.Less:
                    return comparer.Compare(leftValue, rightValue) < 0;

                default:
                    throw new NotSupportedException("Relational operator " + relationalOperator + " is not supported.");
            }
        }

        private static void PromoteTypes(ref object val1, ref object val2)
        {
            if (val1 == null || val2 == null)
            {
                return;
            }

            if (val1.GetType() == val2.GetType())
            {
                return;
            }

            if (val1 is DateTime || val2 is DateTime)
            {
                val1 = Convert.ToDateTime(val1, CultureInfo.InvariantCulture);
                val2 = Convert.ToDateTime(val2, CultureInfo.InvariantCulture);
                return;
            }

            if (val1 is string || val2 is string)
            {
                val1 = Convert.ToString(val1, CultureInfo.InvariantCulture);
                val2 = Convert.ToString(val2, CultureInfo.InvariantCulture);
                return;
            }

            if (val1 is double || val2 is double)
            {
                val1 = Convert.ToDouble(val1, CultureInfo.InvariantCulture);
                val2 = Convert.ToDouble(val2, CultureInfo.InvariantCulture);
                return;
            }

            if (val1 is float || val2 is float)
            {
                val1 = Convert.ToSingle(val1, CultureInfo.InvariantCulture);
                val2 = Convert.ToSingle(val2, CultureInfo.InvariantCulture);
                return;
            }

            if (val1 is decimal || val2 is decimal)
            {
                val1 = Convert.ToDecimal(val1, CultureInfo.InvariantCulture);
                val2 = Convert.ToDecimal(val2, CultureInfo.InvariantCulture);
                return;
            }

            if (val1 is long || val2 is long)
            {
                val1 = Convert.ToInt64(val1, CultureInfo.InvariantCulture);
                val2 = Convert.ToInt64(val2, CultureInfo.InvariantCulture);
                return;
            }

            if (val1 is int || val2 is int)
            {
                val1 = Convert.ToInt32(val1, CultureInfo.InvariantCulture);
                val2 = Convert.ToInt32(val2, CultureInfo.InvariantCulture);
                return;
            }

            if (val1 is bool || val2 is bool)
            {
                val1 = Convert.ToBoolean(val1, CultureInfo.InvariantCulture);
                val2 = Convert.ToBoolean(val2, CultureInfo.InvariantCulture);
                return;
            }

            throw new ConditionEvaluationException("Cannot find common type for '" + val1.GetType().Name + "' and '" + val2.GetType().Name + "'.");
        }

        private string GetOperatorString()
        {
            switch (RelationalOperator)
            {
                case ConditionRelationalOperator.Equal:
                    return "==";

                case ConditionRelationalOperator.NotEqual:
                    return "!=";

                case ConditionRelationalOperator.Greater:
                    return ">";

                case ConditionRelationalOperator.Less:
                    return "<";

                case ConditionRelationalOperator.GreaterOrEqual:
                    return ">=";

                case ConditionRelationalOperator.LessOrEqual:
                    return "<=";

                default:
                    throw new NotSupportedException("Relational operator " + RelationalOperator + " is not supported.");
            }
        }

        #endregion
    }
}