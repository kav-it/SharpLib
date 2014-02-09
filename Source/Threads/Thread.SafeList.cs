// ****************************************************************************
//
// Имя файла    : 'ThreadSafeList.cs'
// Заголовок    : Потокобезопасный список
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 09/01/2014
//
// ****************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib
{
    public class ThreadSafeList<T> : IList<T>
    {
        #region Поля

        private readonly List<T> _items = new List<T>();

        #endregion

        #region Свойства

        public long LongCount
        {
            get
            {
                lock (_items)
                {
                    return _items.LongCount();
                }
            }
        }

        public int Count
        {
            get
            {
                lock (_items)
                {
                    return _items.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get
            {
                lock (_items)
                {
                    return _items[index];
                }
            }
            set
            {
                lock (_items)
                {
                    _items[index] = value;
                }
            }
        }

        #endregion

        #region Конструктор

        public ThreadSafeList(IEnumerable<T> items = null)
        {
            Add(items);
        }

        #endregion

        #region Методы

        public IEnumerator<T> GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (Equals(default(T), item)) return;
            lock (_items)
            {
                _items.Add(item);
            }
        }

        public Boolean TryAdd(T item)
        {
            try
            {
                if (Equals(default(T), item)) return false;
                lock (_items)
                {
                    _items.Add(item);
                    return true;
                }
            }
            catch (NullReferenceException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (ArgumentException)
            {
            }
            return false;
        }

        public void Clear()
        {
            lock (_items)
            {
                _items.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_items)
            {
                return _items.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_items)
            {
                _items.CopyTo(array, arrayIndex);
            }
        }

        public bool Delete(T item)
        {
            return Remove(item);
        }

        public bool Remove(T item)
        {
            lock (_items)
            {
                return _items.Remove(item);
            }
        }

        public int IndexOf(T item)
        {
            lock (_items)
            {
                return _items.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_items)
            {
                _items.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_items)
            {
                _items.RemoveAt(index);
            }
        }

        public void Add(IEnumerable<T> collection)
        {
            if (collection == null) return;
            lock (_items)
            {
                _items.AddRange(collection.Where(arg => !Equals(default(T), arg)));
            }
        }

        public List<T> Clone(Boolean asParallel = true)
        {
            lock (_items)
            {
                return new List<T>(_items);
            }
        }

        #endregion
    }
}