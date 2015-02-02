using System;
using System.Runtime.Serialization;

namespace Id3Lib.Exceptions
{
    [Serializable]
    internal class InvalidFrameException : InvalidStructureException
    {
        #region Конструктор

        protected InvalidFrameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public InvalidFrameException()
        {
        }

        public InvalidFrameException(string message)
            : base(message)
        {
        }

        public InvalidFrameException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}