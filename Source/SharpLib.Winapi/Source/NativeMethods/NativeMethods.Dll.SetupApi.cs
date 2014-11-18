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

        [DllImport(DLLNAME_SETUPAPI, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(
            ref Guid gClass, 
            [MarshalAs(UnmanagedType.LPStr)] string strEnumerator, 
            IntPtr hParent, 
            uint nFlags);

        [DllImport(DLLNAME_SETUPAPI, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(
            IntPtr lpDeviceInfoSet, 
            int index, 
            ref SpDeviInfoData deviceInfoData);

        [DllImport(DLLNAME_SETUPAPI, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(
            IntPtr lpDeviceInfoSet, 
            ref SpDeviInfoData deviceInfoData, 
            ref Guid gClass, uint nIndex, 
            ref SpDeviceInterfaceData interfaceData);

        [DllImport(DLLNAME_SETUPAPI, SetLastError = true)]
        public static extern int SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);

        [DllImport(DLLNAME_SETUPAPI, SetLastError = true)]
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