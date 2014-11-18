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
using System.Text;
using System.Threading;

namespace SharpLib.Winapi
{
    [SuppressUnmanagedCodeSecurity]
    public sealed unsafe partial class NativeMethods
    {
        #region Методы

        [SecurityCritical]
        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern void SetLastError(int dwErrorCode);

        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFile(
            String lpFileName,
            UInt32 dwDesiredAccess,
            UInt32 dwShareMode,
            IntPtr lpSecurityAttributes,
            UInt32 dwCreationDisposition,
            UInt32 dwFlagsAndAttributes,
            NativeOverlapped* hTemplateFile);

        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern UInt32 ReadFileEx(
            IntPtr hFile,
            Byte* lpBuffer,
            int nNumberOfBytesToRead,
            NativeOverlapped* lpOverlapped,
            [MarshalAs(UnmanagedType.FunctionPtr)] IOCompletionCallback callback
            );

        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern UInt32 WriteFileEx(
            IntPtr hFile,
            Byte* lpBuffer,
            int nNumberOfBytesToWrite,
            NativeOverlapped* lpOverlapped,
            [MarshalAs(UnmanagedType.FunctionPtr)] IOCompletionCallback callback
            );

        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.None, SetLastError = true)]
        public static extern UInt32 ReadFile(
            IntPtr hFile,
            Byte* lpBuffer,
            UInt32 nNumberOfBytesToRead,
            UInt32* lpNumberOfBytesRead,
            NativeOverlapped* lpOverlapped);

        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Auto)]
        public static extern int FormatMessage(int dwFlags,
            String lpSource,
            int dwMessageId,
            int dwLanguageId,
            StringBuilder lpBuffer,
            int nSize,
            String[] args);

        [DllImport(DLLNAME_KERNEL32)]
        public static extern UInt32 WriteFile(
            IntPtr hFile,
            Byte* lpBuffer,
            UInt32 nNumberOfBytesToWrite,
            UInt32* lpNumberOfBytesWritten,
            NativeOverlapped* lpOverlapped);

        [DllImport(DLLNAME_KERNEL32)]
        public static extern Boolean CloseHandle(IntPtr hObject);

        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateEvent(
            [In] [Optional] IntPtr eventAttributes,
            [In] Boolean manualReset,
            [In] Boolean initialState,
            [In] [Optional] String name
            );

        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateSemaphore(
            [In] [Optional] IntPtr semaphoreAttributes,
            [In] int initialCount,
            [In] int maximumCount,
            [In] [Optional] String name
            );

        [DllImport(DLLNAME_KERNEL32)]
        public static extern Boolean ReleaseSemaphore(
            [In] IntPtr semaphoreHandle,
            [In] int releaseCount,
            [In] IntPtr previousCount
            );

        [DllImport(DLLNAME_KERNEL32)]
        public static extern Boolean ResetEvent([In] IntPtr eventHandle);

        [DllImport(DLLNAME_KERNEL32)]
        public static extern Boolean SetEvent([In] IntPtr eventHandle);

        [DllImport(DLLNAME_KERNEL32)]
        public static extern int WaitForSingleObject([In] IntPtr handle, [In] int milliseconds);

        [DllImport(DLLNAME_KERNEL32)]
        public static extern UInt32 GetOverlappedResult(
            IntPtr hFile,
            NativeOverlapped* lpOverlapped,
            UInt32* lpNumberOfBytesTransferred,
            UInt32 bWait
            );

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GlobalMemoryStatusEx([In] [Out] MemoryStatusEx lpBuffer);

        [DllImport(DLLNAME_KERNEL32)]
        public static extern bool GetVersionEx(ref OS_VERSION_INFO_EX osVersionInfo);

        [DllImport(DLLNAME_KERNEL32)]
        public static extern bool GetProductInfo(int osMajorVersion, int osMinorVersion, int spMajorVersion, int spMinorVersion, out int edition);

        [DllImport(DLLNAME_KERNEL32)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(String moduleName);

        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] String procName);

        [DllImport(DLLNAME_KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        [DllImport(DLLNAME_KERNEL32, SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResData);

        [DllImport(DLLNAME_KERNEL32, SetLastError = true)]
        public static extern int SizeofResource(IntPtr hInstance, IntPtr hResInfo);

        [DllImport(DLLNAME_KERNEL32, EntryPoint = "FindResourceExW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, UInt16 wLanguage);

        [DllImport(DLLNAME_KERNEL32, SetLastError = true)]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport(DLLNAME_KERNEL32, SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport(DLLNAME_KERNEL32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadLibraryW")]
        public static extern IntPtr LoadLibrary([In] [MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        [DllImport(DLLNAME_KERNEL32, SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport(DLLNAME_KERNEL32, EntryPoint = "BeginUpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        [DllImport(DLLNAME_KERNEL32, EntryPoint = "UpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, IntPtr lpName, UInt16 wLanguage, Byte[] lpData, UInt32 cbData);

        [DllImport(DLLNAME_KERNEL32, EntryPoint = "EndUpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        [DllImport(DLLNAME_KERNEL32, EntryPoint = "AcquireSRWLockExclusive")]
        public static extern void AcquireSrwLockExclusive(ref IntPtr srw);

        [DllImport(DLLNAME_KERNEL32, EntryPoint = "AcquireSRWLockShared")]
        public static extern void AcquireSrwLockShared(ref IntPtr srw);

        [DllImport(DLLNAME_KERNEL32, EntryPoint = "InitializeSRWLock")]
        public static extern void InitializeSrwLock(out IntPtr srw);

        [DllImport(DLLNAME_KERNEL32, EntryPoint = "ReleaseSRWLockExclusive")]
        public static extern void ReleaseSrwLockExclusive(ref IntPtr srw);

        [DllImport(DLLNAME_KERNEL32, EntryPoint = "ReleaseSRWLockShared")]
        public static extern void ReleaseSrwLockShared(ref IntPtr srw);


        #endregion
    }
}