
using System.ComponentModel;
using System.Windows;

namespace SharpLibWpf
{
    public static class DependencyObjectExtension
    {
        public static bool IsDesignMode(this DependencyObject value)
        {
            return DesignerProperties.GetIsInDesignMode(value);
        }
    }
}