using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace SharpLib.Json.Linq
{
    public class JProperty : JContainer
    {
        #region Поля

        private readonly JPropertyList _content = new JPropertyList();

        private readonly string _name;

        #endregion

        #region Свойства

        protected override IList<JToken> ChildrenTokens
        {
            get { return _content; }
        }

        public string Name
        {
            [DebuggerStepThrough]
            get
            {
                return _name;
            }
        }

        public JToken Value
        {
            [DebuggerStepThrough]
            get
            {
                return _content._token;
            }
            set
            {
                CheckReentrancy();

                JToken newValue = value ?? JValue.CreateNull();

                if (_content._token == null)
                {
                    InsertItem(0, newValue, false);
                }
                else
                {
                    SetItem(0, newValue);
                }
            }
        }

        public override JTokenType Type
        {
            [DebuggerStepThrough]
            get
            {
                return JTokenType.Property;
            }
        }

        #endregion

        #region Конструктор

        public JProperty(JProperty other)
            : base(other)
        {
            _name = other.Name;
        }

        internal JProperty(string name)
        {
            ValidationUtils.ArgumentNotNull(name, "name");

            _name = name;
        }

        public JProperty(string name, params object[] content)
            : this(name, (object)content)
        {
        }

        public JProperty(string name, object content)
        {
            ValidationUtils.ArgumentNotNull(name, "name");

            _name = name;

            Value = IsMultiContent(content)
                ? new JArray(content)
                : CreateFromContent(content);
        }

        #endregion

        #region Методы

        internal override JToken GetItem(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return Value;
        }

        internal override void SetItem(int index, JToken item)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (IsTokenUnchanged(Value, item))
            {
                return;
            }

            if (Parent != null)
            {
                ((JObject)Parent).InternalPropertyChanging(this);
            }

            base.SetItem(0, item);

            if (Parent != null)
            {
                ((JObject)Parent).InternalPropertyChanged(this);
            }
        }

        internal override bool RemoveItem(JToken item)
        {
            throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
        }

        internal override void RemoveItemAt(int index)
        {
            throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
        }

        internal override void InsertItem(int index, JToken item, bool skipParentCheck)
        {
            if (item != null && item.Type == JTokenType.Comment)
            {
                return;
            }

            if (Value != null)
            {
                throw new JsonException("{0} cannot have multiple values.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
            }

            base.InsertItem(0, item, false);
        }

        internal override bool ContainsItem(JToken item)
        {
            return (Value == item);
        }

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            JProperty p = content as JProperty;
            if (p == null)
            {
                return;
            }

            if (p.Value != null && p.Value.Type != JTokenType.Null)
            {
                Value = p.Value;
            }
        }

        internal override void ClearItems()
        {
            throw new JsonException("Cannot add or remove items from {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JProperty)));
        }

        internal override bool DeepEquals(JToken node)
        {
            JProperty t = node as JProperty;
            return (t != null && _name == t.Name && ContentsEqual(t));
        }

        internal override JToken CloneToken()
        {
            return new JProperty(this);
        }

        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WritePropertyName(_name);

            JToken value = Value;
            if (value != null)
            {
                value.WriteTo(writer, converters);
            }
            else
            {
                writer.WriteNull();
            }
        }

        internal override int GetDeepHashCode()
        {
            return _name.GetHashCode() ^ ((Value != null) ? Value.GetDeepHashCode() : 0);
        }

        public new static JProperty Load(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.None)
            {
                if (!reader.Read())
                {
                    throw JsonReaderException.Create(reader, "Error reading JProperty from JsonReader.");
                }
            }

            while (reader.TokenType == JsonToken.Comment)
            {
                reader.Read();
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                throw JsonReaderException.Create(reader,
                    "Error reading JProperty from JsonReader. Current JsonReader item is not a property: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            JProperty p = new JProperty((string)reader.Value);
            p.SetLineInfo(reader as IJsonLineInfo);

            p.ReadTokenFrom(reader);

            return p;
        }

        #endregion

        #region Вложенный класс: JPropertyList

        private class JPropertyList : IList<JToken>
        {
            #region Поля

            internal JToken _token;

            #endregion

            #region Свойства

            public int Count
            {
                get { return (_token != null) ? 1 : 0; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public JToken this[int index]
            {
                get { return (index == 0) ? _token : null; }
                set
                {
                    if (index == 0)
                    {
                        _token = value;
                    }
                }
            }

            #endregion

            #region Методы

            public IEnumerator<JToken> GetEnumerator()
            {
                if (_token != null)
                {
                    yield return _token;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(JToken item)
            {
                _token = item;
            }

            public void Clear()
            {
                _token = null;
            }

            public bool Contains(JToken item)
            {
                return (_token == item);
            }

            public void CopyTo(JToken[] array, int arrayIndex)
            {
                if (_token != null)
                {
                    array[arrayIndex] = _token;
                }
            }

            public bool Remove(JToken item)
            {
                if (_token == item)
                {
                    _token = null;
                    return true;
                }
                return false;
            }

            public int IndexOf(JToken item)
            {
                return (_token == item) ? 0 : -1;
            }

            public void Insert(int index, JToken item)
            {
                if (index == 0)
                {
                    _token = item;
                }
            }

            public void RemoveAt(int index)
            {
                if (index == 0)
                {
                    _token = null;
                }
            }

            #endregion
        }

        #endregion
    }
}