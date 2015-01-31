using System;

namespace SharpLib
{
    /// <summary>
    /// Класс расширения "Float"
    /// </summary>
    public static class ExtensionFloat
    {
        #region Константы

        public const float EPSILON = 0.0001f;

        #endregion

        #region Методы

        public static bool EqualEx(this float value1, float value2, float epsilon = EPSILON)
        {
            return Math.Abs(value1 - value2) < epsilon;
        }

        public static bool EqualZeroEx(this float value1)
        {
            return value1.EqualEx(0);
        }

        #endregion
    }
}