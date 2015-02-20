using System;

namespace SharpLib
{
    /// <summary>
    /// Класс расширения "Double"
    /// </summary>
    public static class ExtensionDouble
    {
        #region Константы

        public const double EPSILON = 0.0001;

        #endregion

        #region Методы

        public static bool EqualEx(this double value1, double value2, double epsilon = EPSILON)
        {
            return Math.Abs(value1 - value2) < epsilon;
        }

        public static bool NotEqualEx(this double value1, double value2, double epsilon = EPSILON)
        {
            return value1.EqualEx(value2, epsilon) == false;
        }

        public static bool EqualZeroEx(this double value1)
        {
            return value1.EqualEx(0);
        }

        #endregion
    }
}