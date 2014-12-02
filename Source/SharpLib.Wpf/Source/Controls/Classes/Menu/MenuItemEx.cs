using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class MenuItemEx : MenuItem
    {
        #region Конструктор

        public MenuItemEx(string title, RoutedEventHandler click, object tag)
        {
            Header = title;
            if (click != null)
            {
                Click += click;
            }
            Tag = tag;
        }

        public MenuItemEx(string title)
            : this(title, null, null)
        {
        }

        public MenuItemEx(string title, RoutedEventHandler click)
            : this(title, click, null)
        {
        }

        #endregion

        #region Методы

        public void Add(MenuItem item)
        {
            Items.Add(item);
        }

        public void AddSeparator()
        {
            Items.Add(new Separator());
        }

        public void Clear()
        {
            Items.Clear();
        }

        #endregion
    }
}