using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Threading;

using SharpLib.Docking.Controls;
using SharpLib.Docking.Layout;
using SharpLib.Docking.Themes;

namespace SharpLib.Docking
{
    [ContentProperty("Layout")]
    [TemplatePart(Name = "PART_AutoHideArea")]
    public class DockingManager : Control, IOverlayWindowHost
    {
        #region Поля

        private static readonly DependencyPropertyKey _autoHideWindowPropertyKey;

        public static readonly DependencyProperty ActiveContentProperty;

        public static readonly DependencyProperty AllowMixedOrientationProperty;

        public static readonly DependencyProperty AnchorGroupTemplateProperty;

        public static readonly DependencyProperty AnchorSideTemplateProperty;

        public static readonly DependencyProperty AnchorTemplateProperty;

        public static readonly DependencyProperty AnchorableContextMenuProperty;

        public static readonly DependencyProperty AnchorableHeaderTemplateProperty;

        public static readonly DependencyProperty AnchorableHeaderTemplateSelectorProperty;

        public static readonly DependencyProperty AnchorablePaneControlStyleProperty;

        public static readonly DependencyProperty AnchorablePaneTemplateProperty;

        public static readonly DependencyProperty AnchorableTitleTemplateProperty;

        public static readonly DependencyProperty AnchorableTitleTemplateSelectorProperty;

        public static readonly DependencyProperty AnchorablesSourceProperty;

        public static readonly DependencyProperty AutoHideWindowProperty;

        public static readonly DependencyProperty BottomSidePanelProperty;

        public static readonly DependencyProperty DocumentContextMenuProperty;

        public static readonly DependencyProperty DocumentHeaderTemplateProperty;

        public static readonly DependencyProperty DocumentHeaderTemplateSelectorProperty;

        public static readonly DependencyProperty DocumentPaneControlStyleProperty;

        public static readonly DependencyProperty DocumentPaneMenuItemHeaderTemplateProperty;

        public static readonly DependencyProperty DocumentPaneMenuItemHeaderTemplateSelectorProperty;

        public static readonly DependencyProperty DocumentPaneTemplateProperty;

        public static readonly DependencyProperty DocumentTitleTemplateProperty;

        public static readonly DependencyProperty DocumentTitleTemplateSelectorProperty;

        public static readonly DependencyProperty DocumentsSourceProperty;

        public static readonly DependencyProperty GridSplitterHeightProperty;

        public static readonly DependencyProperty GridSplitterWidthProperty;

        public static readonly DependencyProperty IconContentTemplateProperty;

        public static readonly DependencyProperty IconContentTemplateSelectorProperty;

        public static readonly DependencyProperty LayoutItemContainerStyleProperty;

        public static readonly DependencyProperty LayoutItemContainerStyleSelectorProperty;

        public static readonly DependencyProperty LayoutItemTemplateProperty;

        public static readonly DependencyProperty LayoutItemTemplateSelectorProperty;

        public static readonly DependencyProperty LayoutProperty;

        public static readonly DependencyProperty LayoutRootPanelProperty;

        public static readonly DependencyProperty LayoutUpdateStrategyProperty;

        public static readonly DependencyProperty LeftSidePanelProperty;

        public static readonly DependencyProperty RightSidePanelProperty;

        public static readonly DependencyProperty ShowSystemMenuProperty;

        public static readonly DependencyProperty ThemeProperty;

        public static readonly DependencyProperty TopSidePanelProperty;

        private readonly List<LayoutFloatingWindowControl> _fwList;

        private readonly List<LayoutItem> _layoutItems;

        private readonly List<WeakReference> _logicalChildren;

        internal bool SuspendAnchorablesSourceBinding;

        internal bool SuspendDocumentsSourceBinding;

        private List<IDropArea> _areas;

        private AutoHideWindowManager _autoHideWindowManager;

        private FrameworkElement _autohideArea;

        private DispatcherOperation _collectLayoutItemsOperations;

        private bool _insideInternalSetActiveContent;

        private NavigatorWindow _navigatorWindow;

        private OverlayWindow _overlayWindow;

        private DispatcherOperation _setFocusAsyncOperation;

        private bool _suspendLayoutItemCreation;

        #endregion

        #region Свойства

        public LayoutRoot Layout
        {
            get { return (LayoutRoot)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public ILayoutUpdateStrategy LayoutUpdateStrategy
        {
            get { return (ILayoutUpdateStrategy)GetValue(LayoutUpdateStrategyProperty); }
            set { SetValue(LayoutUpdateStrategyProperty, value); }
        }

        public ControlTemplate DocumentPaneTemplate
        {
            get { return (ControlTemplate)GetValue(DocumentPaneTemplateProperty); }
            set { SetValue(DocumentPaneTemplateProperty, value); }
        }

        public ControlTemplate AnchorablePaneTemplate
        {
            get { return (ControlTemplate)GetValue(AnchorablePaneTemplateProperty); }
            set { SetValue(AnchorablePaneTemplateProperty, value); }
        }

        public ControlTemplate AnchorSideTemplate
        {
            get { return (ControlTemplate)GetValue(AnchorSideTemplateProperty); }
            set { SetValue(AnchorSideTemplateProperty, value); }
        }

        public ControlTemplate AnchorGroupTemplate
        {
            get { return (ControlTemplate)GetValue(AnchorGroupTemplateProperty); }
            set { SetValue(AnchorGroupTemplateProperty, value); }
        }

        public ControlTemplate AnchorTemplate
        {
            get { return (ControlTemplate)GetValue(AnchorTemplateProperty); }
            set { SetValue(AnchorTemplateProperty, value); }
        }

        public Style DocumentPaneControlStyle
        {
            get { return (Style)GetValue(DocumentPaneControlStyleProperty); }
            set { SetValue(DocumentPaneControlStyleProperty, value); }
        }

        public Style AnchorablePaneControlStyle
        {
            get { return (Style)GetValue(AnchorablePaneControlStyleProperty); }
            set { SetValue(AnchorablePaneControlStyleProperty, value); }
        }

        public DataTemplate DocumentHeaderTemplate
        {
            get { return (DataTemplate)GetValue(DocumentHeaderTemplateProperty); }
            set { SetValue(DocumentHeaderTemplateProperty, value); }
        }

        public DataTemplateSelector DocumentHeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DocumentHeaderTemplateSelectorProperty); }
            set { SetValue(DocumentHeaderTemplateSelectorProperty, value); }
        }

        public DataTemplate DocumentTitleTemplate
        {
            get { return (DataTemplate)GetValue(DocumentTitleTemplateProperty); }
            set { SetValue(DocumentTitleTemplateProperty, value); }
        }

        public DataTemplateSelector DocumentTitleTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DocumentTitleTemplateSelectorProperty); }
            set { SetValue(DocumentTitleTemplateSelectorProperty, value); }
        }

        public DataTemplate AnchorableTitleTemplate
        {
            get { return (DataTemplate)GetValue(AnchorableTitleTemplateProperty); }
            set { SetValue(AnchorableTitleTemplateProperty, value); }
        }

        public DataTemplateSelector AnchorableTitleTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(AnchorableTitleTemplateSelectorProperty); }
            set { SetValue(AnchorableTitleTemplateSelectorProperty, value); }
        }

        public DataTemplate AnchorableHeaderTemplate
        {
            get { return (DataTemplate)GetValue(AnchorableHeaderTemplateProperty); }
            set { SetValue(AnchorableHeaderTemplateProperty, value); }
        }

        public DataTemplateSelector AnchorableHeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(AnchorableHeaderTemplateSelectorProperty); }
            set { SetValue(AnchorableHeaderTemplateSelectorProperty, value); }
        }

        public LayoutPanelControl LayoutRootPanel
        {
            get { return (LayoutPanelControl)GetValue(LayoutRootPanelProperty); }
            set { SetValue(LayoutRootPanelProperty, value); }
        }

        public LayoutAnchorSideControl RightSidePanel
        {
            get { return (LayoutAnchorSideControl)GetValue(RightSidePanelProperty); }
            set { SetValue(RightSidePanelProperty, value); }
        }

        public LayoutAnchorSideControl LeftSidePanel
        {
            get { return (LayoutAnchorSideControl)GetValue(LeftSidePanelProperty); }
            set { SetValue(LeftSidePanelProperty, value); }
        }

        public LayoutAnchorSideControl TopSidePanel
        {
            get { return (LayoutAnchorSideControl)GetValue(TopSidePanelProperty); }
            set { SetValue(TopSidePanelProperty, value); }
        }

        public LayoutAnchorSideControl BottomSidePanel
        {
            get { return (LayoutAnchorSideControl)GetValue(BottomSidePanelProperty); }
            set { SetValue(BottomSidePanelProperty, value); }
        }

        protected override IEnumerator LogicalChildren
        {
            get { return _logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).GetEnumerator(); }
        }

        public LayoutAutoHideWindowControl AutoHideWindow
        {
            get { return (LayoutAutoHideWindowControl)GetValue(AutoHideWindowProperty); }
        }

        public IEnumerable<LayoutFloatingWindowControl> FloatingWindows
        {
            get { return _fwList; }
        }

        DockingManager IOverlayWindowHost.Manager
        {
            get { return this; }
        }

        public DataTemplate LayoutItemTemplate
        {
            get { return (DataTemplate)GetValue(LayoutItemTemplateProperty); }
            set { SetValue(LayoutItemTemplateProperty, value); }
        }

        public DataTemplateSelector LayoutItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(LayoutItemTemplateSelectorProperty); }
            set { SetValue(LayoutItemTemplateSelectorProperty, value); }
        }

        public IEnumerable DocumentsSource
        {
            get { return (IEnumerable)GetValue(DocumentsSourceProperty); }
            set { SetValue(DocumentsSourceProperty, value); }
        }

        public ContextMenu DocumentContextMenu
        {
            get { return (ContextMenu)GetValue(DocumentContextMenuProperty); }
            set { SetValue(DocumentContextMenuProperty, value); }
        }

        public IEnumerable AnchorablesSource
        {
            get { return (IEnumerable)GetValue(AnchorablesSourceProperty); }
            set { SetValue(AnchorablesSourceProperty, value); }
        }

        public object ActiveContent
        {
            get { return GetValue(ActiveContentProperty); }
            set { SetValue(ActiveContentProperty, value); }
        }

        public ContextMenu AnchorableContextMenu
        {
            get { return (ContextMenu)GetValue(AnchorableContextMenuProperty); }
            set { SetValue(AnchorableContextMenuProperty, value); }
        }

        public Theme Theme
        {
            get { return (Theme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        public double GridSplitterWidth
        {
            get { return (double)GetValue(GridSplitterWidthProperty); }
            set { SetValue(GridSplitterWidthProperty, value); }
        }

        public double GridSplitterHeight
        {
            get { return (double)GetValue(GridSplitterHeightProperty); }
            set { SetValue(GridSplitterHeightProperty, value); }
        }

        public DataTemplate DocumentPaneMenuItemHeaderTemplate
        {
            get { return (DataTemplate)GetValue(DocumentPaneMenuItemHeaderTemplateProperty); }
            set { SetValue(DocumentPaneMenuItemHeaderTemplateProperty, value); }
        }

        public DataTemplateSelector DocumentPaneMenuItemHeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DocumentPaneMenuItemHeaderTemplateSelectorProperty); }
            set { SetValue(DocumentPaneMenuItemHeaderTemplateSelectorProperty, value); }
        }

        public DataTemplate IconContentTemplate
        {
            get { return (DataTemplate)GetValue(IconContentTemplateProperty); }
            set { SetValue(IconContentTemplateProperty, value); }
        }

        public DataTemplateSelector IconContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(IconContentTemplateSelectorProperty); }
            set { SetValue(IconContentTemplateSelectorProperty, value); }
        }

        public Style LayoutItemContainerStyle
        {
            get { return (Style)GetValue(LayoutItemContainerStyleProperty); }
            set { SetValue(LayoutItemContainerStyleProperty, value); }
        }

        public StyleSelector LayoutItemContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(LayoutItemContainerStyleSelectorProperty); }
            set { SetValue(LayoutItemContainerStyleSelectorProperty, value); }
        }

        private bool IsNavigatorWindowActive
        {
            get { return _navigatorWindow != null; }
        }

        public bool ShowSystemMenu
        {
            get { return (bool)GetValue(ShowSystemMenuProperty); }
            set { SetValue(ShowSystemMenuProperty, value); }
        }

        public bool AllowMixedOrientation
        {
            get { return (bool)GetValue(AllowMixedOrientationProperty); }
            set { SetValue(AllowMixedOrientationProperty, value); }
        }

        #endregion

        #region События

        public event EventHandler ActiveContentChanged;

        public event EventHandler<DocumentClosedEventArgs> DocumentClosed;

        public event EventHandler<DocumentClosingEventArgs> DocumentClosing;

        public event EventHandler LayoutChanged;

        public event EventHandler LayoutChanging;

        #endregion

        #region Конструктор

        static DockingManager()
        {
            ActiveContentProperty = DependencyProperty.Register("ActiveContent", typeof(object), typeof(DockingManager), new FrameworkPropertyMetadata(null, OnActiveContentChanged));
            AllowMixedOrientationProperty = DependencyProperty.Register("AllowMixedOrientation", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));
            AnchorGroupTemplateProperty = DependencyProperty.Register("AnchorGroupTemplate", typeof(ControlTemplate), typeof(DockingManager), new FrameworkPropertyMetadata((ControlTemplate)null));
            AnchorSideTemplateProperty = DependencyProperty.Register("AnchorSideTemplate", typeof(ControlTemplate), typeof(DockingManager), new FrameworkPropertyMetadata((ControlTemplate)null));
            AnchorTemplateProperty = DependencyProperty.Register("AnchorTemplate", typeof(ControlTemplate), typeof(DockingManager), new FrameworkPropertyMetadata((ControlTemplate)null));
            AnchorableContextMenuProperty = DependencyProperty.Register("AnchorableContextMenu", typeof(ContextMenu), typeof(DockingManager), new FrameworkPropertyMetadata((ContextMenu)null));
            AnchorableHeaderTemplateProperty = DependencyProperty.Register("AnchorableHeaderTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAnchorableHeaderTemplateChanged, CoerceAnchorableHeaderTemplateValue));
            AnchorableHeaderTemplateSelectorProperty = DependencyProperty.Register("AnchorableHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAnchorableHeaderTemplateSelectorChanged));
            AnchorablePaneControlStyleProperty = DependencyProperty.Register("AnchorablePaneControlStyle", typeof(Style), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAnchorablePaneControlStyleChanged));
            AnchorablePaneTemplateProperty = DependencyProperty.Register("AnchorablePaneTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAnchorablePaneTemplateChanged));
            AnchorableTitleTemplateProperty = DependencyProperty.Register("AnchorableTitleTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAnchorableTitleTemplateChanged, CoerceAnchorableTitleTemplateValue));
            AnchorableTitleTemplateSelectorProperty = DependencyProperty.Register("AnchorableTitleTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAnchorableTitleTemplateSelectorChanged));
            AnchorablesSourceProperty = DependencyProperty.Register("AnchorablesSource", typeof(IEnumerable), typeof(DockingManager), new FrameworkPropertyMetadata(null, OnAnchorablesSourceChanged));
            _autoHideWindowPropertyKey = DependencyProperty.RegisterReadOnly("AutoHideWindow", typeof(LayoutAutoHideWindowControl), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnAutoHideWindowChanged));
            BottomSidePanelProperty = DependencyProperty.Register("BottomSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnBottomSidePanelChanged));
            DocumentContextMenuProperty = DependencyProperty.Register("DocumentContextMenu", typeof(ContextMenu), typeof(DockingManager), new FrameworkPropertyMetadata((ContextMenu)null));
            DocumentHeaderTemplateProperty = DependencyProperty.Register("DocumentHeaderTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentHeaderTemplateChanged, CoerceDocumentHeaderTemplateValue));
            DocumentHeaderTemplateSelectorProperty = DependencyProperty.Register("DocumentHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentHeaderTemplateSelectorChanged, CoerceDocumentHeaderTemplateSelectorValue));
            DocumentPaneControlStyleProperty = DependencyProperty.Register("DocumentPaneControlStyle", typeof(Style), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentPaneControlStyleChanged));
            DocumentPaneMenuItemHeaderTemplateProperty = DependencyProperty.Register("DocumentPaneMenuItemHeaderTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentPaneMenuItemHeaderTemplateChanged, CoerceDocumentPaneMenuItemHeaderTemplateValue));
            DocumentPaneMenuItemHeaderTemplateSelectorProperty = DependencyProperty.Register("DocumentPaneMenuItemHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentPaneMenuItemHeaderTemplateSelectorChanged, CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue));
            DocumentPaneTemplateProperty = DependencyProperty.Register("DocumentPaneTemplate", typeof(ControlTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentPaneTemplateChanged));
            DocumentTitleTemplateProperty = DependencyProperty.Register("DocumentTitleTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentTitleTemplateChanged, CoerceDocumentTitleTemplateValue));
            DocumentTitleTemplateSelectorProperty = DependencyProperty.Register("DocumentTitleTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnDocumentTitleTemplateSelectorChanged, CoerceDocumentTitleTemplateSelectorValue));
            DocumentsSourceProperty = DependencyProperty.Register("DocumentsSource", typeof(IEnumerable), typeof(DockingManager), new FrameworkPropertyMetadata(null, OnDocumentsSourceChanged));
            GridSplitterHeightProperty = DependencyProperty.Register("GridSplitterHeight", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(6.0));
            GridSplitterWidthProperty = DependencyProperty.Register("GridSplitterWidth", typeof(double), typeof(DockingManager), new FrameworkPropertyMetadata(6.0));
            IconContentTemplateProperty = DependencyProperty.Register("IconContentTemplate", typeof(DataTemplate), typeof(DockingManager), new FrameworkPropertyMetadata((DataTemplate)null));
            IconContentTemplateSelectorProperty = DependencyProperty.Register("IconContentTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata((DataTemplateSelector)null));
            LayoutItemContainerStyleProperty = DependencyProperty.Register("LayoutItemContainerStyle", typeof(Style), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLayoutItemContainerStyleChanged));
            LayoutItemContainerStyleSelectorProperty = DependencyProperty.Register("LayoutItemContainerStyleSelector", typeof(StyleSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLayoutItemContainerStyleSelectorChanged));
            LayoutItemTemplateProperty = DependencyProperty.Register("LayoutItemTemplate", typeof(DataTemplate), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLayoutItemTemplateChanged));
            LayoutItemTemplateSelectorProperty = DependencyProperty.Register("LayoutItemTemplateSelector", typeof(DataTemplateSelector), typeof(DockingManager),
                new FrameworkPropertyMetadata(null, OnLayoutItemTemplateSelectorChanged));
            LayoutProperty = DependencyProperty.Register("Layout", typeof(LayoutRoot), typeof(DockingManager), new FrameworkPropertyMetadata(null, OnLayoutChanged, CoerceLayoutValue));
            LayoutRootPanelProperty = DependencyProperty.Register("LayoutRootPanel", typeof(LayoutPanelControl), typeof(DockingManager), new FrameworkPropertyMetadata(null, OnLayoutRootPanelChanged));
            LayoutUpdateStrategyProperty = DependencyProperty.Register("LayoutUpdateStrategy", typeof(ILayoutUpdateStrategy), typeof(DockingManager),
                new FrameworkPropertyMetadata((ILayoutUpdateStrategy)null));
            LeftSidePanelProperty = DependencyProperty.Register("LeftSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager), new FrameworkPropertyMetadata(null, OnLeftSidePanelChanged));
            RightSidePanelProperty = DependencyProperty.Register("RightSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager), new FrameworkPropertyMetadata(null, OnRightSidePanelChanged));
            ShowSystemMenuProperty = DependencyProperty.Register("ShowSystemMenu", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true));
            ThemeProperty = DependencyProperty.Register("Theme", typeof(Theme), typeof(DockingManager), new FrameworkPropertyMetadata(null, OnThemeChanged));
            TopSidePanelProperty = DependencyProperty.Register("TopSidePanel", typeof(LayoutAnchorSideControl), typeof(DockingManager), new FrameworkPropertyMetadata(null, OnTopSidePanelChanged));
            AutoHideWindowProperty = _autoHideWindowPropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockingManager), new FrameworkPropertyMetadata(typeof(DockingManager)));
            FocusableProperty.OverrideMetadata(typeof(DockingManager), new FrameworkPropertyMetadata(false));
            HwndSource.DefaultAcquireHwndFocusInMenuMode = false;
        }

        public DockingManager()
        {
            SuspendDocumentsSourceBinding = false;
            SuspendAnchorablesSourceBinding = false;
            _logicalChildren = new List<WeakReference>();
            _layoutItems = new List<LayoutItem>();
            _fwList = new List<LayoutFloatingWindowControl>();
            Layout = new LayoutRoot
            {
                RootPanel = new LayoutPanel(new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
            };

            Loaded += DockingManager_Loaded;
            Unloaded += DockingManager_Unloaded;
        }

        #endregion

        #region Методы

        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutChanged(e.OldValue as LayoutRoot, e.NewValue as LayoutRoot);
        }

        protected virtual void OnLayoutChanged(LayoutRoot oldLayout, LayoutRoot newLayout)
        {
            if (oldLayout != null)
            {
                oldLayout.PropertyChanged -= OnLayoutRootPropertyChanged;
                oldLayout.Updated -= OnLayoutRootUpdated;
            }

            foreach (var fwc in _fwList.ToArray())
            {
                fwc.KeepContentVisibleOnClose = true;
                fwc.InternalClose();
            }

            _fwList.Clear();

            DetachDocumentsSource(oldLayout, DocumentsSource);
            DetachAnchorablesSource(oldLayout, AnchorablesSource);

            if (oldLayout != null && Equals(oldLayout.Manager, this))
            {
                oldLayout.Manager = null;
            }

            ClearLogicalChildrenList();
            DetachLayoutItems();

            Layout.Manager = this;

            AttachLayoutItems();
            AttachDocumentsSource(newLayout, DocumentsSource);
            AttachAnchorablesSource(newLayout, AnchorablesSource);

            if (IsLoaded)
            {
                LayoutRootPanel = CreateUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
                LeftSidePanel = CreateUIElementForModel(Layout.LeftSide) as LayoutAnchorSideControl;
                TopSidePanel = CreateUIElementForModel(Layout.TopSide) as LayoutAnchorSideControl;
                RightSidePanel = CreateUIElementForModel(Layout.RightSide) as LayoutAnchorSideControl;
                BottomSidePanel = CreateUIElementForModel(Layout.BottomSide) as LayoutAnchorSideControl;

                foreach (var fw in Layout.FloatingWindows.ToArray())
                {
                    if (fw.IsValid)
                    {
                        _fwList.Add(CreateUIElementForModel(fw) as LayoutFloatingWindowControl);
                    }
                }
            }

            if (newLayout != null)
            {
                newLayout.PropertyChanged += OnLayoutRootPropertyChanged;
                newLayout.Updated += OnLayoutRootUpdated;
            }

            if (LayoutChanged != null)
            {
                LayoutChanged(this, EventArgs.Empty);
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private void OnLayoutRootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "RootPanel")
            {
                if (IsInitialized)
                {
                    var layoutRootPanel = CreateUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
                    LayoutRootPanel = layoutRootPanel;
                }
            }
            else if (e.PropertyName == "ActiveContent")
            {
                if (Layout.ActiveContent != null)
                {
                    if (_setFocusAsyncOperation == null)
                    {
                        _setFocusAsyncOperation = Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (Layout.ActiveContent != null)
                            {
                                FocusElementManager.SetFocusOnLastElement(Layout.ActiveContent);
                            }
                            _setFocusAsyncOperation = null;
                        }), DispatcherPriority.Background);
                    }
                }

                if (!_insideInternalSetActiveContent)
                {
                    ActiveContent = Layout.ActiveContent != null
                        ? Layout.ActiveContent.Content
                        : null;
                }
            }
        }

        private void OnLayoutRootUpdated(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private static object CoerceLayoutValue(DependencyObject d, object value)
        {
            if (value == null)
            {
                return new LayoutRoot
                {
                    RootPanel = new LayoutPanel(new LayoutDocumentPaneGroup(new LayoutDocumentPane()))
                };
            }

            ((DockingManager)d).OnLayoutChanging();

            return value;
        }

        private void OnLayoutChanging()
        {
            if (LayoutChanging != null)
            {
                LayoutChanging(this, EventArgs.Empty);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SetupAutoHideWindow();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (Equals(Layout.Manager, this))
            {
                LayoutRootPanel = CreateUIElementForModel(Layout.RootPanel) as LayoutPanelControl;
                LeftSidePanel = CreateUIElementForModel(Layout.LeftSide) as LayoutAnchorSideControl;
                TopSidePanel = CreateUIElementForModel(Layout.TopSide) as LayoutAnchorSideControl;
                RightSidePanel = CreateUIElementForModel(Layout.RightSide) as LayoutAnchorSideControl;
                BottomSidePanel = CreateUIElementForModel(Layout.BottomSide) as LayoutAnchorSideControl;
            }
        }

        private void DockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                foreach (var fw in Layout.FloatingWindows.Where(fw => !_fwList.Any(fwc => Equals(fwc.Model, fw))))
                {
                    _fwList.Add(CreateUIElementForModel(fw) as LayoutFloatingWindowControl);
                }

                if (IsVisible)
                {
                    CreateOverlayWindow();
                }
                FocusElementManager.SetupFocusManagement(this);
            }
        }

        private void DockingManager_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _autoHideWindowManager.HideAutoWindow();

                foreach (var fw in _fwList.ToArray())
                {
                    fw.SetParentWindowToNull();
                    fw.KeepContentVisibleOnClose = true;
                    fw.Close();
                }

                DestroyOverlayWindow();
                FocusElementManager.FinalizeFocusManagement(this);
            }
        }

        internal UIElement CreateUIElementForModel(ILayoutElement model)
        {
            if (model is LayoutPanel)
            {
                return new LayoutPanelControl(model as LayoutPanel);
            }
            if (model is LayoutAnchorablePaneGroup)
            {
                return new LayoutAnchorablePaneGroupControl(model as LayoutAnchorablePaneGroup);
            }
            if (model is LayoutDocumentPaneGroup)
            {
                return new LayoutDocumentPaneGroupControl(model as LayoutDocumentPaneGroup);
            }

            if (model is LayoutAnchorSide)
            {
                var templateModelView = new LayoutAnchorSideControl(model as LayoutAnchorSide);
                templateModelView.SetBinding(TemplateProperty, new Binding("AnchorSideTemplate")
                {
                    Source = this
                });
                return templateModelView;
            }
            if (model is LayoutAnchorGroup)
            {
                var templateModelView = new LayoutAnchorGroupControl(model as LayoutAnchorGroup);
                templateModelView.SetBinding(TemplateProperty, new Binding("AnchorGroupTemplate")
                {
                    Source = this
                });
                return templateModelView;
            }

            if (model is LayoutDocumentPane)
            {
                var templateModelView = new LayoutDocumentPaneControl(model as LayoutDocumentPane);
                templateModelView.SetBinding(StyleProperty, new Binding("DocumentPaneControlStyle")
                {
                    Source = this
                });
                return templateModelView;
            }
            if (model is LayoutAnchorablePane)
            {
                var templateModelView = new LayoutAnchorablePaneControl(model as LayoutAnchorablePane);
                templateModelView.SetBinding(StyleProperty, new Binding("AnchorablePaneControlStyle")
                {
                    Source = this
                });
                return templateModelView;
            }

            if (model is LayoutAnchorableFloatingWindow)
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    return null;
                }
                var modelFw = model as LayoutAnchorableFloatingWindow;
                var newFw = new LayoutAnchorableFloatingWindowControl(modelFw);
                newFw.SetParentToMainWindowOf(this);

                var paneForExtensions = modelFw.RootPanel.Children.OfType<LayoutAnchorablePane>().FirstOrDefault();
                if (paneForExtensions != null)
                {
                    paneForExtensions.KeepInsideNearestMonitor();

                    newFw.Left = paneForExtensions.FloatingLeft;
                    newFw.Top = paneForExtensions.FloatingTop;
                    newFw.Width = paneForExtensions.FloatingWidth;
                    newFw.Height = paneForExtensions.FloatingHeight;
                }

                newFw.ShowInTaskbar = false;
                newFw.Show();

                if (paneForExtensions != null && paneForExtensions.IsMaximized)
                {
                    newFw.WindowState = WindowState.Maximized;
                }
                return newFw;
            }

            if (model is LayoutDocumentFloatingWindow)
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    return null;
                }
                var modelFw = model as LayoutDocumentFloatingWindow;
                var newFw = new LayoutDocumentFloatingWindowControl(modelFw);
                newFw.SetParentToMainWindowOf(this);

                var paneForExtensions = modelFw.RootDocument;
                if (paneForExtensions != null)
                {
                    paneForExtensions.KeepInsideNearestMonitor();

                    newFw.Left = paneForExtensions.FloatingLeft;
                    newFw.Top = paneForExtensions.FloatingTop;
                    newFw.Width = paneForExtensions.FloatingWidth;
                    newFw.Height = paneForExtensions.FloatingHeight;
                }

                newFw.ShowInTaskbar = false;
                newFw.Show();

                if (paneForExtensions != null && paneForExtensions.IsMaximized)
                {
                    newFw.WindowState = WindowState.Maximized;
                }
                return newFw;
            }

            if (model is LayoutDocument)
            {
                var templateModelView = new LayoutDocumentControl
                {
                    Model = model as LayoutDocument
                };
                return templateModelView;
            }

            return null;
        }

        private static void OnDocumentPaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentPaneTemplateChanged(e);
        }

        protected virtual void OnDocumentPaneTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnAnchorablePaneTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorablePaneTemplateChanged(e);
        }

        protected virtual void OnAnchorablePaneTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnDocumentPaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentPaneControlStyleChanged(e);
        }

        protected virtual void OnDocumentPaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnAnchorablePaneControlStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorablePaneControlStyleChanged(e);
        }

        protected virtual void OnAnchorablePaneControlStyleChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnDocumentHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentHeaderTemplateChanged(e);
        }

        protected virtual void OnDocumentHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceDocumentHeaderTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(DocumentHeaderTemplateSelectorProperty) != null)
            {
                return null;
            }
            return value;
        }

        private static void OnDocumentHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentHeaderTemplateSelectorChanged(e);
        }

        protected virtual void OnDocumentHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null &&
                DocumentHeaderTemplate != null)
            {
                DocumentHeaderTemplate = null;
            }

            if (DocumentPaneMenuItemHeaderTemplateSelector == null)
            {
                DocumentPaneMenuItemHeaderTemplateSelector = DocumentHeaderTemplateSelector;
            }
        }

        private static object CoerceDocumentHeaderTemplateSelectorValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnDocumentTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentTitleTemplateChanged(e);
        }

        protected virtual void OnDocumentTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceDocumentTitleTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(DocumentTitleTemplateSelectorProperty) != null)
            {
                return null;
            }

            return value;
        }

        private static void OnDocumentTitleTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentTitleTemplateSelectorChanged(e);
        }

        protected virtual void OnDocumentTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DocumentTitleTemplate = null;
            }
        }

        private static object CoerceDocumentTitleTemplateSelectorValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnAnchorableTitleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorableTitleTemplateChanged(e);
        }

        protected virtual void OnAnchorableTitleTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceAnchorableTitleTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(AnchorableTitleTemplateSelectorProperty) != null)
            {
                return null;
            }
            return value;
        }

        private static void OnAnchorableTitleTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorableTitleTemplateSelectorChanged(e);
        }

        protected virtual void OnAnchorableTitleTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null &&
                AnchorableTitleTemplate != null)
            {
                AnchorableTitleTemplate = null;
            }
        }

        private static void OnAnchorableHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorableHeaderTemplateChanged(e);
        }

        protected virtual void OnAnchorableHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceAnchorableHeaderTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(AnchorableHeaderTemplateSelectorProperty) != null)
            {
                return null;
            }

            return value;
        }

        private static void OnAnchorableHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorableHeaderTemplateSelectorChanged(e);
        }

        protected virtual void OnAnchorableHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                AnchorableHeaderTemplate = null;
            }
        }

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Trace.WriteLine(string.Format("DockingManager.OnPreviewGotKeyboardFocus({0})", e.NewFocus));

            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Trace.WriteLine(string.Format("DockingManager.OnPreviewLostKeyboardFocus({0})", e.OldFocus));
            base.OnPreviewLostKeyboardFocus(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Trace.WriteLine(string.Format("DockingManager.OnMouseLeftButtonDown([{0}])", e.GetPosition(this)));
            base.OnMouseLeftButtonDown(e);
        }

        private static void OnLayoutRootPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutRootPanelChanged(e);
        }

        protected virtual void OnLayoutRootPanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        private static void OnRightSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnRightSidePanelChanged(e);
        }

        protected virtual void OnRightSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        private static void OnLeftSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLeftSidePanelChanged(e);
        }

        protected virtual void OnLeftSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        private static void OnTopSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnTopSidePanelChanged(e);
        }

        protected virtual void OnTopSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        private static void OnBottomSidePanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnBottomSidePanelChanged(e);
        }

        protected virtual void OnBottomSidePanelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        internal void InternalAddLogicalChild(object element)
        {
            if (_logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).Contains(element))
            {
                return;
            }

            _logicalChildren.Add(new WeakReference(element));
            AddLogicalChild(element);
        }

        internal void InternalRemoveLogicalChild(object element)
        {
            var wrToRemove = _logicalChildren.FirstOrDefault(ch => ch.GetValueOrDefault<object>() == element);
            if (wrToRemove != null)
            {
                _logicalChildren.Remove(wrToRemove);
            }
            RemoveLogicalChild(element);
        }

        private void ClearLogicalChildrenList()
        {
            foreach (var child in _logicalChildren.Select(ch => ch.GetValueOrDefault<object>()).ToArray())
            {
                RemoveLogicalChild(child);
            }
            _logicalChildren.Clear();
        }

        internal void ShowAutoHideWindow(LayoutAnchorControl anchor)
        {
            _autoHideWindowManager.ShowAutoHideWindow(anchor);
        }

        internal void HideAutoHideWindow(LayoutAnchorControl anchor)
        {
            _autoHideWindowManager.HideAutoWindow(anchor);
        }

        private void SetupAutoHideWindow()
        {
            _autohideArea = GetTemplateChild("PART_AutoHideArea") as FrameworkElement;

            if (_autoHideWindowManager != null)
            {
                _autoHideWindowManager.HideAutoWindow();
            }
            else
            {
                _autoHideWindowManager = new AutoHideWindowManager(this);
            }

            if (AutoHideWindow != null)
            {
                AutoHideWindow.Dispose();
            }

            SetAutoHideWindow(new LayoutAutoHideWindowControl());
        }

        internal FrameworkElement GetAutoHideAreaElement()
        {
            return _autohideArea;
        }

        protected void SetAutoHideWindow(LayoutAutoHideWindowControl value)
        {
            SetValue(_autoHideWindowPropertyKey, value);
        }

        private static void OnAutoHideWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAutoHideWindowChanged(e);
        }

        protected virtual void OnAutoHideWindowChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                InternalRemoveLogicalChild(e.OldValue);
            }
            if (e.NewValue != null)
            {
                InternalAddLogicalChild(e.NewValue);
            }
        }

        internal void StartDraggingFloatingWindowForContent(LayoutContent contentModel, bool startDrag = true)
        {
            if (!contentModel.CanFloat)
            {
                return;
            }
            var contentModelAsAnchorable = contentModel as LayoutAnchorable;
            if (contentModelAsAnchorable != null &&
                contentModelAsAnchorable.IsAutoHidden)
            {
                contentModelAsAnchorable.ToggleAutoHide();
            }

            var parentPane = contentModel.Parent as ILayoutPane;
            var parentPaneAsPositionableElement = contentModel.Parent as ILayoutPositionableElement;
            var parentPaneAsWithActualSize = contentModel.Parent as ILayoutPositionableElementWithActualSize;
            var contentModelParentChildrenIndex = parentPane.Children.ToList().IndexOf(contentModel);

            if (contentModel.FindParent<LayoutFloatingWindow>() == null)
            {
                ((ILayoutPreviousContainer)contentModel).PreviousContainer = parentPane;
                contentModel.PreviousContainerIndex = contentModelParentChildrenIndex;
            }

            parentPane.RemoveChildAt(contentModelParentChildrenIndex);

            double fwWidth = contentModel.FloatingWidth;
            double fwHeight = contentModel.FloatingHeight;

            if (fwWidth == 0.0)
            {
                fwWidth = parentPaneAsPositionableElement.FloatingWidth;
            }
            if (fwHeight == 0.0)
            {
                fwHeight = parentPaneAsPositionableElement.FloatingHeight;
            }

            if (fwWidth == 0.0)
            {
                fwWidth = parentPaneAsWithActualSize.ActualWidth;
            }
            if (fwHeight == 0.0)
            {
                fwHeight = parentPaneAsWithActualSize.ActualHeight;
            }

            LayoutFloatingWindow fw;
            LayoutFloatingWindowControl fwc;
            if (contentModel is LayoutAnchorable)
            {
                var anchorableContent = contentModel as LayoutAnchorable;
                fw = new LayoutAnchorableFloatingWindow
                {
                    RootPanel = new LayoutAnchorablePaneGroup(
                        new LayoutAnchorablePane(anchorableContent)
                        {
                            DockWidth = parentPaneAsPositionableElement.DockWidth,
                            DockHeight = parentPaneAsPositionableElement.DockHeight,
                            DockMinHeight = parentPaneAsPositionableElement.DockMinHeight,
                            DockMinWidth = parentPaneAsPositionableElement.DockMinWidth,
                            FloatingLeft = parentPaneAsPositionableElement.FloatingLeft,
                            FloatingTop = parentPaneAsPositionableElement.FloatingTop,
                            FloatingWidth = parentPaneAsPositionableElement.FloatingWidth,
                            FloatingHeight = parentPaneAsPositionableElement.FloatingHeight,
                        })
                };

                Layout.FloatingWindows.Add(fw);

                fwc = new LayoutAnchorableFloatingWindowControl(
                    fw as LayoutAnchorableFloatingWindow)
                {
                    Width = fwWidth,
                    Height = fwHeight,
                    Left = contentModel.FloatingLeft,
                    Top = contentModel.FloatingTop
                };
            }
            else
            {
                var anchorableDocument = contentModel as LayoutDocument;
                fw = new LayoutDocumentFloatingWindow
                {
                    RootDocument = anchorableDocument
                };

                Layout.FloatingWindows.Add(fw);

                fwc = new LayoutDocumentFloatingWindowControl(
                    fw as LayoutDocumentFloatingWindow)
                {
                    Width = fwWidth,
                    Height = fwHeight,
                    Left = contentModel.FloatingLeft,
                    Top = contentModel.FloatingTop
                };
            }

            _fwList.Add(fwc);

            Layout.CollectGarbage();

            UpdateLayout();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (startDrag)
                {
                    fwc.AttachDrag();
                }
                fwc.Show();
            }), DispatcherPriority.Send);
        }

        internal void StartDraggingFloatingWindowForPane(LayoutAnchorablePane paneModel)
        {
            if (paneModel.Children.Any(c => !c.CanFloat))
            {
                return;
            }
            var paneAsPositionableElement = paneModel as ILayoutPositionableElement;
            var paneAsWithActualSize = paneModel as ILayoutPositionableElementWithActualSize;

            double fwWidth = paneAsPositionableElement.FloatingWidth;
            double fwHeight = paneAsPositionableElement.FloatingHeight;
            double fwLeft = paneAsPositionableElement.FloatingLeft;
            double fwTop = paneAsPositionableElement.FloatingTop;

            if (fwWidth == 0.0)
            {
                fwWidth = paneAsWithActualSize.ActualWidth;
            }
            if (fwHeight == 0.0)
            {
                fwHeight = paneAsWithActualSize.ActualHeight;
            }

            var destPane = new LayoutAnchorablePane
            {
                DockWidth = paneAsPositionableElement.DockWidth,
                DockHeight = paneAsPositionableElement.DockHeight,
                DockMinHeight = paneAsPositionableElement.DockMinHeight,
                DockMinWidth = paneAsPositionableElement.DockMinWidth,
                FloatingLeft = paneAsPositionableElement.FloatingLeft,
                FloatingTop = paneAsPositionableElement.FloatingTop,
                FloatingWidth = paneAsPositionableElement.FloatingWidth,
                FloatingHeight = paneAsPositionableElement.FloatingHeight,
            };

            bool savePreviousContainer = paneModel.FindParent<LayoutFloatingWindow>() == null;
            int currentSelectedContentIndex = paneModel.SelectedContentIndex;
            while (paneModel.Children.Count > 0)
            {
                var contentModel = paneModel.Children[paneModel.Children.Count - 1];

                if (savePreviousContainer)
                {
                    var contentModelAsPreviousContainer = contentModel as ILayoutPreviousContainer;
                    contentModelAsPreviousContainer.PreviousContainer = paneModel;
                    contentModel.PreviousContainerIndex = paneModel.Children.Count - 1;
                }

                paneModel.RemoveChildAt(paneModel.Children.Count - 1);
                destPane.Children.Insert(0, contentModel);
            }

            if (destPane.Children.Count > 0)
            {
                destPane.SelectedContentIndex = currentSelectedContentIndex;
            }

            LayoutFloatingWindow fw;
            LayoutFloatingWindowControl fwc;
            fw = new LayoutAnchorableFloatingWindow
            {
                RootPanel = new LayoutAnchorablePaneGroup(
                    destPane)
                {
                    DockHeight = destPane.DockHeight,
                    DockWidth = destPane.DockWidth,
                    DockMinHeight = destPane.DockMinHeight,
                    DockMinWidth = destPane.DockMinWidth,
                }
            };

            Layout.FloatingWindows.Add(fw);

            fwc = new LayoutAnchorableFloatingWindowControl(
                fw as LayoutAnchorableFloatingWindow)
            {
                Width = fwWidth,
                Height = fwHeight
            };

            _fwList.Add(fwc);

            Layout.CollectGarbage();

            InvalidateArrange();

            fwc.AttachDrag();
            fwc.Show();
        }

        internal IEnumerable<LayoutFloatingWindowControl> GetFloatingWindowsByZOrder()
        {
            var parentWindow = Window.GetWindow(this);

            if (parentWindow == null)
            {
                yield break;
            }

            var windowParentHanlde = new WindowInteropHelper(parentWindow).Handle;

            var currentHandle = Win32Helper.GetWindow(windowParentHanlde, (uint)Win32Helper.GetWindowCmd.GW_HWNDFIRST);
            while (currentHandle != IntPtr.Zero)
            {
                var ctrl = _fwList.FirstOrDefault(fw => new WindowInteropHelper(fw).Handle == currentHandle);
                if (ctrl != null && ctrl.Model.Root.Manager == this)
                {
                    yield return ctrl;
                }

                currentHandle = Win32Helper.GetWindow(currentHandle, (uint)Win32Helper.GetWindowCmd.GW_HWNDNEXT);
            }
        }

        internal void RemoveFloatingWindow(LayoutFloatingWindowControl floatingWindow)
        {
            _fwList.Remove(floatingWindow);
        }

        bool IOverlayWindowHost.HitTest(Point dragPoint)
        {
            var detectionRect = new Rect(this.PointToScreenDPIWithoutFlowDirection(new Point()), this.TransformActualSizeToAncestor());
            return detectionRect.Contains(dragPoint);
        }

        private void CreateOverlayWindow()
        {
            if (_overlayWindow == null)
            {
                _overlayWindow = new OverlayWindow(this);
            }
            var rectWindow = new Rect(this.PointToScreenDPIWithoutFlowDirection(new Point()), this.TransformActualSizeToAncestor());
            _overlayWindow.Left = rectWindow.Left;
            _overlayWindow.Top = rectWindow.Top;
            _overlayWindow.Width = rectWindow.Width;
            _overlayWindow.Height = rectWindow.Height;
        }

        private void DestroyOverlayWindow()
        {
            if (_overlayWindow != null)
            {
                _overlayWindow.Close();
                _overlayWindow = null;
            }
        }

        IOverlayWindow IOverlayWindowHost.ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow)
        {
            CreateOverlayWindow();
            _overlayWindow.Owner = draggingWindow;
            _overlayWindow.EnableDropTargets();
            _overlayWindow.Show();
            return _overlayWindow;
        }

        void IOverlayWindowHost.HideOverlayWindow()
        {
            _areas = null;
            _overlayWindow.Owner = null;
            _overlayWindow.HideDropTargets();
        }

        IEnumerable<IDropArea> IOverlayWindowHost.GetDropAreas(LayoutFloatingWindowControl draggingWindow)
        {
            if (_areas != null)
            {
                return _areas;
            }

            bool isDraggingDocuments = draggingWindow.Model is LayoutDocumentFloatingWindow;

            _areas = new List<IDropArea>();

            if (!isDraggingDocuments)
            {
                _areas.Add(new DropArea<DockingManager>(
                    this,
                    DropAreaType.DockingManager));

                foreach (var areaHost in this.FindVisualChildren<LayoutAnchorablePaneControl>())
                {
                    if (areaHost.Model.Descendents().Any())
                    {
                        _areas.Add(new DropArea<LayoutAnchorablePaneControl>(
                            areaHost,
                            DropAreaType.AnchorablePane));
                    }
                }
            }

            foreach (var areaHost in this.FindVisualChildren<LayoutDocumentPaneControl>())
            {
                _areas.Add(new DropArea<LayoutDocumentPaneControl>(
                    areaHost,
                    DropAreaType.DocumentPane));
            }

            foreach (var areaHost in this.FindVisualChildren<LayoutDocumentPaneGroupControl>())
            {
                var documentGroupModel = areaHost.Model as LayoutDocumentPaneGroup;
                if (documentGroupModel.Children.Where(c => c.IsVisible).Count() == 0)
                {
                    _areas.Add(new DropArea<LayoutDocumentPaneGroupControl>(
                        areaHost,
                        DropAreaType.DocumentPaneGroup));
                }
            }

            return _areas;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _areas = null;
            return base.ArrangeOverride(arrangeBounds);
        }

        private static void OnLayoutItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutItemTemplateChanged(e);
        }

        protected virtual void OnLayoutItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnLayoutItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutItemTemplateSelectorChanged(e);
        }

        protected virtual void OnLayoutItemTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnDocumentsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentsSourceChanged(e);
        }

        protected virtual void OnDocumentsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            DetachDocumentsSource(Layout, e.OldValue as IEnumerable);
            AttachDocumentsSource(Layout, e.NewValue as IEnumerable);
        }

        private void AttachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
        {
            if (documentsSource == null)
            {
                return;
            }

            if (layout == null)
            {
                return;
            }

            var documentsImported = layout.Descendents().OfType<LayoutDocument>().Select(d => d.Content).ToArray();
            var documents = documentsSource;
            var listOfDocumentsToImport = new List<object>(documents.OfType<object>());

            foreach (var document in listOfDocumentsToImport.ToArray())
            {
                if (documentsImported.Contains(document))
                {
                    listOfDocumentsToImport.Remove(document);
                }
            }

            LayoutDocumentPane documentPane = null;
            if (layout.LastFocusedDocument != null)
            {
                documentPane = layout.LastFocusedDocument.Parent as LayoutDocumentPane;
            }

            if (documentPane == null)
            {
                documentPane = layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            }

            _suspendLayoutItemCreation = true;
            foreach (var documentContentToImport in listOfDocumentsToImport)
            {
                var documentToImport = new LayoutDocument
                {
                    Content = documentContentToImport
                };

                bool added = false;
                if (LayoutUpdateStrategy != null)
                {
                    added = LayoutUpdateStrategy.BeforeInsertDocument(layout, documentToImport, documentPane);
                }

                if (!added)
                {
                    if (documentPane == null)
                    {
                        throw new InvalidOperationException("Layout must contains at least one LayoutDocumentPane in order to host documents");
                    }

                    documentPane.Children.Add(documentToImport);
                    added = true;
                }

                if (LayoutUpdateStrategy != null)
                {
                    LayoutUpdateStrategy.AfterInsertDocument(layout, documentToImport);
                }

                CreateDocumentLayoutItem(documentToImport);
            }
            _suspendLayoutItemCreation = true;

            var documentsSourceAsNotifier = documentsSource as INotifyCollectionChanged;
            if (documentsSourceAsNotifier != null)
            {
                documentsSourceAsNotifier.CollectionChanged += documentsSourceElementsChanged;
            }
        }

        private void documentsSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Layout == null)
            {
                return;
            }

            if (SuspendDocumentsSourceBinding)
            {
                return;
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    var documentsToRemove = Layout.Descendents().OfType<LayoutDocument>().Where(d => e.OldItems.Contains(d.Content)).ToArray();
                    foreach (var documentToRemove in documentsToRemove)
                    {
                        documentToRemove.Parent.RemoveChild(
                            documentToRemove);
                    }
                }
            }

            if (e.NewItems != null &&
                (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                 e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                if (e.NewItems != null)
                {
                    LayoutDocumentPane documentPane = null;
                    if (Layout.LastFocusedDocument != null)
                    {
                        documentPane = Layout.LastFocusedDocument.Parent as LayoutDocumentPane;
                    }

                    if (documentPane == null)
                    {
                        documentPane = Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                    }

                    _suspendLayoutItemCreation = true;

                    foreach (var documentContentToImport in e.NewItems)
                    {
                        var documentToImport = new LayoutDocument
                        {
                            Content = documentContentToImport
                        };

                        bool added = false;
                        if (LayoutUpdateStrategy != null)
                        {
                            added = LayoutUpdateStrategy.BeforeInsertDocument(Layout, documentToImport, documentPane);
                        }

                        if (!added)
                        {
                            if (documentPane == null)
                            {
                                throw new InvalidOperationException("Layout must contains at least one LayoutDocumentPane in order to host documents");
                            }

                            documentPane.Children.Add(documentToImport);
                            added = true;
                        }

                        if (LayoutUpdateStrategy != null)
                        {
                            LayoutUpdateStrategy.AfterInsertDocument(Layout, documentToImport);
                        }

                        var root = documentToImport.Root;

                        if (root != null && root.Manager == this)
                        {
                            CreateDocumentLayoutItem(documentToImport);
                        }
                    }
                    _suspendLayoutItemCreation = false;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var documentsToRemove = Layout.Descendents().OfType<LayoutDocument>().ToArray();
                foreach (var documentToRemove in documentsToRemove)
                {
                    documentToRemove.Parent.RemoveChild(
                        documentToRemove);
                }
            }

            if (Layout != null)
            {
                Layout.CollectGarbage();
            }
        }

        private void DetachDocumentsSource(LayoutRoot layout, IEnumerable documentsSource)
        {
            if (documentsSource == null)
            {
                return;
            }

            if (layout == null)
            {
                return;
            }

            var documentsToRemove = layout.Descendents().OfType<LayoutDocument>()
                .Where(d => documentsSource.Contains(d.Content)).ToArray();

            foreach (var documentToRemove in documentsToRemove)
            {
                documentToRemove.Parent.RemoveChild(
                    documentToRemove);
            }

            var documentsSourceAsNotifier = documentsSource as INotifyCollectionChanged;
            if (documentsSourceAsNotifier != null)
            {
                documentsSourceAsNotifier.CollectionChanged -= documentsSourceElementsChanged;
            }
        }

        internal void _ExecuteCloseCommand(LayoutDocument document)
        {
            if (DocumentClosing != null)
            {
                var evargs = new DocumentClosingEventArgs(document);
                DocumentClosing(this, evargs);
                if (evargs.Cancel)
                {
                    return;
                }
            }

            if (!document.TestCanClose())
            {
                return;
            }

            document.Close();

            if (DocumentClosed != null)
            {
                var evargs = new DocumentClosedEventArgs(document);
                DocumentClosed(this, evargs);
            }
        }

        internal void _ExecuteCloseAllButThisCommand(LayoutContent contentSelected)
        {
            foreach (
                var contentToClose in
                    Layout.Descendents().OfType<LayoutContent>().Where(d => d != contentSelected && (d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow)).ToArray())
            {
                if (!contentToClose.CanClose)
                {
                    continue;
                }

                var layoutItem = GetLayoutItemFromModel(contentToClose);
                if (layoutItem.CloseCommand != null)
                {
                    if (layoutItem.CloseCommand.CanExecute(null))
                    {
                        layoutItem.CloseCommand.Execute(null);
                    }
                }
                else
                {
                    if (contentToClose is LayoutDocument)
                    {
                        _ExecuteCloseCommand(contentToClose as LayoutDocument);
                    }
                    else if (contentToClose is LayoutAnchorable)
                    {
                        _ExecuteCloseCommand(contentToClose as LayoutAnchorable);
                    }
                }
            }
        }

        private static void OnAnchorablesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnAnchorablesSourceChanged(e);
        }

        protected virtual void OnAnchorablesSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            DetachAnchorablesSource(Layout, e.OldValue as IEnumerable);
            AttachAnchorablesSource(Layout, e.NewValue as IEnumerable);
        }

        private void AttachAnchorablesSource(LayoutRoot layout, IEnumerable anchorablesSource)
        {
            if (anchorablesSource == null)
            {
                return;
            }

            if (layout == null)
            {
                return;
            }

            var anchorablesImported = layout.Descendents().OfType<LayoutAnchorable>().Select(d => d.Content).ToArray();
            var anchorables = anchorablesSource;
            var listOfAnchorablesToImport = new List<object>(anchorables.OfType<object>());

            foreach (var document in listOfAnchorablesToImport.ToArray())
            {
                if (anchorablesImported.Contains(document))
                {
                    listOfAnchorablesToImport.Remove(document);
                }
            }

            LayoutAnchorablePane anchorablePane = null;
            if (layout.ActiveContent != null)
            {
                anchorablePane = layout.ActiveContent.Parent as LayoutAnchorablePane;
            }

            if (anchorablePane == null)
            {
                anchorablePane = layout.Descendents().OfType<LayoutAnchorablePane>().Where(pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right).FirstOrDefault();
            }

            if (anchorablePane == null)
            {
                anchorablePane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault();
            }

            _suspendLayoutItemCreation = true;
            foreach (var anchorableContentToImport in listOfAnchorablesToImport)
            {
                var anchorableToImport = new LayoutAnchorable
                {
                    Content = anchorableContentToImport
                };

                bool added = false;
                if (LayoutUpdateStrategy != null)
                {
                    added = LayoutUpdateStrategy.BeforeInsertAnchorable(layout, anchorableToImport, anchorablePane);
                }

                if (!added)
                {
                    if (anchorablePane == null)
                    {
                        var mainLayoutPanel = new LayoutPanel
                        {
                            Orientation = Orientation.Horizontal
                        };
                        if (layout.RootPanel != null)
                        {
                            mainLayoutPanel.Children.Add(layout.RootPanel);
                        }

                        layout.RootPanel = mainLayoutPanel;
                        anchorablePane = new LayoutAnchorablePane
                        {
                            DockWidth = new GridLength(200.0, GridUnitType.Pixel)
                        };
                        mainLayoutPanel.Children.Add(anchorablePane);
                    }

                    anchorablePane.Children.Add(anchorableToImport);
                    added = true;
                }

                if (LayoutUpdateStrategy != null)
                {
                    LayoutUpdateStrategy.AfterInsertAnchorable(layout, anchorableToImport);
                }

                CreateAnchorableLayoutItem(anchorableToImport);
            }

            _suspendLayoutItemCreation = false;

            var anchorablesSourceAsNotifier = anchorablesSource as INotifyCollectionChanged;
            if (anchorablesSourceAsNotifier != null)
            {
                anchorablesSourceAsNotifier.CollectionChanged += anchorablesSourceElementsChanged;
            }
        }

        private void anchorablesSourceElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Layout == null)
            {
                return;
            }

            if (SuspendAnchorablesSourceBinding)
            {
                return;
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    var anchorablesToRemove = Layout.Descendents().OfType<LayoutAnchorable>().Where(d => e.OldItems.Contains(d.Content)).ToArray();
                    foreach (var anchorableToRemove in anchorablesToRemove)
                    {
                        anchorableToRemove.Parent.RemoveChild(
                            anchorableToRemove);
                    }
                }
            }

            if (e.NewItems != null &&
                (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                 e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                if (e.NewItems != null)
                {
                    LayoutAnchorablePane anchorablePane = null;

                    if (Layout.ActiveContent != null)
                    {
                        anchorablePane = Layout.ActiveContent.Parent as LayoutAnchorablePane;
                    }

                    if (anchorablePane == null)
                    {
                        anchorablePane = Layout.Descendents().OfType<LayoutAnchorablePane>().Where(pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right).FirstOrDefault();
                    }

                    if (anchorablePane == null)
                    {
                        anchorablePane = Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault();
                    }

                    _suspendLayoutItemCreation = true;
                    foreach (var anchorableContentToImport in e.NewItems)
                    {
                        var anchorableToImport = new LayoutAnchorable
                        {
                            Content = anchorableContentToImport
                        };

                        bool added = false;
                        if (LayoutUpdateStrategy != null)
                        {
                            added = LayoutUpdateStrategy.BeforeInsertAnchorable(Layout, anchorableToImport, anchorablePane);
                        }

                        if (!added)
                        {
                            if (anchorablePane == null)
                            {
                                var mainLayoutPanel = new LayoutPanel
                                {
                                    Orientation = Orientation.Horizontal
                                };
                                if (Layout.RootPanel != null)
                                {
                                    mainLayoutPanel.Children.Add(Layout.RootPanel);
                                }

                                Layout.RootPanel = mainLayoutPanel;
                                anchorablePane = new LayoutAnchorablePane
                                {
                                    DockWidth = new GridLength(200.0, GridUnitType.Pixel)
                                };
                                mainLayoutPanel.Children.Add(anchorablePane);
                            }

                            anchorablePane.Children.Add(anchorableToImport);
                            added = true;
                        }

                        if (LayoutUpdateStrategy != null)
                        {
                            LayoutUpdateStrategy.AfterInsertAnchorable(Layout, anchorableToImport);
                        }

                        var root = anchorableToImport.Root;

                        if (root != null && root.Manager == this)
                        {
                            CreateAnchorableLayoutItem(anchorableToImport);
                        }
                    }
                    _suspendLayoutItemCreation = false;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var anchorablesToRemove = Layout.Descendents().OfType<LayoutAnchorable>().ToArray();
                foreach (var anchorableToRemove in anchorablesToRemove)
                {
                    anchorableToRemove.Parent.RemoveChild(
                        anchorableToRemove);
                }
            }

            if (Layout != null)
            {
                Layout.CollectGarbage();
            }
        }

        private void DetachAnchorablesSource(LayoutRoot layout, IEnumerable anchorablesSource)
        {
            if (anchorablesSource == null)
            {
                return;
            }

            if (layout == null)
            {
                return;
            }

            var anchorablesToRemove = layout.Descendents().OfType<LayoutAnchorable>()
                .Where(d => anchorablesSource.Contains(d.Content)).ToArray();

            foreach (var anchorableToRemove in anchorablesToRemove)
            {
                anchorableToRemove.Parent.RemoveChild(
                    anchorableToRemove);
            }

            var anchorablesSourceAsNotifier = anchorablesSource as INotifyCollectionChanged;
            if (anchorablesSourceAsNotifier != null)
            {
                anchorablesSourceAsNotifier.CollectionChanged -= anchorablesSourceElementsChanged;
            }
        }

        internal void _ExecuteCloseCommand(LayoutAnchorable anchorable)
        {
            var model = anchorable;
            if (model != null && model.TestCanClose())
            {
                if (model.IsAutoHidden)
                {
                    model.ToggleAutoHide();
                }

                model.Close();
            }
        }

        internal void _ExecuteHideCommand(LayoutAnchorable anchorable)
        {
            var model = anchorable;
            if (model != null)
            {
                model.Hide();
            }
        }

        internal void _ExecuteAutoHideCommand(LayoutAnchorable _anchorable)
        {
            _anchorable.ToggleAutoHide();
        }

        internal void _ExecuteFloatCommand(LayoutContent contentToFloat)
        {
            contentToFloat.Float();
        }

        internal void _ExecuteDockCommand(LayoutAnchorable anchorable)
        {
            anchorable.Dock();
        }

        internal void _ExecuteDockAsDocumentCommand(LayoutContent content)
        {
            content.DockAsDocument();
        }

        private static void OnActiveContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).InternalSetActiveContent(e.NewValue);
            ((DockingManager)d).OnActiveContentChanged(e);
        }

        protected virtual void OnActiveContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ActiveContentChanged != null)
            {
                ActiveContentChanged(this, EventArgs.Empty);
            }
        }

        private void InternalSetActiveContent(object contentObject)
        {
            var layoutContent = Layout.Descendents().OfType<LayoutContent>().FirstOrDefault(lc => lc == contentObject || lc.Content == contentObject);
            _insideInternalSetActiveContent = true;
            Layout.ActiveContent = layoutContent;
            _insideInternalSetActiveContent = false;
        }

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnThemeChanged(e);
        }

        protected virtual void OnThemeChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldTheme = e.OldValue as Theme;
            var newTheme = e.NewValue as Theme;
            var resources = Resources;
            if (oldTheme != null)
            {
                var resourceDictionaryToRemove =
                    resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                {
                    resources.MergedDictionaries.Remove(
                        resourceDictionaryToRemove);
                }
            }

            if (newTheme != null)
            {
                resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = newTheme.GetResourceUri()
                });
            }

            foreach (var fwc in _fwList)
            {
                fwc.UpdateThemeResources(oldTheme);
            }

            if (_navigatorWindow != null)
            {
                _navigatorWindow.UpdateThemeResources();
            }

            if (_overlayWindow != null)
            {
                _overlayWindow.UpdateThemeResources();
            }
        }

        internal void _ExecuteContentActivateCommand(LayoutContent content)
        {
            content.IsActive = true;
        }

        private static void OnDocumentPaneMenuItemHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentPaneMenuItemHeaderTemplateChanged(e);
        }

        protected virtual void OnDocumentPaneMenuItemHeaderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceDocumentPaneMenuItemHeaderTemplateValue(DependencyObject d, object value)
        {
            if (value != null &&
                d.GetValue(DocumentPaneMenuItemHeaderTemplateSelectorProperty) != null)
            {
                return null;
            }
            if (value == null)
            {
                return d.GetValue(DocumentHeaderTemplateProperty);
            }

            return value;
        }

        private static void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(e);
        }

        protected virtual void OnDocumentPaneMenuItemHeaderTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null &&
                DocumentPaneMenuItemHeaderTemplate != null)
            {
                DocumentPaneMenuItemHeaderTemplate = null;
            }
        }

        private static object CoerceDocumentPaneMenuItemHeaderTemplateSelectorValue(DependencyObject d, object value)
        {
            return value;
        }

        private void DetachLayoutItems()
        {
            if (Layout != null)
            {
                _layoutItems.ForEach<LayoutItem>(i => i.Detach());
                _layoutItems.Clear();
                Layout.ElementAdded -= Layout_ElementAdded;
                Layout.ElementRemoved -= Layout_ElementRemoved;
            }
        }

        private void Layout_ElementRemoved(object sender, LayoutElementEventArgs e)
        {
            if (_suspendLayoutItemCreation)
            {
                return;
            }

            CollectLayoutItemsDeleted();
        }

        private void Layout_ElementAdded(object sender, LayoutElementEventArgs e)
        {
            if (_suspendLayoutItemCreation)
            {
                return;
            }

            foreach (var content in Layout.Descendents().OfType<LayoutContent>())
            {
                if (content is LayoutDocument)
                {
                    CreateDocumentLayoutItem(content as LayoutDocument);
                }
                else
                {
                    CreateAnchorableLayoutItem(content as LayoutAnchorable);
                }
            }

            CollectLayoutItemsDeleted();
        }

        private void CollectLayoutItemsDeleted()
        {
            if (_collectLayoutItemsOperations != null)
            {
                return;
            }
            _collectLayoutItemsOperations = Dispatcher.BeginInvoke(new Action(() =>
            {
                _collectLayoutItemsOperations = null;
                foreach (var itemToRemove in _layoutItems.Where(item => item.LayoutElement.Root != Layout).ToArray())
                {
                    if (itemToRemove != null &&
                        itemToRemove.Model != null &&
                        itemToRemove.Model is UIElement)
                    {
                    }

                    itemToRemove.Detach();
                    _layoutItems.Remove(itemToRemove);
                }
            }));
        }

        private void AttachLayoutItems()
        {
            if (Layout != null)
            {
                foreach (var document in Layout.Descendents().OfType<LayoutDocument>().ToArray())
                {
                    CreateDocumentLayoutItem(document);
                }
                foreach (var anchorable in Layout.Descendents().OfType<LayoutAnchorable>().ToArray())
                {
                    CreateAnchorableLayoutItem(anchorable);
                }

                Layout.ElementAdded += Layout_ElementAdded;
                Layout.ElementRemoved += Layout_ElementRemoved;
            }
        }

        private void ApplyStyleToLayoutItem(LayoutItem layoutItem)
        {
            layoutItem._ClearDefaultBindings();
            if (LayoutItemContainerStyle != null)
            {
                layoutItem.Style = LayoutItemContainerStyle;
            }
            else if (LayoutItemContainerStyleSelector != null)
            {
                layoutItem.Style = LayoutItemContainerStyleSelector.SelectStyle(layoutItem.Model, layoutItem);
            }
            layoutItem._SetDefaultBindings();
        }

        private void CreateAnchorableLayoutItem(LayoutAnchorable contentToAttach)
        {
            if (_layoutItems.Any(item => item.LayoutElement == contentToAttach))
            {
                return;
            }

            var layoutItem = new LayoutAnchorableItem();
            layoutItem.Attach(contentToAttach);
            ApplyStyleToLayoutItem(layoutItem);
            _layoutItems.Add(layoutItem);

            if (contentToAttach != null &&
                contentToAttach.Content != null &&
                contentToAttach.Content is UIElement)
            {
                InternalAddLogicalChild(contentToAttach.Content);
            }
        }

        private void CreateDocumentLayoutItem(LayoutDocument contentToAttach)
        {
            if (_layoutItems.Any(item => item.LayoutElement == contentToAttach))
            {
                return;
            }

            var layoutItem = new LayoutDocumentItem();
            layoutItem.Attach(contentToAttach);
            ApplyStyleToLayoutItem(layoutItem);
            _layoutItems.Add(layoutItem);

            if (contentToAttach != null &&
                contentToAttach.Content != null &&
                contentToAttach.Content is UIElement)
            {
                InternalAddLogicalChild(contentToAttach.Content);
            }
        }

        private static void OnLayoutItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutItemContainerStyleChanged(e);
        }

        protected virtual void OnLayoutItemContainerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            AttachLayoutItems();
        }

        private static void OnLayoutItemContainerStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DockingManager)d).OnLayoutItemContainerStyleSelectorChanged(e);
        }

        protected virtual void OnLayoutItemContainerStyleSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            AttachLayoutItems();
        }

        public LayoutItem GetLayoutItemFromModel(LayoutContent content)
        {
            return _layoutItems.FirstOrDefault(item => item.LayoutElement == content);
        }

        private void ShowNavigatorWindow()
        {
            if (_navigatorWindow == null)
            {
                _navigatorWindow = new NavigatorWindow(this)
                {
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
            }

            _navigatorWindow.ShowDialog();
            _navigatorWindow = null;

            Trace.WriteLine("ShowNavigatorWindow()");
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            Trace.WriteLine(string.Format("OnPreviewKeyDown({0})", e.Key));
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.IsDown && e.Key == Key.Tab)
                {
                    if (!IsNavigatorWindowActive)
                    {
                        ShowNavigatorWindow();
                        e.Handled = true;
                    }
                }
            }

            base.OnPreviewKeyDown(e);
        }

        #endregion
    }
}