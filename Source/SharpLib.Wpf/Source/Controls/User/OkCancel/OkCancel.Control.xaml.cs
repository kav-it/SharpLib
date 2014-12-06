using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SharpLib.Wpf.Controls
{
    /// <summary>
    /// Класс панели с кнопками "ОК", "Отмена"
    /// </summary>
    [DefaultEvent("OkClick")]
    public partial class OkCancelControl
    {
        #region Поля

        /// <summary>
        /// Событие "Cancel"
        /// </summary>
        public static readonly RoutedEvent CancelClickEvent;

        /// <summary>
        /// Событие "OK"
        /// </summary>
        public static readonly RoutedEvent OkClickEvent;

        /// <summary>
        /// Команда обработки нажатия "ОК"
        /// </summary>
        public static DependencyProperty OkCommandProperty;

        /// <summary>
        /// Параметр команды обработки нажатия "ОК"
        /// </summary>
        public static DependencyProperty OkCommandParameterProperty;

        #endregion

        #region Свойства

        [Browsable(true)]
        [Category("SharpLib")]
        [Description("Горизонтальное выравнивание положения кнопок")]
        public HorizontalAlignment Align
        {
            get { return PART_stackPanel.HorizontalAlignment; }
            set { PART_stackPanel.HorizontalAlignment = value; }
        }

        [Browsable(false)]
        public ButtonEx ButtonOk
        {
            get { return PART_buttonOk; }
            set { PART_buttonOk = value; }
        }

        /// <summary>
        /// Команда обработки нажатия "ОК"
        /// </summary>
        [Browsable(false)]
        public ICommand OkCommand
        {
            get { return (ICommand)GetValue(OkCommandProperty); }
            set { SetValue(OkCommandProperty, value); }
        }

        /// <summary>
        /// Параметры команды обработки нажатия "ОК"
        /// </summary>
        [Browsable(false)]
        public object OkCommandParameter
        {
            get { return GetValue(OkCommandParameterProperty); }
            set { SetValue(OkCommandParameterProperty, value); }
        }

        #endregion

        #region События

        /// <summary>
        /// Событие "Cancel"
        /// </summary>
        public event RoutedEventHandler CancelClick
        {
            add { AddHandler(CancelClickEvent, value); }
            remove { RemoveHandler(CancelClickEvent, value); }
        }

        /// <summary>
        /// Событие "OK"
        /// </summary>
        public event RoutedEventHandler OkClick
        {
            add { AddHandler(OkClickEvent, value); }
            remove { RemoveHandler(OkClickEvent, value); }
        }

        #endregion

        #region Конструктор

        static OkCancelControl()
        {
            
            OkClickEvent = EventManager.RegisterRoutedEvent("OkClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OkCancelControl));
            CancelClickEvent = EventManager.RegisterRoutedEvent("CancelClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(OkCancelControl));
            OkCommandProperty = DependencyProperty.Register("OkCommand", typeof(ICommand), typeof(OkCancelControl));
            OkCommandParameterProperty = DependencyProperty.Register("OkCommandParameter", typeof(object), typeof(OkCancelControl), new FrameworkPropertyMetadata((object)null));
        }

        public OkCancelControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обработка нажатия "ОК"
        /// </summary>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (OkClickEvent != null)
            {
                RaiseEvent(new RoutedEventArgs(OkClickEvent, this));
            }
        }

        /// <summary>
        /// Обработка нажатия "Отмена"
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CancelClickEvent, this));
        }

        #endregion

    }
}