﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SharpLib.Json
{
    public class JsonTextReader : JsonReader, IJsonLineInfo
    {
        #region Константы

        private const int MAXIMUM_JAVASCRIPT_INTEGER_CHARACTER_LENGTH = 380;

        private const char UNICODE_REPLACEMENT_CHAR = '\uFFFD';

        #endregion

        #region Поля

        private readonly TextReader _reader;

        internal PropertyNameTable NameTable;

        private StringBuffer _buffer;

        private int _charPos;

        private char[] _chars;

        private int _charsUsed;

        private bool _isEndOfFile;

        private int _lineNumber;

        private int _lineStartPos;

        private StringReference _stringReference;

        #endregion

        #region Свойства

        public int LineNumber
        {
            get
            {
                if (CurrentState == State.Start && LinePosition == 0)
                {
                    return 0;
                }

                return _lineNumber;
            }
        }

        public int LinePosition
        {
            get { return _charPos - _lineStartPos; }
        }

        #endregion

        #region Конструктор

        public JsonTextReader(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            _reader = reader;
            _lineNumber = 1;
            _chars = new char[1025];
        }

        #endregion

        #region Методы

        internal void SetCharBuffer(char[] chars)
        {
#if DEBUG
            _chars = chars;
#endif
        }

        private StringBuffer GetBuffer()
        {
            if (_buffer == null)
            {
                _buffer = new StringBuffer(1025);
            }
            else
            {
                _buffer.Position = 0;
            }

            return _buffer;
        }

        private void OnNewLine(int pos)
        {
            _lineNumber++;
            _lineStartPos = pos - 1;
        }

        private void ParseString(char quote)
        {
            _charPos++;

            ShiftBufferIfNeeded();
            ReadStringIntoBuffer(quote);
            SetPostValueState(true);

            if (_readType == ReadType.ReadAsBytes)
            {
                byte[] data = _stringReference.Length == 0 
                    ? new byte[0] 
                    : Convert.FromBase64CharArray(_stringReference.Chars, _stringReference.StartIndex, _stringReference.Length);

                SetToken(JsonToken.Bytes, data, false);
            }
            else if (_readType == ReadType.ReadAsString)
            {
                string text = _stringReference.ToString();

                SetToken(JsonToken.String, text, false);
                _quoteChar = quote;
            }
            else
            {
                string text = _stringReference.ToString();

                if (_dateParseHandling != DateParseHandling.None)
                {
                    DateParseHandling dateParseHandling;
                    if (_readType == ReadType.ReadAsDateTime)
                    {
                        dateParseHandling = DateParseHandling.DateTime;
                    }
                    else if (_readType == ReadType.ReadAsDateTimeOffset)
                    {
                        dateParseHandling = DateParseHandling.DateTimeOffset;
                    }
                    else
                    {
                        dateParseHandling = _dateParseHandling;
                    }

                    object dt;
                    if (DateTimeUtils.TryParseDateTime(text, dateParseHandling, DateTimeZoneHandling, DateFormatString, Culture, out dt))
                    {
                        SetToken(JsonToken.Date, dt, false);
                        return;
                    }
                }

                SetToken(JsonToken.String, text, false);
                _quoteChar = quote;
            }
        }

        private static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
        {
            const int CHAR_BYTE_COUNT = 2;

            Buffer.BlockCopy(src, srcOffset * CHAR_BYTE_COUNT, dst, dstOffset * CHAR_BYTE_COUNT, count * CHAR_BYTE_COUNT);
        }

        private void ShiftBufferIfNeeded()
        {
            int length = _chars.Length;
            if (length - _charPos <= length * 0.1)
            {
                int count = _charsUsed - _charPos;
                if (count > 0)
                {
                    BlockCopyChars(_chars, _charPos, _chars, 0, count);
                }

                _lineStartPos -= _charPos;
                _charPos = 0;
                _charsUsed = count;
                _chars[_charsUsed] = '\0';
            }
        }

        private int ReadData(bool append)
        {
            return ReadData(append, 0);
        }

        private int ReadData(bool append, int charsRequired)
        {
            if (_isEndOfFile)
            {
                return 0;
            }

            if (_charsUsed + charsRequired >= _chars.Length - 1)
            {
                if (append)
                {
                    int newArrayLength = Math.Max(_chars.Length * 2, _charsUsed + charsRequired + 1);

                    char[] dst = new char[newArrayLength];

                    BlockCopyChars(_chars, 0, dst, 0, _chars.Length);

                    _chars = dst;
                }
                else
                {
                    int remainingCharCount = _charsUsed - _charPos;

                    if (remainingCharCount + charsRequired + 1 >= _chars.Length)
                    {
                        char[] dst = new char[remainingCharCount + charsRequired + 1];

                        if (remainingCharCount > 0)
                        {
                            BlockCopyChars(_chars, _charPos, dst, 0, remainingCharCount);
                        }

                        _chars = dst;
                    }
                    else
                    {
                        if (remainingCharCount > 0)
                        {
                            BlockCopyChars(_chars, _charPos, _chars, 0, remainingCharCount);
                        }
                    }

                    _lineStartPos -= _charPos;
                    _charPos = 0;
                    _charsUsed = remainingCharCount;
                }
            }

            int attemptCharReadCount = _chars.Length - _charsUsed - 1;

            int charsRead = _reader.Read(_chars, _charsUsed, attemptCharReadCount);

            _charsUsed += charsRead;

            if (charsRead == 0)
            {
                _isEndOfFile = true;
            }

            _chars[_charsUsed] = '\0';
            return charsRead;
        }

        private bool EnsureChars(int relativePosition, bool append)
        {
            if (_charPos + relativePosition >= _charsUsed)
            {
                return ReadChars(relativePosition, append);
            }

            return true;
        }

        private bool ReadChars(int relativePosition, bool append)
        {
            if (_isEndOfFile)
            {
                return false;
            }

            int charsRequired = _charPos + relativePosition - _charsUsed + 1;

            int totalCharsRead = 0;

            do
            {
                int charsRead = ReadData(append, charsRequired - totalCharsRead);

                if (charsRead == 0)
                {
                    break;
                }

                totalCharsRead += charsRead;
            } while (totalCharsRead < charsRequired);

            if (totalCharsRead < charsRequired)
            {
                return false;
            }
            return true;
        }

        [DebuggerStepThrough]
        public override bool Read()
        {
            _readType = ReadType.Read;
            if (!ReadInternal())
            {
                SetToken(JsonToken.None);
                return false;
            }

            return true;
        }

        public override byte[] ReadAsBytes()
        {
            return ReadAsBytesInternal();
        }

        public override decimal? ReadAsDecimal()
        {
            return ReadAsDecimalInternal();
        }

        public override int? ReadAsInt32()
        {
            return ReadAsInt32Internal();
        }

        public override string ReadAsString()
        {
            return ReadAsStringInternal();
        }

        public override DateTime? ReadAsDateTime()
        {
            return ReadAsDateTimeInternal();
        }

        public override DateTimeOffset? ReadAsDateTimeOffset()
        {
            return ReadAsDateTimeOffsetInternal();
        }

        internal override bool ReadInternal()
        {
            while (true)
            {
                switch (_currentState)
                {
                    case State.Start:
                    case State.Property:
                    case State.Array:
                    case State.ArrayStart:
                    case State.Constructor:
                    case State.ConstructorStart:
                        return ParseValue();
                    case State.Object:
                    case State.ObjectStart:
                        return ParseObject();
                    case State.PostValue:

                        if (ParsePostValue())
                        {
                            return true;
                        }
                        break;
                    case State.Finished:
                        if (EnsureChars(0, false))
                        {
                            EatWhitespace(false);
                            if (_isEndOfFile)
                            {
                                return false;
                            }
                            if (_chars[_charPos] == '/')
                            {
                                ParseComment();
                                return true;
                            }

                            throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
                        }
                        return false;
                    default:
                        throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, CurrentState));
                }
            }
        }

        private void ReadStringIntoBuffer(char quote)
        {
            int charPos = _charPos;
            int initialPosition = _charPos;
            int lastWritePosition = _charPos;
            StringBuffer buffer = null;

            while (true)
            {
                switch (_chars[charPos++])
                {
                    case '\0':
                        if (_charsUsed == charPos - 1)
                        {
                            charPos--;

                            if (ReadData(true) == 0)
                            {
                                _charPos = charPos;
                                throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
                            }
                        }
                        break;
                    case '\\':
                        _charPos = charPos;
                        if (!EnsureChars(0, true))
                        {
                            _charPos = charPos;
                            throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
                        }

                        int escapeStartPos = charPos - 1;

                        char currentChar = _chars[charPos];

                        char writeChar;

                        switch (currentChar)
                        {
                            case 'b':
                                charPos++;
                                writeChar = '\b';
                                break;
                            case 't':
                                charPos++;
                                writeChar = '\t';
                                break;
                            case 'n':
                                charPos++;
                                writeChar = '\n';
                                break;
                            case 'f':
                                charPos++;
                                writeChar = '\f';
                                break;
                            case 'r':
                                charPos++;
                                writeChar = '\r';
                                break;
                            case '\\':
                                charPos++;
                                writeChar = '\\';
                                break;
                            case '"':
                            case '\'':
                            case '/':
                                writeChar = currentChar;
                                charPos++;
                                break;
                            case 'u':
                                charPos++;
                                _charPos = charPos;
                                writeChar = ParseUnicode();

                                if (StringUtils.IsLowSurrogate(writeChar))
                                {
                                    writeChar = UNICODE_REPLACEMENT_CHAR;
                                }
                                else if (StringUtils.IsHighSurrogate(writeChar))
                                {
                                    bool anotherHighSurrogate;

                                    do
                                    {
                                        anotherHighSurrogate = false;

                                        if (EnsureChars(2, true) && _chars[_charPos] == '\\' && _chars[_charPos + 1] == 'u')
                                        {
                                            char highSurrogate = writeChar;

                                            _charPos += 2;
                                            writeChar = ParseUnicode();

                                            if (StringUtils.IsLowSurrogate(writeChar))
                                            {
                                            }
                                            else if (StringUtils.IsHighSurrogate(writeChar))
                                            {
                                                highSurrogate = UNICODE_REPLACEMENT_CHAR;
                                                anotherHighSurrogate = true;
                                            }
                                            else
                                            {
                                                highSurrogate = UNICODE_REPLACEMENT_CHAR;
                                            }

                                            if (buffer == null)
                                            {
                                                buffer = GetBuffer();
                                            }

                                            WriteCharToBuffer(buffer, highSurrogate, lastWritePosition, escapeStartPos);
                                            lastWritePosition = _charPos;
                                        }
                                        else
                                        {
                                            writeChar = UNICODE_REPLACEMENT_CHAR;
                                        }
                                    } while (anotherHighSurrogate);
                                }

                                charPos = _charPos;
                                break;
                            default:
                                charPos++;
                                _charPos = charPos;
                                throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, @"\" + currentChar));
                        }

                        if (buffer == null)
                        {
                            buffer = GetBuffer();
                        }

                        WriteCharToBuffer(buffer, writeChar, lastWritePosition, escapeStartPos);

                        lastWritePosition = charPos;
                        break;
                    case StringUtils.CarriageReturn:
                        _charPos = charPos - 1;
                        ProcessCarriageReturn(true);
                        charPos = _charPos;
                        break;
                    case StringUtils.LineFeed:
                        _charPos = charPos - 1;
                        ProcessLineFeed();
                        charPos = _charPos;
                        break;
                    case '"':
                    case '\'':
                        if (_chars[charPos - 1] == quote)
                        {
                            charPos--;

                            if (initialPosition == lastWritePosition)
                            {
                                _stringReference = new StringReference(_chars, initialPosition, charPos - initialPosition);
                            }
                            else
                            {
                                if (buffer == null)
                                {
                                    buffer = GetBuffer();
                                }

                                if (charPos > lastWritePosition)
                                {
                                    buffer.Append(_chars, lastWritePosition, charPos - lastWritePosition);
                                }

                                _stringReference = new StringReference(buffer.GetInternalBuffer(), 0, buffer.Position);
                            }

                            charPos++;
                            _charPos = charPos;
                            return;
                        }
                        break;
                }
            }
        }

        private void WriteCharToBuffer(StringBuffer buffer, char writeChar, int lastWritePosition, int writeToPosition)
        {
            if (writeToPosition > lastWritePosition)
            {
                buffer.Append(_chars, lastWritePosition, writeToPosition - lastWritePosition);
            }

            buffer.Append(writeChar);
        }

        private char ParseUnicode()
        {
            char writeChar;
            if (EnsureChars(4, true))
            {
                string hexValues = new string(_chars, _charPos, 4);
                char hexChar = Convert.ToChar(int.Parse(hexValues, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
                writeChar = hexChar;

                _charPos += 4;
            }
            else
            {
                throw JsonReaderException.Create(this, "Unexpected end while parsing unicode character.");
            }
            return writeChar;
        }

        private void ReadNumberIntoBuffer()
        {
            int charPos = _charPos;

            while (true)
            {
                switch (_chars[charPos])
                {
                    case '\0':
                        _charPos = charPos;

                        if (_charsUsed == charPos)
                        {
                            if (ReadData(true) == 0)
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case '-':
                    case '+':
                    case 'a':
                    case 'A':
                    case 'b':
                    case 'B':
                    case 'c':
                    case 'C':
                    case 'd':
                    case 'D':
                    case 'e':
                    case 'E':
                    case 'f':
                    case 'F':
                    case 'x':
                    case 'X':
                    case '.':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        charPos++;
                        break;
                    default:
                        _charPos = charPos;

                        char currentChar = _chars[_charPos];
                        if (char.IsWhiteSpace(currentChar)
                            || currentChar == ','
                            || currentChar == '}'
                            || currentChar == ']'
                            || currentChar == ')'
                            || currentChar == '/')
                        {
                            return;
                        }

                        throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
                }
            }
        }

        private void ClearRecentString()
        {
            if (_buffer != null)
            {
                _buffer.Position = 0;
            }

            _stringReference = new StringReference();
        }

        private bool ParsePostValue()
        {
            while (true)
            {
                char currentChar = _chars[_charPos];

                switch (currentChar)
                {
                    case '\0':
                        if (_charsUsed == _charPos)
                        {
                            if (ReadData(false) == 0)
                            {
                                _currentState = State.Finished;
                                return false;
                            }
                        }
                        else
                        {
                            _charPos++;
                        }
                        break;
                    case '}':
                        _charPos++;
                        SetToken(JsonToken.EndObject);
                        return true;
                    case ']':
                        _charPos++;
                        SetToken(JsonToken.EndArray);
                        return true;
                    case ')':
                        _charPos++;
                        SetToken(JsonToken.EndConstructor);
                        return true;
                    case '/':
                        ParseComment();
                        return true;
                    case ',':
                        _charPos++;

                        SetStateBasedOnCurrent();
                        return false;
                    case ' ':
                    case StringUtils.Tab:

                        _charPos++;
                        break;
                    case StringUtils.CarriageReturn:
                        ProcessCarriageReturn(false);
                        break;
                    case StringUtils.LineFeed:
                        ProcessLineFeed();
                        break;
                    default:
                        if (char.IsWhiteSpace(currentChar))
                        {
                            _charPos++;
                        }
                        else
                        {
                            throw JsonReaderException.Create(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
                        }
                        break;
                }
            }
        }

        private bool ParseObject()
        {
            while (true)
            {
                char currentChar = _chars[_charPos];

                switch (currentChar)
                {
                    case '\0':
                        if (_charsUsed == _charPos)
                        {
                            if (ReadData(false) == 0)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            _charPos++;
                        }
                        break;
                    case '}':
                        SetToken(JsonToken.EndObject);
                        _charPos++;
                        return true;
                    case '/':
                        ParseComment();
                        return true;
                    case StringUtils.CarriageReturn:
                        ProcessCarriageReturn(false);
                        break;
                    case StringUtils.LineFeed:
                        ProcessLineFeed();
                        break;
                    case ' ':
                    case StringUtils.Tab:

                        _charPos++;
                        break;
                    default:
                        if (char.IsWhiteSpace(currentChar))
                        {
                            _charPos++;
                        }
                        else
                        {
                            return ParseProperty();
                        }
                        break;
                }
            }
        }

        private bool ParseProperty()
        {
            char firstChar = _chars[_charPos];
            char quoteChar;

            if (firstChar == '"' || firstChar == '\'')
            {
                _charPos++;
                quoteChar = firstChar;
                ShiftBufferIfNeeded();
                ReadStringIntoBuffer(quoteChar);
            }
            else if (ValidIdentifierChar(firstChar))
            {
                quoteChar = '\0';
                ShiftBufferIfNeeded();
                ParseUnquotedProperty();
            }
            else
            {
                throw JsonReaderException.Create(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
            }

            string propertyName;

            if (NameTable != null)
            {
                propertyName = NameTable.Get(_stringReference.Chars, _stringReference.StartIndex, _stringReference.Length) ?? _stringReference.ToString();
            }
            else
            {
                propertyName = _stringReference.ToString();
            }

            EatWhitespace(false);

            if (_chars[_charPos] != ':')
            {
                throw JsonReaderException.Create(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
            }

            _charPos++;

            SetToken(JsonToken.PropertyName, propertyName);
            _quoteChar = quoteChar;
            ClearRecentString();

            return true;
        }

        private bool ValidIdentifierChar(char value)
        {
            return (char.IsLetterOrDigit(value) || value == '_' || value == '$');
        }

        private void ParseUnquotedProperty()
        {
            int initialPosition = _charPos;

            while (true)
            {
                switch (_chars[_charPos])
                {
                    case '\0':
                        if (_charsUsed == _charPos)
                        {
                            if (ReadData(true) == 0)
                            {
                                throw JsonReaderException.Create(this, "Unexpected end while parsing unquoted property name.");
                            }

                            break;
                        }

                        _stringReference = new StringReference(_chars, initialPosition, _charPos - initialPosition);
                        return;
                    default:
                        char currentChar = _chars[_charPos];

                        if (ValidIdentifierChar(currentChar))
                        {
                            _charPos++;
                            break;
                        }
                        if (char.IsWhiteSpace(currentChar) || currentChar == ':')
                        {
                            _stringReference = new StringReference(_chars, initialPosition, _charPos - initialPosition);
                            return;
                        }

                        throw JsonReaderException.Create(this, "Invalid JavaScript property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
                }
            }
        }

        private bool ParseValue()
        {
            while (true)
            {
                char currentChar = _chars[_charPos];

                switch (currentChar)
                {
                    case '\0':
                        if (_charsUsed == _charPos)
                        {
                            if (ReadData(false) == 0)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            _charPos++;
                        }
                        break;
                    case '"':
                    case '\'':
                        ParseString(currentChar);
                        return true;
                    case 't':
                        ParseTrue();
                        return true;
                    case 'f':
                        ParseFalse();
                        return true;
                    case 'n':
                        if (EnsureChars(1, true))
                        {
                            char next = _chars[_charPos + 1];

                            if (next == 'u')
                            {
                                ParseNull();
                            }
                            else if (next == 'e')
                            {
                                ParseConstructor();
                            }
                            else
                            {
                                throw JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
                            }
                        }
                        else
                        {
                            throw JsonReaderException.Create(this, "Unexpected end.");
                        }
                        return true;
                    case 'N':
                        ParseNumberNaN();
                        return true;
                    case 'I':
                        ParseNumberPositiveInfinity();
                        return true;
                    case '-':
                        if (EnsureChars(1, true) && _chars[_charPos + 1] == 'I')
                        {
                            ParseNumberNegativeInfinity();
                        }
                        else
                        {
                            ParseNumber();
                        }
                        return true;
                    case '/':
                        ParseComment();
                        return true;
                    case 'u':
                        ParseUndefined();
                        return true;
                    case '{':
                        _charPos++;
                        SetToken(JsonToken.StartObject);
                        return true;
                    case '[':
                        _charPos++;
                        SetToken(JsonToken.StartArray);
                        return true;
                    case ']':
                        _charPos++;
                        SetToken(JsonToken.EndArray);
                        return true;
                    case ',':

                        SetToken(JsonToken.Undefined);
                        return true;
                    case ')':
                        _charPos++;
                        SetToken(JsonToken.EndConstructor);
                        return true;
                    case StringUtils.CarriageReturn:
                        ProcessCarriageReturn(false);
                        break;
                    case StringUtils.LineFeed:
                        ProcessLineFeed();
                        break;
                    case ' ':
                    case StringUtils.Tab:

                        _charPos++;
                        break;
                    default:
                        if (char.IsWhiteSpace(currentChar))
                        {
                            _charPos++;
                            break;
                        }
                        if (char.IsNumber(currentChar) || currentChar == '-' || currentChar == '.')
                        {
                            ParseNumber();
                            return true;
                        }

                        throw JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
                }
            }
        }

        private void ProcessLineFeed()
        {
            _charPos++;
            OnNewLine(_charPos);
        }

        private void ProcessCarriageReturn(bool append)
        {
            _charPos++;

            if (EnsureChars(1, append) && _chars[_charPos] == StringUtils.LineFeed)
            {
                _charPos++;
            }

            OnNewLine(_charPos);
        }

        private bool EatWhitespace(bool oneOrMore)
        {
            bool finished = false;
            bool ateWhitespace = false;
            while (!finished)
            {
                char currentChar = _chars[_charPos];

                switch (currentChar)
                {
                    case '\0':
                        if (_charsUsed == _charPos)
                        {
                            if (ReadData(false) == 0)
                            {
                                finished = true;
                            }
                        }
                        else
                        {
                            _charPos++;
                        }
                        break;
                    case StringUtils.CarriageReturn:
                        ProcessCarriageReturn(false);
                        break;
                    case StringUtils.LineFeed:
                        ProcessLineFeed();
                        break;
                    default:
                        if (currentChar == ' ' || char.IsWhiteSpace(currentChar))
                        {
                            ateWhitespace = true;
                            _charPos++;
                        }
                        else
                        {
                            finished = true;
                        }
                        break;
                }
            }

            return (!oneOrMore || ateWhitespace);
        }

        private void ParseConstructor()
        {
            if (MatchValueWithTrailingSeparator("new"))
            {
                EatWhitespace(false);

                int initialPosition = _charPos;
                int endPosition;

                while (true)
                {
                    char currentChar = _chars[_charPos];
                    if (currentChar == '\0')
                    {
                        if (_charsUsed == _charPos)
                        {
                            if (ReadData(true) == 0)
                            {
                                throw JsonReaderException.Create(this, "Unexpected end while parsing constructor.");
                            }
                        }
                        else
                        {
                            endPosition = _charPos;
                            _charPos++;
                            break;
                        }
                    }
                    else if (char.IsLetterOrDigit(currentChar))
                    {
                        _charPos++;
                    }
                    else if (currentChar == StringUtils.CarriageReturn)
                    {
                        endPosition = _charPos;
                        ProcessCarriageReturn(true);
                        break;
                    }
                    else if (currentChar == StringUtils.LineFeed)
                    {
                        endPosition = _charPos;
                        ProcessLineFeed();
                        break;
                    }
                    else if (char.IsWhiteSpace(currentChar))
                    {
                        endPosition = _charPos;
                        _charPos++;
                        break;
                    }
                    else if (currentChar == '(')
                    {
                        endPosition = _charPos;
                        break;
                    }
                    else
                    {
                        throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, currentChar));
                    }
                }

                _stringReference = new StringReference(_chars, initialPosition, endPosition - initialPosition);
                string constructorName = _stringReference.ToString();

                EatWhitespace(false);

                if (_chars[_charPos] != '(')
                {
                    throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
                }

                _charPos++;

                ClearRecentString();

                SetToken(JsonToken.StartConstructor, constructorName);
            }
            else
            {
                throw JsonReaderException.Create(this, "Unexpected content while parsing JSON.");
            }
        }

        private void ParseNumber()
        {
            ShiftBufferIfNeeded();

            char firstChar = _chars[_charPos];
            int initialPosition = _charPos;

            ReadNumberIntoBuffer();

            SetPostValueState(true);

            _stringReference = new StringReference(_chars, initialPosition, _charPos - initialPosition);

            object numberValue;
            JsonToken numberType;

            bool singleDigit = (char.IsDigit(firstChar) && _stringReference.Length == 1);
            bool nonBase10 = (firstChar == '0' && _stringReference.Length > 1
                              && _stringReference.Chars[_stringReference.StartIndex + 1] != '.'
                              && _stringReference.Chars[_stringReference.StartIndex + 1] != 'e'
                              && _stringReference.Chars[_stringReference.StartIndex + 1] != 'E');

            if (_readType == ReadType.ReadAsInt32)
            {
                if (singleDigit)
                {
                    numberValue = firstChar - 48;
                }
                else if (nonBase10)
                {
                    string number = _stringReference.ToString();

                    try
                    {
                        int integer = number.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                            ? Convert.ToInt32(number, 16)
                            : Convert.ToInt32(number, 8);

                        numberValue = integer;
                    }
                    catch (Exception ex)
                    {
                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, number), ex);
                    }
                }
                else
                {
                    int value;
                    ParseResult parseResult = ConvertUtils.Int32TryParse(_stringReference.Chars, _stringReference.StartIndex, _stringReference.Length, out value);
                    if (parseResult == ParseResult.Success)
                    {
                        numberValue = value;
                    }
                    else if (parseResult == ParseResult.Overflow)
                    {
                        throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int32.".FormatWith(CultureInfo.InvariantCulture, _stringReference.ToString()));
                    }
                    else
                    {
                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, _stringReference.ToString()));
                    }
                }

                numberType = JsonToken.Integer;
            }
            else if (_readType == ReadType.ReadAsDecimal)
            {
                if (singleDigit)
                {
                    numberValue = (decimal)firstChar - 48;
                }
                else if (nonBase10)
                {
                    string number = _stringReference.ToString();

                    try
                    {
                        long integer = number.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                            ? Convert.ToInt64(number, 16)
                            : Convert.ToInt64(number, 8);

                        numberValue = Convert.ToDecimal(integer);
                    }
                    catch (Exception ex)
                    {
                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, number), ex);
                    }
                }
                else
                {
                    string number = _stringReference.ToString();

                    decimal value;
                    if (decimal.TryParse(number, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out value))
                    {
                        numberValue = value;
                    }
                    else
                    {
                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, _stringReference.ToString()));
                    }
                }

                numberType = JsonToken.Float;
            }
            else
            {
                if (singleDigit)
                {
                    numberValue = (long)firstChar - 48;
                    numberType = JsonToken.Integer;
                }
                else if (nonBase10)
                {
                    string number = _stringReference.ToString();

                    try
                    {
                        numberValue = number.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                            ? Convert.ToInt64(number, 16)
                            : Convert.ToInt64(number, 8);
                    }
                    catch (Exception ex)
                    {
                        throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, number), ex);
                    }

                    numberType = JsonToken.Integer;
                }
                else
                {
                    long value;
                    ParseResult parseResult = ConvertUtils.Int64TryParse(_stringReference.Chars, _stringReference.StartIndex, _stringReference.Length, out value);
                    if (parseResult == ParseResult.Success)
                    {
                        numberValue = value;
                        numberType = JsonToken.Integer;
                    }
                    else if (parseResult == ParseResult.Overflow)
                    {
                        string number = _stringReference.ToString();

                        if (number.Length > MAXIMUM_JAVASCRIPT_INTEGER_CHARACTER_LENGTH)
                        {
                            throw JsonReaderException.Create(this, "JSON integer {0} is too large to parse.".FormatWith(CultureInfo.InvariantCulture, _stringReference.ToString()));
                        }

                        numberValue = BigIntegerParse(number, CultureInfo.InvariantCulture);
                        numberType = JsonToken.Integer;
                    }
                    else
                    {
                        string number = _stringReference.ToString();

                        if (_floatParseHandling == FloatParseHandling.Decimal)
                        {
                            decimal d;
                            if (decimal.TryParse(number, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out d))
                            {
                                numberValue = d;
                            }
                            else
                            {
                                throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, number));
                            }
                        }
                        else
                        {
                            double d;
                            if (double.TryParse(number, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d))
                            {
                                numberValue = d;
                            }
                            else
                            {
                                throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, number));
                            }
                        }

                        numberType = JsonToken.Float;
                    }
                }
            }

            ClearRecentString();

            SetToken(numberType, numberValue, false);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static object BigIntegerParse(string number, CultureInfo culture)
        {
            return System.Numerics.BigInteger.Parse(number, culture);
        }

        private void ParseComment()
        {
            _charPos++;

            if (!EnsureChars(1, false))
            {
                throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
            }

            bool singlelineComment;

            if (_chars[_charPos] == '*')
            {
                singlelineComment = false;
            }
            else if (_chars[_charPos] == '/')
            {
                singlelineComment = true;
            }
            else
            {
                throw JsonReaderException.Create(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, _chars[_charPos]));
            }

            _charPos++;

            int initialPosition = _charPos;

            bool commentFinished = false;

            while (!commentFinished)
            {
                switch (_chars[_charPos])
                {
                    case '\0':
                        if (_charsUsed == _charPos)
                        {
                            if (ReadData(true) == 0)
                            {
                                if (!singlelineComment)
                                {
                                    throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
                                }

                                _stringReference = new StringReference(_chars, initialPosition, _charPos - initialPosition);
                                commentFinished = true;
                            }
                        }
                        else
                        {
                            _charPos++;
                        }
                        break;
                    case '*':
                        _charPos++;

                        if (!singlelineComment)
                        {
                            if (EnsureChars(0, true))
                            {
                                if (_chars[_charPos] == '/')
                                {
                                    _stringReference = new StringReference(_chars, initialPosition, _charPos - initialPosition - 1);

                                    _charPos++;
                                    commentFinished = true;
                                }
                            }
                        }
                        break;
                    case StringUtils.CarriageReturn:
                        if (singlelineComment)
                        {
                            _stringReference = new StringReference(_chars, initialPosition, _charPos - initialPosition);
                            commentFinished = true;
                        }
                        ProcessCarriageReturn(true);
                        break;
                    case StringUtils.LineFeed:
                        if (singlelineComment)
                        {
                            _stringReference = new StringReference(_chars, initialPosition, _charPos - initialPosition);
                            commentFinished = true;
                        }
                        ProcessLineFeed();
                        break;
                    default:
                        _charPos++;
                        break;
                }
            }

            SetToken(JsonToken.Comment, _stringReference.ToString());

            ClearRecentString();
        }

        private bool MatchValue(string value)
        {
            if (!EnsureChars(value.Length - 1, true))
            {
                return false;
            }

            if (value.Where((t, i) => _chars[_charPos + i] != t).Any())
            {
                return false;
            }

            _charPos += value.Length;

            return true;
        }

        private bool MatchValueWithTrailingSeparator(string value)
        {
            bool match = MatchValue(value);

            if (!match)
            {
                return false;
            }

            if (!EnsureChars(0, false))
            {
                return true;
            }

            return IsSeparator(_chars[_charPos]) || _chars[_charPos] == '\0';
        }

        private bool IsSeparator(char c)
        {
            switch (c)
            {
                case '}':
                case ']':
                case ',':
                    return true;
                case '/':

                    if (!EnsureChars(1, false))
                    {
                        return false;
                    }

                    var nextChart = _chars[_charPos + 1];

                    return (nextChart == '*' || nextChart == '/');
                case ')':
                    if (CurrentState == State.Constructor || CurrentState == State.ConstructorStart)
                    {
                        return true;
                    }
                    break;
                case ' ':
                case StringUtils.Tab:
                case StringUtils.LineFeed:
                case StringUtils.CarriageReturn:
                    return true;
                default:
                    if (char.IsWhiteSpace(c))
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        private void ParseTrue()
        {
            if (MatchValueWithTrailingSeparator(Json.True))
            {
                SetToken(JsonToken.Boolean, true);
            }
            else
            {
                throw JsonReaderException.Create(this, "Error parsing boolean value.");
            }
        }

        private void ParseNull()
        {
            if (MatchValueWithTrailingSeparator(Json.Null))
            {
                SetToken(JsonToken.Null);
            }
            else
            {
                throw JsonReaderException.Create(this, "Error parsing null value.");
            }
        }

        private void ParseUndefined()
        {
            if (MatchValueWithTrailingSeparator(Json.Undefined))
            {
                SetToken(JsonToken.Undefined);
            }
            else
            {
                throw JsonReaderException.Create(this, "Error parsing undefined value.");
            }
        }

        private void ParseFalse()
        {
            if (MatchValueWithTrailingSeparator(Json.False))
            {
                SetToken(JsonToken.Boolean, false);
            }
            else
            {
                throw JsonReaderException.Create(this, "Error parsing boolean value.");
            }
        }

        private void ParseNumberNegativeInfinity()
        {
            if (MatchValueWithTrailingSeparator(Json.NegativeInfinity))
            {
                if (_floatParseHandling == FloatParseHandling.Decimal)
                {
                    throw new JsonReaderException("Cannot read -Infinity as a decimal.");
                }

                SetToken(JsonToken.Float, double.NegativeInfinity);
            }
            else
            {
                throw JsonReaderException.Create(this, "Error parsing negative infinity value.");
            }
        }

        private void ParseNumberPositiveInfinity()
        {
            if (MatchValueWithTrailingSeparator(Json.PositiveInfinity))
            {
                if (_floatParseHandling == FloatParseHandling.Decimal)
                {
                    throw new JsonReaderException("Cannot read Infinity as a decimal.");
                }

                SetToken(JsonToken.Float, double.PositiveInfinity);
            }
            else
            {
                throw JsonReaderException.Create(this, "Error parsing positive infinity value.");
            }
        }

        private void ParseNumberNaN()
        {
            if (MatchValueWithTrailingSeparator(Json.NaN))
            {
                if (_floatParseHandling == FloatParseHandling.Decimal)
                {
                    throw new JsonReaderException("Cannot read NaN as a decimal.");
                }

                SetToken(JsonToken.Float, double.NaN);
            }
            else
            {
                throw JsonReaderException.Create(this, "Error parsing NaN value.");
            }
        }

        public override void Close()
        {
            base.Close();

            if (CloseInput && _reader != null)
            {
                _reader.Close();
            }
            if (_buffer != null)
            {
                _buffer.Clear();
            }
        }

        public bool HasLineInfo()
        {
            return true;
        }

        #endregion
    }
}