using System;
using System.Windows;
using System.Windows.Media;

namespace SharpLib.Wpf
{
    public static class VisualTree
    {
        public static T FindUp<T>(Object element) where T : DependencyObject
        {
            if (element is DependencyObject)
            {
                DependencyObject current = element as DependencyObject;

                current = VisualTreeHelper.GetParent(current);

                while (current != null)
                {
                    if (current is T)
                        return (T)current;
                    current = VisualTreeHelper.GetParent(current);
                }
            }
            return null;
        }

        public static T FindUp<T>(DependencyObject current, T lookupItem) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T && current == lookupItem)
                    return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        public static T FindUp<T>(DependencyObject current, string parentName) where T : DependencyObject
        {
            while (current != null)
            {
                if (!string.IsNullOrEmpty(parentName))
                {
                    var frameworkElement = current as FrameworkElement;
                    if (current is T && frameworkElement != null && frameworkElement.Name == parentName)
                        return (T)current;
                }
                else if (current is T)
                    return (T)current;
                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        private static DependencyObject FindDownRecurse(DependencyObject parent, Type searchType, String searchName)
        {
            if (parent == null) return null;
            DependencyObject foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                Type childType = child.GetType();

                if (childType != searchType)
                {
                    // Типы не совпадают: Продолжение поиска
                    foundChild = FindDownRecurse(child, searchType, searchName);

                    // Элемент найден
                    if (foundChild != null) break;
                }
                else if (searchName.IsValid())
                {
                    var frameworkElement = child as FrameworkElement;
                    // Имя не указано для поиска
                    if (frameworkElement != null && frameworkElement.Name == searchName)
                    {
                        // Совпадение имени: Элемент найден
                        foundChild = child;
                        break;
                    }
                    else
                    {
                        // Продолжение поиска
                        foundChild = FindDownRecurse(child, searchType, searchName);

                        // Элемент не найден
                        if (foundChild != null) break;
                    }
                }
                else
                {
                    // Элемент найден
                    foundChild = child;
                    break;
                }
            }

            return foundChild;
        }

        public static DependencyObject FindDown(Type searchType, String searchName, DependencyObject root = null)
        {
            if (root == null)
                root = Application.Current.MainWindow;

            DependencyObject result = FindDownRecurse(root, searchType, searchName);

            return result;
        }

        private static void PrintVisualRecurse(DependencyObject current, ref int ident, ref String result)
        {
            if (current == null) return;

            String name = "";

            if (current is FrameworkElement)
                name = (current as FrameworkElement).Name;

            for (int i = 0; i < ident; i++)
                result += "    ";

            result += String.Format("'{0}' ({1})\r\n", name, current.GetType());

            // Увеличение отступа
            ++ident;
            // ===========================
            //
            // Обход дочерних элементов
            int childrenCount = VisualTreeHelper.GetChildrenCount(current);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(current, i);
                PrintVisualRecurse(child, ref ident, ref result);
            }
            //
            // ==========================
            // Уменьшение отступа
            --ident;
        }

        public static void PrintVisual(DependencyObject root = null)
        {
            int ident = 0;

            if (root == null)
                root = Application.Current.MainWindow;

            String result = String.Format("\r\n----------- Print visual tree '{0}' -----------\r\n", root);

            PrintVisualRecurse(root, ref ident, ref result);
        }

        private static void PrintLogicalRecurse(Object current, ref int ident, ref String result)
        {
            if (current == null) return;

            String name = "";

            if (current is FrameworkElement)
                name = (current as FrameworkElement).Name;

            for (int i = 0; i < ident; i++)
                result += "    ";

            result += String.Format("'{0}' ({1})\r\n", name, current.GetType());

            ++ident;

            // ==================================
            // 
            if (current is FrameworkElement)
            {
                var children = LogicalTreeHelper.GetChildren(current as FrameworkElement);
                foreach (Object child in children)
                    PrintLogicalRecurse(child, ref ident, ref result);
            }
            // 
            // =================================

            --ident;
        }

        public static void PrintLogical(Object root = null)
        {
            int ident = 0;

            if (root == null)
                root = Application.Current.MainWindow;

            String result = String.Format("\r\n----------- Print logical tree '{0}' -----------\r\n", root);

            PrintLogicalRecurse(root, ref ident, ref result);
        }
    }
}
