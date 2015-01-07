using System;
using System.Reflection;

namespace SharpLib
{
    public static class ExtensionReflector
    {
        #region Методы

        public static object GetValueEx(this PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);    
        }

        public static void SetValueEx(this PropertyInfo property, object obj, object value)
        {
            value = Convert.ChangeType(value, property.PropertyType);

            property.SetValue(obj, value, null);    
        }

        #endregion
    }


}