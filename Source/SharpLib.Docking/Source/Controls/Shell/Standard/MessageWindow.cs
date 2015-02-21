using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace Standard
{
    internal sealed class MessageWindow : DispatcherObject, IDisposable
    {
        #region Поля

        private static readonly Dictionary<IntPtr, MessageWindow> _sWindowLookup;

        private static readonly WndProc _sWndProc;

        private readonly WndProc _wndProcCallback;

        private string _className;

        private bool _isDisposed;

        #endregion

        #region Свойства

        public IntPtr Handle { get; private set; }

        #endregion

        #region Конструктор

        static MessageWindow()
        {
            _sWindowLookup = new Dictionary<IntPtr, MessageWindow>();
            _sWndProc = _WndProc;
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public MessageWindow(CS classStyle, WS style, WS_EX exStyle, Rect location, string name, WndProc callback)
        {
            _wndProcCallback = callback;
            _className = "MessageWindowClass+" + Guid.NewGuid();

            var wc = new WNDCLASSEX
            {
                cbSize = Marshal.SizeOf(typeof(WNDCLASSEX)),
                style = classStyle,
                lpfnWndProc = _sWndProc,
                hInstance = NativeMethods.GetModuleHandle(null),
                hbrBackground = NativeMethods.GetStockObject(StockObject.NULL_BRUSH),
                lpszMenuName = "",
                lpszClassName = _className,
            };

            NativeMethods.RegisterClassEx(ref wc);

            var gcHandle = default(GCHandle);
            try
            {
                gcHandle = GCHandle.Alloc(this);
                var pinnedThisPtr = (IntPtr)gcHandle;

                Handle = NativeMethods.CreateWindowEx(
                    exStyle,
                    _className,
                    name,
                    style,
                    (int)location.X,
                    (int)location.Y,
                    (int)location.Width,
                    (int)location.Height,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    pinnedThisPtr);
            }
            finally
            {
                gcHandle.Free();
            }
        }

        ~MessageWindow()
        {
            _Dispose(false, false);
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            _Dispose(true, false);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing")]
        private void _Dispose(bool disposing, bool isHwndBeingDestroyed)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            var hwnd = Handle;
            string className = _className;

            if (isHwndBeingDestroyed)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)(arg => _DestroyWindow(IntPtr.Zero, className)));
            }
            else if (Handle != IntPtr.Zero)
            {
                if (CheckAccess())
                {
                    _DestroyWindow(hwnd, className);
                }
                else
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (DispatcherOperationCallback)(arg => _DestroyWindow(hwnd, className)));
                }
            }

            _sWindowLookup.Remove(hwnd);

            _className = null;
            Handle = IntPtr.Zero;
        }

        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
        private static IntPtr _WndProc(IntPtr hwnd, WM msg, IntPtr wParam, IntPtr lParam)
        {
            var ret = IntPtr.Zero;
            MessageWindow hwndWrapper;

            if (msg == WM.CREATE)
            {
                var createStruct = (CREATESTRUCT)Marshal.PtrToStructure(lParam, typeof(CREATESTRUCT));
                var gcHandle = GCHandle.FromIntPtr(createStruct.lpCreateParams);
                hwndWrapper = (MessageWindow)gcHandle.Target;
                _sWindowLookup.Add(hwnd, hwndWrapper);
            }
            else
            {
                if (!_sWindowLookup.TryGetValue(hwnd, out hwndWrapper))
                {
                    return NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);
                }
            }
            Assert.IsNotNull(hwndWrapper);

            var callback = hwndWrapper._wndProcCallback;
            ret = callback != null 
                ? callback(hwnd, msg, wParam, lParam) 
                : NativeMethods.DefWindowProc(hwnd, msg, wParam, lParam);

            if (msg == WM.NCDESTROY)
            {
                hwndWrapper._Dispose(true, true);
                GC.SuppressFinalize(hwndWrapper);
            }

            return ret;
        }

        private static object _DestroyWindow(IntPtr hwnd, string className)
        {
            Utility.SafeDestroyWindow(ref hwnd);
            NativeMethods.UnregisterClass(className, NativeMethods.GetModuleHandle(null));
            return null;
        }

        #endregion
    }
}