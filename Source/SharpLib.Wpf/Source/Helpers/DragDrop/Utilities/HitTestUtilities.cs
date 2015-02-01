using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpLib.Wpf.Dragging.Utilities
{
    internal static class HitTestUtilities
    {
        #region Методы

        public static bool HitTest4Type<T>(object sender, Point elementPosition) where T : UIElement
        {
            var uiElement = GetHitTestElement4Type<T>(sender, elementPosition);
            return uiElement != null && uiElement.Visibility == Visibility.Visible;
        }

        private static T GetHitTestElement4Type<T>(object sender, Point elementPosition) where T : UIElement
        {
            var visual = sender as Visual;
            if (visual == null)
            {
                return null;
            }
            var hit = VisualTreeHelper.HitTest(visual, elementPosition);
            if (hit == null)
            {
                return null;
            }
            var uiElement = hit.VisualHit.GetVisualAncestor<T>();
            return uiElement;
        }

        public static bool HitTest4GridViewColumnHeader(object sender, Point elementPosition)
        {
            if (sender is ListView)
            {
                var columnHeader = GetHitTestElement4Type<GridViewColumnHeader>(sender, elementPosition);
                if (columnHeader != null && (columnHeader.Role == GridViewColumnHeaderRole.Floating || columnHeader.Visibility == Visibility.Visible))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HitTest4DataGridTypes(object sender, Point elementPosition)
        {
            if (sender is DataGrid)
            {
                var columnHeader = GetHitTestElement4Type<DataGridColumnHeader>(sender, elementPosition);
                if (columnHeader != null && columnHeader.Visibility == Visibility.Visible)
                {
                    return true;
                }

                var rowHeader = GetHitTestElement4Type<DataGridRowHeader>(sender, elementPosition);
                if (rowHeader != null && rowHeader.Visibility == Visibility.Visible)
                {
                    return true;
                }

                var dataRow = GetHitTestElement4Type<DataGridRow>(sender, elementPosition);
                return dataRow == null;
            }
            return false;
        }

        public static bool HitTest4DataGridTypesOnDragOver(object sender, Point elementPosition)
        {
            if (sender is DataGrid)
            {
                var columnHeader = GetHitTestElement4Type<DataGridColumnHeader>(sender, elementPosition);
                if (columnHeader != null && columnHeader.Visibility == Visibility.Visible)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNotPartOfSender(object sender, MouseButtonEventArgs e)
        {
            var visual = e.OriginalSource as Visual;
            if (visual == null)
            {
                return false;
            }
            var hit = VisualTreeHelper.HitTest(visual, e.GetPosition((IInputElement)visual));

            if (hit == null)
            {
                return false;
            }
            var depObj = e.OriginalSource as DependencyObject;
            if (depObj == null)
            {
                return false;
            }
            var item = VisualTreeHelper.GetParent(depObj.FindVisualTreeRoot());

            while (item != null && !Equals(item, sender))
            {
                item = VisualTreeHelper.GetParent(item);
            }
            return !Equals(item, sender);
        }

        #endregion
    }
}