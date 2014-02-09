//*****************************************************************************
//
// Имя файла    : 'TreeView.cs'
// Заголовок    : Очередная итерация TreeView
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 15/04/2012
//
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpLib
{

    #region Делегат TreeViewDragDropCallback

    public delegate Boolean TreeViewDragDropCallback(DragDroperOperation oper, List<TreeViewItemEx> items, List<TreeViewItemEx> newItems);

    public delegate void TreeViewItemCallback(TreeViewItemEx item);

    #endregion Делегат TreeViewDragDropCallback

    #region Класс TreeViewControl

    public partial class TreeViewControl : UserControl
    {
        #region Поля

        private DragDroper _dragDrop;

        private TreeViewItemEx _dragOverItem;

        private List<TreeViewItemEx> _selectedItems;

        private SortDirection _sortDirection;

        private TreeViewSorter _sorter;

        #endregion

        #region Свойства

        public TreeView TreeView
        {
            get { return PART_treeView; }
        }

        public TreeViewItemEx SelectedItem
        {
            get
            {
                List<TreeViewItemEx> items = SelectedItems;
                if (items.Count > 0)
                    return items[0];

                return null;
            }
        }

        public List<TreeViewItemEx> SelectedItems
        {
            get { return _selectedItems; }
        }

        public TreeViewItemEx Root
        {
            get
            {
                if (PART_treeView.Items.Count > 0)
                    return (TreeViewItemEx)PART_treeView.Items[0];

                return null;
            }
        }

        public SortDirection SortDirection
        {
            get { return _sortDirection; }
            set { _sortDirection = value; }
        }

        public TreeViewItemEx DragOverItem
        {
            get { return _dragOverItem; }
            private set { _dragOverItem = value; }
        }

        public TreeViewItemEx DragDestItem { get; set; }

        public TreeViewSorter Sorter
        {
            get { return _sorter; }
            set { _sorter = value; }
        }

        #endregion

        #region События

        public event TreeViewItemCallback OnDoubleClick;

        public event TreeViewDragDropCallback OnDragDrop;

        public event SelectionChangedEventHandler OnSelectedItem;

        #endregion

        #region Конструктор

        public TreeViewControl()
        {
            InitializeComponent();

            // Конфигурация работы "Веделение элементов"
            _selectedItems = new List<TreeViewItemEx>();
            PART_treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
            PART_treeView.MouseDoubleClick += TreeView_MouseDoubleClick;

            // Конфигурация работы "Drag&Drop"
            _dragDrop = new DragDroper();
            _dragDrop.Init(PART_treeView, TreeView_DragDrop);
            _dragOverItem = null;
            OnDragDrop = null;

            // Конфигурация работы "DoubleClick"
            OnDoubleClick = null;

            // Настройка сортировки
            _sortDirection = SortDirection.None;
            _sorter = new TreeViewSorter();
        }

        #endregion

        #region Вспомогательные методы

        private int SearchSort(ItemCollection coll, TreeViewItemEx item)
        {
            int count = coll.Count;

            if (_sortDirection != SortDirection.None)
            {
                for (int index = 0; index < count; index++)
                {
                    Object curr = coll[index];

                    if (_sorter.Compare(curr, item) == (int)_sortDirection)
                        return index;
                }
            }

            return count;
        }

        #endregion Вспомогательные методы

        #region Операции "Drag&Drop"

        private TreeViewItemEx GetDragOverItem()
        {
            Point pt = Mouser.GetCursorPos(this);

            HitTestResult result = VisualTreeHelper.HitTest(this, pt);

            if (result != null)
            {
                for (DependencyObject obj = result.VisualHit; obj != null; obj = VisualTreeHelper.GetParent(obj))
                {
                    if (obj is TreeViewItem)
                        return (TreeViewItemEx)obj;
                }
            }

            return null;
        }

        private void SetDragOverItem(TreeViewItemEx item)
        {
            if (_dragOverItem != item)
            {
                // Смена выделения
                // Удаление предыдущего выделения
                if (_dragOverItem != null)
                    SetSelectItemColor(_dragOverItem, false);

                // Установка нового выделения (если выделение не содержится в текущих перемещаемых элементах)
                if (item != null)
                {
                    int index = SelectedItems.IndexOf(item);
                    if (index == -1)
                    {
                        SetSelectItemColor(item, true);
                        _dragOverItem = item;
                    }
                }
                else
                    _dragOverItem = null;
            }
        }

        private Boolean CheckDragDrop(DragDroperOperation oper, Object data_1, Object data_2)
        {
            Boolean result = false;

            if (OnDragDrop != null)
            {
                List<TreeViewItemEx> items = (List<TreeViewItemEx>)data_1;
                List<TreeViewItemEx> newItems = (List<TreeViewItemEx>)data_2;
                result = OnDragDrop(oper, items, newItems);
            }

            return result;
        }

        private Object TreeView_DragDrop(DragDroperOperation oper, Object data)
        {
            switch (oper)
            {
                case DragDroperOperation.Start:
                    {
                        List<TreeViewItemEx> items = SelectedItems;

                        if (items.Count > 0)
                        {
                            if (CheckDragDrop(oper, items, null))
                                return items;
                        }
                    }
                    break;

                case DragDroperOperation.Move:
                    {
                        if (CheckDragDrop(oper, data, null))
                        {
                            // Определение элемента, над которым сейчас находится курсор
                            TreeViewItemEx dragOverItem = GetDragOverItem();

                            // Выделение элемента (для подстветки позиции куда перемещается элемент
                            SetDragOverItem(dragOverItem);

                            // Возвращение результата: Можно перемещать данные
                            return data;
                        }
                    }
                    break;

                case DragDroperOperation.Stop:
                    {
                        if (CheckDragDrop(oper, data, null))
                        {
                            TreeViewItemEx destItem = DragDestItem;

                            if (DragDestItem == null)
                                destItem = DragOverItem;

                            if (destItem != null)
                            {
                                // Перемещение элементов на новые позиции (копирование)
                                List<TreeViewItemEx> items = (List<TreeViewItemEx>)data;
                                List<TreeViewItemEx> newItems = new List<TreeViewItemEx>();

                                foreach (TreeViewItemEx item in items)
                                {
                                    TreeViewItemEx newItem = Add(destItem, item.Text, item.ImageUri, item.Tag);
                                    newItems.Add(newItem);
                                }

                                Boolean isMove = (Keyboarder.IsControl == false);
                                if (isMove)
                                {
                                    // Операция копирования не использовалась: Операция перемещения, удаление перемещаемых элементов
                                    foreach (TreeViewItemEx item in items)
                                        Remove(item);
                                }

                                // Оповещение приложения об окончании операции
                                if (isMove)
                                    CheckDragDrop(DragDroperOperation.EndMove, items, newItems);
                                else
                                    CheckDragDrop(DragDroperOperation.EndCopy, items, newItems);
                            } // end if (определен получатель операции)
                        } // end if (операция подтверждена приложением)

                        // Удаление выделения (использовался в операции DragDrop)
                        SetDragOverItem(null);
                    }
                    break;
            } // end switch (анализ операции)

            return null;
        }

        #endregion Операции "Drag&Drop"

        #region Обработка события "MouseDoubleClick"

        private void TreeView_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            if (OnDoubleClick != null)
            {
                TreeViewItemEx item = SelectedItem;

                if (item != null)
                    OnDoubleClick(item);
            }
        }

        #endregion Обработка события "MouseDoubleClick"

        #region Обработка событий "RightButtonDown"

        private void TreeViewItem_PreviewMouseRightButtonDown(Object sender, MouseEventArgs e)
        {
            TreeView treeView = PART_treeView;
            IInputElement element = treeView.InputHitTest(e.GetPosition(treeView));
            while (!((element is TreeView) || element == null))
            {
                if (element is TreeViewItem)
                    break;

                if (element is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)element;
                    element = (IInputElement)(fe.Parent ?? fe.TemplatedParent);
                }
                else
                    break;
            }

            if (element is TreeViewItem)
            {
                element.Focus();
                e.Handled = true;
            }
        }

        #endregion Обработка событий "RightButtonDown"

        #region Обработка события "Selected"

        private void TreeView_SelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            TreeViewItemEx selectedItem = PART_treeView.SelectedItem as TreeViewItemEx;

            if (selectedItem == null)
                return;

            // Удаление текущего выделения
            selectedItem.IsSelected = false;
            selectedItem.Focus();

            // Выделение с нажатым "Shift"
            if (Keyboarder.IsShift)
            {
                // Выделение обработано функцией
                if (SelectedWithShift(selectedItem))
                    return;
            }

            // Выделение с нажатым "Control"
            if (Keyboarder.IsControl == false)
            {
                // Сброс предыдущего выделенного списка
                foreach (TreeViewItemEx item in _selectedItems)
                    SetSelectItemColor(item, false);

                _selectedItems.Clear();
            }

            // Добавление нового элемента в список
            SetSelectItemColor(selectedItem, true);
            _selectedItems.Add(selectedItem);

            // Передача события
            if (OnSelectedItem != null)
                OnSelectedItem(selectedItem, null);
        }

        private void SetSelectItemColor(TreeViewItemEx item, Boolean select)
        {
            if (select)
            {
                item.BorderBrush = Gui.HexToColor("#adc6e5");
                // item.BorderThickness = new Thickness(1.5);
                item.Background = (Brush)FindResource("brushSelected");
                item.Foreground = Brushes.Black;
            }
            else
            {
                item.BorderBrush = null;
                item.Background = Brushes.White;
                item.Foreground = Brushes.Black;
            }
        }

        private Boolean SelectedWithShift(TreeViewItemEx selectedItem)
        {
            if (_selectedItems.Count > 0)
            {
                // Уже был выделен элемент: Удаление всех
                TreeViewItemEx firstItem = _selectedItems[0];
                // Сброс предыдущего выделенного списка
                foreach (TreeViewItemEx item in _selectedItems)
                    SetSelectItemColor(item, false);
                _selectedItems.Clear();

                // Добавление всех элементов от первого до нового выделенного
                // Определение направления прохода (вверх или вниз)
                int firstIndex = firstItem.Index;
                int lastIndex = selectedItem.Index;

                Boolean inc = (firstIndex < lastIndex) ? true : false;

                do
                {
                    // Добавление в список выделенных
                    _selectedItems.Add(firstItem);

                    if (firstItem == selectedItem)
                        break;

                    if (inc)
                        firstItem = firstItem.NextItem;
                    else
                        firstItem = firstItem.PrevItem;
                } while (firstItem != null);

                // Визульаное выделение элементов
                foreach (TreeViewItemEx item in _selectedItems)
                    SetSelectItemColor(item, true);

                return true;
            }

            return false;
        }

        #endregion Обработка события "Selected"

        #region Добавление элементов

        public TreeViewItemEx Add(TreeViewItemEx parent, TreeViewItemEx item)
        {
            ItemCollection coll;

            if (parent == null)
                coll = PART_treeView.Items;
            else
                coll = parent.Items;

            // Поиск вхождения сортировки
            int index = SearchSort(coll, item);
            // Добавление элемента
            coll.Insert(index, item);

            item.IsExpanded = true;

            return item;
        }

        public TreeViewItemEx Add(TreeViewItemEx parent, String text, String imageUri, Object tag)
        {
            TreeViewItemEx item = new TreeViewItemEx(PART_treeView, parent);
            item.Text = text;
            item.ImageUri = imageUri;
            item.Tag = tag;

            Add(parent, item);

            return item;
        }

        public TreeViewItemEx Add(TreeViewItemEx parent, String text)
        {
            return Add(parent, text, null, null);
        }

        public void Remove(TreeViewItemEx item)
        {
            if (item != null)
            {
                TreeViewItemEx parent = item.ParentItem;

                if (parent == null)
                    PART_treeView.Items.Remove(item);
                else
                    parent.Items.Remove(item);
            }
        }

        public void Clear()
        {
            PART_treeView.Items.Clear();
        }

        #endregion Добавление элементов

        #region Поиск

        public TreeViewItemEx Search(TreeViewItemEx item, Object tag)
        {
            TreeViewItemEx result = TreeViewItemEx.Search(Root, item, tag);

            return result;
        }

        #endregion Поиск

        #region Сворачивание/Разворачивание

        private void ExpandAllNodes(ItemCollection rootItems, Boolean expand)
        {
            foreach (TreeViewItemEx node in rootItems)
            {
                if (node != null)
                {
                    ExpandAllNodes(node.Items, expand);
                    node.IsExpanded = expand;
                }
            }
        }

        public void Collapse()
        {
            ExpandAllNodes(PART_treeView.Items, false);
        }

        public void Expand()
        {
            ExpandAllNodes(PART_treeView.Items, true);
        }

        #endregion Сворачивание/Разворачивание
    }

    #endregion Класс TreeViewControl

    #region Класс TreeViewItemEx

    public class TreeViewItemEx : TreeViewItem
    {
        #region Константы

        private const int IMAGE_SIZE = 16;

        #endregion

        #region Поля

        private ImageSource _iconSource;

        private Image _image;

        private String _imageUri;

        private TreeViewItemEx _parentItem;

        private ItemCollection _parentItems;

        private TextBlock _textBlock;

        private TreeView _tree;

        #endregion

        #region Свойства

        public String Text
        {
            get { return _textBlock.Text; }
            set { _textBlock.Text = value; }
        }

        public String ImageUri
        {
            get { return _imageUri; }
            set
            {
                _imageUri = value;
                if (value.IsValid())
                {
                    _iconSource = ResourcesWpf.LoadImageSource(value);
                    _image.Source = _iconSource;
                }
                else
                {
                    _iconSource = null;
                    _image.Source = null;
                    _image.Visibility = Visibility.Collapsed;
                }
            }
        }

        public TreeViewItemEx ParentItem
        {
            get { return _parentItem; }
        }

        public ItemCollection ParentItems
        {
            get { return _parentItems; }
        }

        public TreeViewItemEx NextItem
        {
            get { return SearchNext(this, true); }
        }

        public TreeViewItemEx PrevItem
        {
            get { return SearchPrev(this); }
        }

        public int Index
        {
            get
            {
                Boolean stop;
                int index = GetIndexInTree(_tree.Items, 0, out stop);

                return index;
            }
        }

        #endregion

        #region Конструктор

        public TreeViewItemEx(TreeView treeView, TreeViewItemEx parent)
        {
            _tree = treeView;
            _parentItem = parent;

            if (parent == null) _parentItems = _tree.Items;
            else _parentItems = parent.Items;

            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            Header = stack;
            // Uncomment this code If you want to add an Image after the Node-HeaderText
            // textBlock = new TextBlock();
            // textBlock.VerticalAlignment = VerticalAlignment.Center;
            // stack.Children.Add(textBlock);
            _image = new Image();
            _image.VerticalAlignment = VerticalAlignment.Center;
            _image.Margin = new Thickness(0, 0, 4, 0);
            _image.Width = IMAGE_SIZE;
            _image.Height = IMAGE_SIZE;
            _image.Source = _iconSource;
            stack.Children.Add(_image);

            // Add the HeaderText After Adding the icon
            _textBlock = new TextBlock();
            _textBlock.VerticalAlignment = VerticalAlignment.Center;
            stack.Children.Add(_textBlock);
        }

        #endregion

        #region Методы

        protected override void OnUnselected(RoutedEventArgs args)
        {
            base.OnUnselected(args);

            _image.Source = _iconSource;
        }

        protected override void OnSelected(RoutedEventArgs args)
        {
            base.OnSelected(args);

            _image.Source = _iconSource;
        }

        private int GetIndexInTree(ItemCollection coll, int index, out Boolean stop)
        {
            stop = true;

            foreach (TreeViewItemEx item in coll)
            {
                if (item == this)
                    return index;

                index++;

                if (item.HasItems)
                {
                    index = GetIndexInTree(item.Items, index, out stop);
                    if (stop) return index;
                }
            }

            stop = false;

            return index;
        }

        private TreeViewItemEx SearchNext(TreeViewItemEx item, Boolean first)
        {
            TreeViewItemEx result = null;

            if (first && item.HasItems)
                result = (TreeViewItemEx)item.Items[0];
            else
            {
                int index = item.ParentItems.IndexOf(item);
                if (index == -1) return null;

                if (item.ParentItems.Count > (index + 1))
                {
                    // В текущий коллекции найден следующий элемент
                    result = (TreeViewItemEx)item.ParentItems[index + 1];
                }
                else
                {
                    // Перебор по родителям, любой следующий дочерний элемент будет найденным
                    result = SearchNext(item.ParentItem, false);
                }
            }

            return result;
        }

        public TreeViewItemEx SearchPrev(TreeViewItemEx item)
        {
            int index = ParentItems.IndexOf(item);
            if (index == -1) return null;

            TreeViewItemEx result = null;

            if (index != 0)
            {
                // В текущий коллекции найден предыдущий элемент
                result = (TreeViewItemEx)ParentItems[index - 1];
            }
            else if (ParentItem != null)
                result = ParentItem;
            else
            {
                // Перебор по родителям, любой следующий дочерний элемент будет найденным
                result = SearchPrev(ParentItem);
            }

            return result;
        }

        public static TreeViewItemEx Search(TreeViewItemEx root, TreeViewItemEx value, Object tag)
        {
            if (root == null) return null;

            foreach (TreeViewItemEx item in root.ParentItems)
            {
                if (tag != null)
                {
                    if (item.Tag == tag)
                        return item;
                }
                else
                {
                    if (item == value)
                        return item;
                }

                if (item.HasItems)
                {
                    TreeViewItemEx result = TreeViewItemEx.Search((TreeViewItemEx)item.Items[0], value, tag);
                    if (result != null) return result;
                }
            }

            return null;
        }

        public override string ToString()
        {
            return Text;
        }

        #endregion
    }

    #endregion Класс TreeViewItemEx

    #region Класс TreeViewSorter

    public class TreeViewSorter
    {
        #region Методы

        public virtual int Compare(Object x, Object y)
        {
            TreeViewItemEx localX = x as TreeViewItemEx;
            TreeViewItemEx localY = y as TreeViewItemEx;

            int result = String.Compare(localX.Text, localY.Text);

            return result;
        }

        #endregion
    }

    #endregion Класс TreeViewSorter
}