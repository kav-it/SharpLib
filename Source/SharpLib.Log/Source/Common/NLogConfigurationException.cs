using System;

namespace NLog
{
    [Serializable]
    public class NLogConfigurationException : Exception
    {
        public NLogConfigurationException()
        {
        }

        public NLogConfigurationException(string message)
            : base(message)
        {
        }

        public NLogConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NLogConfigurationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}