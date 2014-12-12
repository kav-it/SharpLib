using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SharpLib.Json
{
    public static class Json
    {
        #region Поля

        public static readonly string False = "false";

        public static readonly string NaN = "NaN";

        public static readonly string NegativeInfinity = "-Infinity";

        public static readonly string Null = "null";

        public static readonly string PositiveInfinity = "Infinity";

        public static readonly string True = "true";

        public static readonly string Undefined = "undefined";

        #endregion

        #region Свойства

        public static Func<JsonSerializerSettings> DefaultSettings { get; set; }

        #endregion

        #region Методы

        public static string ToString(DateTime value)
        {
            return ToString(value, DateFormatHandling.IsoDateFormat, DateTimeZoneHandling.RoundtripKind);
        }

        public static string ToString(DateTime value, DateFormatHandling format, DateTimeZoneHandling timeZoneHandling)
        {
            DateTime updatedDateTime = DateTimeUtils.EnsureDateTime(value, timeZoneHandling);

            using (StringWriter writer = StringUtils.CreateStringWriter(64))
            {
                writer.Write('"');
                DateTimeUtils.WriteDateTimeString(writer, updatedDateTime, format, null, CultureInfo.InvariantCulture);
                writer.Write('"');
                return writer.ToString();
            }
        }

        public static string ToString(DateTimeOffset value)
        {
            return ToString(value, DateFormatHandling.IsoDateFormat);
        }

        public static string ToString(DateTimeOffset value, DateFormatHandling format)
        {
            using (StringWriter writer = StringUtils.CreateStringWriter(64))
            {
                writer.Write('"');
                DateTimeUtils.WriteDateTimeOffsetString(writer, value, format, null, CultureInfo.InvariantCulture);
                writer.Write('"');
                return writer.ToString();
            }
        }

        public static string ToString(bool value)
        {
            return (value) ? True : False;
        }

        public static string ToString(char value)
        {
            return ToString(char.ToString(value));
        }

        public static string ToString(Enum value)
        {
            return value.ToString("D");
        }

        public static string ToString(int value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        public static string ToString(short value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static string ToString(ushort value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static string ToString(uint value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        public static string ToString(long value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        private static string ToStringInternal(BigInteger value)
        {
            return value.ToString(string.Empty, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static string ToString(ulong value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        public static string ToString(float value)
        {
            return EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture));
        }

        internal static string ToString(float value, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
        {
            return EnsureFloatFormat(value, EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture)), floatFormatHandling, quoteChar, nullable);
        }

        private static string EnsureFloatFormat(double value, string text, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
        {
            if (floatFormatHandling == FloatFormatHandling.Symbol || !(double.IsInfinity(value) || double.IsNaN(value)))
            {
                return text;
            }

            if (floatFormatHandling == FloatFormatHandling.DefaultValue)
            {
                return (!nullable) ? "0.0" : Null;
            }

            return quoteChar + text + quoteChar;
        }

        public static string ToString(double value)
        {
            return EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture));
        }

        internal static string ToString(double value, FloatFormatHandling floatFormatHandling, char quoteChar, bool nullable)
        {
            return EnsureFloatFormat(value, EnsureDecimalPlace(value, value.ToString("R", CultureInfo.InvariantCulture)), floatFormatHandling, quoteChar, nullable);
        }

        private static string EnsureDecimalPlace(double value, string text)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || text.IndexOf('.') != -1 || text.IndexOf('E') != -1 || text.IndexOf('e') != -1)
            {
                return text;
            }

            return text + ".0";
        }

        private static string EnsureDecimalPlace(string text)
        {
            if (text.IndexOf('.') != -1)
            {
                return text;
            }

            return text + ".0";
        }

        public static string ToString(byte value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static string ToString(sbyte value)
        {
            return value.ToString(null, CultureInfo.InvariantCulture);
        }

        public static string ToString(decimal value)
        {
            return EnsureDecimalPlace(value.ToString(null, CultureInfo.InvariantCulture));
        }

        public static string ToString(Guid value)
        {
            return ToString(value, '"');
        }

        internal static string ToString(Guid value, char quoteChar)
        {
            var text = value.ToString("D", CultureInfo.InvariantCulture);
            var qc = quoteChar.ToString(CultureInfo.InvariantCulture);

            return qc + text + qc;
        }

        public static string ToString(TimeSpan value)
        {
            return ToString(value, '"');
        }

        internal static string ToString(TimeSpan value, char quoteChar)
        {
            return ToString(value.ToString(), quoteChar);
        }

        public static string ToString(Uri value)
        {
            if (value == null)
            {
                return Null;
            }

            return ToString(value, '"');
        }

        internal static string ToString(Uri value, char quoteChar)
        {
            return ToString(value.OriginalString, quoteChar);
        }

        public static string ToString(string value)
        {
            return ToString(value, '"');
        }

        public static string ToString(string value, char delimiter)
        {
            return ToString(value, delimiter, StringEscapeHandling.Default);
        }

        public static string ToString(string value, char delimiter, StringEscapeHandling stringEscapeHandling)
        {
            if (delimiter != '"' && delimiter != '\'')
            {
                throw new ArgumentException("Delimiter must be a single or double quote.", "delimiter");
            }

            return JavaScriptUtils.ToEscapedJavaScriptString(value, delimiter, true, stringEscapeHandling);
        }

        public static string ToString(object value)
        {
            if (value == null)
            {
                return Null;
            }

            PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(value.GetType());

            switch (typeCode)
            {
                case PrimitiveTypeCode.String:
                    return ToString((string)value);
                case PrimitiveTypeCode.Char:
                    return ToString((char)value);
                case PrimitiveTypeCode.Boolean:
                    return ToString((bool)value);
                case PrimitiveTypeCode.SByte:
                    return ToString((sbyte)value);
                case PrimitiveTypeCode.Int16:
                    return ToString((short)value);
                case PrimitiveTypeCode.UInt16:
                    return ToString((ushort)value);
                case PrimitiveTypeCode.Int32:
                    return ToString((int)value);
                case PrimitiveTypeCode.Byte:
                    return ToString((byte)value);
                case PrimitiveTypeCode.UInt32:
                    return ToString((uint)value);
                case PrimitiveTypeCode.Int64:
                    return ToString((long)value);
                case PrimitiveTypeCode.UInt64:
                    return ToString((ulong)value);
                case PrimitiveTypeCode.Single:
                    return ToString((float)value);
                case PrimitiveTypeCode.Double:
                    return ToString((double)value);
                case PrimitiveTypeCode.DateTime:
                    return ToString((DateTime)value);
                case PrimitiveTypeCode.Decimal:
                    return ToString((decimal)value);
                case PrimitiveTypeCode.DBNull:
                    return Null;
                case PrimitiveTypeCode.DateTimeOffset:
                    return ToString((DateTimeOffset)value);
                case PrimitiveTypeCode.Guid:
                    return ToString((Guid)value);
                case PrimitiveTypeCode.Uri:
                    return ToString((Uri)value);
                case PrimitiveTypeCode.TimeSpan:
                    return ToString((TimeSpan)value);
                case PrimitiveTypeCode.BigInteger:
                    return ToStringInternal((BigInteger)value);
            }

            throw new ArgumentException("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
        }

        public static string SerializeObject(object value)
        {
            return SerializeObject(value, null, (JsonSerializerSettings)null);
        }

        public static string SerializeObject(object value, JsonFormatting formatting)
        {
            return SerializeObject(value, formatting, (JsonSerializerSettings)null);
        }

        public static string SerializeObject(object value, params JsonConverter[] converters)
        {
            JsonSerializerSettings settings = (converters != null && converters.Length > 0)
                ? new JsonSerializerSettings
                {
                    Converters = converters
                }
                : null;

            return SerializeObject(value, null, settings);
        }

        public static string SerializeObject(object value, JsonFormatting formatting, params JsonConverter[] converters)
        {
            JsonSerializerSettings settings = (converters != null && converters.Length > 0)
                ? new JsonSerializerSettings
                {
                    Converters = converters
                }
                : null;

            return SerializeObject(value, null, formatting, settings);
        }

        public static string SerializeObject(object value, JsonSerializerSettings settings)
        {
            return SerializeObject(value, null, settings);
        }

        public static string SerializeObject(object value, Type type, JsonSerializerSettings settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);

            return SerializeObjectInternal(value, type, jsonSerializer);
        }

        public static string SerializeObject(object value, JsonFormatting formatting, JsonSerializerSettings settings)
        {
            return SerializeObject(value, null, formatting, settings);
        }

        public static string SerializeObject(object value, Type type, JsonFormatting formatting, JsonSerializerSettings settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);
            jsonSerializer.Formatting = formatting;

            return SerializeObjectInternal(value, type, jsonSerializer);
        }

        private static string SerializeObjectInternal(object value, Type type, JsonSerializer jsonSerializer)
        {
            StringBuilder sb = new StringBuilder(256);
            StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = jsonSerializer.Formatting;

                jsonSerializer.Serialize(jsonWriter, value, type);
            }

            return sw.ToString();
        }

        public static object DeserializeObject(string value)
        {
            return DeserializeObject(value, null, (JsonSerializerSettings)null);
        }

        public static object DeserializeObject(string value, JsonSerializerSettings settings)
        {
            return DeserializeObject(value, null, settings);
        }

        public static object DeserializeObject(string value, Type type)
        {
            return DeserializeObject(value, type, (JsonSerializerSettings)null);
        }

        public static T DeserializeObject<T>(string value)
        {
            return DeserializeObject<T>(value, (JsonSerializerSettings)null);
        }

        public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject)
        {
            return DeserializeObject<T>(value);
        }

        public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject, JsonSerializerSettings settings)
        {
            return DeserializeObject<T>(value, settings);
        }

        public static T DeserializeObject<T>(string value, params JsonConverter[] converters)
        {
            return (T)DeserializeObject(value, typeof(T), converters);
        }

        public static T DeserializeObject<T>(string value, JsonSerializerSettings settings)
        {
            return (T)DeserializeObject(value, typeof(T), settings);
        }

        public static object DeserializeObject(string value, Type type, params JsonConverter[] converters)
        {
            JsonSerializerSettings settings = (converters != null && converters.Length > 0)
                ? new JsonSerializerSettings
                {
                    Converters = converters
                }
                : null;

            return DeserializeObject(value, type, settings);
        }

        public static object DeserializeObject(string value, Type type, JsonSerializerSettings settings)
        {
            ValidationUtils.ArgumentNotNull(value, "value");

            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);

            if (!jsonSerializer.IsCheckAdditionalContentSet())
            {
                jsonSerializer.CheckAdditionalContent = true;
            }

            using (var reader = new JsonTextReader(new StringReader(value)))
            {
                return jsonSerializer.Deserialize(reader, type);
            }
        }

        public static void PopulateObject(string value, object target)
        {
            PopulateObject(value, target, null);
        }

        public static void PopulateObject(string value, object target, JsonSerializerSettings settings)
        {
            JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(settings);

            using (JsonReader jsonReader = new JsonTextReader(new StringReader(value)))
            {
                jsonSerializer.Populate(jsonReader, target);

                if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
                {
                    throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
                }
            }
        }

        public static string SerializeXmlNode(XmlNode node)
        {
            return SerializeXmlNode(node, JsonFormatting.None);
        }

        public static string SerializeXmlNode(XmlNode node, JsonFormatting formatting)
        {
            XmlNodeConverter converter = new XmlNodeConverter();

            return SerializeObject(node, formatting, converter);
        }

        public static string SerializeXmlNode(XmlNode node, JsonFormatting formatting, bool omitRootObject)
        {
            XmlNodeConverter converter = new XmlNodeConverter
            {
                OmitRootObject = omitRootObject
            };

            return SerializeObject(node, formatting, converter);
        }

        public static XmlDocument DeserializeXmlNode(string value)
        {
            return DeserializeXmlNode(value, null);
        }

        public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName)
        {
            return DeserializeXmlNode(value, deserializeRootElementName, false);
        }

        public static XmlDocument DeserializeXmlNode(string value, string deserializeRootElementName, bool writeArrayAttribute)
        {
            XmlNodeConverter converter = new XmlNodeConverter();
            converter.DeserializeRootElementName = deserializeRootElementName;
            converter.WriteArrayAttribute = writeArrayAttribute;

            return (XmlDocument)DeserializeObject(value, typeof(XmlDocument), converter);
        }

        public static string SerializeXNode(XObject node)
        {
            return SerializeXNode(node, JsonFormatting.None);
        }

        public static string SerializeXNode(XObject node, JsonFormatting formatting)
        {
            return SerializeXNode(node, formatting, false);
        }

        public static string SerializeXNode(XObject node, JsonFormatting formatting, bool omitRootObject)
        {
            XmlNodeConverter converter = new XmlNodeConverter
            {
                OmitRootObject = omitRootObject
            };

            return SerializeObject(node, formatting, converter);
        }

        public static XDocument DeserializeXNode(string value)
        {
            return DeserializeXNode(value, null);
        }

        public static XDocument DeserializeXNode(string value, string deserializeRootElementName)
        {
            return DeserializeXNode(value, deserializeRootElementName, false);
        }

        public static XDocument DeserializeXNode(string value, string deserializeRootElementName, bool writeArrayAttribute)
        {
            XmlNodeConverter converter = new XmlNodeConverter();
            converter.DeserializeRootElementName = deserializeRootElementName;
            converter.WriteArrayAttribute = writeArrayAttribute;

            return (XDocument)DeserializeObject(value, typeof(XDocument), converter);
        }

        /// <summary>
        /// Сохранение объекта в файл .json
        /// </summary>
        public static void SaveToFile(string path, object value)
        {
            using (StreamWriter streamWriter = File.CreateText(path))
            using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = JsonFormatting.Indented;
                
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, value);
            }
        }

        /// <summary>
        /// Загрузка объекта из файла .json
        /// </summary>
        public static T LoadFromFile<T>(string path)
        {
            var text = File.ReadAllText(path);
            T result = DeserializeObject<T>(text);

            return result;
        }

        /// <summary>
        /// Загрузка объекта из файла .json
        /// </summary>
        public static object LoadFromFile(string path, Type valueType)
        {
            var text = File.ReadAllText(path);
            var result = DeserializeObject(text, valueType);

            return result;
        }

        #endregion
    }
}