using System;

namespace SharpLib.Docking.Themes
{
    public class Vs2013DarkTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("/SharpLib.Docking;component/Source/Themes/VS2013/Theme.VS2013.Dark.xaml", UriKind.Relative);
        }
    }
}