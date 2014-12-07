using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SharpLib.Json.Linq
{
    public sealed class JArray : JContainer, IList<JToken>
    {
        #region Поля

        private readonly List<JToken> _values = new List<JToken>();

        #endregion

        #region Свойства

        protected override IList<JToken> ChildrenTokens
        {
            get { return _values; }
        }

        public override JTokenType Type
        {
            get { return JTokenType.Array; }
        }

        public override JToken this[object key]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(key, "o");

                if (!(key is int))
                {
                    throw new ArgumentException("Accessed JArray values with invalid key value: {0}. Array position index expected.".FormatWith(CultureInfo.InvariantCulture,
                        MiscellaneousUtils.ToString(key)));
                }

                return GetItem((int)key);
            }
            set
            {
                ValidationUtils.ArgumentNotNull(key, "o");

                if (!(key is int))
                {
                    throw new ArgumentException("Set JArray values with invalid key value: {0}. Array position index expected.".FormatWith(CultureInfo.InvariantCulture,
                        MiscellaneousUtils.ToString(key)));
                }

                SetItem((int)key, value);
            }
        }

        public JToken this[int index]
        {
            get { return GetItem(index); }
            set { SetItem(index, value); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Конструктор

        public JArray()
        {
        }

        public JArray(JArray other)
            : base(other)
        {
        }

        public JArray(params object[] content)
            : this((object)content)
        {
        }

        public JArray(object content)
        {
            Add(content);
        }

        #endregion

        #region Методы

        internal override bool DeepEquals(JToken node)
        {
            JArray t = node as JArray;
            return (t != null && ContentsEqual(t));
        }

        internal override JToken CloneToken()
        {
            return new JArray(this);
        }

        public new static JArray Load(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.None)
            {
                if (!reader.Read())
                {
                    throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader.");
                }
            }

            while (reader.TokenType == JsonToken.Comment)
            {
                reader.Read();
            }

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader. Current JsonReader item is not an array: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            JArray a = new JArray();
            a.SetLineInfo(reader as IJsonLineInfo);

            a.ReadTokenFrom(reader);

            return a;
        }

        public new static JArray Parse(string json)
        {
            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
            {
                JArray a = Load(reader);

                if (reader.Read() && reader.TokenType != JsonToken.Comment)
                {
                    throw JsonReaderException.Create(reader, "Additional text found in JSON string after parsing content.");
                }

                return a;
            }
        }

        public new static JArray FromObject(object o)
        {
            return FromObject(o, JsonSerializer.CreateDefault());
        }

        public new static JArray FromObject(object o, JsonSerializer jsonSerializer)
        {
            JToken token = FromObjectInternal(o, jsonSerializer);

            if (token.Type != JTokenType.Array)
            {
                throw new ArgumentException("Object serialized to {0}. JArray instance expected.".FormatWith(CultureInfo.InvariantCulture, token.Type));
            }

            return (JArray)token;
        }

        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WriteStartArray();

            foreach (JToken t in _values)
            {
                t.WriteTo(writer, converters);
            }

            writer.WriteEndArray();
        }

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            IEnumerable a = (IsMultiContent(content) || content is JArray)
                ? (IEnumerable)content
                : null;
            if (a == null)
            {
                return;
            }

            MergeEnumerableContent(this, a, settings);
        }

        public int IndexOf(JToken item)
        {
            return IndexOfItem(item);
        }

        public void Insert(int index, JToken item)
        {
            InsertItem(index, item, false);
        }

        public void RemoveAt(int index)
        {
            RemoveItemAt(index);
        }

        public IEnumerator<JToken> GetEnumerator()
        {
            return Children().GetEnumerator();
        }

        public void Add(JToken item)
        {
            Add((object)item);
        }

        public void Clear()
        {
            ClearItems();
        }

        public bool Contains(JToken item)
        {
            return ContainsItem(item);
        }

        public void CopyTo(JToken[] array, int arrayIndex)
        {
            CopyItemsTo(array, arrayIndex);
        }

        public bool Remove(JToken item)
        {
            return RemoveItem(item);
        }

        internal override int GetDeepHashCode()
        {
            return ContentsHashCode();
        }

        #endregion
    }
}