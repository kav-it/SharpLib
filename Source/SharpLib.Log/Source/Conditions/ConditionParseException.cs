using System;

namespace NLog.Conditions
{
    [Serializable]
    public class ConditionParseException : Exception
    {
        public ConditionParseException()
        {
        }

        public ConditionParseException(string message)
            : base(message)
        {
        }

        public ConditionParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConditionParseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}