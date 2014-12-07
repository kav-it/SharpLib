using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;

namespace SharpLib.Json.Linq
{
    public abstract class JToken : IJEnumerable<JToken>, IJsonLineInfo, ICloneable, IDynamicMetaObjectProvider
    {
        #region Поля

        private static readonly JTokenType[] BigIntegerTypes = { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean, JTokenType.Bytes };

        private static readonly JTokenType[] BooleanTypes = { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean };

        private static readonly JTokenType[] BytesTypes = { JTokenType.Bytes, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Integer };

        private static readonly JTokenType[] CharTypes = { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw };

        private static readonly JTokenType[] DateTimeTypes = { JTokenType.Date, JTokenType.String, JTokenType.Comment, JTokenType.Raw };

        private static readonly JTokenType[] GuidTypes = { JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Guid, JTokenType.Bytes };

        private static readonly JTokenType[] NumberTypes = { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean };

        private static readonly JTokenType[] StringTypes =
        {
            JTokenType.Date, JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean,
            JTokenType.Bytes, JTokenType.Guid, JTokenType.TimeSpan, JTokenType.Uri
        };

        private static readonly JTokenType[] TimeSpanTypes = { JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.TimeSpan };

        private static readonly JTokenType[] UriTypes = { JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Uri };

        private static JTokenEqualityComparer _equalityComparer;

        private int? _lineNumber;

        private int? _linePosition;

        private JContainer _parent;

        #endregion

        #region Свойства

        public static JTokenEqualityComparer EqualityComparer
        {
            get { return _equalityComparer ?? (_equalityComparer = new JTokenEqualityComparer()); }
        }

        public JContainer Parent
        {
            [DebuggerStepThrough]
            get { return _parent; }
            internal set { _parent = value; }
        }

        public JToken Root
        {
            get
            {
                JContainer parent = Parent;
                if (parent == null)
                {
                    return this;
                }

                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                return parent;
            }
        }

        public abstract JTokenType Type { get; }

        public abstract bool HasValues { get; }

        public JToken Next { get; internal set; }

        public JToken Previous { get; internal set; }

        public string Path
        {
            get
            {
                if (Parent == null)
                {
                    return string.Empty;
                }

                IList<JToken> ancestors = Ancestors().Reverse().ToList();
                ancestors.Add(this);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < ancestors.Count; i++)
                {
                    JToken current = ancestors[i];
                    JToken next = null;
                    if (i + 1 < ancestors.Count)
                    {
                        next = ancestors[i + 1];
                    }
                    else if (ancestors[i].Type == JTokenType.Property)
                    {
                        next = ancestors[i];
                    }

                    if (next != null)
                    {
                        switch (current.Type)
                        {
                            case JTokenType.Property:
                                JProperty property = (JProperty)current;

                                if (sb.Length > 0)
                                {
                                    sb.Append('.');
                                }

                                sb.Append(property.Name);
                                break;
                            case JTokenType.Array:
                            case JTokenType.Constructor:
                                int index = ((IList<JToken>)current).IndexOf(next);

                                sb.Append('[');
                                sb.Append(index);
                                sb.Append(']');
                                break;
                        }
                    }
                }

                return sb.ToString();
            }
        }

        public virtual JToken this[object key]
        {
            get { throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, GetType())); }
            set { throw new InvalidOperationException("Cannot set child value on {0}.".FormatWith(CultureInfo.InvariantCulture, GetType())); }
        }

        public virtual JToken First
        {
            get { throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, GetType())); }
        }

        public virtual JToken Last
        {
            get { throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, GetType())); }
        }

        IJEnumerable<JToken> IJEnumerable<JToken>.this[object key]
        {
            get { return this[key]; }
        }

        int IJsonLineInfo.LineNumber
        {
            get { return _lineNumber ?? 0; }
        }

        int IJsonLineInfo.LinePosition
        {
            get { return _linePosition ?? 0; }
        }

        #endregion

        #region Конструктор

        internal JToken()
        {
        }

        #endregion

        #region Методы

        internal abstract JToken CloneToken();

        internal abstract bool DeepEquals(JToken node);

        public static bool DeepEquals(JToken t1, JToken t2)
        {
            return (t1 == t2 || (t1 != null && t2 != null && t1.DeepEquals(t2)));
        }

        public void AddAfterSelf(object content)
        {
            if (_parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }

            int index = _parent.IndexOfItem(this);
            _parent.AddInternal(index + 1, content, false);
        }

        public void AddBeforeSelf(object content)
        {
            if (_parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }

            int index = _parent.IndexOfItem(this);
            _parent.AddInternal(index, content, false);
        }

        public IEnumerable<JToken> Ancestors()
        {
            for (JToken parent = Parent; parent != null; parent = parent.Parent)
            {
                yield return parent;
            }
        }

        public IEnumerable<JToken> AfterSelf()
        {
            if (Parent == null)
            {
                yield break;
            }

            for (JToken o = Next; o != null; o = o.Next)
            {
                yield return o;
            }
        }

        public IEnumerable<JToken> BeforeSelf()
        {
            for (JToken o = Parent.First; o != this; o = o.Next)
            {
                yield return o;
            }
        }

        public virtual T Value<T>(object key)
        {
            JToken token = this[key];

            return token == null ? default(T) : token.Convert<JToken, T>();
        }

        public virtual JEnumerable<JToken> Children()
        {
            return JEnumerable<JToken>.Empty;
        }

        public JEnumerable<T> Children<T>() where T : JToken
        {
            return new JEnumerable<T>(Children().OfType<T>());
        }

        public virtual IEnumerable<T> Values<T>()
        {
            throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, GetType()));
        }

        public void Remove()
        {
            if (_parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }

            _parent.RemoveItem(this);
        }

        public void Replace(JToken value)
        {
            if (_parent == null)
            {
                throw new InvalidOperationException("The parent is missing.");
            }

            _parent.ReplaceItem(this, value);
        }

        public abstract void WriteTo(JsonWriter writer, params JsonConverter[] converters);

        public override string ToString()
        {
            return ToString(JsonFormatting.Indented);
        }

        public string ToString(JsonFormatting formatting, params JsonConverter[] converters)
        {
            using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonTextWriter jw = new JsonTextWriter(sw);
                jw.Formatting = formatting;

                WriteTo(jw, converters);

                return sw.ToString();
            }
        }

        private static JValue EnsureValue(JToken value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value is JProperty)
            {
                value = ((JProperty)value).Value;
            }

            JValue v = value as JValue;

            return v;
        }

        private static string GetType(JToken token)
        {
            ValidationUtils.ArgumentNotNull(token, "token");

            if (token is JProperty)
            {
                token = ((JProperty)token).Value;
            }

            return token.Type.ToString();
        }

        private static bool ValidateToken(JToken o, JTokenType[] validTypes, bool nullable)
        {
            return (Array.IndexOf(validTypes, o.Type) != -1) || (nullable && (o.Type == JTokenType.Null || o.Type == JTokenType.Undefined));
        }

        private static BigInteger ToBigInteger(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BigIntegerTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            return ConvertUtils.ToBigInteger(v.Value);
        }

        private static BigInteger? ToBigIntegerNullable(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BigIntegerTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to BigInteger.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value == null)
            {
                return null;
            }

            return ConvertUtils.ToBigInteger(v.Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<JToken>)this).GetEnumerator();
        }

        IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator()
        {
            return Children().GetEnumerator();
        }

        internal abstract int GetDeepHashCode();

        public JsonReader CreateReader()
        {
            return new JTokenReader(this, Path);
        }

        internal static JToken FromObjectInternal(object o, JsonSerializer jsonSerializer)
        {
            ValidationUtils.ArgumentNotNull(o, "o");
            ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");

            JToken token;
            using (JTokenWriter jsonWriter = new JTokenWriter())
            {
                jsonSerializer.Serialize(jsonWriter, o);
                token = jsonWriter.Token;
            }

            return token;
        }

        public static JToken FromObject(object o)
        {
            return FromObjectInternal(o, JsonSerializer.CreateDefault());
        }

        public static JToken FromObject(object o, JsonSerializer jsonSerializer)
        {
            return FromObjectInternal(o, jsonSerializer);
        }

        public T ToObject<T>()
        {
            return (T)ToObject(typeof(T));
        }

        public object ToObject(Type objectType)
        {
            if (Json.DefaultSettings == null)
            {
                bool isEnum;
                PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(objectType, out isEnum);

                if (isEnum && Type == JTokenType.String)
                {
                    Type enumType = objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType);
                    try
                    {
                        return Enum.Parse(enumType, (string)this, true);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Could not convert '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, (string)this, enumType.Name), ex);
                    }
                }

                switch (typeCode)
                {
                    case PrimitiveTypeCode.BooleanNullable:
                        return (bool?)this;
                    case PrimitiveTypeCode.Boolean:
                        return (bool)this;
                    case PrimitiveTypeCode.CharNullable:
                        return (char?)this;
                    case PrimitiveTypeCode.Char:
                        return (char)this;
                    case PrimitiveTypeCode.SByte:
                        return (sbyte?)this;
                    case PrimitiveTypeCode.SByteNullable:
                        return (sbyte)this;
                    case PrimitiveTypeCode.ByteNullable:
                        return (byte?)this;
                    case PrimitiveTypeCode.Byte:
                        return (byte)this;
                    case PrimitiveTypeCode.Int16Nullable:
                        return (short?)this;
                    case PrimitiveTypeCode.Int16:
                        return (short)this;
                    case PrimitiveTypeCode.UInt16Nullable:
                        return (ushort?)this;
                    case PrimitiveTypeCode.UInt16:
                        return (ushort)this;
                    case PrimitiveTypeCode.Int32Nullable:
                        return (int?)this;
                    case PrimitiveTypeCode.Int32:
                        return (int)this;
                    case PrimitiveTypeCode.UInt32Nullable:
                        return (uint?)this;
                    case PrimitiveTypeCode.UInt32:
                        return (uint)this;
                    case PrimitiveTypeCode.Int64Nullable:
                        return (long?)this;
                    case PrimitiveTypeCode.Int64:
                        return (long)this;
                    case PrimitiveTypeCode.UInt64Nullable:
                        return (ulong?)this;
                    case PrimitiveTypeCode.UInt64:
                        return (ulong)this;
                    case PrimitiveTypeCode.SingleNullable:
                        return (float?)this;
                    case PrimitiveTypeCode.Single:
                        return (float)this;
                    case PrimitiveTypeCode.DoubleNullable:
                        return (double?)this;
                    case PrimitiveTypeCode.Double:
                        return (double)this;
                    case PrimitiveTypeCode.DecimalNullable:
                        return (decimal?)this;
                    case PrimitiveTypeCode.Decimal:
                        return (decimal)this;
                    case PrimitiveTypeCode.DateTimeNullable:
                        return (DateTime?)this;
                    case PrimitiveTypeCode.DateTime:
                        return (DateTime)this;
                    case PrimitiveTypeCode.DateTimeOffsetNullable:
                        return (DateTimeOffset?)this;
                    case PrimitiveTypeCode.DateTimeOffset:
                        return (DateTimeOffset)this;
                    case PrimitiveTypeCode.String:
                        return (string)this;
                    case PrimitiveTypeCode.GuidNullable:
                        return (Guid?)this;
                    case PrimitiveTypeCode.Guid:
                        return (Guid)this;
                    case PrimitiveTypeCode.Uri:
                        return (Uri)this;
                    case PrimitiveTypeCode.TimeSpanNullable:
                        return (TimeSpan?)this;
                    case PrimitiveTypeCode.TimeSpan:
                        return (TimeSpan)this;
                    case PrimitiveTypeCode.BigIntegerNullable:
                        return ToBigIntegerNullable(this);
                    case PrimitiveTypeCode.BigInteger:
                        return ToBigInteger(this);
                }
            }

            return ToObject(objectType, JsonSerializer.CreateDefault());
        }

        public T ToObject<T>(JsonSerializer jsonSerializer)
        {
            return (T)ToObject(typeof(T), jsonSerializer);
        }

        public object ToObject(Type objectType, JsonSerializer jsonSerializer)
        {
            ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");

            using (JTokenReader jsonReader = new JTokenReader(this))
            {
                return jsonSerializer.Deserialize(jsonReader, objectType);
            }
        }

        public static JToken ReadFrom(JsonReader reader)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");

            if (reader.TokenType == JsonToken.None)
            {
                if (!reader.Read())
                {
                    throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader.");
                }
            }

            IJsonLineInfo lineInfo = reader as IJsonLineInfo;

            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    return JObject.Load(reader);
                case JsonToken.StartArray:
                    return JArray.Load(reader);
                case JsonToken.StartConstructor:
                    return JConstructor.Load(reader);
                case JsonToken.PropertyName:
                    return JProperty.Load(reader);
                case JsonToken.String:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Date:
                case JsonToken.Boolean:
                case JsonToken.Bytes:
                    JValue v = new JValue(reader.Value);
                    v.SetLineInfo(lineInfo);
                    return v;
                case JsonToken.Comment:
                    v = JValue.CreateComment(reader.Value.ToString());
                    v.SetLineInfo(lineInfo);
                    return v;
                case JsonToken.Null:
                    v = JValue.CreateNull();
                    v.SetLineInfo(lineInfo);
                    return v;
                case JsonToken.Undefined:
                    v = JValue.CreateUndefined();
                    v.SetLineInfo(lineInfo);
                    return v;
                default:
                    throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader. Unexpected token: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }
        }

        public static JToken Parse(string json)
        {
            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
            {
                JToken t = Load(reader);

                if (reader.Read() && reader.TokenType != JsonToken.Comment)
                {
                    throw JsonReaderException.Create(reader, "Additional text found in JSON string after parsing content.");
                }

                return t;
            }
        }

        public static JToken Load(JsonReader reader)
        {
            return ReadFrom(reader);
        }

        internal void SetLineInfo(IJsonLineInfo lineInfo)
        {
            if (lineInfo == null || !lineInfo.HasLineInfo())
            {
                return;
            }

            SetLineInfo(lineInfo.LineNumber, lineInfo.LinePosition);
        }

        internal void SetLineInfo(int lineNumber, int linePosition)
        {
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            return (_lineNumber != null && _linePosition != null);
        }

        public JToken SelectToken(string path)
        {
            return SelectToken(path, false);
        }

        public JToken SelectToken(string path, bool errorWhenNoMatch)
        {
            JPath p = new JPath(path);

            JToken token = null;
            foreach (JToken t in p.Evaluate(this, errorWhenNoMatch))
            {
                if (token != null)
                {
                    throw new JsonException("Path returned multiple tokens.");
                }

                token = t;
            }

            return token;
        }

        public IEnumerable<JToken> SelectTokens(string path)
        {
            return SelectTokens(path, false);
        }

        public IEnumerable<JToken> SelectTokens(string path, bool errorWhenNoMatch)
        {
            JPath p = new JPath(path);
            return p.Evaluate(this, errorWhenNoMatch);
        }

        protected virtual DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicProxyMetaObject<JToken>(parameter, this, new DynamicProxy<JToken>(), true);
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return GetMetaObject(parameter);
        }

        object ICloneable.Clone()
        {
            return DeepClone();
        }

        public JToken DeepClone()
        {
            return CloneToken();
        }

        #endregion

        public static explicit operator bool(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BooleanTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return Convert.ToBoolean((int)(BigInteger)v.Value);
            }

            return Convert.ToBoolean(v.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator DateTimeOffset(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, DateTimeTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is DateTimeOffset)
            {
                return (DateTimeOffset)v.Value;
            }
            if (v.Value is string)
            {
                return DateTimeOffset.Parse((string)v.Value, CultureInfo.InvariantCulture);
            }
            return new DateTimeOffset(Convert.ToDateTime(v.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator bool?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BooleanTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return Convert.ToBoolean((int)(BigInteger)v.Value);
            }

            return (v.Value != null) ? (bool?)Convert.ToBoolean(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator long(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (long)(BigInteger)v.Value;
            }

            return Convert.ToInt64(v.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator DateTime?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, DateTimeTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is DateTimeOffset)
            {
                return ((DateTimeOffset)v.Value).DateTime;
            }

            return (v.Value != null) ? (DateTime?)Convert.ToDateTime(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator DateTimeOffset?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, DateTimeTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value == null)
            {
                return null;
            }
            if (v.Value is DateTimeOffset)
            {
                return (DateTimeOffset?)v.Value;
            }
            if (v.Value is string)
            {
                return DateTimeOffset.Parse((string)v.Value, CultureInfo.InvariantCulture);
            }
            return new DateTimeOffset(Convert.ToDateTime(v.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator decimal?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (decimal?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (decimal?)Convert.ToDecimal(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator double?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (double?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (double?)Convert.ToDouble(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator char?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, CharTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (char?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (char?)Convert.ToChar(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator int(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (int)(BigInteger)v.Value;
            }

            return Convert.ToInt32(v.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator short(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (short)(BigInteger)v.Value;
            }

            return Convert.ToInt16(v.Value, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static explicit operator ushort(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (ushort)(BigInteger)v.Value;
            }

            return Convert.ToUInt16(v.Value, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static explicit operator char(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, CharTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (char)(BigInteger)v.Value;
            }

            return Convert.ToChar(v.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator byte(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (byte)(BigInteger)v.Value;
            }

            return Convert.ToByte(v.Value, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static explicit operator sbyte(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (sbyte)(BigInteger)v.Value;
            }

            return Convert.ToSByte(v.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator int?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (int?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (int?)Convert.ToInt32(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator short?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (short?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (short?)Convert.ToInt16(v.Value, CultureInfo.InvariantCulture) : null;
        }

        [CLSCompliant(false)]
        public static explicit operator ushort?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (ushort?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (ushort?)Convert.ToUInt16(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator byte?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (byte?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (byte?)Convert.ToByte(v.Value, CultureInfo.InvariantCulture) : null;
        }

        [CLSCompliant(false)]
        public static explicit operator sbyte?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (sbyte?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (sbyte?)Convert.ToByte(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator DateTime(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, DateTimeTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is DateTimeOffset)
            {
                return ((DateTimeOffset)v.Value).DateTime;
            }

            return Convert.ToDateTime(v.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator long?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (long?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (long?)Convert.ToInt64(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator float?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (float?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (float?)Convert.ToSingle(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator decimal(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (decimal)(BigInteger)v.Value;
            }

            return Convert.ToDecimal(v.Value, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static explicit operator uint?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (uint?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (uint?)Convert.ToUInt32(v.Value, CultureInfo.InvariantCulture) : null;
        }

        [CLSCompliant(false)]
        public static explicit operator ulong?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (ulong?)(BigInteger)v.Value;
            }

            return (v.Value != null) ? (ulong?)Convert.ToUInt64(v.Value, CultureInfo.InvariantCulture) : null;
        }

        public static explicit operator double(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (double)(BigInteger)v.Value;
            }

            return Convert.ToDouble(v.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator float(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (float)(BigInteger)v.Value;
            }

            return Convert.ToSingle(v.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator string(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, StringTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to String.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value == null)
            {
                return null;
            }
            if (v.Value is byte[])
            {
                return Convert.ToBase64String((byte[])v.Value);
            }
            if (v.Value is BigInteger)
            {
                return ((BigInteger)v.Value).ToString(CultureInfo.InvariantCulture);
            }

            return Convert.ToString(v.Value, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static explicit operator uint(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (uint)(BigInteger)v.Value;
            }

            return Convert.ToUInt32(v.Value, CultureInfo.InvariantCulture);
        }

        [CLSCompliant(false)]
        public static explicit operator ulong(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, NumberTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is BigInteger)
            {
                return (ulong)(BigInteger)v.Value;
            }

            return Convert.ToUInt64(v.Value, CultureInfo.InvariantCulture);
        }

        public static explicit operator byte[](JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, BytesTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is string)
            {
                return Convert.FromBase64String(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
            }
            if (v.Value is BigInteger)
            {
                return ((BigInteger)v.Value).ToByteArray();
            }

            if (v.Value is byte[])
            {
                return (byte[])v.Value;
            }

            throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
        }

        public static explicit operator Guid(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, GuidTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value is byte[])
            {
                return new Guid((byte[])v.Value);
            }

            return (v.Value is Guid) ? (Guid)v.Value : new Guid(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator Guid?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, GuidTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value == null)
            {
                return null;
            }

            if (v.Value is byte[])
            {
                return new Guid((byte[])v.Value);
            }

            return (v.Value is Guid) ? (Guid)v.Value : new Guid(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator TimeSpan(JToken value)
        {
            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, TimeSpanTypes, false))
            {
                throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            return (v.Value is TimeSpan) ? (TimeSpan)v.Value : ConvertUtils.ParseTimeSpan(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator TimeSpan?(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, TimeSpanTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value == null)
            {
                return null;
            }

            return (v.Value is TimeSpan) ? (TimeSpan)v.Value : ConvertUtils.ParseTimeSpan(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
        }

        public static explicit operator Uri(JToken value)
        {
            if (value == null)
            {
                return null;
            }

            JValue v = EnsureValue(value);
            if (v == null || !ValidateToken(v, UriTypes, true))
            {
                throw new ArgumentException("Can not convert {0} to Uri.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
            }

            if (v.Value == null)
            {
                return null;
            }

            return (v.Value is Uri) ? (Uri)v.Value : new Uri(Convert.ToString(v.Value, CultureInfo.InvariantCulture));
        }

        public static implicit operator JToken(bool value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(DateTimeOffset value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(byte value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(byte? value)
        {
            return new JValue(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JToken(sbyte value)
        {
            return new JValue(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JToken(sbyte? value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(bool? value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(long value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(DateTime? value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(DateTimeOffset? value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(decimal? value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(double? value)
        {
            return new JValue(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JToken(short value)
        {
            return new JValue(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JToken(ushort value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(int value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(int? value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(DateTime value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(long? value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(float? value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(decimal value)
        {
            return new JValue(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JToken(short? value)
        {
            return new JValue(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JToken(ushort? value)
        {
            return new JValue(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JToken(uint? value)
        {
            return new JValue(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JToken(ulong? value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(double value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(float value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(string value)
        {
            return new JValue(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JToken(uint value)
        {
            return new JValue(value);
        }

        [CLSCompliant(false)]
        public static implicit operator JToken(ulong value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(byte[] value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(Uri value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(TimeSpan value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(TimeSpan? value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(Guid value)
        {
            return new JValue(value);
        }

        public static implicit operator JToken(Guid? value)
        {
            return new JValue(value);
        }
    }
}