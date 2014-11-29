using System.Collections.Generic;

using NLog.Internal;

namespace NLog
{
    public static class MappedDiagnosticsContext
    {
        #region Поля

        private static readonly object dataSlot = ThreadLocalStorageHelper.AllocateDataSlot();

        #endregion

        #region Свойства

        internal static IDictionary<string, string> ThreadDictionary
        {
            get { return ThreadLocalStorageHelper.GetDataForSlot<Dictionary<string, string>>(dataSlot); }
        }

        #endregion

        #region Методы

        public static void Set(string item, string value)
        {
            ThreadDictionary[item] = value;
        }

        public static string Get(string item)
        {
            string s;

            if (!ThreadDictionary.TryGetValue(item, out s))
            {
                s = string.Empty;
            }

            return s;
        }

        public static bool Contains(string item)
        {
            return ThreadDictionary.ContainsKey(item);
        }

        public static void Remove(string item)
        {
            ThreadDictionary.Remove(item);
        }

        public static void Clear()
        {
            ThreadDictionary.Clear();
        }

        #endregion
    }
}