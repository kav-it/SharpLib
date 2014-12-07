using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

using SharpLib.Json.Linq;

namespace SharpLib.Json
{
    public class DiscriminatedUnionConverter : JsonConverter
    {
        #region Константы

        private const string CasePropertyName = "Case";

        private const string FieldsPropertyName = "Fields";

        #endregion

        #region Методы

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DefaultContractResolver resolver = serializer.ContractResolver as DefaultContractResolver;

            Type t = value.GetType();

            object result = FSharpUtils.GetUnionFields(null, value, t, null);
            object info = FSharpUtils.GetUnionCaseInfo(result);
            object fields = FSharpUtils.GetUnionCaseFields(result);
            object caseName = FSharpUtils.GetUnionCaseInfoName(info);
            object[] fieldsAsArray = fields as object[];

            writer.WriteStartObject();
            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(CasePropertyName) : CasePropertyName);
            writer.WriteValue((string)caseName);
            if (fieldsAsArray != null && fieldsAsArray.Length > 0)
            {
                writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(FieldsPropertyName) : FieldsPropertyName);
                serializer.Serialize(writer, fields);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            object matchingCaseInfo = null;
            string caseName = null;
            JArray fields = null;

            ReadAndAssert(reader);

            while (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = reader.Value.ToString();
                if (string.Equals(propertyName, CasePropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    ReadAndAssert(reader);

                    IEnumerable cases = (IEnumerable)FSharpUtils.GetUnionCases(null, objectType, null);

                    caseName = reader.Value.ToString();

                    foreach (object c in cases)
                    {
                        if ((string)FSharpUtils.GetUnionCaseInfoName(c) == caseName)
                        {
                            matchingCaseInfo = c;
                            break;
                        }
                    }

                    if (matchingCaseInfo == null)
                    {
                        throw JsonSerializationException.Create(reader, "No union type found with the name '{0}'.".FormatWith(CultureInfo.InvariantCulture, caseName));
                    }
                }
                else if (string.Equals(propertyName, FieldsPropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    ReadAndAssert(reader);
                    if (reader.TokenType != JsonToken.StartArray)
                    {
                        throw JsonSerializationException.Create(reader, "Union fields must been an array.");
                    }

                    fields = (JArray)JToken.ReadFrom(reader);
                }
                else
                {
                    throw JsonSerializationException.Create(reader, "Unexpected property '{0}' found when reading union.".FormatWith(CultureInfo.InvariantCulture, propertyName));
                }

                ReadAndAssert(reader);
            }

            if (matchingCaseInfo == null)
            {
                throw JsonSerializationException.Create(reader, "No '{0}' property with union name found.".FormatWith(CultureInfo.InvariantCulture, CasePropertyName));
            }

            PropertyInfo[] fieldProperties = (PropertyInfo[])FSharpUtils.GetUnionCaseInfoFields(matchingCaseInfo);
            object[] typedFieldValues = new object[fieldProperties.Length];

            if (fieldProperties.Length > 0 && fields == null)
            {
                throw JsonSerializationException.Create(reader, "No '{0}' property with union fields found.".FormatWith(CultureInfo.InvariantCulture, FieldsPropertyName));
            }

            if (fields != null)
            {
                if (fieldProperties.Length != fields.Count)
                {
                    throw JsonSerializationException.Create(reader,
                        "The number of field values does not match the number of properties definied by union '{0}'.".FormatWith(CultureInfo.InvariantCulture, caseName));
                }

                for (int i = 0; i < fields.Count; i++)
                {
                    JToken t = fields[i];
                    PropertyInfo fieldProperty = fieldProperties[i];

                    typedFieldValues[i] = t.ToObject(fieldProperty.PropertyType);
                }
            }

            return FSharpUtils.MakeUnion(null, matchingCaseInfo, typedFieldValues, null);
        }

        public override bool CanConvert(Type objectType)
        {
            if (typeof(IEnumerable).IsAssignableFrom(objectType))
            {
                return false;
            }

            object[] attributes = objectType.GetCustomAttributes(true);

            bool isFSharpType = false;
            foreach (object attribute in attributes)
            {
                Type attributeType = attribute.GetType();
                if (attributeType.Name == "CompilationMappingAttribute")
                {
                    FSharpUtils.EnsureInitialized(attributeType.Assembly());

                    isFSharpType = true;
                    break;
                }
            }

            if (!isFSharpType)
            {
                return false;
            }

            return (bool)FSharpUtils.IsUnion(null, objectType, null);
        }

        private static void ReadAndAssert(JsonReader reader)
        {
            if (!reader.Read())
            {
                throw JsonSerializationException.Create(reader, "Unexpected end when reading union.");
            }
        }

        #endregion
    }
}

