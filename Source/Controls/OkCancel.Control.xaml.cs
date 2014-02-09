//*****************************************************************************
//
// Имя файла    : 'OkCancel.Control.cs'
// Заголовок    : Компонент "Кнопки ОК, Отмена" в группе
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 15/07/2012
//
//*****************************************************************************

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SharpLib
{

    #region Класс OkCancelControl

    [DefaultEvent("OkClick")]
    public partial class OkCancelControl : UserControl
    {
        #region Свойства

        [Browsable(true)]
        [Category("Common")]
        [Description("Горизонтальное выравнивание положения кнопок")]
        public HorizontalAlignment Align
        {
            get { return PART_stackPanel.HorizontalAlignment; }
            set { PART_stackPanel.HorizontalAlignment = value; }
        }

        public Button ButtonOk
        {
            get { return PART_buttonOk; }
            set { PART_buttonOk = value; }
        }

        #endregion

        #region События

        public event RoutedEventHandler CancelClick
        {
            add { AddHandler(CancelClickEvent, value); }
            remove { RemoveHandler(CancelClickEvent, value); }
        }

        public event RoutedEventHandler OkClick
        {
            add { AddHandler(OkClickEvent, value); }
            remove { RemoveHandler(OkClickEvent, value); }
        }

        #endregion

        #region Маршрутизируемые события

        public static readonly RoutedEvent CancelClickEvent;

        public static readonly RoutedEvent OkClickEvent;

        #endregion Маршрутизируемые события

        #region Конструктор

        static OkCancelControl()
        {
            OkClickEvent = EventManager.RegisterRoutedEvent("OkClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OkCancelControl));
            CancelClickEvent = EventManager.RegisterRoutedEvent("CancelClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OkCancelControl));
        }

        public OkCancelControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Методы

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OkCancelControl.OkClickEvent, this));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OkCancelControl.CancelClickEvent, this));
        }

        #endregion
    }

    #endregion Класс OkCancelControl
}