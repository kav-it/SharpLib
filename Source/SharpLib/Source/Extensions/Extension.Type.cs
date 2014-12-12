using System;

namespace SharpLib
{
    public static class ExtensionType
    {
        public static bool IsNullableEx(this Type self)
        {
            if (Nullable.GetUnderlyingType(self) != null)
            {
                return true;
            }

            return false;
        }
    }
}