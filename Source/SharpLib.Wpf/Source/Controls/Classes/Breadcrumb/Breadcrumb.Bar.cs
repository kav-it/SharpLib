using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    [ContentProperty("Root")]
    [TemplatePart(Name = PART_COMBO_BOX)]
    [TemplatePart(Name = PART_ROOT)]
    public class BreadcrumbBar : Control, IAddChild
    {
        #region Константы

        private const int BREADCRUMBS_TO_HIDE = 1;

        private const string PART_COMBO_BOX = "PART_ComboBox";

        private const string PART_ROOT = "PART_Root";

        private const string SEPARATOR_DEFAULT = @"\";

        #endregion

        #region Поля

        public static readonly RoutedEvent ApplyPropertiesEvent;

        public static readonly RoutedEvent BreadcrumbItemDropDownClosedEvent;

        public static readonly RoutedEvent BreadcrumbItemDropDownOpenedEvent;

        public static readonly DependencyProperty BreadcrumbItemTemplateProperty;

        public static readonly DependencyProperty BreadcrumbItemTemplateSelectorProperty;

        public static readonly DependencyProperty CollapsedTracesProperty;

        public static readonly DependencyProperty DropDownItemTemplateProperty;

        public static readonly DependencyProperty DropDownItemTemplateSelectorProperty;

        public static readonly DependencyProperty DropDownItemsPanelProperty;

        public static readonly DependencyProperty DropDownItemsSourceProperty;

        public static readonly DependencyProperty HasDropDownItemsProperty;

        public static readonly DependencyProperty IsDropDownOpenProperty;

        public static readonly DependencyProperty IsEditableProperty;

        public static readonly DependencyProperty IsOverflowPressedProperty;

        public static readonly DependencyProperty IsRootSelectedProperty;

        public static readonly DependencyProperty OverflowItemTemplateProperty;

        public static readonly DependencyProperty OverflowItemTemplateSelectorProperty;

        public static readonly DependencyProperty OverflowModeProperty;

        public static readonly RoutedEvent PathChangedEvent;

        public static readonly RoutedEvent PathConversionEvent;

        public static readonly DependencyProperty PathProperty;

        public static readonly RoutedEvent PopulateItemsEvent;

        public static readonly DependencyProperty ProgressMaximumProperty;

        public static readonly DependencyProperty ProgressMinimumProperty;

        public static readonly RoutedEvent ProgressValueChangedEvent;

        public static readonly DependencyProperty ProgressValueProperty;

        public static readonly DependencyProperty RootProperty;

        public static readonly RoutedEvent SelectedBreadcrumbChangedEvent;

        public static readonly DependencyProperty SelectedBreadcrumbProperty;

        public static readonly DependencyProperty SelectedDropDownIndexProperty;

        public static readonly DependencyProperty SelectedItemProperty;

        public static readonly DependencyProperty SeparatorStringProperty;

        private static readonly DependencyPropertyKey _collapsedTracesPropertyKey;

        private static readonly DependencyPropertyKey _isRootSelectedPropertyKey;

        private static readonly DependencyPropertyKey _overflowModePropertyKey;

        private static readonly DependencyProperty _rootItemProperty;

        private static readonly DependencyPropertyKey _rootItemPropertyKey;

        private static readonly RoutedUICommand _selectRootCommand;

        private static readonly RoutedUICommand _selectTraceCommand;

        private static readonly DependencyPropertyKey _selectedBreadcrumbPropertyKey;

        private static readonly RoutedUICommand _showDropDownCommand;

        private readonly ObservableCollection<ButtonBase> _buttons;

        private readonly ItemsControl _comboBoxControlItems;

        private readonly ObservableCollection<object> _traces;

        /// <summary>
        /// Элемент "Редактирование пути вручную"
        /// </summary>
        private ComboBox _comboBox;

        /// <summary>
        /// 
        /// </summary>
        private string _initPath;

        /// <summary>
        /// 
        /// </summary>
        private BreadcrumbButton _rootButton;

        /// <summary>
        /// 
        /// </summary>
        private BreadcrumbItem _selectedBreadcrumb;

        #endregion

        #region Свойства

        public static RoutedUICommand ShowDropDownCommand
        {
            get { return _showDropDownCommand; }
        }

        public static RoutedUICommand SelectTraceCommand
        {
            get { return _selectTraceCommand; }
        }

        public static RoutedUICommand SelectRootCommand
        {
            get { return _selectRootCommand; }
        }

        public bool IsRootSelected
        {
            get { return (bool)GetValue(IsRootSelectedProperty); }
            private set { SetValue(_isRootSelectedPropertyKey, value); }
        }

        public DataTemplateSelector OverflowItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(OverflowItemTemplateSelectorProperty); }
            set { SetValue(OverflowItemTemplateSelectorProperty, value); }
        }

        public DataTemplate OverflowItemTemplate
        {
            get { return (DataTemplate)GetValue(OverflowItemTemplateProperty); }
            set { SetValue(OverflowItemTemplateProperty, value); }
        }

        public IEnumerable CollapsedTraces
        {
            get { return (IEnumerable)GetValue(CollapsedTracesProperty); }
            private set { SetValue(_collapsedTracesPropertyKey, value); }
        }

        public object Root
        {
            get { return GetValue(RootProperty); }
            set { SetValue(RootProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemProperty, value); }
        }

        public BreadcrumbItem SelectedBreadcrumb
        {
            get { return (BreadcrumbItem)GetValue(SelectedBreadcrumbProperty); }
            private set
            {
                _selectedBreadcrumb = value;
                SetValue(_selectedBreadcrumbPropertyKey, value);
            }
        }

        public bool IsOverflowPressed
        {
            get { return (bool)GetValue(IsOverflowPressedProperty); }
            private set { SetValue(IsOverflowPressedProperty, value); }
        }

        public BreadcrumbItem RootItem
        {
            get { return (BreadcrumbItem)GetValue(_rootItemProperty); }
            protected set { SetValue(_rootItemPropertyKey, value); }
        }

        public DataTemplateSelector BreadcrumbItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(BreadcrumbItemTemplateSelectorProperty); }
            set { SetValue(BreadcrumbItemTemplateSelectorProperty, value); }
        }

        public DataTemplate BreadcrumbItemTemplate
        {
            get { return (DataTemplate)GetValue(BreadcrumbItemTemplateProperty); }
            set { SetValue(BreadcrumbItemTemplateProperty, value); }
        }

        public BreadcrumbButton.ButtonMode OverflowMode
        {
            get { return (BreadcrumbButton.ButtonMode)GetValue(OverflowModeProperty); }
            private set { SetValue(_overflowModePropertyKey, value); }
        }

        public IEnumerable DropDownItemsSource
        {
            get { return (IEnumerable)GetValue(DropDownItemsSourceProperty); }
            set { SetValue(DropDownItemsSourceProperty, value); }
        }

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public string SeparatorString
        {
            get { return (string)GetValue(SeparatorStringProperty); }
            set { SetValue(SeparatorStringProperty, value); }
        }

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public ObservableCollection<ButtonBase> Buttons
        {
            get { return _buttons; }
        }

        public ItemCollection DropDownItems
        {
            get { return _comboBoxControlItems.Items; }
        }

        public bool HasDropDownItems
        {
            get { return (bool)GetValue(HasDropDownItemsProperty); }
            private set { SetValue(HasDropDownItemsProperty, value); }
        }

        public ItemsPanelTemplate DropDownItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(DropDownItemsPanelProperty); }
            set { SetValue(DropDownItemsPanelProperty, value); }
        }

        public DataTemplateSelector DropDownItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DropDownItemTemplateSelectorProperty); }
            set { SetValue(DropDownItemTemplateSelectorProperty, value); }
        }

        public DataTemplate DropDownItemTemplate
        {
            get { return (DataTemplate)GetValue(DropDownItemTemplateProperty); }
            set { SetValue(DropDownItemTemplateProperty, value); }
        }

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public int SelectedDropDownIndex
        {
            get { return (int)GetValue(SelectedDropDownIndexProperty); }
            set { SetValue(SelectedDropDownIndexProperty, value); }
        }

        public double ProgressValue
        {
            get { return (double)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }

        public double ProgressMaximum
        {
            get { return (double)GetValue(ProgressMaximumProperty); }
            set { SetValue(ProgressMaximumProperty, value); }
        }

        public double ProgressMinimum
        {
            get { return (double)GetValue(ProgressMinimumProperty); }
            set { SetValue(ProgressMinimumProperty, value); }
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                object content = RootItem;

                if (content == null)
                {
                    return base.LogicalChildren;
                }
                if (TemplatedParent != null)
                {
                    var current = content as DependencyObject;
                    if (current != null)
                    {
                        var parent = LogicalTreeHelper.GetParent(current);
                        if ((parent != null) && (!Equals(parent, this)))
                        {
                            return base.LogicalChildren;
                        }
                    }
                }

                object[] array = { RootItem };
                return array.GetEnumerator();
            }
        }

        public BindingBase TraceBinding { get; set; }

        public BindingBase ImageBinding { get; set; }

        #endregion

        #region События

        public event ApplyPropertiesEventHandler ApplyProperties
        {
            add { AddHandler(ApplyPropertiesEvent, value); }
            remove { RemoveHandler(ApplyPropertiesEvent, value); }
        }

        public event BreadcrumbItemEventHandler BreadcrumbItemDropDownClosed
        {
            add { AddHandler(BreadcrumbItemDropDownClosedEvent, value); }
            remove { RemoveHandler(BreadcrumbItemDropDownClosedEvent, value); }
        }

        public event BreadcrumbItemEventHandler BreadcrumbItemDropDownOpened
        {
            add { AddHandler(BreadcrumbItemDropDownOpenedEvent, value); }
            remove { RemoveHandler(BreadcrumbItemDropDownOpenedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<string> PathChanged
        {
            add { AddHandler(PathChangedEvent, value); }
            remove { RemoveHandler(PathChangedEvent, value); }
        }

        public event PathConversionEventHandler PathConversion
        {
            add { AddHandler(PathConversionEvent, value); }
            remove { RemoveHandler(PathConversionEvent, value); }
        }

        public event BreadcrumbItemEventHandler PopulateItems
        {
            add { AddHandler(PopulateItemsEvent, value); }
            remove { RemoveHandler(PopulateItemsEvent, value); }
        }

        public event RoutedEventHandler ProgressValueChanged
        {
            add { AddHandler(ProgressValueChangedEvent, value); }
            remove { RemoveHandler(ProgressValueChangedEvent, value); }
        }

        public event RoutedEventHandler SelectedBreadcrumbChanged
        {
            add { AddHandler(SelectedBreadcrumbChangedEvent, value); }
            remove { RemoveHandler(SelectedBreadcrumbChangedEvent, value); }
        }

        #endregion

        #region Конструктор

        static BreadcrumbBar()
        {
            _isRootSelectedPropertyKey = DependencyProperty.RegisterReadOnly("IsRootSelected", typeof(bool), typeof(BreadcrumbBar), new UIPropertyMetadata(true));
            _overflowModePropertyKey = DependencyProperty.RegisterReadOnly("OverflowMode", typeof(BreadcrumbButton.ButtonMode), typeof(BreadcrumbBar),
                new FrameworkPropertyMetadata(BreadcrumbButton.ButtonMode.Overflow, FrameworkPropertyMetadataOptions.AffectsRender));
            _collapsedTracesPropertyKey = DependencyProperty.RegisterReadOnly("CollapsedTraces", typeof(IEnumerable), typeof(BreadcrumbBar), new UIPropertyMetadata(null));
            _rootItemPropertyKey = DependencyProperty.RegisterReadOnly("RootItem", typeof(BreadcrumbItem), typeof(BreadcrumbBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsMeasure));
            _selectedBreadcrumbPropertyKey = DependencyProperty.RegisterReadOnly("SelectedBreadcrumb", typeof(BreadcrumbItem), typeof(BreadcrumbBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange,
                    SelectedBreadcrumbPropertyChanged));

            CollapsedTracesProperty = _collapsedTracesPropertyKey.DependencyProperty;
            _rootItemProperty = _rootItemPropertyKey.DependencyProperty;
            OverflowModeProperty = _overflowModePropertyKey.DependencyProperty;
            IsRootSelectedProperty = _isRootSelectedPropertyKey.DependencyProperty;
            SelectedBreadcrumbProperty = _selectedBreadcrumbPropertyKey.DependencyProperty;

            ApplyPropertiesEvent = EventManager.RegisterRoutedEvent("ApplyProperties", RoutingStrategy.Bubble, typeof(ApplyPropertiesEventHandler), typeof(BreadcrumbBar));
            BreadcrumbItemDropDownClosedEvent = EventManager.RegisterRoutedEvent("BreadcrumbItemDropDownClosed", RoutingStrategy.Bubble, typeof(BreadcrumbItemEventHandler), typeof(BreadcrumbBar));
            BreadcrumbItemDropDownOpenedEvent = EventManager.RegisterRoutedEvent("BreadcrumbItemDropDownOpened", RoutingStrategy.Bubble, typeof(BreadcrumbItemEventHandler), typeof(BreadcrumbBar));
            BreadcrumbItemTemplateProperty = DependencyProperty.Register("BreadcrumbItemTemplate", typeof(DataTemplate), typeof(BreadcrumbBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
            BreadcrumbItemTemplateSelectorProperty = DependencyProperty.Register("BreadcrumbItemTemplateSelector", typeof(DataTemplateSelector), typeof(BreadcrumbBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

            DropDownItemTemplateProperty = DependencyProperty.Register("DropDownItemTemplate", typeof(DataTemplate), typeof(BreadcrumbBar), new UIPropertyMetadata(null));
            DropDownItemTemplateSelectorProperty = DependencyProperty.Register("DropDownItemTemplateSelector", typeof(DataTemplateSelector), typeof(BreadcrumbBar), new UIPropertyMetadata(null));
            DropDownItemsPanelProperty = DependencyProperty.Register("DropDownItemsPanel", typeof(ItemsPanelTemplate), typeof(BreadcrumbBar), new UIPropertyMetadata(null));
            DropDownItemsSourceProperty = DependencyProperty.Register("DropDownItemsSource", typeof(IEnumerable), typeof(BreadcrumbBar),
                new UIPropertyMetadata(null, DropDownItemsSourcePropertyChanged));
            HasDropDownItemsProperty = DependencyProperty.Register("HasDropDownItems", typeof(bool), typeof(BreadcrumbBar), new UIPropertyMetadata(false));
            IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(BreadcrumbBar), new UIPropertyMetadata(false, IsDropDownOpenChanged));
            IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(BreadcrumbBar), new UIPropertyMetadata(true));
            IsOverflowPressedProperty = DependencyProperty.Register("IsOverflowPressed", typeof(bool), typeof(BreadcrumbBar), new UIPropertyMetadata(false, OverflowPressedChanged));
            OverflowItemTemplateProperty = DependencyProperty.Register("OverflowItemTemplate", typeof(DataTemplate), typeof(BreadcrumbBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
            OverflowItemTemplateSelectorProperty = DependencyProperty.Register("OverflowItemTemplateSelector", typeof(DataTemplateSelector), typeof(BreadcrumbBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
            PathChangedEvent = EventManager.RegisterRoutedEvent("PathChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(BreadcrumbBar));
            PathConversionEvent = EventManager.RegisterRoutedEvent("PathConversion", RoutingStrategy.Bubble, typeof(PathConversionEventHandler), typeof(BreadcrumbBar));
            PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(BreadcrumbBar), new UIPropertyMetadata(null, PathPropertyChanged));
            PopulateItemsEvent = EventManager.RegisterRoutedEvent("PopulateItems", RoutingStrategy.Bubble, typeof(BreadcrumbItemEventHandler), typeof(BreadcrumbBar));
            ProgressMaximumProperty = DependencyProperty.Register("ProgressMaximum", typeof(double), typeof(BreadcrumbBar), new UIPropertyMetadata(100.0, null, CoerceProgressMaximum));
            ProgressMinimumProperty = DependencyProperty.Register("ProgressMinimum", typeof(double), typeof(BreadcrumbBar), new UIPropertyMetadata(0.0, null, CoerceProgressMinimum));
            ProgressValueChangedEvent = EventManager.RegisterRoutedEvent("ProgressValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbBar));
            ProgressValueProperty = DependencyProperty.Register("ProgressValue", typeof(double), typeof(BreadcrumbBar), new UIPropertyMetadata(0.0, ProgressValuePropertyChanged, CoerceProgressValue));
            RootProperty = DependencyProperty.Register("Root", typeof(object), typeof(BreadcrumbBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange,
                    RootPropertyChanged));
            SelectedBreadcrumbChangedEvent = EventManager.RegisterRoutedEvent("SelectedBreadcrumbChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<BreadcrumbItem>),
                typeof(BreadcrumbBar));
            SelectedDropDownIndexProperty = DependencyProperty.Register("SelectedDropDownIndex", typeof(int), typeof(BreadcrumbBar), new UIPropertyMetadata(-1));
            SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(BreadcrumbBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsMeasure,
                    SelectedItemPropertyChanged));
            SeparatorStringProperty = DependencyProperty.Register("SeparatorString", typeof(string), typeof(BreadcrumbBar), new UIPropertyMetadata(SEPARATOR_DEFAULT));

            _selectRootCommand = new RoutedUICommand("Select", "SelectRootCommand", typeof(BreadcrumbBar));
            _selectTraceCommand = new RoutedUICommand("Select", "SelectTraceCommand", typeof(BreadcrumbBar));
            _showDropDownCommand = new RoutedUICommand("Show DropDown", "ShowDropDownCommand", typeof(BreadcrumbBar));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbBar), new FrameworkPropertyMetadata(typeof(BreadcrumbBar)));
            BorderThicknessProperty.OverrideMetadata(typeof(BreadcrumbBar), new FrameworkPropertyMetadata(new Thickness(1)));
            BorderBrushProperty.OverrideMetadata(typeof(BreadcrumbBar), new FrameworkPropertyMetadata(Brushes.Black));

            var c = new Color
            {
                R = 245,
                G = 245,
                B = 245,
                A = 255
            };

            BackgroundProperty.OverrideMetadata(typeof(BreadcrumbBar), new FrameworkPropertyMetadata(new SolidColorBrush(c)));

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), new CommandBinding(_selectRootCommand, SelectRootCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), new CommandBinding(_selectTraceCommand, SelectTraceCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), new CommandBinding(_showDropDownCommand, ShowDropDownExecuted));
        }

        public BreadcrumbBar()
        {
            _buttons = new ObservableCollection<ButtonBase>();
            _comboBoxControlItems = new ItemsControl();
            var b = new Binding("HasItems");
            b.Source = _comboBoxControlItems;
            SetBinding(HasDropDownItemsProperty, b);

            _traces = new ObservableCollection<object>();
            CollapsedTraces = _traces;
            AddHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(BreadcrumbItemSelectedItemChangedHandler));
            AddHandler(BreadcrumbItem.TraceChangedEvent, new RoutedEventHandler(BreadcrumbItemTraceValueChangedHandler));
            AddHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(BreadcrumbItemSelectionChangedEventHandler));
            AddHandler(BreadcrumbItem.DropDownPressedChangedEvent, new RoutedEventHandler(BreadcrumbItemDropDownChangedEventHandler));
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ButtonClickedEventHandler));
            _traces.Add(null);

            InputBindings.Add(new KeyBinding(ShowDropDownCommand, new KeyGesture(Key.Down, ModifierKeys.Alt)));
        }

        #endregion

        #region Методы

        /// <summary>
        /// Реализация интерфейса "AddChild"
        /// </summary>
        void IAddChild.AddChild(object value)
        {
            Root = value;
        }

        /// <summary>
        /// Реализация интерфейса "AddChild"
        /// </summary>
        void IAddChild.AddText(string text)
        {
            ((IAddChild)this).AddChild(text);
        }

        /// <summary>
        /// Событие DP "IsDropDownOpenProperty"
        /// </summary>
        private static void IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bar = d as BreadcrumbBar;

            if (bar != null)
            {
                bar.RaiseDropDownOpenChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        /// <summary>
        /// Событие DP "SelectedBreadcrumbProperty"
        /// </summary>
        private static void SelectedBreadcrumbPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bar = d as BreadcrumbBar;
            var selected = e.NewValue as BreadcrumbItem;
            if (bar != null)
            {
                bar.IsRootSelected = Equals(selected, bar.RootItem);
                if (bar.IsInitialized)
                {
                    var args = new RoutedPropertyChangedEventArgs<BreadcrumbItem>(e.OldValue as BreadcrumbItem, e.NewValue as BreadcrumbItem, SelectedBreadcrumbChangedEvent);
                    bar.RaiseEvent(args);
                }
            }
        }

        /// <summary>
        /// Событие DP "PathProperty"
        /// </summary>
        private static void PathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bar = d as BreadcrumbBar;
            string newPath = e.NewValue as string;

            if (bar != null && !bar.IsInitialized)
            {
                bar.Path = bar._initPath = newPath;
            }
            else
            {
                if (bar != null)
                {
                    bar.BuildBreadcrumbsFromPath(newPath);
                    bar.RaisePathChanged(e.OldValue as string, newPath);
                }
            }
        }

        /// <summary>
        /// Событие DP "DropDownItemsSourceProperty"
        /// </summary>
        private static void DropDownItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bar = d as BreadcrumbBar;

            if (bar != null)
            {
                bar._comboBoxControlItems.ItemsSource = e.NewValue as IEnumerable;
            }
        }

        /// <summary>
        /// Событие DP "RootProperty"
        /// </summary>
        private static void RootPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bar = d as BreadcrumbBar;
            if (bar != null)
            {
                bar.OnRootChanged(e.OldValue, e.NewValue);
            }
        }

        /// <summary>
        /// Событие DP "SelectedItemProperty"
        /// </summary>
        private static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bar = d as BreadcrumbBar;
            if (bar != null)
            {
                bar.RaiseSelectedItemChanged(e.OldValue, e.NewValue);
            }
        }

        /// <summary>
        /// Событие DP "IsOverflowPressedProperty"
        /// </summary>
        private static void OverflowPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bar = d as BreadcrumbBar;
            if (bar != null)
            {
                bar.RaiseOverflowPressedChanged();
            }
        }

        /// <summary>
        /// Событие DP "ProgressValueProperty"
        /// </summary>
        private static object CoerceProgressValue(DependencyObject d, object baseValue)
        {
            var bar = d as BreadcrumbBar;
            double value = (double)baseValue;
            if (bar != null && value > bar.ProgressMaximum)
            {
                value = bar.ProgressMaximum;
            }
            if (bar != null && value < bar.ProgressMinimum)
            {
                value = bar.ProgressMinimum;
            }

            return value;
        }

        /// <summary>
        /// Событие DP "ProgressValueProperty"
        /// </summary>
        private static void ProgressValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var args = new RoutedEventArgs(ProgressValueChangedEvent);
            var bar = d as BreadcrumbBar;
            if (bar != null)
            {
                bar.RaiseEvent(args);
            }
        }

        /// <summary>
        /// Событие DP "ProgressMaximumProperty"
        /// </summary>
        private static object CoerceProgressMaximum(DependencyObject d, object baseValue)
        {
            var bar = d as BreadcrumbBar;
            double value = (double)baseValue;
            if (bar != null && value < bar.ProgressMinimum)
            {
                value = bar.ProgressMinimum;
            }
            if (bar != null && value < bar.ProgressValue)
            {
                bar.ProgressValue = value;
            }
            if (value < 0)
            {
                value = 0;
            }

            return value;
        }

        /// <summary>
        /// Событие DP "ProgressMinimumProperty"
        /// </summary>
        private static object CoerceProgressMinimum(DependencyObject d, object baseValue)
        {
            var bar = d as BreadcrumbBar;
            double value = (double)baseValue;
            if (bar != null && value > bar.ProgressMaximum)
            {
                value = bar.ProgressMaximum;
            }
            if (bar != null && value > bar.ProgressValue)
            {
                bar.ProgressValue = value;
            }

            return value;
        }

        /// <summary>
        /// Выполнение RoutedCommand "SelectRootCommand"
        /// </summary>
        private static void SelectRootCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as BreadcrumbItem;
            if (item != null)
            {
                item.SelectedItem = null;
            }
        }

        /// <summary>
        /// Выполнение RoutedCommand "SelectTraceCommand"
        /// </summary>
        private static void SelectTraceCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as BreadcrumbItem;
            if (item != null)
            {
                item.SelectedItem = null;
            }
        }

        /// <summary>
        /// Выполнение RoutedCommand "ShowDropDownCommand"
        /// </summary>
        private static void ShowDropDownExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var bar = sender as BreadcrumbBar;
            if (bar != null && (bar.IsEditable && bar.DropDownItems.Count > 0))
            {
                bar.IsDropDownOpen = true;
            }
        }

        /// <summary>
        /// Генерация события "PathChanged"
        /// </summary>
        protected virtual void RaisePathChanged(string oldValue, string newValue)
        {
            BuildBreadcrumbsFromPath(Path);
            if (IsLoaded)
            {
                var args = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue, PathChangedEvent);
                RaiseEvent(args);
            }
        }

        /// <summary>
        /// Генерация события "DropDownOpenChanged"
        /// </summary>
        protected virtual void RaiseDropDownOpenChanged(bool oldValue, bool newValue)
        {
            if (_comboBox != null && newValue)
            {
                EnterToInputState();
                if (IsEditable)
                {
                    _comboBox.Visibility = Visibility.Visible;
                    _comboBox.IsDropDownOpen = true;
                }
            }
        }

        /// <summary>
        /// Генерация события "PopulateItems"
        /// </summary>
        protected virtual void RaisePopulateItems(BreadcrumbItem item)
        {
            var args = new BreadcrumbItemEventArgs(item, PopulateItemsEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Генерация события "BreadcrumbItemDropDownOpened"
        /// </summary>
        protected virtual void RaiseBreadcrumbItemDropDownOpened(RoutedEventArgs e)
        {
            var args = new BreadcrumbItemEventArgs(e.Source as BreadcrumbItem, BreadcrumbItemDropDownOpenedEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Генерация события "RaiseBreadcrumbItemDropDownClosed"
        /// </summary>
        protected virtual void RaiseBreadcrumbItemDropDownClosed(RoutedEventArgs e)
        {
            var args = new BreadcrumbItemEventArgs(e.Source as BreadcrumbItem, BreadcrumbItemDropDownClosedEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Генерация события "RaiseBreadcrumbItemDropDownClosed"
        /// </summary>
        protected virtual void RaiseSelectedItemChanged(object oldvalue, object newValue)
        {
        }

        /// <summary>
        /// Генерация события "OverflowPressedChanged"
        /// </summary>
        protected virtual void RaiseOverflowPressedChanged()
        {
            if (IsOverflowPressed)
            {
                BuildTraces();
            }
        }

        /// <summary>
        /// Генерация события "RootChanged"
        /// </summary>
        protected virtual void OnRootChanged(object oldValue, object newValue)
        {
            newValue = GetFirstItem(newValue);
            var oldRoot = oldValue as BreadcrumbItem;
            if (oldRoot != null)
            {
                oldRoot.IsRoot = false;
            }

            if (newValue == null)
            {
                RootItem = null;
                Path = null;
            }
            else
            {
                var root = newValue as BreadcrumbItem ?? BreadcrumbItem.CreateItem(newValue);
                if (root != null)
                {
                    root.IsRoot = true;
                }
                RemoveLogicalChild(oldValue);
                RootItem = root;
                if (root != null)
                {
                    if (LogicalTreeHelper.GetParent(root) == null)
                    {
                        AddLogicalChild(root);
                    }
                }
                SelectedItem = root != null ? root.DataContext : null;
                if (IsInitialized)
                {
                    SelectedBreadcrumb = root;
                }
                else
                {
                    _selectedBreadcrumb = root;
                }
            }
        }

        /// <summary>
        /// Переопределение "UIElement - OnInitialized"
        /// </summary>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (_initPath != null)
            {
                _initPath = null;
                BuildBreadcrumbsFromPath(Path);
            }
        }

        /// <summary>
        /// Переопределение "UIElement - ArrangeOverride"
        /// </summary>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);
            CheckOverflowImage();

            return size;
        }

        /// <summary>
        /// Переопределение "UIElement - OnApplyTemplate"
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _comboBox = GetTemplateChild(PART_COMBO_BOX) as ComboBox;
            _rootButton = GetTemplateChild(PART_ROOT) as BreadcrumbButton;
            if (_comboBox != null)
            {
                _comboBox.DropDownClosed += ComboBoxDropDownClosedHandler;
                _comboBox.IsKeyboardFocusWithinChanged += ComboBoxIsKeyboardFocusWithinChangedHandler;
                _comboBox.KeyDown += ComboBoxKeyDownHandler;
            }
            if (_rootButton != null)
            {
                _rootButton.Click += RootButtonClickHandler;
            }
        }

        /// <summary>
        /// Переопределение "UIElement - OnMouseDown"
        /// </summary>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
            {
                e.Handled = true;
                EnterToInputState();
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Переопределение "UIElement - OnMouseLeave"
        /// </summary>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (IsKeyboardFocusWithin)
            {
                Focus();
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Событие "ComboBox - KeyDown"
        /// </summary>
        private void ComboBoxKeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    ExitFromInputState(false);
                    break;
                case Key.Enter:
                    ExitFromInputState(true);
                    break;
                default:
                    return;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Событие "ComboBox - IsKeyboardFocusWithinChanged"
        /// </summary>
        private void ComboBoxIsKeyboardFocusWithinChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isKeyboardFocusWithin = (bool)e.NewValue;
            if (!isKeyboardFocusWithin)
            {
                ExitFromInputState(true);
            }
        }

        /// <summary>
        /// Событие "ComboBox - DropDownClosed"
        /// </summary>
        private void ComboBoxDropDownClosedHandler(object sender, EventArgs e)
        {
            IsDropDownOpen = false;
            Path = _comboBox.Text;
        }

        /// <summary>
        /// Событие "MenuItem - Click"
        /// </summary>
        private void MenuItemClickHandler(object sender, RoutedEventArgs e)
        {
            var item = e.Source as MenuItem;
            if (RootItem != null && item != null)
            {
                var dataItem = item.Tag;
                if (dataItem != null && dataItem.Equals(RootItem.SelectedItem))
                {
                    RootItem.SelectedItem = null;
                }
                RootItem.SelectedItem = dataItem;
            }
        }

        /// <summary>
        /// Событие "RootButton - Click"
        /// </summary>
        private void RootButtonClickHandler(object sender, RoutedEventArgs e)
        {
            EnterToInputState();
        }

        /// <summary>
        /// Событие RoutedEvent "BreadcrumbItemSelectedItemChangedHandler"
        /// </summary>
        private void BreadcrumbItemSelectedItemChangedHandler(object sender, RoutedEventArgs e)
        {
            var breadcrumb = e.OriginalSource as BreadcrumbItem;
            if (breadcrumb != null && breadcrumb.SelectedBreadcrumb != null)
            {
                breadcrumb = breadcrumb.SelectedBreadcrumb;
            }
            SelectedBreadcrumb = breadcrumb;

            if (SelectedBreadcrumb != null)
            {
                SelectedItem = SelectedBreadcrumb.Data;
            }
            Path = GetEditPath();
        }

        /// <summary>
        /// Событие RoutedEvent "BreadcrumbItemTraceValueChangedHandler"
        /// </summary>
        private void BreadcrumbItemTraceValueChangedHandler(object sender, RoutedEventArgs e)
        {
            if (Equals(e.OriginalSource, RootItem))
            {
                Path = GetEditPath();
            }
        }

        /// <summary>
        /// Событие RoutedEvent "BreadcrumbItemSelectionChangedEventHandler"
        /// </summary>
        private void BreadcrumbItemSelectionChangedEventHandler(object sender, RoutedEventArgs e)
        {
            var parent = e.Source as BreadcrumbItem;
            if (parent != null && parent.SelectedBreadcrumb != null)
            {
                RaisePopulateItems(parent.SelectedBreadcrumb);
            }
        }

        /// <summary>
        /// Событие RoutedEvent "BreadcrumbItemDropDownChangedEventHandler"
        /// </summary>
        private void BreadcrumbItemDropDownChangedEventHandler(object sender, RoutedEventArgs e)
        {
            var breadcrumb = e.Source as BreadcrumbItem;
            if (breadcrumb != null && breadcrumb.IsDropDownPressed)
            {
                RaiseBreadcrumbItemDropDownOpened(e);
            }
            else
            {
                RaiseBreadcrumbItemDropDownClosed(e);
            }
        }

        /// <summary>
        /// Событие RoutedEvent "ButtonClickedEventHandler"
        /// </summary>
        private void ButtonClickedEventHandler(object sender, RoutedEventArgs e)
        {
            if (IsKeyboardFocusWithin)
            {
                Focus();
            }
        }

        /// <summary>
        /// Генерация элемента "" из пути
        /// </summary>
        private void BuildBreadcrumbsFromPath(string newPath)
        {
            var e = new PathConversionEventArgs(PathConversionEventArgs.ConversionMode.EditToDisplay, newPath, Root, PathConversionEvent);
            RaiseEvent(e);
            newPath = e.DisplayPath;

            var item = RootItem;
            if (item == null)
            {
                Path = null;
                return;
            }

            newPath = newPath.Trim().TrimEndEx(SeparatorString);
            var chunks = newPath.Split(new[] { SeparatorString }, StringSplitOptions.None);
            if (chunks.Length == 0)
            {
                RootItem.SelectedItem = null;
            }
            int index = 0;

            var itemIndex = new List<int>();

            int length = chunks.Length;
            int max = BREADCRUMBS_TO_HIDE;
            if (max > 0 && chunks[index] == (RootItem.TraceValue))
            {
                length--;
                index++;
                max--;
            }

            for (int i = index; i < chunks.Length; i++)
            {
                if (item == null)
                {
                    break;
                }

                string trace = chunks[i];
                RaisePopulateItems(item);
                var next = item.GetTraceItem(trace);
                if (next == null)
                {
                    break;
                }
                itemIndex.Add(item.Items.IndexOf(next));
                var container = item.ContainerFromItem(next);

                item = container;
            }
            if (length != itemIndex.Count)
            {
                Path = GetDisplayPath();
                return;
            }

            try
            {
                RemoveHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(BreadcrumbItemSelectedItemChangedHandler));

                item = RootItem;

                foreach (int t in itemIndex)
                {
                    if (item == null)
                    {
                        break;
                    }
                    item.SelectedIndex = t;
                    item = item.SelectedBreadcrumb;
                }
                if (item != null)
                {
                    item.SelectedItem = null;
                }
                SelectedBreadcrumb = item;
                SelectedItem = item != null ? item.Data : null;
            }
            finally
            {
                AddHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(BreadcrumbItemSelectedItemChangedHandler));
            }
        }

        private void CheckOverflowImage()
        {
            bool isOverflow = (RootItem != null && RootItem.SelectedBreadcrumb != null && RootItem.SelectedBreadcrumb.IsOverflow);
            OverflowMode = isOverflow ? BreadcrumbButton.ButtonMode.Overflow : BreadcrumbButton.ButtonMode.Breadcrumb;
        }

        private void BuildTraces()
        {
            var item = RootItem;

            _traces.Clear();
            if (item != null && item.IsOverflow)
            {
                foreach (object trace in item.Items)
                {
                    var menuItem = new MenuItem();
                    menuItem.Tag = trace;
                    var bcItem = item.ContainerFromItem(trace);
                    menuItem.Header = bcItem.TraceValue;
                    menuItem.Click += MenuItemClickHandler;
                    menuItem.Icon = GetImage(bcItem.Image);
                    if (trace == RootItem.SelectedItem)
                    {
                        menuItem.FontWeight = FontWeights.Bold;
                    }
                    _traces.Add(menuItem);
                }
                _traces.Insert(0, new Separator());
                var rootMenuItem = new MenuItem();
                rootMenuItem.Header = item.TraceValue;
                rootMenuItem.Command = SelectRootCommand;
                rootMenuItem.CommandParameter = item;
                rootMenuItem.Icon = GetImage(item.Image);
                _traces.Insert(0, rootMenuItem);
            }

            item = item != null ? item.SelectedBreadcrumb : null;

            while (item != null)
            {
                if (!item.IsOverflow)
                {
                    break;
                }
                var traceMenuItem = new MenuItem();
                traceMenuItem.Header = item.TraceValue;
                traceMenuItem.Command = SelectRootCommand;
                traceMenuItem.CommandParameter = item;
                traceMenuItem.Icon = GetImage(item.Image);
                _traces.Insert(0, traceMenuItem);
                item = item.SelectedBreadcrumb;
            }
        }

        private object GetImage(ImageSource imageSource)
        {
            if (imageSource == null)
            {
                return null;
            }
            var image = new Image();
            image.Source = imageSource;
            image.Stretch = Stretch.Fill;
            image.SnapsToDevicePixels = true;
            image.Width = image.Height = 16;

            return image;
        }

        private object GetFirstItem(object entity)
        {
            var c = entity as ICollection;
            if (c != null)
            {
                foreach (object item in c)
                {
                    return item;
                }
            }
            return entity;
        }

        public string GetEditPath()
        {
            string displayPath = GetDisplayPath();
            var e = new PathConversionEventArgs(PathConversionEventArgs.ConversionMode.DisplayToEdit, displayPath, Root, PathConversionEvent);

            RaiseEvent(e);

            return e.EditPath;
        }

        public string PathFromBreadcrumbItem(BreadcrumbItem item)
        {
            var sb = new StringBuilder();
            while (item != null)
            {
                if (Equals(item, RootItem) && sb.Length > 0)
                {
                    break;
                }
                if (sb.Length > 0)
                {
                    sb.Insert(0, SeparatorString);
                }
                sb.Insert(0, item.TraceValue);
                item = item.ParentBreadcrumbItem;
            }
            var e = new PathConversionEventArgs(PathConversionEventArgs.ConversionMode.DisplayToEdit, sb.ToString(), Root, PathConversionEvent);
            RaiseEvent(e);
            return e.EditPath;
        }

        public string GetDisplayPath()
        {
            var separator = SeparatorString;
            var sb = new StringBuilder();
            var item = RootItem;
            int index = 0;

            while (item != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                if (index >= BREADCRUMBS_TO_HIDE || item.SelectedItem == null)
                {
                    sb.Append(item.GetTracePathValue());
                }
                index++;
                item = item.SelectedBreadcrumb;
            }

            return sb.ToString();
        }


        /// <summary>
        /// Вход в режим редактирования пути в ComobBox
        /// </summary>
        private void EnterToInputState()
        {
            if (_comboBox != null && IsEditable)
            {
                _comboBox.Text = Path;
                _comboBox.Visibility = Visibility.Visible;
                _comboBox.Focus();
            }
        }

        /// <summary>
        /// Выход из режима редактирования пути в ComobBox
        /// </summary>
        private void ExitFromInputState(bool updatePath)
        {
            if (_comboBox != null)
            {
                if (updatePath && _comboBox.IsVisible)
                {
                    Path = _comboBox.Text;
                }
                _comboBox.Visibility = Visibility.Hidden;
            }
        }

        #endregion
    }
}