using System.IO;
using System.Linq;
using System.Windows;

namespace SharpLib.Wpf.Dialogs
{
    internal class DialogCustomContextMenu : ContextMenuBase
    {
        #region Поля

        private readonly MenuItemBase _createItem;

        private readonly MenuItemBase _deleteItem;

        private readonly CustomDialog _parent;

        private readonly MenuItemBase _renameItem;

        #endregion

        #region Конструктор

        public DialogCustomContextMenu(CustomDialog parent)
        {
            _parent = parent;

            _createItem = new MenuItemBase("Create folder...", CreateFolder);
            _deleteItem = new MenuItemBase("Delete...", DeleteClick);
            _renameItem = new MenuItemBase("Rename...", RenameClick);

            Add(_createItem);
            Add(_renameItem);
            AddSeparator();
            Add(_deleteItem);
        }

        #endregion

        #region Методы

        private void CreateFolder(object sender, RoutedEventArgs args)
        {
            var root = _parent.LocationDir;
            var result = WindowPromt.ShowText(string.Empty, string.Empty, "Enter name folder", true);

            if (result.IsValid())
            {
                var location = Path.Combine(root, result);
                if (Directory.Exists(location) == false)
                {
                    Files.CreateDirectory(location);
                    _parent.UpdateEntries();    
                }
            }
        }

        private void DeleteClick(object sender, RoutedEventArgs args)
        {
            var items = _parent.SelectedItems.Where(x => x.IsRoot == false);

            foreach (var item in items)
            {
                Files.Delete(item.Location);
                _parent.UpdateEntries();    
            }
        }

        private void RenameClick(object sender, RoutedEventArgs args)
        {
            var item = _parent.SelectedItems.FirstOrDefault();

            if (item == null)
            {
                return;
            }

            var newName = WindowPromt.ShowText("Rename", item.Name, "Enter new name", true);

            if (newName.IsValid())
            {
                Files.Rename(item.Location, newName);
                _parent.UpdateEntries();    
            }
        }

        internal void UpdateStates()
        {
            var items = _parent.SelectedItems;
            var item = items.FirstOrDefault();

            _createItem.IsEnabled = true;
            _renameItem.IsEnabled = items.Count == 1 && item != null && item.IsRoot == false;
            _deleteItem.IsEnabled = items.Any() && items.All(x => x.IsRoot == false);
        }

        #endregion
    }
}