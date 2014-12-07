using System;
using System.Collections.Generic;
using System.Globalization;

namespace SharpLib.Json.Linq
{
    public sealed class JConstructor : JContainer
    {
        #region Поля

        private readonly List<JToken> _values = new List<JToken>();

        private string _name;

        #endregion

        #region Свойства

        protected override IList<JToken> ChildrenTokens
        {
            get { return _values; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override JTokenType Type
        {
            get { return JTokenType.Constructor; }
        }

        public override JToken this[object key]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(key, "o");

                if (!(key is int))
                {
                    throw new ArgumentException("Accessed JConstructor values with invalid key value: {0}. Argument position index expected.".FormatWith(CultureInfo.InvariantCulture,
                        MiscellaneousUtils.ToString(key)));
                }

                return GetItem((int)key);
            }
            set
            {
                ValidationUtils.ArgumentNotNull(key, "o");

                if (!(key is int))
                {
                    throw new ArgumentException("Set JConstructor values with invalid key value: {0}. Argument position index expected.".FormatWith(CultureInfo.InvariantCulture,
                        MiscellaneousUtils.ToString(key)));
                }

                SetItem((int)key, value);
            }
        }

        #endregion

        #region Конструктор

        public JConstructor()
        {
        }

        public JConstructor(JConstructor other)
            : base(other)
        {
            _name = other.Name;
        }

        public JConstructor(string name, params object[] content)
            : this(name, (object)content)
        {
        }

        public JConstructor(string name, object content)
            : this(name)
        {
            Add(content);
        }

        public JConstructor(string name)
        {
            ValidationUtils.ArgumentNotNullOrEmpty(name, "name");

            _name = name;
        }

        #endregion

        #region Методы

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            JConstructor c = content as JConstructor;
            if (c == null)
            {
                return;
            }

            if (c.Name != null)
            {
                Name = c.Name;
            }
            MergeEnumerableContent(this, c, settings);
        }

        internal override bool DeepEquals(JToken node)
        {
            JConstructor c = node as JConstructor;
            return (c != null && _name == c.Name && ContentsEqual(c));
        }

        internal override JToken CloneToken()
        {
            return new JConstructor(this);
        }

        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WriteStartConstructor(_name);

            foreach (JToken token in Children())
            {
                token.WriteTo(writer, converters);
            }

            writer.WriteEndConstructor();
        }

        internal override int GetDeepHashCode()
        {
            return _name.GetHashCode() ^ ContentsHashCode();
        }

        public new static JConstructor Load(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.None)
            {
                if (!reader.Read())
                {
                    throw JsonReaderException.Create(reader, "Error reading JConstructor from JsonReader.");
                }
            }

            while (reader.TokenType == JsonToken.Comment)
            {
                reader.Read();
            }

            if (reader.TokenType != JsonToken.StartConstructor)
            {
                throw JsonReaderException.Create(reader,
                    "Error reading JConstructor from JsonReader. Current JsonReader item is not a constructor: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            JConstructor c = new JConstructor((string)reader.Value);
            c.SetLineInfo(reader as IJsonLineInfo);

            c.ReadTokenFrom(reader);

            return c;
        }

        #endregion
    }
}