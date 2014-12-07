using System;
using System.Globalization;
using System.Linq;

namespace SharpLib.Json
{
    internal static class MiscellaneousUtils
    {
        #region Методы

        public static bool ValueEquals(object objA, object objB)
        {
            if (objA == null && objB == null)
            {
                return true;
            }
            if (objA != null && objB == null)
            {
                return false;
            }
            if (objA == null)
            {
                return false;
            }

            if (objA.GetType() != objB.GetType())
            {
                if (ConvertUtils.IsInteger(objA) && ConvertUtils.IsInteger(objB))
                {
                    return Convert.ToDecimal(objA, CultureInfo.CurrentCulture).Equals(Convert.ToDecimal(objB, CultureInfo.CurrentCulture));
                }
                if ((objA is double || objA is float || objA is decimal) && (objB is double || objB is float || objB is decimal))
                {
                    return MathUtils.ApproxEquals(Convert.ToDouble(objA, CultureInfo.CurrentCulture), Convert.ToDouble(objB, CultureInfo.CurrentCulture));
                }
                return false;
            }

            return objA.Equals(objB);
        }

        public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string paramName, object actualValue, string message)
        {
            string newMessage = message + Environment.NewLine + @"Actual value was {0}.".FormatWith(CultureInfo.InvariantCulture, actualValue);

            return new ArgumentOutOfRangeException(paramName, newMessage);
        }

        public static string ToString(object value)
        {
            if (value == null)
            {
                return "{null}";
            }

            return (value is string) ? @"""" + value + @"""" : value.ToString();
        }

        public static int ByteArrayCompare(byte[] a1, byte[] a2)
        {
            int lengthCompare = a1.Length.CompareTo(a2.Length);

            return lengthCompare != 0 
                ? lengthCompare 
                : a1.Select((t, i) => t.CompareTo(a2[i])).FirstOrDefault(valueCompare => valueCompare != 0);
        }

        public static string GetPrefix(string qualifiedName)
        {
            string prefix;
            string localName;
            GetQualifiedNameParts(qualifiedName, out prefix, out localName);

            return prefix;
        }

        public static string GetLocalName(string qualifiedName)
        {
            string prefix;
            string localName;
            GetQualifiedNameParts(qualifiedName, out prefix, out localName);

            return localName;
        }

        public static void GetQualifiedNameParts(string qualifiedName, out string prefix, out string localName)
        {
            int colonPosition = qualifiedName.IndexOf(':');

            if ((colonPosition == -1 || colonPosition == 0) || (qualifiedName.Length - 1) == colonPosition)
            {
                prefix = null;
                localName = qualifiedName;
            }
            else
            {
                prefix = qualifiedName.Substring(0, colonPosition);
                localName = qualifiedName.Substring(colonPosition + 1);
            }
        }

        internal static string FormatValueForPrint(object value)
        {
            if (value == null)
            {
                return "{null}";
            }

            if (value is string)
            {
                return @"""" + value + @"""";
            }

            return value.ToString();
        }

        #endregion
    }
}