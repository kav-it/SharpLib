using System.Collections.Concurrent;

namespace SharpLib
{
    public static class ExtensionConcurrentDictionary
    {
        #region Методы

        /// <summary>
        /// Просто метод с названием родным мне
        /// </summary>
        public static bool AddEx<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key, TValue value)
        {
            return self.TryAdd(key, value);
        }

        /// <summary>
        /// Человеческое удаление, без лишнего параметра "out"
        /// </summary>
        public static bool RemoveEx<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> self, TKey key)
        {
            TValue value;
            return self.TryRemove(key, out value);
        }

        #endregion
    }
}