//*****************************************************************************
//
// Имя файла    : 'FormPanel.Control.cs'
// Заголовок    : Компонент контейнер элементов в виде формы
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 29/11/2012
//
//*****************************************************************************

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SharpLib
{

    #region Класс FormPanel

    [ContentProperty("Groups")]
    public partial class FormPanel : UserControl
    {
        #region Поля

        private ObservableCollection<FormPanelGroup> _groups;

        #endregion

        #region Свойства

        public ObservableCollection<FormPanelGroup> Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        #endregion

        #region Конструктор

        public FormPanel()
        {
            InitializeComponent();

            _groups = new ObservableCollection<FormPanelGroup>();
            _groups.CollectionChanged += _groups_CollectionChanged;
        }

        #endregion

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

        #endregion
    }

    #endregion Класс FormPanel

    #region Класс FormPanelGroup

    [ContentProperty("Columns")]
    public class FormPanelGroup : GroupBox
    {
        #region Поля

        private StackPanel PART_stackPanel;

        private ObservableCollection<FormPanelColumn> _columns;

        #endregion

        #region Свойства

        public FormPanel Panel { get; set; }

        public ObservableCollection<FormPanelColumn> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        #endregion

        #region Конструктор

        public FormPanelGroup()
        {
            PART_stackPanel = new StackPanel();
            PART_stackPanel.Orientation = Orientation.Horizontal;
            Content = PART_stackPanel;

            _columns = new ObservableCollection<FormPanelColumn>();
            _columns.CollectionChanged += _columns_CollectionChanged;
        }

        #endregion

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

        #endregion
    }

    #endregion Класс FormPanelGroup

    #region Класс FormPanelColumn

    public class FormPanelColumn : StackPanel
    {
        // private StackPanel PART_stackPanel;

        #region Свойства

        public FormPanelGroup Group { get; set; }

        #endregion

        #region Конструктор

        public FormPanelColumn()
        {
            Orientation = Orientation.Vertical;
        }

        #endregion
    }

    #endregion Класс FormPanelColumn
}