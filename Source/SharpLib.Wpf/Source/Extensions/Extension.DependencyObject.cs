
using System.ComponentModel;
using System.Windows;

namespace SharpLib.Wpf
{
    public static class DependencyObjectExtension
    {
        public static bool IsDesignMode(this DependencyObject value)
        {
            return DesignerProperties.GetIsInDesignMode(value);
        }
    }
}