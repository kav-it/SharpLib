using System.Windows;

namespace SharpLib.Wpf.Controls
{
    public class BreadcrumbItemEventArgs : RoutedEventArgs
    {
        #region Свойства

        public BreadcrumbItem Item { get; private set; }

        #endregion

        #region Конструктор

        public BreadcrumbItemEventArgs(BreadcrumbItem item, RoutedEvent routedEvent)
            : base(routedEvent)
        {
            Item = item;
        }

        #endregion
    }

    public delegate void BreadcrumbItemEventHandler(object sender, BreadcrumbItemEventArgs e);
}