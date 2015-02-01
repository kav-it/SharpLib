using System.ComponentModel;
using System.Windows;

namespace SharpLib.Wpf
{
    public static class VsHelper
    {
        public static bool IsDesignMode()
        {
            var a = (DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)));
            if ((bool)a.DefaultValue)
            {
                return true;
            }

            return false;
        }
    }
}
