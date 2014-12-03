using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpLib
{
    public static class ExtensionReflector
    {
        #region Методы

        public static Object GetValueEx(this PropertyInfo property, Object obj)
        {
            return property.GetValue(obj, null);    
        }

        public static void SetValueEx(this PropertyInfo property, Object obj, Object value)
        {
            value = Convert.ChangeType(value, property.PropertyType);

            property.SetValue(obj, value, null);    
        }

        #endregion
    }


}