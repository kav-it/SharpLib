using System;
using System.Linq;
using System.Reflection;

namespace SharpLib.Log
{
    internal static class ReflectionHelpers
    {
        #region Методы

        public static Type[] SafeGetTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException typeLoadException)
            {
                return typeLoadException.Types.Where(t => t != null).ToArray();
            }
        }

        #endregion
    }
}