using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.Native.Windows
{
    [SuppressUnmanagedCodeSecurity]
    public partial class NativeMethods
    {
        #region Константы

        private const string DLLNAME_GDI32 = "gdi32.dll";

        #endregion

        #region Методы

        [DllImport(DLLNAME_GDI32, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr objectHandle);

        [DllImport(DLLNAME_GDI32, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr deviceContext, IntPtr objectHandle);

        [DllImport(DLLNAME_GDI32, SetLastError = true)]
        public static extern bool SetPixelFormat(IntPtr deviceContext, int pixelFormat, ref PIXELFORMATDESCRIPTOR pixelFormatDescriptor);

        [DllImport(DLLNAME_GDI32, SetLastError = true)]
        public static extern int ChoosePixelFormat(IntPtr deviceContext, ref PIXELFORMATDESCRIPTOR pixelFormatDescriptor);

        [DllImport(DLLNAME_GDI32, SetLastError = true)]
        public static extern bool SwapBuffers(IntPtr deviceContext);

        [DllImport(DLLNAME_GDI32, CallingConvention = CallingConvention.StdCall, EntryPoint = "SwapBuffers")]
        public static extern int SwapBuffersFast([In] IntPtr deviceContext);

        #endregion
    }
}