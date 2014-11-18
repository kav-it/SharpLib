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
        #region Вложенный класс: BrowseInfo

        [StructLayout(LayoutKind.Sequential)]
        public struct BrowseInfo
        {
            /// <summary>
            /// Handle to the owner window for the dialog box.
            /// </summary>
            internal IntPtr HwndOwner;

            /// <summary>
            /// Pointer to an item identifier list (PIDL) specifying the
            /// location of the root folder from which to start browsing.
            /// </summary>
            internal IntPtr Root;

            /// <summary>
            /// Address of a buffer to receive the display name of the
            /// folder selected by the user.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string DisplayName;

            /// <summary>
            /// Address of a null-terminated string that is displayed
            /// above the tree view control in the dialog box.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string Title;

            /// <summary>
            /// Flags specifying the options for the dialog box.
            /// </summary>
            public uint Flags;

            /// <summary>
            /// Address of an application-defined function that the
            /// dialog box calls when an event occurs.
            /// </summary>
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc Callback;

            /// <summary>
            /// Application-defined value that the dialog box passes to
            /// the callback function
            /// </summary>
            public int LParam;

            /// <summary>
            /// Variable to receive the image associated with the selected folder.
            /// </summary>
            public int Image;
        }

        #endregion

        #region Вложенный класс: Exception

        public class Exception : SystemException
        {
            #region Конструктор

            public Exception()
            {
            }

            public Exception(String text)
                : base(text)
            {
            }

            public Exception(String text, Exception innerException)
                : base(text, innerException)
            {
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: IMalloc

        [ComImport]
        [Guid("00000002-0000-0000-c000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMalloc
        {
            [PreserveSig]
            IntPtr Alloc(int cb);

            [PreserveSig]
            IntPtr Realloc(IntPtr pv, int cb);

            [PreserveSig]
            void Free(IntPtr pv);

            [PreserveSig]
            int GetSize(IntPtr pv);

            [PreserveSig]
            int DidAlloc(IntPtr pv);

            [PreserveSig]
            void HeapMinimize();
        }

        #endregion

        #region Вложенный класс: MapiFileDesc

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MapiFileDesc
        {
            public int Reserved;

            public int Flags;

            public int Position;

            public String Path;

            public String Name;

            internal IntPtr Typ;
        }

        #endregion

        #region Вложенный класс: MapiMessage

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MapiMessage
        {
            public int Reserved;

            public String Subject;

            public String NoteText;

            public String MessageTyp;

            public String DateReceived;

            public String ConversationID;

            public int Flags;

            internal IntPtr Originator;

            public int RecipCount;

            internal IntPtr Recips;

            public int FileCount;

            internal IntPtr Files;
        }

        #endregion

        #region Вложенный класс: MapiRecipDesc

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct MapiRecipDesc
        {
            #region Поля

            public int Reserved;

            public int RecipClass;

            public String Name;

            public String Address;

            public int IdSize;

            internal IntPtr EntryID;

            #endregion Поля
        }

        #endregion

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

        #region Вложенный класс: NotifyIconData

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NotifyIconData
        {
            public int CbSize = Marshal.SizeOf(typeof(NotifyIconData));

            internal IntPtr HandleWnd;

            public int ID;

            public NotifyIconFlags Flags;

            public int CallbackMessage;

            internal IntPtr HandleIcon;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
            public String Tip;

            public int State;

            public int StateMask;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
            public String Info;

            public int TimeoutOrVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
            public String InfoTitle;

            public int InfoFlags;
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

        #region Вложенный класс: POINT

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;

            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override String ToString()
            {
                String text = String.Format("X: {0}, Y: {1}", X, Y);

                return text;
            }
        }

        #endregion

        #region Вложенный класс: RECT

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;

            public int Top;

            public int Right;

            public int Bottom;

            public int Height
            {
                get { return Bottom - Top; }
            }

            public int Width
            {
                get { return Right - Left; }
            }

            public POINT Center
            {
                get
                {
                    POINT p = new POINT();

                    p.X = Left + (int)Math.Round((double)Width / 2);
                    p.Y = Top + (int)Math.Round((double)Height / 2);

                    return p;
                }
                set
                {
                    POINT p = new POINT(value.X, value.Y);

                    Left = p.X - (int)Math.Round((double)Width / 2);
                    Top = p.Y - (int)Math.Round((double)Height / 2);
                }
            }

            public RECT(int left, int top, int width, int height)
            {
                Left = left;
                Top = top;
                Right = left + width;
                Bottom = top + height;
            }

            public override String ToString()
            {
                String text = String.Format("X: {0}, Y: {1}, Width: {2}, Height: {3}", Left, Top, Width, Height);

                return text;
            }
        }

        #endregion

        #region Вложенный класс: ResourceHeader

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ResourceHeader
        {
            /// <summary>
            /// Header length.
            /// </summary>
            public UInt16 Length;

            /// <summary>
            /// Data length.
            /// </summary>
            public UInt16 ValueLength;

            /// <summary>
            /// Resource type.
            /// </summary>
            public UInt16 Typ;

            /// <summary>
            /// A new resource header of a given length.
            /// </summary>
            /// <param name="valueLength"></param>
            public ResourceHeader(UInt16 valueLength)
            {
                Length = 0;
                ValueLength = valueLength;
                Typ = 0;
            }
        }

        #endregion

        #region Вложенный класс: ResourceWinVer

        public abstract class ResourceWinVer
        {
            #region Перечисления

            public enum FileFlags : uint
            {
                /// <summary>
                /// The file contains debugging information.
                /// </summary>
                VS_FF_DEBUG = 0x00000001,

                /// <summary>
                /// The file is a prerelease development version, not a final commercial release.
                /// </summary>
                VS_FF_PRERELEASE = 0x00000002,

                /// <summary>
                /// PThe file has been modified somehow and is not identical to the original file
                /// that shipped with the product.
                /// </summary>
                VS_FF_PATCHED = 0x00000004,

                /// <summary>
                /// The file was not built using standard release procedures. There should be data
                /// in the file's "PrivateBuild" version information String.
                /// </summary>
                VS_FF_PRIVATEBUILD = 0x00000008,

                /// <summary>
                /// The version information in this structure was not found inside the file,
                /// but instead was created when needed based on the best information available.
                /// Therefore, this structure's information may differ slightly from what the "real"
                /// values are.
                /// </summary>
                VS_FF_INFOINFERRED = 0x00000010,

                /// <summary>
                /// The file was built using standard release procedures, but is somehow different
                /// from the normal file having the same version number. There should be data in the
                /// file's "SpecialBuild" version information String.
                /// </summary>
                VS_FF_SPECIALBUILD = 0x00000020,
            }

            public enum FileOs : uint
            {
                /// <summary>
                /// The operating system under which the file was designed to run could not be determined.
                /// </summary>
                VOS_UNKNOWN = 0x00000000,

                /// <summary>
                /// The file was designed to run under MS-DOS.
                /// </summary>
                VOS_DOS = 0x00010000,

                /// <summary>
                /// The file was designed to run under a 16-bit version of OS/2.
                /// </summary>
                VOS_OS216 = 0x00020000,

                /// <summary>
                /// The file was designed to run under a 32-bit version of OS/2.
                /// </summary>
                VOS_OS232 = 0x00030000,

                /// <summary>
                /// The file was designed to run under Windows NT/2000.
                /// </summary>
                VOS_NT = 0x00040000,

                /// <summary>
                /// </summary>
                VOS_WINCE = 0x00050000,

                /// <summary>
                /// The file was designed to run under the 16-bit Windows API.
                /// </summary>
                VOS_WINDOWS16 = 0x00000001,

                /// <summary>
                /// The file was designed to be run under a 16-bit version of Presentation Manager.
                /// </summary>
                VOS_PM16 = 0x00000002,

                /// <summary>
                /// The file was designed to be run under a 32-bit version of Presentation Manager.
                /// </summary>
                VOS_PM32 = 0x00000003,

                /// <summary>
                /// The file was designed to run under the 32-bit Windows API.
                /// </summary>
                VOS_WINDOWS32 = 0x00000004,

                /// <summary>
                /// </summary>
                VOS_DOS_WINDOWS16 = 0x00010001,

                /// <summary>
                /// </summary>
                VOS_DOS_WINDOWS32 = 0x00010004,

                /// <summary>
                /// </summary>
                VOS_OS216_PM16 = 0x00020002,

                /// <summary>
                /// </summary>
                VOS_OS232_PM32 = 0x00030003,

                /// <summary>
                /// </summary>
                VOS_NT_WINDOWS32 = 0x00040004
            }

            public enum FileSubType : uint
            {
                /// <summary>
                /// The type of driver could not be determined.
                /// </summary>
                VFT2_UNKNOWN = 0x00000000,

                /// <summary>
                /// The file is a printer driver.
                /// </summary>
                VFT2_DRV_PRINTER = 0x00000001,

                /// <summary>
                /// The file is a keyboard driver.
                /// </summary>
                VFT2_DRV_KEYBOARD = 0x00000002,

                /// <summary>
                /// The file is a language driver.
                /// </summary>
                VFT2_DRV_LANGUAGE = 0x00000003,

                /// <summary>
                /// The file is a display driver.
                /// </summary>
                VFT2_DRV_DISPLAY = 0x00000004,

                /// <summary>
                /// The file is a mouse driver.
                /// </summary>
                VFT2_DRV_MOUSE = 0x00000005,

                /// <summary>
                /// The file is a network driver.
                /// </summary>
                VFT2_DRV_NETWORK = 0x00000006,

                /// <summary>
                /// The file is a system driver.
                /// </summary>
                VFT2_DRV_SYSTEM = 0x00000007,

                /// <summary>
                /// The file is an installable driver.
                /// </summary>
                VFT2_DRV_INSTALLABLE = 0x00000008,

                /// <summary>
                /// The file is a sound driver.
                /// </summary>
                VFT2_DRV_SOUND = 0x00000009,

                /// <summary>
                /// The file is a communications driver.
                /// </summary>
                VFT2_DRV_COMM = 0x0000000A,

                /// <summary>
                /// The file is an input method driver.
                /// </summary>
                VFT2_DRV_INPUTMETHOD = 0x0000000B,

                /// <summary>
                /// The file is a versioned printer driver.
                /// </summary>
                VFT2_DRV_VERSIONED_PRINTER = 0x0000000C,

                /// <summary>
                /// The file is a raster font.
                /// </summary>
                VFT2_FONT_RASTER = 0x00000001,

                /// <summary>
                /// The file is a vector font.
                /// </summary>
                VFT2_FONT_VECTOR = 0x00000002,

                /// <summary>
                /// The file is a TrueType font.
                /// </summary>
                VFT2_FONT_TRUETYPE = 0x00000003,
            }

            public enum FileType : uint
            {
                /// <summary>
                /// The type of file could not be determined.
                /// </summary>
                VFT_UNKNOWN = 0x00000000,

                /// <summary>
                /// The file is an application.
                /// </summary>
                VFT_APP = 0x00000001,

                /// <summary>
                /// The file is a Dynamic Link Library (DLL).
                /// </summary>
                VFT_DLL = 0x00000002,

                /// <summary>
                /// The file is a device driver. dwFileSubtype contains more information.
                /// </summary>
                VFT_DRV = 0x00000003,

                /// <summary>
                /// The file is a font. dwFileSubtype contains more information.
                /// </summary>
                VFT_FONT = 0x00000004,

                /// <summary>
                /// The file is a virtual device.
                /// </summary>
                VFT_VXD = 0x00000005,

                /// <summary>
                /// The file is a static link library.
                /// </summary>
                VFT_STATIC_LIB = 0x00000007
            }

            #endregion

            #region Константы

            public const UInt32 VS_FFI_FILEFLAGSMASK = 0x0000003F;

            public const UInt32 VS_FFI_SIGNATURE = 0xFEEF04BD;

            public const UInt32 VS_FFI_STRUCVERSION = 0x00010000;

            #endregion
        }

        #endregion

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

        #region Вложенный класс: VarHeader

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct VarHeader
        {
            /// <summary>
            /// Microsoft language identifier.
            /// </summary>
            public UInt16 LanguageIdms;

            /// <summary>
            /// IBM code page number.
            /// </summary>
            public UInt16 CodePageIbm;
        }

        #endregion

        #region Вложенный класс: VsFixedFileInfo

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct VsFixedFileInfo
        {
            /// <summary>
            /// Contains the value 0xFEEF04BD. This is used with the szKey member of the VS_VERSIONINFO structure when searching a file
            /// for the VS_FIXEDFILEINFO structure.
            /// </summary>
            public UInt32 Signature;

            /// <summary>
            /// Specifies the binary version number of this structure. The high-order word of this member contains the major version
            /// number, and the low-order word contains the minor version number.
            /// </summary>
            public UInt32 StrucVersion;

            /// <summary>
            /// Specifies the most significant 32 bits of the file's binary version number. This member is used with dwFileVersionLS to
            /// form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 FileVersionMs;

            /// <summary>
            /// Specifies the least significant 32 bits of the file's binary version number. This member is used with dwFileVersionMS
            /// to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 FileVersionLs;

            /// <summary>
            /// Specifies the most significant 32 bits of the binary version number of the product with which this file was
            /// distributed. This member is used with dwProductVersionLS to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 ProductVersionMs;

            /// <summary>
            /// Specifies the least significant 32 bits of the binary version number of the product with which this file was
            /// distributed. This member is used with dwProductVersionMS to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 ProductVersionLs;

            /// <summary>
            /// Contains a bitmask that specifies the valid bits in dwFileFlags. A bit is valid only if it was defined when the file
            /// was created.
            /// </summary>
            public UInt32 FileFlagsMask;

            /// <summary>
            /// Contains a bitmask that specifies the Boolean attributes of the file.
            /// </summary>
            public UInt32 FileFlags;

            /// <summary>
            /// Specifies the operating system for which this file was designed.
            /// </summary>
            public UInt32 FileOs;

            /// <summary>
            /// Specifies the general type of file.
            /// </summary>
            public UInt32 FileTyp;

            /// <summary>
            /// Specifies the function of the file.
            /// </summary>
            public UInt32 FileSubtyp;

            /// <summary>
            /// Specifies the most significant 32 bits of the file's 64-bit binary creation date and time stamp.
            /// </summary>
            public UInt32 FileDateMs;

            /// <summary>
            /// Specifies the least significant 32 bits of the file's 64-bit binary creation date and time stamp.
            /// </summary>
            public UInt32 FileDateLs;

            /// <summary>
            /// Creates a default Windows VS_FIXEDFILEINFO structure.
            /// </summary>
            /// <returns>A default Windows VS_FIXEDFILEINFO.</returns>
            public static VsFixedFileInfo GetWindowsDefault()
            {
                VsFixedFileInfo fixedFileInfo = new VsFixedFileInfo();
                fixedFileInfo.Signature = ResourceWinVer.VS_FFI_SIGNATURE;
                fixedFileInfo.StrucVersion = ResourceWinVer.VS_FFI_STRUCVERSION;
                fixedFileInfo.FileFlagsMask = ResourceWinVer.VS_FFI_FILEFLAGSMASK;
                fixedFileInfo.FileOs = (uint)ResourceWinVer.FileOs.VOS_WINDOWS32;
                fixedFileInfo.FileSubtyp = (uint)ResourceWinVer.FileSubType.VFT2_UNKNOWN;
                fixedFileInfo.FileTyp = (uint)ResourceWinVer.FileType.VFT_DLL;
                return fixedFileInfo;
            }
        }

        #endregion
    }
}