using System;

namespace NLog
{
    [Serializable]
    public class NLogRuntimeException : Exception
    {
        public NLogRuntimeException()
        {
        }

        public NLogRuntimeException(string message)
            : base(message)
        {
        }

        public NLogRuntimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NLogRuntimeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}