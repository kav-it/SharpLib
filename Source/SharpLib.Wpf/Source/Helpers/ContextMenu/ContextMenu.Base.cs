using System.Windows.Controls;

namespace SharpLib.Wpf
{
    /// <summary>
    /// Расширение класса контектсного меню
    /// </summary>
    public class ContextMenuBase : ContextMenu
    {
        #region Методы

        /// <summary>
        /// Добавление пункта меню
        /// </summary>
        /// <param name="item"></param>
        public void Add(MenuItem item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Добавление разделителя
        /// </summary>
        public void AddSeparator()
        {
            Items.Add(new Separator());
        }

        /// <summary>
        /// Показ контекстного меню
        /// </summary>
        public void Show()
        {
            IsOpen = true;
        }

        /// <summary>
        /// Очистка всех элементов меню
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        #endregion
    }
}