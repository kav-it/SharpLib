using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib
{
    /// <summary>
    /// Расширения класса IEnumerable
    /// </summary>
    public static class ExtensionEnumerableT
    {
        /// <summary>
        /// Объединение строк 
        /// </summary>
        public static string JoinEx<T>(this IEnumerable<T> self, string separator)
        {
            return string.Join(separator, self);
        }

        /// <summary>
        /// Представление дерева в виде плоского списка
        /// </summary>
        public static IEnumerable<T> FlatternEx<T>(this IEnumerable<T> self, Func<T, IEnumerable<T>> func)
        {
            var list = self.ToList();

            return list.SelectMany(c => func(c).FlatternEx(func)).Concat(list);
        }
    }
}