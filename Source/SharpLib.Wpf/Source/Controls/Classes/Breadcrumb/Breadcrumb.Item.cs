using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;

namespace SharpLib.Wpf.Controls
{
    [TemplatePart(Name = PART_HEADER)]
    [TemplatePart(Name = PART_SELECTED)]
    public class BreadcrumbItem : Selector
    {
        #region Константы

        private const string PART_HEADER = "PART_Header";

        private const string PART_SELECTED = "PART_Selected";

        #endregion

        #region Поля

        public static readonly RoutedEvent DropDownPressedChangedEvent;

        public static readonly DependencyProperty HeaderProperty;

        public static readonly DependencyProperty HeaderTemplateProperty;

        public static readonly DependencyProperty HeaderTemplateSelectorProperty;

        public static readonly DependencyProperty ImageProperty;

        public static readonly DependencyProperty IsButtonVisibleProperty;

        public static readonly DependencyProperty IsDropDownPressedProperty;

        public static readonly DependencyProperty IsImageVisibleProperty;

        public static readonly DependencyProperty IsOverflowProperty;

        public static readonly DependencyProperty IsRootProperty;

        public static readonly RoutedEvent OverflowChangedEvent;

        public static readonly DependencyProperty OverflowItemTemplateProperty;

        public static readonly DependencyProperty OverflowItemTemplateSelectorProperty;

        public static readonly DependencyProperty SelectedBreadcrumbProperty;

        public static readonly RoutedEvent TraceChangedEvent;

        public static readonly DependencyProperty TraceProperty;

        private static readonly DependencyPropertyKey _selectedBreadcrumbPropertyKey;

        private FrameworkElement _headerControl;

        private FrameworkElement _selectedControl;

        #endregion

        #region Свойства

        public object Data
        {
            get { return DataContext ?? this; }
        }

        public bool IsDropDownPressed
        {
            get { return (bool)GetValue(IsDropDownPressedProperty); }
            set { SetValue(IsDropDownPressedProperty, value); }
        }

        public bool IsOverflow
        {
            get { return (bool)GetValue(IsOverflowProperty); }
            private set { SetValue(IsOverflowProperty, value); }
        }

        public bool IsRoot
        {
            get { return (bool)GetValue(IsRootProperty); }
            set { SetValue(IsRootProperty, value); }
        }

        public BreadcrumbItem SelectedBreadcrumb
        {
            get { return (BreadcrumbItem)GetValue(SelectedBreadcrumbProperty); }
            private set { SetValue(_selectedBreadcrumbPropertyKey, value); }
        }

        public DataTemplateSelector BreadcrumbTemplateSelector { get; set; }

        public DataTemplate BreadcrumbItemTemplate { get; set; }

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

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public object Trace
        {
            get { return GetValue(TraceProperty); }
            set { SetValue(TraceProperty, value); }
        }

        public BindingBase TraceBinding { get; set; }

        public BindingBase ImageBinding { get; set; }

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public DataTemplateSelector HeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
            set { SetValue(HeaderTemplateSelectorProperty, value); }
        }

        public BreadcrumbBar BreadcrumbBar
        {
            get
            {
                DependencyObject current = this;
                while (current != null)
                {
                    current = LogicalTreeHelper.GetParent(current);
                    if (current is BreadcrumbBar)
                    {
                        return current as BreadcrumbBar;
                    }
                }
                return null;
            }
        }

        public BreadcrumbItem ParentBreadcrumbItem
        {
            get
            {
                BreadcrumbItem parent = LogicalTreeHelper.GetParent(this) as BreadcrumbItem;
                return parent;
            }
        }

        public string TraceValue
        {
            get
            {
                XmlNode xml = Trace as XmlNode;
                if (xml != null)
                {
                    return xml.Value;
                }

                if (Trace != null)
                {
                    return Trace.ToString();
                }
                if (Header != null)
                {
                    return Header.ToString();
                }
                return string.Empty;
            }
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                object content = SelectedBreadcrumb;

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

                object[] array = { SelectedBreadcrumb };
                return array.GetEnumerator();
            }
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

        #endregion

        #region События

        public event RoutedPropertyChangedEventHandler<object> DropDownPressedChanged
        {
            add { AddHandler(DropDownPressedChangedEvent, value); }
            remove { RemoveHandler(DropDownPressedChangedEvent, value); }
        }

        public event RoutedPropertyChangedEventHandler<object> TraceChanged
        {
            add { AddHandler(TraceChangedEvent, value); }
            remove { RemoveHandler(TraceChangedEvent, value); }
        }

        #endregion

        #region Конструктор

        static BreadcrumbItem()
        {
            _selectedBreadcrumbPropertyKey = DependencyProperty.RegisterReadOnly("SelectedBreadcrumb", typeof(BreadcrumbItem), typeof(BreadcrumbItem),
                new UIPropertyMetadata(null, SelectedBreadcrumbPropertyChanged));

            DropDownPressedChangedEvent = EventManager.RegisterRoutedEvent("DropDownPressedChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(BreadcrumbItem));
            HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(BreadcrumbItem), new UIPropertyMetadata(null, HeaderPropertyChanged));
            ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(BreadcrumbItem), new UIPropertyMetadata(null));
            IsButtonVisibleProperty = DependencyProperty.Register("IsButtonVisible", typeof(bool), typeof(BreadcrumbItem), new UIPropertyMetadata(true));
            IsDropDownPressedProperty = DependencyProperty.Register("IsDropDownPressed", typeof(bool), typeof(BreadcrumbItem), new UIPropertyMetadata(false, DropDownPressedPropertyChanged));
            IsImageVisibleProperty = DependencyProperty.Register("IsImageVisible", typeof(bool), typeof(BreadcrumbItem), new UIPropertyMetadata(false));
            IsOverflowProperty = DependencyProperty.Register("IsOverflow", typeof(bool), typeof(BreadcrumbItem),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                    OverflowPropertyChanged));
            IsRootProperty = DependencyProperty.Register("IsRoot", typeof(bool), typeof(BreadcrumbItem), new UIPropertyMetadata(false));
            OverflowChangedEvent = EventManager.RegisterRoutedEvent("OverflowChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbItem));
            SelectedBreadcrumbProperty = _selectedBreadcrumbPropertyKey.DependencyProperty;
            TraceChangedEvent = EventManager.RegisterRoutedEvent("TraceChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(BreadcrumbItem));
            TraceProperty = DependencyProperty.Register("Trace", typeof(object), typeof(BreadcrumbItem), new UIPropertyMetadata(null, TracePropertyChanged));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbItem), new FrameworkPropertyMetadata(typeof(BreadcrumbItem)));

            OverflowItemTemplateProperty = BreadcrumbBar.OverflowItemTemplateProperty.AddOwner(typeof(BreadcrumbItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
            OverflowItemTemplateSelectorProperty = BreadcrumbBar.OverflowItemTemplateSelectorProperty.AddOwner(typeof(BreadcrumbItem),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
            HeaderTemplateSelectorProperty = BreadcrumbBar.BreadcrumbItemTemplateSelectorProperty.AddOwner(typeof(BreadcrumbItem),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
            HeaderTemplateProperty = BreadcrumbBar.BreadcrumbItemTemplateProperty.AddOwner(typeof(BreadcrumbItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        }

        public BreadcrumbItem()
        {
        }

        public BreadcrumbItem(string header)
        {
            Header = header;
        }

        #endregion

        #region Методы

        private static void HeaderPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // BreadcrumbItem item = sender as BreadcrumbItem;
        }

        private static void TracePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as BreadcrumbItem;
            if (item != null)
            {
                var args = new RoutedPropertyChangedEventArgs<object>(e.OldValue, e.NewValue, TraceChangedEvent);
                item.RaiseEvent(args);
            }
        }

        public static BreadcrumbItem CreateItem(object dataContext)
        {
            var item = dataContext as BreadcrumbItem;
            if (item == null && dataContext != null)
            {
                item = new BreadcrumbItem();
                item.DataContext = dataContext;
            }
            return item;
        }

        private static void SelectedBreadcrumbPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as BreadcrumbItem;
            if (item != null)
            {
                item.OnSelectedBreadcrumbChanged(e.OldValue, e.NewValue);
            }
        }

        protected virtual void OnSelectedBreadcrumbChanged(object oldItem, object newItem)
        {
            if (SelectedBreadcrumb != null)
            {
                SelectedBreadcrumb.SelectedItem = null;
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is BreadcrumbItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new BreadcrumbItem();
            return item;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _headerControl = GetTemplateChild(PART_HEADER) as FrameworkElement;
            _selectedControl = GetTemplateChild(PART_SELECTED) as FrameworkElement;

            ApplyBinding();
        }

        public static void DropDownPressedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as BreadcrumbItem;
            if (item != null)
            {
                item.OnDropDownPressedChanged();
            }
        }

        protected virtual void OnDropDownPressedChanged()
        {
            var args = new RoutedEventArgs(DropDownPressedChangedEvent);
            RaiseEvent(args);
        }

        public static void OverflowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as BreadcrumbItem;
            if (item != null)
            {
                item.OnOverflowChanged((bool)e.NewValue);
            }
        }

        protected virtual void OnOverflowChanged(bool newValue)
        {
            var args = new RoutedEventArgs(OverflowChangedEvent);
            RaiseEvent(args);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (SelectedItem != null)
            {
                _headerControl.Visibility = Visibility.Visible;
                _headerControl.Measure(constraint);
                Size size = new Size(constraint.Width - _headerControl.DesiredSize.Width, constraint.Height);
                _selectedControl.Measure(new Size(double.PositiveInfinity, constraint.Height));
                double width = _headerControl.DesiredSize.Width + _selectedControl.DesiredSize.Width;
                if (width > constraint.Width || (IsRoot && SelectedItem != null))
                {
                    _headerControl.Visibility = Visibility.Collapsed;
                }
            }
            else if (_headerControl != null)
            {
                _headerControl.Visibility = Visibility.Visible;
            }
            IsOverflow = _headerControl != null && _headerControl.Visibility != Visibility.Visible;

            Size result = base.MeasureOverride(constraint);
            return result;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectedBreadcrumb = SelectedItem == null ? null : ContainerFromItem(SelectedItem);
            base.OnSelectionChanged(e);
        }

        public BreadcrumbItem ContainerFromItem(object item)
        {
            var result = item as BreadcrumbItem;
            if (result != null)
            {
                return result;
            }
            result = CreateItem(item);
            if (result != null)
            {
                AddLogicalChild(result);
                result.ApplyTemplate();
            }
            return result;
        }

        public void ApplyBinding()
        {
            object item = DataContext;
            if (item == null)
            {
                return;
            }

            var root = this;
            var template = HeaderTemplate;
            var templateSelector = HeaderTemplateSelector;
            if (templateSelector != null)
            {
                template = templateSelector.SelectTemplate(item, root);
            }
            if (template == null)
            {
                var key = GetResourceKey(item);
                if (key != null)
                {
                    template = TryFindResource(key) as DataTemplate;
                }
            }

            root.SelectedItem = null;

            var hdt = template as HierarchicalDataTemplate;
            root.Header = template != null ? template.LoadContent() : item;
            root.DataContext = item;

            if (hdt != null)
            {
                root.SetBinding(ItemsSourceProperty, hdt.ItemsSource);
            }

            var bar = BreadcrumbBar;

            if (bar != null)
            {
                if (TraceBinding == null)
                {
                    TraceBinding = bar.TraceBinding;
                }
                if (ImageBinding == null)
                {
                    ImageBinding = bar.ImageBinding;
                }
            }

            if (TraceBinding != null)
            {
                root.SetBinding(TraceProperty, TraceBinding);
            }
            if (ImageBinding != null)
            {
                root.SetBinding(ImageProperty, ImageBinding);
            }

            ApplyProperties(item);
        }

        private static DataTemplateKey GetResourceKey(object item)
        {
            var xml = item as XmlDataProvider;
            DataTemplateKey key;
            if (xml != null)
            {
                key = new DataTemplateKey(xml.XPath);
            }
            else
            {
                XmlNode node = item as XmlNode;
                key = node != null ? new DataTemplateKey(node.Name) : new DataTemplateKey(item.GetType());
            }
            return key;
        }

        private void ApplyProperties(object item)
        {
            var e = new ApplyPropertiesEventArgs(item, this, BreadcrumbBar.ApplyPropertiesEvent);
            e.Image = Image;
            e.Trace = Trace;
            e.TraceValue = TraceValue;
            RaiseEvent(e);
            Image = e.Image;
            Trace = e.Trace;
        }

        public string GetTracePathValue()
        {
            var e = new ApplyPropertiesEventArgs(DataContext, this, BreadcrumbBar.ApplyPropertiesEvent);
            e.Trace = Trace;
            e.TraceValue = TraceValue;
            RaiseEvent(e);
            return e.TraceValue;
        }

        public object GetTraceItem(string trace)
        {
            foreach (object item in Items)
            {
                var bcItem = ContainerFromItem(item);
                if (item != null)
                {
                    string value = bcItem.TraceValue;
                    if (value != null && value.Equals(trace, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return item;
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        #endregion
    }
}