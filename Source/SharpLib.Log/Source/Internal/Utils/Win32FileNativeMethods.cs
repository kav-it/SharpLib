using System;
using System.Runtime.InteropServices;

namespace SharpLib.Log
{
    internal static class Win32FileNativeMethods
    {
        #region Перечисления

        public enum CreationDisposition : uint
        {
            New = 1,

            CreateAlways = 2,

            OpenExisting = 3,

            OpenAlways = 4,

            TruncateExisting = 5,
        }

        [Flags]
        public enum FileAccess : uint
        {
            GenericRead = 0x80000000,

            GenericWrite = 0x40000000,

            GenericExecute = 0x20000000,

            GenericAll = 0x10000000,
        }

        #endregion

        #region Константы

        public const int FILE_SHARE_DELETE = 4;

        public const int FILE_SHARE_READ = 1;

        public const int FILE_SHARE_WRITE = 2;

        #endregion

        #region Методы

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFile(
            string lpFileName,
            FileAccess dwDesiredAccess,
            int dwShareMode,
            IntPtr lpSecurityAttributes,
            CreationDisposition dwCreationDisposition,
            Win32FileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetFileInformationByHandle(IntPtr hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        #endregion

        #region Вложенный класс: BY_HANDLE_FILE_INFORMATION

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BY_HANDLE_FILE_INFORMATION
        {
            public uint dwFileAttributes;

            public long ftCreationTime;

            public long ftLastAccessTime;

            public long ftLastWriteTime;

            public uint dwVolumeSerialNumber;

            public uint nFileSizeHigh;

            public uint nFileSizeLow;

            public uint nNumberOfLinks;

            public uint nFileIndexHigh;

            public uint nFileIndexLow;
        }

        #endregion
    }
}