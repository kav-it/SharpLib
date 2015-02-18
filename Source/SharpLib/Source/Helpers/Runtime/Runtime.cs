using System;

namespace SharpLib
{
    public static class Runtime
    {
        /// <summary>
        /// Проверка выполнения условия
        /// </summary>
        public static void Check(bool expr)
        {
            if (expr == false)
            {
                throw new Exception();
            }
        }

        public static void Check<T>(bool expr) where T : Exception, new()
        {
            if (expr == false)
            {
                throw new T();
            }
        }
    }
}
