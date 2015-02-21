using System;

namespace SharpLib.Docking.Themes
{
    public class GenericTheme : Theme
    {
        #region Методы

        public override Uri GetResourceUri()
        {
            return new Uri("/SharpLib.Docking;component/Themes/generic.xaml", UriKind.Relative);
        }

        #endregion
    }
}