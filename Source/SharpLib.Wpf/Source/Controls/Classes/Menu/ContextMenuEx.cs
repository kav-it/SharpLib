using System.Windows.Controls;

namespace SharpLib.Wpf.Controls
{
    public class ContextMenuEx : ContextMenu
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