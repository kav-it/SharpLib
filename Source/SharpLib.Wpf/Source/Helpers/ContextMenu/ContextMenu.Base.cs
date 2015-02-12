using System.Windows.Controls;

namespace SharpLib.Wpf
{
    /// <summary>
    /// Расширение класса контектсного меню
    /// </summary>
    public class ContextMenuBase : ContextMenu
    {
        #region Методы

        public void Add(MenuItem item)
        {
            Items.Add(item);
        }

        public void AddSeparator()
        {
            Items.Add(new Separator());
        }

        public void Show()
        {
            IsOpen = true;
        }

        #endregion
    }
}