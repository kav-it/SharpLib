using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SharpLib.Wpf
{
    /// <summary>
    /// Расширение элемента меню
    /// </summary>
    public sealed class MenuItemBase : MenuItem
    {
        #region Конструктор

        public MenuItemBase(string title, RoutedEventHandler click, BitmapSource icon, object tag)
        {
            Header = title;
            if (click != null)
            {
                Click += click;
            }
            Tag = tag;

            if (icon != null)
            {
                Icon = new Image { Source = icon };
            }
        }

        public MenuItemBase(string title)
            : this(title, null, null, null)
        {
        }

        public MenuItemBase(string title, RoutedEventHandler click)
            : this(title, click, null, null)
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