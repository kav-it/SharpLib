using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.Native.Windows
{
    [SuppressUnmanagedCodeSecurity]
    public partial class NativeMethods
    {
        #region Константы

        private const string DLLNAME_SHELL32 = "shell32.dll";

        #endregion

        #region Методы

        [SecurityCritical]
        [DllImport(DLLNAME_SHELL32, EntryPoint = "SHGetMalloc")]
        public static extern int ShellGetMalloc([Out] [MarshalAs(UnmanagedType.LPArray)] IMalloc[] ppMalloc);

        [SecurityCritical]
        [DllImport(DLLNAME_SHELL32, EntryPoint = "SHGetFolderLocation")]
        public static extern int ShellGetFolderLocation(IntPtr hwndOwner, Int32 nFolder, IntPtr hToken, uint dwReserved, out IntPtr ppidl);

        [SecurityCritical]
        [DllImport(DLLNAME_SHELL32, EntryPoint = "SHParseDisplayName")]
        public static extern int ShellParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string pszName, IntPtr pbc, out IntPtr ppidl, uint sfgaoIn, out uint psfgaoOut);

        [SecurityCritical]
        [DllImport(DLLNAME_SHELL32, EntryPoint = "SHBrowseForFolder")]
        public static extern IntPtr ShellBrowseForFolder(ref BrowseInfo lbpi);

        [SecurityCritical]
        [DllImport(DLLNAME_SHELL32, CharSet = CharSet.Auto, EntryPoint = "SHGetPathFromIDList")]
        public static extern bool ShellGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

        [SecurityCritical]
        [DllImport(DLLNAME_SHELL32, CharSet = CharSet.Auto, EntryPoint = "Shell_NotifyIcon")]
        public static extern int ShellNotifyIcon(int message, NotifyIconData pnid);

        [DllImport(DLLNAME_SHELL32)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);


        #endregion
    }
}