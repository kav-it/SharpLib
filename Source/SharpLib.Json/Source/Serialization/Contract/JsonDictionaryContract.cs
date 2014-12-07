using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SharpLib.Json
{
    public class JsonDictionaryContract : JsonContainerContract
    {
        #region Поля

        private readonly Type _genericCollectionDefinitionType;

        private readonly ConstructorInfo _parametrizedConstructor;

        private Func<object> _genericTemporaryDictionaryCreator;

        private ObjectConstructor<object> _genericWrapperCreator;

        private Type _genericWrapperType;

        private ObjectConstructor<object> _parametrizedCreator;

        #endregion

        #region Свойства

        public Func<string, string> PropertyNameResolver { get; set; }

        public Type DictionaryKeyType { get; private set; }

        public Type DictionaryValueType { get; private set; }

        internal JsonContract KeyContract { get; set; }

        internal bool ShouldCreateWrapper { get; private set; }

        internal ObjectConstructor<object> ParametrizedCreator
        {
            get { return _parametrizedCreator ?? (_parametrizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor(_parametrizedConstructor)); }
        }

        internal bool HasParametrizedCreator
        {
            get { return _parametrizedCreator != null || _parametrizedConstructor != null; }
        }

        #endregion

        #region Конструктор

        public JsonDictionaryContract(Type underlyingType)
            : base(underlyingType)
        {
            ContractType = JsonContractType.Dictionary;

            Type keyType;
            Type valueType;

            if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IDictionary<,>), out _genericCollectionDefinitionType))
            {
                keyType = _genericCollectionDefinitionType.GetGenericArguments()[0];
                valueType = _genericCollectionDefinitionType.GetGenericArguments()[1];

                if (ReflectionUtils.IsGenericDefinition(UnderlyingType, typeof(IDictionary<,>)))
                {
                    CreatedType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                }

#if !(NET40 || NET35 || NET20 || PORTABLE40)
                IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof(ReadOnlyDictionary<,>));
#endif
            }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
            else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IReadOnlyDictionary<,>), out _genericCollectionDefinitionType))
            {
                keyType = _genericCollectionDefinitionType.GetGenericArguments()[0];
                valueType = _genericCollectionDefinitionType.GetGenericArguments()[1];

                if (ReflectionUtils.IsGenericDefinition(UnderlyingType, typeof(IReadOnlyDictionary<,>)))
                    CreatedType = typeof(ReadOnlyDictionary<,>).MakeGenericType(keyType, valueType);

                IsReadOnlyOrFixedSize = true;
            }
#endif
            else
            {
                ReflectionUtils.GetDictionaryKeyValueTypes(UnderlyingType, out keyType, out valueType);

                if (UnderlyingType == typeof(IDictionary))
                {
                    CreatedType = typeof(Dictionary<object, object>);
                }
            }

            if (keyType != null && valueType != null)
            {
                _parametrizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(CreatedType, typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType));

#if !(NET35 || NET20 || NETFX_CORE)
                if (!HasParametrizedCreator && underlyingType.Name == FSharpUtils.FSharpMapTypeName)
                {
                    FSharpUtils.EnsureInitialized(underlyingType.Assembly());
                    _parametrizedCreator = FSharpUtils.CreateMap(keyType, valueType);
                }
#endif
            }

            ShouldCreateWrapper = !typeof(IDictionary).IsAssignableFrom(CreatedType);

            DictionaryKeyType = keyType;
            DictionaryValueType = valueType;

#if (NET20 || NET35)
            if (DictionaryValueType != null && ReflectionUtils.IsNullableType(DictionaryValueType))
            {
                Type tempDictioanryType;

                
                
                if (ReflectionUtils.InheritsGenericDefinition(CreatedType, typeof(Dictionary<,>), out tempDictioanryType))
                {
                    ShouldCreateWrapper = true;
                }
            }
#endif

#if !(NET20 || NET35 || NET40 || PORTABLE40)
            Type immutableCreatedType;
            ObjectConstructor<object> immutableParameterizedCreator;
            if (ImmutableCollectionsUtils.TryBuildImmutableForDictionaryContract(underlyingType, DictionaryKeyType, DictionaryValueType, out immutableCreatedType, out immutableParameterizedCreator))
            {
                CreatedType = immutableCreatedType;
                _parametrizedCreator = immutableParameterizedCreator;
                IsReadOnlyOrFixedSize = true;
            }
#endif
        }

        #endregion

        #region Методы

        internal IWrappedDictionary CreateWrapper(object dictionary)
        {
            if (_genericWrapperCreator == null)
            {
                _genericWrapperType = typeof(DictionaryWrapper<,>).MakeGenericType(DictionaryKeyType, DictionaryValueType);

                ConstructorInfo genericWrapperConstructor = _genericWrapperType.GetConstructor(new[] { _genericCollectionDefinitionType });
                _genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor(genericWrapperConstructor);
            }

            return (IWrappedDictionary)_genericWrapperCreator(dictionary);
        }

        internal IDictionary CreateTemporaryDictionary()
        {
            if (_genericTemporaryDictionaryCreator == null)
            {
                Type temporaryDictionaryType = typeof(Dictionary<,>).MakeGenericType(DictionaryKeyType, DictionaryValueType);

                _genericTemporaryDictionaryCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(temporaryDictionaryType);
            }

            return (IDictionary)_genericTemporaryDictionaryCreator();
        }

        #endregion
    }
}