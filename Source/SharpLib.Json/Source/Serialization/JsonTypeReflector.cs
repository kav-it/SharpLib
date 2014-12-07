using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace SharpLib.Json
{
    internal static class JsonTypeReflector
    {
        #region Константы

        public const string ArrayValuesPropertyName = "$values";

        public const string IdPropertyName = "$id";

        private const string MetadataTypeAttributeTypeName =
            "System.ComponentModel.DataAnnotations.MetadataTypeAttribute, System.ComponentModel.DataAnnotations, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

        public const string RefPropertyName = "$ref";

        public const string ShouldSerializePrefix = "ShouldSerialize";

        public const string SpecifiedPostfix = "Specified";

        public const string TypePropertyName = "$type";

        public const string ValuePropertyName = "$value";

        #endregion

        #region Поля

        private static readonly ThreadSafeStore<Type, Type> AssociatedMetadataTypesCache = new ThreadSafeStore<Type, Type>(GetAssociateMetadataTypeFromAttribute);

        private static readonly ThreadSafeStore<Type, Func<object[], JsonConverter>> JsonConverterCreatorCache =
            new ThreadSafeStore<Type, Func<object[], JsonConverter>>(GetJsonConverterCreator);

        private static Type _cachedMetadataTypeAttributeType;

        private static bool? _dynamicCodeGeneration;

        private static bool? _fullyTrusted;

        private static ReflectionObject _metadataTypeAttributeReflectionObject;

        #endregion

        #region Свойства

        public static bool DynamicCodeGeneration
        {
            [SecuritySafeCritical]
            get
            {
                if (_dynamicCodeGeneration == null)
                {
                    try
                    {
                        new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
                        new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess).Demand();
                        new SecurityPermission(SecurityPermissionFlag.SkipVerification).Demand();
                        new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                        new SecurityPermission(PermissionState.Unrestricted).Demand();
                        _dynamicCodeGeneration = true;
                    }
                    catch (Exception)
                    {
                        _dynamicCodeGeneration = false;
                    }
                }

                return _dynamicCodeGeneration.Value;
            }
        }

        public static bool FullyTrusted
        {
            get
            {
                if (_fullyTrusted == null)
                {
                    AppDomain appDomain = AppDomain.CurrentDomain;

                    _fullyTrusted = appDomain.IsHomogenous && appDomain.IsFullyTrusted;
                }

                return _fullyTrusted.Value;
            }
        }

        public static ReflectionDelegateFactory ReflectionDelegateFactory
        {
            get
            {
                if (DynamicCodeGeneration)
                {
                    return DynamicReflectionDelegateFactory.Instance;
                }

                return LateBoundReflectionDelegateFactory.Instance;
            }
        }

        #endregion

        #region Методы

        public static T GetCachedAttribute<T>(object attributeProvider) where T : Attribute
        {
            return CachedAttributeGetter<T>.GetAttribute(attributeProvider);
        }

        public static DataContractAttribute GetDataContractAttribute(Type type)
        {
            Type currentType = type;

            while (currentType != null)
            {
                DataContractAttribute result = CachedAttributeGetter<DataContractAttribute>.GetAttribute(currentType);
                if (result != null)
                {
                    return result;
                }

                currentType = currentType.BaseType();
            }

            return null;
        }

        public static DataMemberAttribute GetDataMemberAttribute(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType() == MemberTypes.Field)
            {
                return CachedAttributeGetter<DataMemberAttribute>.GetAttribute(memberInfo);
            }

            PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
            DataMemberAttribute result = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(propertyInfo);
            if (result == null)
            {
                if (propertyInfo.IsVirtual())
                {
                    Type currentType = propertyInfo.DeclaringType;

                    while (result == null && currentType != null)
                    {
                        PropertyInfo baseProperty = (PropertyInfo)ReflectionUtils.GetMemberInfoFromType(currentType, propertyInfo);
                        if (baseProperty != null && baseProperty.IsVirtual())
                        {
                            result = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(baseProperty);
                        }

                        currentType = currentType.BaseType();
                    }
                }
            }

            return result;
        }

        public static MemberSerialization GetObjectMemberSerialization(Type objectType, bool ignoreSerializableAttribute)
        {
            JsonObjectAttribute objectAttribute = GetCachedAttribute<JsonObjectAttribute>(objectType);
            if (objectAttribute != null)
            {
                return objectAttribute.MemberSerialization;
            }

            DataContractAttribute dataContractAttribute = GetDataContractAttribute(objectType);
            if (dataContractAttribute != null)
            {
                return MemberSerialization.OptIn;
            }

            if (!ignoreSerializableAttribute)
            {
                SerializableAttribute serializableAttribute = GetCachedAttribute<SerializableAttribute>(objectType);
                if (serializableAttribute != null)
                {
                    return MemberSerialization.Fields;
                }
            }

            return MemberSerialization.OptOut;
        }

        public static JsonConverter GetJsonConverter(object attributeProvider)
        {
            JsonConverterAttribute converterAttribute = GetCachedAttribute<JsonConverterAttribute>(attributeProvider);

            if (converterAttribute != null)
            {
                Func<object[], JsonConverter> creator = JsonConverterCreatorCache.Get(converterAttribute.ConverterType);
                if (creator != null)
                {
                    return creator(converterAttribute.ConverterParameters);
                }
            }

            return null;
        }

        public static JsonConverter CreateJsonConverterInstance(Type converterType, object[] converterArgs)
        {
            Func<object[], JsonConverter> converterCreator = JsonConverterCreatorCache.Get(converterType);
            return converterCreator(converterArgs);
        }

        private static Func<object[], JsonConverter> GetJsonConverterCreator(Type converterType)
        {
            Func<object> defaultConstructor = (ReflectionUtils.HasDefaultConstructor(converterType, false))
                ? ReflectionDelegateFactory.CreateDefaultConstructor<object>(converterType)
                : null;

            return parameters =>
            {
                try
                {
                    if (parameters != null)
                    {
                        Type[] paramTypes = parameters.Select(param => param.GetType()).ToArray();
                        ConstructorInfo parameterizedConstructorInfo = converterType.GetConstructor(paramTypes);

                        if (null != parameterizedConstructorInfo)
                        {
                            var parameterizedConstructor = ReflectionDelegateFactory.CreateParametrizedConstructor(parameterizedConstructorInfo);
                            return (JsonConverter)parameterizedConstructor(parameters);
                        }
                        throw new JsonException("No matching parameterized constructor found for '{0}'.".FormatWith(CultureInfo.InvariantCulture, converterType));
                    }

                    if (defaultConstructor == null)
                    {
                        throw new JsonException("No parameterless constructor defined for '{0}'.".FormatWith(CultureInfo.InvariantCulture, converterType));
                    }

                    return (JsonConverter)defaultConstructor();
                }
                catch (Exception ex)
                {
                    throw new JsonException("Error creating '{0}'.".FormatWith(CultureInfo.InvariantCulture, converterType), ex);
                }
            };
        }

        public static TypeConverter GetTypeConverter(Type type)
        {
            return TypeDescriptor.GetConverter(type);
        }

        private static Type GetAssociatedMetadataType(Type type)
        {
            return AssociatedMetadataTypesCache.Get(type);
        }

        private static Type GetAssociateMetadataTypeFromAttribute(Type type)
        {
            Type metadataTypeAttributeType = GetMetadataTypeAttributeType();
            if (metadataTypeAttributeType == null)
            {
                return null;
            }

            Attribute attribute = ReflectionUtils.GetAttributes(type, metadataTypeAttributeType, true).SingleOrDefault();
            if (attribute == null)
            {
                return null;
            }

            const string METADATA_CLASS_TYPE_NAME = "MetadataClassType";

            if (_metadataTypeAttributeReflectionObject == null)
            {
                _metadataTypeAttributeReflectionObject = ReflectionObject.Create(metadataTypeAttributeType, METADATA_CLASS_TYPE_NAME);
            }

            return (Type)_metadataTypeAttributeReflectionObject.GetValue(attribute, METADATA_CLASS_TYPE_NAME);
        }

        private static Type GetMetadataTypeAttributeType()
        {
            if (_cachedMetadataTypeAttributeType == null)
            {
                Type metadataTypeAttributeType = Type.GetType(MetadataTypeAttributeTypeName);

                if (metadataTypeAttributeType != null)
                {
                    _cachedMetadataTypeAttributeType = metadataTypeAttributeType;
                }
                else
                {
                    return null;
                }
            }

            return _cachedMetadataTypeAttributeType;
        }

        private static T GetAttribute<T>(Type type) where T : Attribute
        {
            T attribute;

            Type metadataType = GetAssociatedMetadataType(type);
            if (metadataType != null)
            {
                attribute = ReflectionUtils.GetAttribute<T>(metadataType, true);
                if (attribute != null)
                {
                    return attribute;
                }
            }

            attribute = ReflectionUtils.GetAttribute<T>(type, true);
            if (attribute != null)
            {
                return attribute;
            }

            foreach (Type typeInterface in type.GetInterfaces())
            {
                attribute = ReflectionUtils.GetAttribute<T>(typeInterface, true);
                if (attribute != null)
                {
                    return attribute;
                }
            }

            return null;
        }

        private static T GetAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            T attribute;

            Type metadataType = GetAssociatedMetadataType(memberInfo.DeclaringType);
            if (metadataType != null)
            {
                MemberInfo metadataTypeMemberInfo = ReflectionUtils.GetMemberInfoFromType(metadataType, memberInfo);

                if (metadataTypeMemberInfo != null)
                {
                    attribute = ReflectionUtils.GetAttribute<T>(metadataTypeMemberInfo, true);
                    if (attribute != null)
                    {
                        return attribute;
                    }
                }
            }

            attribute = ReflectionUtils.GetAttribute<T>(memberInfo, true);
            if (attribute != null)
            {
                return attribute;
            }

            if (memberInfo.DeclaringType != null)
            {
                foreach (Type typeInterface in memberInfo.DeclaringType.GetInterfaces())
                {
                    MemberInfo interfaceTypeMemberInfo = ReflectionUtils.GetMemberInfoFromType(typeInterface, memberInfo);

                    if (interfaceTypeMemberInfo != null)
                    {
                        attribute = ReflectionUtils.GetAttribute<T>(interfaceTypeMemberInfo, true);
                        if (attribute != null)
                        {
                            return attribute;
                        }
                    }
                }
            }

            return null;
        }

        public static T GetAttribute<T>(object provider) where T : Attribute
        {
            Type type = provider as Type;
            if (type != null)
            {
                return GetAttribute<T>(type);
            }

            MemberInfo memberInfo = provider as MemberInfo;
            if (memberInfo != null)
            {
                return GetAttribute<T>(memberInfo);
            }

            return ReflectionUtils.GetAttribute<T>(provider, true);
        }

        internal static void SetFullyTrusted(bool fullyTrusted)
        {
#if DEBUG
            _fullyTrusted = fullyTrusted;
#endif
        }

        internal static void SetDynamicCodeGeneration(bool dynamicCodeGeneration)
        {
#if DEBUG
            _dynamicCodeGeneration = dynamicCodeGeneration;
#endif
        }

        #endregion
    }
}