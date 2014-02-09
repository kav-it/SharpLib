//*****************************************************************************
//
// Имя файла    : 'Stopwatch.Control.cs'
// Заголовок    : Компонент "Секундомер/Таймер"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 20/01/2013
//
//*****************************************************************************

using System.Collections.Generic;
using System.Windows.Controls;

namespace SharpLib
{

    #region Класс TabControlEx

    public partial class TabControlEx : UserControl
    {
        #region Свойства

        public TabItem ActiveTab
        {
            get { return (TabItem)PART_tabControl.SelectedItem; }
            set { PART_tabControl.SelectedItem = value; }
        }

        public ItemCollection Tabs
        {
            get { return PART_tabControl.Items; }
        }

        #endregion

        #region Конструктор

        public TabControlEx()
        {
            InitializeComponent();
        }

        #endregion

        #region Методы

        public void Add(TabItem value)
        {
            PART_tabControl.Items.Add(value);
        }

        public void Remove(TabItem value)
        {
            PART_tabControl.Items.Remove(value);
        }

        public void RemoveAll()
        {
            PART_tabControl.Items.Clear();
        }

        public void RemoveAllBut(TabItem value)
        {
            List<TabItem> itemsForRemove = new List<TabItem>();

            foreach (TabItem item in PART_tabControl.Items)
            {
                if (item != value)
                    itemsForRemove.Add(item);
            }

            foreach (TabItem item in itemsForRemove)
                PART_tabControl.Items.Remove(item);
        }

        #endregion
    }

    #endregion Класс TabControlEx
}