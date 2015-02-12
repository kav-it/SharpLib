using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpLib
{
    /// <summary>
    /// Класс расширения для "ListT"
    /// </summary>
    public static class ExtensionListT
    {
        public static void MoveEx<T>(this List<T> self, int newIndex, T value)
        {
            if (newIndex < 0 || newIndex > self.Count)
            {
                return;
            }

            var oldIndex = self.IndexOf(value);

            if (oldIndex == -1)
            {
                return;
            }

            if (newIndex == self.Count)
            {
                // Добавление к конец списка
                self.RemoveAt(oldIndex);
                self.Add(value);
            }
            else
            {
                self.RemoveAt(oldIndex);

                if (newIndex > oldIndex)
                {
                    newIndex--;
                }

                self.Insert(newIndex, value);
            }
        }

        /// <summary>
        /// Добавление элемента в начало
        /// </summary>
        public static void AddFirstEx<T>(this List<T> self, T value)
        {
            self.Insert(0, value);
        }

        /// <summary>
        /// Удаление последнего элемента
        /// </summary>
        public static void RemoveLastEx<T>(this List<T> self)
        {
            if (self.Any())
            {
                self.RemoveAt(self.Count - 1);
            }
        }

        /// <summary>
        /// Сортировка
        /// </summary>
        public static List<TSource> SortEx<TSource, TKey>(this List<TSource> self, Func<TSource, TKey> keySelector)
        {
            return self.OrderBy(keySelector).ToList();
        }
    }
}