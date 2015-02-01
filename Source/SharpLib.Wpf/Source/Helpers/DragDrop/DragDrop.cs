using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpLib.Wpf.Dragging.Icons;
using SharpLib.Wpf.Dragging.Utilities;

namespace SharpLib.Wpf.Dragging
{
    public static class DragDrop
    {
        #region Поля

        public static readonly DataFormat DataFormat;

        public static readonly DependencyProperty DefaultDragAdornerOpacityProperty;

        public static readonly DependencyProperty DragAdornerTemplateProperty;

        public static readonly DependencyProperty DragAdornerTemplateSelectorProperty;

        public static readonly DependencyProperty DragDirectlySelectedOnlyProperty;

        public static readonly DependencyProperty DragDropContextProperty;

        public static readonly DependencyProperty DragHandlerProperty;

        public static readonly DependencyProperty DragMouseAnchorPointProperty;

        public static readonly DependencyProperty DragSourceIgnoreProperty;

        public static readonly DependencyProperty DropHandlerProperty;

        public static readonly DependencyProperty EffectAllAdornerTemplateProperty;

        public static readonly DependencyProperty EffectCopyAdornerTemplateProperty;

        public static readonly DependencyProperty EffectLinkAdornerTemplateProperty;

        public static readonly DependencyProperty EffectMoveAdornerTemplateProperty;

        public static readonly DependencyProperty EffectNoneAdornerTemplateProperty;

        public static readonly DependencyProperty EffectScrollAdornerTemplateProperty;

        public static readonly DependencyProperty IsDragSourceProperty;

        public static readonly DependencyProperty IsDropTargetProperty;

        public static readonly DependencyProperty UseDefaultDragAdornerProperty;

        public static readonly DependencyProperty UseDefaultEffectDataTemplateProperty;

        private static Point _adornerPos;

        private static Size _adornerSize;

        private static object _clickSupressItem;

        private static IDragSource _defaultDragHandler;

        private static IDropTarget _defaultDropHandler;

        private static DragAdorner _dragAdorner;

        private static bool _dragInProgress;

        private static DragInfo _dragInfo;

        private static DropTargetAdorner _dropTargetAdorner;

        private static DragAdorner _effectAdorner;

        #endregion

        #region Свойства

        public static IDragSource DefaultDragHandler
        {
            get { return _defaultDragHandler ?? (_defaultDragHandler = new DefaultDragHandler()); }
            set { _defaultDragHandler = value; }
        }

        public static IDropTarget DefaultDropHandler
        {
            get { return _defaultDropHandler ?? (_defaultDropHandler = new DefaultDropHandler()); }
            set { _defaultDropHandler = value; }
        }

        private static DragAdorner DragAdorner
        {
            get { return _dragAdorner; }
            set
            {
                if (_dragAdorner != null)
                {
                    _dragAdorner.Detatch();
                }

                _dragAdorner = value;
            }
        }

        private static DragAdorner EffectAdorner
        {
            get { return _effectAdorner; }
            set
            {
                if (_effectAdorner != null)
                {
                    _effectAdorner.Detatch();
                }

                _effectAdorner = value;
            }
        }

        private static DropTargetAdorner DropTargetAdorner
        {
            get { return _dropTargetAdorner; }
            set
            {
                if (_dropTargetAdorner != null)
                {
                    _dropTargetAdorner.Detatch();
                }

                _dropTargetAdorner = value;
            }
        }

        #endregion

        #region Конструктор

        static DragDrop()
        {
            DataFormat = DataFormats.GetDataFormat("GongSolutions.Wpf.DragDrop");
            DefaultDragAdornerOpacityProperty = DependencyProperty.RegisterAttached("DefaultDragAdornerOpacity", typeof(double), typeof(DragDrop), new PropertyMetadata(0.8));
            DragAdornerTemplateProperty = DependencyProperty.RegisterAttached("DragAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));
            DragAdornerTemplateSelectorProperty = DependencyProperty.RegisterAttached("DragAdornerTemplateSelector", typeof(DataTemplateSelector), typeof(DragDrop),
                new PropertyMetadata(default(DataTemplateSelector)));
            DragDirectlySelectedOnlyProperty = DependencyProperty.RegisterAttached("DragDirectlySelectedOnly", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));
            DragDropContextProperty = DependencyProperty.RegisterAttached("DragDropContext", typeof(string), typeof(DragDrop), new UIPropertyMetadata(string.Empty));
            DragHandlerProperty = DependencyProperty.RegisterAttached("DragHandler", typeof(IDragSource), typeof(DragDrop));
            DragMouseAnchorPointProperty = DependencyProperty.RegisterAttached("DragMouseAnchorPoint", typeof(Point), typeof(DragDrop), new PropertyMetadata(new Point(0, 1)));
            DragSourceIgnoreProperty = DependencyProperty.RegisterAttached("DragSourceIgnore", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));
            DropHandlerProperty = DependencyProperty.RegisterAttached("DropHandler", typeof(IDropTarget), typeof(DragDrop));
            EffectAllAdornerTemplateProperty = DependencyProperty.RegisterAttached("EffectAllAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));
            EffectCopyAdornerTemplateProperty = DependencyProperty.RegisterAttached("EffectCopyAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));
            EffectLinkAdornerTemplateProperty = DependencyProperty.RegisterAttached("EffectLinkAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));
            EffectMoveAdornerTemplateProperty = DependencyProperty.RegisterAttached("EffectMoveAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));
            EffectNoneAdornerTemplateProperty = DependencyProperty.RegisterAttached("EffectNoneAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));
            EffectScrollAdornerTemplateProperty = DependencyProperty.RegisterAttached("EffectScrollAdornerTemplate", typeof(DataTemplate), typeof(DragDrop));
            IsDragSourceProperty = DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDrop), new UIPropertyMetadata(false, IsDragSourceChanged));
            IsDropTargetProperty = DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDrop), new UIPropertyMetadata(false, IsDropTargetChanged));
            UseDefaultDragAdornerProperty = DependencyProperty.RegisterAttached("UseDefaultDragAdorner", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));
            UseDefaultEffectDataTemplateProperty = DependencyProperty.RegisterAttached("UseDefaultEffectDataTemplate", typeof(bool), typeof(DragDrop), new PropertyMetadata(false));
        }

        #endregion

        #region Методы

        public static DataTemplate GetDragAdornerTemplate(UIElement target)
        {
            return (DataTemplate)target.GetValue(DragAdornerTemplateProperty);
        }

        public static void SetDragAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(DragAdornerTemplateProperty, value);
        }

        public static void SetDragAdornerTemplateSelector(DependencyObject element, DataTemplateSelector value)
        {
            element.SetValue(DragAdornerTemplateSelectorProperty, value);
        }

        public static DataTemplateSelector GetDragAdornerTemplateSelector(DependencyObject element)
        {
            return (DataTemplateSelector)element.GetValue(DragAdornerTemplateSelectorProperty);
        }

        public static bool GetUseDefaultDragAdorner(UIElement target)
        {
            return (bool)target.GetValue(UseDefaultDragAdornerProperty);
        }

        public static void SetUseDefaultDragAdorner(UIElement target, bool value)
        {
            target.SetValue(UseDefaultDragAdornerProperty, value);
        }

        public static double GetDefaultDragAdornerOpacity(UIElement target)
        {
            return (double)target.GetValue(DefaultDragAdornerOpacityProperty);
        }

        public static void SetDefaultDragAdornerOpacity(UIElement target, double value)
        {
            target.SetValue(DefaultDragAdornerOpacityProperty, value);
        }

        public static bool GetUseDefaultEffectDataTemplate(UIElement target)
        {
            return (bool)target.GetValue(UseDefaultEffectDataTemplateProperty);
        }

        public static void SetUseDefaultEffectDataTemplate(UIElement target, bool value)
        {
            target.SetValue(UseDefaultEffectDataTemplateProperty, value);
        }

        public static DataTemplate GetEffectNoneAdornerTemplate(UIElement target)
        {
            var template = (DataTemplate)target.GetValue(EffectNoneAdornerTemplateProperty);

            if (template == null)
            {
                if (!GetUseDefaultEffectDataTemplate(target))
                {
                    return null;
                }

                var imageSourceFactory = new FrameworkElementFactory(typeof(Image));
                imageSourceFactory.SetValue(Image.SourceProperty, IconFactory.EffectNone);
                imageSourceFactory.SetValue(FrameworkElement.HeightProperty, 12.0);
                imageSourceFactory.SetValue(FrameworkElement.WidthProperty, 12.0);

                template = new DataTemplate();
                template.VisualTree = imageSourceFactory;
            }

            return template;
        }

        public static void SetEffectNoneAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectNoneAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectCopyAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectCopyAdornerTemplateProperty) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectCopy, "Copy to", destinationText);

            return template;
        }

        public static void SetEffectCopyAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectCopyAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectMoveAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectMoveAdornerTemplateProperty) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectMove, "Move to", destinationText);

            return template;
        }

        public static void SetEffectMoveAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectMoveAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectLinkAdornerTemplate(UIElement target, string destinationText)
        {
            var template = (DataTemplate)target.GetValue(EffectLinkAdornerTemplateProperty) ?? CreateDefaultEffectDataTemplate(target, IconFactory.EffectLink, "Link to", destinationText);

            return template;
        }

        public static void SetEffectLinkAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectLinkAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectAllAdornerTemplate(UIElement target)
        {
            var template = (DataTemplate)target.GetValue(EffectAllAdornerTemplateProperty);

            return template;
        }

        public static void SetEffectAllAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectAllAdornerTemplateProperty, value);
        }

        public static DataTemplate GetEffectScrollAdornerTemplate(UIElement target)
        {
            var template = (DataTemplate)target.GetValue(EffectScrollAdornerTemplateProperty);

            return template;
        }

        public static void SetEffectScrollAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(EffectScrollAdornerTemplateProperty, value);
        }

        public static bool GetIsDragSource(UIElement target)
        {
            return (bool)target.GetValue(IsDragSourceProperty);
        }

        public static void SetIsDragSource(UIElement target, bool value)
        {
            target.SetValue(IsDragSourceProperty, value);
        }

        public static bool GetIsDropTarget(UIElement target)
        {
            return (bool)target.GetValue(IsDropTargetProperty);
        }

        public static void SetIsDropTarget(UIElement target, bool value)
        {
            target.SetValue(IsDropTargetProperty, value);
        }

        public static string GetDragDropContext(UIElement target)
        {
            return (string)target.GetValue(DragDropContextProperty);
        }

        public static void SetDragDropContext(UIElement target, string value)
        {
            target.SetValue(DragDropContextProperty, value);
        }

        public static IDragSource GetDragHandler(UIElement target)
        {
            return (IDragSource)target.GetValue(DragHandlerProperty);
        }

        public static void SetDragHandler(UIElement target, IDragSource value)
        {
            target.SetValue(DragHandlerProperty, value);
        }

        public static IDropTarget GetDropHandler(UIElement target)
        {
            return (IDropTarget)target.GetValue(DropHandlerProperty);
        }

        public static void SetDropHandler(UIElement target, IDropTarget value)
        {
            target.SetValue(DropHandlerProperty, value);
        }

        public static bool GetDragSourceIgnore(UIElement target)
        {
            return (bool)target.GetValue(DragSourceIgnoreProperty);
        }

        public static void SetDragSourceIgnore(UIElement target, bool value)
        {
            target.SetValue(DragSourceIgnoreProperty, value);
        }

        public static bool GetDragDirectlySelectedOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(DragDirectlySelectedOnlyProperty);
        }

        public static void SetDragDirectlySelectedOnly(DependencyObject obj, bool value)
        {
            obj.SetValue(DragDirectlySelectedOnlyProperty, value);
        }

        public static Point GetDragMouseAnchorPoint(UIElement target)
        {
            return (Point)target.GetValue(DragMouseAnchorPointProperty);
        }

        public static void SetDragMouseAnchorPoint(UIElement target, Point value)
        {
            target.SetValue(DragMouseAnchorPointProperty, value);
        }

        private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue)
            {
                uiElement.PreviewMouseLeftButtonDown += DragSource_PreviewMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp += DragSource_PreviewMouseLeftButtonUp;
                uiElement.PreviewMouseMove += DragSource_PreviewMouseMove;
                uiElement.QueryContinueDrag += DragSource_QueryContinueDrag;
            }
            else
            {
                uiElement.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
                uiElement.PreviewMouseMove -= DragSource_PreviewMouseMove;
                uiElement.QueryContinueDrag -= DragSource_QueryContinueDrag;
            }
        }

        private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue)
            {
                uiElement.AllowDrop = true;

                if (uiElement is ItemsControl)
                {
                    uiElement.DragEnter += DropTarget_PreviewDragEnter;
                    uiElement.DragLeave += DropTarget_PreviewDragLeave;
                    uiElement.DragOver += DropTarget_PreviewDragOver;
                    uiElement.Drop += DropTarget_PreviewDrop;
                    uiElement.GiveFeedback += DropTarget_GiveFeedback;
                }
                else
                {
                    uiElement.PreviewDragEnter += DropTarget_PreviewDragEnter;
                    uiElement.PreviewDragLeave += DropTarget_PreviewDragLeave;
                    uiElement.PreviewDragOver += DropTarget_PreviewDragOver;
                    uiElement.PreviewDrop += DropTarget_PreviewDrop;
                    uiElement.PreviewGiveFeedback += DropTarget_GiveFeedback;
                }
            }
            else
            {
                uiElement.AllowDrop = false;

                if (uiElement is ItemsControl)
                {
                    uiElement.DragEnter -= DropTarget_PreviewDragEnter;
                    uiElement.DragLeave -= DropTarget_PreviewDragLeave;
                    uiElement.DragOver -= DropTarget_PreviewDragOver;
                    uiElement.Drop -= DropTarget_PreviewDrop;
                    uiElement.GiveFeedback -= DropTarget_GiveFeedback;
                }
                else
                {
                    uiElement.PreviewDragEnter -= DropTarget_PreviewDragEnter;
                    uiElement.PreviewDragLeave -= DropTarget_PreviewDragLeave;
                    uiElement.PreviewDragOver -= DropTarget_PreviewDragOver;
                    uiElement.PreviewDrop -= DropTarget_PreviewDrop;
                    uiElement.PreviewGiveFeedback -= DropTarget_GiveFeedback;
                }

                Mouse.OverrideCursor = null;
            }
        }

        private static void CreateDragAdorner()
        {
            var template = GetDragAdornerTemplate(_dragInfo.VisualSource);
            var templateSelector = GetDragAdornerTemplateSelector(_dragInfo.VisualSource);

            UIElement adornment = null;

            var useDefaultDragAdorner = GetUseDefaultDragAdorner(_dragInfo.VisualSource);

            if (template == null && templateSelector == null && useDefaultDragAdorner)
            {
                template = new DataTemplate();

                var factory = new FrameworkElementFactory(typeof(Image));

                var bs = CaptureScreen(_dragInfo.VisualSourceItem, _dragInfo.VisualSourceFlowDirection);
                factory.SetValue(Image.SourceProperty, bs);
                factory.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                factory.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
                if (_dragInfo.VisualSourceItem is FrameworkElement)
                {
                    factory.SetValue(FrameworkElement.WidthProperty, ((FrameworkElement)_dragInfo.VisualSourceItem).ActualWidth);
                    factory.SetValue(FrameworkElement.HeightProperty, ((FrameworkElement)_dragInfo.VisualSourceItem).ActualHeight);
                    factory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
                }
                template.VisualTree = factory;
            }

            if (template != null || templateSelector != null)
            {
                if (_dragInfo.Data is IEnumerable && !(_dragInfo.Data is string))
                {
                    if (((IEnumerable)_dragInfo.Data).Cast<object>().Count() <= 10)
                    {
                        var itemsControl = new ItemsControl();
                        itemsControl.ItemsSource = (IEnumerable)_dragInfo.Data;
                        itemsControl.ItemTemplate = template;
                        itemsControl.ItemTemplateSelector = templateSelector;

                        var border = new Border();
                        border.Child = itemsControl;
                        adornment = border;
                    }
                }
                else
                {
                    var contentPresenter = new ContentPresenter();
                    contentPresenter.Content = _dragInfo.Data;
                    contentPresenter.ContentTemplate = template;
                    contentPresenter.ContentTemplateSelector = templateSelector;
                    adornment = contentPresenter;
                }
            }

            if (adornment != null)
            {
                if (useDefaultDragAdorner)
                {
                    adornment.Opacity = GetDefaultDragAdornerOpacity(_dragInfo.VisualSource);
                }

                var parentWindow = _dragInfo.VisualSource.GetVisualAncestor<Window>();
                var rootElement = parentWindow != null ? parentWindow.Content as UIElement : null;
                if (rootElement == null && Application.Current != null && Application.Current.MainWindow != null)
                {
                    rootElement = (UIElement)Application.Current.MainWindow.Content;
                }

                if (rootElement == null)
                {
                    rootElement = _dragInfo.VisualSource.GetVisualAncestor<UserControl>();
                }

                DragAdorner = new DragAdorner(rootElement, adornment);
            }
        }

        private static BitmapSource CaptureScreen(Visual target, FlowDirection flowDirection, double dpiX = 96.0, double dpiY = 96.0)
        {
            if (target == null)
            {
                return null;
            }

            var bounds = VisualTreeHelper.GetDescendantBounds(target);

            var rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                (int)(bounds.Height * dpiY / 96.0),
                dpiX,
                dpiY,
                PixelFormats.Pbgra32);

            var dv = new DrawingVisual();
            using (var ctx = dv.RenderOpen())
            {
                var vb = new VisualBrush(target);
                if (flowDirection == FlowDirection.RightToLeft)
                {
                    var transformGroup = new TransformGroup();
                    transformGroup.Children.Add(new ScaleTransform(-1, 1));
                    transformGroup.Children.Add(new TranslateTransform(bounds.Size.Width - 1, 0));
                    ctx.PushTransform(transformGroup);
                }
                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            return rtb;
        }

        private static void CreateEffectAdorner(DropInfo dropInfo)
        {
            var template = GetEffectAdornerTemplate(_dragInfo.VisualSource, dropInfo.Effects, dropInfo.DestinationText);

            if (template != null)
            {
                var rootElement = (UIElement)Application.Current.MainWindow.Content;

                var contentPresenter = new ContentPresenter();
                contentPresenter.Content = _dragInfo.Data;
                contentPresenter.ContentTemplate = template;

                var adornment = contentPresenter;

                EffectAdorner = new DragAdorner(rootElement, adornment);
            }
        }

        private static DataTemplate CreateDefaultEffectDataTemplate(UIElement target, BitmapImage effectIcon, string effectText, string destinationText)
        {
            if (!GetUseDefaultEffectDataTemplate(target))
            {
                return null;
            }

            var imageFactory = new FrameworkElementFactory(typeof(Image));
            imageFactory.SetValue(Image.SourceProperty, effectIcon);
            imageFactory.SetValue(FrameworkElement.HeightProperty, 12.0);
            imageFactory.SetValue(FrameworkElement.WidthProperty, 12.0);
            imageFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(0.0, 0.0, 3.0, 0.0));

            var effectTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            effectTextBlockFactory.SetValue(TextBlock.TextProperty, effectText);
            effectTextBlockFactory.SetValue(TextBlock.FontSizeProperty, 11.0);
            effectTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.Blue);

            var destinationTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            destinationTextBlockFactory.SetValue(TextBlock.TextProperty, destinationText);
            destinationTextBlockFactory.SetValue(TextBlock.FontSizeProperty, 11.0);
            destinationTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.DarkBlue);
            destinationTextBlockFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(3, 0, 0, 0));
            destinationTextBlockFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.DemiBold);

            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            stackPanelFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(2.0));
            stackPanelFactory.AppendChild(imageFactory);
            stackPanelFactory.AppendChild(effectTextBlockFactory);
            stackPanelFactory.AppendChild(destinationTextBlockFactory);

            var borderFactory = new FrameworkElementFactory(typeof(Border));
            var stopCollection = new GradientStopCollection
            {
                new GradientStop(Colors.White, 0.0),
                new GradientStop(Colors.AliceBlue, 1.0)
            };
            var gradientBrush = new LinearGradientBrush(stopCollection)
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            borderFactory.SetValue(Panel.BackgroundProperty, gradientBrush);
            borderFactory.SetValue(Border.BorderBrushProperty, Brushes.DimGray);
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3.0));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));
            borderFactory.AppendChild(stackPanelFactory);

            var template = new DataTemplate();
            template.VisualTree = borderFactory;
            return template;
        }

        private static DataTemplate GetEffectAdornerTemplate(UIElement target, DragDropEffects effect, string destinationText)
        {
            switch (effect)
            {
                case DragDropEffects.All:
                    return null;
                case DragDropEffects.Copy:
                    return GetEffectCopyAdornerTemplate(target, destinationText);
                case DragDropEffects.Link:
                    return GetEffectLinkAdornerTemplate(target, destinationText);
                case DragDropEffects.Move:
                    return GetEffectMoveAdornerTemplate(target, destinationText);
                case DragDropEffects.None:
                    return GetEffectNoneAdornerTemplate(target);
                case DragDropEffects.Scroll:
                    return null;
                default:
                    return null;
            }
        }

        private static void Scroll(DependencyObject o, DragEventArgs e)
        {
            var scrollViewer = o.GetVisualDescendent<ScrollViewer>();

            if (scrollViewer != null)
            {
                var position = e.GetPosition(scrollViewer);
                var scrollMargin = Math.Min(scrollViewer.FontSize * 2, scrollViewer.ActualHeight / 2);

                if (position.X >= scrollViewer.ActualWidth - scrollMargin &&
                    scrollViewer.HorizontalOffset < scrollViewer.ExtentWidth - scrollViewer.ViewportWidth)
                {
                    scrollViewer.LineRight();
                }
                else if (position.X < scrollMargin && scrollViewer.HorizontalOffset > 0)
                {
                    scrollViewer.LineLeft();
                }
                else if (position.Y >= scrollViewer.ActualHeight - scrollMargin &&
                         scrollViewer.VerticalOffset < scrollViewer.ExtentHeight - scrollViewer.ViewportHeight)
                {
                    scrollViewer.LineDown();
                }
                else if (position.Y < scrollMargin && scrollViewer.VerticalOffset > 0)
                {
                    scrollViewer.LineUp();
                }
            }
        }

        private static IDragSource TryGetDragHandler(DragInfo dragInfo, UIElement sender)
        {
            IDragSource dragHandler = null;
            if (dragInfo != null && dragInfo.VisualSource != null)
            {
                dragHandler = GetDragHandler(dragInfo.VisualSource);
            }
            if (dragHandler == null && sender != null)
            {
                dragHandler = GetDragHandler(sender);
            }
            return dragHandler ?? DefaultDragHandler;
        }

        private static IDropTarget TryGetDropHandler(DropInfo dropInfo, UIElement sender)
        {
            IDropTarget dropHandler = null;
            if (dropInfo != null && dropInfo.VisualTarget != null)
            {
                dropHandler = GetDropHandler(dropInfo.VisualTarget);
            }
            if (dropHandler == null && sender != null)
            {
                dropHandler = GetDropHandler(sender);
            }
            return dropHandler ?? DefaultDropHandler;
        }

        private static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var elementPosition = e.GetPosition((IInputElement)sender);
            if (e.ClickCount != 1
                || HitTestUtilities.HitTest4Type<RangeBase>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<ButtonBase>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<TextBoxBase>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<PasswordBox>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<ComboBox>(sender, elementPosition)
                || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
                || HitTestUtilities.HitTest4DataGridTypes(sender, elementPosition)
                || HitTestUtilities.IsNotPartOfSender(sender, e)
                || GetDragSourceIgnore((UIElement)sender))
            {
                _dragInfo = null;
                return;
            }

            _dragInfo = new DragInfo(sender, e);

            var dragHandler = TryGetDragHandler(_dragInfo, sender as UIElement);
            if (!dragHandler.CanStartDrag(_dragInfo))
            {
                _dragInfo = null;
                return;
            }

            var itemsControl = sender as ItemsControl;

            if (_dragInfo.VisualSourceItem != null && itemsControl != null && itemsControl.CanSelectMultipleItems())
            {
                var selectedItems = itemsControl.GetSelectedItems().Cast<object>().ToList();

                if (selectedItems.Count() > 1 && selectedItems.Contains(_dragInfo.SourceItem))
                {
                    _clickSupressItem = _dragInfo.SourceItem;
                    e.Handled = true;
                }
            }
        }

        private static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var itemsControl = sender as ItemsControl;

            if (itemsControl != null && _dragInfo != null && _clickSupressItem == _dragInfo.SourceItem)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                {
                    itemsControl.SetItemSelected(_dragInfo.SourceItem, false);
                }
                else
                {
                    itemsControl.SetSelectedItem(_dragInfo.SourceItem);
                }
            }

            _dragInfo = null;
            _clickSupressItem = null;
        }

        private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragInfo != null && !_dragInProgress)
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    _dragInfo = null;
                    return;
                }

                var dragStart = _dragInfo.DragStartPosition;
                var position = e.GetPosition((IInputElement)sender);

                if (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var dragHandler = TryGetDragHandler(_dragInfo, sender as UIElement);
                    if (dragHandler.CanStartDrag(_dragInfo))
                    {
                        dragHandler.StartDrag(_dragInfo);

                        if (_dragInfo.Effects != DragDropEffects.None && _dragInfo.Data != null)
                        {
                            var data = _dragInfo.DataObject;

                            if (data == null)
                            {
                                data = new DataObject(DataFormat.Name, _dragInfo.Data);
                            }
                            else
                            {
                                data.SetData(DataFormat.Name, _dragInfo.Data);
                            }

                            try
                            {
                                _dragInProgress = true;
                                var result = System.Windows.DragDrop.DoDragDrop(_dragInfo.VisualSource, data, _dragInfo.Effects);
                                if (result == DragDropEffects.None)
                                {
                                    dragHandler.DragCancelled();
                                }
                            }
                            finally
                            {
                                _dragInProgress = false;
                            }

                            _dragInfo = null;
                        }
                    }
                }
            }
        }

        private static void DragSource_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel || e.EscapePressed)
            {
                DragAdorner = null;
                EffectAdorner = null;
                DropTargetAdorner = null;
            }
        }

        private static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            DropTarget_PreviewDragOver(sender, e);

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            DragAdorner = null;
            EffectAdorner = null;
            DropTargetAdorner = null;

            Mouse.OverrideCursor = null;
        }

        private static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            var elementPosition = e.GetPosition((IInputElement)sender);
            if (HitTestUtilities.HitTest4Type<ScrollBar>(sender, elementPosition)
                || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
                || HitTestUtilities.HitTest4DataGridTypesOnDragOver(sender, elementPosition))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            var dropInfo = new DropInfo(sender, e, _dragInfo);
            var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);
            var itemsControl = dropInfo.VisualTarget;

            dropHandler.DragOver(dropInfo);

            if (DragAdorner == null && _dragInfo != null)
            {
                CreateDragAdorner();
            }

            if (DragAdorner != null)
            {
                var tempAdornerPos = e.GetPosition(DragAdorner.AdornedElement);

                if (tempAdornerPos.X >= 0 && tempAdornerPos.Y >= 0)
                {
                    _adornerPos = tempAdornerPos;
                }

                if (DragAdorner.RenderSize.Width > 0 && DragAdorner.RenderSize.Height > 0)
                {
                    _adornerSize = DragAdorner.RenderSize;
                }

                if (_dragInfo != null)
                {
                    var offsetX = _adornerSize.Width * -GetDragMouseAnchorPoint(_dragInfo.VisualSource).X;
                    var offsetY = _adornerSize.Height * -GetDragMouseAnchorPoint(_dragInfo.VisualSource).Y;
                    _adornerPos.Offset(offsetX, offsetY);
                    var maxAdornerPosX = DragAdorner.AdornedElement.RenderSize.Width;
                    var adornerPosRightX = (_adornerPos.X + _adornerSize.Width);
                    if (adornerPosRightX > maxAdornerPosX)
                    {
                        _adornerPos.Offset(-adornerPosRightX + maxAdornerPosX, 0);
                    }
                }

                DragAdorner.MousePosition = _adornerPos;
                DragAdorner.InvalidateVisual();
            }

            if (itemsControl != null)
            {
                var adornedElement =
                    itemsControl.GetVisualDescendent<ItemsPresenter>() ??
                    (UIElement)itemsControl.GetVisualDescendent<ScrollContentPresenter>();

                if (adornedElement != null)
                {
                    if (dropInfo.DropTargetAdorner == null)
                    {
                        DropTargetAdorner = null;
                    }
                    else if (!dropInfo.DropTargetAdorner.IsInstanceOfType(DropTargetAdorner))
                    {
                        DropTargetAdorner = DropTargetAdorner.Create(dropInfo.DropTargetAdorner, adornedElement);
                    }

                    if (DropTargetAdorner != null)
                    {
                        DropTargetAdorner.DropInfo = dropInfo;
                        DropTargetAdorner.InvalidateVisual();
                    }
                }
            }

            if (EffectAdorner == null && _dragInfo != null)
            {
                CreateEffectAdorner(dropInfo);
            }

            if (EffectAdorner != null)
            {
                var adornerPos = e.GetPosition(EffectAdorner.AdornedElement);
                adornerPos.Offset(20, 20);
                EffectAdorner.MousePosition = adornerPos;
                EffectAdorner.InvalidateVisual();
            }

            e.Effects = dropInfo.Effects;
            e.Handled = !dropInfo.NotHandled;

            if (!dropInfo.IsSameDragDropContextAsSource)
            {
                e.Effects = DragDropEffects.None;
            }

            Scroll(dropInfo.VisualTarget, e);
        }

        private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            var dropInfo = new DropInfo(sender, e, _dragInfo);
            var dropHandler = TryGetDropHandler(dropInfo, sender as UIElement);
            var dragHandler = TryGetDragHandler(_dragInfo, sender as UIElement);

            DragAdorner = null;
            EffectAdorner = null;
            DropTargetAdorner = null;

            dropHandler.Drop(dropInfo);
            dragHandler.Dropped(dropInfo);

            Mouse.OverrideCursor = null;
            e.Handled = !dropInfo.NotHandled;
        }

        private static void DropTarget_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (EffectAdorner != null)
            {
                e.UseDefaultCursors = false;
                e.Handled = true;
            }
            else
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }

        #endregion
    }
}