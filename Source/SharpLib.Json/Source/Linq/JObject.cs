using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace SharpLib.Json.Linq
{
    public sealed class JObject : JContainer, IDictionary<string, JToken>, INotifyPropertyChanged, ICustomTypeDescriptor, INotifyPropertyChanging
    {
        #region Поля

        private readonly JPropertyKeyedCollection _properties = new JPropertyKeyedCollection();

        #endregion

        #region Свойства

        protected override IList<JToken> ChildrenTokens
        {
            get { return _properties; }
        }

        public override JTokenType Type
        {
            get { return JTokenType.Object; }
        }

        public override JToken this[object key]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(key, "o");

                string propertyName = key as string;
                if (propertyName == null)
                {
                    throw new ArgumentException("Accessed JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture,
                        MiscellaneousUtils.ToString(key)));
                }

                return this[propertyName];
            }
            set
            {
                ValidationUtils.ArgumentNotNull(key, "o");

                string propertyName = key as string;
                if (propertyName == null)
                {
                    throw new ArgumentException("Set JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture,
                        MiscellaneousUtils.ToString(key)));
                }

                this[propertyName] = value;
            }
        }

        public JToken this[string propertyName]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(propertyName, "propertyName");

                JProperty property = Property(propertyName);

                return (property != null) ? property.Value : null;
            }
            set
            {
                JProperty property = Property(propertyName);
                if (property != null)
                {
                    property.Value = value;
                }
                else
                {
                    OnPropertyChanging(propertyName);
                    Add(new JProperty(propertyName, value));
                    OnPropertyChanged(propertyName);
                }
            }
        }

        ICollection<string> IDictionary<string, JToken>.Keys
        {
            get { return _properties.Keys; }
        }

        ICollection<JToken> IDictionary<string, JToken>.Values
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<KeyValuePair<string, JToken>>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region События

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Конструктор

        public JObject()
        {
        }

        public JObject(JObject other)
            : base(other)
        {
        }

        public JObject(params object[] content)
            : this((object)content)
        {
        }

        public JObject(object content)
        {
            Add(content);
        }

        #endregion

        #region Методы

        internal override bool DeepEquals(JToken node)
        {
            JObject t = node as JObject;
            if (t == null)
            {
                return false;
            }

            return _properties.Compare(t._properties);
        }

        internal override void InsertItem(int index, JToken item, bool skipParentCheck)
        {
            if (item != null && item.Type == JTokenType.Comment)
            {
                return;
            }

            base.InsertItem(index, item, skipParentCheck);
        }

        internal override void ValidateToken(JToken o, JToken existing)
        {
            ValidationUtils.ArgumentNotNull(o, "o");

            if (o.Type != JTokenType.Property)
            {
                throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), GetType()));
            }

            JProperty newProperty = (JProperty)o;

            if (existing != null)
            {
                JProperty existingProperty = (JProperty)existing;

                if (newProperty.Name == existingProperty.Name)
                {
                    return;
                }
            }

            if (_properties.TryGetValue(newProperty.Name, out existing))
            {
                throw new ArgumentException("Can not add property {0} to {1}. Property with the same name already exists on object.".FormatWith(CultureInfo.InvariantCulture, newProperty.Name,
                    GetType()));
            }
        }

        internal override void MergeItem(object content, JsonMergeSettings settings)
        {
            JObject o = content as JObject;
            if (o == null)
            {
                return;
            }

            foreach (KeyValuePair<string, JToken> contentItem in o)
            {
                JProperty existingProperty = Property(contentItem.Key);

                if (existingProperty == null)
                {
                    Add(contentItem.Key, contentItem.Value);
                }
                else if (contentItem.Value != null)
                {
                    JContainer existingContainer = existingProperty.Value as JContainer;
                    if (existingContainer == null)
                    {
                        if (contentItem.Value.Type != JTokenType.Null)
                        {
                            existingProperty.Value = contentItem.Value;
                        }
                    }
                    else if (existingContainer.Type != contentItem.Value.Type)
                    {
                        existingProperty.Value = contentItem.Value;
                    }
                    else
                    {
                        existingContainer.Merge(contentItem.Value, settings);
                    }
                }
            }
        }

        internal void InternalPropertyChanged(JProperty childProperty)
        {
            OnPropertyChanged(childProperty.Name);
            if (_listChanged != null)
            {
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, IndexOfItem(childProperty)));
            }
            if (_collectionChanged != null)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, childProperty, childProperty, IndexOfItem(childProperty)));
            }
        }

        internal void InternalPropertyChanging(JProperty childProperty)
        {
            OnPropertyChanging(childProperty.Name);
        }

        internal override JToken CloneToken()
        {
            return new JObject(this);
        }

        public IEnumerable<JProperty> Properties()
        {
            return _properties.Cast<JProperty>();
        }

        public JProperty Property(string name)
        {
            if (name == null)
            {
                return null;
            }

            JToken property;
            _properties.TryGetValue(name, out property);
            return (JProperty)property;
        }

        public JEnumerable<JToken> PropertyValues()
        {
            return new JEnumerable<JToken>(Properties().Select(p => p.Value));
        }

        public new static JObject Load(JsonReader reader)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");

            if (reader.TokenType == JsonToken.None)
            {
                if (!reader.Read())
                {
                    throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader.");
                }
            }

            while (reader.TokenType == JsonToken.Comment)
            {
                reader.Read();
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw JsonReaderException.Create(reader,
                    "Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            JObject o = new JObject();
            o.SetLineInfo(reader as IJsonLineInfo);

            o.ReadTokenFrom(reader);

            return o;
        }

        public new static JObject Parse(string json)
        {
            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
            {
                JObject o = Load(reader);

                if (reader.Read() && reader.TokenType != JsonToken.Comment)
                {
                    throw JsonReaderException.Create(reader, "Additional text found in JSON string after parsing content.");
                }

                return o;
            }
        }

        public new static JObject FromObject(object o)
        {
            return FromObject(o, JsonSerializer.CreateDefault());
        }

        public new static JObject FromObject(object o, JsonSerializer jsonSerializer)
        {
            JToken token = FromObjectInternal(o, jsonSerializer);

            if (token != null && token.Type != JTokenType.Object)
            {
                throw new ArgumentException("Object serialized to {0}. JObject instance expected.".FormatWith(CultureInfo.InvariantCulture, token.Type));
            }

            return (JObject)token;
        }

        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WriteStartObject();

            foreach (JToken t in _properties)
            {
                t.WriteTo(writer, converters);
            }

            writer.WriteEndObject();
        }

        public JToken GetValue(string propertyName)
        {
            return GetValue(propertyName, StringComparison.Ordinal);
        }

        public JToken GetValue(string propertyName, StringComparison comparison)
        {
            if (propertyName == null)
            {
                return null;
            }

            JProperty property = Property(propertyName);
            if (property != null)
            {
                return property.Value;
            }

            if (comparison != StringComparison.Ordinal)
            {
                return (from JProperty p in _properties
                        where string.Equals(p.Name, propertyName, comparison)
                        select p.Value).FirstOrDefault();
            }

            return null;
        }

        public bool TryGetValue(string propertyName, StringComparison comparison, out JToken value)
        {
            value = GetValue(propertyName, comparison);
            return (value != null);
        }

        public void Add(string propertyName, JToken value)
        {
            Add(new JProperty(propertyName, value));
        }

        bool IDictionary<string, JToken>.ContainsKey(string key)
        {
            return _properties.Contains(key);
        }

        public bool Remove(string propertyName)
        {
            JProperty property = Property(propertyName);
            if (property == null)
            {
                return false;
            }

            property.Remove();
            return true;
        }

        public bool TryGetValue(string propertyName, out JToken value)
        {
            JProperty property = Property(propertyName);
            if (property == null)
            {
                value = null;
                return false;
            }

            value = property.Value;
            return true;
        }

        void ICollection<KeyValuePair<string, JToken>>.Add(KeyValuePair<string, JToken> item)
        {
            Add(new JProperty(item.Key, item.Value));
        }

        void ICollection<KeyValuePair<string, JToken>>.Clear()
        {
            RemoveAll();
        }

        bool ICollection<KeyValuePair<string, JToken>>.Contains(KeyValuePair<string, JToken> item)
        {
            JProperty property = Property(item.Key);
            if (property == null)
            {
                return false;
            }

            return (property.Value == item.Value);
        }

        void ICollection<KeyValuePair<string, JToken>>.CopyTo(KeyValuePair<string, JToken>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
            }
            if (arrayIndex >= array.Length && arrayIndex != 0)
            {
                throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
            }
            if (Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
            }

            int index = 0;
            foreach (var jToken in _properties)
            {
                var property = (JProperty)jToken;
                array[arrayIndex + index] = new KeyValuePair<string, JToken>(property.Name, property.Value);
                index++;
            }
        }

        bool ICollection<KeyValuePair<string, JToken>>.Remove(KeyValuePair<string, JToken> item)
        {
            if (!((ICollection<KeyValuePair<string, JToken>>)this).Contains(item))
            {
                return false;
            }

            Remove(item.Key);
            return true;
        }

        internal override int GetDeepHashCode()
        {
            return ContentsHashCode();
        }

        public IEnumerator<KeyValuePair<string, JToken>> GetEnumerator()
        {
            return (from JProperty property in _properties
                    select new KeyValuePair<string, JToken>(property.Name, property.Value)).GetEnumerator();
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection descriptors = new PropertyDescriptorCollection(null);

            foreach (KeyValuePair<string, JToken> propertyValue in this)
            {
                descriptors.Add(new JPropertyDescriptor(propertyValue.Key));
            }

            return descriptors;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return AttributeCollection.Empty;
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return null;
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return new TypeConverter();
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return null;
        }

        protected override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicProxyMetaObject<JObject>(parameter, this, new JObjectDynamicProxy(), true);
        }

        #endregion

        #region Вложенный класс: JObjectDynamicProxy

        private class JObjectDynamicProxy : DynamicProxy<JObject>
        {
            #region Методы

            public override bool TryGetMember(JObject instance, GetMemberBinder binder, out object result)
            {
                result = instance[binder.Name];
                return true;
            }

            public override bool TrySetMember(JObject instance, SetMemberBinder binder, object value)
            {
                JToken v = value as JToken ?? new JValue(value);

                instance[binder.Name] = v;
                return true;
            }

            public override IEnumerable<string> GetDynamicMemberNames(JObject instance)
            {
                return instance.Properties().Select(p => p.Name);
            }

            #endregion
        }

        #endregion
    }
}