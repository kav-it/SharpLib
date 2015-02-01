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

namespace SharpLib.Wpf.Controls
{

    #region Класс ListViewEx

    public partial class ListViewEx
    {
        #region Поля

        public static readonly DependencyProperty HeightLinesProperty;

        public static readonly DependencyProperty ShowPairLinesProperty;

        private ContextMenu _headerMenu;

        private ListSortDirection _lastDirection;

        private GridViewColumnHeader _lastHeaderClicked;

        #endregion

        #region Свойства

        public ListViewExGroup Group { get; set; }

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

        public bool AutoScroll { get; set; }

        public ListViewExConfig Config
        {
            get { return GetConfig(); }
            set { SetConfig(value); }
        }

        public object SelectedItem
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

        public ListViewExSorter Sorter { get; set; }

        public bool ShowPairLines
        {
            get { return (Boolean)GetValue(ShowPairLinesProperty); }
            set { SetValue(ShowPairLinesProperty, value); }
        }

        public bool ShowStatusBar
        {
            get { return (PART_statusBar.Visibility == Visibility.Visible); }
            set
            {
                Visibility visible = value ? Visibility.Visible : Visibility.Collapsed;

                PART_statusBar.Visibility = visible;
            }
        }

        public bool ShowFilterBar
        {
            get { return (PART_textEdit.Visibility == Visibility.Visible); }
            set
            {
                Visibility visible = value ? Visibility.Visible : Visibility.Collapsed;

                PART_textEdit.Visibility = visible;

                if (visible != Visibility.Visible)
                {
                    // Фильтр скрыт: Отключение фильтрации данных
                    FilterText = string.Empty;
                }
            }
        }

        public Double HeightLines
        {
            get { return (Double)GetValue(HeightLinesProperty); }
            set { SetValue(HeightLinesProperty, value); }
        }

        public string FilterText
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
            Sorter = null;

            PART_listView.Loaded += ListViewLoaded;
            PART_listView.SelectionChanged += HandleSelectedItem;
            PART_listView.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(ListViewDragDelta), true);
        }

        #endregion

        #region Методы

        private void ListViewLoaded(object sender, RoutedEventArgs e)
        {
            // Создание контекстного меню
            OnCreateContextMenu();
        }

        public void Refresh()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);
            view.Refresh();
        }

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
                        var column = (ListViewExColumn)PART_gridView.Columns[i];

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

        private void OnCreateContextMenu()
        {
            _headerMenu = new ContextMenu();
            foreach (var gridViewColumn in PART_gridView.Columns)
            {
                var column = (ListViewExColumn)gridViewColumn;

                var item = new MenuItem
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

        private void OnHeaderMenuClick(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item != null)
            {
                var column = (ListViewExColumn)item.Tag;
                column.Visible = item.IsChecked;
                e.Handled = true;
            }
        }

        private void SetCheckContextMenu(ListViewExColumn column, bool value)
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

        private void UpdateGroup()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);

            if (view != null)
            {
                if (Group != null)
                {
                    view.GroupDescriptions.Clear();

                    if (Group.GroupName != "")
                    {
                        view.GroupDescriptions.Add(new PropertyGroupDescription(Group.GroupName));
                    }
                }
            }
        }

        /// <summary>
        /// Изменилась коллекция
        /// </summary>
        private void ListViewEx_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (AutoScroll)
            {
                // Включен режим "Автоскролл"
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0)
                {
                    PART_listView.ScrollIntoView(e.NewItems[0]);
                }
            }
        }

        private void HandleSelectedItem(object sender, SelectionChangedEventArgs e)
        {
            if (OnSelectedItem != null)
            {
                object item = e.AddedItems.Count == 0 ? null : e.AddedItems[0];

                OnSelectedItem(item, e);
            }
        }

        private void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OnDoubleClickItem != null)
            {
                object item = ((FrameworkElement)e.OriginalSource).DataContext;

                if (item != null)
                {
                    OnDoubleClickItem(item, e);
                }
            }
        }

        private void ListViewDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb senderAsThumb = e.OriginalSource as Thumb;
            if (senderAsThumb == null)
            {
                return;
            }

            var header = senderAsThumb.TemplatedParent as GridViewColumnHeader;
            if (header == null)
            {
                return;
            }
            var column = header.Column as ListViewExColumn;

            if (column != null)
            {
                Double minWidth = column.MinWidth;
                Double maxWidth = column.MaxWidth;

                if (Double.IsNaN(minWidth) == false)
                {
                    if (header.Column.ActualWidth < minWidth)
                    {
                        header.Column.Width = minWidth;
                    }
                }

                if (Double.IsNaN(maxWidth) == false)
                {
                    if (header.Column.ActualWidth > maxWidth)
                    {
                        header.Column.Width = maxWidth;
                    }
                }
            }
        }

        private void ColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null)
            {
                if (_lastHeaderClicked != null)
                {
                    _lastHeaderClicked.Column.HeaderTemplate = null;
                }

                ListSortDirection direction;
                if (headerClicked != _lastHeaderClicked)
                {
                    direction = ListSortDirection.Ascending;
                }
                else
                {
                    direction = _lastDirection == ListSortDirection.Ascending
                        ? ListSortDirection.Descending
                        : ListSortDirection.Ascending;
                }

                var column = headerClicked.Column as ListViewExColumn;

                if (column != null)
                {
                    if (ColumnSort(column, direction) == false)
                    {
                        return;
                    }

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private bool ColumnSort(ListViewExColumn column, ListSortDirection direction)
        {
            if (column.AllowSort == false)
            {
                return false;
            }

            if (Sorter != null)
            {
                Sorter.Direction = direction;
                Sorter.ColumnName = column.Name;
                var view = (ListCollectionView)CollectionViewSource.GetDefaultView(PART_listView.ItemsSource);
                view.CustomSort = Sorter;
                view.Refresh();
            }
            else
            {
                var view = CollectionViewSource.GetDefaultView(PART_listView.ItemsSource);
                if (view != null)
                {
                    view.SortDescriptions.Clear();
                    var sortDesc = new SortDescription(column.Header as string, direction);
                    view.SortDescriptions.Add(sortDesc);
                    view.Refresh();
                }
            }

            return true;
        }

        private void PART_textEdit_TextChanged(object sender, TextChangedEventArgs e)
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

        private bool FilterDefault(object value)
        {
            string filter = FilterText;

            if (value == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(filter))
            {
                return true;
            }

            bool result = FilterInProperties(value, filter);

            return result;
        }

        private bool FilterInProperties(object value, string filter)
        {
            var type = value.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo item in properties)
            {
                object s = item.GetValue(value, null);
                if (s == null)
                {
                    continue;
                }

                int index = s.ToString().IndexOf(filter, 0, StringComparison.InvariantCultureIgnoreCase);

                if (index > -1)
                {
                    return true;
                }
            }

            return false;
        }

        public void SelectedAll()
        {
            PART_listView.SelectAll();
        }

        #endregion
    }

    #endregion Класс ListViewEx

    #region Класс ListViewExColumn

    [ContentProperty("Content")]
    public class ListViewExColumn : GridViewColumn
    {
        #region Поля

        public static readonly DependencyProperty NameProperty;

        private bool _visible;

        #endregion

        #region Свойства

        public bool AllowSort { get; set; }

        public DataTemplate Content
        {
            get { return CellTemplate; }
            set { CellTemplate = value; }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;

                    if (value)
                    {
                        Width = SaveWidth;
                    }
                    else
                    {
                        SaveWidth = ActualWidth;
                        Width = 0;
                    }
                }
            }
        }

        public string Title
        {
            get { return (string)Header; }
            set { Header = value; }
        }

        public double SaveWidth { get; set; }

        public Double MinWidth { get; set; }

        public Double MaxWidth { get; set; }

        public string Name
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
            SaveWidth = Double.NaN;
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
        public string GroupName { get; set; }

        public string GroupText { get; set; }

        public DataTemplate Template { get; set; }

        #endregion
    }

    #endregion Класс ListViewExColumn

    #region Класс ColumnConfig

    public class ColumnConfig
    {
        #region Свойства

        public int Index { get; set; }

        public string Name { get; set; }

        public bool Visible { get; set; }

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

        public string ColumnName { get; internal set; }

        public ListSortDirection Direction { get; internal set; }

        #endregion

        #region Конструктор

        protected ListViewExSorter()
        {
            Direction = ListSortDirection.Ascending;
            ColumnName = string.Empty;
        }

        #endregion

        #region Методы

        public abstract int CompareByColumn(ListViewExSorterArgs args);

        protected int CompareString(object x, object y)
        {
            string a = x as string;
            string b = y as string;

            return string.Compare(a, b, StringComparison.Ordinal);
        }

        protected int CompareInt(object x, object y)
        {
            int a = (int)x;
            int b = (int)y;

            return a.CompareTo(b);
        }

        public int Compare(object x, object y)
        {
            var args = new ListViewExSorterArgs(Direction, ColumnName, x, y);
            int result = CompareByColumn(args);

            if (args.IsHandledDirection)
            {
                return result;
            }

            if (Direction == ListSortDirection.Descending)
            {
                result *= (-1);
            }

            return result;
        }

        #endregion
    }

    #endregion Класс ListViewExSorter

    #region Класс ListViewExSorterArgs

    public class ListViewExSorterArgs
    {
        public bool IsHandledDirection { get; set; }

        public ListSortDirection Direction { get; private set; }

        public object X { get; private set; }

        public object Y { get; private set; }

        public string ColumnName { get; private set; }

        public ListViewExSorterArgs(ListSortDirection direction, string columnName, object x, object y)
        {
            IsHandledDirection = false;
            Direction = direction;
            ColumnName = columnName;
            X = x;
            Y = y;
        }
    }

    #endregion Класс ListViewExSorterArgs

    #region Класс ListViewExBackgroundConvertor

    internal sealed class ListViewExBackgroundConvertor : IMultiValueConverter
    {
        #region Поля

        private readonly SolidColorBrush _lightBlueBrush;

        #endregion

        #region Конструктор

        public ListViewExBackgroundConvertor()
        {
            _lightBlueBrush = new SolidColorBrush(Color.FromArgb(255, 240, 245, 255));
        }

        #endregion

        #region Методы

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool show = (bool)values[0];

            if (show)
            {
                var item = (ListViewItem)values[1];
                var listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;

                if (listView != null)
                {
                    int index = listView.ItemContainerGenerator.IndexFromContainer(item);

                    return index % 2 == 0 ? _lightBlueBrush : Brushes.White;
                }
            }

            return Brushes.White;
        }

        public Object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion Класс ListViewExBackgroundConvertor

    #region Класс ListViewExMenu

    //public class ListViewExMenu<T> : ContextMenuBase
    //{
    //    #region Поля

    //    private GuiList<T> _items;

    //    private ListViewEx _owner;

    //    #endregion

    //    #region Конструктор

    //    public ListViewExMenu(ListViewEx owner, GuiList<T> items)
    //    {
    //        _owner = owner;
    //        _items = items;

    //        MenuItemBase clear = new MenuItemBase("Очистить", OnClickClear);
    //        Add(clear);
    //    }

    //    #endregion

    //    #region Методы

    //    private void OnClickClear(object sender, RoutedEventArgs e)
    //    {
    //        _items.Clear();
    //    }

    //    #endregion
    //}

    #endregion Класс ListViewExMenu
}