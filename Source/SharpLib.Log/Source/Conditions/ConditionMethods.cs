using System;
using System.Runtime.InteropServices;

namespace NLog.Conditions
{
    [ConditionMethods]
    public static class ConditionMethods
    {
        #region Ìועמהû

        [ConditionMethod("equals")]
        public static bool Equals2(object firstValue, object secondValue)
        {
            return firstValue.Equals(secondValue);
        }

        [ConditionMethod("strequals")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Not called directly, only ever Invoked.")]
        public static bool Equals2(string firstValue, string secondValue, [Optional, DefaultParameterValue(false)] bool ignoreCase)
        {
            bool ic = ignoreCase;
            return firstValue.Equals(secondValue, ic ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [ConditionMethod("contains")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Not called directly, only ever Invoked.")]
        public static bool Contains(string haystack, string needle, [Optional, DefaultParameterValue(true)] bool ignoreCase)
        {
            bool ic = ignoreCase;
            return haystack.IndexOf(needle, ic ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;
        }

        [ConditionMethod("starts-with")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Not called directly, only ever Invoked.")]
        public static bool StartsWith(string haystack, string needle, [Optional, DefaultParameterValue(true)] bool ignoreCase)
        {
            bool ic = ignoreCase;
            return haystack.StartsWith(needle, ic ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [ConditionMethod("ends-with")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Not called directly, only ever Invoked.")]
        public static bool EndsWith(string haystack, string needle, [Optional, DefaultParameterValue(true)] bool ignoreCase)
        {
            bool ic = ignoreCase;
            return haystack.EndsWith(needle, ic ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [ConditionMethod("length")]
        public static int Length(string text)
        {
            return text.Length;
        }

        #endregion
    }
}