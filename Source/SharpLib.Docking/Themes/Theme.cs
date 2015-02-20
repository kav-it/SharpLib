using System;
using System.Windows;

namespace SharpLib.Wpf.Docking.Themes
{
    public abstract class Theme : DependencyObject
    {
        #region Методы

        public abstract Uri GetResourceUri();

        #endregion
    }
}