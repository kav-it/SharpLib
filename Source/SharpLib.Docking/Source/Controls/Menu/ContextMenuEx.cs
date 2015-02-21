using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SharpLib.Docking.Controls
{
    public class ContextMenuEx : ContextMenu
    {
        #region Методы

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItemEx();
        }

        protected override void OnOpened(RoutedEventArgs e)
        {
            var expression = BindingOperations.GetBindingExpression(this, ItemsSourceProperty);
            if (expression != null)
            {
                expression.UpdateTarget();
            }

            base.OnOpened(e);
        }

        #endregion
    }
}