using System.Collections.Generic;

namespace SharpLib
{
    public static class ExtensionDictionary
    {
        #region Методы

        /// <summary>
        /// Получение элемента из словаря. Если нет возвращение default(T). Null для классов
        /// </summary>
        public static TValue GetValueEx<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            dictionary.TryGetValue(key, out value);

            return value;
        }

        #endregion
    }
}