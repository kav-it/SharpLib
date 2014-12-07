using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Json.Linq
{
    internal enum QueryOperator
    {
        None,

        Equals,

        NotEquals,

        Exists,

        LessThan,

        LessThanOrEquals,

        GreaterThan,

        GreaterThanOrEquals,

        And,

        Or
    }

    internal abstract class QueryExpression
    {
        #region Свойства

        public QueryOperator Operator { get; set; }

        #endregion

        #region Методы

        public abstract bool IsMatch(JToken t);

        #endregion
    }

    internal class CompositeExpression : QueryExpression
    {
        #region Свойства

        public List<QueryExpression> Expressions { get; set; }

        #endregion

        #region Конструктор

        public CompositeExpression()
        {
            Expressions = new List<QueryExpression>();
        }

        #endregion

        #region Методы

        public override bool IsMatch(JToken t)
        {
            switch (Operator)
            {
                case QueryOperator.And:
                    if (Expressions.Any(e => !e.IsMatch(t)))
                    {
                        return false;
                    }
                    return true;
                case QueryOperator.Or:
                    if (Expressions.Any(e => e.IsMatch(t)))
                    {
                        return true;
                    }
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }

    internal class BooleanQueryExpression : QueryExpression
    {
        #region Свойства

        public List<PathFilter> Path { get; set; }

        public JValue Value { get; set; }

        #endregion

        #region Методы

        public override bool IsMatch(JToken t)
        {
            IEnumerable<JToken> pathResult = JPath.Evaluate(Path, t, false);

            foreach (JToken r in pathResult)
            {
                JValue v = r as JValue;
                switch (Operator)
                {
                    case QueryOperator.Equals:
                        if (v != null && v.Equals(Value))
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.NotEquals:
                        if (v != null && !v.Equals(Value))
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.GreaterThan:
                        if (v != null && v.CompareTo(Value) > 0)
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.GreaterThanOrEquals:
                        if (v != null && v.CompareTo(Value) >= 0)
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.LessThan:
                        if (v != null && v.CompareTo(Value) < 0)
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.LessThanOrEquals:
                        if (v != null && v.CompareTo(Value) <= 0)
                        {
                            return true;
                        }
                        break;
                    case QueryOperator.Exists:
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return false;
        }

        #endregion
    }
}