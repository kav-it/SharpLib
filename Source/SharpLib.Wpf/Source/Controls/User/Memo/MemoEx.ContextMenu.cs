using System;
using System.Windows;
using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    internal class MemoExContextMenu : ContextMenuEx
    {
        #region Поля

        private readonly MemoEx _memo;

        #endregion

        #region Конструктор

        public MemoExContextMenu(MemoEx memo)
        {
            _memo = memo;

            var itemClear = new MenuItemEx("Очистить", OnClear, null);
            var itemSelectAll = new MenuItemEx("Выделить все", OnSelectAll, null);
            var itemAutoScroll = new MenuItemEx("Автоскролл", OnAutoScroll, null);
            itemAutoScroll.IsCheckable = true;
            itemAutoScroll.IsChecked = true;

            Add(itemClear);
            Add(itemSelectAll);
            AddSeparator();
            Add(itemAutoScroll);
        }

        #endregion

        #region Методы

        private void OnClear(Object sender, RoutedEventArgs e)
        {
            _memo.Clear();
        }

        private void OnSelectAll(Object sender, RoutedEventArgs e)
        {
            _memo.SelectAll();
        }

        private void OnAutoScroll(Object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (item != null)
            {
                _memo.AutoScroll = item.IsChecked;
            }
        }

        #endregion
    }
}