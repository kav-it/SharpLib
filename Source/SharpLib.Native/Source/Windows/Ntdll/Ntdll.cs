using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.Native.Windows
{
    [SuppressUnmanagedCodeSecurity]
    public partial class NativeMethods
    {
        #region Константы

        private const string DLLNAME_NTDLL = "ntdll.dll";

        #endregion

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