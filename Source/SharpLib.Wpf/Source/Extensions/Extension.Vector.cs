
using System.Windows;

namespace SharpLib.Wpf
{
    public static class ExtensionVector
    {
        public static bool EqualsEx(this Vector self, Vector value)
        {
            return self.X.EqualEx(value.X) && self.Y.EqualEx(value.Y);
        }
    }
}