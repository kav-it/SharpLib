using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SharpLib.Native.Windows
{
    [SuppressUnmanagedCodeSecurity]
    public partial class NativeMethods
    {
        private const string DLLNAME_USER32 = "user32.dll";

        #region Методы

        [SecurityCritical]
        [DllImport(DLLNAME_USER32, CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool EnableMenuItem(HandleRef handleMenu, SystemMenu idEnabledItem, int enable);

        [SecurityCritical]
        [DllImport(DLLNAME_USER32, CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetSystemMenu(HandleRef hWnd, bool bRevert);

        [SecurityCritical]
        [DllImport(DLLNAME_USER32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport(DLLNAME_USER32, EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetCursorPos(int x, int y);

        [SecurityCritical]
        [DllImport(DLLNAME_USER32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);

        [DllImport(DLLNAME_USER32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

        [SecurityCritical]
        [DllImport(DLLNAME_USER32, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(HandleRef hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [SecurityCritical]
        [DllImport(DLLNAME_USER32, CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetForegroundWindow(HandleRef hWnd);

        [SecurityCritical]
        [DllImport(DLLNAME_USER32, CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string msg);

        [SecurityCritical]
        [DllImport(DLLNAME_USER32, CharSet = CharSet.Auto)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport(DLLNAME_USER32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadStringW")]
        public static extern int LoadString([In] [Optional] IntPtr hInstance, [In] uint uId, [Out] StringBuilder lpBuffer, [In] int nBufferMax);

        [SecurityCritical]
        [DllImport(DLLNAME_USER32, EntryPoint = "GetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int IntGetWindowLong(HandleRef hWnd, int nIndex);

        [SecurityCritical]
        [DllImport(DLLNAME_USER32, EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr IntGetWindowLongPtr(HandleRef hWnd, int nIndex);

        [DllImport(DLLNAME_USER32, EntryPoint = "SetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int IntSetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport(DLLNAME_USER32, EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr IntSetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport(DLLNAME_USER32)]
        public static extern bool GetKeyboardState(Byte[] lpKeyState);

        [DllImport(DLLNAME_USER32)]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        [DllImport(DLLNAME_USER32, SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(DLLNAME_USER32, SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(DLLNAME_USER32, SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

        [DllImport(DLLNAME_USER32, SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(DLLNAME_USER32, SetLastError = true)]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport(DLLNAME_USER32)]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            Byte[] lpKeyState,
            [Out] [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)] StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport(DLLNAME_USER32)]
        public static extern IntPtr GetActiveWindow();

        [DllImport(DLLNAME_USER32, CallingConvention = CallingConvention.StdCall, SetLastError = true), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr GetDC(IntPtr windowHandle);

        [DllImport(DLLNAME_USER32, CallingConvention = CallingConvention.StdCall, SetLastError = true), SuppressUnmanagedCodeSecurity]
        public static extern bool ReleaseDC(IntPtr windowHandle, IntPtr deviceContext);

        #endregion
    }
}