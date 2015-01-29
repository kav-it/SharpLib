
using System.Windows;

namespace SharpLib.Wpf
{
    public static class ExtensionSize
    {
        public static bool EqualsEx(this Size self, Size value)
        {
            return self.Width.EqualEx(value.Width) && self.Height.EqualEx(value.Height);
        }
    }
}