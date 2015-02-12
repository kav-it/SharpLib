using System;

namespace SharpLib
{
    public static class ExtensionStringArray
    {
        #region Методы

        public static void SortEx(this string[] value, Boolean descending = false)
        {
            Array.Sort(value);
            if (descending)
            {
                Array.Reverse(value);
            }
        }

        #endregion
    }
}