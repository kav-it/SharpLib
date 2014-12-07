using System;
using System.Globalization;

namespace SharpLib.Json
{
    public class BsonObjectIdConverter : JsonConverter
    {
        #region Методы

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            BsonObjectId objectId = (BsonObjectId)value;

            BsonWriter bsonWriter = writer as BsonWriter;
            if (bsonWriter != null)
            {
                bsonWriter.WriteObjectId(objectId.Value);
            }
            else
            {
                writer.WriteValue(objectId.Value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Bytes)
            {
                throw new JsonSerializationException("Expected Bytes but got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            byte[] value = (byte[])reader.Value;

            return new BsonObjectId(value);
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(BsonObjectId));
        }

        #endregion
    }
}