using System;
using System.Collections.Generic;

namespace SharpLib.Json
{
    public class KeyValuePairConverter : JsonConverter
    {
        #region Константы

        private const string KeyName = "Key";

        private const string ValueName = "Value";

        #endregion

        #region Поля

        private static readonly ThreadSafeStore<Type, ReflectionObject> ReflectionObjectPerType = new ThreadSafeStore<Type, ReflectionObject>(InitializeReflectionObject);

        #endregion

        #region Методы

        private static ReflectionObject InitializeReflectionObject(Type t)
        {
            IList<Type> genericArguments = t.GetGenericArguments();
            Type keyType = genericArguments[0];
            Type valueType = genericArguments[1];

            return ReflectionObject.Create(t, t.GetConstructor(new[] { keyType, valueType }), KeyName, ValueName);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ReflectionObject reflectionObject = ReflectionObjectPerType.Get(value.GetType());

            DefaultContractResolver resolver = serializer.ContractResolver as DefaultContractResolver;

            writer.WriteStartObject();
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(KeyName) : KeyName);
            serializer.Serialize(writer, reflectionObject.GetValue(value, KeyName), reflectionObject.GetType(KeyName));
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(ValueName) : ValueName);
            serializer.Serialize(writer, reflectionObject.GetValue(value, ValueName), reflectionObject.GetType(ValueName));
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = ReflectionUtils.IsNullableType(objectType);

            Type t = (isNullable)
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            ReflectionObject reflectionObject = ReflectionObjectPerType.Get(t);

            if (reader.TokenType == JsonToken.Null)
            {
                if (!isNullable)
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to KeyValuePair.");
                }

                return null;
            }

            object key = null;
            object value = null;

            ReadAndAssert(reader);

            while (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = reader.Value.ToString();
                if (string.Equals(propertyName, KeyName, StringComparison.OrdinalIgnoreCase))
                {
                    ReadAndAssert(reader);
                    key = serializer.Deserialize(reader, reflectionObject.GetType(KeyName));
                }
                else if (string.Equals(propertyName, ValueName, StringComparison.OrdinalIgnoreCase))
                {
                    ReadAndAssert(reader);
                    value = serializer.Deserialize(reader, reflectionObject.GetType(ValueName));
                }
                else
                {
                    reader.Skip();
                }

                ReadAndAssert(reader);
            }

            return reflectionObject.Creator(key, value);
        }

        public override bool CanConvert(Type objectType)
        {
            Type t = (ReflectionUtils.IsNullableType(objectType))
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;

            if (t.IsValueType() && t.IsGenericType())
            {
                return (t.GetGenericTypeDefinition() == typeof(KeyValuePair<,>));
            }

            return false;
        }

        private static void ReadAndAssert(JsonReader reader)
        {
            if (!reader.Read())
            {
                throw JsonSerializationException.Create(reader, "Unexpected end when reading KeyValuePair.");
            }
        }

        #endregion
    }
}