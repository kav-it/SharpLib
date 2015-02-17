using System.Windows;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    public class ApplyPropertiesEventArgs : RoutedEventArgs
    {
        #region Свойства

        public BreadcrumbItem Breadcrumb { get; private set; }

        public object Item { get; private set; }

        public ImageSource Image { get; set; }

        public object Trace { get; set; }

        public string TraceValue { get; set; }

        #endregion

        #region Конструктор

        public ApplyPropertiesEventArgs(object item, BreadcrumbItem breadcrumb, RoutedEvent routedEvent)
            : base(routedEvent)
        {
            Item = item;
            Breadcrumb = breadcrumb;
        }

        #endregion
    }

    public delegate void ApplyPropertiesEventHandler(object sender, ApplyPropertiesEventArgs e);
}