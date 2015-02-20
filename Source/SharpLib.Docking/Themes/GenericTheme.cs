using System;

namespace SharpLib.Wpf.Docking.Themes
{
    public class GenericTheme : Theme
    {
        #region Методы

        public override Uri GetResourceUri()
        {
            return new Uri("/Xceed.Wpf.AvalonDock;component/Themes/generic.xaml", UriKind.Relative);
        }

        #endregion
    }
}