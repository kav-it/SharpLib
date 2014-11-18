using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SharpLib
{
    public static class ExtensionEnum
    {
        #region Методы

        public static string ToStringEx(this Enum value)
        {
            var info = value.GetType().GetField(value.ToString());
            var attributes =
                (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

            var text = (attributes.Length > 0) ? attributes[0].Description : value.ToString();

            return text;
        }

        public static List<Object> GetValuesEx(Type typValue)
        {
            var list = new List<Object>();

            if (typValue.IsEnum)
            {
                list.AddRange(Enum.GetValues(typValue).Cast<object>());
            }

            return list;
        }

        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);

            return (lValue & lFlag) != 0;
        }

        #endregion
    }
}