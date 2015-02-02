using System;
using System.Runtime.Serialization;

namespace Id3Lib.Exceptions
{
    [Serializable]
    internal class InvalidStructureException : Exception
    {
        #region Конструктор

        protected InvalidStructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public InvalidStructureException()
        {
        }

        public InvalidStructureException(string message)
            : base(message)
        {
        }

        public InvalidStructureException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}