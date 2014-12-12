using System;
using System.Runtime.Serialization;

namespace SharpLib
{
    /// <summary>
    /// Исключение с возможностию форматирования
    /// </summary>
    public class FormattedException: Exception
    {
        public FormattedException(string message)
            : base(message) { }

        public FormattedException(string format, params object[] args): base(string.Format(format, args)) { }

        public FormattedException(string message, Exception innerException)
            : base(message, innerException) { }

        public FormattedException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected FormattedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}