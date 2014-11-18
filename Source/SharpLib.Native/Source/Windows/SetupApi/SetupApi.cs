using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.Native.Windows
{
    [SuppressUnmanagedCodeSecurity]
    public partial class NativeMethods
    {
        #region Константы

        private const string DLLNAME_SETUP_API = "setupapi.dll";

        #endregion

        #region Методы

        [DllImport(DLLNAME_SETUP_API, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(
            ref Guid gClass,
            [MarshalAs(UnmanagedType.LPStr)] string strEnumerator,
            IntPtr hParent,
            uint nFlags);

        [DllImport(DLLNAME_SETUP_API, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(
            IntPtr lpDeviceInfoSet,
            int index,
            ref SpDeviInfoData deviceInfoData);

        [DllImport(DLLNAME_SETUP_API, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(
            IntPtr lpDeviceInfoSet,
            ref SpDeviInfoData deviceInfoData,
            ref Guid gClass,
            uint nIndex,
            ref SpDeviceInterfaceData interfaceData);

        [DllImport(DLLNAME_SETUP_API, SetLastError = true)]
        public static extern int SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);

        [DllImport(DLLNAME_SETUP_API, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr lpDeviceInfoSet,
            ref SpDeviceInterfaceData interfaceData,
            ref SpDeviceInterfaceDetailDataWithPath detailData,
            uint nDeviceInterfaceDetailDataSize,
            IntPtr nRequiredSize,
            IntPtr lpDeviceInfoData);

        #endregion
    }
}