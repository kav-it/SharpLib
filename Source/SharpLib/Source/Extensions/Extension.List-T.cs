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
            if (newIndex < 0 || newIndex >= self.Count)
            {
                return;
            }

            var oldIndex = self.IndexOf(value);

            if (oldIndex == -1)
            {
                return;
            }

            self.RemoveAt(oldIndex);

            if (newIndex > oldIndex)
            {
                newIndex--;
            }

            self.Insert(newIndex, value);
        }
    }
}