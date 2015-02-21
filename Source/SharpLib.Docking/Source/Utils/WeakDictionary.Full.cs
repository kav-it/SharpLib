using System;
using System.Collections.Generic;

namespace SharpLib.Docking.Controls
{
    internal class FullWeakDictionary<TK, TV> where TK : class
    {
        #region Поля

        private readonly List<WeakReference> _keys;

        private readonly List<WeakReference> _values;

        public FullWeakDictionary()
        {
            _values = new List<WeakReference>();
            _keys = new List<WeakReference>();
        }

        #endregion

        #region Свойства

        public TV this[TK key]
        {
            get
            {
                TV valueToReturn;
                if (!GetValue(key, out valueToReturn))
                {
                    throw new ArgumentException();
                }
                return valueToReturn;
            }
            set { SetValue(key, value); }
        }

        #endregion

        #region Методы

        public bool ContainsKey(TK key)
        {
            CollectGarbage();
            return -1 != _keys.FindIndex(k => k.GetValueOrDefault<TK>() == key);
        }

        public void SetValue(TK key, TV value)
        {
            CollectGarbage();
            int vIndex = _keys.FindIndex(k => k.GetValueOrDefault<TK>() == key);
            if (vIndex > -1)
            {
                _values[vIndex] = new WeakReference(value);
            }
            else
            {
                _values.Add(new WeakReference(value));
                _keys.Add(new WeakReference(key));
            }
        }

        public bool GetValue(TK key, out TV value)
        {
            CollectGarbage();
            int vIndex = _keys.FindIndex(k => k.GetValueOrDefault<TK>() == key);
            value = default(TV);
            if (vIndex == -1)
            {
                return false;
            }
            value = _values[vIndex].GetValueOrDefault<TV>();
            return true;
        }

        private void CollectGarbage()
        {
            int vIndex = 0;

            do
            {
                vIndex = _keys.FindIndex(vIndex, k => !k.IsAlive);
                if (vIndex >= 0)
                {
                    _keys.RemoveAt(vIndex);
                    _values.RemoveAt(vIndex);
                }
            } while (vIndex >= 0);

            vIndex = 0;
            do
            {
                vIndex = _values.FindIndex(vIndex, v => !v.IsAlive);
                if (vIndex >= 0)
                {
                    _values.RemoveAt(vIndex);
                    _keys.RemoveAt(vIndex);
                }
            } while (vIndex >= 0);
        }

        #endregion
    }
}