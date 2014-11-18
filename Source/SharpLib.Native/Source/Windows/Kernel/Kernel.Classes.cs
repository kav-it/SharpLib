using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.Native.Windows
{
    [SuppressUnmanagedCodeSecurity]
    public partial class NativeMethods
    {
        #region Вложенный класс: MemoryStatusEx

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MemoryStatusEx
        {
            #region Поля

            public uint Length;

            public uint MemoryLoad;

            public ulong TotalPhys;

            public ulong AvailPhys;

            public ulong TotalPageFile;

            public ulong AvailPageFile;

            public ulong TotalVirtual;

            public ulong AvailVirtual;

            public ulong AvailExtendedVirtual;

            #endregion Поля

            #region Конструктор

            public MemoryStatusEx()
            {
                Length = (uint)Marshal.SizeOf(typeof(MemoryStatusEx));
            }

            #endregion Конструктор
        }

        #endregion

        #region Вложенный класс: OS_VERSION_INFO_EX

        [StructLayout(LayoutKind.Sequential)]
        public struct OS_VERSION_INFO_EX
        {
            public int OsVersionInfoSize;

            public int MajorVersion;

            public int MinorVersion;

            public int BuildNumber;

            public int PlatformId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String CsdVersion;

            public short ServicePackMajor;

            public short ServicePackMinor;

            public short SuiteMask;

            public Byte ProductTyp;

            public Byte Reserved;
        }

        #endregion
    }
}