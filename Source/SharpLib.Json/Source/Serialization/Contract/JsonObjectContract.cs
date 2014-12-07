using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace SharpLib.Json
{
    public class JsonObjectContract : JsonContainerContract
    {
        #region Поля

        private bool? _hasRequiredOrDefaultValueProperties;

        private ConstructorInfo _overrideConstructor;

        private ObjectConstructor<object> _overrideCreator;

        private ConstructorInfo _parametrizedConstructor;

        private ObjectConstructor<object> _parametrizedCreator;

        #endregion

        #region Свойства

        public MemberSerialization MemberSerialization { get; set; }

        public Required? ItemRequired { get; set; }

        public JsonPropertyCollection Properties { get; private set; }

        [Obsolete("ConstructorParameters is obsolete. Use CreatorParameters instead.")]
        public JsonPropertyCollection ConstructorParameters
        {
            get { return CreatorParameters; }
        }

        public JsonPropertyCollection CreatorParameters { get; private set; }

        [Obsolete("OverrideConstructor is obsolete. Use OverrideCreator instead.")]
        public ConstructorInfo OverrideConstructor
        {
            get { return _overrideConstructor; }
            set
            {
                _overrideConstructor = value;
                _overrideCreator = (value != null) ? JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor(value) : null;
            }
        }

        [Obsolete("ParametrizedConstructor is obsolete. Use OverrideCreator instead.")]
        public ConstructorInfo ParametrizedConstructor
        {
            get { return _parametrizedConstructor; }
            set
            {
                _parametrizedConstructor = value;
                _parametrizedCreator = (value != null) ? JsonTypeReflector.ReflectionDelegateFactory.CreateParametrizedConstructor(value) : null;
            }
        }

        public ObjectConstructor<object> OverrideCreator
        {
            get { return _overrideCreator; }
            set
            {
                _overrideCreator = value;
                _overrideConstructor = null;
            }
        }

        internal ObjectConstructor<object> ParametrizedCreator
        {
            get { return _parametrizedCreator; }
        }

        public ExtensionDataSetter ExtensionDataSetter { get; set; }

        public ExtensionDataGetter ExtensionDataGetter { get; set; }

        internal bool HasRequiredOrDefaultValueProperties
        {
            get
            {
                if (_hasRequiredOrDefaultValueProperties == null)
                {
                    _hasRequiredOrDefaultValueProperties = false;

                    if (ItemRequired.GetValueOrDefault(Required.Default) != Required.Default)
                    {
                        _hasRequiredOrDefaultValueProperties = true;
                    }
                    else
                    {
                        foreach (JsonProperty property in Properties)
                        {
                            if (property.Required != Required.Default || ((property.DefaultValueHandling & DefaultValueHandling.Populate) == DefaultValueHandling.Populate) && property.Writable)
                            {
                                _hasRequiredOrDefaultValueProperties = true;
                                break;
                            }
                        }
                    }
                }

                return _hasRequiredOrDefaultValueProperties.Value;
            }
        }

        #endregion

        #region Конструктор

        public JsonObjectContract(Type underlyingType)
            : base(underlyingType)
        {
            ContractType = JsonContractType.Object;

            Properties = new JsonPropertyCollection(UnderlyingType);
            CreatorParameters = new JsonPropertyCollection(UnderlyingType);
        }

        #endregion

        #region Методы

        [SecuritySafeCritical]
        internal object GetUninitializedObject()
        {
            if (!JsonTypeReflector.FullyTrusted)
            {
                throw new JsonException("Insufficient permissions. Creating an uninitialized '{0}' type requires full trust.".FormatWith(CultureInfo.InvariantCulture, NonNullableUnderlyingType));
            }

            return FormatterServices.GetUninitializedObject(NonNullableUnderlyingType);
        }

        #endregion
    }
}