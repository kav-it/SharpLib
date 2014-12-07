using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Json.Linq
{
    public struct JEnumerable<T> : IJEnumerable<T>, IEquatable<JEnumerable<T>> where T : JToken
    {
        #region Поля

        public static readonly JEnumerable<T> Empty = new JEnumerable<T>(Enumerable.Empty<T>());

        private readonly IEnumerable<T> _enumerable;

        #endregion

        #region Свойства

        public IJEnumerable<JToken> this[object key]
        {
            get
            {
                if (_enumerable == null)
                {
                    return JEnumerable<JToken>.Empty;
                }

                return new JEnumerable<JToken>(_enumerable.Values<T, JToken>(key));
            }
        }

        #endregion

        #region Конструктор

        public JEnumerable(IEnumerable<T> enumerable)
        {
            ValidationUtils.ArgumentNotNull(enumerable, "enumerable");

            _enumerable = enumerable;
        }

        #endregion

        #region Методы

        public IEnumerator<T> GetEnumerator()
        {
            if (_enumerable == null)
            {
                return Empty.GetEnumerator();
            }

            return _enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(JEnumerable<T> other)
        {
            return Equals(_enumerable, other._enumerable);
        }

        public override bool Equals(object obj)
        {
            if (obj is JEnumerable<T>)
            {
                return Equals((JEnumerable<T>)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (_enumerable == null)
            {
                return 0;
            }

            return _enumerable.GetHashCode();
        }

        #endregion
    }
}