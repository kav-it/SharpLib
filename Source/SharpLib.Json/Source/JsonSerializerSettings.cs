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

        internal const bool DefaultCheckAdditionalContent = false;

        internal const ConstructorHandling DefaultConstructorHandling = ConstructorHandling.Default;

        internal const DateFormatHandling DefaultDateFormatHandling = DateFormatHandling.IsoDateFormat;

        internal const string DefaultDateFormatString = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        internal const DateParseHandling DefaultDateParseHandling = DateParseHandling.DateTime;

        internal const DateTimeZoneHandling DefaultDateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;

        internal const DefaultValueHandling DefaultDefaultValueHandling = DefaultValueHandling.Include;

        internal const FloatFormatHandling DefaultFloatFormatHandling = FloatFormatHandling.String;

        internal const FloatParseHandling DefaultFloatParseHandling = FloatParseHandling.Double;

        internal const FormatterAssemblyStyle DefaultFormatterAssemblyStyle = FormatterAssemblyStyle.Simple;

        internal const Formatting DefaultFormatting = Formatting.None;

        internal const MetadataPropertyHandling DefaultMetadataPropertyHandling = MetadataPropertyHandling.Default;

        internal const MissingMemberHandling DefaultMissingMemberHandling = MissingMemberHandling.Ignore;

        internal const NullValueHandling DefaultNullValueHandling = NullValueHandling.Include;

        internal const ObjectCreationHandling DefaultObjectCreationHandling = ObjectCreationHandling.Auto;

        internal const PreserveReferencesHandling DefaultPreserveReferencesHandling = PreserveReferencesHandling.None;

        internal const ReferenceLoopHandling DefaultReferenceLoopHandling = ReferenceLoopHandling.Error;

        internal const StringEscapeHandling DefaultStringEscapeHandling = StringEscapeHandling.Default;

        internal const FormatterAssemblyStyle DefaultTypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;

        internal const TypeNameHandling DefaultTypeNameHandling = TypeNameHandling.None;

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

        internal Formatting? _formatting;

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
            get { return _referenceLoopHandling ?? DefaultReferenceLoopHandling; }
            set { _referenceLoopHandling = value; }
        }

        public MissingMemberHandling MissingMemberHandling
        {
            get { return _missingMemberHandling ?? DefaultMissingMemberHandling; }
            set { _missingMemberHandling = value; }
        }

        public ObjectCreationHandling ObjectCreationHandling
        {
            get { return _objectCreationHandling ?? DefaultObjectCreationHandling; }
            set { _objectCreationHandling = value; }
        }

        public NullValueHandling NullValueHandling
        {
            get { return _nullValueHandling ?? DefaultNullValueHandling; }
            set { _nullValueHandling = value; }
        }

        public DefaultValueHandling DefaultValueHandling
        {
            get { return _defaultValueHandling ?? DefaultDefaultValueHandling; }
            set { _defaultValueHandling = value; }
        }

        public IList<JsonConverter> Converters { get; set; }

        public PreserveReferencesHandling PreserveReferencesHandling
        {
            get { return _preserveReferencesHandling ?? DefaultPreserveReferencesHandling; }
            set { _preserveReferencesHandling = value; }
        }

        public TypeNameHandling TypeNameHandling
        {
            get { return _typeNameHandling ?? DefaultTypeNameHandling; }
            set { _typeNameHandling = value; }
        }

        public MetadataPropertyHandling MetadataPropertyHandling
        {
            get { return _metadataPropertyHandling ?? DefaultMetadataPropertyHandling; }
            set { _metadataPropertyHandling = value; }
        }

        public FormatterAssemblyStyle TypeNameAssemblyFormat
        {
            get { return _typeNameAssemblyFormat ?? DefaultFormatterAssemblyStyle; }
            set { _typeNameAssemblyFormat = value; }
        }

        public ConstructorHandling ConstructorHandling
        {
            get { return _constructorHandling ?? DefaultConstructorHandling; }
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
            get { return _dateFormatString ?? DefaultDateFormatString; }
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

        public Formatting Formatting
        {
            get { return _formatting ?? DefaultFormatting; }
            set { _formatting = value; }
        }

        public DateFormatHandling DateFormatHandling
        {
            get { return _dateFormatHandling ?? DefaultDateFormatHandling; }
            set { _dateFormatHandling = value; }
        }

        public DateTimeZoneHandling DateTimeZoneHandling
        {
            get { return _dateTimeZoneHandling ?? DefaultDateTimeZoneHandling; }
            set { _dateTimeZoneHandling = value; }
        }

        public DateParseHandling DateParseHandling
        {
            get { return _dateParseHandling ?? DefaultDateParseHandling; }
            set { _dateParseHandling = value; }
        }

        public FloatFormatHandling FloatFormatHandling
        {
            get { return _floatFormatHandling ?? DefaultFloatFormatHandling; }
            set { _floatFormatHandling = value; }
        }

        public FloatParseHandling FloatParseHandling
        {
            get { return _floatParseHandling ?? DefaultFloatParseHandling; }
            set { _floatParseHandling = value; }
        }

        public StringEscapeHandling StringEscapeHandling
        {
            get { return _stringEscapeHandling ?? DefaultStringEscapeHandling; }
            set { _stringEscapeHandling = value; }
        }

        public CultureInfo Culture
        {
            get { return _culture ?? DefaultCulture; }
            set { _culture = value; }
        }

        public bool CheckAdditionalContent
        {
            get { return _checkAdditionalContent ?? DefaultCheckAdditionalContent; }
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