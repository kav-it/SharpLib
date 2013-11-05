//*****************************************************************************
//
// Имя файла    : 'FormPanel.Control.cs'
// Заголовок    : Компонент контейнер элементов в виде формы
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 29/11/2012
//
//*****************************************************************************
			
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;

namespace SharpLib
{
    #region Класс FormPanel
    [ContentProperty("Groups")]
    public partial class FormPanel : UserControl
    {
        #region Поля
        private ObservableCollection<FormPanelGroup> _groups;
        #endregion Поля

        #region Свойства
        public ObservableCollection<FormPanelGroup> Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }
        #endregion Свойства

        #region Конструктор
        public FormPanel()
        {
            InitializeComponent();

            _groups = new ObservableCollection<FormPanelGroup>();
            _groups.CollectionChanged += new NotifyCollectionChangedEventHandler(_groups_CollectionChanged);
        }
        #endregion Конструктор

        #region Методы
        private void _groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PART_stackPanel.Children.Clear();

            foreach (FormPanelGroup group in _groups)
            {
                group.Panel = this;
                PART_stackPanel.Children.Add(group);
            }
        }
        #endregion Методы
    }
    #endregion Класс FormPanel

    #region Класс FormPanelGroup
    [ContentProperty("Columns")]
    public class FormPanelGroup : GroupBox
    {
        #region Поля
        private FormPanel _panel;
        private StackPanel PART_stackPanel;
        private ObservableCollection<FormPanelColumn> _columns;
        #endregion Поля

        #region Свойства
        public FormPanel Panel
        {
            get { return _panel; }
            set { _panel = value; }
        }
        public ObservableCollection<FormPanelColumn> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }
        #endregion Свойства

        #region Конструктор
        public FormPanelGroup()
        {
            PART_stackPanel = new StackPanel();
            PART_stackPanel.Orientation = Orientation.Horizontal;
            this.Content = PART_stackPanel;

            _columns = new ObservableCollection<FormPanelColumn>();
            _columns.CollectionChanged += new NotifyCollectionChangedEventHandler(_columns_CollectionChanged);
        }
        #endregion Конструктор

        #region Методы
        private void _columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PART_stackPanel.Children.Clear();

            foreach (FormPanelColumn column in _columns)
            {
                column.Group = this;
                PART_stackPanel.Children.Add(column);
            }
        }
        #endregion Методы
    }
    #endregion Класс FormPanelGroup

    #region Класс FormPanelColumn
    public class FormPanelColumn: StackPanel
    {
        #region Поля
        // private StackPanel PART_stackPanel;
        private FormPanelGroup _group;
        #endregion Поля

        #region Свойства
        public FormPanelGroup Group
        {
            get { return _group; }
            set { _group = value; }
        }
        #endregion Свойства

        #region Конструктор
        public FormPanelColumn()
        {
            this.Orientation = Orientation.Vertical;

        }
        #endregion Конструктор

        #region Методы
        #endregion Методы
    }
    #endregion Класс FormPanelColumn
}
