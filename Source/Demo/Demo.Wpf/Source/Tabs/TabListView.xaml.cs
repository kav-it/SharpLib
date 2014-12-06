using System.Collections.Generic;

namespace DemoWpf
{
    public partial class TabListView
    {
        #region Конструктор

        public TabListView()
        {
            InitializeComponent();

            InitGrid();
        }

        private void InitGrid()
        {
            var list = new List<ClassA>();

            for (int i = 0; i < 30; i++)
            {
                var item = new ClassA()
                {
                    Value1 = i,
                    Value2 = i + 1,
                    Value3 = i * 3
                };

                list.Add(item);
            }

            PART_listView.ItemsSource = list;
            PART_listView2.ItemsSource = list;
        }

        #endregion

        public class ClassA
        {
            public int Value1 { get; set; }

            public int Value2 { get; set; }

            public int Value3 { get; set; }
        }
    }
}