using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    internal static class TreeListExExtensionMethods
    {
        #region Ìועמהû

        public static T FindAncestor<T>(this DependencyObject d) where T : class
        {
            return AncestorsAndSelf(d).OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject d)
        {
            while (d != null)
            {
                yield return d;
                d = VisualTreeHelper.GetParent(d);
            }
        }

        public static void AddOnce(this IList list, object item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        #endregion
    }
}