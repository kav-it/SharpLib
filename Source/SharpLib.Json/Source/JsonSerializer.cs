using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace SharpLib.Json
{
    public class JsonSerializer
    {
        #region Поля

        internal SerializationBinder _binder;

        private bool? _checkAdditionalContent;

        internal ConstructorHandling _constructorHandling;

        internal StreamingContext _context;

        internal IContractResolver _contractResolver;

        internal JsonConverterCollection _converters;

        private CultureInfo _culture;

        private DateFormatHandling? _dateFormatHandling;

        private string _dateFormatString;

        private bool _dateFormatStringSet;

        private DateParseHandling? _dateParseHandling;

        private DateTimeZoneHandling? _dateTimeZoneHandling;

        internal DefaultValueHandling _defaultValueHandling;

        private FloatFormatHandling? _floatFormatHandling;

        private FloatParseHandling? _floatParseHandling;

        private JsonFormatting? _formatting;

        private int? _maxDepth;

        private bool _maxDepthSet;

        internal MetadataPropertyHandling _metadataPropertyHandling;

        internal MissingMemberHandling _missingMemberHandling;

        internal NullValueHandling _nullValueHandling;

        internal ObjectCreationHandling _objectCreationHandling;

        internal PreserveReferencesHandling _preserveReferencesHandling;

        internal ReferenceLoopHandling _referenceLoopHandling;

        private IReferenceResolver _referenceResolver;

        private StringEscapeHandling? _stringEscapeHandling;

        internal ITraceWriter _traceWriter;

        internal FormatterAssemblyStyle _typeNameAssemblyFormat;

        internal TypeNameHandling _typeNameHandling;

        #endregion

        #region Свойства

        public virtual IReferenceResolver ReferenceResolver
        {
            get { return GetReferenceResolver(); }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Reference resolver cannot be null.");
                }

                _referenceResolver = value;
            }
        }

        public virtual SerializationBinder Binder
        {
            get { return _binder; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Serialization binder cannot be null.");
                }

                _binder = value;
            }
        }

        public virtual ITraceWriter TraceWriter
        {
            get { return _traceWriter; }
            set { _traceWriter = value; }
        }

        public virtual TypeNameHandling TypeNameHandling
        {
            get { return _typeNameHandling; }
            set
            {
                if (value < TypeNameHandling.None || value > TypeNameHandling.Auto)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _typeNameHandling = value;
            }
        }

        public virtual FormatterAssemblyStyle TypeNameAssemblyFormat
        {
            get { return _typeNameAssemblyFormat; }
            set
            {
                if (value < FormatterAssemblyStyle.Simple || value > FormatterAssemblyStyle.Full)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _typeNameAssemblyFormat = value;
            }
        }

        public virtual PreserveReferencesHandling PreserveReferencesHandling
        {
            get { return _preserveReferencesHandling; }
            set
            {
                if (value < PreserveReferencesHandling.None || value > PreserveReferencesHandling.All)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _preserveReferencesHandling = value;
            }
        }

        public virtual ReferenceLoopHandling ReferenceLoopHandling
        {
            get { return _referenceLoopHandling; }
            set
            {
                if (value < ReferenceLoopHandling.Error || value > ReferenceLoopHandling.Serialize)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _referenceLoopHandling = value;
            }
        }

        public virtual MissingMemberHandling MissingMemberHandling
        {
            get { return _missingMemberHandling; }
            set
            {
                if (value < MissingMemberHandling.Ignore || value > MissingMemberHandling.Error)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _missingMemberHandling = value;
            }
        }

        public virtual NullValueHandling NullValueHandling
        {
            get { return _nullValueHandling; }
            set
            {
                if (value < NullValueHandling.Include || value > NullValueHandling.Ignore)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _nullValueHandling = value;
            }
        }

        public virtual DefaultValueHandling DefaultValueHandling
        {
            get { return _defaultValueHandling; }
            set
            {
                if (value < DefaultValueHandling.Include || value > DefaultValueHandling.IgnoreAndPopulate)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _defaultValueHandling = value;
            }
        }

        public virtual ObjectCreationHandling ObjectCreationHandling
        {
            get { return _objectCreationHandling; }
            set
            {
                if (value < ObjectCreationHandling.Auto || value > ObjectCreationHandling.Replace)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _objectCreationHandling = value;
            }
        }

        public virtual ConstructorHandling ConstructorHandling
        {
            get { return _constructorHandling; }
            set
            {
                if (value < ConstructorHandling.Default || value > ConstructorHandling.AllowNonPublicDefaultConstructor)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _constructorHandling = value;
            }
        }

        public virtual MetadataPropertyHandling MetadataPropertyHandling
        {
            get { return _metadataPropertyHandling; }
            set
            {
                if (value < MetadataPropertyHandling.Default || value > MetadataPropertyHandling.Ignore)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _metadataPropertyHandling = value;
            }
        }

        public virtual JsonConverterCollection Converters
        {
            get { return _converters ?? (_converters = new JsonConverterCollection()); }
        }

        public virtual IContractResolver ContractResolver
        {
            get { return _contractResolver; }
            set { _contractResolver = value ?? DefaultContractResolver.Instance; }
        }

        public virtual StreamingContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public virtual JsonFormatting Formatting
        {
            get { return _formatting ?? JsonSerializerSettings.DEFAULT_FORMATTING; }
            set { _formatting = value; }
        }

        public virtual DateFormatHandling DateFormatHandling
        {
            get { return _dateFormatHandling ?? JsonSerializerSettings.DEFAULT_DATE_FORMAT_HANDLING; }
            set { _dateFormatHandling = value; }
        }

        public virtual DateTimeZoneHandling DateTimeZoneHandling
        {
            get { return _dateTimeZoneHandling ?? JsonSerializerSettings.DEFAULT_DATE_TIME_ZONE_HANDLING; }
            set { _dateTimeZoneHandling = value; }
        }

        public virtual DateParseHandling DateParseHandling
        {
            get { return _dateParseHandling ?? JsonSerializerSettings.DEFAULT_DATE_PARSE_HANDLING; }
            set { _dateParseHandling = value; }
        }

        public virtual FloatParseHandling FloatParseHandling
        {
            get { return _floatParseHandling ?? JsonSerializerSettings.DEFAULT_FLOAT_PARSE_HANDLING; }
            set { _floatParseHandling = value; }
        }

        public virtual FloatFormatHandling FloatFormatHandling
        {
            get { return _floatFormatHandling ?? JsonSerializerSettings.DEFAULT_FLOAT_FORMAT_HANDLING; }
            set { _floatFormatHandling = value; }
        }

        public virtual StringEscapeHandling StringEscapeHandling
        {
            get { return _stringEscapeHandling ?? JsonSerializerSettings.DEFAULT_STRING_ESCAPE_HANDLING; }
            set { _stringEscapeHandling = value; }
        }

        public virtual string DateFormatString
        {
            get { return _dateFormatString ?? JsonSerializerSettings.DEFAULT_DATE_FORMAT_STRING; }
            set
            {
                _dateFormatString = value;
                _dateFormatStringSet = true;
            }
        }

        public virtual CultureInfo Culture
        {
            get { return _culture ?? JsonSerializerSettings.DefaultCulture; }
            set { _culture = value; }
        }

        public virtual int? MaxDepth
        {
            get { return _maxDepth; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Value must be positive.", "value");
                }

                _maxDepth = value;
                _maxDepthSet = true;
            }
        }

        public virtual bool CheckAdditionalContent
        {
            get { return _checkAdditionalContent ?? JsonSerializerSettings.DEFAULT_CHECK_ADDITIONAL_CONTENT; }
            set { _checkAdditionalContent = value; }
        }

        #endregion

        #region События

        public virtual event EventHandler<ErrorEventArgs> Error;

        #endregion

        #region Конструктор

        public JsonSerializer()
        {
            _referenceLoopHandling = JsonSerializerSettings.DEFAULT_REFERENCE_LOOP_HANDLING;
            _missingMemberHandling = JsonSerializerSettings.DEFAULT_MISSING_MEMBER_HANDLING;
            _nullValueHandling = JsonSerializerSettings.DEFAULT_NULL_VALUE_HANDLING;
            _defaultValueHandling = JsonSerializerSettings.DEFAULT_DEFAULT_VALUE_HANDLING;
            _objectCreationHandling = JsonSerializerSettings.DEFAULT_OBJECT_CREATION_HANDLING;
            _preserveReferencesHandling = JsonSerializerSettings.DEFAULT_PRESERVE_REFERENCES_HANDLING;
            _constructorHandling = JsonSerializerSettings.DEFAULT_CONSTRUCTOR_HANDLING;
            _typeNameHandling = JsonSerializerSettings.DEFAULT_TYPE_NAME_HANDLING;
            _metadataPropertyHandling = JsonSerializerSettings.DEFAULT_METADATA_PROPERTY_HANDLING;
            _context = JsonSerializerSettings.DefaultContext;
            _binder = DefaultSerializationBinder.Instance;

            _culture = JsonSerializerSettings.DefaultCulture;
            _contractResolver = DefaultContractResolver.Instance;
        }

        #endregion

        #region Методы

        internal bool IsCheckAdditionalContentSet()
        {
            return (_checkAdditionalContent != null);
        }

        public static JsonSerializer Create()
        {
            return new JsonSerializer();
        }

        public static JsonSerializer Create(JsonSerializerSettings settings)
        {
            JsonSerializer serializer = Create();

            if (settings != null)
            {
                ApplySerializerSettings(serializer, settings);
            }

            return serializer;
        }

        public static JsonSerializer CreateDefault()
        {
            Func<JsonSerializerSettings> defaultSettingsCreator = Json.DefaultSettings;
            JsonSerializerSettings defaultSettings = (defaultSettingsCreator != null) ? defaultSettingsCreator() : null;

            return Create(defaultSettings);
        }

        public static JsonSerializer CreateDefault(JsonSerializerSettings settings)
        {
            JsonSerializer serializer = CreateDefault();
            if (settings != null)
            {
                ApplySerializerSettings(serializer, settings);
            }

            return serializer;
        }

        private static void ApplySerializerSettings(JsonSerializer serializer, JsonSerializerSettings settings)
        {
            if (!CollectionUtils.IsNullOrEmpty(settings.Converters))
            {
                for (int i = 0; i < settings.Converters.Count; i++)
                {
                    serializer.Converters.Insert(i, settings.Converters[i]);
                }
            }

            if (settings._typeNameHandling != null)
            {
                serializer.TypeNameHandling = settings.TypeNameHandling;
            }
            if (settings._metadataPropertyHandling != null)
            {
                serializer.MetadataPropertyHandling = settings.MetadataPropertyHandling;
            }
            if (settings._typeNameAssemblyFormat != null)
            {
                serializer.TypeNameAssemblyFormat = settings.TypeNameAssemblyFormat;
            }
            if (settings._preserveReferencesHandling != null)
            {
                serializer.PreserveReferencesHandling = settings.PreserveReferencesHandling;
            }
            if (settings._referenceLoopHandling != null)
            {
                serializer.ReferenceLoopHandling = settings.ReferenceLoopHandling;
            }
            if (settings._missingMemberHandling != null)
            {
                serializer.MissingMemberHandling = settings.MissingMemberHandling;
            }
            if (settings._objectCreationHandling != null)
            {
                serializer.ObjectCreationHandling = settings.ObjectCreationHandling;
            }
            if (settings._nullValueHandling != null)
            {
                serializer.NullValueHandling = settings.NullValueHandling;
            }
            if (settings._defaultValueHandling != null)
            {
                serializer.DefaultValueHandling = settings.DefaultValueHandling;
            }
            if (settings._constructorHandling != null)
            {
                serializer.ConstructorHandling = settings.ConstructorHandling;
            }
            if (settings._context != null)
            {
                serializer.Context = settings.Context;
            }
            if (settings._checkAdditionalContent != null)
            {
                serializer._checkAdditionalContent = settings._checkAdditionalContent;
            }

            if (settings.Error != null)
            {
                serializer.Error += settings.Error;
            }

            if (settings.ContractResolver != null)
            {
                serializer.ContractResolver = settings.ContractResolver;
            }
            if (settings.ReferenceResolver != null)
            {
                serializer.ReferenceResolver = settings.ReferenceResolver;
            }
            if (settings.TraceWriter != null)
            {
                serializer.TraceWriter = settings.TraceWriter;
            }
            if (settings.Binder != null)
            {
                serializer.Binder = settings.Binder;
            }

            if (settings._formatting != null)
            {
                serializer._formatting = settings._formatting;
            }
            if (settings._dateFormatHandling != null)
            {
                serializer._dateFormatHandling = settings._dateFormatHandling;
            }
            if (settings._dateTimeZoneHandling != null)
            {
                serializer._dateTimeZoneHandling = settings._dateTimeZoneHandling;
            }
            if (settings._dateParseHandling != null)
            {
                serializer._dateParseHandling = settings._dateParseHandling;
            }
            if (settings._dateFormatStringSet)
            {
                serializer._dateFormatString = settings._dateFormatString;
                serializer._dateFormatStringSet = settings._dateFormatStringSet;
            }
            if (settings._floatFormatHandling != null)
            {
                serializer._floatFormatHandling = settings._floatFormatHandling;
            }
            if (settings._floatParseHandling != null)
            {
                serializer._floatParseHandling = settings._floatParseHandling;
            }
            if (settings._stringEscapeHandling != null)
            {
                serializer._stringEscapeHandling = settings._stringEscapeHandling;
            }
            if (settings._culture != null)
            {
                serializer._culture = settings._culture;
            }
            if (settings._maxDepthSet)
            {
                serializer._maxDepth = settings._maxDepth;
                serializer._maxDepthSet = settings._maxDepthSet;
            }
        }

        public void Populate(TextReader reader, object target)
        {
            Populate(new JsonTextReader(reader), target);
        }

        public void Populate(JsonReader reader, object target)
        {
            PopulateInternal(reader, target);
        }

        internal virtual void PopulateInternal(JsonReader reader, object target)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");
            ValidationUtils.ArgumentNotNull(target, "target");

            CultureInfo previousCulture;
            DateTimeZoneHandling? previousDateTimeZoneHandling;
            DateParseHandling? previousDateParseHandling;
            FloatParseHandling? previousFloatParseHandling;
            int? previousMaxDepth;
            string previousDateFormatString;
            SetupReader(reader, out previousCulture, out previousDateTimeZoneHandling, out previousDateParseHandling, out previousFloatParseHandling, out previousMaxDepth, out previousDateFormatString);

            TraceJsonReader traceJsonReader = (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
                ? new TraceJsonReader(reader)
                : null;

            JsonSerializerInternalReader serializerReader = new JsonSerializerInternalReader(this);
            serializerReader.Populate(traceJsonReader ?? reader, target);

            if (traceJsonReader != null)
            {
                TraceWriter.Trace(TraceLevel.Verbose, "Deserialized JSON: " + Environment.NewLine + traceJsonReader.GetJson(), null);
            }

            ResetReader(reader, previousCulture, previousDateTimeZoneHandling, previousDateParseHandling, previousFloatParseHandling, previousMaxDepth, previousDateFormatString);
        }

        public object Deserialize(JsonReader reader)
        {
            return Deserialize(reader, null);
        }

        public object Deserialize(TextReader reader, Type objectType)
        {
            return Deserialize(new JsonTextReader(reader), objectType);
        }

        public T Deserialize<T>(JsonReader reader)
        {
            return (T)Deserialize(reader, typeof(T));
        }

        public object Deserialize(JsonReader reader, Type objectType)
        {
            return DeserializeInternal(reader, objectType);
        }

        internal virtual object DeserializeInternal(JsonReader reader, Type objectType)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");

            CultureInfo previousCulture;
            DateTimeZoneHandling? previousDateTimeZoneHandling;
            DateParseHandling? previousDateParseHandling;
            FloatParseHandling? previousFloatParseHandling;
            int? previousMaxDepth;
            string previousDateFormatString;
            SetupReader(reader, out previousCulture, out previousDateTimeZoneHandling, out previousDateParseHandling, out previousFloatParseHandling, out previousMaxDepth, out previousDateFormatString);

            TraceJsonReader traceJsonReader = (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
                ? new TraceJsonReader(reader)
                : null;

            JsonSerializerInternalReader serializerReader = new JsonSerializerInternalReader(this);
            object value = serializerReader.Deserialize(traceJsonReader ?? reader, objectType, CheckAdditionalContent);

            if (traceJsonReader != null)
            {
                TraceWriter.Trace(TraceLevel.Verbose, "Deserialized JSON: " + Environment.NewLine + traceJsonReader.GetJson(), null);
            }

            ResetReader(reader, previousCulture, previousDateTimeZoneHandling, previousDateParseHandling, previousFloatParseHandling, previousMaxDepth, previousDateFormatString);

            return value;
        }

        private void SetupReader(JsonReader reader,
            out CultureInfo previousCulture,
            out DateTimeZoneHandling? previousDateTimeZoneHandling,
            out DateParseHandling? previousDateParseHandling,
            out FloatParseHandling? previousFloatParseHandling,
            out int? previousMaxDepth,
            out string previousDateFormatString)
        {
            if (_culture != null && !_culture.Equals(reader.Culture))
            {
                previousCulture = reader.Culture;
                reader.Culture = _culture;
            }
            else
            {
                previousCulture = null;
            }

            if (_dateTimeZoneHandling != null && reader.DateTimeZoneHandling != _dateTimeZoneHandling)
            {
                previousDateTimeZoneHandling = reader.DateTimeZoneHandling;
                reader.DateTimeZoneHandling = _dateTimeZoneHandling.Value;
            }
            else
            {
                previousDateTimeZoneHandling = null;
            }

            if (_dateParseHandling != null && reader.DateParseHandling != _dateParseHandling)
            {
                previousDateParseHandling = reader.DateParseHandling;
                reader.DateParseHandling = _dateParseHandling.Value;
            }
            else
            {
                previousDateParseHandling = null;
            }

            if (_floatParseHandling != null && reader.FloatParseHandling != _floatParseHandling)
            {
                previousFloatParseHandling = reader.FloatParseHandling;
                reader.FloatParseHandling = _floatParseHandling.Value;
            }
            else
            {
                previousFloatParseHandling = null;
            }

            if (_maxDepthSet && reader.MaxDepth != _maxDepth)
            {
                previousMaxDepth = reader.MaxDepth;
                reader.MaxDepth = _maxDepth;
            }
            else
            {
                previousMaxDepth = null;
            }

            if (_dateFormatStringSet && reader.DateFormatString != _dateFormatString)
            {
                previousDateFormatString = reader.DateFormatString;
                reader.DateFormatString = _dateFormatString;
            }
            else
            {
                previousDateFormatString = null;
            }

            JsonTextReader textReader = reader as JsonTextReader;
            if (textReader != null)
            {
                DefaultContractResolver resolver = _contractResolver as DefaultContractResolver;
                if (resolver != null)
                {
                    textReader.NameTable = resolver.GetState().NameTable;
                }
            }
        }

        private void ResetReader(JsonReader reader,
            CultureInfo previousCulture,
            DateTimeZoneHandling? previousDateTimeZoneHandling,
            DateParseHandling? previousDateParseHandling,
            FloatParseHandling? previousFloatParseHandling,
            int? previousMaxDepth,
            string previousDateFormatString)
        {
            if (previousCulture != null)
            {
                reader.Culture = previousCulture;
            }
            if (previousDateTimeZoneHandling != null)
            {
                reader.DateTimeZoneHandling = previousDateTimeZoneHandling.Value;
            }
            if (previousDateParseHandling != null)
            {
                reader.DateParseHandling = previousDateParseHandling.Value;
            }
            if (previousFloatParseHandling != null)
            {
                reader.FloatParseHandling = previousFloatParseHandling.Value;
            }
            if (_maxDepthSet)
            {
                reader.MaxDepth = previousMaxDepth;
            }
            if (_dateFormatStringSet)
            {
                reader.DateFormatString = previousDateFormatString;
            }

            JsonTextReader textReader = reader as JsonTextReader;
            if (textReader != null)
            {
                textReader.NameTable = null;
            }
        }

        public void Serialize(TextWriter textWriter, object value)
        {
            Serialize(new JsonTextWriter(textWriter), value);
        }

        public void Serialize(JsonWriter jsonWriter, object value, Type objectType)
        {
            SerializeInternal(jsonWriter, value, objectType);
        }

        public void Serialize(TextWriter textWriter, object value, Type objectType)
        {
            Serialize(new JsonTextWriter(textWriter), value, objectType);
        }

        public void Serialize(JsonWriter jsonWriter, object value)
        {
            SerializeInternal(jsonWriter, value, null);
        }

        internal virtual void SerializeInternal(JsonWriter jsonWriter, object value, Type objectType)
        {
            ValidationUtils.ArgumentNotNull(jsonWriter, "jsonWriter");

            JsonFormatting? previousFormatting = null;
            if (_formatting != null && jsonWriter.Formatting != _formatting)
            {
                previousFormatting = jsonWriter.Formatting;
                jsonWriter.Formatting = _formatting.Value;
            }

            DateFormatHandling? previousDateFormatHandling = null;
            if (_dateFormatHandling != null && jsonWriter.DateFormatHandling != _dateFormatHandling)
            {
                previousDateFormatHandling = jsonWriter.DateFormatHandling;
                jsonWriter.DateFormatHandling = _dateFormatHandling.Value;
            }

            DateTimeZoneHandling? previousDateTimeZoneHandling = null;
            if (_dateTimeZoneHandling != null && jsonWriter.DateTimeZoneHandling != _dateTimeZoneHandling)
            {
                previousDateTimeZoneHandling = jsonWriter.DateTimeZoneHandling;
                jsonWriter.DateTimeZoneHandling = _dateTimeZoneHandling.Value;
            }

            FloatFormatHandling? previousFloatFormatHandling = null;
            if (_floatFormatHandling != null && jsonWriter.FloatFormatHandling != _floatFormatHandling)
            {
                previousFloatFormatHandling = jsonWriter.FloatFormatHandling;
                jsonWriter.FloatFormatHandling = _floatFormatHandling.Value;
            }

            StringEscapeHandling? previousStringEscapeHandling = null;
            if (_stringEscapeHandling != null && jsonWriter.StringEscapeHandling != _stringEscapeHandling)
            {
                previousStringEscapeHandling = jsonWriter.StringEscapeHandling;
                jsonWriter.StringEscapeHandling = _stringEscapeHandling.Value;
            }

            CultureInfo previousCulture = null;
            if (_culture != null && !_culture.Equals(jsonWriter.Culture))
            {
                previousCulture = jsonWriter.Culture;
                jsonWriter.Culture = _culture;
            }

            string previousDateFormatString = null;
            if (_dateFormatStringSet && jsonWriter.DateFormatString != _dateFormatString)
            {
                previousDateFormatString = jsonWriter.DateFormatString;
                jsonWriter.DateFormatString = _dateFormatString;
            }

            TraceJsonWriter traceJsonWriter = (TraceWriter != null && TraceWriter.LevelFilter >= TraceLevel.Verbose)
                ? new TraceJsonWriter(jsonWriter)
                : null;

            JsonSerializerInternalWriter serializerWriter = new JsonSerializerInternalWriter(this);
            serializerWriter.Serialize(traceJsonWriter ?? jsonWriter, value, objectType);

            if (traceJsonWriter != null)
            {
                TraceWriter.Trace(TraceLevel.Verbose, "Serialized JSON: " + Environment.NewLine + traceJsonWriter.GetJson(), null);
            }

            if (previousFormatting != null)
            {
                jsonWriter.Formatting = previousFormatting.Value;
            }
            if (previousDateFormatHandling != null)
            {
                jsonWriter.DateFormatHandling = previousDateFormatHandling.Value;
            }
            if (previousDateTimeZoneHandling != null)
            {
                jsonWriter.DateTimeZoneHandling = previousDateTimeZoneHandling.Value;
            }
            if (previousFloatFormatHandling != null)
            {
                jsonWriter.FloatFormatHandling = previousFloatFormatHandling.Value;
            }
            if (previousStringEscapeHandling != null)
            {
                jsonWriter.StringEscapeHandling = previousStringEscapeHandling.Value;
            }
            if (_dateFormatStringSet)
            {
                jsonWriter.DateFormatString = previousDateFormatString;
            }
            if (previousCulture != null)
            {
                jsonWriter.Culture = previousCulture;
            }
        }

        internal IReferenceResolver GetReferenceResolver()
        {
            return _referenceResolver ?? (_referenceResolver = new DefaultReferenceResolver());
        }

        internal JsonConverter GetMatchingConverter(Type type)
        {
            return GetMatchingConverter(_converters, type);
        }

        internal static JsonConverter GetMatchingConverter(IList<JsonConverter> converters, Type objectType)
        {
#if DEBUG
            ValidationUtils.ArgumentNotNull(objectType, "objectType");
#endif

            if (converters != null)
            {
                return converters.FirstOrDefault(converter => converter.CanConvert(objectType));
            }

            return null;
        }

        internal void OnError(ErrorEventArgs e)
        {
            EventHandler<ErrorEventArgs> error = Error;
            if (error != null)
            {
                error(this, e);
            }
        }

        #endregion
    }
}