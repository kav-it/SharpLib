﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Notepad.Utils
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [Serializable]
    internal sealed class Deque<T> : ICollection<T>
    {
        #region Поля

        private T[] arr = Empty<T>.Array;

        private int head;

        private int size;

        private int tail;

        #endregion

        #region Свойства

        public int Count
        {
            get { return size; }
        }

        public T this[int index]
        {
            get
            {
                ThrowUtil.CheckInRangeInclusive(index, "index", 0, size - 1);
                return arr[(head + index) % arr.Length];
            }
            set
            {
                ThrowUtil.CheckInRangeInclusive(index, "index", 0, size - 1);
                arr[(head + index) % arr.Length] = value;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Методы

        public void Clear()
        {
            arr = Empty<T>.Array;
            size = 0;
            head = 0;
            tail = 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "PushBack")]
        public void PushBack(T item)
        {
            if (size == arr.Length)
            {
                SetCapacity(Math.Max(4, arr.Length * 2));
            }
            arr[tail++] = item;
            if (tail == arr.Length)
            {
                tail = 0;
            }
            size++;
        }

        public T PopBack()
        {
            if (size == 0)
            {
                throw new InvalidOperationException();
            }
            if (tail == 0)
            {
                tail = arr.Length - 1;
            }
            else
            {
                tail--;
            }
            var val = arr[tail];
            arr[tail] = default(T);
            size--;
            return val;
        }

        public void PushFront(T item)
        {
            if (size == arr.Length)
            {
                SetCapacity(Math.Max(4, arr.Length * 2));
            }
            if (head == 0)
            {
                head = arr.Length - 1;
            }
            else
            {
                head--;
            }
            arr[head] = item;
            size++;
        }

        public T PopFront()
        {
            if (size == 0)
            {
                throw new InvalidOperationException();
            }
            var val = arr[head];
            arr[head] = default(T);
            head++;
            if (head == arr.Length)
            {
                head = 0;
            }
            size--;
            return val;
        }

        private void SetCapacity(int capacity)
        {
            var newArr = new T[capacity];
            CopyTo(newArr, 0);
            head = 0;
            tail = (size == capacity) ? 0 : size;
            arr = newArr;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (head < tail)
            {
                for (int i = head; i < tail; i++)
                {
                    yield return arr[i];
                }
            }
            else
            {
                for (int i = head; i < arr.Length; i++)
                {
                    yield return arr[i];
                }
                for (int i = 0; i < tail; i++)
                {
                    yield return arr[i];
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            PushBack(item);
        }

        public bool Contains(T item)
        {
            var comparer = EqualityComparer<T>.Default;

            return this.Any(element => comparer.Equals(item, element));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (head < tail)
            {
                Array.Copy(arr, head, array, arrayIndex, tail - head);
            }
            else
            {
                int num1 = arr.Length - head;
                Array.Copy(arr, head, array, arrayIndex, num1);
                Array.Copy(arr, 0, array, arrayIndex + num1, tail);
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}