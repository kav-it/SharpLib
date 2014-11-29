using System;

namespace NLog.Conditions
{
    [Serializable]
    public class ConditionEvaluationException : Exception
    {
        public ConditionEvaluationException()
        {
        }

        public ConditionEvaluationException(string message)
            : base(message)
        {
        }

        public ConditionEvaluationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConditionEvaluationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}