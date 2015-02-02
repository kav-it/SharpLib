using System;
using System.Runtime.Serialization;

namespace Id3Lib.Exceptions
{
    [Serializable]
    internal class TagNotFoundException : Exception
    {
        #region Конструктор

        protected TagNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public TagNotFoundException()
        {
        }

        public TagNotFoundException(string message)
            : base(message)
        {
        }

        public TagNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        #endregion
    }
}