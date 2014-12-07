﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SharpLib.Json
{
    internal class DictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>, IWrappedDictionary
    {
        #region Поля

        private readonly IDictionary _dictionary;

        private readonly IDictionary<TKey, TValue> _genericDictionary;

        private object _syncRoot;

        #endregion

        #region Свойства

        public ICollection<TKey> Keys
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary.Keys.Cast<TKey>().ToList();
                }
                return _genericDictionary.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary.Values.Cast<TValue>().ToList();
                }
                return _genericDictionary.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_dictionary != null)
                {
                    return (TValue)_dictionary[key];
                }
                return _genericDictionary[key];
            }
            set
            {
                if (_dictionary != null)
                {
                    _dictionary[key] = value;
                }
                else
                {
                    _genericDictionary[key] = value;
                }
            }
        }

        public int Count
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary.Count;
                }
                return _genericDictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary.IsReadOnly;
                }
                return _genericDictionary.IsReadOnly;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary[key];
                }
                return _genericDictionary[(TKey)key];
            }
            set
            {
                if (_dictionary != null)
                {
                    _dictionary[key] = value;
                }
                else
                {
                    _genericDictionary[(TKey)key] = (TValue)value;
                }
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                if (_genericDictionary != null)
                {
                    return false;
                }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
                else if (_readOnlyDictionary != null)
                    return true;
#endif
                return _dictionary.IsFixedSize;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                if (_genericDictionary != null)
                {
                    return _genericDictionary.Keys.ToList();
                }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
                else if (_readOnlyDictionary != null)
                    return _readOnlyDictionary.Keys.ToList();
#endif
                return _dictionary.Keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                if (_genericDictionary != null)
                {
                    return _genericDictionary.Values.ToList();
                }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
                else if (_readOnlyDictionary != null)
                    return _readOnlyDictionary.Values.ToList();
#endif
                return _dictionary.Values;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary.IsSynchronized;
                }
                return false;
            }
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

        public object UnderlyingDictionary
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary;
                }
                return _genericDictionary;
            }
        }

        #endregion

        #region Конструктор

        public DictionaryWrapper(IDictionary dictionary)
        {
            ValidationUtils.ArgumentNotNull(dictionary, "dictionary");

            _dictionary = dictionary;
        }

        public DictionaryWrapper(IDictionary<TKey, TValue> dictionary)
        {
            ValidationUtils.ArgumentNotNull(dictionary, "dictionary");

            _genericDictionary = dictionary;
        }

        #endregion

        #region Методы

        public void Add(TKey key, TValue value)
        {
            if (_dictionary != null)
            {
                _dictionary.Add(key, value);
            }
            else if (_genericDictionary != null)
            {
                _genericDictionary.Add(key, value);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (_dictionary != null)
            {
                return _dictionary.Contains(key);
            }
            return _genericDictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (_dictionary != null)
            {
                if (_dictionary.Contains(key))
                {
                    _dictionary.Remove(key);
                    return true;
                }
                return false;
            }
            return _genericDictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_dictionary != null)
            {
                if (!_dictionary.Contains(key))
                {
                    value = default(TValue);
                    return false;
                }
                value = (TValue)_dictionary[key];
                return true;
            }
            return _genericDictionary.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary != null)
            {
                ((IList)_dictionary).Add(item);
            }
            else if (_genericDictionary != null)
            {
                _genericDictionary.Add(item);
            }
        }

        public void Clear()
        {
            if (_dictionary != null)
            {
                _dictionary.Clear();
            }
            else
            {
                _genericDictionary.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary != null)
            {
                return ((IList)_dictionary).Contains(item);
            }
            return _genericDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (_dictionary != null)
            {
                foreach (DictionaryEntry item in _dictionary)
                {
                    array[arrayIndex++] = new KeyValuePair<TKey, TValue>((TKey)item.Key, (TValue)item.Value);
                }
            }
            else
            {
                _genericDictionary.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary != null)
            {
                if (_dictionary.Contains(item.Key))
                {
                    object value = _dictionary[item.Key];

                    if (Equals(value, item.Value))
                    {
                        _dictionary.Remove(item.Key);
                        return true;
                    }
                    return false;
                }
                return true;
            }
            return _genericDictionary.Remove(item);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (_dictionary != null)
            {
                return _dictionary.Cast<DictionaryEntry>().Select(de => new KeyValuePair<TKey, TValue>((TKey)de.Key, (TValue)de.Value)).GetEnumerator();
            }
            return _genericDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IDictionary.Add(object key, object value)
        {
            if (_dictionary != null)
            {
                _dictionary.Add(key, value);
            }
            else
            {
                _genericDictionary.Add((TKey)key, (TValue)value);
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            if (_dictionary != null)
            {
                return _dictionary.GetEnumerator();
            }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
            else if (_readOnlyDictionary != null)
                return new DictionaryEnumerator<TKey, TValue>(_readOnlyDictionary.GetEnumerator());
#endif
            return new DictionaryEnumerator<TKey, TValue>(_genericDictionary.GetEnumerator());
        }

        bool IDictionary.Contains(object key)
        {
            if (_genericDictionary != null)
            {
                return _genericDictionary.ContainsKey((TKey)key);
            }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
            else if (_readOnlyDictionary != null)
                return _readOnlyDictionary.ContainsKey((TKey)key);
#endif
            return _dictionary.Contains(key);
        }

        public void Remove(object key)
        {
            if (_dictionary != null)
            {
                _dictionary.Remove(key);
            }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
            else if (_readOnlyDictionary != null)
                throw new NotSupportedException();
#endif
            else
            {
                _genericDictionary.Remove((TKey)key);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (_dictionary != null)
            {
                _dictionary.CopyTo(array, index);
            }
#if !(NET40 || NET35 || NET20 || PORTABLE40)
            else if (_readOnlyDictionary != null)
                throw new NotSupportedException();
#endif
            else
            {
                _genericDictionary.CopyTo((KeyValuePair<TKey, TValue>[])array, index);
            }
        }

        #endregion

        #region Вложенный класс: DictionaryEnumerator

        private struct DictionaryEnumerator<TEnumeratorKey, TEnumeratorValue> : IDictionaryEnumerator
        {
            #region Поля

            private readonly IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;

            #endregion

            #region Свойства

            public DictionaryEntry Entry
            {
                get { return (DictionaryEntry)Current; }
            }

            public object Key
            {
                get { return Entry.Key; }
            }

            public object Value
            {
                get { return Entry.Value; }
            }

            public object Current
            {
                get { return new DictionaryEntry(_e.Current.Key, _e.Current.Value); }
            }

            #endregion

            #region Конструктор

            public DictionaryEnumerator(IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
            {
                ValidationUtils.ArgumentNotNull(e, "e");
                _e = e;
            }

            #endregion

            #region Методы

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            public void Reset()
            {
                _e.Reset();
            }

            #endregion
        }

        #endregion
    }
}