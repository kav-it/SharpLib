using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpLib.Log
{
    internal class DictionaryAdapter<TKey, TValue> : IDictionary
    {
        #region Поля

        private readonly IDictionary<TKey, TValue> implementation;

        #endregion

        #region Свойства

        public ICollection Values
        {
            get { return new List<TValue>(implementation.Values); }
        }

        public int Count
        {
            get { return implementation.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return implementation; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return implementation.IsReadOnly; }
        }

        public ICollection Keys
        {
            get { return new List<TKey>(implementation.Keys); }
        }

        public object this[object key]
        {
            get
            {
                TValue value;

                if (implementation.TryGetValue((TKey)key, out value))
                {
                    return value;
                }
                return null;
            }

            set { implementation[(TKey)key] = (TValue)value; }
        }

        #endregion

        #region Конструктор

        public DictionaryAdapter(IDictionary<TKey, TValue> implementation)
        {
            this.implementation = implementation;
        }

        #endregion

        #region Методы

        public void Add(object key, object value)
        {
            implementation.Add((TKey)key, (TValue)value);
        }

        public void Clear()
        {
            implementation.Clear();
        }

        public bool Contains(object key)
        {
            return implementation.ContainsKey((TKey)key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return new MyEnumerator(implementation.GetEnumerator());
        }

        public void Remove(object key)
        {
            implementation.Remove((TKey)key);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Вложенный класс: MyEnumerator

        private class MyEnumerator : IDictionaryEnumerator
        {
            #region Поля

            private readonly IEnumerator<KeyValuePair<TKey, TValue>> wrapped;

            #endregion

            #region Свойства

            public DictionaryEntry Entry
            {
                get { return new DictionaryEntry(wrapped.Current.Key, wrapped.Current.Value); }
            }

            public object Key
            {
                get { return wrapped.Current.Key; }
            }

            public object Value
            {
                get { return wrapped.Current.Value; }
            }

            public object Current
            {
                get { return Entry; }
            }

            #endregion

            #region Конструктор

            public MyEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> wrapped)
            {
                this.wrapped = wrapped;
            }

            #endregion

            #region Методы

            public bool MoveNext()
            {
                return wrapped.MoveNext();
            }

            public void Reset()
            {
                wrapped.Reset();
            }

            #endregion
        }

        #endregion
    }
}