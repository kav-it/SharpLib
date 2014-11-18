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
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.Winapi
{
    [SuppressUnmanagedCodeSecurity]
    public sealed partial class NativeMethods
    {
        #region Методы

        [DllImport(DLLNAME_NTDLL)]
        public static extern int NtCreateKeyedEvent(
            [Out] out IntPtr keyedEventHandle,
            [In] int desiredAccess,
            [In] [Optional] IntPtr objectAttributes,
            [In] int flags
            );

        [DllImport(DLLNAME_NTDLL)]
        public static extern int NtReleaseKeyedEvent(
            [In] IntPtr keyedEventHandle,
            [In] IntPtr keyValue,
            [In] Boolean alertable,
            [In] [Optional] IntPtr timeout
            );

        [DllImport(DLLNAME_NTDLL)]
        public static extern int NtWaitForKeyedEvent(
            [In] IntPtr keyedEventHandle,
            [In] IntPtr keyValue,
            [In] Boolean alertable,
            [In] [Optional] IntPtr timeout
            );

        #endregion
    }
}