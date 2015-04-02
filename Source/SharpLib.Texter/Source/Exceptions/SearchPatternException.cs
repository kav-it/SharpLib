using System;
using System.Runtime.Serialization;

namespace SharpLib.Texter.Search
{
    public class SearchPatternException : Exception
    {
        #region Конструктор

        public SearchPatternException()
        {
        }

        public SearchPatternException(string message)
            : base(message)
        {
        }

        public SearchPatternException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SearchPatternException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}