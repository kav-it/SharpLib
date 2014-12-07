using System;
using System.Runtime.Serialization;

namespace SharpLib.Json.Schema
{
    [Serializable]
    public class JsonSchemaException : JsonException
    {
        #region ��������

        public int LineNumber { get; private set; }

        public int LinePosition { get; private set; }

        public string Path { get; private set; }

        #endregion

        #region �����������

        public JsonSchemaException()
        {
        }

        public JsonSchemaException(string message)
            : base(message)
        {
        }

        public JsonSchemaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public JsonSchemaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        internal JsonSchemaException(string message, Exception innerException, string path, int lineNumber, int linePosition)
            : base(message, innerException)
        {
            Path = path;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        #endregion
    }
}