
using System;

namespace SharpLib.Log
{
    [LogConfigurationItem]
    [ThreadAgnostic]
    public abstract class ConditionExpression
    {
        #region Методы

        public object Evaluate(LogEventInfo context)
        {
            try
            {
                return EvaluateNode(context);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                throw new ConditionEvaluationException("Exception occurred when evaluating condition", exception);
            }
        }

        public abstract override string ToString();

        protected abstract object EvaluateNode(LogEventInfo context);

        #endregion

        public static implicit operator ConditionExpression(string conditionExpressionText)
        {
            return ConditionParser.ParseExpression(conditionExpressionText);
        }
    }    
}
