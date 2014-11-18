// ****************************************************************************
//
// Имя файла    : 'NativeMethods.cs'
// Заголовок    : Реализация работы с WinApi-функциями
// Автор        : Крыцкий А.В./Тихомиров В.С.
// Контакты     : kav.it@mail.ru
// Дата         : 17/05/2012
//
// ****************************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SharpLib.Winapi
{
    [SuppressUnmanagedCodeSecurity]
    public sealed partial class NativeMethods
    {
        #region Делегаты

        public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Константы

        private const string DLLNAME_KERNEL32 = "kernel32.dll";

        private const string DLLNAME_MAPI32 = "mapi32.dll";

        private const string DLLNAME_NTDLL = "ntdll.dll";

        private const string DLLNAME_SETUPAPI = "setupapi.dll";

        private const string DLLNAME_SHELL32 = "shell32.dll";

        private const string DLLNAME_USER32 = "user32.dll";

        private const string DLLNAME_GDI32 = "gdi32.dll";

        #endregion

        #region Поля

        public static readonly IntPtr InvalidHandle = new IntPtr(-1L);

        public static readonly int SpinCount = Environment.ProcessorCount != 1 ? 4000 : 0;

        public static readonly Boolean SpinEnabled = Environment.ProcessorCount != 1;

        #endregion

        #region Методы

        [SecurityCritical]
        public static void SetSystemMenuItems(HandleRef hwnd, bool isEnabled, params SystemMenu[] menus)
        {
            if (menus != null && menus.Length > 0)
            {
                HandleRef hMenu = new HandleRef(null, GetSystemMenu(hwnd, false));

                foreach (SystemMenu menu in menus)
                {
                    SetMenuItem(hMenu, menu, isEnabled);
                }
            }
        }

        [SecurityCritical]
        public static void SetMenuItem(HandleRef hMenu, SystemMenu menu, bool isEnabled)
        {
            EnableMenuItem(hMenu, menu, (isEnabled) ? ~1 : 1);
        }

        /// <summary>
        /// Преобразование кода последней ошибки в строку
        /// </summary>
        public static String LastErrorToString(int err)
        {
            StringBuilder strLastErrorMessage = new StringBuilder(255);
            int ret2 = err;
            int dwFlags = 4096;

            FormatMessage(dwFlags,
                null,
                ret2,
                0,
                strLastErrorMessage,
                strLastErrorMessage.Capacity,
                null);

            return strLastErrorMessage.ToString();
        }

        /// <summary>
        /// Генерация исключения, соответсвующего коду GetLastError
        /// </summary>
        private static void RaiseSystemException()
        {
            int err = GetLastError();
            String message = "(#" + err + ") " + LastErrorToString(err);

            switch (err)
            {
                case ERROR_INVALID_HANDLE:
                    throw new IOException(message);
                case ERROR_FILE_NOT_FOUND:
                    throw new FileNotFoundException(message);
                case ERROR_ACCESS_DENIED:
                    throw new UnauthorizedAccessException(message);
                default:
                    throw new Exception(message);
            }
        }

        /// <summary>
        /// Handle check for INVALID_HANDLE
        /// </summary>
        public static void Check(UInt32 result)
        {
            if (result == 0)
            {
                RaiseSystemException();
            }
        }

        /// <summary>
        /// Handle check for INVALID_HANDLE
        /// </summary>
        public static void Check(IntPtr handle)
        {
            if (handle == InvalidHandle)
            {
                RaiseSystemException();
            }
        }

        /// <summary>
        /// Чтение кода последней ошибки
        /// </summary>
        /// <returns></returns>
        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }

        /// <summary>
        /// Выделение памяти (используется в диалогах)
        /// </summary>
        /// <returns></returns>
        [SecurityCritical]
        public static IMalloc GetShMalloc()
        {
            IMalloc[] ppMalloc = new IMalloc[1];
            ShellGetMalloc(ppMalloc);
            return ppMalloc[0];
        }

        /// <summary>
        /// Преобразование IntPtr в int
        /// </summary>
        /// <param name="intPtr"></param>
        /// <returns></returns>
        public static int IntPtrToInt32(IntPtr intPtr)
        {
            return (int)intPtr.ToInt64();
        }

        /// <summary>
        /// Позиционирование окна
        /// </summary>
        public static void MoveWindow(IntPtr hWnd, RECT rect, bool bRepaint)
        {
            MoveWindow(hWnd, rect.Left, rect.Top, rect.Width, rect.Height, bRepaint);
        }

        /// <summary>
        /// Центрирование окна относительно родительского
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="hwndParent"></param>
        public static void CenterTo(IntPtr hwnd, IntPtr hwndParent)
        {
            if (hwndParent == IntPtr.Zero)
            {
                // Центрирование относительно рабочего стола
                hwndParent = GetDesktopWindow();
            }

            RECT rcWindow;
            GetWindowRect(hwnd, out rcWindow);

            RECT rcParent;
            GetWindowRect(hwndParent, out rcParent);

            int cx = (rcParent.Width - rcWindow.Width) / 2;
            int cy = (rcParent.Height - rcWindow.Height) / 2;

            // rcWindow.Center = rcParent.Center;

            rcWindow = new RECT(rcParent.Left + cx, rcParent.Top + cy, rcWindow.Width, rcWindow.Height);

            MoveWindow(hwnd, rcWindow, true);
        }

        [SecurityCritical]
        public static int GetWindowLong(HandleRef hWnd, WindowLongValue nIndex)
        {
            int result;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                result = IntGetWindowLong(hWnd, (int)nIndex);
                Marshal.GetLastWin32Error();
            }
            else
            {
                IntPtr resultPtr = IntGetWindowLongPtr(hWnd, (int)nIndex);
                Marshal.GetLastWin32Error();
                result = IntPtrToInt32(resultPtr);
            }
            return result;
        }

        [SecurityCritical]
        public static IntPtr SetWindowLong(HandleRef hWnd, WindowLongValue nIndex, IntPtr dwNewLong)
        {
            IntPtr result;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                int intResult = IntSetWindowLong(hWnd, (int)nIndex, IntPtrToInt32(dwNewLong));
                Marshal.GetLastWin32Error();
                result = new IntPtr(intResult);
            }
            else
            {
                result = IntSetWindowLongPtr(hWnd, (int)nIndex, dwNewLong);
                Marshal.GetLastWin32Error();
            }
            return result;
        }

        #endregion
    }
}