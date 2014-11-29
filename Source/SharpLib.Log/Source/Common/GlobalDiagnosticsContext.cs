using System.Collections.Generic;

namespace NLog
{
    public static class GlobalDiagnosticsContext
    {
        #region Поля

        private static readonly Dictionary<string, string> dict = new Dictionary<string, string>();

        #endregion

        #region Методы

        public static void Set(string item, string value)
        {
            lock (dict)
            {
                dict[item] = value;
            }
        }

        public static string Get(string item)
        {
            lock (dict)
            {
                string s;

                if (!dict.TryGetValue(item, out s))
                {
                    s = string.Empty;
                }

                return s;
            }
        }

        public static bool Contains(string item)
        {
            lock (dict)
            {
                return dict.ContainsKey(item);
            }
        }

        public static void Remove(string item)
        {
            lock (dict)
            {
                dict.Remove(item);
            }
        }

        public static void Clear()
        {
            lock (dict)
            {
                dict.Clear();
            }
        }

        #endregion
    }
}