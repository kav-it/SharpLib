using System;

namespace SharpLib.Docking
{
    public class MetroTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("/SharpLib.Docking;component/Source/Themes/Metro/Theme.Metro.xaml", UriKind.Relative);
        }
    }
}