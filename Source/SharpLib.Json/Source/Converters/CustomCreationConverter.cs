using System;

namespace SharpLib.Json
{
    public abstract class CustomCreationConverter<T> : JsonConverter
    {
        #region Свойства

        public override bool CanWrite
        {
            get { return false; }
        }

        #endregion

        #region Методы

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("CustomCreationConverter should only be used while deserializing.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            T value = Create(objectType);
            if (value == null)
            {
                throw new JsonSerializationException("No object created.");
            }

            serializer.Populate(reader, value);
            return value;
        }

        public abstract T Create(Type objectType);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        #endregion
    }
}