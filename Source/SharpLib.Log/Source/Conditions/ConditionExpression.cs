using System;

using NLog.Config;
using NLog.Internal;

namespace NLog.Conditions
{
    [NLogConfigurationItem]
    [ThreadAgnostic]
    public abstract class ConditionExpression
    {
        #region Ìועמהû

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