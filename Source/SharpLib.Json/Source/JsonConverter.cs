using System;

using SharpLib.Json.Schema;

namespace SharpLib.Json
{
    public abstract class JsonConverter
    {
        #region Свойства

        public virtual bool CanRead
        {
            get { return true; }
        }

        public virtual bool CanWrite
        {
            get { return true; }
        }

        #endregion

        #region Методы

        public abstract void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);

        public abstract object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);

        public abstract bool CanConvert(Type objectType);

        public virtual JsonSchema GetSchema()
        {
            return null;
        }

        #endregion
    }
}