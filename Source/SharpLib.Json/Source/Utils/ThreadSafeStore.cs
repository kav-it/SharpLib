using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpLib.Json
{
    internal class ThreadSafeStore<TKey, TValue>
    {
        #region Поля

        private readonly Func<TKey, TValue> _creator;

        private readonly object _lock = new object();

        private Dictionary<TKey, TValue> _store;

        #endregion

        #region Конструктор

        public ThreadSafeStore(Func<TKey, TValue> creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }

            _creator = creator;
            _store = new Dictionary<TKey, TValue>();
        }

        #endregion

        #region Методы

        public TValue Get(TKey key)
        {
            TValue value;
            if (!_store.TryGetValue(key, out value))
            {
                return AddValue(key);
            }

            return value;
        }

        private TValue AddValue(TKey key)
        {
            TValue value = _creator(key);

            lock (_lock)
            {
                if (_store == null)
                {
                    _store = new Dictionary<TKey, TValue>();
                    _store[key] = value;
                }
                else
                {
                    // double check locking
                    TValue checkValue;
                    if (_store.TryGetValue(key, out checkValue))
                    {
                        return checkValue;
                    }

                    Dictionary<TKey, TValue> newStore = new Dictionary<TKey, TValue>(_store);
                    newStore[key] = value;

                    Thread.MemoryBarrier();
                    _store = newStore;
                }

                return value;
            }
        }

        #endregion
    }
}