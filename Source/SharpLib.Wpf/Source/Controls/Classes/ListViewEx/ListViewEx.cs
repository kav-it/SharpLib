using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SharpLib.Wpf.Controls
{
    public class ListViewEx : Selector
    {
        #region Поля

        public static readonly DependencyProperty ColumnsProperty;

        #endregion

        #region Свойства

        [Category("SharpLib")]
        public ListViewColumnCollection Columns
        {
            get { return (ListViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        #endregion

        #region Конструктор

        static ListViewEx()
        {
            // Переопределение стиля по умолчанию
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListViewEx), new FrameworkPropertyMetadata(typeof(ListViewEx)));

            ColumnsProperty = DependencyProperty.Register("Columns", typeof(ListViewColumnCollection), typeof(ListViewEx), new PropertyMetadata(null));
        }

        public ListViewEx()
        {
            Columns = new ListViewColumnCollection();
        }

        #endregion
    }
}