using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using SharpLib.Json.Linq;

namespace SharpLib.Json
{
    public class DefaultContractResolver : IContractResolver
    {
        #region Поля

        private static readonly JsonConverter[] BuiltInConverters =
        {
            new EntityKeyMemberConverter(),
            new ExpandoObjectConverter(),
            new XmlNodeConverter(),
            new BinaryConverter(),
            new DataSetConverter(),
            new DataTableConverter(),
            new DiscriminatedUnionConverter(),
            new KeyValuePairConverter(),
            new BsonObjectIdConverter(),
            new RegexConverter()
        };

        private static readonly object TypeContractCacheLock = new object();

        private static readonly IContractResolver _instance = new DefaultContractResolver(true);

        private static readonly DefaultContractResolverState _sharedState = new DefaultContractResolverState();

        private readonly DefaultContractResolverState _instanceState = new DefaultContractResolverState();

        private readonly bool _sharedCache;

        #endregion

        #region Свойства

        [Obsolete("DefaultMembersSearchFlags is obsolete. To modify the members serialized inherit from DefaultContractResolver and override the GetSerializableMembers method instead.")]
        public BindingFlags DefaultMembersSearchFlags { get; set; }

        internal static IContractResolver Instance
        {
            get { return _instance; }
        }

        public bool DynamicCodeGeneration
        {
            get { return JsonTypeReflector.DynamicCodeGeneration; }
        }

        public bool SerializeCompilerGeneratedMembers { get; set; }

        public bool IgnoreSerializableInterface { get; set; }

        public bool IgnoreSerializableAttribute { get; set; }

        #endregion

        #region Конструктор

        public DefaultContractResolver()
            : this(false)
        {
        }

        public DefaultContractResolver(bool shareCache)
        {
#pragma warning disable 618
            DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.Instance;
#pragma warning restore 618
            IgnoreSerializableAttribute = true;

            _sharedCache = shareCache;
        }

        #endregion

        #region Методы

        internal DefaultContractResolverState GetState()
        {
            if (_sharedCache)
            {
                return _sharedState;
            }
            return _instanceState;
        }

        public virtual JsonContract ResolveContract(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            DefaultContractResolverState state = GetState();

            JsonContract contract;
            ResolverContractKey key = new ResolverContractKey(GetType(), type);
            Dictionary<ResolverContractKey, JsonContract> cache = state.ContractCache;
            if (cache == null || !cache.TryGetValue(key, out contract))
            {
                contract = CreateContract(type);

                lock (TypeContractCacheLock)
                {
                    cache = state.ContractCache;
                    Dictionary<ResolverContractKey, JsonContract> updatedCache = (cache != null)
                        ? new Dictionary<ResolverContractKey, JsonContract>(cache)
                        : new Dictionary<ResolverContractKey, JsonContract>();
                    updatedCache[key] = contract;

                    state.ContractCache = updatedCache;
                }
            }

            return contract;
        }

        protected virtual List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            bool ignoreSerializableAttribute = IgnoreSerializableAttribute;

            MemberSerialization memberSerialization = JsonTypeReflector.GetObjectMemberSerialization(objectType, ignoreSerializableAttribute);

            List<MemberInfo> allMembers = ReflectionUtils.GetFieldsAndProperties(objectType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Where(m => !ReflectionUtils.IsIndexedProperty(m)).ToList();

            List<MemberInfo> serializableMembers = new List<MemberInfo>();

            if (memberSerialization != MemberSerialization.Fields)
            {
                DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(objectType);

#pragma warning disable 618
                List<MemberInfo> defaultMembers = ReflectionUtils.GetFieldsAndProperties(objectType, DefaultMembersSearchFlags)
                    .Where(m => !ReflectionUtils.IsIndexedProperty(m)).ToList();
#pragma warning restore 618

                foreach (MemberInfo member in allMembers)
                {
                    if (SerializeCompilerGeneratedMembers || !member.IsDefined(typeof(CompilerGeneratedAttribute), true))
                    {
                        if (defaultMembers.Contains(member))
                        {
                            serializableMembers.Add(member);
                        }
                        else
                        {
                            if (JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(member) != null)
                            {
                                serializableMembers.Add(member);
                            }
                            else if (dataContractAttribute != null && JsonTypeReflector.GetAttribute<DataMemberAttribute>(member) != null)
                            {
                                serializableMembers.Add(member);
                            }
                            else if (memberSerialization == MemberSerialization.Fields && member.MemberType() == MemberTypes.Field)
                            {
                                serializableMembers.Add(member);
                            }
                        }
                    }
                }

                Type match;

                if (objectType.AssignableToTypeName("System.Data.Objects.DataClasses.EntityObject", out match))
                {
                    serializableMembers = serializableMembers.Where(ShouldSerializeEntityMember).ToList();
                }
            }
            else
            {
                serializableMembers.AddRange(from member in allMembers
                                             let field = member as FieldInfo
                                             where field != null && !field.IsStatic
                                             select member);
            }

            return serializableMembers;
        }

        private bool ShouldSerializeEntityMember(MemberInfo memberInfo)
        {
            PropertyInfo propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                if (propertyInfo.PropertyType.IsGenericType() && propertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "System.Data.Objects.DataClasses.EntityReference`1")
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual JsonObjectContract CreateObjectContract(Type objectType)
        {
            JsonObjectContract contract = new JsonObjectContract(objectType);
            InitializeContract(contract);

            var ignoreSerializableAttribute = IgnoreSerializableAttribute;

            contract.MemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(contract.NonNullableUnderlyingType, ignoreSerializableAttribute);
            contract.Properties.AddRange(CreateProperties(contract.NonNullableUnderlyingType, contract.MemberSerialization));

            JsonObjectAttribute attribute = JsonTypeReflector.GetCachedAttribute<JsonObjectAttribute>(contract.NonNullableUnderlyingType);
            if (attribute != null)
            {
                contract.ItemRequired = attribute._itemRequired;
            }

            if (contract.IsInstantiable)
            {
                ConstructorInfo overrideConstructor = GetAttributeConstructor(contract.NonNullableUnderlyingType);

                if (overrideConstructor != null)
                {
#pragma warning disable 618
                    contract.OverrideConstructor = overrideConstructor;
#pragma warning restore 618
                    contract.CreatorParameters.AddRange(CreateConstructorParameters(overrideConstructor, contract.Properties));
                }
                else if (contract.MemberSerialization == MemberSerialization.Fields)
                {
                    if (JsonTypeReflector.FullyTrusted)
                    {
                        contract.DefaultCreator = contract.GetUninitializedObject;
                    }
                }
                else if (contract.DefaultCreator == null || contract.DefaultCreatorNonPublic)
                {
                    ConstructorInfo constructor = GetParametrizedConstructor(contract.NonNullableUnderlyingType);
                    if (constructor != null)
                    {
#pragma warning disable 618
                        contract.ParametrizedConstructor = constructor;
#pragma warning restore 618
                        contract.CreatorParameters.AddRange(CreateConstructorParameters(constructor, contract.Properties));
                    }
                }
            }

            MemberInfo extensionDataMember = GetExtensionDataMemberForType(contract.NonNullableUnderlyingType);
            if (extensionDataMember != null)
            {
                SetExtensionDataDelegates(contract, extensionDataMember);
            }

            return contract;
        }

        private MemberInfo GetExtensionDataMemberForType(Type type)
        {
            IEnumerable<MemberInfo> members = GetClassHierarchyForType(type).SelectMany(baseType =>
            {
                IList<MemberInfo> m = new List<MemberInfo>();
                m.AddRange(baseType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                m.AddRange(baseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

                return m;
            });

            MemberInfo extensionDataMember = members.LastOrDefault(m =>
            {
                MemberTypes memberType = m.MemberType();
                if (memberType != MemberTypes.Property && memberType != MemberTypes.Field)
                {
                    return false;
                }

                if (!m.IsDefined(typeof(JsonExtensionDataAttribute), false))
                {
                    return false;
                }

                Type t = ReflectionUtils.GetMemberUnderlyingType(m);

                Type dictionaryType;
                if (ReflectionUtils.ImplementsGenericDefinition(t, typeof(IDictionary<,>), out dictionaryType))
                {
                    Type keyType = dictionaryType.GetGenericArguments()[0];
                    Type valueType = dictionaryType.GetGenericArguments()[1];

                    if (keyType.IsAssignableFrom(typeof(string)) && valueType.IsAssignableFrom(typeof(JToken)))
                    {
                        return true;
                    }
                }

                throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' type must implement IDictionary<string, JToken>.".FormatWith(CultureInfo.InvariantCulture,
                    GetClrTypeFullName(m.DeclaringType), m.Name));
            });

            return extensionDataMember;
        }

        private static void SetExtensionDataDelegates(JsonObjectContract contract, MemberInfo member)
        {
            JsonExtensionDataAttribute extensionDataAttribute = ReflectionUtils.GetAttribute<JsonExtensionDataAttribute>(member);
            if (extensionDataAttribute == null)
            {
                return;
            }

            Type t = ReflectionUtils.GetMemberUnderlyingType(member);

            Type dictionaryType;
            ReflectionUtils.ImplementsGenericDefinition(t, typeof(IDictionary<,>), out dictionaryType);

            Type keyType = dictionaryType.GetGenericArguments()[0];
            Type valueType = dictionaryType.GetGenericArguments()[1];
            bool isJTokenValueType = typeof(JToken).IsAssignableFrom(valueType);

            if (ReflectionUtils.IsGenericDefinition(t, typeof(IDictionary<,>)))
            {
                t = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            }

            MethodInfo addMethod = t.GetMethod("Add", new[] { keyType, valueType });
            Func<object, object> getExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(member);
            Action<object, object> setExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(member);
            Func<object> createExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(t);
            MethodCall<object, object> setExtensionDataDictionaryValue = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(addMethod);

            ExtensionDataSetter extensionDataSetter = (o, key, value) =>
            {
                object dictionary = getExtensionDataDictionary(o);
                if (dictionary == null)
                {
                    dictionary = createExtensionDataDictionary();
                    setExtensionDataDictionary(o, dictionary);
                }

                if (isJTokenValueType && !(value is JToken))
                {
                    value = (value != null) ? JToken.FromObject(value) : JValue.CreateNull();
                }

                setExtensionDataDictionaryValue(dictionary, key, value);
            };

            Type enumerableWrapper = typeof(DictionaryEnumerator<,>).MakeGenericType(keyType, valueType);
            ConstructorInfo constructors = enumerableWrapper.GetConstructors().First();
            ObjectConstructor<object> createEnumerableWrapper = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor(constructors);

            ExtensionDataGetter extensionDataGetter = o =>
            {
                object dictionary = getExtensionDataDictionary(o);
                if (dictionary == null)
                {
                    return null;
                }

                return (IEnumerable<KeyValuePair<object, object>>)createEnumerableWrapper(dictionary);
            };

            if (extensionDataAttribute.ReadData)
            {
                contract.ExtensionDataSetter = extensionDataSetter;
            }

            if (extensionDataAttribute.WriteData)
            {
                contract.ExtensionDataGetter = extensionDataGetter;
            }
        }

        private ConstructorInfo GetAttributeConstructor(Type objectType)
        {
            IList<ConstructorInfo> markedConstructors =
                objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(c => c.IsDefined(typeof(JsonConstructorAttribute), true)).ToList();

            if (markedConstructors.Count > 1)
            {
                throw new JsonException("Multiple constructors with the JsonConstructorAttribute.");
            }
            if (markedConstructors.Count == 1)
            {
                return markedConstructors[0];
            }

            if (objectType == typeof(Version))
            {
                return objectType.GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(int) });
            }

            return null;
        }

        private ConstructorInfo GetParametrizedConstructor(Type objectType)
        {
            IList<ConstructorInfo> constructors = objectType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).ToList();

            if (constructors.Count == 1)
            {
                return constructors[0];
            }
            return null;
        }

        protected virtual IList<JsonProperty> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
        {
            var constructorParameters = constructor.GetParameters();

            JsonPropertyCollection parameterCollection = new JsonPropertyCollection(constructor.DeclaringType);

            foreach (ParameterInfo parameterInfo in constructorParameters)
            {
                JsonProperty matchingMemberProperty = (parameterInfo.Name != null) ? memberProperties.GetClosestMatchProperty(parameterInfo.Name) : null;

                if (matchingMemberProperty != null && matchingMemberProperty.PropertyType != parameterInfo.ParameterType)
                {
                    matchingMemberProperty = null;
                }

                if (matchingMemberProperty != null || parameterInfo.Name != null)
                {
                    JsonProperty property = CreatePropertyFromConstructorParameter(matchingMemberProperty, parameterInfo);

                    if (property != null)
                    {
                        parameterCollection.AddProperty(property);
                    }
                }
            }

            return parameterCollection;
        }

        protected virtual JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
        {
            JsonProperty property = new JsonProperty();
            property.PropertyType = parameterInfo.ParameterType;

            bool allowNonPublicAccess;
            SetPropertySettingsFromAttributes(property, parameterInfo, parameterInfo.Name, parameterInfo.Member.DeclaringType, MemberSerialization.OptOut, out allowNonPublicAccess);

            property.Readable = false;
            property.Writable = true;

            if (matchingMemberProperty != null)
            {
                property.PropertyName = (property.PropertyName != parameterInfo.Name) ? property.PropertyName : matchingMemberProperty.PropertyName;
                property.Converter = property.Converter ?? matchingMemberProperty.Converter;
                property.MemberConverter = property.MemberConverter ?? matchingMemberProperty.MemberConverter;

                if (!property._hasExplicitDefaultValue && matchingMemberProperty._hasExplicitDefaultValue)
                {
                    property.DefaultValue = matchingMemberProperty.DefaultValue;
                }

                property._required = property._required ?? matchingMemberProperty._required;
                property.IsReference = property.IsReference ?? matchingMemberProperty.IsReference;
                property.NullValueHandling = property.NullValueHandling ?? matchingMemberProperty.NullValueHandling;
                property.DefaultValueHandling = property.DefaultValueHandling ?? matchingMemberProperty.DefaultValueHandling;
                property.ReferenceLoopHandling = property.ReferenceLoopHandling ?? matchingMemberProperty.ReferenceLoopHandling;
                property.ObjectCreationHandling = property.ObjectCreationHandling ?? matchingMemberProperty.ObjectCreationHandling;
                property.TypeNameHandling = property.TypeNameHandling ?? matchingMemberProperty.TypeNameHandling;
            }

            return property;
        }

        protected virtual JsonConverter ResolveContractConverter(Type objectType)
        {
            return JsonTypeReflector.GetJsonConverter(objectType);
        }

        private Func<object> GetDefaultCreator(Type createdType)
        {
            return JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(createdType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Runtime.Serialization.DataContractAttribute.#get_IsReference()")]
        private void InitializeContract(JsonContract contract)
        {
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(contract.NonNullableUnderlyingType);
            if (containerAttribute != null)
            {
                contract.IsReference = containerAttribute._isReference;
            }
            else
            {
                DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(contract.NonNullableUnderlyingType);

                if (dataContractAttribute != null && dataContractAttribute.IsReference)
                {
                    contract.IsReference = true;
                }
            }

            contract.Converter = ResolveContractConverter(contract.NonNullableUnderlyingType);

            contract.InternalConverter = JsonSerializer.GetMatchingConverter(BuiltInConverters, contract.NonNullableUnderlyingType);

            if (contract.IsInstantiable
                && (ReflectionUtils.HasDefaultConstructor(contract.CreatedType, true) || contract.CreatedType.IsValueType()))
            {
                contract.DefaultCreator = GetDefaultCreator(contract.CreatedType);

                contract.DefaultCreatorNonPublic = (!contract.CreatedType.IsValueType() &&
                                                    ReflectionUtils.GetDefaultConstructor(contract.CreatedType) == null);
            }

            ResolveCallbackMethods(contract, contract.NonNullableUnderlyingType);
        }

        private void ResolveCallbackMethods(JsonContract contract, Type t)
        {
            List<SerializationCallback> onSerializing;
            List<SerializationCallback> onSerialized;
            List<SerializationCallback> onDeserializing;
            List<SerializationCallback> onDeserialized;
            List<SerializationErrorCallback> onError;

            GetCallbackMethodsForType(t, out onSerializing, out onSerialized, out onDeserializing, out onDeserialized, out onError);

            if (onSerializing != null)
            {
                if (t.Name != FSharpUtils.FSharpSetTypeName && t.Name != FSharpUtils.FSharpMapTypeName)
                {
                    contract.OnSerializingCallbacks.AddRange(onSerializing);
                }
            }

            if (onSerialized != null)
            {
                contract.OnSerializedCallbacks.AddRange(onSerialized);
            }

            if (onDeserializing != null)
            {
                contract.OnDeserializingCallbacks.AddRange(onDeserializing);
            }

            if (onDeserialized != null)
            {
                if (t.Name != FSharpUtils.FSharpSetTypeName && t.Name != FSharpUtils.FSharpMapTypeName)
                {
                    if (!t.IsGenericType() || (t.GetGenericTypeDefinition() != typeof(ConcurrentDictionary<,>)))
                    {
                        contract.OnDeserializedCallbacks.AddRange(onDeserialized);
                    }
                }
            }

            if (onError != null)
            {
                contract.OnErrorCallbacks.AddRange(onError);
            }
        }

        private void GetCallbackMethodsForType(Type type,
            out List<SerializationCallback> onSerializing,
            out List<SerializationCallback> onSerialized,
            out List<SerializationCallback> onDeserializing,
            out List<SerializationCallback> onDeserialized,
            out List<SerializationErrorCallback> onError)
        {
            onSerializing = null;
            onSerialized = null;
            onDeserializing = null;
            onDeserialized = null;
            onError = null;

            foreach (Type baseType in GetClassHierarchyForType(type))
            {
                MethodInfo currentOnSerializing = null;
                MethodInfo currentOnSerialized = null;
                MethodInfo currentOnDeserializing = null;
                MethodInfo currentOnDeserialized = null;
                MethodInfo currentOnError = null;

                foreach (MethodInfo method in baseType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (method.ContainsGenericParameters)
                    {
                        continue;
                    }

                    Type prevAttributeType = null;
                    ParameterInfo[] parameters = method.GetParameters();

                    if (IsValidCallback(method, parameters, typeof(OnSerializingAttribute), currentOnSerializing, ref prevAttributeType))
                    {
                        onSerializing = onSerializing ?? new List<SerializationCallback>();
                        onSerializing.Add(JsonContract.CreateSerializationCallback(method));
                        currentOnSerializing = method;
                    }
                    if (IsValidCallback(method, parameters, typeof(OnSerializedAttribute), currentOnSerialized, ref prevAttributeType))
                    {
                        onSerialized = onSerialized ?? new List<SerializationCallback>();
                        onSerialized.Add(JsonContract.CreateSerializationCallback(method));
                        currentOnSerialized = method;
                    }
                    if (IsValidCallback(method, parameters, typeof(OnDeserializingAttribute), currentOnDeserializing, ref prevAttributeType))
                    {
                        onDeserializing = onDeserializing ?? new List<SerializationCallback>();
                        onDeserializing.Add(JsonContract.CreateSerializationCallback(method));
                        currentOnDeserializing = method;
                    }
                    if (IsValidCallback(method, parameters, typeof(OnDeserializedAttribute), currentOnDeserialized, ref prevAttributeType))
                    {
                        onDeserialized = onDeserialized ?? new List<SerializationCallback>();
                        onDeserialized.Add(JsonContract.CreateSerializationCallback(method));
                        currentOnDeserialized = method;
                    }
                    if (IsValidCallback(method, parameters, typeof(OnErrorAttribute), currentOnError, ref prevAttributeType))
                    {
                        onError = onError ?? new List<SerializationErrorCallback>();
                        onError.Add(JsonContract.CreateSerializationErrorCallback(method));
                        currentOnError = method;
                    }
                }
            }
        }

        private List<Type> GetClassHierarchyForType(Type type)
        {
            List<Type> ret = new List<Type>();

            Type current = type;
            while (current != null && current != typeof(object))
            {
                ret.Add(current);
                current = current.BaseType();
            }

            ret.Reverse();
            return ret;
        }

        protected virtual JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            JsonDictionaryContract contract = new JsonDictionaryContract(objectType);
            InitializeContract(contract);

            contract.PropertyNameResolver = ResolvePropertyName;

            return contract;
        }

        protected virtual JsonArrayContract CreateArrayContract(Type objectType)
        {
            JsonArrayContract contract = new JsonArrayContract(objectType);
            InitializeContract(contract);

            return contract;
        }

        protected virtual JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
        {
            JsonPrimitiveContract contract = new JsonPrimitiveContract(objectType);
            InitializeContract(contract);

            return contract;
        }

        protected virtual JsonLinqContract CreateLinqContract(Type objectType)
        {
            JsonLinqContract contract = new JsonLinqContract(objectType);
            InitializeContract(contract);

            return contract;
        }

        protected virtual JsonISerializableContract CreateISerializableContract(Type objectType)
        {
            JsonISerializableContract contract = new JsonISerializableContract(objectType);
            InitializeContract(contract);

            ConstructorInfo constructorInfo = contract.NonNullableUnderlyingType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
            if (constructorInfo != null)
            {
                ObjectConstructor<object> creator = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor(constructorInfo);

                contract.ISerializableCreator = creator;
            }

            return contract;
        }

        protected virtual JsonDynamicContract CreateDynamicContract(Type objectType)
        {
            JsonDynamicContract contract = new JsonDynamicContract(objectType);
            InitializeContract(contract);

            contract.PropertyNameResolver = ResolvePropertyName;
            contract.Properties.AddRange(CreateProperties(objectType, MemberSerialization.OptOut));

            return contract;
        }

        protected virtual JsonStringContract CreateStringContract(Type objectType)
        {
            JsonStringContract contract = new JsonStringContract(objectType);
            InitializeContract(contract);

            return contract;
        }

        protected virtual JsonContract CreateContract(Type objectType)
        {
            if (IsJsonPrimitiveType(objectType))
            {
                return CreatePrimitiveContract(objectType);
            }

            Type t = ReflectionUtils.EnsureNotNullableType(objectType);
            JsonContainerAttribute containerAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(t);

            if (containerAttribute is JsonObjectAttribute)
            {
                return CreateObjectContract(objectType);
            }

            if (containerAttribute is JsonArrayAttribute)
            {
                return CreateArrayContract(objectType);
            }

            if (containerAttribute is JsonDictionaryAttribute)
            {
                return CreateDictionaryContract(objectType);
            }

            if (t == typeof(JToken) || t.IsSubclassOf(typeof(JToken)))
            {
                return CreateLinqContract(objectType);
            }

            if (CollectionUtils.IsDictionaryType(t))
            {
                return CreateDictionaryContract(objectType);
            }

            if (typeof(IEnumerable).IsAssignableFrom(t))
            {
                return CreateArrayContract(objectType);
            }

            if (CanConvertToString(t))
            {
                return CreateStringContract(objectType);
            }

            if (!IgnoreSerializableInterface && typeof(ISerializable).IsAssignableFrom(t))
            {
                return CreateISerializableContract(objectType);
            }

            if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(t))
            {
                return CreateDynamicContract(objectType);
            }

            if (IsIConvertible(t))
            {
                return CreatePrimitiveContract(t);
            }

            return CreateObjectContract(objectType);
        }

        internal static bool IsJsonPrimitiveType(Type t)
        {
            PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(t);

            return (typeCode != PrimitiveTypeCode.Empty && typeCode != PrimitiveTypeCode.Object);
        }

        internal static bool IsIConvertible(Type t)
        {
            if (typeof(IConvertible).IsAssignableFrom(t)
                || (ReflectionUtils.IsNullableType(t) && typeof(IConvertible).IsAssignableFrom(Nullable.GetUnderlyingType(t))))
            {
                return !typeof(JToken).IsAssignableFrom(t);
            }

            return false;
        }

        internal static bool CanConvertToString(Type type)
        {
            TypeConverter converter = ConvertUtils.GetConverter(type);

            if (converter != null
                && !(converter is ComponentConverter)
                && !(converter is ReferenceConverter)
                && converter.GetType() != typeof(TypeConverter))
            {
                if (converter.CanConvertTo(typeof(string)))
                {
                    return true;
                }
            }

            if (type == typeof(Type) || type.IsSubclassOf(typeof(Type)))
            {
                return true;
            }

            return false;
        }

        private static bool IsValidCallback(MethodInfo method, ParameterInfo[] parameters, Type attributeType, MethodInfo currentCallback, ref Type prevAttributeType)
        {
            if (!method.IsDefined(attributeType, false))
            {
                return false;
            }

            if (currentCallback != null)
            {
                throw new JsonException("Invalid attribute. Both '{0}' and '{1}' in type '{2}' have '{3}'.".FormatWith(CultureInfo.InvariantCulture, method, currentCallback,
                    GetClrTypeFullName(method.DeclaringType), attributeType));
            }

            if (prevAttributeType != null)
            {
                throw new JsonException("Invalid Callback. Method '{3}' in type '{2}' has both '{0}' and '{1}'.".FormatWith(CultureInfo.InvariantCulture, prevAttributeType, attributeType,
                    GetClrTypeFullName(method.DeclaringType), method));
            }

            if (method.IsVirtual)
            {
                throw new JsonException("Virtual Method '{0}' of type '{1}' cannot be marked with '{2}' attribute.".FormatWith(CultureInfo.InvariantCulture, method,
                    GetClrTypeFullName(method.DeclaringType), attributeType));
            }

            if (method.ReturnType != typeof(void))
            {
                throw new JsonException("Serialization Callback '{1}' in type '{0}' must return void.".FormatWith(CultureInfo.InvariantCulture, GetClrTypeFullName(method.DeclaringType), method));
            }

            if (attributeType == typeof(OnErrorAttribute))
            {
                if (parameters == null || parameters.Length != 2 || parameters[0].ParameterType != typeof(StreamingContext) || parameters[1].ParameterType != typeof(ErrorContext))
                {
                    throw new JsonException("Serialization Error Callback '{1}' in type '{0}' must have two parameters of type '{2}' and '{3}'.".FormatWith(CultureInfo.InvariantCulture,
                        GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext), typeof(ErrorContext)));
                }
            }
            else
            {
                if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != typeof(StreamingContext))
                {
                    throw new JsonException("Serialization Callback '{1}' in type '{0}' must have a single parameter of type '{2}'.".FormatWith(CultureInfo.InvariantCulture,
                        GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext)));
                }
            }

            prevAttributeType = attributeType;

            return true;
        }

        internal static string GetClrTypeFullName(Type type)
        {
            if (type.IsGenericTypeDefinition() || !type.ContainsGenericParameters())
            {
                return type.FullName;
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { type.Namespace, type.Name });
        }

        protected virtual IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            List<MemberInfo> members = GetSerializableMembers(type);
            if (members == null)
            {
                throw new JsonSerializationException("Null collection of seralizable members returned.");
            }

            JsonPropertyCollection properties = new JsonPropertyCollection(type);

            foreach (MemberInfo member in members)
            {
                JsonProperty property = CreateProperty(member, memberSerialization);

                if (property != null)
                {
                    DefaultContractResolverState state = GetState();

                    lock (state.NameTable)
                    {
                        property.PropertyName = state.NameTable.Add(property.PropertyName);
                    }

                    properties.AddProperty(property);
                }
            }

            IList<JsonProperty> orderedProperties = properties.OrderBy(p => p.Order ?? -1).ToList();
            return orderedProperties;
        }

        protected virtual IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            IValueProvider valueProvider;

            if (DynamicCodeGeneration)
            {
                valueProvider = new DynamicValueProvider(member);
            }
            else
            {
                valueProvider = new ReflectionValueProvider(member);
            }

            return valueProvider;
        }

        protected virtual JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = new JsonProperty();
            property.PropertyType = ReflectionUtils.GetMemberUnderlyingType(member);
            property.DeclaringType = member.DeclaringType;
            property.ValueProvider = CreateMemberValueProvider(member);

            bool allowNonPublicAccess;
            SetPropertySettingsFromAttributes(property, member, member.Name, member.DeclaringType, memberSerialization, out allowNonPublicAccess);

            if (memberSerialization != MemberSerialization.Fields)
            {
                property.Readable = ReflectionUtils.CanReadMemberValue(member, allowNonPublicAccess);
                property.Writable = ReflectionUtils.CanSetMemberValue(member, allowNonPublicAccess, property.HasMemberAttribute);
            }
            else
            {
                property.Readable = true;
                property.Writable = true;
            }
            property.ShouldSerialize = CreateShouldSerializeTest(member);

            SetIsSpecifiedActions(property, member, allowNonPublicAccess);

            return property;
        }

        private void SetPropertySettingsFromAttributes(JsonProperty property,
            object attributeProvider,
            string name,
            Type declaringType,
            MemberSerialization memberSerialization,
            out bool allowNonPublicAccess)
        {
            DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(declaringType);

            MemberInfo memberInfo = attributeProvider as MemberInfo;

            DataMemberAttribute dataMemberAttribute;
            if (dataContractAttribute != null && memberInfo != null)
            {
                dataMemberAttribute = JsonTypeReflector.GetDataMemberAttribute(memberInfo);
            }
            else
            {
                dataMemberAttribute = null;
            }

            JsonPropertyAttribute propertyAttribute = JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(attributeProvider);
            if (propertyAttribute != null)
            {
                property.HasMemberAttribute = true;
            }

            string mappedName;
            if (propertyAttribute != null && propertyAttribute.PropertyName != null)
            {
                mappedName = propertyAttribute.PropertyName;
            }
            else if (dataMemberAttribute != null && dataMemberAttribute.Name != null)
            {
                mappedName = dataMemberAttribute.Name;
            }
            else
            {
                mappedName = name;
            }

            property.PropertyName = ResolvePropertyName(mappedName);
            property.UnderlyingName = name;

            bool hasMemberAttribute = false;
            if (propertyAttribute != null)
            {
                property._required = propertyAttribute._required;
                property.Order = propertyAttribute._order;
                property.DefaultValueHandling = propertyAttribute._defaultValueHandling;
                hasMemberAttribute = true;
            }
            else if (dataMemberAttribute != null)
            {
                property._required = (dataMemberAttribute.IsRequired) ? Required.AllowNull : Required.Default;
                property.Order = (dataMemberAttribute.Order != -1) ? (int?)dataMemberAttribute.Order : null;
                property.DefaultValueHandling = (!dataMemberAttribute.EmitDefaultValue) ? (DefaultValueHandling?)DefaultValueHandling.Ignore : null;
                hasMemberAttribute = true;
            }

            bool hasJsonIgnoreAttribute =
                JsonTypeReflector.GetAttribute<JsonIgnoreAttribute>(attributeProvider) != null
                || JsonTypeReflector.GetAttribute<JsonExtensionDataAttribute>(attributeProvider) != null
                || JsonTypeReflector.GetAttribute<NonSerializedAttribute>(attributeProvider) != null;

            if (memberSerialization != MemberSerialization.OptIn)
            {
                bool hasIgnoreDataMemberAttribute = false;

                hasIgnoreDataMemberAttribute = (JsonTypeReflector.GetAttribute<IgnoreDataMemberAttribute>(attributeProvider) != null);

                property.Ignored = (hasJsonIgnoreAttribute || hasIgnoreDataMemberAttribute);
            }
            else
            {
                property.Ignored = (hasJsonIgnoreAttribute || !hasMemberAttribute);
            }

            property.Converter = JsonTypeReflector.GetJsonConverter(attributeProvider);
            property.MemberConverter = JsonTypeReflector.GetJsonConverter(attributeProvider);

            DefaultValueAttribute defaultValueAttribute = JsonTypeReflector.GetAttribute<DefaultValueAttribute>(attributeProvider);
            if (defaultValueAttribute != null)
            {
                property.DefaultValue = defaultValueAttribute.Value;
            }

            property.NullValueHandling = (propertyAttribute != null) ? propertyAttribute._nullValueHandling : null;
            property.ReferenceLoopHandling = (propertyAttribute != null) ? propertyAttribute._referenceLoopHandling : null;
            property.ObjectCreationHandling = (propertyAttribute != null) ? propertyAttribute._objectCreationHandling : null;
            property.TypeNameHandling = (propertyAttribute != null) ? propertyAttribute._typeNameHandling : null;
            property.IsReference = (propertyAttribute != null) ? propertyAttribute._isReference : null;

            property.ItemIsReference = (propertyAttribute != null) ? propertyAttribute._itemIsReference : null;
            property.ItemConverter =
                (propertyAttribute != null && propertyAttribute.ItemConverterType != null)
                    ? JsonTypeReflector.CreateJsonConverterInstance(propertyAttribute.ItemConverterType, propertyAttribute.ItemConverterParameters)
                    : null;
            property.ItemReferenceLoopHandling = (propertyAttribute != null) ? propertyAttribute._itemReferenceLoopHandling : null;
            property.ItemTypeNameHandling = (propertyAttribute != null) ? propertyAttribute._itemTypeNameHandling : null;

            allowNonPublicAccess = false;
#pragma warning disable 618
            if ((DefaultMembersSearchFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
            {
                allowNonPublicAccess = true;
            }
#pragma warning restore 618
            if (propertyAttribute != null)
            {
                allowNonPublicAccess = true;
            }
            if (memberSerialization == MemberSerialization.Fields)
            {
                allowNonPublicAccess = true;
            }

            if (dataMemberAttribute != null)
            {
                allowNonPublicAccess = true;
                property.HasMemberAttribute = true;
            }
        }

        private Predicate<object> CreateShouldSerializeTest(MemberInfo member)
        {
            MethodInfo shouldSerializeMethod = member.DeclaringType.GetMethod(JsonTypeReflector.ShouldSerializePrefix + member.Name, ReflectionUtils.EmptyTypes);

            if (shouldSerializeMethod == null || shouldSerializeMethod.ReturnType != typeof(bool))
            {
                return null;
            }

            MethodCall<object, object> shouldSerializeCall =
                JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(shouldSerializeMethod);

            return o => (bool)shouldSerializeCall(o);
        }

        private void SetIsSpecifiedActions(JsonProperty property, MemberInfo member, bool allowNonPublicAccess)
        {
            MemberInfo specifiedMember = member.DeclaringType.GetProperty(member.Name + JsonTypeReflector.SpecifiedPostfix);
            if (specifiedMember == null)
            {
                specifiedMember = member.DeclaringType.GetField(member.Name + JsonTypeReflector.SpecifiedPostfix);
            }

            if (specifiedMember == null || ReflectionUtils.GetMemberUnderlyingType(specifiedMember) != typeof(bool))
            {
                return;
            }

            Func<object, object> specifiedPropertyGet = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(specifiedMember);

            property.GetIsSpecified = o => (bool)specifiedPropertyGet(o);

            if (ReflectionUtils.CanSetMemberValue(specifiedMember, allowNonPublicAccess, false))
            {
                property.SetIsSpecified = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(specifiedMember);
            }
        }

        protected internal virtual string ResolvePropertyName(string propertyName)
        {
            return propertyName;
        }

        public string GetResolvedPropertyName(string propertyName)
        {
            return ResolvePropertyName(propertyName);
        }

        #endregion

        #region Вложенный класс: DictionaryEnumerator

        internal struct DictionaryEnumerator<TEnumeratorKey, TEnumeratorValue> : IEnumerable<KeyValuePair<object, object>>, IEnumerator<KeyValuePair<object, object>>
        {
            #region Поля

            private readonly IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;

            #endregion

            #region Свойства

            public KeyValuePair<object, object> Current
            {
                get { return new KeyValuePair<object, object>(_e.Current.Key, _e.Current.Value); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            #endregion

            #region Конструктор

            public DictionaryEnumerator(IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
            {
                ValidationUtils.ArgumentNotNull(e, "e");
                _e = e.GetEnumerator();
            }

            #endregion

            #region Методы

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            public void Reset()
            {
                _e.Reset();
            }

            public void Dispose()
            {
                _e.Dispose();
            }

            public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            #endregion
        }

        #endregion
    }
}