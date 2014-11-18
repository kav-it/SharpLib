using System;
using System.Runtime.InteropServices;

namespace SharpLib.Native.Windows
{
    public partial class NativeMethods
    {
        #region Вложенный класс: SpDeviInfoData

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SpDeviInfoData
        {
            public int Size;

            public Guid ClassGuid;

            public int DevInst;

            private readonly IntPtr _reserved;
        }

        #endregion

        #region Вложенный класс: SpDeviceInterfaceData

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SpDeviceInterfaceData
        {
            public int Size;

            public Guid InterfaceClassGuid;

            public int Flags;

            internal IntPtr Reserved;
        }

        #endregion

        #region Вложенный класс: SpDeviceInterfaceDetailData

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SpDeviceInterfaceDetailData
        {
            public int Size;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string DevicePath;
        }

        #endregion

        #region Вложенный класс: SpDeviceInterfaceDetailDataWithPath

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SpDeviceInterfaceDetailDataWithPath
        {
            public int Size;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        #endregion
    }
}