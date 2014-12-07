using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace SharpLib.Json
{
    public class JsonSerializerSettings
    {
        #region Константы

        internal const bool DEFAULT_CHECK_ADDITIONAL_CONTENT = false;

        internal const ConstructorHandling DEFAULT_CONSTRUCTOR_HANDLING = ConstructorHandling.Default;

        internal const DateFormatHandling DEFAULT_DATE_FORMAT_HANDLING = DateFormatHandling.IsoDateFormat;

        internal const string DEFAULT_DATE_FORMAT_STRING = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        internal const DateParseHandling DEFAULT_DATE_PARSE_HANDLING = DateParseHandling.DateTime;

        internal const DateTimeZoneHandling DEFAULT_DATE_TIME_ZONE_HANDLING = DateTimeZoneHandling.RoundtripKind;

        internal const DefaultValueHandling DEFAULT_DEFAULT_VALUE_HANDLING = DefaultValueHandling.Include;

        internal const FloatFormatHandling DEFAULT_FLOAT_FORMAT_HANDLING = FloatFormatHandling.String;

        internal const FloatParseHandling DEFAULT_FLOAT_PARSE_HANDLING = FloatParseHandling.Double;

        internal const FormatterAssemblyStyle DEFAULT_FORMATTER_ASSEMBLY_STYLE = FormatterAssemblyStyle.Simple;

        internal const JsonFormatting DEFAULT_FORMATTING = JsonFormatting.None;

        internal const MetadataPropertyHandling DEFAULT_METADATA_PROPERTY_HANDLING = MetadataPropertyHandling.Default;

        internal const MissingMemberHandling DEFAULT_MISSING_MEMBER_HANDLING = MissingMemberHandling.Ignore;

        internal const NullValueHandling DEFAULT_NULL_VALUE_HANDLING = NullValueHandling.Include;

        internal const ObjectCreationHandling DEFAULT_OBJECT_CREATION_HANDLING = ObjectCreationHandling.Auto;

        internal const PreserveReferencesHandling DEFAULT_PRESERVE_REFERENCES_HANDLING = PreserveReferencesHandling.None;

        internal const ReferenceLoopHandling DEFAULT_REFERENCE_LOOP_HANDLING = ReferenceLoopHandling.Error;

        internal const StringEscapeHandling DEFAULT_STRING_ESCAPE_HANDLING = StringEscapeHandling.Default;

        internal const FormatterAssemblyStyle DEFAULT_TYPE_NAME_ASSEMBLY_FORMAT = FormatterAssemblyStyle.Simple;

        internal const TypeNameHandling DEFAULT_TYPE_NAME_HANDLING = TypeNameHandling.None;

        #endregion

        #region Поля

        internal static readonly StreamingContext DefaultContext;

        internal static readonly CultureInfo DefaultCulture;

        internal bool? _checkAdditionalContent;

        internal ConstructorHandling? _constructorHandling;

        internal StreamingContext? _context;

        internal CultureInfo _culture;

        internal DateFormatHandling? _dateFormatHandling;

        internal string _dateFormatString;

        internal bool _dateFormatStringSet;

        internal DateParseHandling? _dateParseHandling;

        internal DateTimeZoneHandling? _dateTimeZoneHandling;

        internal DefaultValueHandling? _defaultValueHandling;

        internal FloatFormatHandling? _floatFormatHandling;

        internal FloatParseHandling? _floatParseHandling;

        internal JsonFormatting? _formatting;

        internal int? _maxDepth;

        internal bool _maxDepthSet;

        internal MetadataPropertyHandling? _metadataPropertyHandling;

        internal MissingMemberHandling? _missingMemberHandling;

        internal NullValueHandling? _nullValueHandling;

        internal ObjectCreationHandling? _objectCreationHandling;

        internal PreserveReferencesHandling? _preserveReferencesHandling;

        internal ReferenceLoopHandling? _referenceLoopHandling;

        internal StringEscapeHandling? _stringEscapeHandling;

        internal FormatterAssemblyStyle? _typeNameAssemblyFormat;

        internal TypeNameHandling? _typeNameHandling;

        #endregion

        #region Свойства

        public ReferenceLoopHandling ReferenceLoopHandling
        {
            get { return _referenceLoopHandling ?? DEFAULT_REFERENCE_LOOP_HANDLING; }
            set { _referenceLoopHandling = value; }
        }

        public MissingMemberHandling MissingMemberHandling
        {
            get { return _missingMemberHandling ?? DEFAULT_MISSING_MEMBER_HANDLING; }
            set { _missingMemberHandling = value; }
        }

        public ObjectCreationHandling ObjectCreationHandling
        {
            get { return _objectCreationHandling ?? DEFAULT_OBJECT_CREATION_HANDLING; }
            set { _objectCreationHandling = value; }
        }

        public NullValueHandling NullValueHandling
        {
            get { return _nullValueHandling ?? DEFAULT_NULL_VALUE_HANDLING; }
            set { _nullValueHandling = value; }
        }

        public DefaultValueHandling DefaultValueHandling
        {
            get { return _defaultValueHandling ?? DEFAULT_DEFAULT_VALUE_HANDLING; }
            set { _defaultValueHandling = value; }
        }

        public IList<JsonConverter> Converters { get; set; }

        public PreserveReferencesHandling PreserveReferencesHandling
        {
            get { return _preserveReferencesHandling ?? DEFAULT_PRESERVE_REFERENCES_HANDLING; }
            set { _preserveReferencesHandling = value; }
        }

        public TypeNameHandling TypeNameHandling
        {
            get { return _typeNameHandling ?? DEFAULT_TYPE_NAME_HANDLING; }
            set { _typeNameHandling = value; }
        }

        public MetadataPropertyHandling MetadataPropertyHandling
        {
            get { return _metadataPropertyHandling ?? DEFAULT_METADATA_PROPERTY_HANDLING; }
            set { _metadataPropertyHandling = value; }
        }

        public FormatterAssemblyStyle TypeNameAssemblyFormat
        {
            get { return _typeNameAssemblyFormat ?? DEFAULT_FORMATTER_ASSEMBLY_STYLE; }
            set { _typeNameAssemblyFormat = value; }
        }

        public ConstructorHandling ConstructorHandling
        {
            get { return _constructorHandling ?? DEFAULT_CONSTRUCTOR_HANDLING; }
            set { _constructorHandling = value; }
        }

        public IContractResolver ContractResolver { get; set; }

        public IReferenceResolver ReferenceResolver { get; set; }

        public ITraceWriter TraceWriter { get; set; }

        public SerializationBinder Binder { get; set; }

        public EventHandler<ErrorEventArgs> Error { get; set; }

        public StreamingContext Context
        {
            get { return _context ?? DefaultContext; }
            set { _context = value; }
        }

        public string DateFormatString
        {
            get { return _dateFormatString ?? DEFAULT_DATE_FORMAT_STRING; }
            set
            {
                _dateFormatString = value;
                _dateFormatStringSet = true;
            }
        }

        public int? MaxDepth
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

        public JsonFormatting Formatting
        {
            get { return _formatting ?? DEFAULT_FORMATTING; }
            set { _formatting = value; }
        }

        public DateFormatHandling DateFormatHandling
        {
            get { return _dateFormatHandling ?? DEFAULT_DATE_FORMAT_HANDLING; }
            set { _dateFormatHandling = value; }
        }

        public DateTimeZoneHandling DateTimeZoneHandling
        {
            get { return _dateTimeZoneHandling ?? DEFAULT_DATE_TIME_ZONE_HANDLING; }
            set { _dateTimeZoneHandling = value; }
        }

        public DateParseHandling DateParseHandling
        {
            get { return _dateParseHandling ?? DEFAULT_DATE_PARSE_HANDLING; }
            set { _dateParseHandling = value; }
        }

        public FloatFormatHandling FloatFormatHandling
        {
            get { return _floatFormatHandling ?? DEFAULT_FLOAT_FORMAT_HANDLING; }
            set { _floatFormatHandling = value; }
        }

        public FloatParseHandling FloatParseHandling
        {
            get { return _floatParseHandling ?? DEFAULT_FLOAT_PARSE_HANDLING; }
            set { _floatParseHandling = value; }
        }

        public StringEscapeHandling StringEscapeHandling
        {
            get { return _stringEscapeHandling ?? DEFAULT_STRING_ESCAPE_HANDLING; }
            set { _stringEscapeHandling = value; }
        }

        public CultureInfo Culture
        {
            get { return _culture ?? DefaultCulture; }
            set { _culture = value; }
        }

        public bool CheckAdditionalContent
        {
            get { return _checkAdditionalContent ?? DEFAULT_CHECK_ADDITIONAL_CONTENT; }
            set { _checkAdditionalContent = value; }
        }

        #endregion

        #region Конструктор

        static JsonSerializerSettings()
        {
            DefaultContext = new StreamingContext();
            DefaultCulture = CultureInfo.InvariantCulture;
        }

        public JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>();
        }

        #endregion
    }
}