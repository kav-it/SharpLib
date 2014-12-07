﻿using System;
using System.Runtime.Serialization;

namespace SharpLib.Json
{
    [Serializable]
    public class JsonReaderException : JsonException
    {
        #region Свойства

        public int LineNumber { get; private set; }

        public int LinePosition { get; private set; }

        public string Path { get; private set; }

        #endregion

        #region Конструктор

        public JsonReaderException()
        {
        }

        public JsonReaderException(string message)
            : base(message)
        {
        }

        public JsonReaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public JsonReaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        internal JsonReaderException(string message, Exception innerException, string path, int lineNumber, int linePosition)
            : base(message, innerException)
        {
            Path = path;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        #endregion

        #region Методы

        internal static JsonReaderException Create(JsonReader reader, string message)
        {
            return Create(reader, message, null);
        }

        internal static JsonReaderException Create(JsonReader reader, string message, Exception ex)
        {
            return Create(reader as IJsonLineInfo, reader.Path, message, ex);
        }

        internal static JsonReaderException Create(IJsonLineInfo lineInfo, string path, string message, Exception ex)
        {
            message = JsonPosition.FormatMessage(lineInfo, path, message);

            int lineNumber;
            int linePosition;
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                lineNumber = lineInfo.LineNumber;
                linePosition = lineInfo.LinePosition;
            }
            else
            {
                lineNumber = 0;
                linePosition = 0;
            }

            return new JsonReaderException(message, ex, path, lineNumber, linePosition);
        }

        #endregion
    }
}