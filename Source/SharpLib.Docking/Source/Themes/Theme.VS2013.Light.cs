using System;

namespace SharpLib.Docking
{
    public class Vs2013LightTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("/SharpLib.Docking;component/Source/Themes/VS2013/Theme.VS2013.Light.xaml", UriKind.Relative);
        }
    }
}
