using System;

namespace SharpLib.Docking.Themes
{
    public class Vs2010Theme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("/SharpLib.Docking;component/Source/Themes/VS2010/Theme.VS2010.xaml", UriKind.Relative);
        }
    }
}
