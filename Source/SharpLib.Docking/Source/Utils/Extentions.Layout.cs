using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpLib.Docking
{
    public static class ExtensionsLayout
    {
        #region Методы

        public static IEnumerable<ILayoutElement> Descendents(this ILayoutElement element)
        {
            var container = element as ILayoutContainer;
            if (container != null)
            {
                foreach (var childElement in container.Children)
                {
                    yield return childElement;

                    var childs = childElement.Descendents();
                    foreach (var childChildElement in childs)
                    {
                        yield return childChildElement;
                    }
                }
            }
        }

        public static T FindParent<T>(this ILayoutElement element)
        {
            var parent = element.Parent;
            while (parent != null &&
                   !(parent is T))
            {
                parent = parent.Parent;
            }

            return (T)parent;
        }

        public static ILayoutRoot GetRoot(this ILayoutElement element)
        {
            if (element is ILayoutRoot)
            {
                return element as ILayoutRoot;
            }

            var parent = element.Parent;
            while (parent != null &&
                   !(parent is ILayoutRoot))
            {
                parent = parent.Parent;
            }

            return (ILayoutRoot)parent;
        }

        public static bool ContainsChildOfType<T>(this ILayoutContainer element)
        {
            return element.Descendents().OfType<T>().Any();
        }

        public static bool ContainsChildOfType<T, TS>(this ILayoutContainer container)
        {
            return container.Descendents().Any(childElement => childElement is T || childElement is TS);
        }

        public static bool IsOfType<T, TS>(this ILayoutContainer container)
        {
            return container is T || container is TS;
        }

        public static AnchorSide GetSide(this ILayoutElement element)
        {
            var parentContainer = element.Parent as ILayoutOrientableGroup;
            if (parentContainer != null)
            {
                if (!parentContainer.ContainsChildOfType<LayoutDocumentPaneGroup, LayoutDocumentPane>())
                {
                    return GetSide(parentContainer);
                }

                foreach (var childElement in parentContainer.Children)
                {
                    if (childElement == element ||
                        childElement.Descendents().Contains(element))
                    {
                        return parentContainer.Orientation == System.Windows.Controls.Orientation.Horizontal
                            ? AnchorSide.Left
                            : AnchorSide.Top;
                    }

                    var childElementAsContainer = childElement as ILayoutContainer;
                    if (childElementAsContainer != null &&
                        (childElementAsContainer.IsOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>() ||
                         childElementAsContainer.ContainsChildOfType<LayoutDocumentPane, LayoutDocumentPaneGroup>()))
                    {
                        return parentContainer.Orientation == System.Windows.Controls.Orientation.Horizontal
                            ? AnchorSide.Right
                            : AnchorSide.Bottom;
                    }
                }
            }

            Debug.Fail("Unable to find the side for an element, possible layout problem!");
            return AnchorSide.Right;
        }

        internal static void KeepInsideNearestMonitor(this ILayoutElementForFloatingWindow paneInsideFloatingWindow)
        {
            var r = new Win32Helper.RECT();
            r.Left = (int)paneInsideFloatingWindow.FloatingLeft;
            r.Top = (int)paneInsideFloatingWindow.FloatingTop;
            r.Bottom = r.Top + (int)paneInsideFloatingWindow.FloatingHeight;
            r.Right = r.Left + (int)paneInsideFloatingWindow.FloatingWidth;

            uint monitorDefaulttonearest = 0x00000002;
            uint monitorDefaulttonull = 0x00000000;

            var monitor = Win32Helper.MonitorFromRect(ref r, monitorDefaulttonull);
            if (monitor == System.IntPtr.Zero)
            {
                var nearestmonitor = Win32Helper.MonitorFromRect(ref r, monitorDefaulttonearest);
                if (nearestmonitor != System.IntPtr.Zero)
                {
                    var monitorInfo = new Win32Helper.MonitorInfo();
                    monitorInfo.Size = Marshal.SizeOf(monitorInfo);
                    Win32Helper.GetMonitorInfo(nearestmonitor, monitorInfo);

                    if (paneInsideFloatingWindow.FloatingLeft < monitorInfo.Work.Left)
                    {
                        paneInsideFloatingWindow.FloatingLeft = monitorInfo.Work.Left + 10;
                    }

                    if (paneInsideFloatingWindow.FloatingLeft + paneInsideFloatingWindow.FloatingWidth > monitorInfo.Work.Right)
                    {
                        paneInsideFloatingWindow.FloatingLeft = monitorInfo.Work.Right - (paneInsideFloatingWindow.FloatingWidth + 10);
                    }

                    if (paneInsideFloatingWindow.FloatingTop < monitorInfo.Work.Top)
                    {
                        paneInsideFloatingWindow.FloatingTop = monitorInfo.Work.Top + 10;
                    }

                    if (paneInsideFloatingWindow.FloatingTop + paneInsideFloatingWindow.FloatingHeight > monitorInfo.Work.Bottom)
                    {
                        paneInsideFloatingWindow.FloatingTop = monitorInfo.Work.Bottom - (paneInsideFloatingWindow.FloatingHeight + 10);
                    }
                }
            }
        }

        #endregion
    }
}