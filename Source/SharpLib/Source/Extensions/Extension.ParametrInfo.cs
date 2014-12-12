using System.Reflection;

namespace SharpLib
{
    public static class ExtensionParametrInfo
    {
        public static bool HasDefaultValueEx(this ParameterInfo self)
        {
            if (self.IsOptional && (self.Attributes.IsFlagSet(ParameterAttributes.HasDefault)))
            {
                return true;
            }

            return false;
        }
    }
}