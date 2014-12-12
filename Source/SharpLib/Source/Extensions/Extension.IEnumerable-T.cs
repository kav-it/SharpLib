using System.Collections.Generic;

namespace SharpLib
{
    public static class ExtensionEnumerableT
    {
        public static string JoinEx<T>(this IEnumerable<T> enumerable, string separator)
        {
            return string.Join(separator, enumerable);
        }
    }
}