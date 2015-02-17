using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    [TemplatePart(Name = PART_MENU)]
    [TemplatePart(Name = PART_TOGGLE)]
    [TemplatePart(Name = PART_BUTTON)]
    [TemplatePart(Name = PART_DROP_DOWN)]
    public class BreadcrumbButton : HeaderedItemsControl
    {
        #region Перечисления

        public enum ButtonMode
        {
            Breadcrumb,

            Overflow,

            DropDown
        }

        #endregion

        #region Константы

        private const string PART_BUTTON = "PART_button";

        private const string PART_DROP_DOWN = "PART_DropDown";

        private const string PART_MENU = "PART_Menu";

        private const string PART_TOGGLE = "PART_Toggle";

        #endregion

        #region Поля

        public static readonly RoutedEvent ClickEvent;

        public static readonly DependencyProperty DropDownContentTemplateProperty;

        public static readonly DependencyProperty EnableVisualButtonStyleProperty;

        public static readonly DependencyProperty ImageProperty;

        public static readonly DependencyProperty IsButtonVisibleProperty;

        public static readonly DependencyProperty IsDropDownPressedProperty;

        public static readonly DependencyProperty IsDropDownVisibleProperty;

        public static readonly DependencyProperty IsImageVisibleProperty;

        public static readonly DependencyProperty IsPressedProperty;

        public static readonly DependencyProperty ModeProperty;

        public static readonly RoutedEvent SelectedItemChanged;

        public static readonly DependencyProperty SelectedItemProperty;

        private static readonly RoutedUICommand _openOverflowCommand;

        private static readonly RoutedUICommand _selectCommand;

        private ContextMenu _contextMenu;

        private Control _dropDownBtn;

        private bool _isPressed;

        #endregion

        #region Свойства

        public static RoutedUICommand OpenOverflowCommand
        {
            get { return _openOverflowCommand; }
        }

        public static RoutedUICommand SelectCommand
        {
            get { return _selectCommand; }
        }

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public ButtonMode Mode
        {
            get { return (ButtonMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }

        public bool IsDropDownPressed
        {
            get { return (bool)GetValue(IsDropDownPressedProperty); }
            set { SetValue(IsDropDownPressedProperty, value); }
        }

        public DataTemplate DropDownContentTemplate
        {
            get { return (DataTemplate)GetValue(DropDownContentTemplateProperty); }
            set { SetValue(DropDownContentTemplateProperty, value); }
        }

        public bool IsDropDownVisible
        {
            get { return (bool)GetValue(IsDropDownVisibleProperty); }
            set { SetValue(IsDropDownVisibleProperty, value); }
        }

        public bool IsButtonVisible
        {
            get { return (bool)GetValue(IsButtonVisibleProperty); }
            set { SetValue(IsButtonVisibleProperty, value); }
        }

        public bool IsImageVisible
        {
            get { return (bool)GetValue(IsImageVisibleProperty); }
            set { SetValue(IsImageVisibleProperty, value); }
        }

        public bool EnableVisualButtonStyle
        {
            get { return (bool)GetValue(EnableVisualButtonStyleProperty); }
            set { SetValue(EnableVisualButtonStyleProperty, value); }
        }

        #endregion

        #region События

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public event RoutedEventHandler Select
        {
            add { AddHandler(SelectedItemChanged, value); }
            remove { RemoveHandler(SelectedItemChanged, value); }
        }

        #endregion

        #region Конструктор

        static BreadcrumbButton()
        {
            ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbButton));
            DropDownContentTemplateProperty = DependencyProperty.Register("DropDownContentTemplate", typeof(DataTemplate), typeof(BreadcrumbButton), new UIPropertyMetadata(null));
            EnableVisualButtonStyleProperty = DependencyProperty.Register("EnableVisualButtonStyle", typeof(bool), typeof(BreadcrumbButton), new UIPropertyMetadata(true));
            ImageProperty = DependencyProperty.Register("Image", typeof(object), typeof(BreadcrumbButton), new UIPropertyMetadata(null));
            IsButtonVisibleProperty = DependencyProperty.Register("IsButtonVisible", typeof(bool), typeof(BreadcrumbButton), new UIPropertyMetadata(true));
            IsDropDownPressedProperty = DependencyProperty.Register("IsDropDownPressed", typeof(bool), typeof(BreadcrumbButton), new UIPropertyMetadata(false, OverflowPressedChanged));
            IsDropDownVisibleProperty = DependencyProperty.Register("IsDropDownVisible", typeof(bool), typeof(BreadcrumbButton), new UIPropertyMetadata(true));
            IsImageVisibleProperty = DependencyProperty.Register("IsImageVisible", typeof(bool), typeof(BreadcrumbButton), new UIPropertyMetadata(true));
            IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), typeof(BreadcrumbButton), new UIPropertyMetadata(false));
            ModeProperty = DependencyProperty.Register("Mode", typeof(ButtonMode), typeof(BreadcrumbButton), new UIPropertyMetadata(ButtonMode.Breadcrumb));
            SelectedItemChanged = EventManager.RegisterRoutedEvent("SelectedItemChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbButton));
            SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(BreadcrumbButton), new UIPropertyMetadata(null, SelectedItemChangedEvent));
            _openOverflowCommand = new RoutedUICommand("Open Overflow", "OpenOverflowCommand", typeof(BreadcrumbButton));
            _selectCommand = new RoutedUICommand("Select", "SelectCommand", typeof(BreadcrumbButton));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbButton), new FrameworkPropertyMetadata(typeof(BreadcrumbButton)));
        }

        public BreadcrumbButton()
        {
            CommandBindings.Add(new CommandBinding(SelectCommand, SelectCommandExecuted));
            CommandBindings.Add(new CommandBinding(OpenOverflowCommand, OpenOverflowCommandExecuted, OpenOverflowCommandCanExecute));

            InputBindings.Add(new KeyBinding(SelectCommand, new KeyGesture(Key.Enter)));
            InputBindings.Add(new KeyBinding(SelectCommand, new KeyGesture(Key.Space)));
            InputBindings.Add(new KeyBinding(OpenOverflowCommand, new KeyGesture(Key.Down)));
            InputBindings.Add(new KeyBinding(OpenOverflowCommand, new KeyGesture(Key.Up)));
        }

        #endregion

        #region Методы

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            IsPressed = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            IsPressed = _isPressed = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (_isPressed)
            {
                var args = new RoutedEventArgs(ClickEvent);
                RaiseEvent(args);
                _selectCommand.Execute(null, this);
            }
            IsPressed = _isPressed = false;
            base.OnMouseUp(e);
        }

        private void SelectCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedItem = null;
            var args = new RoutedEventArgs(ButtonBase.ClickEvent);
            RaiseEvent(args);
        }

        private void OpenOverflowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            IsDropDownPressed = true;
        }

        private void OpenOverflowCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Items.Count > 0;
        }

        public override void OnApplyTemplate()
        {
            _dropDownBtn = GetTemplateChild(PART_DROP_DOWN) as Control;
            _contextMenu = GetTemplateChild(PART_MENU) as ContextMenu;
            if (_contextMenu != null)
            {
                _contextMenu.Opened += contextMenu_Opened;
            }
            if (_dropDownBtn != null)
            {
                _dropDownBtn.MouseDown += dropDownBtn_MouseDown;
            }
            base.OnApplyTemplate();
        }

        private void dropDownBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            IsDropDownPressed ^= true;
        }

        private void contextMenu_Opened(object sender, RoutedEventArgs e)
        {
            _contextMenu.Items.Clear();
            _contextMenu.ItemTemplate = ItemTemplate;
            _contextMenu.ItemTemplateSelector = ItemTemplateSelector;
            foreach (object item in Items)
            {
                if (!(item is MenuItem) && !(item is Separator))
                {
                    var menuItem = new MenuItem();
                    menuItem.DataContext = item;
                    var bi = item as BreadcrumbItem;
                    if (bi == null)
                    {
                        var parent = TemplatedParent as BreadcrumbItem;
                        if (parent != null)
                        {
                            bi = parent.ContainerFromItem(item);
                        }
                    }
                    if (bi != null)
                    {
                        menuItem.Header = bi.TraceValue;
                    }

                    var image = new Image();
                    image.Source = bi != null ? bi.Image : null;
                    image.SnapsToDevicePixels = true;
                    image.Stretch = Stretch.Fill;
                    image.VerticalAlignment = VerticalAlignment.Center;
                    image.HorizontalAlignment = HorizontalAlignment.Center;
                    image.Width = 16;
                    image.Height = 16;

                    menuItem.Icon = image;

                    menuItem.Click += item_Click;
                    if (item != null && item.Equals(SelectedItem))
                    {
                        menuItem.FontWeight = FontWeights.Bold;
                    }
                    menuItem.ItemTemplate = ItemTemplate;
                    menuItem.ItemTemplateSelector = ItemTemplateSelector;
                    _contextMenu.Items.Add(menuItem);
                }
                else
                {
                    _contextMenu.Items.Add(item);
                }
            }
            _contextMenu.Placement = PlacementMode.Relative;
            _contextMenu.PlacementTarget = _dropDownBtn;
            _contextMenu.VerticalOffset = _dropDownBtn.ActualHeight;
        }

        private void item_Click(object sender, RoutedEventArgs e)
        {
            var item = e.Source as MenuItem;
            if (item != null)
            {
                var dataItem = item.DataContext;
                RemoveSelectedItem(dataItem);
                SelectedItem = dataItem;
            }
        }

        private void RemoveSelectedItem(object dataItem)
        {
            if (dataItem != null && dataItem.Equals(SelectedItem))
            {
                SelectedItem = null;
            }
        }

        private static void SelectedItemChangedEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as BreadcrumbButton;
            if (button != null && button.IsInitialized)
            {
                var args = new RoutedEventArgs(SelectedItemChanged);
                button.RaiseEvent(args);
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            _isPressed = e.LeftButton == MouseButtonState.Pressed;
            var parent = TemplatedParent as FrameworkElement;
            while (parent != null && !(parent is BreadcrumbBar))
            {
                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
            var bar = parent as BreadcrumbBar;
            if (bar != null && bar.IsKeyboardFocusWithin)
            {
                Focus();
            }
            IsPressed = _isPressed;
            base.OnMouseEnter(e);
        }

        private static void OverflowPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as BreadcrumbButton;
            if (button != null)
            {
                button.OnOverflowPressedChanged();
            }
        }

        protected virtual void OnOverflowPressedChanged()
        {
        }

        #endregion
    }
}