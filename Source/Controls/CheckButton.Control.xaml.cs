// ****************************************************************************
//
// Имя файла    : 'CheckButton.Control.cs'
// Заголовок    : Нормальный ToggleButton 
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 30/09/2012
//
// ****************************************************************************

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace SharpLib
{

    #region Перечисление

    public enum CheckButtonView
    {
        Normal,

        Square,

        Circle
    }

    #endregion Перечисление

    #region Класс CheckButton

    [DefaultEvent("Click")]
    public partial class CheckButton : ToggleButton
    {
        #region Поля

        public static readonly DependencyProperty GroupProperty;

        public static readonly DependencyProperty IsDisableGroupProperty;

        public static readonly DependencyProperty ViewProperty;

        #endregion

        #region Свойства

        [Browsable(true)]
        [Category("Common")]
        [Description("Группа элемента")]
        public String Group
        {
            get { return (String)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        [Browsable(true)]
        [Category("Common")]
        [Description("Запрет выключения элемента в группе")]
        public Boolean IsDisableGroup
        {
            get { return (Boolean)GetValue(IsDisableGroupProperty); }
            set { SetValue(IsDisableGroupProperty, value); }
        }

        [Browsable(true)]
        [Category("Common")]
        [Description("Вид кнопки")]
        public CheckButtonView View
        {
            get { return (CheckButtonView)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        [Browsable(true)]
        [Category("Common")]
        [Description("Надпись (включено)")]
        public String TextCheck { get; set; }

        [Browsable(true)]
        [Category("Common")]
        [Description("Надпись (выключено)")]
        public String TextUncheck { get; set; }

        public Boolean IsCheck
        {
            get { return IsChecked == true; }
            set { IsChecked = value; }
        }

        #endregion

        #region Конструктор

        static CheckButton()
        {
            GroupProperty = DependencyProperty.Register("Group", typeof(String), typeof(CheckButton), new PropertyMetadata(""));
            IsDisableGroupProperty = DependencyProperty.Register("IsDisableGroup", typeof(Boolean), typeof(CheckButton), new PropertyMetadata(true));
            ViewProperty = DependencyProperty.Register("View", typeof(CheckButtonView), typeof(CheckButton), new PropertyMetadata(CheckButtonView.Normal));
        }

        public CheckButton()
        {
            InitializeComponent();
        }

        #endregion

        #region Методы

        private void UpdateText()
        {
            if (View == CheckButtonView.Normal)
            {
                if (TextCheck.IsValid() && TextUncheck.IsValid())
                {
                    Content = IsChecked == true ? TextUncheck : TextCheck;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            UpdateText();

            base.OnApplyTemplate();
        }

        protected override void OnToggle()
        {
            // Переключение зависимых элементов
            if (String.IsNullOrEmpty(Group) == false)
            {
                // Поиск всех элементов с подобной группой
                DependencyObject parent = Parent;

                int childAmount = VisualTreeHelper.GetChildrenCount(parent);

                for (int i = 0; i < childAmount; i++)
                {
                    CheckButton button = VisualTreeHelper.GetChild(parent, i) as CheckButton;

                    if (button != null && button.Equals(this) == false && Group == button.Group)
                        button.IsChecked = false;
                }
            }

            // Элемент принадлежит группе
            if (String.IsNullOrEmpty(Group) == false && IsDisableGroup && IsChecked != false)
                return;

            // Переключение состоянния текущего элемента
            base.OnToggle();

            // Смена подписи
            UpdateText();
        }

        #endregion
    }

    #endregion Класс CheckButton

    #region Класс CheckButtonStyleConverter

    public class CheckButtonStyleConverter : IMultiValueConverter
    {
        #region Методы

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FrameworkElement targetElement = values[0] as FrameworkElement;

            if (values[1] != null && values[1] is CheckButtonView)
            {
                CheckButtonView view = (CheckButtonView)values[1];

                switch (view)
                {
                    case CheckButtonView.Square:
                        return targetElement.TryFindResource("PlusMinusSquareStyle");
                    case CheckButtonView.Circle:
                        return targetElement.TryFindResource("PlusMinusCircleStyle");
                }
            }

            return targetElement.Style;

            /// return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion Класс CheckButtonStyleConverter
}