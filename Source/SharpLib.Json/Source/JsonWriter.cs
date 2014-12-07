using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace SharpLib.Json
{
    public abstract class JsonWriter : IDisposable
    {
        #region Перечисления

        internal enum State
        {
            Start,

            Property,

            ObjectStart,

            Object,

            ArrayStart,

            Array,

            ConstructorStart,

            Constructor,

            Closed,

            Error
        }

        #endregion

        #region Поля

        private static readonly State[][] StateArray;

        internal static readonly State[][] StateArrayTempate =
        {
            new[] { State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error },
            new[] { State.ObjectStart, State.ObjectStart, State.Error, State.Error, State.ObjectStart, State.ObjectStart, State.ObjectStart, State.ObjectStart, State.Error, State.Error },
            new[] { State.ArrayStart, State.ArrayStart, State.Error, State.Error, State.ArrayStart, State.ArrayStart, State.ArrayStart, State.ArrayStart, State.Error, State.Error },
            new[]
            {
                State.ConstructorStart, State.ConstructorStart, State.Error, State.Error, State.ConstructorStart, State.ConstructorStart, State.ConstructorStart, State.ConstructorStart, State.Error,
                State.Error
            },
            new[] { State.Property, State.Error, State.Property, State.Property, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error },
            new[] { State.Start, State.Property, State.ObjectStart, State.Object, State.ArrayStart, State.Array, State.Constructor, State.Constructor, State.Error, State.Error },
            new[] { State.Start, State.Property, State.ObjectStart, State.Object, State.ArrayStart, State.Array, State.Constructor, State.Constructor, State.Error, State.Error },
            new[] { State.Start, State.Object, State.Error, State.Error, State.Array, State.Array, State.Constructor, State.Constructor, State.Error, State.Error }
        };

        private readonly List<JsonPosition> _stack;

        private CultureInfo _culture;

        private JsonPosition _currentPosition;

        private State _currentState;

        private Formatting _formatting;

        private StringEscapeHandling _stringEscapeHandling;

        #endregion

        #region Свойства

        public bool CloseOutput { get; set; }

        protected internal int Top
        {
            get
            {
                int depth = _stack.Count;
                if (Peek() != JsonContainerType.None)
                {
                    depth++;
                }

                return depth;
            }
        }

        public WriteState WriteState
        {
            get
            {
                switch (_currentState)
                {
                    case State.Error:
                        return WriteState.Error;
                    case State.Closed:
                        return WriteState.Closed;
                    case State.Object:
                    case State.ObjectStart:
                        return WriteState.Object;
                    case State.Array:
                    case State.ArrayStart:
                        return WriteState.Array;
                    case State.Constructor:
                    case State.ConstructorStart:
                        return WriteState.Constructor;
                    case State.Property:
                        return WriteState.Property;
                    case State.Start:
                        return WriteState.Start;
                    default:
                        throw JsonWriterException.Create(this, "Invalid state: " + _currentState, null);
                }
            }
        }

        internal string ContainerPath
        {
            get
            {
                if (_currentPosition.Type == JsonContainerType.None)
                {
                    return string.Empty;
                }

                return JsonPosition.BuildPath(_stack);
            }
        }

        public string Path
        {
            get
            {
                if (_currentPosition.Type == JsonContainerType.None)
                {
                    return string.Empty;
                }

                bool insideContainer = (_currentState != State.ArrayStart
                                        && _currentState != State.ConstructorStart
                                        && _currentState != State.ObjectStart);

                IEnumerable<JsonPosition> positions = (!insideContainer)
                    ? _stack
                    : _stack.Concat(new[] { _currentPosition });

                return JsonPosition.BuildPath(positions);
            }
        }

        public Formatting Formatting
        {
            get { return _formatting; }
            set { _formatting = value; }
        }

        public DateFormatHandling DateFormatHandling { get; set; }

        public DateTimeZoneHandling DateTimeZoneHandling { get; set; }

        public StringEscapeHandling StringEscapeHandling
        {
            get { return _stringEscapeHandling; }
            set
            {
                _stringEscapeHandling = value;
                OnStringEscapeHandlingChanged();
            }
        }

        public FloatFormatHandling FloatFormatHandling { get; set; }

        public string DateFormatString { get; set; }

        public CultureInfo Culture
        {
            get { return _culture ?? CultureInfo.InvariantCulture; }
            set { _culture = value; }
        }

        #endregion

        #region Конструктор

        static JsonWriter()
        {
            StateArray = BuildStateArray();
        }

        protected JsonWriter()
        {
            _stack = new List<JsonPosition>(4);
            _currentState = State.Start;
            _formatting = Formatting.None;
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;

            CloseOutput = true;
        }

        #endregion

        #region Методы

        internal static State[][] BuildStateArray()
        {
            var allStates = StateArrayTempate.ToList();
            var errorStates = StateArrayTempate[0];
            var valueStates = StateArrayTempate[7];

            foreach (JsonToken valueToken in EnumUtils.GetValues(typeof(JsonToken)))
            {
                if (allStates.Count <= (int)valueToken)
                {
                    switch (valueToken)
                    {
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.String:
                        case JsonToken.Boolean:
                        case JsonToken.Null:
                        case JsonToken.Undefined:
                        case JsonToken.Date:
                        case JsonToken.Bytes:
                            allStates.Add(valueStates);
                            break;
                        default:
                            allStates.Add(errorStates);
                            break;
                    }
                }
            }

            return allStates.ToArray();
        }

        internal virtual void OnStringEscapeHandlingChanged()
        {
        }

        internal void UpdateScopeWithFinishedValue()
        {
            if (_currentPosition.HasIndex)
            {
                _currentPosition.Position++;
            }
        }

        private void Push(JsonContainerType value)
        {
            if (_currentPosition.Type != JsonContainerType.None)
            {
                _stack.Add(_currentPosition);
            }

            _currentPosition = new JsonPosition(value);
        }

        private JsonContainerType Pop()
        {
            JsonPosition oldPosition = _currentPosition;

            if (_stack.Count > 0)
            {
                _currentPosition = _stack[_stack.Count - 1];
                _stack.RemoveAt(_stack.Count - 1);
            }
            else
            {
                _currentPosition = new JsonPosition();
            }

            return oldPosition.Type;
        }

        private JsonContainerType Peek()
        {
            return _currentPosition.Type;
        }

        public abstract void Flush();

        public virtual void Close()
        {
            AutoCompleteAll();
        }

        public virtual void WriteStartObject()
        {
            InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
        }

        public virtual void WriteEndObject()
        {
            InternalWriteEnd(JsonContainerType.Object);
        }

        public virtual void WriteStartArray()
        {
            InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
        }

        public virtual void WriteEndArray()
        {
            InternalWriteEnd(JsonContainerType.Array);
        }

        public virtual void WriteStartConstructor(string name)
        {
            InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
        }

        public virtual void WriteEndConstructor()
        {
            InternalWriteEnd(JsonContainerType.Constructor);
        }

        public virtual void WritePropertyName(string name)
        {
            InternalWritePropertyName(name);
        }

        public virtual void WritePropertyName(string name, bool escape)
        {
            WritePropertyName(name);
        }

        public virtual void WriteEnd()
        {
            WriteEnd(Peek());
        }

        public void WriteToken(JsonReader reader)
        {
            WriteToken(reader, true, true);
        }

        public void WriteToken(JsonReader reader, bool writeChildren)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");

            WriteToken(reader, writeChildren, true);
        }

        internal void WriteToken(JsonReader reader, bool writeChildren, bool writeDateConstructorAsDate)
        {
            int initialDepth;

            if (reader.TokenType == JsonToken.None)
            {
                initialDepth = -1;
            }
            else if (!IsStartToken(reader.TokenType))
            {
                initialDepth = reader.Depth + 1;
            }
            else
            {
                initialDepth = reader.Depth;
            }

            WriteToken(reader, initialDepth, writeChildren, writeDateConstructorAsDate);
        }

        internal void WriteToken(JsonReader reader, int initialDepth, bool writeChildren, bool writeDateConstructorAsDate)
        {
            do
            {
                switch (reader.TokenType)
                {
                    case JsonToken.None:

                        break;
                    case JsonToken.StartObject:
                        WriteStartObject();
                        break;
                    case JsonToken.StartArray:
                        WriteStartArray();
                        break;
                    case JsonToken.StartConstructor:
                        string constructorName = reader.Value.ToString();

                        if (writeDateConstructorAsDate && string.Equals(constructorName, "Date", StringComparison.Ordinal))
                        {
                            WriteConstructorDate(reader);
                        }
                        else
                        {
                            WriteStartConstructor(reader.Value.ToString());
                        }
                        break;
                    case JsonToken.PropertyName:
                        WritePropertyName(reader.Value.ToString());
                        break;
                    case JsonToken.Comment:
                        WriteComment((reader.Value != null) ? reader.Value.ToString() : null);
                        break;
                    case JsonToken.Integer:
                        if (reader.Value is BigInteger)
                        {
                            WriteValue((BigInteger)reader.Value);
                        }
                        else
                        {
                            WriteValue(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
                        }
                        break;
                    case JsonToken.Float:
                        object value = reader.Value;

                        if (value is decimal)
                        {
                            WriteValue((decimal)value);
                        }
                        else if (value is double)
                        {
                            WriteValue((double)value);
                        }
                        else if (value is float)
                        {
                            WriteValue((float)value);
                        }
                        else
                        {
                            WriteValue(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                        }
                        break;
                    case JsonToken.String:
                        WriteValue(reader.Value.ToString());
                        break;
                    case JsonToken.Boolean:
                        WriteValue(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
                        break;
                    case JsonToken.Null:
                        WriteNull();
                        break;
                    case JsonToken.Undefined:
                        WriteUndefined();
                        break;
                    case JsonToken.EndObject:
                        WriteEndObject();
                        break;
                    case JsonToken.EndArray:
                        WriteEndArray();
                        break;
                    case JsonToken.EndConstructor:
                        WriteEndConstructor();
                        break;
                    case JsonToken.Date:
                        if (reader.Value is DateTimeOffset)
                        {
                            WriteValue((DateTimeOffset)reader.Value);
                        }
                        else
                        {
                            WriteValue(Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture));
                        }
                        break;
                    case JsonToken.Raw:
                        WriteRawValue((reader.Value != null) ? reader.Value.ToString() : null);
                        break;
                    case JsonToken.Bytes:
                        if (reader.Value is Guid)
                        {
                            WriteValue((Guid)reader.Value);
                        }
                        else
                        {
                            WriteValue((byte[])reader.Value);
                        }
                        break;
                    default:
                        throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", reader.TokenType, "Unexpected token type.");
                }
            } while (
                initialDepth - 1 < reader.Depth - (IsEndToken(reader.TokenType) ? 1 : 0)
                && writeChildren
                && reader.Read());
        }

        private void WriteConstructorDate(JsonReader reader)
        {
            if (!reader.Read())
            {
                throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
            }
            if (reader.TokenType != JsonToken.Integer)
            {
                throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected Integer, got " + reader.TokenType, null);
            }

            long ticks = (long)reader.Value;
            DateTime date = DateTimeUtils.ConvertJavaScriptTicksToDateTime(ticks);

            if (!reader.Read())
            {
                throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
            }
            if (reader.TokenType != JsonToken.EndConstructor)
            {
                throw JsonWriterException.Create(this, "Unexpected token when reading date constructor. Expected EndConstructor, got " + reader.TokenType, null);
            }

            WriteValue(date);
        }

        internal static bool IsEndToken(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.EndObject:
                case JsonToken.EndArray:
                case JsonToken.EndConstructor:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsStartToken(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.StartObject:
                case JsonToken.StartArray:
                case JsonToken.StartConstructor:
                    return true;
                default:
                    return false;
            }
        }

        private void WriteEnd(JsonContainerType type)
        {
            switch (type)
            {
                case JsonContainerType.Object:
                    WriteEndObject();
                    break;
                case JsonContainerType.Array:
                    WriteEndArray();
                    break;
                case JsonContainerType.Constructor:
                    WriteEndConstructor();
                    break;
                default:
                    throw JsonWriterException.Create(this, "Unexpected type when writing end: " + type, null);
            }
        }

        private void AutoCompleteAll()
        {
            while (Top > 0)
            {
                WriteEnd();
            }
        }

        private JsonToken GetCloseTokenForType(JsonContainerType type)
        {
            switch (type)
            {
                case JsonContainerType.Object:
                    return JsonToken.EndObject;
                case JsonContainerType.Array:
                    return JsonToken.EndArray;
                case JsonContainerType.Constructor:
                    return JsonToken.EndConstructor;
                default:
                    throw JsonWriterException.Create(this, "No close token for type: " + type, null);
            }
        }

        private void AutoCompleteClose(JsonContainerType type)
        {
            int levelsToComplete = 0;

            if (_currentPosition.Type == type)
            {
                levelsToComplete = 1;
            }
            else
            {
                int top = Top - 2;
                for (int i = top; i >= 0; i--)
                {
                    int currentLevel = top - i;

                    if (_stack[currentLevel].Type == type)
                    {
                        levelsToComplete = i + 2;
                        break;
                    }
                }
            }

            if (levelsToComplete == 0)
            {
                throw JsonWriterException.Create(this, "No token to close.", null);
            }

            for (int i = 0; i < levelsToComplete; i++)
            {
                JsonToken token = GetCloseTokenForType(Pop());

                if (_currentState == State.Property)
                {
                    WriteNull();
                }

                if (_formatting == Formatting.Indented)
                {
                    if (_currentState != State.ObjectStart && _currentState != State.ArrayStart)
                    {
                        WriteIndent();
                    }
                }

                WriteEnd(token);

                JsonContainerType currentLevelType = Peek();

                switch (currentLevelType)
                {
                    case JsonContainerType.Object:
                        _currentState = State.Object;
                        break;
                    case JsonContainerType.Array:
                        _currentState = State.Array;
                        break;
                    case JsonContainerType.Constructor:
                        _currentState = State.Array;
                        break;
                    case JsonContainerType.None:
                        _currentState = State.Start;
                        break;
                    default:
                        throw JsonWriterException.Create(this, "Unknown JsonType: " + currentLevelType, null);
                }
            }
        }

        protected virtual void WriteEnd(JsonToken token)
        {
        }

        protected virtual void WriteIndent()
        {
        }

        protected virtual void WriteValueDelimiter()
        {
        }

        protected virtual void WriteIndentSpace()
        {
        }

        internal void AutoComplete(JsonToken tokenBeingWritten)
        {
            State newState = StateArray[(int)tokenBeingWritten][(int)_currentState];

            if (newState == State.Error)
            {
                throw JsonWriterException.Create(this,
                    "Token {0} in state {1} would result in an invalid JSON object.".FormatWith(CultureInfo.InvariantCulture, tokenBeingWritten.ToString(), _currentState.ToString()), null);
            }

            if ((_currentState == State.Object || _currentState == State.Array || _currentState == State.Constructor) && tokenBeingWritten != JsonToken.Comment)
            {
                WriteValueDelimiter();
            }

            if (_formatting == Formatting.Indented)
            {
                if (_currentState == State.Property)
                {
                    WriteIndentSpace();
                }

                if ((_currentState == State.Array || _currentState == State.ArrayStart || _currentState == State.Constructor || _currentState == State.ConstructorStart)
                    || (tokenBeingWritten == JsonToken.PropertyName && _currentState != State.Start))
                {
                    WriteIndent();
                }
            }

            _currentState = newState;
        }

        public virtual void WriteNull()
        {
            InternalWriteValue(JsonToken.Null);
        }

        public virtual void WriteUndefined()
        {
            InternalWriteValue(JsonToken.Undefined);
        }

        public virtual void WriteRaw(string json)
        {
            InternalWriteRaw();
        }

        public virtual void WriteRawValue(string json)
        {
            UpdateScopeWithFinishedValue();
            AutoComplete(JsonToken.Undefined);
            WriteRaw(json);
        }

        public virtual void WriteValue(string value)
        {
            InternalWriteValue(JsonToken.String);
        }

        public virtual void WriteValue(int value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(uint value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        public virtual void WriteValue(long value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(ulong value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        public virtual void WriteValue(float value)
        {
            InternalWriteValue(JsonToken.Float);
        }

        public virtual void WriteValue(double value)
        {
            InternalWriteValue(JsonToken.Float);
        }

        public virtual void WriteValue(bool value)
        {
            InternalWriteValue(JsonToken.Boolean);
        }

        public virtual void WriteValue(short value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(ushort value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        public virtual void WriteValue(char value)
        {
            InternalWriteValue(JsonToken.String);
        }

        public virtual void WriteValue(byte value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(sbyte value)
        {
            InternalWriteValue(JsonToken.Integer);
        }

        public virtual void WriteValue(decimal value)
        {
            InternalWriteValue(JsonToken.Float);
        }

        public virtual void WriteValue(DateTime value)
        {
            InternalWriteValue(JsonToken.Date);
        }

        public virtual void WriteValue(DateTimeOffset value)
        {
            InternalWriteValue(JsonToken.Date);
        }

        public virtual void WriteValue(Guid value)
        {
            InternalWriteValue(JsonToken.String);
        }

        public virtual void WriteValue(TimeSpan value)
        {
            InternalWriteValue(JsonToken.String);
        }

        public virtual void WriteValue(int? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(uint? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(long? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(ulong? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(float? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(double? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(bool? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(short? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(ushort? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(char? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(byte? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        [CLSCompliant(false)]
        public virtual void WriteValue(sbyte? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(decimal? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(DateTime? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(DateTimeOffset? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(Guid? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(TimeSpan? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                WriteValue(value.Value);
            }
        }

        public virtual void WriteValue(byte[] value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                InternalWriteValue(JsonToken.Bytes);
            }
        }

        public virtual void WriteValue(Uri value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                InternalWriteValue(JsonToken.String);
            }
        }

        public virtual void WriteValue(object value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                if (value is BigInteger)
                {
                    throw CreateUnsupportedTypeException(this, value);
                }

                WriteValue(this, ConvertUtils.GetTypeCode(value.GetType()), value);
            }
        }

        public virtual void WriteComment(string text)
        {
            InternalWriteComment();
        }

        public virtual void WriteWhitespace(string ws)
        {
            InternalWriteWhitespace(ws);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_currentState != State.Closed)
            {
                Close();
            }
        }

        internal static void WriteValue(JsonWriter writer, PrimitiveTypeCode typeCode, object value)
        {
            switch (typeCode)
            {
                case PrimitiveTypeCode.Char:
                    writer.WriteValue((char)value);
                    break;
                case PrimitiveTypeCode.CharNullable:
                    writer.WriteValue((value == null) ? (char?)null : (char)value);
                    break;
                case PrimitiveTypeCode.Boolean:
                    writer.WriteValue((bool)value);
                    break;
                case PrimitiveTypeCode.BooleanNullable:
                    writer.WriteValue((value == null) ? (bool?)null : (bool)value);
                    break;
                case PrimitiveTypeCode.SByte:
                    writer.WriteValue((sbyte)value);
                    break;
                case PrimitiveTypeCode.SByteNullable:
                    writer.WriteValue((value == null) ? (sbyte?)null : (sbyte)value);
                    break;
                case PrimitiveTypeCode.Int16:
                    writer.WriteValue((short)value);
                    break;
                case PrimitiveTypeCode.Int16Nullable:
                    writer.WriteValue((value == null) ? (short?)null : (short)value);
                    break;
                case PrimitiveTypeCode.UInt16:
                    writer.WriteValue((ushort)value);
                    break;
                case PrimitiveTypeCode.UInt16Nullable:
                    writer.WriteValue((value == null) ? (ushort?)null : (ushort)value);
                    break;
                case PrimitiveTypeCode.Int32:
                    writer.WriteValue((int)value);
                    break;
                case PrimitiveTypeCode.Int32Nullable:
                    writer.WriteValue((value == null) ? (int?)null : (int)value);
                    break;
                case PrimitiveTypeCode.Byte:
                    writer.WriteValue((byte)value);
                    break;
                case PrimitiveTypeCode.ByteNullable:
                    writer.WriteValue((value == null) ? (byte?)null : (byte)value);
                    break;
                case PrimitiveTypeCode.UInt32:
                    writer.WriteValue((uint)value);
                    break;
                case PrimitiveTypeCode.UInt32Nullable:
                    writer.WriteValue((value == null) ? (uint?)null : (uint)value);
                    break;
                case PrimitiveTypeCode.Int64:
                    writer.WriteValue((long)value);
                    break;
                case PrimitiveTypeCode.Int64Nullable:
                    writer.WriteValue((value == null) ? (long?)null : (long)value);
                    break;
                case PrimitiveTypeCode.UInt64:
                    writer.WriteValue((ulong)value);
                    break;
                case PrimitiveTypeCode.UInt64Nullable:
                    writer.WriteValue((value == null) ? (ulong?)null : (ulong)value);
                    break;
                case PrimitiveTypeCode.Single:
                    writer.WriteValue((float)value);
                    break;
                case PrimitiveTypeCode.SingleNullable:
                    writer.WriteValue((value == null) ? (float?)null : (float)value);
                    break;
                case PrimitiveTypeCode.Double:
                    writer.WriteValue((double)value);
                    break;
                case PrimitiveTypeCode.DoubleNullable:
                    writer.WriteValue((value == null) ? (double?)null : (double)value);
                    break;
                case PrimitiveTypeCode.DateTime:
                    writer.WriteValue((DateTime)value);
                    break;
                case PrimitiveTypeCode.DateTimeNullable:
                    writer.WriteValue((value == null) ? (DateTime?)null : (DateTime)value);
                    break;
                case PrimitiveTypeCode.DateTimeOffset:
                    writer.WriteValue((DateTimeOffset)value);
                    break;
                case PrimitiveTypeCode.DateTimeOffsetNullable:
                    writer.WriteValue((value == null) ? (DateTimeOffset?)null : (DateTimeOffset)value);
                    break;
                case PrimitiveTypeCode.Decimal:
                    writer.WriteValue((decimal)value);
                    break;
                case PrimitiveTypeCode.DecimalNullable:
                    writer.WriteValue((value == null) ? (decimal?)null : (decimal)value);
                    break;
                case PrimitiveTypeCode.Guid:
                    writer.WriteValue((Guid)value);
                    break;
                case PrimitiveTypeCode.GuidNullable:
                    writer.WriteValue((value == null) ? (Guid?)null : (Guid)value);
                    break;
                case PrimitiveTypeCode.TimeSpan:
                    writer.WriteValue((TimeSpan)value);
                    break;
                case PrimitiveTypeCode.TimeSpanNullable:
                    writer.WriteValue((value == null) ? (TimeSpan?)null : (TimeSpan)value);
                    break;
                case PrimitiveTypeCode.BigInteger:

                    writer.WriteValue((BigInteger)value);
                    break;
                case PrimitiveTypeCode.BigIntegerNullable:

                    writer.WriteValue((value == null) ? (BigInteger?)null : (BigInteger)value);
                    break;
                case PrimitiveTypeCode.Uri:
                    writer.WriteValue((Uri)value);
                    break;
                case PrimitiveTypeCode.String:
                    writer.WriteValue((string)value);
                    break;
                case PrimitiveTypeCode.Bytes:
                    writer.WriteValue((byte[])value);
                    break;
                case PrimitiveTypeCode.DBNull:
                    writer.WriteNull();
                    break;
                default:
                    if (value is IConvertible)
                    {
                        IConvertible convertable = (IConvertible)value;

                        TypeInformation typeInformation = ConvertUtils.GetTypeInformation(convertable);

                        PrimitiveTypeCode resolvedTypeCode = (typeInformation.TypeCode == PrimitiveTypeCode.Object) ? PrimitiveTypeCode.String : typeInformation.TypeCode;
                        Type resolvedType = (typeInformation.TypeCode == PrimitiveTypeCode.Object) ? typeof(string) : typeInformation.Type;

                        object convertedValue = convertable.ToType(resolvedType, CultureInfo.InvariantCulture);

                        WriteValue(writer, resolvedTypeCode, convertedValue);
                        break;
                    }
                    throw CreateUnsupportedTypeException(writer, value);
            }
        }

        private static JsonWriterException CreateUnsupportedTypeException(JsonWriter writer, object value)
        {
            return JsonWriterException.Create(writer,
                "Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()), null);
        }

        protected void SetWriteState(JsonToken token, object value)
        {
            switch (token)
            {
                case JsonToken.StartObject:
                    InternalWriteStart(token, JsonContainerType.Object);
                    break;
                case JsonToken.StartArray:
                    InternalWriteStart(token, JsonContainerType.Array);
                    break;
                case JsonToken.StartConstructor:
                    InternalWriteStart(token, JsonContainerType.Constructor);
                    break;
                case JsonToken.PropertyName:
                    if (!(value is string))
                    {
                        throw new ArgumentException("A name is required when setting property name state.", "value");
                    }

                    InternalWritePropertyName((string)value);
                    break;
                case JsonToken.Comment:
                    InternalWriteComment();
                    break;
                case JsonToken.Raw:
                    InternalWriteRaw();
                    break;
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Date:
                case JsonToken.Bytes:
                case JsonToken.Null:
                case JsonToken.Undefined:
                    InternalWriteValue(token);
                    break;
                case JsonToken.EndObject:
                    InternalWriteEnd(JsonContainerType.Object);
                    break;
                case JsonToken.EndArray:
                    InternalWriteEnd(JsonContainerType.Array);
                    break;
                case JsonToken.EndConstructor:
                    InternalWriteEnd(JsonContainerType.Constructor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("token");
            }
        }

        internal void InternalWriteEnd(JsonContainerType container)
        {
            AutoCompleteClose(container);
        }

        internal void InternalWritePropertyName(string name)
        {
            _currentPosition.PropertyName = name;
            AutoComplete(JsonToken.PropertyName);
        }

        internal void InternalWriteRaw()
        {
        }

        internal void InternalWriteStart(JsonToken token, JsonContainerType container)
        {
            UpdateScopeWithFinishedValue();
            AutoComplete(token);
            Push(container);
        }

        internal void InternalWriteValue(JsonToken token)
        {
            UpdateScopeWithFinishedValue();
            AutoComplete(token);
        }

        internal void InternalWriteWhitespace(string ws)
        {
            if (ws != null)
            {
                if (!StringUtils.IsWhiteSpace(ws))
                {
                    throw JsonWriterException.Create(this, "Only white space characters should be used.", null);
                }
            }
        }

        internal void InternalWriteComment()
        {
            AutoComplete(JsonToken.Comment);
        }

        #endregion
    }
}