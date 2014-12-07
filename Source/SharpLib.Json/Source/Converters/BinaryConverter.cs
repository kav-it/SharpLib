using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;

namespace SharpLib.Json
{
    public class BinaryConverter : JsonConverter
    {
        #region Константы

        private const string BinaryToArrayName = "ToArray";

        private const string BinaryTypeName = "System.Data.Linq.Binary";

        #endregion

        #region Поля

        private ReflectionObject _reflectionObject;

        #endregion

        #region Методы

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            byte[] data = GetByteArray(value);

            writer.WriteValue(data);
        }

        private byte[] GetByteArray(object value)
        {
            if (value.GetType().AssignableToTypeName(BinaryTypeName))
            {
                EnsureReflectionObject(value.GetType());
                return (byte[])_reflectionObject.GetValue(value, BinaryToArrayName);
            }
            if (value is SqlBinary)
            {
                return ((SqlBinary)value).Value;
            }

            throw new JsonSerializationException("Unexpected value type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
        }

        private void EnsureReflectionObject(Type t)
        {
            if (_reflectionObject == null)
            {
                _reflectionObject = ReflectionObject.Create(t, t.GetConstructor(new[] { typeof(byte[]) }), BinaryToArrayName);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type t = (ReflectionUtils.IsNullableType(objectType))
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            if (reader.TokenType == JsonToken.Null)
            {
                if (!ReflectionUtils.IsNullable(objectType))
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
                }

                return null;
            }

            byte[] data;

            if (reader.TokenType == JsonToken.StartArray)
            {
                data = ReadByteArray(reader);
            }
            else if (reader.TokenType == JsonToken.String)
            {
                string encodedData = reader.Value.ToString();
                data = Convert.FromBase64String(encodedData);
            }
            else
            {
                throw JsonSerializationException.Create(reader, "Unexpected token parsing binary. Expected String or StartArray, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            if (t.AssignableToTypeName(BinaryTypeName))
            {
                EnsureReflectionObject(t);

                return _reflectionObject.Creator(data);
            }

            if (t == typeof(SqlBinary))
            {
                return new SqlBinary(data);
            }

            throw JsonSerializationException.Create(reader, "Unexpected object type when writing binary: {0}".FormatWith(CultureInfo.InvariantCulture, objectType));
        }

        private byte[] ReadByteArray(JsonReader reader)
        {
            List<byte> byteList = new List<byte>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Integer:
                        byteList.Add(Convert.ToByte(reader.Value, CultureInfo.InvariantCulture));
                        break;
                    case JsonToken.EndArray:
                        return byteList.ToArray();
                    case JsonToken.Comment:

                        break;
                    default:
                        throw JsonSerializationException.Create(reader, "Unexpected token when reading bytes: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
                }
            }

            throw JsonSerializationException.Create(reader, "Unexpected end when reading bytes.");
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.AssignableToTypeName(BinaryTypeName))
            {
                return true;
            }

            if (objectType == typeof(SqlBinary) || objectType == typeof(SqlBinary?))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}