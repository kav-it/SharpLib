using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.Native.Windows
{
    [SuppressUnmanagedCodeSecurity]
    public partial class NativeMethods
    {
        #region Константы

        private const string DLLNAME_MAPI32 = "mapi32.dll";

        #endregion

        #region Методы

        [DllImport(DLLNAME_MAPI32, SetLastError = true)]
        public static extern int MapiSendMail(IntPtr sess, IntPtr hwnd, MapiMessage message, int flg, int rsv);

        #endregion
    }
}