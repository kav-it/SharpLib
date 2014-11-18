using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.Winapi
{
    [SuppressUnmanagedCodeSecurity]
    public sealed partial class NativeMethods
    {
        #region Методы

        [DllImport(DLLNAME_GDI32, EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        #endregion
    }
}