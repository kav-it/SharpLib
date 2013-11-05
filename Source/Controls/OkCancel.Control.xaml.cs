//*****************************************************************************
//
// Имя файла    : 'OkCancel.Control.cs'
// Заголовок    : Компонент "Кнопки ОК, Отмена" в группе
// Автор        : Крыцкий А.В.
// Контакты     : kav.it@mail.ru
// Дата         : 15/07/2012
//
//*****************************************************************************
			
using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace SharpLib
{
    #region Класс OkCancelControl
    [DefaultEvent("OkClick")]
    public partial class OkCancelControl : UserControl
    {
        #region Свойства
        [Browsable(true), Category("Common"), Description("Горизонтальное выравнивание положения кнопок")]
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
        #endregion Свойства

        #region События
        public event RoutedEventHandler OkClick
        {
            add    { AddHandler(OkClickEvent, value);    }
            remove { RemoveHandler(OkClickEvent, value); }
        }
        public event RoutedEventHandler CancelClick
        {
            add    { AddHandler(CancelClickEvent, value);    }
            remove { RemoveHandler(CancelClickEvent, value); }
        }        
        #endregion События

        #region Маршрутизируемые события
        public static readonly RoutedEvent OkClickEvent;
        public static readonly RoutedEvent CancelClickEvent;
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
        #endregion Конструктор

        #region Методы
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OkCancelControl.OkClickEvent, this));
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(OkCancelControl.CancelClickEvent, this));
        }
        #endregion Методы
    }
    #endregion Класс OkCancelControl
}
