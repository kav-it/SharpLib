using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace SharpLib.Json
{
    public class JsonArrayContract : JsonContainerContract
    {
        #region Поля

        private readonly Type _genericCollectionDefinitionType;

        private readonly ConstructorInfo _parametrizedConstructor;

        private Func<object> _genericTemporaryCollectionCreator;

        private ObjectConstructor<object> _genericWrapperCreator;

        private Type _genericWrapperType;

        private ObjectConstructor<object> _parametrizedCreator;

        #endregion

        #region Свойства

        public Type CollectionItemType { get; private set; }

        public bool IsMultidimensionalArray { get; private set; }

        internal bool IsArray { get; private set; }

        internal bool ShouldCreateWrapper { get; private set; }

        internal bool CanDeserialize { get; private set; }

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

        public JsonArrayContract(Type underlyingType)
            : base(underlyingType)
        {
            ContractType = JsonContractType.Array;
            IsArray = CreatedType.IsArray;

            bool canDeserialize;

            Type tempCollectionType;
            if (IsArray)
            {
                CollectionItemType = ReflectionUtils.GetCollectionItemType(UnderlyingType);
                IsReadOnlyOrFixedSize = true;
                _genericCollectionDefinitionType = typeof(List<>).MakeGenericType(CollectionItemType);

                canDeserialize = true;
                IsMultidimensionalArray = (IsArray && UnderlyingType.GetArrayRank() > 1);
            }
            else if (typeof(IList).IsAssignableFrom(underlyingType))
            {
                CollectionItemType = ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out _genericCollectionDefinitionType) 
                    ? _genericCollectionDefinitionType.GetGenericArguments()[0] 
                    : ReflectionUtils.GetCollectionItemType(underlyingType);

                if (underlyingType == typeof(IList))
                {
                    CreatedType = typeof(List<object>);
                }

                if (CollectionItemType != null)
                {
                    _parametrizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, CollectionItemType);
                }

                IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof(ReadOnlyCollection<>));
                canDeserialize = true;
            }
            else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out _genericCollectionDefinitionType))
            {
                CollectionItemType = _genericCollectionDefinitionType.GetGenericArguments()[0];

                if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(ICollection<>))
                    || ReflectionUtils.IsGenericDefinition(underlyingType, typeof(IList<>)))
                {
                    CreatedType = typeof(List<>).MakeGenericType(CollectionItemType);
                }

#if !(NET20 || NET35 || PORTABLE40)
                if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(ISet<>)))
                {
                    CreatedType = typeof(HashSet<>).MakeGenericType(CollectionItemType);
                }
#endif

                _parametrizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, CollectionItemType);
                canDeserialize = true;
                ShouldCreateWrapper = true;
            }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
            else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IReadOnlyCollection<>), out tempCollectionType))
            {
                CollectionItemType = tempCollectionType.GetGenericArguments()[0];

                if (ReflectionUtils.IsGenericDefinition(underlyingType, typeof(IReadOnlyCollection<>))
                    || ReflectionUtils.IsGenericDefinition(underlyingType, typeof(IReadOnlyList<>)))
                    CreatedType = typeof(ReadOnlyCollection<>).MakeGenericType(CollectionItemType);

                _genericCollectionDefinitionType = typeof(List<>).MakeGenericType(CollectionItemType);
                _parametrizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(CreatedType, CollectionItemType);
                IsReadOnlyOrFixedSize = true;
                canDeserialize = HasParametrizedCreator;
            }
#endif
            else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IEnumerable<>), out tempCollectionType))
            {
                CollectionItemType = tempCollectionType.GetGenericArguments()[0];

                if (ReflectionUtils.IsGenericDefinition(UnderlyingType, typeof(IEnumerable<>)))
                {
                    CreatedType = typeof(List<>).MakeGenericType(CollectionItemType);
                }

                _parametrizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(underlyingType, CollectionItemType);

#if !(NET35 || NET20 || NETFX_CORE)
                if (!HasParametrizedCreator && underlyingType.Name == FSharpUtils.FSharpListTypeName)
                {
                    FSharpUtils.EnsureInitialized(underlyingType.Assembly());
                    _parametrizedCreator = FSharpUtils.CreateSeq(CollectionItemType);
                }
#endif

                if (underlyingType.IsGenericType() && underlyingType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    _genericCollectionDefinitionType = tempCollectionType;

                    IsReadOnlyOrFixedSize = false;
                    ShouldCreateWrapper = false;
                    canDeserialize = true;
                }
                else
                {
                    _genericCollectionDefinitionType = typeof(List<>).MakeGenericType(CollectionItemType);

                    IsReadOnlyOrFixedSize = true;
                    ShouldCreateWrapper = true;
                    canDeserialize = HasParametrizedCreator;
                }
            }
            else
            {
                canDeserialize = false;
                ShouldCreateWrapper = true;
            }

            CanDeserialize = canDeserialize;

#if (NET20 || NET35)
            if (CollectionItemType != null && ReflectionUtils.IsNullableType(CollectionItemType))
            {
                
                
                if (ReflectionUtils.InheritsGenericDefinition(CreatedType, typeof(List<>), out tempCollectionType)
                    || (IsArray && !IsMultidimensionalArray))
                {
                    ShouldCreateWrapper = true;
                }
            }
#endif

#if !(NET20 || NET35 || NET40 || PORTABLE40)
            Type immutableCreatedType;
            ObjectConstructor<object> immutableParameterizedCreator;
            if (ImmutableCollectionsUtils.TryBuildImmutableForArrayContract(underlyingType, CollectionItemType, out immutableCreatedType, out immutableParameterizedCreator))
            {
                CreatedType = immutableCreatedType;
                _parametrizedCreator = immutableParameterizedCreator;
                IsReadOnlyOrFixedSize = true;
                CanDeserialize = true;
            }
#endif
        }

        #endregion

        #region Методы

        internal IWrappedCollection CreateWrapper(object list)
        {
            if (_genericWrapperCreator == null)
            {
                _genericWrapperType = typeof(CollectionWrapper<>).MakeGenericType(CollectionItemType);

                Type constructorArgument;

                if (ReflectionUtils.InheritsGenericDefinition(_genericCollectionDefinitionType, typeof(List<>))
                    || _genericCollectionDefinitionType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    constructorArgument = typeof(ICollection<>).MakeGenericType(CollectionItemType);
                }
                else
                {
                    constructorArgument = _genericCollectionDefinitionType;
                }

                ConstructorInfo genericWrapperConstructor = _genericWrapperType.GetConstructor(new[] { constructorArgument });
                _genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor(genericWrapperConstructor);
            }

            return (IWrappedCollection)_genericWrapperCreator(list);
        }

        internal IList CreateTemporaryCollection()
        {
            if (_genericTemporaryCollectionCreator == null)
            {
                Type collectionItemType = (IsMultidimensionalArray) ? typeof(object) : CollectionItemType;
                Type temporaryListType = typeof(List<>).MakeGenericType(collectionItemType);
                _genericTemporaryCollectionCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(temporaryListType);
            }

            return (IList)_genericTemporaryCollectionCreator();
        }

        #endregion
    }
}