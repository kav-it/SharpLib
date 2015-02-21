using System;

namespace SharpLib.Docking.Themes
{
    public class Vs2008Theme : Theme
    {
        #region Методы

        public override Uri GetResourceUri()
        {
            return new Uri("/SharpLib.Docking;component/Source/Themes/VS2008/Theme.VS2008.xaml", UriKind.Relative);
        }

        #endregion
    }
}