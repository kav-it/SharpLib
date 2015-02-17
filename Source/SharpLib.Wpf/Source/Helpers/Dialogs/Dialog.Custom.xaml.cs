using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SharpLib.Wpf.Dialogs
{
    /// <summary>
    /// Базовый класс выбора файлов, директорий
    /// </summary>
    internal partial class CustomDialog
    {
        #region Константы

        internal const string ROOT_DIR_NAME = "..";

        #endregion

        #region Поля

        /// <summary>
        /// Последняя стартовая директория
        /// </summary>
        private static string _lastDirectory;

        /// <summary>
        /// Текущий список файлов/директорий
        /// </summary>
        private readonly ObservableCollection<DialogCustomEntryModel> _entries;

        /// <summary>
        /// Элементы "Избранное"
        /// </summary>
        private readonly ObservableCollection<DialogCustomPlaceModel> _places;

        /// <summary>
        /// Элементы "Фаловое дерево"
        /// </summary>
        private readonly ObservableCollection<DialogCustomTreeItemModel> _treeItems;

        /// <summary>
        /// История переходов
        /// </summary>
        private DialogCustomHistory _history;

        /// <summary>
        /// Текущая директория
        /// </summary>
        private string _locationDir;

        #endregion

        #region Свойства

        /// <summary>
        /// Начальная директория
        /// </summary>
        public string StartLocation { get; private set; }

        /// <summary>
        /// Выбор только директорий
        /// </summary>
        public DialogCustomSelectMode SelectMode { get; private set; }

        /// <summary>
        /// Выделенные в диалоге элементы
        /// </summary>
        public List<DialogCustomEntryModel> SelectedItems
        {
            get { return PART_listView.SelectedItems.Cast<DialogCustomEntryModel>().ToList(); }
        }

        /// <summary>
        /// Выбранные файлы
        /// </summary>
        public List<string> SelectedFiles
        {
            get { return SelectedItems.Where(x => !x.IsDirectory).Select(x => x.Location).ToList(); }
        }

        /// <summary>
        /// Выбранные папки
        /// </summary>
        public List<string> SelectedFolders
        {
            get { return SelectedItems.Where(x => x.IsDirectory).Select(x => x.Location).ToList(); }
        }

        #endregion

        #region Конструктор

        static CustomDialog()
        {
            _lastDirectory = Enviroments.Folders.Desktop;
        }

        internal CustomDialog()
        {
            InitializeComponent();

            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Owner = Gui.GetActiveWindow();
            Closed += WindowOnClose;

            _places = new ObservableCollection<DialogCustomPlaceModel>();
            _entries = new ObservableCollection<DialogCustomEntryModel>();
            _treeItems = new ObservableCollection<DialogCustomTreeItemModel>();

            PART_listBox.Items.Clear();
            PART_listBox.ItemsSource = _places;
            PART_listView.Items.Clear();
            PART_listView.ItemsSource = _entries;
            PART_treeView.Items.Clear();
            PART_treeView.ItemsSource = _treeItems;

            Loaded += CustomDialog_Loaded;
            StartLocation = Directory.Exists(_lastDirectory) ? _lastDirectory : Enviroments.Folders.Desktop;
        }

        public CustomDialog(DialogCustomSelectMode selectMode, string startLocation, string caption) : this()
        {
            if (caption.IsNotValid())
            {
                var isFile = selectMode.IsFlagSet(DialogCustomSelectMode.File);
                var isFolder = selectMode.IsFlagSet(DialogCustomSelectMode.Folder);
                var isMany = selectMode.IsFlagSet(DialogCustomSelectMode.Many);

                if (isFile && isFolder)
                {
                    caption = "Выбор файлов и/или директорий";
                }
                else if (isFile)
                {
                    caption = isMany ? "Выбор файлов" : "Выбор файла";
                }
                else if (isFolder)
                {
                    caption = isMany ? "Выбор директорий" : "Выбор директории";
                }
                else
                {
                    caption = "Выбор";
                }
            }

            Title = caption;
            if (startLocation != null)
            {
                StartLocation = startLocation;
            }
            SelectMode = selectMode;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Сохранение последней открытой директории
        /// </summary>
        private void WindowOnClose(object sender, EventArgs eventArgs)
        {
            _lastDirectory = _locationDir;
        }

        /// <summary>
        /// Загрузка диалога
        /// </summary>
        private void CustomDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Инициализация "Избранное"
            var drives = DriveInfo.GetDrives().Where(x => x.IsReady);

            _places.Clear();

            foreach (var drive in drives)
            {
                var name = string.Format("{0}: ({1})", drive.Name.Substring(0, 1), drive.VolumeLabel);
                var place = new DialogCustomPlaceModel(name, drive.Name);
                _places.Add(place);
            }
            _places.Add(new DialogCustomPlaceModel("Desktop", Enviroments.Folders.Desktop));

            // Инициализация "Файловый список"
            _entries.Clear();

            // Инициализация "Файловое дерево"
            _treeItems.Clear();

            // Установка начального диалог
            SetFirstLocation(StartLocation);
        }

        /// <summary>
        /// Установка перевого пути при открытии
        /// </summary>
        private void SetFirstLocation(string location)
        {
            _history = new DialogCustomHistory(location);
            PART_stackPanelButtons.DataContext = _history;

            SetLocation(location);
        }

        /// <summary>
        /// Смена положения (обновление файловых списков)
        /// </summary>
        private void SetLocation(string location)
        {
            var root = Files.GetDirectory(location);

            if (root.EqualsOrdinalEx(_locationDir))
            {
                // Директория не изменилась
                return;
            }

            // Установка текущей директории
            _locationDir = root;
            PART_textBox.Text = root;

            _entries.Clear();

            // Добавление директорий
            var folders = Files.GetDirectories(root, false, false).ToList();
            // Сортировка по имени
            folders.Sort(StringComparer.Ordinal);

            var parent = Files.GetDirectoryParent(root);
            if (parent.IsValid())
            {
                _entries.Add(new DialogCustomEntryModel(parent, ROOT_DIR_NAME));
            }
            foreach (var folder in folders)
            {
                _entries.Add(new DialogCustomEntryModel(folder));
            }

            // Добавление файлов
            var files = Files.GetFiles(root, false, false).ToList();
            // Сортировка файлов (по расширению + по имени)
            files.Sort(Comparers.Filename);

            foreach (var file in files)
            {
                _entries.Add(new DialogCustomEntryModel(file));
            }

            // Обновление состояний кнопки "ОК"
            UpdateButtonStates();
        }

        /// <summary>
        /// Удаление иконки приложения
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            DialogCustomHelper.RemoveIcon(this);
            DialogCustomHelper.HideMinimizeAndMaximizeButtons(this);
        }

        /// <summary>
        /// Событие "Смена выделенного элемента "Избранное""
        /// </summary>
        private void PART_listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = PART_listBox.SelectedItem as DialogCustomPlaceModel;

            if (item != null)
            {
                SetLocation(item.Location);
            }
        }

        /// <summary>
        /// Событие "DoubleClick мышью"
        /// </summary>
        private void PART_listView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = PART_listView.SelectedItem as DialogCustomEntryModel;

            if (item != null)
            {
                if (item.IsDirectory)
                {
                    // Переход в директорию
                    SetLocation(item.Location);
                    _history.Add(item.Location);
                }
                else if (SelectMode.IsFlagSet(DialogCustomSelectMode.File))
                {
                    // Выбран файл по DoubleClick
                    ButtonOkClick(this, null);
                }
            }
        }

        /// <summary>
        /// Смена выделения в списке файлов/папок
        /// </summary>
        private void PART_listView_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            UpdateButtonStates();
        }

        /// <summary>
        /// Обновление состояний кнопки "ОК" (Enable/Disable)
        /// </summary>
        private void UpdateButtonStates()
        {
            var items = SelectedItems;
            var isMany = items.Count > 1;
            var canMany = SelectMode.IsFlagSet(DialogCustomSelectMode.Many);

            // Выбран хоть один элемент
            bool isEnable = items.Any();

            if (isEnable)
            {
                // Выбрано несколько элементов, но разрешено только SingleSelect
                if (canMany == false && isMany)
                {
                    isEnable = false;
                }
            }

            if (isEnable && SelectMode.IsFlagSet(DialogCustomSelectMode.File) == false)
            {
                // Файлы не разрешены
                isEnable = items.All(x => !x.IsDirectory);
            }

            if (isEnable && SelectMode.IsFlagSet(DialogCustomSelectMode.Folder) == false)
            {
                // Директории не разрешены
                isEnable = items.All(x => x.IsDirectory && !x.IsRoot);
            }

            PART_buttonOk.IsEnabled = isEnable;
        }

        /// <summary>
        /// Событие "Переход на директорию выше"
        /// </summary>
        private void ButtonUpClick(object sender, RoutedEventArgs e)
        {
            var parentDir = Files.GetDirectoryParent(_locationDir);

            if (Directory.Exists(parentDir))
            {
                SetLocation(parentDir);
                _history.Add(parentDir);
            }
        }

        /// <summary>
        /// Событие "Переход на директорию назад"
        /// </summary>
        private void ButtonPrevClick(object sender, RoutedEventArgs e)
        {
            var location = _history.Prev();

            if (location.IsValid())
            {
                SetLocation(location);
            }
        }

        /// <summary>
        /// Событие "Переход на директорию вперед"
        /// </summary>
        private void ButtonNextClick(object sender, RoutedEventArgs e)
        {
            var location = _history.Next();

            if (location.IsValid())
            {
                SetLocation(location);
            }
        }

        /// <summary>
        /// Нажат "ОК"
        /// </summary>
        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        #endregion
    }

    #region Вложенный класс: DialogCustomEntryModel

    internal class DialogCustomEntryModel
    {
        #region Свойства

        /// <summary>
        /// Файловый путь к элементу
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Иконка файла или директории
        /// </summary>
        public BitmapSource Icon { get; private set; }

        /// <summary>
        /// Имя элемента
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Дата модификации
        /// </summary>
        public string StampText
        {
            get
            {
                var stamp = Files.IsFile(Location)
                    ? File.GetLastWriteTime(Location)
                    : Directory.GetLastAccessTime(Location);

                return string.Format("{0:dd.MM.yyyy hh:mm}", stamp);
            }
        }

        /// <summary>
        /// Размер элемента
        /// </summary>
        public string SizeText
        {
            get
            {
                if (Files.IsFile(Location))
                {
                    long size = Files.GetFileSize(Location);

                    return size.ToFileSizeEx();
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Расширение файла
        /// </summary>
        public string ExtText
        {
            get { return IsDirectory == false ? Files.GetExtension(Location) : string.Empty; }
        }

        /// <summary>
        /// Элемент директория
        /// </summary>
        internal bool IsDirectory { get; private set; }

        /// <summary>
        /// true = корневая директория (ссылка на уровень выше)
        /// </summary>
        internal bool IsRoot
        {
            get { return IsDirectory && Name.EqualsOrdinalEx(CustomDialog.ROOT_DIR_NAME); }
        }

        #endregion

        #region Конструктор

        public DialogCustomEntryModel(string location, string name = null)
        {
            Name = name ?? Files.GetFileName(location);
            Location = location;
            Icon = Shell.GetIconByLocation(Location, false);
            IsDirectory = Files.IsDirectory(location);
        }

        #endregion
    }

    #endregion

    internal class DialogCustomPlaceModel
    {
        #region Свойства

        public string Location { get; set; }

        public string Text { get; set; }

        #endregion

        #region Конструктор

        public DialogCustomPlaceModel(string text, string location)
        {
            Text = text;
            Location = location;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return Text;
        }

        #endregion
    }

    internal class DialogCustomTreeItemModel
    {
    }
}