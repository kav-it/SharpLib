using System.ComponentModel;
using System.Windows;

namespace SharpLib.Wpf.Controls
{
    public class ListViewColumn : DependencyObject
    {
        #region Поля

        public static readonly DependencyProperty FieldNameProperty;

        public static readonly DependencyProperty TitleProperty;

        public static readonly DependencyProperty WidthProperty;

        #endregion

        #region Свойства

        [Category("SharpLib")]
        public string FieldName
        {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }

        [Category("SharpLib")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        [Category("SharpLib")]
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        #endregion

        #region Конструктор

        static ListViewColumn()
        {
            FieldNameProperty = DependencyProperty.Register("FieldName", typeof(string), typeof(ListViewColumn), new PropertyMetadata(string.Empty));

            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ListViewColumn), new PropertyMetadata(string.Empty));

            WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(ListViewColumn), new PropertyMetadata(100.0));
        }

        #endregion
    }
}