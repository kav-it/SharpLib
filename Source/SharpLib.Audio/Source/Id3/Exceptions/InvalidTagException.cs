using System;
using System.Runtime.Serialization;

namespace Id3Lib.Exceptions
{
    [Serializable]
    internal class InvalidTagException : InvalidStructureException
    {
        #region Конструктор

        protected InvalidTagException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public InvalidTagException()
        {
        }

        public InvalidTagException(string message)
            : base(message)
        {
        }

        public InvalidTagException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}