using System;

namespace SharpLib.Json.Linq
{
    public class JTokenReader : JsonReader, IJsonLineInfo
    {
        private readonly string _initialPath;

        private readonly JToken _root;

        private JToken _parent;

        internal JToken _current;

        public JTokenReader(JToken token)
        {
            ValidationUtils.ArgumentNotNull(token, "token");

            _root = token;
            _current = token;
        }

        internal JTokenReader(JToken token, string initialPath)
            : this(token)
        {
            _initialPath = initialPath;
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

#if !NET20

        public override DateTimeOffset? ReadAsDateTimeOffset()
        {
            return ReadAsDateTimeOffsetInternal();
        }
#endif

        internal override bool ReadInternal()
        {
            if (CurrentState != State.Start)
            {
                JContainer container = _current as JContainer;
                if (container != null && _parent != container)
                {
                    return ReadInto(container);
                }
                return ReadOver(_current);
            }

            SetToken(_current);
            return true;
        }

        public override bool Read()
        {
            _readType = ReadType.Read;

            return ReadInternal();
        }

        private bool ReadOver(JToken t)
        {
            if (t == _root)
            {
                return ReadToEnd();
            }

            JToken next = t.Next;
            if ((next == null || next == t) || t == t.Parent.Last)
            {
                if (t.Parent == null)
                {
                    return ReadToEnd();
                }

                return SetEnd(t.Parent);
            }
            _current = next;
            SetToken(_current);
            return true;
        }

        private bool ReadToEnd()
        {
            SetToken(JsonToken.None);
            return false;
        }

        private bool IsEndElement
        {
            get { return (_current == _parent); }
        }

        private JsonToken? GetEndToken(JContainer c)
        {
            switch (c.Type)
            {
                case JTokenType.Object:
                    return JsonToken.EndObject;
                case JTokenType.Array:
                    return JsonToken.EndArray;
                case JTokenType.Constructor:
                    return JsonToken.EndConstructor;
                case JTokenType.Property:
                    return null;
                default:
                    throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", c.Type, "Unexpected JContainer type.");
            }
        }

        private bool ReadInto(JContainer c)
        {
            JToken firstChild = c.First;
            if (firstChild == null)
            {
                return SetEnd(c);
            }
            SetToken(firstChild);
            _current = firstChild;
            _parent = c;
            return true;
        }

        private bool SetEnd(JContainer c)
        {
            JsonToken? endToken = GetEndToken(c);
            if (endToken != null)
            {
                SetToken(endToken.Value);
                _current = c;
                _parent = c;
                return true;
            }
            return ReadOver(c);
        }

        private void SetToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    SetToken(JsonToken.StartObject);
                    break;
                case JTokenType.Array:
                    SetToken(JsonToken.StartArray);
                    break;
                case JTokenType.Constructor:
                    SetToken(JsonToken.StartConstructor, ((JConstructor)token).Name);
                    break;
                case JTokenType.Property:
                    SetToken(JsonToken.PropertyName, ((JProperty)token).Name);
                    break;
                case JTokenType.Comment:
                    SetToken(JsonToken.Comment, ((JValue)token).Value);
                    break;
                case JTokenType.Integer:
                    SetToken(JsonToken.Integer, ((JValue)token).Value);
                    break;
                case JTokenType.Float:
                    SetToken(JsonToken.Float, ((JValue)token).Value);
                    break;
                case JTokenType.String:
                    SetToken(JsonToken.String, ((JValue)token).Value);
                    break;
                case JTokenType.Boolean:
                    SetToken(JsonToken.Boolean, ((JValue)token).Value);
                    break;
                case JTokenType.Null:
                    SetToken(JsonToken.Null, ((JValue)token).Value);
                    break;
                case JTokenType.Undefined:
                    SetToken(JsonToken.Undefined, ((JValue)token).Value);
                    break;
                case JTokenType.Date:
                    SetToken(JsonToken.Date, ((JValue)token).Value);
                    break;
                case JTokenType.Raw:
                    SetToken(JsonToken.Raw, ((JValue)token).Value);
                    break;
                case JTokenType.Bytes:
                    SetToken(JsonToken.Bytes, ((JValue)token).Value);
                    break;
                case JTokenType.Guid:
                    SetToken(JsonToken.String, SafeToString(((JValue)token).Value));
                    break;
                case JTokenType.Uri:
                    SetToken(JsonToken.String, SafeToString(((JValue)token).Value));
                    break;
                case JTokenType.TimeSpan:
                    SetToken(JsonToken.String, SafeToString(((JValue)token).Value));
                    break;
                default:
                    throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", token.Type, "Unexpected JTokenType.");
            }
        }

        private string SafeToString(object value)
        {
            return (value != null) ? value.ToString() : null;
        }

        bool IJsonLineInfo.HasLineInfo()
        {
            if (CurrentState == State.Start)
            {
                return false;
            }

            IJsonLineInfo info = IsEndElement ? null : _current;
            return (info != null && info.HasLineInfo());
        }

        int IJsonLineInfo.LineNumber
        {
            get
            {
                if (CurrentState == State.Start)
                {
                    return 0;
                }

                IJsonLineInfo info = IsEndElement ? null : _current;
                if (info != null)
                {
                    return info.LineNumber;
                }

                return 0;
            }
        }

        int IJsonLineInfo.LinePosition
        {
            get
            {
                if (CurrentState == State.Start)
                {
                    return 0;
                }

                IJsonLineInfo info = IsEndElement ? null : _current;
                if (info != null)
                {
                    return info.LinePosition;
                }

                return 0;
            }
        }

        public override string Path
        {
            get
            {
                string path = base.Path;

                if (!string.IsNullOrEmpty(_initialPath))
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        return _initialPath;
                    }

                    if (_initialPath.EndsWith(']')
                        || path.StartsWith('['))
                    {
                        path = _initialPath + path;
                    }
                    else
                    {
                        path = _initialPath + "." + path;
                    }
                }

                return path;
            }
        }
    }
}