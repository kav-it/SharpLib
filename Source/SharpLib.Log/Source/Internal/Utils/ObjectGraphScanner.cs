
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SharpLib.Log
{
    internal class ObjectGraphScanner
    {
        #region Методы

        public static T[] FindReachableObjects<T>(params object[] rootObjects)
            where T : class
        {
            var result = new List<T>();
            var visitedObjects = new Dictionary<object, int>();

            foreach (var rootObject in rootObjects)
            {
                ScanProperties(result, rootObject, 0, visitedObjects);
            }

            return result.ToArray();
        }

        private static void ScanProperties<T>(List<T> result, object o, int level, Dictionary<object, int> visitedObjects)
            where T : class
        {
            if (o == null)
            {
                return;
            }

            if (!o.GetType().IsDefined(typeof(LogConfigurationItemAttribute), true))
            {
                return;
            }

            if (visitedObjects.ContainsKey(o))
            {
                return;
            }

            visitedObjects.Add(o, 0);

            var t = o as T;
            if (t != null)
            {
                result.Add(t);
            }

            foreach (PropertyInfo prop in PropertyHelper.GetAllReadableProperties(o.GetType()))
            {
                if (prop.PropertyType.IsPrimitive || prop.PropertyType.IsEnum || prop.PropertyType == typeof(string) || prop.IsDefined(typeof(LogConfigurationIgnorePropertyAttribute), true))
                {
                    continue;
                }

                object value = prop.GetValue(o, null);
                if (value == null)
                {
                    continue;
                }

                var enumerable = value as IEnumerable;
                if (enumerable != null)
                {
                    foreach (object element in enumerable)
                    {
                        ScanProperties(result, element, level + 1, visitedObjects);
                    }
                }
                else
                {
                    ScanProperties(result, value, level + 1, visitedObjects);
                }
            }
        }

        #endregion
    }
}
