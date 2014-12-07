using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace SharpLib.Json.Linq
{
    public abstract class JContainer : JToken, IList<JToken>, ITypedList, IBindingList, INotifyCollectionChanged
    {
        #region Поля

        internal AddingNewEventHandler _addingNew;

        private bool _busy;

        internal NotifyCollectionChangedEventHandler _collectionChanged;

        internal ListChangedEventHandler _listChanged;

        private object _syncRoot;

        #endregion

        #region Свойства

        protected abstract IList<JToken> ChildrenTokens { get; }

        public override bool HasValues
        {
            get { return ChildrenTokens.Count > 0; }
        }

        public override JToken First
        {
            get { return ChildrenTokens.FirstOrDefault(); }
        }

        public override JToken Last
        {
            get { return ChildrenTokens.LastOrDefault(); }
        }

        JToken IList<JToken>.this[int index]
        {
            get { return GetItem(index); }
            set { SetItem(index, value); }
        }

        bool ICollection<JToken>.IsReadOnly
        {
            get { return false; }
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        object IList.this[int index]
        {
            get { return GetItem(index); }
            set { SetItem(index, EnsureValue(value)); }
        }

        public int Count
        {
            get { return ChildrenTokens.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        bool IBindingList.AllowEdit
        {
            get { return true; }
        }

        bool IBindingList.AllowNew
        {
            get { return true; }
        }

        bool IBindingList.AllowRemove
        {
            get { return true; }
        }

        bool IBindingList.IsSorted
        {
            get { return false; }
        }

        ListSortDirection IBindingList.SortDirection
        {
            get { return ListSortDirection.Ascending; }
        }

        PropertyDescriptor IBindingList.SortProperty
        {
            get { return null; }
        }

        bool IBindingList.SupportsChangeNotification
        {
            get { return true; }
        }

        bool IBindingList.SupportsSearching
        {
            get { return false; }
        }

        bool IBindingList.SupportsSorting
        {
            get { return false; }
        }

        #endregion

        #region События

        public event AddingNewEventHandler AddingNew
        {
            add { _addingNew += value; }
            remove { _addingNew -= value; }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _collectionChanged += value; }
            remove { _collectionChanged -= value; }
        }

        public event ListChangedEventHandler ListChanged
        {
            add { _listChanged += value; }
            remove { _listChanged -= value; }
        }

        #endregion

        #region Конструктор

        internal JContainer()
        {
        }

        internal JContainer(IEnumerable<JToken> other)
            : this()
        {
            ValidationUtils.ArgumentNotNull(other, "c");

            int i = 0;
            foreach (JToken child in other)
            {
                AddInternal(i, child, false);
                i++;
            }
        }

        #endregion

        #region Методы

        internal void CheckReentrancy()
        {
            if (_busy)
            {
                throw new InvalidOperationException("Cannot change {0} during a collection change event.".FormatWith(CultureInfo.InvariantCulture, GetType()));
            }
        }

        internal virtual IList<JToken> CreateChildrenCollection()
        {
            return new List<JToken>();
        }

        protected virtual void OnAddingNew(AddingNewEventArgs e)
        {
            AddingNewEventHandler handler = _addingNew;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            ListChangedEventHandler handler = _listChanged;

            if (handler != null)
            {
                _busy = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    _busy = false;
                }
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = _collectionChanged;

            if (handler != null)
            {
                _busy = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    _busy = false;
                }
            }
        }

        internal bool ContentsEqual(JContainer container)
        {
            if (container == this)
            {
                return true;
            }

            IList<JToken> t1 = ChildrenTokens;
            IList<JToken> t2 = container.ChildrenTokens;

            if (t1.Count != t2.Count)
            {
                return false;
            }

            return !t1.Where((t, i) => !t.DeepEquals(t2[i])).Any();
        }

        public override JEnumerable<JToken> Children()
        {
            return new JEnumerable<JToken>(ChildrenTokens);
        }

        public override IEnumerable<T> Values<T>()
        {
            return ChildrenTokens.Convert<JToken, T>();
        }

        public IEnumerable<JToken> Descendants()
        {
            foreach (JToken o in ChildrenTokens)
            {
                yield return o;
                JContainer c = o as JContainer;
                if (c != null)
                {
                    foreach (JToken d in c.Descendants())
                    {
                        yield return d;
                    }
                }
            }
        }

        internal bool IsMultiContent(object content)
        {
            return (content is IEnumerable && !(content is string) && !(content is JToken) && !(content is byte[]));
        }

        internal JToken EnsureParentToken(JToken item, bool skipParentCheck)
        {
            if (item == null)
            {
                return JValue.CreateNull();
            }

            if (skipParentCheck)
            {
                return item;
            }

            if (item.Parent != null || item == this || (item.HasValues && Root == item))
            {
                item = item.CloneToken();
            }

            return item;
        }

        internal int IndexOfItem(JToken item)
        {
            return ChildrenTokens.IndexOf(item, JTokenReferenceEqualityComparer.Instance);
        }

        internal virtual void InsertItem(int index, JToken item, bool skipParentCheck)
        {
            if (index > ChildrenTokens.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index must be within the bounds of the List.");
            }

            CheckReentrancy();

            item = EnsureParentToken(item, skipParentCheck);

            JToken previous = (index == 0) ? null : ChildrenTokens[index - 1];

            JToken next = (index == ChildrenTokens.Count) ? null : ChildrenTokens[index];

            ValidateToken(item, null);

            item.Parent = this;

            item.Previous = previous;
            if (previous != null)
            {
                previous.Next = item;
            }

            item.Next = next;
            if (next != null)
            {
                next.Previous = item;
            }

            ChildrenTokens.Insert(index, item);

            if (_listChanged != null)
            {
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
            }
            if (_collectionChanged != null)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        }

        internal virtual void RemoveItemAt(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
            }
            if (index >= ChildrenTokens.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
            }

            CheckReentrancy();

            JToken item = ChildrenTokens[index];
            JToken previous = (index == 0) ? null : ChildrenTokens[index - 1];
            JToken next = (index == ChildrenTokens.Count - 1) ? null : ChildrenTokens[index + 1];

            if (previous != null)
            {
                previous.Next = next;
            }
            if (next != null)
            {
                next.Previous = previous;
            }

            item.Parent = null;
            item.Previous = null;
            item.Next = null;

            ChildrenTokens.RemoveAt(index);

            if (_listChanged != null)
            {
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
            }
            if (_collectionChanged != null)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
        }

        internal virtual bool RemoveItem(JToken item)
        {
            int index = IndexOfItem(item);
            if (index >= 0)
            {
                RemoveItemAt(index);
                return true;
            }

            return false;
        }

        internal virtual JToken GetItem(int index)
        {
            return ChildrenTokens[index];
        }

        internal virtual void SetItem(int index, JToken item)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
            }
            if (index >= ChildrenTokens.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");
            }

            JToken existing = ChildrenTokens[index];

            if (IsTokenUnchanged(existing, item))
            {
                return;
            }

            CheckReentrancy();

            item = EnsureParentToken(item, false);

            ValidateToken(item, existing);

            JToken previous = (index == 0) ? null : ChildrenTokens[index - 1];
            JToken next = (index == ChildrenTokens.Count - 1) ? null : ChildrenTokens[index + 1];

            item.Parent = this;

            item.Previous = previous;
            if (previous != null)
            {
                previous.Next = item;
            }

            item.Next = next;
            if (next != null)
            {
                next.Previous = item;
            }

            ChildrenTokens[index] = item;

            existing.Parent = null;
            existing.Previous = null;
            existing.Next = null;

            if (_listChanged != null)
            {
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
            }
            if (_collectionChanged != null)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, existing, index));
            }
        }

        internal virtual void ClearItems()
        {
            CheckReentrancy();

            foreach (JToken item in ChildrenTokens)
            {
                item.Parent = null;
                item.Previous = null;
                item.Next = null;
            }

            ChildrenTokens.Clear();

            if (_listChanged != null)
            {
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
            if (_collectionChanged != null)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        internal virtual void ReplaceItem(JToken existing, JToken replacement)
        {
            if (existing == null || existing.Parent != this)
            {
                return;
            }

            int index = IndexOfItem(existing);
            SetItem(index, replacement);
        }

        internal virtual bool ContainsItem(JToken item)
        {
            return (IndexOfItem(item) != -1);
        }

        internal virtual void CopyItemsTo(Array array, int arrayIndex)
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
            foreach (JToken token in ChildrenTokens)
            {
                array.SetValue(token, arrayIndex + index);
                index++;
            }
        }

        internal static bool IsTokenUnchanged(JToken currentValue, JToken newValue)
        {
            JValue v1 = currentValue as JValue;
            if (v1 != null)
            {
                if (v1.Type == JTokenType.Null && newValue == null)
                {
                    return true;
                }

                return v1.Equals(newValue);
            }

            return false;
        }

        internal virtual void ValidateToken(JToken o, JToken existing)
        {
            ValidationUtils.ArgumentNotNull(o, "o");

            if (o.Type == JTokenType.Property)
            {
                throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), GetType()));
            }
        }

        public virtual void Add(object content)
        {
            AddInternal(ChildrenTokens.Count, content, false);
        }

        internal void AddAndSkipParentCheck(JToken token)
        {
            AddInternal(ChildrenTokens.Count, token, true);
        }

        public void AddFirst(object content)
        {
            AddInternal(0, content, false);
        }

        internal void AddInternal(int index, object content, bool skipParentCheck)
        {
            if (IsMultiContent(content))
            {
                IEnumerable enumerable = (IEnumerable)content;

                int multiIndex = index;
                foreach (object c in enumerable)
                {
                    AddInternal(multiIndex, c, skipParentCheck);
                    multiIndex++;
                }
            }
            else
            {
                JToken item = CreateFromContent(content);

                InsertItem(index, item, skipParentCheck);
            }
        }

        internal static JToken CreateFromContent(object content)
        {
            if (content is JToken)
            {
                return (JToken)content;
            }

            return new JValue(content);
        }

        public JsonWriter CreateWriter()
        {
            return new JTokenWriter(this);
        }

        public void ReplaceAll(object content)
        {
            ClearItems();
            Add(content);
        }

        public void RemoveAll()
        {
            ClearItems();
        }

        internal abstract void MergeItem(object content, JsonMergeSettings settings);

        public void Merge(object content)
        {
            MergeItem(content, new JsonMergeSettings());
        }

        public void Merge(object content, JsonMergeSettings settings)
        {
            MergeItem(content, settings);
        }

        internal void ReadTokenFrom(JsonReader reader)
        {
            int startDepth = reader.Depth;

            if (!reader.Read())
            {
                throw JsonReaderException.Create(reader, "Error reading {0} from JsonReader.".FormatWith(CultureInfo.InvariantCulture, GetType().Name));
            }

            ReadContentFrom(reader);

            int endDepth = reader.Depth;

            if (endDepth > startDepth)
            {
                throw JsonReaderException.Create(reader, "Unexpected end of content while loading {0}.".FormatWith(CultureInfo.InvariantCulture, GetType().Name));
            }
        }

        internal void ReadContentFrom(JsonReader r)
        {
            ValidationUtils.ArgumentNotNull(r, "r");
            IJsonLineInfo lineInfo = r as IJsonLineInfo;

            JContainer parent = this;

            do
            {
                if (parent is JProperty && ((JProperty)parent).Value != null)
                {
                    if (parent == this)
                    {
                        return;
                    }

                    parent = parent.Parent;
                }

                switch (r.TokenType)
                {
                    case JsonToken.None:

                        break;
                    case JsonToken.StartArray:
                        JArray a = new JArray();
                        a.SetLineInfo(lineInfo);
                        parent.Add(a);
                        parent = a;
                        break;

                    case JsonToken.EndArray:
                        if (parent == this)
                        {
                            return;
                        }

                        parent = parent.Parent;
                        break;
                    case JsonToken.StartObject:
                        JObject o = new JObject();
                        o.SetLineInfo(lineInfo);
                        parent.Add(o);
                        parent = o;
                        break;
                    case JsonToken.EndObject:
                        if (parent == this)
                        {
                            return;
                        }

                        parent = parent.Parent;
                        break;
                    case JsonToken.StartConstructor:
                        JConstructor constructor = new JConstructor(r.Value.ToString());
                        constructor.SetLineInfo(lineInfo);
                        parent.Add(constructor);
                        parent = constructor;
                        break;
                    case JsonToken.EndConstructor:
                        if (parent == this)
                        {
                            return;
                        }

                        parent = parent.Parent;
                        break;
                    case JsonToken.String:
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.Date:
                    case JsonToken.Boolean:
                    case JsonToken.Bytes:
                        JValue v = new JValue(r.Value);
                        v.SetLineInfo(lineInfo);
                        parent.Add(v);
                        break;
                    case JsonToken.Comment:
                        v = JValue.CreateComment(r.Value.ToString());
                        v.SetLineInfo(lineInfo);
                        parent.Add(v);
                        break;
                    case JsonToken.Null:
                        v = JValue.CreateNull();
                        v.SetLineInfo(lineInfo);
                        parent.Add(v);
                        break;
                    case JsonToken.Undefined:
                        v = JValue.CreateUndefined();
                        v.SetLineInfo(lineInfo);
                        parent.Add(v);
                        break;
                    case JsonToken.PropertyName:
                        string propertyName = r.Value.ToString();
                        JProperty property = new JProperty(propertyName);
                        property.SetLineInfo(lineInfo);
                        JObject parentObject = (JObject)parent;

                        JProperty existingPropertyWithName = parentObject.Property(propertyName);
                        if (existingPropertyWithName == null)
                        {
                            parent.Add(property);
                        }
                        else
                        {
                            existingPropertyWithName.Replace(property);
                        }
                        parent = property;
                        break;
                    default:
                        throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith(CultureInfo.InvariantCulture, r.TokenType));
                }
            } while (r.Read());
        }

        internal int ContentsHashCode()
        {
            return ChildrenTokens.Aggregate(0, (current, item) => current ^ item.GetDeepHashCode());
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return string.Empty;
        }

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            ICustomTypeDescriptor d = First as ICustomTypeDescriptor;
            if (d != null)
            {
                return d.GetProperties();
            }

            return null;
        }

        int IList<JToken>.IndexOf(JToken item)
        {
            return IndexOfItem(item);
        }

        void IList<JToken>.Insert(int index, JToken item)
        {
            InsertItem(index, item, false);
        }

        void IList<JToken>.RemoveAt(int index)
        {
            RemoveItemAt(index);
        }

        void ICollection<JToken>.Add(JToken item)
        {
            Add(item);
        }

        void ICollection<JToken>.Clear()
        {
            ClearItems();
        }

        bool ICollection<JToken>.Contains(JToken item)
        {
            return ContainsItem(item);
        }

        void ICollection<JToken>.CopyTo(JToken[] array, int arrayIndex)
        {
            CopyItemsTo(array, arrayIndex);
        }

        bool ICollection<JToken>.Remove(JToken item)
        {
            return RemoveItem(item);
        }

        private JToken EnsureValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is JToken)
            {
                return (JToken)value;
            }

            throw new ArgumentException("Argument is not a JToken.");
        }

        int IList.Add(object value)
        {
            Add(EnsureValue(value));
            return Count - 1;
        }

        void IList.Clear()
        {
            ClearItems();
        }

        bool IList.Contains(object value)
        {
            return ContainsItem(EnsureValue(value));
        }

        int IList.IndexOf(object value)
        {
            return IndexOfItem(EnsureValue(value));
        }

        void IList.Insert(int index, object value)
        {
            InsertItem(index, EnsureValue(value), false);
        }

        void IList.Remove(object value)
        {
            RemoveItem(EnsureValue(value));
        }

        void IList.RemoveAt(int index)
        {
            RemoveItemAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyItemsTo(array, index);
        }

        void IBindingList.AddIndex(PropertyDescriptor property)
        {
        }

        object IBindingList.AddNew()
        {
            AddingNewEventArgs args = new AddingNewEventArgs();
            OnAddingNew(args);

            if (args.NewObject == null)
            {
                throw new JsonException("Could not determine new value to add to '{0}'.".FormatWith(CultureInfo.InvariantCulture, GetType()));
            }

            if (!(args.NewObject is JToken))
            {
                throw new JsonException("New item to be added to collection must be compatible with {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(JToken)));
            }

            JToken newItem = (JToken)args.NewObject;
            Add(newItem);

            return newItem;
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotSupportedException();
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
        }

        void IBindingList.RemoveSort()
        {
            throw new NotSupportedException();
        }

        internal static void MergeEnumerableContent(JContainer target, IEnumerable content, JsonMergeSettings settings)
        {
            switch (settings.MergeArrayHandling)
            {
                case MergeArrayHandling.Concat:
                    foreach (JToken item in content)
                    {
                        target.Add(item);
                    }
                    break;
                case MergeArrayHandling.Union:
                    HashSet<JToken> items = new HashSet<JToken>(target, EqualityComparer);

                    foreach (JToken item in content)
                    {
                        if (items.Add(item))
                        {
                            target.Add(item);
                        }
                    }
                    break;
                case MergeArrayHandling.Replace:
                    target.ClearItems();
                    foreach (JToken item in content)
                    {
                        target.Add(item);
                    }
                    break;
                case MergeArrayHandling.Merge:
                    int i = 0;
                    foreach (object targetItem in content)
                    {
                        if (i < target.Count)
                        {
                            JToken sourceItem = target[i];

                            JContainer existingContainer = sourceItem as JContainer;
                            if (existingContainer != null)
                            {
                                existingContainer.Merge(targetItem, settings);
                            }
                            else
                            {
                                if (targetItem != null)
                                {
                                    JToken contentValue = CreateFromContent(targetItem);
                                    if (contentValue.Type != JTokenType.Null)
                                    {
                                        target[i] = contentValue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            target.Add(targetItem);
                        }

                        i++;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("settings", "Unexpected merge array handling when merging JSON.");
            }
        }

        #endregion

        #region Вложенный класс: JTokenReferenceEqualityComparer

        private class JTokenReferenceEqualityComparer : IEqualityComparer<JToken>
        {
            #region Поля

            public static readonly JTokenReferenceEqualityComparer Instance = new JTokenReferenceEqualityComparer();

            #endregion

            #region Методы

            public bool Equals(JToken x, JToken y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(JToken obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                return obj.GetHashCode();
            }

            #endregion
        }

        #endregion
    }
}