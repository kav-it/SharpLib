using System;
using System.Windows;

namespace SharpLib.Docking.Themes
{
    public abstract class Theme : DependencyObject
    {
        #region Методы

        public abstract Uri GetResourceUri();

        #endregion
    }
}