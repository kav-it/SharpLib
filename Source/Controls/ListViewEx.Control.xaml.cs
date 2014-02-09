// ****************************************************************************
//
// Имя файла    : 'ListViewEx.Control.cs'
// Заголовок    : Компонент "ListView" с дополнительным функциями
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 29/09/2012
//
// ****************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace SharpLib
{

    #region Класс ListViewEx

    public partial class ListViewEx : UserControl
    {
        #region Поля

        public static readonly DependencyProperty HeightLinesProperty;

        public static readonly DependencyProperty ShowPairLinesProperty;

        private ListViewExGroup _group;

        private ContextMenu _headerMenu;

        private ListSortDirection _lastDirection;

        private GridViewColumnHeader _lastHeaderClicked;

        private ListViewExSorter _sorter;

        #endregion

        #region Свойства

        public ListViewExGroup Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public GridViewColumnCollection Columns
        {
            get { return PART_gridView.Columns; }
        }

        public IEnumerable ItemsSource
        {
            get { return PART_listView.ItemsSource; }
            set
            {
                PART_listView.ItemsSource = value;

                ((INotifyCollectionChanged)PART_listView.Items).CollectionChanged += ListViewEx_CollectionChanged;

                UpdateGroup();
            }
        }

        public ItemCollection Items
        {
            get { return PART_listView.Items; }
        }

        public Boolean AutoScroll { get; set; }

        public ListViewExConfig Config
        {
            get { return GetConfig(); }
            set { SetConfig(value); }
        }

        public Object SelectedItem
        {
            get { return PART_listView.SelectedItem; }
            set
            {
                PART_listView.SelectedItem = value;
                PART_listView.ScrollIntoView(value);
            }
        }

        public IList SelectedItems
        {
            get { return PART_listView.SelectedItems; }
        }

        public Double RowHeight { get; set; }

        public ListViewExSorter Sorter
        {
            get { return _sorter; }
            set { _sorter = value; }
        }

        public Boolean ShowPairLines
        {
            get { return (Boolean)GetValue(ShowPairLinesProperty); }
            set { SetValue(ShowPairLinesProperty, value); }
        }

        public Boolean ShowStatusBar
        {
            get { return (PART_statusBar.Visibility == Visibility.Visible); }
            set
            {
                Visibility visible = value ? Visibility.Visible : Visibility.Collapsed;

                PART_statusBar.Visibility = visible;
            }
        }

        public Boolean ShowFilterBar
        {
            get { return (PART_textEdit.Visibility == Visibility.Visible); }
            set
            {
                Visibility visible = value ? Visibility.Visible : Visibility.Collapsed;

                PART_textEdit.Visibility = visible;

                if (visible != Visibility.Visible)
                {
                    // Фильтр скрыт: Отключение фильтрации данных
                    FilterText = "";
                }
            }
        }

        public Double HeightLines
        {
            get { return (Double)GetValue(HeightLinesProperty); }
            set { SetValue(HeightLinesProperty, value); }
        }

        public String FilterText
        {
            get { return PART_textEdit.Text; }
            set { PART_textEdit.Text = value; }
        }

        #endregion

        #region События

        public event MouseButtonEventHandler OnDoubleClickItem;

        public event SelectionChangedEventHandler OnSelectedItem;

        #endregion

        #region Конструктор

        static ListViewEx()
        {
            ShowPairLinesProperty = DependencyProperty.Register("ShowPairLines", typeof(Boolean), typeof(ListViewEx), new PropertyMetadata(true));
            HeightLinesProperty = DependencyProperty.Register("HeightLines", typeof(Double), typeof(ListViewEx), new PropertyMetadata(25.0));
        }

        public ListViewEx()
        {
            InitializeComponent();

            AutoScroll = true;
            RowHeight = Double.NaN;
            _lastHeaderClicked = null;
            _lastDirection = ListSortDirection.Ascending;
            _sorter = null;

            PART_listView.Loaded += ListViewLoaded;
            PART_listView.SelectionChanged += HandleSelectedItem;
            PART_listView.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(ListViewDragDelta), true);
        }

        #endregion

        #region Компонент загружен

        private void ListViewLoaded(Object sender, RoutedEventArgs e)
        {
            // Создание контекстного меню
            OnCreateContextMenu();
        }

        #endregion Компонент загружен

        #region Визуальное обновление

        public void Refresh()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);
            view.Refresh();
        }

        #endregion Визуальное обновление

        #region Загрузка/Сохранение конфигурации

        private ListViewExConfig GetConfig()
        {
            ListViewExConfig config = new ListViewExConfig();

            for (int i = 0; i < PART_gridView.Columns.Count; i++)
            {
                ListViewExColumn column = (ListViewExColumn)PART_gridView.Columns[i];
                ColumnConfig item = new ColumnConfig();

                item.Index = i;
                item.Name = (String)column.Header;

                if (column.Visible)
                {
                    item.Visible = true;
                    item.Width = column.ActualWidth;
                }
                else
                {
                    item.Visible = false;
                    item.Width = column.SaveWidth;
                }

                config.Columns.Add(item);
            }

            return config;
        }

        private void SetConfig(ListViewExConfig config)
        {
            for (int i = 0; i < PART_gridView.Columns.Count; i++)
            {
                foreach (ColumnConfig item in config.Columns)
                {
                    if (i == item.Index)
                    {
                        ListViewExColumn column = (ListViewExColumn)PART_gridView.Columns[i];

                        if (item.Visible)
                        {
                            column.SaveWidth = item.Width;
                            column.Visible = true;
                            column.Width = item.Width;
                        }
                        else
                        {
                            column.SaveWidth = item.Width;
                            column.Visible = false;
                        }
                    } // end if (найдена текущая колонка
                } // end foreach (перебор конфигурации колонок)
            } // end foreach (перебор текущих колонок)
        }

        #endregion Загрузка/Сохранение конфигурации

        #region Работа с контекстным меню "HeaderContextMenu"

        private void OnCreateContextMenu()
        {
            _headerMenu = new ContextMenu();
            foreach (ListViewExColumn column in PART_gridView.Columns)
            {
                MenuItem item = new MenuItem
                    {
                        Header = column.Header,
                        IsCheckable = true,
                        IsChecked = column.Visible,
                        Tag = column
                    };
                item.Click += OnHeaderMenuClick;

                _headerMenu.Items.Add(item);
            }

            PART_gridView.ColumnHeaderContextMenu = _headerMenu;
        }

        private void OnHeaderMenuClick(Object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (item != null)
            {
                ListViewExColumn column = (ListViewExColumn)item.Tag;

                column.Visible = item.IsChecked;

                e.Handled = true;
            }
        }

        private void SetCheckContextMenu(ListViewExColumn column, Boolean value)
        {
            foreach (MenuItem item in _headerMenu.Items)
            {
                ListViewExColumn itemColumn = (ListViewExColumn)item.Tag;

                if (column == itemColumn)
                {
                    item.IsChecked = value;
                    return;
                }
            }
        }

        #endregion Работа с контекстным меню "HeaderContextMenu"

        #region Реализация "Группы"

        private void UpdateGroup()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);

            if (view != null)
            {
                if (_group != null)
                {
                    view.GroupDescriptions.Clear();

                    if (_group.GroupName != "")
                        view.GroupDescriptions.Add(new PropertyGroupDescription(_group.GroupName));
                }
            }
        }

        #endregion Реализация "Группы"

        #region Реализация "Автоскролл"

        /// <summary>
        /// Изменилась коллекция
        /// </summary>
        private void ListViewEx_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {
            if (AutoScroll)
            {
                // Включен режим "Автоскролл"
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0)
                    PART_listView.ScrollIntoView(e.NewItems[0]);
            }
        }

        #endregion Реализация "Автоскролл"

        #region Событие "SelectedItem"

        private void HandleSelectedItem(Object sender, SelectionChangedEventArgs e)
        {
            if (OnSelectedItem != null)
            {
                Object item;

                item = e.AddedItems.Count == 0 ? null : e.AddedItems[0];

                OnSelectedItem(item, e);
            }
        }

        #endregion Событие "SelectedItem"

        #region Событие "DoubleClick"

        private void HandleDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            if (OnDoubleClickItem != null)
            {
                Object item = ((FrameworkElement)e.OriginalSource).DataContext;

                if (item != null)
                    OnDoubleClickItem(item, e);
            }
        }

        #endregion Событие "DoubleClick"

        #region Событие "DragThumb"

        private void ListViewDragDelta(Object sender, DragDeltaEventArgs e)
        {
            Thumb senderAsThumb = e.OriginalSource as Thumb;
            GridViewColumnHeader header = senderAsThumb.TemplatedParent as GridViewColumnHeader;
            if (header == null) return;
            ListViewExColumn column = header.Column as ListViewExColumn;

            if (column != null)
            {
                Double minWidth = column.MinWidth;
                Double maxWidth = column.MaxWidth;

                if (Double.IsNaN(minWidth) == false)
                {
                    if (header.Column.ActualWidth < minWidth)
                        header.Column.Width = minWidth;
                }

                if (Double.IsNaN(maxWidth) == false)
                {
                    if (header.Column.ActualWidth > maxWidth)
                        header.Column.Width = maxWidth;
                }
            }
        }

        #endregion Событие "DragThumb"

        #region Событие "Сортировка"

        private void ColumnHeaderClickedHandler(Object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (_lastHeaderClicked != null)
                    _lastHeaderClicked.Column.HeaderTemplate = null;

                if (headerClicked != _lastHeaderClicked)
                    direction = ListSortDirection.Ascending;
                else
                {
                    if (_lastDirection == ListSortDirection.Ascending)
                        direction = ListSortDirection.Descending;
                    else
                        direction = ListSortDirection.Ascending;
                }

                ListViewExColumn column = headerClicked.Column as ListViewExColumn;

                if (column != null)
                {
                    ColumnSort(column, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                            Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                            Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void ColumnSort(ListViewExColumn column, ListSortDirection direction)
        {
            if (_sorter != null)
            {
                _sorter.Direction = direction;
                _sorter.ColumnName = column.Name;
                ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(PART_listView.ItemsSource);
                view.CustomSort = _sorter;
                view.Refresh();
            }
            else
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(PART_listView.ItemsSource);
                if (view != null)
                {
                    view.SortDescriptions.Clear();
                    SortDescription sortDesc = new SortDescription(column.Header as String, direction);
                    view.SortDescriptions.Add(sortDesc);
                    view.Refresh();
                }
            }
        }

        #endregion Событие "Сортировка"

        #region Событие "Фильтрация"

        private void PART_textEdit_TextChanged(Object sender, TextEditChangeEventArg e)
        {
            FilterRefresh();
        }

        private void FilterRefresh()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(PART_listView.ItemsSource);
            if (view != null)
            {
                view.Filter = FilterDefault;
                view.Refresh();
            }
        }

        private Boolean FilterDefault(Object value)
        {
            String filter = FilterText;

            if (value == null)
                return false;

            if (String.IsNullOrEmpty(filter))
                return true;

            Boolean result = FilterInProperties(value, filter);

            return result;
        }

        private Boolean FilterInProperties(Object value, String filter)
        {
            Type type = value.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo item in properties)
            {
                Object s = item.GetValue(value, null);
                if (s == null) continue;

                int index = s.ToString().IndexOf(filter, 0, StringComparison.InvariantCultureIgnoreCase);

                if (index > -1) return true;
            }

            return false;
        }

        #endregion Событие "Фильтрация"

        #region Управление выделением

        public void SelectedAll()
        {
            PART_listView.SelectAll();
        }

        #endregion Управление выделением
    }

    #endregion Класс ListViewEx

    #region Класс ListViewExColumn

    [ContentProperty("Content")]
    public class ListViewExColumn : GridViewColumn
    {
        #region Поля

        public static readonly DependencyProperty NameProperty;

        private Double _saveWidth;

        private Boolean _visible;

        #endregion

        #region Свойства

        public DataTemplate Content
        {
            get { return CellTemplate; }
            set { CellTemplate = value; }
        }

        public Boolean Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;

                    if (value)
                        Width = _saveWidth;
                    else
                    {
                        _saveWidth = ActualWidth;
                        Width = 0;
                    }
                }
            }
        }

        public String Title
        {
            get { return (String)Header; }
            set { Header = value; }
        }

        public Double SaveWidth
        {
            get { return _saveWidth; }
            set { _saveWidth = value; }
        }

        public Double MinWidth { get; set; }

        public Double MaxWidth { get; set; }

        public String Name
        {
            get { return (String)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        #endregion

        #region Конструктор

        static ListViewExColumn()
        {
            NameProperty = DependencyProperty.Register("Name", typeof(String), typeof(ListViewExColumn), new UIPropertyMetadata(""));
        }

        public ListViewExColumn()
        {
            _saveWidth = Double.NaN;
            _visible = true;
            MinWidth = 10;
            MaxWidth = Double.NaN;
            HorizontalAlignment = HorizontalAlignment.Center;
        }

        #endregion
    }

    #endregion Класс ListViewExColumn

    #region Класс ListViewExGroup

    [ContentProperty("Template")]
    public class ListViewExGroup : DependencyObject
    {
        #region Свойства

        /// <summary>
        /// Имя свойства, по которому производится группировка
        /// </summary>
        public String GroupName { get; set; }

        public String GroupText { get; set; }

        public DataTemplate Template { get; set; }

        #endregion
    }

    #endregion Класс ListViewExColumn

    #region Класс ColumnConfig

    public class ColumnConfig
    {
        #region Свойства

        public int Index { get; set; }

        public String Name { get; set; }

        public Boolean Visible { get; set; }

        public Double Width { get; set; }

        #endregion

        #region Конструктор

        public ColumnConfig()
        {
            Name = "";
            Visible = true;
            Width = 20;
        }

        #endregion
    }

    #endregion ColumnConfig

    #region Класс ListViewExConfig

    public class ListViewExConfig
    {
        #region Свойства

        public List<ColumnConfig> Columns { get; set; }

        #endregion

        #region Конструктор

        public ListViewExConfig()
        {
            Columns = new List<ColumnConfig>();
        }

        #endregion
    }

    #endregion Класс ListViewExConfig

    #region Класс ListViewExSorter

    public abstract class ListViewExSorter : IComparer
    {
        #region Поля

        public String ColumnName;

        public ListSortDirection Direction;

        #endregion

        #region Конструктор

        protected ListViewExSorter()
        {
            Direction = ListSortDirection.Ascending;
            ColumnName = "";
        }

        #endregion

        #region Методы

        public abstract int CompareByColumn(Object x, Object y, String columnName);

        protected int CompareString(Object x, Object y)
        {
            String a = x as String;
            String b = y as String;

            return a.CompareTo(b);
        }

        protected int CompareInt(Object x, Object y)
        {
            int a = (int)x;
            int b = (int)y;

            return a.CompareTo(b);
        }

        public int Compare(Object x, Object y)
        {
            int result = CompareByColumn(x, y, ColumnName);

            if (Direction == ListSortDirection.Descending)
                result *= (-1);

            return result;
        }

        #endregion
    }

    #endregion Класс ListViewExSorter

    #region Класс ListViewExBackgroundConvertor

    internal sealed class ListViewExBackgroundConvertor : IMultiValueConverter
    {
        #region Поля

        private SolidColorBrush _lightBlueBrush = new SolidColorBrush(Color.FromArgb(255, 240, 245, 255));

        #endregion

        #region Методы

        public Object Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture)
        {
            Boolean show = (Boolean)values[0];

            if (show)
            {
                ListViewItem item = (ListViewItem)values[1];
                ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;

                int index = listView.ItemContainerGenerator.IndexFromContainer(item);

                if (index % 2 == 0)
                    return _lightBlueBrush;
                else
                    return Brushes.White;
            }

            return Brushes.White;
        }

        public Object[] ConvertBack(Object value, Type[] targetType, Object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс ListViewExBackgroundConvertor

    #region Класс ListViewExMenu

    public class ListViewExMenu<T> : ContextMenuBase
    {
        #region Поля

        private GuiList<T> _items;

        private ListViewEx _owner;

        #endregion

        #region Конструктор

        public ListViewExMenu(ListViewEx owner, GuiList<T> items)
        {
            _owner = owner;
            _items = items;

            MenuItemBase clear = new MenuItemBase("Очистить", OnClickClear);
            Add(clear);
        }

        #endregion

        #region Методы

        private void OnClickClear(Object sender, RoutedEventArgs e)
        {
            _items.Clear();
        }

        #endregion
    }

    #endregion Класс ListViewExMenu
}