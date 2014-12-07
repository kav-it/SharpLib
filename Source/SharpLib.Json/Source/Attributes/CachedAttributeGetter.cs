using System;

namespace SharpLib.Json
{
    internal static class CachedAttributeGetter<T> where T : Attribute
    {
        #region Поля

        private static readonly ThreadSafeStore<object, T> TypeAttributeCache = new ThreadSafeStore<object, T>(JsonTypeReflector.GetAttribute<T>);

        #endregion

        #region Методы

        public static T GetAttribute(object type)
        {
            return TypeAttributeCache.Get(type);
        }

        #endregion
    }
}