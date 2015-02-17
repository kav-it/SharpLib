using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SharpLib.Wpf.Dialogs
{
    internal static class DialogCustomHelper
    {
        #region Константы

        private const int GWL_EXSTYLE = -20;

        private const int GWL_STYLE = -16;

        private const int SWP_FRAMECHANGED = 0x0020;

        private const int SWP_NOMOVE = 0x0002;

        private const int SWP_NOSIZE = 0x0001;

        private const int SWP_NOZORDER = 0x0004;

        private const uint WM_SETICON = 0x0080;

        private const int WS_EX_DLGMODALFRAME = 0x0001;

        // from winuser.h

        private const int WS_MAXIMIZEBOX = 0x10000;

        private const int WS_MINIMIZEBOX = 0x20000;

        #endregion

        #region Методы

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd,
            IntPtr hwndInsertAfter,
            int x,
            int y,
            int width,
            int height,
            uint flags);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hwnd,
            uint msg,
            IntPtr wParam,
            IntPtr lParam);

        internal static void RemoveIcon(Window window)
        {
            // Чтение указателя окна
            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            // Чтения стиля окна
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

            // Смена стиля окна
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        internal static void HideMinimizeAndMaximizeButtons(Window window)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

            SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX));
        }

        #endregion
    }
}