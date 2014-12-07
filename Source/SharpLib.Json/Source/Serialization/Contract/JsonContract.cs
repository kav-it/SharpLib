using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace SharpLib.Json
{
    public delegate void SerializationCallback(object o, StreamingContext context);

    public delegate void SerializationErrorCallback(object o, StreamingContext context, ErrorContext errorContext);

    public delegate void ExtensionDataSetter(object o, string key, object value);

    public delegate IEnumerable<KeyValuePair<object, object>> ExtensionDataGetter(object o);

    public abstract class JsonContract
    {
        #region Поля

        internal JsonContractType ContractType;

        internal ReadType InternalReadType;

        internal bool IsConvertable;

        internal bool IsEnum;

        internal bool IsInstantiable;

        internal bool IsNullable;

        internal bool IsReadOnlyOrFixedSize;

        internal bool IsSealed;

        internal Type NonNullableUnderlyingType;

        private Type _createdType;

        private List<SerializationCallback> _onDeserializedCallbacks;

        private IList<SerializationCallback> _onDeserializingCallbacks;

        private IList<SerializationErrorCallback> _onErrorCallbacks;

        private IList<SerializationCallback> _onSerializedCallbacks;

        private IList<SerializationCallback> _onSerializingCallbacks;

        #endregion

        #region Свойства

        public Type UnderlyingType { get; private set; }

        public Type CreatedType
        {
            get { return _createdType; }
            set
            {
                _createdType = value;

                IsSealed = _createdType.IsSealed();
                IsInstantiable = !(_createdType.IsInterface() || _createdType.IsAbstract());
            }
        }

        public bool? IsReference { get; set; }

        public JsonConverter Converter { get; set; }

        internal JsonConverter InternalConverter { get; set; }

        public IList<SerializationCallback> OnDeserializedCallbacks
        {
            get { return _onDeserializedCallbacks ?? (_onDeserializedCallbacks = new List<SerializationCallback>()); }
        }

        public IList<SerializationCallback> OnDeserializingCallbacks
        {
            get { return _onDeserializingCallbacks ?? (_onDeserializingCallbacks = new List<SerializationCallback>()); }
        }

        public IList<SerializationCallback> OnSerializedCallbacks
        {
            get { return _onSerializedCallbacks ?? (_onSerializedCallbacks = new List<SerializationCallback>()); }
        }

        public IList<SerializationCallback> OnSerializingCallbacks
        {
            get { return _onSerializingCallbacks ?? (_onSerializingCallbacks = new List<SerializationCallback>()); }
        }

        public IList<SerializationErrorCallback> OnErrorCallbacks
        {
            get { return _onErrorCallbacks ?? (_onErrorCallbacks = new List<SerializationErrorCallback>()); }
        }

        public Func<object> DefaultCreator { get; set; }

        public bool DefaultCreatorNonPublic { get; set; }

        #endregion

        #region Конструктор

        internal JsonContract(Type underlyingType)
        {
            ValidationUtils.ArgumentNotNull(underlyingType, "underlyingType");

            UnderlyingType = underlyingType;

            IsNullable = ReflectionUtils.IsNullable(underlyingType);
            NonNullableUnderlyingType = (IsNullable && ReflectionUtils.IsNullableType(underlyingType)) ? Nullable.GetUnderlyingType(underlyingType) : underlyingType;

            CreatedType = NonNullableUnderlyingType;

            IsConvertable = ConvertUtils.IsConvertible(NonNullableUnderlyingType);
            IsEnum = NonNullableUnderlyingType.IsEnum();

            if (NonNullableUnderlyingType == typeof(byte[]))
            {
                InternalReadType = ReadType.ReadAsBytes;
            }
            else if (NonNullableUnderlyingType == typeof(int))
            {
                InternalReadType = ReadType.ReadAsInt32;
            }
            else if (NonNullableUnderlyingType == typeof(decimal))
            {
                InternalReadType = ReadType.ReadAsDecimal;
            }
            else if (NonNullableUnderlyingType == typeof(string))
            {
                InternalReadType = ReadType.ReadAsString;
            }
            else if (NonNullableUnderlyingType == typeof(DateTime))
            {
                InternalReadType = ReadType.ReadAsDateTime;
            }
#if !NET20
            else if (NonNullableUnderlyingType == typeof(DateTimeOffset))
            {
                InternalReadType = ReadType.ReadAsDateTimeOffset;
            }
#endif
            else
            {
                InternalReadType = ReadType.Read;
            }
        }

        #endregion

        #region Методы

        internal void InvokeOnSerializing(object o, StreamingContext context)
        {
            if (_onSerializingCallbacks != null)
            {
                foreach (SerializationCallback callback in _onSerializingCallbacks)
                {
                    callback(o, context);
                }
            }
        }

        internal void InvokeOnSerialized(object o, StreamingContext context)
        {
            if (_onSerializedCallbacks != null)
            {
                foreach (SerializationCallback callback in _onSerializedCallbacks)
                {
                    callback(o, context);
                }
            }
        }

        internal void InvokeOnDeserializing(object o, StreamingContext context)
        {
            if (_onDeserializingCallbacks != null)
            {
                foreach (SerializationCallback callback in _onDeserializingCallbacks)
                {
                    callback(o, context);
                }
            }
        }

        internal void InvokeOnDeserialized(object o, StreamingContext context)
        {
            if (_onDeserializedCallbacks != null)
            {
                foreach (SerializationCallback callback in _onDeserializedCallbacks)
                {
                    callback(o, context);
                }
            }
        }

        internal void InvokeOnError(object o, StreamingContext context, ErrorContext errorContext)
        {
            if (_onErrorCallbacks != null)
            {
                foreach (SerializationErrorCallback callback in _onErrorCallbacks)
                {
                    callback(o, context, errorContext);
                }
            }
        }

        internal static SerializationCallback CreateSerializationCallback(MethodInfo callbackMethodInfo)
        {
            return (o, context) => callbackMethodInfo.Invoke(o, new object[] { context });
        }

        internal static SerializationErrorCallback CreateSerializationErrorCallback(MethodInfo callbackMethodInfo)
        {
            return (o, context, econtext) => callbackMethodInfo.Invoke(o, new object[] { context, econtext });
        }

        #endregion
    }
}