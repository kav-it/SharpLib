using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    internal static class FocusElementManager
    {
        #region Поля

        private static readonly List<DockingManager> _managers;

        private static readonly FullWeakDictionary<ILayoutElement, IInputElement> _modelFocusedElement;

        private static readonly WeakDictionary<ILayoutElement, IntPtr> _modelFocusedWindowHandle;

        private static WeakReference _lastFocusedElement;

        private static WeakReference _lastFocusedElementBeforeEnterMenuMode;

        private static DispatcherOperation _setFocusAsyncOperation;

        private static WindowHookHandler _windowHandler;

        #endregion

        #region Конструктор

        static FocusElementManager()
        {
            _managers = new List<DockingManager>();
            _modelFocusedElement = new FullWeakDictionary<ILayoutElement, IInputElement>();
            _modelFocusedWindowHandle = new WeakDictionary<ILayoutElement, IntPtr>();
        }

        #endregion

        #region Методы

        internal static void SetupFocusManagement(DockingManager manager)
        {
            if (_managers.Count == 0)
            {
                _windowHandler = new WindowHookHandler();
                _windowHandler.FocusChanged += WindowFocusChanging;

                _windowHandler.Attach();

                if (Application.Current != null)
                {
                    Application.Current.Exit += Current_Exit;
                }
            }

            manager.PreviewGotKeyboardFocus += manager_PreviewGotKeyboardFocus;
            _managers.Add(manager);
        }

        internal static void FinalizeFocusManagement(DockingManager manager)
        {
            manager.PreviewGotKeyboardFocus -= manager_PreviewGotKeyboardFocus;
            _managers.Remove(manager);

            if (_managers.Count == 0)
            {
                if (_windowHandler != null)
                {
                    _windowHandler.FocusChanged -= WindowFocusChanging;

                    _windowHandler.Detach();
                    _windowHandler = null;
                }
            }
        }

        private static void Current_Exit(object sender, ExitEventArgs e)
        {
            Application.Current.Exit -= Current_Exit;
            if (_windowHandler != null)
            {
                _windowHandler.FocusChanged -= WindowFocusChanging;

                _windowHandler.Detach();
                _windowHandler = null;
            }
        }

        private static void manager_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var focusedElement = e.NewFocus as Visual;
            if (focusedElement != null &&
                !(focusedElement is LayoutAnchorableTabItem || focusedElement is LayoutDocumentTabItem))
            {
                var parentAnchorable = focusedElement.FindVisualAncestor<LayoutAnchorableControl>();
                if (parentAnchorable != null)
                {
                    _modelFocusedElement[parentAnchorable.Model] = e.NewFocus;
                }
                else
                {
                    var parentDocument = focusedElement.FindVisualAncestor<LayoutDocumentControl>();
                    if (parentDocument != null)
                    {
                        _modelFocusedElement[parentDocument.Model] = e.NewFocus;
                    }
                }
            }
        }

        internal static IInputElement GetLastFocusedElement(ILayoutElement model)
        {
            IInputElement objectWithFocus;
            if (_modelFocusedElement.GetValue(model, out objectWithFocus))
            {
                return objectWithFocus;
            }

            return null;
        }

        internal static IntPtr GetLastWindowHandle(ILayoutElement model)
        {
            IntPtr handleWithFocus;
            if (_modelFocusedWindowHandle.GetValue(model, out handleWithFocus))
            {
                return handleWithFocus;
            }

            return IntPtr.Zero;
        }

        internal static void SetFocusOnLastElement(ILayoutElement model)
        {
            bool focused = false;
            IInputElement objectToFocus;
            if (_modelFocusedElement.GetValue(model, out objectToFocus))
            {
                focused = objectToFocus == Keyboard.Focus(objectToFocus);
            }

            IntPtr handleToFocus;
            if (_modelFocusedWindowHandle.GetValue(model, out handleToFocus))
            {
                focused = IntPtr.Zero != Win32Helper.SetFocus(handleToFocus);
            }

            if (focused)
            {
                _lastFocusedElement = new WeakReference(model);
            }
        }

        private static void WindowFocusChanging(object sender, FocusChangeEventArgs e)
        {
            foreach (var manager in _managers)
            {
                var hostContainingFocusedHandle = manager.FindLogicalChildren<HwndHost>().FirstOrDefault(hw => Win32Helper.IsChild(hw.Handle, e.GotFocusWinHandle));

                if (hostContainingFocusedHandle != null)
                {
                    var parentAnchorable = hostContainingFocusedHandle.FindVisualAncestor<LayoutAnchorableControl>();
                    if (parentAnchorable != null)
                    {
                        _modelFocusedWindowHandle[parentAnchorable.Model] = e.GotFocusWinHandle;
                        if (parentAnchorable.Model != null)
                        {
                            parentAnchorable.Model.IsActive = true;
                        }
                    }
                    else
                    {
                        var parentDocument = hostContainingFocusedHandle.FindVisualAncestor<LayoutDocumentControl>();
                        if (parentDocument != null)
                        {
                            _modelFocusedWindowHandle[parentDocument.Model] = e.GotFocusWinHandle;
                            if (parentDocument.Model != null)
                            {
                                parentDocument.Model.IsActive = true;
                            }
                        }
                    }
                }
            }
        }

        private static void WindowActivating(object sender, WindowActivateEventArgs e)
        {
            if (Keyboard.FocusedElement == null && _lastFocusedElement != null && _lastFocusedElement.IsAlive)
            {
                var elementToSetFocus = _lastFocusedElement.Target as ILayoutElement;
                if (elementToSetFocus != null)
                {
                    var manager = elementToSetFocus.Root.Manager;
                    if (manager == null)
                    {
                        return;
                    }

                    IntPtr parentHwnd;
                    if (!manager.GetParentWindowHandle(out parentHwnd))
                    {
                        return;
                    }

                    if (e.HwndActivating != parentHwnd)
                    {
                        return;
                    }

                    _setFocusAsyncOperation = Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            SetFocusOnLastElement(elementToSetFocus);
                        }
                        finally
                        {
                            _setFocusAsyncOperation = null;
                        }
                    }), DispatcherPriority.Background);
                }
            }
        }

        private static void InputManager_EnterMenuMode(object sender, EventArgs e)
        {
            if (Keyboard.FocusedElement == null)
            {
                return;
            }

            var lastfocusDepObj = Keyboard.FocusedElement as DependencyObject;
            if (lastfocusDepObj.FindLogicalAncestor<DockingManager>() == null)
            {
                _lastFocusedElementBeforeEnterMenuMode = null;
                return;
            }

            _lastFocusedElementBeforeEnterMenuMode = new WeakReference(Keyboard.FocusedElement);
        }

        #endregion
    }
}