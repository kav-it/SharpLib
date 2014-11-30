
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    internal static class PropertyHelper
    {
        #region Поля

        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> parameterInfoCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        #endregion

        #region Методы

        internal static void SetPropertyFromString(object o, string name, string value, ConfigurationItemFactory configurationItemFactory)
        {
            PropertyInfo propInfo;

            if (!TryGetPropertyInfo(o, name, out propInfo))
            {
                throw new NotSupportedException("Parameter " + name + " not supported on " + o.GetType().Name);
            }

            if (propInfo.IsDefined(typeof(ArrayParameterAttribute), false))
            {
                throw new NotSupportedException("Parameter " + name + " of " + o.GetType().Name + " is an array and cannot be assigned a scalar value.");
            }

            object newValue;

            Type propertyType = propInfo.PropertyType;

            propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (!TryLogSpecificConversion(propertyType, value, out newValue, configurationItemFactory))
            {
                if (!TryGetEnumValue(propertyType, value, out newValue))
                {
                    if (!TryImplicitConversion(propertyType, value, out newValue))
                    {
                        if (!TrySpecialConversion(propertyType, value, out newValue))
                        {
                            newValue = Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            propInfo.SetValue(o, newValue, null);
        }

        internal static bool IsArrayProperty(Type t, string name)
        {
            PropertyInfo propInfo;

            if (!TryGetPropertyInfo(t, name, out propInfo))
            {
                throw new NotSupportedException("Parameter " + name + " not supported on " + t.Name);
            }

            return propInfo.IsDefined(typeof(ArrayParameterAttribute), false);
        }

        internal static bool TryGetPropertyInfo(object o, string propertyName, out PropertyInfo result)
        {
            PropertyInfo propInfo = o.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propInfo != null)
            {
                result = propInfo;
                return true;
            }

            lock (parameterInfoCache)
            {
                Type targetType = o.GetType();
                Dictionary<string, PropertyInfo> cache;

                if (!parameterInfoCache.TryGetValue(targetType, out cache))
                {
                    cache = BuildPropertyInfoDictionary(targetType);
                    parameterInfoCache[targetType] = cache;
                }

                return cache.TryGetValue(propertyName, out result);
            }
        }

        internal static Type GetArrayItemType(PropertyInfo propInfo)
        {
            var arrayParameterAttribute = (ArrayParameterAttribute)Attribute.GetCustomAttribute(propInfo, typeof(ArrayParameterAttribute));
            if (arrayParameterAttribute != null)
            {
                return arrayParameterAttribute.ItemType;
            }

            return null;
        }

        internal static IEnumerable<PropertyInfo> GetAllReadableProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        internal static void CheckRequiredParameters(object o)
        {
            foreach (PropertyInfo propInfo in PropertyHelper.GetAllReadableProperties(o.GetType()))
            {
                if (propInfo.IsDefined(typeof(RequiredParameterAttribute), false))
                {
                    object value = propInfo.GetValue(o, null);
                    if (value == null)
                    {
                        throw new Exception("Required parameter '" + propInfo.Name + "' on '" + o + "' was not specified.");
                    }
                }
            }
        }

        private static bool TryImplicitConversion(Type resultType, string value, out object result)
        {
            MethodInfo operatorImplicitMethod = resultType.GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            if (operatorImplicitMethod == null)
            {
                result = null;
                return false;
            }

            result = operatorImplicitMethod.Invoke(null, new object[] { value });
            return true;
        }

        private static bool TryLogSpecificConversion(Type propertyType, string value, out object newValue, ConfigurationItemFactory configurationItemFactory)
        {
            if (propertyType == typeof(Layout) || propertyType == typeof(SimpleLayout))
            {
                newValue = new SimpleLayout(value, configurationItemFactory);
                return true;
            }

            if (propertyType == typeof(ConditionExpression))
            {
                newValue = ConditionParser.ParseExpression(value, configurationItemFactory);
                return true;
            }

            newValue = null;
            return false;
        }

        private static bool TryGetEnumValue(Type resultType, string value, out object result)
        {
            if (!resultType.IsEnum)
            {
                result = null;
                return false;
            }

            if (resultType.IsDefined(typeof(FlagsAttribute), false))
            {
                ulong union = 0;

                foreach (string v in value.Split(','))
                {
                    FieldInfo enumField = resultType.GetField(v.Trim(), BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                    if (enumField == null)
                    {
                        throw new Exception("Invalid enumeration value '" + value + "'.");
                    }

                    union |= Convert.ToUInt64(enumField.GetValue(null), CultureInfo.InvariantCulture);
                }

                result = Convert.ChangeType(union, Enum.GetUnderlyingType(resultType), CultureInfo.InvariantCulture);
                result = Enum.ToObject(resultType, result);

                return true;
            }
            var enumField2 = resultType.GetField(value, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
            if (enumField2 == null)
            {
                throw new Exception("Invalid enumeration value '" + value + "'.");
            }

            result = enumField2.GetValue(null);
            return true;
        }

        private static bool TrySpecialConversion(Type type, string value, out object newValue)
        {
            if (type == typeof(Uri))
            {
                newValue = new Uri(value, UriKind.RelativeOrAbsolute);
                return true;
            }

            if (type == typeof(Encoding))
            {
                newValue = Encoding.GetEncoding(value);
                return true;
            }

            if (type == typeof(CultureInfo))
            {
                newValue = new CultureInfo(value);
                return true;
            }

            if (type == typeof(Type))
            {
                newValue = Type.GetType(value, true);
                return true;
            }

            newValue = null;
            return false;
        }

        private static bool TryGetPropertyInfo(Type targetType, string propertyName, out PropertyInfo result)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo propInfo = targetType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    result = propInfo;
                    return true;
                }
            }

            lock (parameterInfoCache)
            {
                Dictionary<string, PropertyInfo> cache;

                if (!parameterInfoCache.TryGetValue(targetType, out cache))
                {
                    cache = BuildPropertyInfoDictionary(targetType);
                    parameterInfoCache[targetType] = cache;
                }

                return cache.TryGetValue(propertyName, out result);
            }
        }

        private static Dictionary<string, PropertyInfo> BuildPropertyInfoDictionary(Type t)
        {
            var retVal = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyInfo propInfo in GetAllReadableProperties(t))
            {
                var arrayParameterAttribute = (ArrayParameterAttribute)Attribute.GetCustomAttribute(propInfo, typeof(ArrayParameterAttribute));

                if (arrayParameterAttribute != null)
                {
                    retVal[arrayParameterAttribute.ElementName] = propInfo;
                }
                else
                {
                    retVal[propInfo.Name] = propInfo;
                }

                if (propInfo.IsDefined(typeof(DefaultParameterAttribute), false))
                {
                    retVal[string.Empty] = propInfo;
                }
            }

            return retVal;
        }

        #endregion
    }
}
