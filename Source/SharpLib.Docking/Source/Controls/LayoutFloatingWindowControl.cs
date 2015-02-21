using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using SharpLib.Docking.Layout;
using SharpLib.Docking.Themes;

namespace SharpLib.Docking.Controls
{
    public abstract class LayoutFloatingWindowControl : Window, ILayoutControl
    {
        #region Поля

        public static readonly DependencyProperty IsDraggingProperty;

        public static readonly DependencyProperty IsMaximizedProperty;

        private static readonly DependencyPropertyKey _isDraggingPropertyKey;

        private static readonly DependencyPropertyKey _isMaximizedPropertyKey;

        private readonly ILayoutElement _model;

        private bool _attachDrag;

        private DragService _dragService;

        private HwndSource _hwndSrc;

        private HwndSourceHook _hwndSrcHook;

        private bool _internalCloseFlag;

        #endregion

        #region Свойства

        public abstract ILayoutElement Model { get; }

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
        }

        protected bool CloseInitiatedByUser
        {
            get { return !_internalCloseFlag; }
        }

        internal bool KeepContentVisibleOnClose { get; set; }

        public bool IsMaximized
        {
            get { return (bool)GetValue(IsMaximizedProperty); }
        }

        #endregion

        #region Конструктор

        static LayoutFloatingWindowControl()
        {
            _isDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(false, OnIsDraggingChanged));
            _isMaximizedPropertyKey = DependencyProperty.RegisterReadOnly("IsMaximized", typeof(bool), typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(false));
            IsDraggingProperty = _isDraggingPropertyKey.DependencyProperty;
            IsMaximizedProperty = _isMaximizedPropertyKey.DependencyProperty;

            ContentProperty.OverrideMetadata(typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(null, null, CoerceContentValue));
            AllowsTransparencyProperty.OverrideMetadata(typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(false));
            ShowInTaskbarProperty.OverrideMetadata(typeof(LayoutFloatingWindowControl), new FrameworkPropertyMetadata(false));
        }

        protected LayoutFloatingWindowControl(ILayoutElement model)
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            _model = model;
            UpdateThemeResources();
        }

        #endregion

        #region Методы

        private static object CoerceContentValue(DependencyObject sender, object content)
        {
            return new FloatingWindowContentHost(sender as LayoutFloatingWindowControl)
            {
                Content = content as UIElement
            };
        }

        internal virtual void UpdateThemeResources(Theme oldTheme = null)
        {
            if (oldTheme != null)
            {
                var resourceDictionaryToRemove =
                    Resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                {
                    Resources.MergedDictionaries.Remove(
                        resourceDictionaryToRemove);
                }
            }

            var manager = _model.Root.Manager;
            if (manager.Theme != null)
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = manager.Theme.GetResourceUri()
                });
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (Content != null)
            {
                var host = Content as FloatingWindowContentHost;
                host.Dispose();

                if (_hwndSrc != null)
                {
                    _hwndSrc.RemoveHook(_hwndSrcHook);
                    _hwndSrc.Dispose();
                    _hwndSrc = null;
                }
            }

            base.OnClosed(e);
        }

        internal void AttachDrag(bool onActivated = true)
        {
            if (onActivated)
            {
                _attachDrag = true;
                Activated += OnActivated;
            }
            else
            {
                var windowHandle = new WindowInteropHelper(this).Handle;
                var lParam = new IntPtr(((int)Left & 0xFFFF) | (((int)Top) << 16));
                Win32Helper.SendMessage(windowHandle, Win32Helper.WM_NCLBUTTONDOWN, new IntPtr(Win32Helper.HT_CAPTION), lParam);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            this.SetParentToMainWindowOf(Model.Root.Manager);

            _hwndSrc = PresentationSource.FromDependencyObject(this) as HwndSource;
            _hwndSrcHook = FilterMessage;
            _hwndSrc.AddHook(_hwndSrcHook);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;

            if (_hwndSrc != null)
            {
                _hwndSrc.RemoveHook(_hwndSrcHook);
                _hwndSrc.Dispose();
                _hwndSrc = null;
            }
        }

        private void OnActivated(object sender, EventArgs e)
        {
            Activated -= OnActivated;

            if (_attachDrag && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                var windowHandle = new WindowInteropHelper(this).Handle;
                var mousePosition = this.PointToScreenDPI(Mouse.GetPosition(this));
                var clientArea = Win32Helper.GetClientRect(windowHandle);
                var windowArea = Win32Helper.GetWindowRect(windowHandle);

                Left = mousePosition.X - windowArea.Width / 2.0;
                Top = mousePosition.Y - (windowArea.Height - clientArea.Height) / 2.0;
                _attachDrag = false;

                var lParam = new IntPtr(((int)mousePosition.X & 0xFFFF) | (((int)mousePosition.Y) << 16));
                Win32Helper.SendMessage(windowHandle, Win32Helper.WM_NCLBUTTONDOWN, new IntPtr(Win32Helper.HT_CAPTION), lParam);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.CloseWindowCommand,
                (s, args) => Microsoft.Windows.Shell.SystemCommands.CloseWindow((Window)args.Parameter)));
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MaximizeWindowCommand,
                (s, args) => Microsoft.Windows.Shell.SystemCommands.MaximizeWindow((Window)args.Parameter)));
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MinimizeWindowCommand,
                (s, args) => Microsoft.Windows.Shell.SystemCommands.MinimizeWindow((Window)args.Parameter)));
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.RestoreWindowCommand,
                (s, args) => Microsoft.Windows.Shell.SystemCommands.RestoreWindow((Window)args.Parameter)));

            base.OnInitialized(e);
        }

        protected void SetIsDragging(bool value)
        {
            SetValue(_isDraggingPropertyKey, value);
        }

        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutFloatingWindowControl)d).OnIsDraggingChanged(e);
        }

        protected virtual void OnIsDraggingChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void UpdatePositionAndSizeOfPanes()
        {
            foreach (var posElement in Model.Descendents().OfType<ILayoutElementForFloatingWindow>())
            {
                posElement.FloatingLeft = Left;
                posElement.FloatingTop = Top;
                posElement.FloatingWidth = Width;
                posElement.FloatingHeight = Height;
            }
        }

        private void UpdateMaximizedState(bool isMaximized)
        {
            foreach (var posElement in Model.Descendents().OfType<ILayoutElementForFloatingWindow>())
            {
                posElement.IsMaximized = isMaximized;
            }
        }

        protected virtual IntPtr FilterMessage(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled
            )
        {
            handled = false;

            switch (msg)
            {
                case Win32Helper.WM_ACTIVATE:
                    if (((int)wParam & 0xFFFF) == Win32Helper.WA_INACTIVE)
                    {
                        if (lParam == this.GetParentWindowHandle())
                        {
                            Win32Helper.SetActiveWindow(_hwndSrc.Handle);
                            handled = true;
                        }
                    }
                    break;
                case Win32Helper.WM_EXITSIZEMOVE:
                    UpdatePositionAndSizeOfPanes();

                    if (_dragService != null)
                    {
                        bool dropFlag;
                        var mousePosition = this.TransformToDeviceDPI(Win32Helper.GetMousePosition());
                        _dragService.Drop(mousePosition, out dropFlag);
                        _dragService = null;
                        SetIsDragging(false);

                        if (dropFlag)
                        {
                            InternalClose();
                        }
                    }

                    break;
                case Win32Helper.WM_MOVING:
                    {
                        UpdateDragPosition();
                    }
                    break;
                case Win32Helper.WM_LBUTTONUP:
                    if (_dragService != null && Mouse.LeftButton == MouseButtonState.Released)
                    {
                        _dragService.Abort();
                        _dragService = null;
                        SetIsDragging(false);
                    }
                    break;
                case Win32Helper.WM_SYSCOMMAND:
                    var wMaximize = new IntPtr(Win32Helper.SC_MAXIMIZE);
                    var wRestore = new IntPtr(Win32Helper.SC_RESTORE);
                    if (wParam == wMaximize || wParam == wRestore)
                    {
                        UpdateMaximizedState(wParam == wMaximize);
                    }
                    break;
            }

            return IntPtr.Zero;
        }

        private void UpdateDragPosition()
        {
            if (_dragService == null)
            {
                _dragService = new DragService(this);
                SetIsDragging(true);
            }

            var mousePosition = this.TransformToDeviceDPI(Win32Helper.GetMousePosition());
            _dragService.UpdateMouseLocation(mousePosition);
        }

        internal void InternalClose()
        {
            _internalCloseFlag = true;
            Close();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }

        protected void SetIsMaximized(bool value)
        {
            SetValue(_isMaximizedPropertyKey, value);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            SetIsMaximized(WindowState == System.Windows.WindowState.Maximized);
            base.OnStateChanged(e);
        }

        #endregion

        #region Вложенный класс: FloatingWindowContentHost

        protected class FloatingWindowContentHost : HwndHost
        {
            #region Поля

            public static readonly DependencyProperty ContentProperty =
                DependencyProperty.Register("Content", typeof(UIElement), typeof(FloatingWindowContentHost),
                    new FrameworkPropertyMetadata(null,
                        OnContentChanged));

            private readonly LayoutFloatingWindowControl _owner;

            private DockingManager _manager;

            private Border _rootPresenter;

            private HwndSource _wpfContentHost;

            #endregion

            #region Свойства

            public Visual RootVisual
            {
                get { return _rootPresenter; }
            }

            public UIElement Content
            {
                get { return (UIElement)GetValue(ContentProperty); }
                set { SetValue(ContentProperty, value); }
            }

            #endregion

            #region Конструктор

            public FloatingWindowContentHost(LayoutFloatingWindowControl owner)
            {
                _owner = owner;
                var manager = _owner.Model.Root.Manager;
            }

            #endregion

            #region Методы

            protected override System.Runtime.InteropServices.HandleRef BuildWindowCore(System.Runtime.InteropServices.HandleRef hwndParent)
            {
                _wpfContentHost = new HwndSource(new HwndSourceParameters
                {
                    ParentWindow = hwndParent.Handle,
                    WindowStyle = Win32Helper.WS_CHILD | Win32Helper.WS_VISIBLE | Win32Helper.WS_CLIPSIBLINGS | Win32Helper.WS_CLIPCHILDREN,
                    Width = 1,
                    Height = 1
                });

                _rootPresenter = new Border
                {
                    Child = new AdornerDecorator
                    {
                        Child = Content
                    },
                    Focusable = true
                };
                _rootPresenter.SetBinding(Border.BackgroundProperty, new Binding("Background")
                {
                    Source = _owner
                });
                _wpfContentHost.RootVisual = _rootPresenter;
                _wpfContentHost.SizeToContent = SizeToContent.Manual;
                _manager = _owner.Model.Root.Manager;
                _manager.InternalAddLogicalChild(_rootPresenter);

                return new HandleRef(this, _wpfContentHost.Handle);
            }

            protected override void DestroyWindowCore(HandleRef hwnd)
            {
                _manager.InternalRemoveLogicalChild(_rootPresenter);
                if (_wpfContentHost != null)
                {
                    _wpfContentHost.Dispose();
                    _wpfContentHost = null;
                }
            }

            protected override Size MeasureOverride(Size constraint)
            {
                if (Content == null)
                {
                    return base.MeasureOverride(constraint);
                }

                Content.Measure(constraint);
                return Content.DesiredSize;
            }

            private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                ((FloatingWindowContentHost)d).OnContentChanged(e);
            }

            protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
            {
                if (_rootPresenter != null)
                {
                    _rootPresenter.Child = Content;
                }
            }

            #endregion
        }

        #endregion
    }
}