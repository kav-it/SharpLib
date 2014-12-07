using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using SharpLib.Json.Linq;

namespace SharpLib.Json.Schema
{
    public class JsonSchemaGenerator
    {
        #region Поля

        private readonly IList<TypeSchema> _stack = new List<TypeSchema>();

        private IContractResolver _contractResolver;

        private JsonSchema _currentSchema;

        private JsonSchemaResolver _resolver;

        #endregion

        #region Свойства

        public UndefinedSchemaIdHandling UndefinedSchemaIdHandling { get; set; }

        public IContractResolver ContractResolver
        {
            get
            {
                if (_contractResolver == null)
                {
                    return DefaultContractResolver.Instance;
                }

                return _contractResolver;
            }
            set { _contractResolver = value; }
        }

        private JsonSchema CurrentSchema
        {
            get { return _currentSchema; }
        }

        #endregion

        #region Методы

        private void Push(TypeSchema typeSchema)
        {
            _currentSchema = typeSchema.Schema;
            _stack.Add(typeSchema);
            _resolver.LoadedSchemas.Add(typeSchema.Schema);
        }

        private TypeSchema Pop()
        {
            TypeSchema popped = _stack[_stack.Count - 1];
            _stack.RemoveAt(_stack.Count - 1);
            TypeSchema newValue = _stack.LastOrDefault();
            _currentSchema = newValue != null ? newValue.Schema : null;

            return popped;
        }

        public JsonSchema Generate(Type type)
        {
            return Generate(type, new JsonSchemaResolver(), false);
        }

        public JsonSchema Generate(Type type, JsonSchemaResolver resolver)
        {
            return Generate(type, resolver, false);
        }

        public JsonSchema Generate(Type type, bool rootSchemaNullable)
        {
            return Generate(type, new JsonSchemaResolver(), rootSchemaNullable);
        }

        public JsonSchema Generate(Type type, JsonSchemaResolver resolver, bool rootSchemaNullable)
        {
            ValidationUtils.ArgumentNotNull(type, "type");
            ValidationUtils.ArgumentNotNull(resolver, "resolver");

            _resolver = resolver;

            return GenerateInternal(type, (!rootSchemaNullable) ? Required.Always : Required.Default, false);
        }

        private string GetTitle(Type type)
        {
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);

            if (containerAttribute != null && !string.IsNullOrEmpty(containerAttribute.Title))
            {
                return containerAttribute.Title;
            }

            return null;
        }

        private string GetDescription(Type type)
        {
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);

            if (containerAttribute != null && !string.IsNullOrEmpty(containerAttribute.Description))
            {
                return containerAttribute.Description;
            }

            DescriptionAttribute descriptionAttribute = ReflectionUtils.GetAttribute<DescriptionAttribute>(type);
            if (descriptionAttribute != null)
            {
                return descriptionAttribute.Description;
            }

            return null;
        }

        private string GetTypeId(Type type, bool explicitOnly)
        {
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);

            if (containerAttribute != null && !string.IsNullOrEmpty(containerAttribute.Id))
            {
                return containerAttribute.Id;
            }

            if (explicitOnly)
            {
                return null;
            }

            switch (UndefinedSchemaIdHandling)
            {
                case UndefinedSchemaIdHandling.UseTypeName:
                    return type.FullName;
                case UndefinedSchemaIdHandling.UseAssemblyQualifiedName:
                    return type.AssemblyQualifiedName;
                default:
                    return null;
            }
        }

        private JsonSchema GenerateInternal(Type type, Required valueRequired, bool required)
        {
            ValidationUtils.ArgumentNotNull(type, "type");

            string resolvedId = GetTypeId(type, false);
            string explicitId = GetTypeId(type, true);

            if (!string.IsNullOrEmpty(resolvedId))
            {
                JsonSchema resolvedSchema = _resolver.GetSchema(resolvedId);
                if (resolvedSchema != null)
                {
                    if (valueRequired != Required.Always && !HasFlag(resolvedSchema.Type, JsonSchemaType.Null))
                    {
                        resolvedSchema.Type |= JsonSchemaType.Null;
                    }
                    if (required && resolvedSchema.Required != true)
                    {
                        resolvedSchema.Required = true;
                    }

                    return resolvedSchema;
                }
            }

            if (_stack.Any(tc => tc.Type == type))
            {
                throw new JsonException(
                    "Unresolved circular reference for type '{0}'. Explicitly define an Id for the type using a JsonObject/JsonArray attribute or automatically generate a type Id using the UndefinedSchemaIdHandling property."
                        .FormatWith(CultureInfo.InvariantCulture, type));
            }

            JsonContract contract = ContractResolver.ResolveContract(type);
            JsonConverter converter;
            if ((converter = contract.Converter) != null || (converter = contract.InternalConverter) != null)
            {
                JsonSchema converterSchema = converter.GetSchema();
                if (converterSchema != null)
                {
                    return converterSchema;
                }
            }

            Push(new TypeSchema(type, new JsonSchema()));

            if (explicitId != null)
            {
                CurrentSchema.Id = explicitId;
            }

            if (required)
            {
                CurrentSchema.Required = true;
            }
            CurrentSchema.Title = GetTitle(type);
            CurrentSchema.Description = GetDescription(type);

            if (converter != null)
            {
                CurrentSchema.Type = JsonSchemaType.Any;
            }
            else
            {
                switch (contract.ContractType)
                {
                    case JsonContractType.Object:
                        CurrentSchema.Type = AddNullType(JsonSchemaType.Object, valueRequired);
                        CurrentSchema.Id = GetTypeId(type, false);
                        GenerateObjectSchema(type, (JsonObjectContract)contract);
                        break;
                    case JsonContractType.Array:
                        CurrentSchema.Type = AddNullType(JsonSchemaType.Array, valueRequired);

                        CurrentSchema.Id = GetTypeId(type, false);

                        JsonArrayAttribute arrayAttribute = JsonTypeReflector.GetCachedAttribute<JsonArrayAttribute>(type);
                        bool allowNullItem = (arrayAttribute == null || arrayAttribute.AllowNullItems);

                        Type collectionItemType = ReflectionUtils.GetCollectionItemType(type);
                        if (collectionItemType != null)
                        {
                            CurrentSchema.Items = new List<JsonSchema>();
                            CurrentSchema.Items.Add(GenerateInternal(collectionItemType, (!allowNullItem) ? Required.Always : Required.Default, false));
                        }
                        break;
                    case JsonContractType.Primitive:
                        CurrentSchema.Type = GetJsonSchemaType(type, valueRequired);

                        if (CurrentSchema.Type == JsonSchemaType.Integer && type.IsEnum() && !type.IsDefined(typeof(FlagsAttribute), true))
                        {
                            CurrentSchema.Enum = new List<JToken>();

                            IList<EnumValue<long>> enumValues = EnumUtils.GetNamesAndValues<long>(type);
                            foreach (EnumValue<long> enumValue in enumValues)
                            {
                                JToken value = JToken.FromObject(enumValue.Value);

                                CurrentSchema.Enum.Add(value);
                            }
                        }
                        break;
                    case JsonContractType.String:
                        JsonSchemaType schemaType = (!ReflectionUtils.IsNullable(contract.UnderlyingType))
                            ? JsonSchemaType.String
                            : AddNullType(JsonSchemaType.String, valueRequired);

                        CurrentSchema.Type = schemaType;
                        break;
                    case JsonContractType.Dictionary:
                        CurrentSchema.Type = AddNullType(JsonSchemaType.Object, valueRequired);

                        Type keyType;
                        Type valueType;
                        ReflectionUtils.GetDictionaryKeyValueTypes(type, out keyType, out valueType);

                        if (keyType != null)
                        {
                            JsonContract keyContract = ContractResolver.ResolveContract(keyType);

                            if (keyContract.ContractType == JsonContractType.Primitive)
                            {
                                CurrentSchema.AdditionalProperties = GenerateInternal(valueType, Required.Default, false);
                            }
                        }
                        break;
                    case JsonContractType.Serializable:
                        CurrentSchema.Type = AddNullType(JsonSchemaType.Object, valueRequired);
                        CurrentSchema.Id = GetTypeId(type, false);
                        GenerateISerializableContract(type, (JsonISerializableContract)contract);
                        break;
                    case JsonContractType.Dynamic:
                    case JsonContractType.Linq:
                        CurrentSchema.Type = JsonSchemaType.Any;
                        break;
                    default:
                        throw new JsonException("Unexpected contract type: {0}".FormatWith(CultureInfo.InvariantCulture, contract));
                }
            }

            return Pop().Schema;
        }

        private JsonSchemaType AddNullType(JsonSchemaType type, Required valueRequired)
        {
            if (valueRequired != Required.Always)
            {
                return type | JsonSchemaType.Null;
            }

            return type;
        }

        private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
        {
            return ((value & flag) == flag);
        }

        private void GenerateObjectSchema(Type type, JsonObjectContract contract)
        {
            CurrentSchema.Properties = new Dictionary<string, JsonSchema>();
            foreach (JsonProperty property in contract.Properties)
            {
                if (!property.Ignored)
                {
                    bool optional = property.NullValueHandling == NullValueHandling.Ignore ||
                                    HasFlag(property.DefaultValueHandling.GetValueOrDefault(), DefaultValueHandling.Ignore) ||
                                    property.ShouldSerialize != null ||
                                    property.GetIsSpecified != null;

                    JsonSchema propertySchema = GenerateInternal(property.PropertyType, property.Required, !optional);

                    if (property.DefaultValue != null)
                    {
                        propertySchema.Default = JToken.FromObject(property.DefaultValue);
                    }

                    CurrentSchema.Properties.Add(property.PropertyName, propertySchema);
                }
            }

            if (type.IsSealed())
            {
                CurrentSchema.AllowAdditionalProperties = false;
            }
        }

        private void GenerateISerializableContract(Type type, JsonISerializableContract contract)
        {
            CurrentSchema.AllowAdditionalProperties = true;
        }

        internal static bool HasFlag(JsonSchemaType? value, JsonSchemaType flag)
        {
            if (value == null)
            {
                return true;
            }

            bool match = ((value & flag) == flag);
            if (match)
            {
                return true;
            }

            if (flag == JsonSchemaType.Integer && (value & JsonSchemaType.Float) == JsonSchemaType.Float)
            {
                return true;
            }

            return false;
        }

        private JsonSchemaType GetJsonSchemaType(Type type, Required valueRequired)
        {
            JsonSchemaType schemaType = JsonSchemaType.None;
            if (valueRequired != Required.Always && ReflectionUtils.IsNullable(type))
            {
                schemaType = JsonSchemaType.Null;
                if (ReflectionUtils.IsNullableType(type))
                {
                    type = Nullable.GetUnderlyingType(type);
                }
            }

            PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(type);

            switch (typeCode)
            {
                case PrimitiveTypeCode.Empty:
                case PrimitiveTypeCode.Object:
                    return schemaType | JsonSchemaType.String;
                case PrimitiveTypeCode.DBNull:
                    return schemaType | JsonSchemaType.Null;
                case PrimitiveTypeCode.Boolean:
                    return schemaType | JsonSchemaType.Boolean;
                case PrimitiveTypeCode.Char:
                    return schemaType | JsonSchemaType.String;
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.UInt32:
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                case PrimitiveTypeCode.BigInteger:
                    return schemaType | JsonSchemaType.Integer;
                case PrimitiveTypeCode.Single:
                case PrimitiveTypeCode.Double:
                case PrimitiveTypeCode.Decimal:
                    return schemaType | JsonSchemaType.Float;

                case PrimitiveTypeCode.DateTime:
                case PrimitiveTypeCode.DateTimeOffset:
                    return schemaType | JsonSchemaType.String;
                case PrimitiveTypeCode.String:
                case PrimitiveTypeCode.Uri:
                case PrimitiveTypeCode.Guid:
                case PrimitiveTypeCode.TimeSpan:
                case PrimitiveTypeCode.Bytes:
                    return schemaType | JsonSchemaType.String;
                default:
                    throw new JsonException("Unexpected type code '{0}' for type '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeCode, type));
            }
        }

        #endregion

        #region Вложенный класс: TypeSchema

        private class TypeSchema
        {
            #region Свойства

            public Type Type { get; private set; }

            public JsonSchema Schema { get; private set; }

            #endregion

            #region Конструктор

            public TypeSchema(Type type, JsonSchema schema)
            {
                ValidationUtils.ArgumentNotNull(type, "type");
                ValidationUtils.ArgumentNotNull(schema, "schema");

                Type = type;
                Schema = schema;
            }

            #endregion
        }

        #endregion
    }
}