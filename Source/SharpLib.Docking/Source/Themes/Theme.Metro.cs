using System;

namespace SharpLib.Docking.Themes
{
    public class MetroTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("/SharpLib.Docking;component/Themes/metro.xaml", UriKind.Relative);
        }
    }
}