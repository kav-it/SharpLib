using System;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace SharpLib.Json
{
    public class JsonTextWriter : JsonWriter
    {
        private readonly TextWriter _writer;

        private Base64Encoder _base64Encoder;

        private char _indentChar;

        private int _indentation;

        private char _quoteChar;

        private bool _quoteName;

        private bool[] _charEscapeFlags;

        private char[] _writeBuffer;

        private char[] _indentChars;

        private Base64Encoder Base64Encoder
        {
            get { return _base64Encoder ?? (_base64Encoder = new Base64Encoder(_writer)); }
        }

        public int Indentation
        {
            get { return _indentation; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Indentation value must be greater than 0.");
                }

                _indentation = value;
            }
        }

        public char QuoteChar
        {
            get { return _quoteChar; }
            set
            {
                if (value != '"' && value != '\'')
                {
                    throw new ArgumentException(@"Invalid JavaScript string quote character. Valid quote characters are ' and "".");
                }

                _quoteChar = value;
                UpdateCharEscapeFlags();
            }
        }

        public char IndentChar
        {
            get { return _indentChar; }
            set
            {
                if (value != _indentChar)
                {
                    _indentChar = value;
                    _indentChars = null;
                }
            }
        }

        public bool QuoteName
        {
            get { return _quoteName; }
            set { _quoteName = value; }
        }

        public JsonTextWriter(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException("textWriter");
            }

            _writer = textWriter;
            _quoteChar = '"';
            _quoteName = true;
            _indentChar = ' ';
            _indentation = 2;

            UpdateCharEscapeFlags();
        }

        public override void Flush()
        {
            _writer.Flush();
        }

        public override void Close()
        {
            base.Close();

            if (CloseOutput && _writer != null)
            {
                _writer.Close();
            }
        }

        public override void WriteStartObject()
        {
            InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);

            _writer.Write('{');
        }

        public override void WriteStartArray()
        {
            InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);

            _writer.Write('[');
        }

        public override void WriteStartConstructor(string name)
        {
            InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);

            _writer.Write("new ");
            _writer.Write(name);
            _writer.Write('(');
        }

        protected override void WriteEnd(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.EndObject:
                    _writer.Write('}');
                    break;
                case JsonToken.EndArray:
                    _writer.Write(']');
                    break;
                case JsonToken.EndConstructor:
                    _writer.Write(')');
                    break;
                default:
                    throw JsonWriterException.Create(this, "Invalid JsonToken: " + token, null);
            }
        }

        public override void WritePropertyName(string name)
        {
            InternalWritePropertyName(name);

            WriteEscapedString(name, _quoteName);

            _writer.Write(':');
        }

        public override void WritePropertyName(string name, bool escape)
        {
            InternalWritePropertyName(name);

            if (escape)
            {
                WriteEscapedString(name, _quoteName);
            }
            else
            {
                if (_quoteName)
                {
                    _writer.Write(_quoteChar);
                }

                _writer.Write(name);

                if (_quoteName)
                {
                    _writer.Write(_quoteChar);
                }
            }

            _writer.Write(':');
        }

        internal override void OnStringEscapeHandlingChanged()
        {
            UpdateCharEscapeFlags();
        }

        private void UpdateCharEscapeFlags()
        {
            _charEscapeFlags = JavaScriptUtils.GetCharEscapeFlags(StringEscapeHandling, _quoteChar);
        }

        protected override void WriteIndent()
        {
            _writer.WriteLine();

            int currentIndentCount = Top * _indentation;

            if (currentIndentCount > 0)
            {
                if (_indentChars == null)
                {
                    _indentChars = new string(_indentChar, 10).ToCharArray();
                }

                while (currentIndentCount > 0)
                {
                    int writeCount = Math.Min(currentIndentCount, 10);

                    _writer.Write(_indentChars, 0, writeCount);

                    currentIndentCount -= writeCount;
                }
            }
        }

        protected override void WriteValueDelimiter()
        {
            _writer.Write(',');
        }

        protected override void WriteIndentSpace()
        {
            _writer.Write(' ');
        }

        private void WriteValueInternal(string value, JsonToken token)
        {
            _writer.Write(value);
        }

        #region WriteValue methods

        public override void WriteValue(object value)
        {
            if (value is BigInteger)
            {
                InternalWriteValue(JsonToken.Integer);
                WriteValueInternal(((BigInteger)value).ToString(CultureInfo.InvariantCulture), JsonToken.String);
            }
            else
            {
                base.WriteValue(value);
            }
        }

        public override void WriteNull()
        {
            InternalWriteValue(JsonToken.Null);
            WriteValueInternal(JsonConvert.Null, JsonToken.Null);
        }

        public override void WriteUndefined()
        {
            InternalWriteValue(JsonToken.Undefined);
            WriteValueInternal(JsonConvert.Undefined, JsonToken.Undefined);
        }

        public override void WriteRaw(string json)
        {
            InternalWriteRaw();

            _writer.Write(json);
        }

        public override void WriteValue(string value)
        {
            InternalWriteValue(JsonToken.String);

            if (value == null)
            {
                WriteValueInternal(JsonConvert.Null, JsonToken.Null);
            }
            else
            {
                WriteEscapedString(value, true);
            }
        }

        private void WriteEscapedString(string value, bool quote)
        {
            EnsureWriteBuffer();
            JavaScriptUtils.WriteEscapedJavaScriptString(_writer, value, _quoteChar, quote, _charEscapeFlags, StringEscapeHandling, ref _writeBuffer);
        }

        public override void WriteValue(int value)
        {
            InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        [CLSCompliant(false)]
        public override void WriteValue(uint value)
        {
            InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        public override void WriteValue(long value)
        {
            InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        [CLSCompliant(false)]
        public override void WriteValue(ulong value)
        {
            InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        public override void WriteValue(float value)
        {
            InternalWriteValue(JsonToken.Float);
            WriteValueInternal(JsonConvert.ToString(value, FloatFormatHandling, QuoteChar, false), JsonToken.Float);
        }

        public override void WriteValue(float? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                InternalWriteValue(JsonToken.Float);
                WriteValueInternal(JsonConvert.ToString(value.Value, FloatFormatHandling, QuoteChar, true), JsonToken.Float);
            }
        }

        public override void WriteValue(double value)
        {
            InternalWriteValue(JsonToken.Float);
            WriteValueInternal(JsonConvert.ToString(value, FloatFormatHandling, QuoteChar, false), JsonToken.Float);
        }

        public override void WriteValue(double? value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                InternalWriteValue(JsonToken.Float);
                WriteValueInternal(JsonConvert.ToString(value.Value, FloatFormatHandling, QuoteChar, true), JsonToken.Float);
            }
        }

        public override void WriteValue(bool value)
        {
            InternalWriteValue(JsonToken.Boolean);
            WriteValueInternal(JsonConvert.ToString(value), JsonToken.Boolean);
        }

        public override void WriteValue(short value)
        {
            InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        [CLSCompliant(false)]
        public override void WriteValue(ushort value)
        {
            InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        public override void WriteValue(char value)
        {
            InternalWriteValue(JsonToken.String);
            WriteValueInternal(JsonConvert.ToString(value), JsonToken.String);
        }

        public override void WriteValue(byte value)
        {
            InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        [CLSCompliant(false)]
        public override void WriteValue(sbyte value)
        {
            InternalWriteValue(JsonToken.Integer);
            WriteIntegerValue(value);
        }

        public override void WriteValue(decimal value)
        {
            InternalWriteValue(JsonToken.Float);
            WriteValueInternal(JsonConvert.ToString(value), JsonToken.Float);
        }

        public override void WriteValue(DateTime value)
        {
            InternalWriteValue(JsonToken.Date);
            value = DateTimeUtils.EnsureDateTime(value, DateTimeZoneHandling);

            if (string.IsNullOrEmpty(DateFormatString))
            {
                EnsureWriteBuffer();

                int pos = 0;
                _writeBuffer[pos++] = _quoteChar;
                pos = DateTimeUtils.WriteDateTimeString(_writeBuffer, pos, value, null, value.Kind, DateFormatHandling);
                _writeBuffer[pos++] = _quoteChar;

                _writer.Write(_writeBuffer, 0, pos);
            }
            else
            {
                _writer.Write(_quoteChar);
                _writer.Write(value.ToString(DateFormatString, Culture));
                _writer.Write(_quoteChar);
            }
        }

        public override void WriteValue(byte[] value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                InternalWriteValue(JsonToken.Bytes);
                _writer.Write(_quoteChar);
                Base64Encoder.Encode(value, 0, value.Length);
                Base64Encoder.Flush();
                _writer.Write(_quoteChar);
            }
        }

        public override void WriteValue(DateTimeOffset value)
        {
            InternalWriteValue(JsonToken.Date);

            if (string.IsNullOrEmpty(DateFormatString))
            {
                EnsureWriteBuffer();

                int pos = 0;
                _writeBuffer[pos++] = _quoteChar;
                pos = DateTimeUtils.WriteDateTimeString(_writeBuffer, pos, (DateFormatHandling == DateFormatHandling.IsoDateFormat) ? value.DateTime : value.UtcDateTime, value.Offset,
                    DateTimeKind.Local, DateFormatHandling);
                _writeBuffer[pos++] = _quoteChar;

                _writer.Write(_writeBuffer, 0, pos);
            }
            else
            {
                _writer.Write(_quoteChar);
                _writer.Write(value.ToString(DateFormatString, Culture));
                _writer.Write(_quoteChar);
            }
        }

        public override void WriteValue(Guid value)
        {
            InternalWriteValue(JsonToken.String);

            var text = value.ToString("D", CultureInfo.InvariantCulture);

            _writer.Write(_quoteChar);
            _writer.Write(text);
            _writer.Write(_quoteChar);
        }

        public override void WriteValue(TimeSpan value)
        {
            InternalWriteValue(JsonToken.String);

            string text = value.ToString(null, CultureInfo.InvariantCulture);

            _writer.Write(_quoteChar);
            _writer.Write(text);
            _writer.Write(_quoteChar);
        }

        public override void WriteValue(Uri value)
        {
            if (value == null)
            {
                WriteNull();
            }
            else
            {
                InternalWriteValue(JsonToken.String);
                WriteEscapedString(value.OriginalString, true);
            }
        }

        #endregion

        public override void WriteComment(string text)
        {
            InternalWriteComment();

            _writer.Write("/*");
            _writer.Write(text);
            _writer.Write("*/");
        }

        public override void WriteWhitespace(string ws)
        {
            InternalWriteWhitespace(ws);

            _writer.Write(ws);
        }

        private void EnsureWriteBuffer()
        {
            if (_writeBuffer == null)
            {
                _writeBuffer = new char[35];
            }
        }

        private void WriteIntegerValue(long value)
        {
            if (value >= 0 && value <= 9)
            {
                _writer.Write((char)('0' + value));
            }
            else
            {
                ulong uvalue = (value < 0) ? (ulong)-value : (ulong)value;

                if (value < 0)
                {
                    _writer.Write('-');
                }

                WriteIntegerValue(uvalue);
            }
        }

        private void WriteIntegerValue(ulong uvalue)
        {
            if (uvalue <= 9)
            {
                _writer.Write((char)('0' + uvalue));
            }
            else
            {
                EnsureWriteBuffer();

                int totalLength = MathUtils.IntLength(uvalue);
                int length = 0;

                do
                {
                    _writeBuffer[totalLength - ++length] = (char)('0' + (uvalue % 10));
                    uvalue /= 10;
                } while (uvalue != 0);

                _writer.Write(_writeBuffer, 0, length);
            }
        }
    }
}