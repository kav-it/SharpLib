﻿using System;
using System.Runtime.Serialization;

namespace SharpLib.Json
{
    [Serializable]
    public class JsonWriterException : JsonException
    {
        #region Свойства

        public string Path { get; private set; }

        #endregion

        #region Конструктор

        public JsonWriterException()
        {
        }

        public JsonWriterException(string message)
            : base(message)
        {
        }

        public JsonWriterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public JsonWriterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        internal JsonWriterException(string message, Exception innerException, string path)
            : base(message, innerException)
        {
            Path = path;
        }

        #endregion

        #region Методы

        internal static JsonWriterException Create(JsonWriter writer, string message, Exception ex)
        {
            return Create(writer.ContainerPath, message, ex);
        }

        internal static JsonWriterException Create(string path, string message, Exception ex)
        {
            message = JsonPosition.FormatMessage(null, path, message);

            return new JsonWriterException(message, ex, path);
        }

        #endregion
    }
}