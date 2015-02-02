using System;
using System.Reflection;

namespace SharpLib.Audio.Utils
{
    internal static class FieldDescriptionHelper
    {
        #region Методы

        public static string Describe(Type t, Guid guid)
        {
            foreach (var f in t
#if NETFX_CORE
                .GetRuntimeFields())
#else
                .GetFields(BindingFlags.Static | BindingFlags.Public))
#endif
            {
                if (f.IsPublic && f.IsStatic && f.FieldType == typeof(Guid) && (Guid)f.GetValue(null) == guid)
                {
                    foreach (var a in f.GetCustomAttributes(false))
                    {
                        var d = a as FieldDescriptionAttribute;
                        if (d != null)
                        {
                            return d.Description;
                        }
                    }

                    return f.Name;
                }
            }
            return guid.ToString();
        }

        #endregion
    }
}