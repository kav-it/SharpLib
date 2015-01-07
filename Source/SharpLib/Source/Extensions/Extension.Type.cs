using System;

namespace SharpLib
{
    public static class ExtensionType
    {
        /// <summary>
        /// Проверка, что тип является nullable 
        /// </summary>
        /// <remarks>
        /// DateTime? 
        /// int?
        /// class (any)
        /// </remarks>
        public static bool IsNullableEx(this Type self)
        {
            if (Nullable.GetUnderlyingType(self) != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверка, что тип реализует интерфейс
        /// </summary>
        public static bool IsInterfaceImplEx(this Type self, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(self);
        }
    }
}