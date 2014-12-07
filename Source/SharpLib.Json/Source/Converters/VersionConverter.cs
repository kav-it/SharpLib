using System;
using System.Globalization;

namespace SharpLib.Json
{
    public class VersionConverter : JsonConverter
    {
        #region Методы

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else if (value is Version)
            {
                writer.WriteValue(value.ToString());
            }
            else
            {
                throw new JsonSerializationException("Expected Version object value");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            if (reader.TokenType == JsonToken.String)
            {
                try
                {
                    Version v = new Version((string)reader.Value);
                    return v;
                }
                catch (Exception ex)
                {
                    throw JsonSerializationException.Create(reader, "Error parsing version string: {0}".FormatWith(CultureInfo.InvariantCulture, reader.Value), ex);
                }
            }
            throw JsonSerializationException.Create(reader,
                "Unexpected token or value when parsing version. Token: {0}, Value: {1}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType, reader.Value));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Version);
        }

        #endregion
    }
}