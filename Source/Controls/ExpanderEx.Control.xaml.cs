//*****************************************************************************
//
// Имя файла    : 'ExpanderEx.Control.cs'
// Заголовок    : Компонент "Нормальный экспандер"
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 03/10/2012
//
//*****************************************************************************

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SharpLib
{

    #region Перечисление

    public enum ExpanderView
    {
        Animate,

        Square,

        Kaka
    }

    #endregion Перечисление

    #region Класс ExpanderEx

    public partial class ExpanderEx : Expander
    {
        #region Поля

        public static readonly DependencyProperty ViewProperty;

        #endregion

        #region Свойства

        [Browsable(true)]
        [Category("Common")]
        [Description("Вид компонента")]
        public ExpanderView View
        {
            get { return (ExpanderView)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        #endregion

        #region Конструктор

        static ExpanderEx()
        {
            ViewProperty = DependencyProperty.Register("View", typeof(ExpanderView), typeof(ExpanderEx),
                                                       new PropertyMetadata(ExpanderView.Square));
        }

        public ExpanderEx()
        {
            InitializeComponent();
        }

        #endregion
    }

    #endregion Класс ExpanderEx

    #region Класс ExpanderExStyleConverter

    public class ExpanderExStyleConverter : IMultiValueConverter
    {
        #region Методы

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FrameworkElement targetElement = values[0] as FrameworkElement;
            if (values[1] == null) return null;
            ExpanderView view = (ExpanderView)values[1];

            switch (view)
            {
                case ExpanderView.Animate:
                    return targetElement.TryFindResource("AnimatedExpanderStyle");
                case ExpanderView.Square:
                    return targetElement.TryFindResource("SquareExpanderStyle");
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion Класс ExpanderExStyleConverter
}