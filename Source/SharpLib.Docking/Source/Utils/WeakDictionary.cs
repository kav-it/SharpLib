using System;
using System.Collections.Generic;

namespace SharpLib.Docking.Controls
{
    internal class WeakDictionary<TK, TV> where TK : class
    {
        #region Поля

        private readonly List<WeakReference> _keys;

        private readonly List<TV> _values;

        public WeakDictionary()
        {
            _values = new List<TV>();
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
                _values[vIndex] = value;
            }
            else
            {
                _values.Add(value);
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
            value = _values[vIndex];
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
        }

        #endregion
    }
}