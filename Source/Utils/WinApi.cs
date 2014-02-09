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
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace SharpLib
{

    #region Класс NativeMethods

    [SuppressUnmanagedCodeSecurity]
    public sealed unsafe class NativeMethods
    {
        #region Делегаты

        public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Типы

        #region Перечисления

        [Flags]
        public enum ClassDevsFlags
        {
            DIGCF_DEFAULT = 0x00000001,

            DIGCF_PRESENT = 0x00000002,

            DIGCF_ALLCLASSES = 0x00000004,

            DIGCF_PROFILE = 0x00000008,

            DIGCF_DEVICEINTERFACE = 0x00000010,
        }

        [Flags]
        public enum FolderBrowserOptions
        {
            /// <summary>
            /// None.
            /// </summary>
            None = 0,

            /// <summary>
            /// For finding a folder to start document searching
            /// </summary>
            FolderOnly = 0x0001,

            /// <summary>
            /// For starting the Find Computer
            /// </summary>
            FindComputer = 0x0002,

            /// <summary>
            /// Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and 
            /// one line if this flag is set.  Passing the message 
            /// BFFM_SETSTATUSTEXTA to the hwnd can set the rest of the text.  
            /// This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets
            /// all three lines of text.
            /// </summary>
            ShowStatusText = 0x0004,

            ReturnAncestors = 0x0008,

            /// <summary>
            /// Add an editbox to the dialog
            /// </summary>
            ShowEditBox = 0x0010,

            /// <summary>
            /// insist on valid result (or CANCEL)
            /// </summary>
            ValidateResult = 0x0020,

            /// <summary>
            /// Use the new dialog layout with the ability to resize
            /// Caller needs to call OleInitialize() before using this API
            /// </summary>
            UseNewStyle = 0x0040,

            UseNewStyleWithEditBox = (UseNewStyle | ShowEditBox),

            /// <summary>
            /// Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
            /// </summary>
            AllowUrls = 0x0080,

            /// <summary>
            /// Add a UA hint to the dialog, in place of the edit box. May not be
            /// combined with BIF_EDITBOX.
            /// </summary>
            ShowUsageHint = 0x0100,

            /// <summary>
            /// Do not add the "New Folder" button to the dialog.  Only applicable 
            /// with BIF_NEWDIALOGSTYLE.
            /// </summary>
            HideNewFolderButton = 0x0200,

            /// <summary>
            /// don't traverse target as shortcut
            /// </summary>
            GetShortcuts = 0x0400,

            /// <summary>
            /// Browsing for Computers.
            /// </summary>
            BrowseComputers = 0x1000,

            /// <summary>
            /// Browsing for Printers.
            /// </summary>
            BrowsePrinters = 0x2000,

            /// <summary>
            /// Browsing for Everything
            /// </summary>
            BrowseFiles = 0x4000,

            /// <summary>
            /// sharable resources displayed (remote shares, requires BIF_USENEWUI)
            /// </summary>
            BrowseShares = 0x8000
        }

        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,

            MAPVK_VSC_TO_VK = 0x1,

            MAPVK_VK_TO_CHAR = 0x2,

            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [Flags]
        public enum NotifyIconFlags
        {
            /// <summary>
            /// The hIcon member is valid.
            /// </summary>
            Icon = 2,

            /// <summary>
            /// The uCallbackMessage member is valid.
            /// </summary>
            Message = 1,

            /// <summary>
            /// The szTip member is valid.
            /// </summary>
            ToolTip = 4,

            /// <summary>
            /// The dwState and dwStateMask members are valid.
            /// </summary>
            State = 8,

            /// <summary>
            /// Use a balloon ToolTip instead of a standard ToolTip. The szInfo, uTimeout, szInfoTitle, and dwInfoFlags members are valid.
            /// </summary>
            Balloon = 0x10,
        }

        public enum NotifyIconMessage
        {
            BalloonShow = 0x402,

            BalloonHide = 0x403,

            BalloonTimeout = 0x404,

            BalloonUserClick = 0x405,

            PopupOpen = 0x406,

            PopupClose = 0x407,
        }

        public enum RegPropertyType
        {
            SPDRP_DEVICEDESC = 0x00000000, // DeviceDesc (R/W)
            SPDRP_HARDWAREID = 0x00000001, // HardwareID (R/W)
            SPDRP_COMPATIBLEIDS = 0x00000002, // CompatibleIDs (R/W)
            SPDRP_UNUSED0 = 0x00000003, // unused
            SPDRP_SERVICE = 0x00000004, // Service (R/W)
            SPDRP_UNUSED1 = 0x00000005, // unused
            SPDRP_UNUSED2 = 0x00000006, // unused
            SPDRP_CLASS = 0x00000007, // Class (R--tied to ClassGUID)
            SPDRP_CLASSGUID = 0x00000008, // ClassGUID (R/W)
            SPDRP_DRIVER = 0x00000009, // Driver (R/W)
            SPDRP_CONFIGFLAGS = 0x0000000A, // ConfigFlags (R/W)
            SPDRP_MFG = 0x0000000B, // Mfg (R/W)
            SPDRP_FRIENDLYNAME = 0x0000000C, // FriendlyName (R/W)
            SPDRP_LOCATION_INFORMATION = 0x0000000D, // LocationInformation (R/W)
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E, // PhysicalDeviceObjectName (R)
            SPDRP_CAPABILITIES = 0x0000000F, // Capabilities (R)
            SPDRP_UI_NUMBER = 0x00000010, // UiNumber (R)
            SPDRP_UPPERFILTERS = 0x00000011, // UpperFilters (R/W)
            SPDRP_LOWERFILTERS = 0x00000012, // LowerFilters (R/W)
            SPDRP_BUSTYPEGUID = 0x00000013, // BusTypeGUID (R)
            SPDRP_LEGACYBUSTYPE = 0x00000014, // LegacyBusType (R)
            SPDRP_BUSNUMBER = 0x00000015, // BusNumber (R)
            SPDRP_ENUMERATOR_NAME = 0x00000016, // Enumerator Name (R)
            SPDRP_SECURITY = 0x00000017, // Security (R/W, binary form)
            SPDRP_SECURITY_SDS = 0x00000018, // Security (W, SDS form)
            SPDRP_DEVTYPE = 0x00000019, // Device Type (R/W)
            SPDRP_EXCLUSIVE = 0x0000001A, // Device is exclusive-access (R/W)
            SPDRP_CHARACTERISTICS = 0x0000001B, // Device Characteristics (R/W)
            SPDRP_ADDRESS = 0x0000001C, // Device Address (R)
            SPDRP_UI_NUMBER_DESC_FORMAT = 0x0000001E, // UiNumberDescFormat (R/W)
            SPDRP_MAXIMUM_PROPERTY = 0x0000001F // Upper bound on ordinals
        }

        public enum SystemMenu
        {
            Size = 0xF000,

            Close = 0xF060,

            Restore = 0xF120,

            Minimize = 0xF020,

            Maximize = 0xF030,
        }

        [Flags]
        public enum WindowExStyles
        {
            DlgModalFrame = 0x1,
        }

        public enum WindowLongValue
        {
            WndProc = -4,

            HInstace = -6,

            HwndParent = -8,

            Style = -16,

            ExtendedStyle = -20,

            UserData = -21,

            ID = -12,
        }

        public enum WindowMessage
        {
            Destroy = 0x2,

            Close = 0x10,

            SetIcon = 0x80,

            MeasureItem = 0x2c,

            MouseMove = 0x200,

            MouseDown = 0x201,

            LButtonUp = 0x0202,

            LButtonDblClk = 0x0203,

            RButtonDown = 0x0204,

            RButtonUp = 0x0205,

            RButtonDblClk = 0x0206,

            MButtonDown = 0x0207,

            MButtonUp = 0x0208,

            MButtonDblClk = 0x0209,

            TrayMouseMessage = 0x800,
        }

        [Flags]
        public enum WindowStyles
        {
            SysMemu = 0x80000,

            MinimizeBox = 0x20000,

            MaximizeBox = 0x10000,

            ThickFrame = 0x40000,
        }

        #endregion

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

        #region Перечисление ResourceTypes

        public enum ResourceTypes
        {
            /// <summary>
            /// Hardware-dependent cursor resource.
            /// </summary>
            RT_CURSOR = 1,

            /// <summary>
            /// Bitmap resource.
            /// </summary>
            RT_BITMAP = 2,

            /// <summary>
            /// Hardware-dependent icon resource.
            /// </summary>
            RT_ICON = 3,

            /// <summary>
            /// Menu resource.
            /// </summary>
            RT_MENU = 4,

            /// <summary>
            /// Dialog box.
            /// </summary>
            RT_DIALOG = 5,

            /// <summary>
            /// String-table entry.
            /// </summary>
            RT_STRING = 6,

            /// <summary>
            /// Font directory resource.
            /// </summary>
            RT_FONTDIR = 7,

            /// <summary>
            /// Font resource.
            /// </summary>
            RT_FONT = 8,

            /// <summary>
            /// Accelerator table.
            /// </summary>
            RT_ACCELERATOR = 9,

            /// <summary>
            /// Application-defined resource (raw data).
            /// </summary>
            RT_RCDATA = 10,

            /// <summary>
            /// Message-table entry.
            /// </summary>
            RT_MESSAGETABLE = 11,

            /// <summary>
            /// Hardware-independent cursor resource.
            /// </summary>
            RT_GROUP_CURSOR = 12,

            /// <summary>
            /// Hardware-independent icon resource.
            /// </summary>
            RT_GROUP_ICON = 14,

            /// <summary>
            /// Version resource.
            /// </summary>
            RT_VERSION = 16,

            /// <summary>
            /// Allows a resource editing tool to associate a string with an .rc file.
            /// </summary>
            RT_DLGINCLUDE = 17,

            /// <summary>
            /// Plug and Play resource.
            /// </summary>
            RT_PLUGPLAY = 19,

            /// <summary>
            /// VXD.
            /// </summary>
            RT_VXD = 20,

            /// <summary>
            /// Animated cursor.
            /// </summary>
            RT_ANICURSOR = 21,

            /// <summary>
            /// Animated icon.
            /// </summary>
            RT_ANIICON = 22,

            /// <summary>
            /// HTML.
            /// </summary>
            RT_HTML = 23,

            /// <summary>
            /// Microsoft Windows XP: Side-by-Side Assembly XML Manifest.
            /// </summary>
            RT_MANIFEST = 24,
        }

        #endregion Перечисление ResourceTypes

        #region Перечисление ResourceHeaderTyp

        public enum ResourceHeaderTyp
        {
            /// <summary>
            /// Binary data.
            /// </summary>
            BinaryData = 0,

            /// <summary>
            /// String data.
            /// </summary>
            StringData = 1
        }

        #endregion Перечисление ResourceHeaderTyp

        #region Структура RESOURCE_HEADER

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

        #endregion Структура RESOURCE_HEADER

        #region Структура VS_FIXEDFILEINFO

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct VsFixedFileInfo
        {
            /// <summary>
            /// Contains the value 0xFEEF04BD. This is used with the szKey member of the VS_VERSIONINFO structure when searching a file for the VS_FIXEDFILEINFO structure. 
            /// </summary>
            public UInt32 Signature;

            /// <summary>
            /// Specifies the binary version number of this structure. The high-order word of this member contains the major version number, and the low-order word contains the minor version number.
            /// </summary>
            public UInt32 StrucVersion;

            /// <summary>
            /// Specifies the most significant 32 bits of the file's binary version number. This member is used with dwFileVersionLS to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 FileVersionMs;

            /// <summary>
            /// Specifies the least significant 32 bits of the file's binary version number. This member is used with dwFileVersionMS to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 FileVersionLs;

            /// <summary>
            /// Specifies the most significant 32 bits of the binary version number of the product with which this file was distributed. This member is used with dwProductVersionLS to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 ProductVersionMs;

            /// <summary>
            /// Specifies the least significant 32 bits of the binary version number of the product with which this file was distributed. This member is used with dwProductVersionMS to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 ProductVersionLs;

            /// <summary>
            /// Contains a bitmask that specifies the valid bits in dwFileFlags. A bit is valid only if it was defined when the file was created. 
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
                fixedFileInfo.FileOs = (uint)ResourceWinVer.FileOs.VOS__WINDOWS32;
                fixedFileInfo.FileSubtyp = (uint)ResourceWinVer.FileSubType.VFT2_UNKNOWN;
                fixedFileInfo.FileTyp = (uint)ResourceWinVer.FileType.VFT_DLL;
                return fixedFileInfo;
            }
        }

        #endregion Структура VS_FIXEDFILEINFO

        #region Структура VAR_HEADER

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

        #endregion Структура VAR_HEADER

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

        #region Структура OSVERSIONINFOEX

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

        #endregion Структура OSVERSIONINFOEX

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

        #endregion Типы

        #region Файлы

        public const UInt32 CREATE_ALWAYS = 2;

        public const UInt32 CREATE_NEW = 1;

        public const UInt32 FILE_ACTION_ADDED = 0x00000001;

        public const UInt32 FILE_ACTION_MODIFIED = 0x00000003;

        public const UInt32 FILE_ACTION_REMOVED = 0x00000002;

        public const UInt32 FILE_ACTION_RENAMED_NEW_NAME = 0x00000005;

        public const UInt32 FILE_ACTION_RENAMED_OLD_NAME = 0x00000004;

        public const UInt32 FILE_ATTRIBUTE_ARCHIVE = 0x00000020;

        public const UInt32 FILE_ATTRIBUTE_COMPRESSED = 0x00000800;

        public const UInt32 FILE_ATTRIBUTE_DEVICE = 0x00000040;

        public const UInt32 FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        public const UInt32 FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;

        public const UInt32 FILE_ATTRIBUTE_HIDDEN = 0x00000002;

        public const UInt32 FILE_ATTRIBUTE_NORMAL = 0x00000080;

        public const UInt32 FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;

        public const UInt32 FILE_ATTRIBUTE_OFFLINE = 0x00001000;

        public const UInt32 FILE_ATTRIBUTE_READONLY = 0x00000001;

        public const UInt32 FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;

        public const UInt32 FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;

        public const UInt32 FILE_ATTRIBUTE_SYSTEM = 0x00000004;

        public const UInt32 FILE_ATTRIBUTE_TEMPORARY = 0x00000100;

        public const UInt32 FILE_ATTRIBUTE_VIRTUAL = 0x00010000;

        public const UInt32 FILE_CASE_PRESERVED_NAMES = 0x00000002;

        public const UInt32 FILE_CASE_SENSITIVE_SEARCH = 0x00000001;

        public const UInt32 FILE_FILE_COMPRESSION = 0x00000010;

        public const UInt32 FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

        public const UInt32 FILE_FLAG_DELETE_ON_CLOSE = 0x04000000;

        public const UInt32 FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000;

        public const UInt32 FILE_FLAG_NO_BUFFERING = 0x20000000;

        public const UInt32 FILE_FLAG_OPEN_NO_RECALL = 0x00100000;

        public const UInt32 FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;

        public const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;

        public const UInt32 FILE_FLAG_POSIX_SEMANTICS = 0x01000000;

        public const UInt32 FILE_FLAG_RANDOM_ACCESS = 0x10000000;

        public const UInt32 FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000;

        public const UInt32 FILE_FLAG_WRITE_THROUGH = 0x80000000;

        public const UInt32 FILE_NAMED_STREAMS = 0x00040000;

        public const UInt32 FILE_NOTIFY_CHANGE_ATTRIBUTES = 0x00000004;

        public const UInt32 FILE_NOTIFY_CHANGE_CREATION = 0x00000040;

        public const UInt32 FILE_NOTIFY_CHANGE_DIR_NAME = 0x00000002;

        public const UInt32 FILE_NOTIFY_CHANGE_FILE_NAME = 0x00000001;

        public const UInt32 FILE_NOTIFY_CHANGE_LAST_ACCESS = 0x00000020;

        public const UInt32 FILE_NOTIFY_CHANGE_LAST_WRITE = 0x00000010;

        public const UInt32 FILE_NOTIFY_CHANGE_SECURITY = 0x00000100;

        public const UInt32 FILE_NOTIFY_CHANGE_SIZE = 0x00000008;

        public const UInt32 FILE_PERSISTENT_ACLS = 0x00000008;

        public const UInt32 FILE_READ_ONLY_VOLUME = 0x00080000;

        public const UInt32 FILE_SEQUENTIAL_WRITE_ONCE = 0x00100000;

        public const UInt32 FILE_SHARE_DELETE = 0x00000004;

        public const UInt32 FILE_SHARE_READ = 0x00000001;

        public const UInt32 FILE_SHARE_WRITE = 0x00000002;

        public const UInt32 FILE_SUPPORTS_ENCRYPTION = 0x00020000;

        public const UInt32 FILE_SUPPORTS_OBJECT_IDS = 0x00010000;

        public const UInt32 FILE_SUPPORTS_REMOTE_STORAGE = 0x00000100;

        public const UInt32 FILE_SUPPORTS_REPARSE_POINTS = 0x00000080;

        public const UInt32 FILE_SUPPORTS_SPARSE_FILES = 0x00000040;

        public const UInt32 FILE_SUPPORTS_TRANSACTIONS = 0x00200000;

        public const UInt32 FILE_UNICODE_ON_DISK = 0x00000004;

        public const UInt32 FILE_VOLUME_IS_COMPRESSED = 0x00008000;

        public const UInt32 FILE_VOLUME_QUOTAS = 0x00000020;

        public const UInt32 GENERIC_ALL = 0x10000000;

        public const UInt32 GENERIC_EXECUTE = 0x20000000;

        public const UInt32 GENERIC_READ = 0x80000000;

        public const UInt32 GENERIC_WRITE = 0x40000000;

        public const UInt32 MAILSLOT_NO_MESSAGE = 0xffffffff;

        public const UInt32 MAILSLOT_WAIT_FOREVER = 0xffffffff;

        public const UInt32 OPEN_ALWAYS = 4;

        public const UInt32 OPEN_EXISTING = 3;

        public const UInt32 TRUNCATE_EXISTING = 5;

        #endregion Файлы

        #region Ресурсы

        public const UInt16 CREATEPROCESS_MANIFEST_RESOURCE_ID = 1;

        internal const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;

        public const UInt16 ISOLATIONAWARE_MANIFEST_RESOURCE_ID = 2;

        public const UInt16 ISOLATIONAWARE_NOSTATICIMPORT_MANIFEST_RESOURCE_ID = 3;

        public const UInt16 LANG_ENGLISH = 9;

        public const UInt16 LANG_NEUTRAL = 0;

        internal const uint LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010;

        internal const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        internal const uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;

        public const UInt16 SUBLANG_ENGLISH_US = 1;

        public const UInt16 SUBLANG_NEUTRAL = 0;

        #endregion Ресурсы

        #region Коды ошибок

        internal const int ERROR_ACCESS_DENIED = 5;

        internal const int ERROR_CALL_NOT_IMPLEMENTED = 120;

        internal const int ERROR_DIR_NOT_EMPTY = 145;

        internal const int ERROR_DISK_FULL = 112;

        internal const int ERROR_DUP_NAME = 52;

        internal const int ERROR_FILE_EXISTS = 80;

        internal const int ERROR_FILE_NOT_FOUND = 2;

        internal const int ERROR_INVALID_HANDLE = 6;

        internal const int ERROR_INVALID_PARAMETER = 87;

        internal const int ERROR_IO_PENDING = 997;

        internal const int ERROR_NOT_READY = 21;

        internal const int ERROR_NOT_SUPPORTED = 50;

        internal const int ERROR_NO_MORE_FILES = 18;

        internal const int ERROR_PATH_NOT_FOUND = 3;

        internal const int ERROR_SHARING_VIOLATION = 32;

        #endregion Коды ошибок

        #region Коды WaitResult

        public const UInt32 WAIT_ABANDONED = 0x00000080;

        public const UInt32 WAIT_FAILED = 0xFFFFFFFF;

        public const UInt32 WAIT_OBJECT_0 = 0x00000000;

        public const UInt32 WAIT_TIMEOUT = 0x00000102;

        #endregion Коды WaitResult

        #region Spin-блокировки

        public static readonly int SpinCount = Environment.ProcessorCount != 1 ? 4000 : 0;

        public static readonly Boolean SpinEnabled = Environment.ProcessorCount != 1;

        #endregion Spin-блокировки

        #region Интерфейсы

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

        #endregion Интерфейсы

        #region Поля

        public static readonly IntPtr InvalidHandle = new IntPtr(-1L);

        #endregion

        #region Методы

        [DllImport("ntdll.dll")]
        public static extern int NtCreateKeyedEvent(
            [Out] out IntPtr keyedEventHandle,
            [In] int desiredAccess,
            [In] [Optional] IntPtr objectAttributes,
            [In] int flags
            );

        [DllImport("ntdll.dll")]
        public static extern int NtReleaseKeyedEvent(
            [In] IntPtr keyedEventHandle,
            [In] IntPtr keyValue,
            [In] Boolean alertable,
            [In] [Optional] IntPtr timeout
            );

        [DllImport("ntdll.dll")]
        public static extern int NtWaitForKeyedEvent(
            [In] IntPtr keyedEventHandle,
            [In] IntPtr keyValue,
            [In] Boolean alertable,
            [In] [Optional] IntPtr timeout
            );

        [SecurityCritical]
        [DllImport("kernel32", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern void SetLastError(int dwErrorCode);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr CreateFile(
            String lpFileName,
            UInt32 dwDesiredAccess,
            UInt32 dwShareMode,
            IntPtr lpSecurityAttributes,
            UInt32 dwCreationDisposition,
            UInt32 dwFlagsAndAttributes,
            NativeOverlapped* hTemplateFile);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern UInt32 ReadFileEx(
            IntPtr hFile,
            Byte* lpBuffer,
            int nNumberOfBytesToRead,
            NativeOverlapped* lpOverlapped,
            [MarshalAs(UnmanagedType.FunctionPtr)] IOCompletionCallback callback
            );

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern UInt32 WriteFileEx(
            IntPtr hFile,
            Byte* lpBuffer,
            int nNumberOfBytesToWrite,
            NativeOverlapped* lpOverlapped,
            [MarshalAs(UnmanagedType.FunctionPtr)] IOCompletionCallback callback
            );

        [DllImport("Kernel32.dll", CharSet = CharSet.None, SetLastError = true)]
        internal static extern UInt32 ReadFile(
            IntPtr hFile,
            Byte* lpBuffer,
            UInt32 nNumberOfBytesToRead,
            UInt32* lpNumberOfBytesRead,
            NativeOverlapped* lpOverlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern int FormatMessage(int dwFlags,
                                                 String lpSource,
                                                 int dwMessageId,
                                                 int dwLanguageId,
                                                 StringBuilder lpBuffer,
                                                 int nSize,
                                                 String[] args);

        [DllImport("Kernel32.dll")]
        internal static extern UInt32 WriteFile(
            IntPtr hFile,
            Byte* lpBuffer,
            UInt32 nNumberOfBytesToWrite,
            UInt32* lpNumberOfBytesWritten,
            NativeOverlapped* lpOverlapped);

        [DllImport("Kernel32.dll")]
        internal static extern Boolean CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateEvent(
            [In] [Optional] IntPtr eventAttributes,
            [In] Boolean manualReset,
            [In] Boolean initialState,
            [In] [Optional] String name
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateSemaphore(
            [In] [Optional] IntPtr semaphoreAttributes,
            [In] int initialCount,
            [In] int maximumCount,
            [In] [Optional] String name
            );

        [DllImport("kernel32.dll")]
        public static extern Boolean ReleaseSemaphore(
            [In] IntPtr semaphoreHandle,
            [In] int releaseCount,
            [In] IntPtr previousCount
            );

        [DllImport("kernel32.dll")]
        public static extern Boolean ResetEvent([In] IntPtr eventHandle);

        [DllImport("kernel32.dll")]
        public static extern Boolean SetEvent([In] IntPtr eventHandle);

        [DllImport("kernel32.dll")]
        public static extern int WaitForSingleObject([In] IntPtr handle, [In] int milliseconds);

        [DllImport("Kernel32.dll")]
        internal static extern UInt32 GetOverlappedResult(
            IntPtr hFile,
            NativeOverlapped* lpOverlapped,
            UInt32* lpNumberOfBytesTransferred,
            UInt32 bWait
            );

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GlobalMemoryStatusEx([In] [Out] MemoryStatusEx lpBuffer);

        [DllImport("kernel32.dll")]
        internal static extern bool GetVersionEx(ref OS_VERSION_INFO_EX osVersionInfo);

        [DllImport("Kernel32.dll")]
        internal static extern bool GetProductInfo(int osMajorVersion, int osMinorVersion, int spMajorVersion, int spMinorVersion, out int edition);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(String moduleName);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] String procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, [MarshalAs(UnmanagedType.LPStr)] string strEnumerator, IntPtr hParent, uint nFlags);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiEnumDeviceInfo(IntPtr lpDeviceInfoSet, int index, ref SpDeviInfoData deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr lpDeviceInfoSet, ref SpDeviInfoData deviceInfoData, ref Guid gClass, uint nIndex, ref SpDeviceInterfaceData interfaceData);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern int SetupDiDestroyDeviceInfoList(IntPtr lpInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref SpDeviceInterfaceData interfaceData, ref SpDeviceInterfaceDetailDataWithPath detailData, uint nDeviceInterfaceDetailDataSize, IntPtr nRequiredSize, IntPtr lpDeviceInfoData);

        [SecurityCritical]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal static extern Boolean SetCursorPos(int x, int y);

        [SecurityCritical]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

        [SecurityCritical]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(HandleRef hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [SecurityCritical]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern bool SetForegroundWindow(HandleRef hWnd);

        [SecurityCritical]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int RegisterWindowMessage(string msg);

        [SecurityCritical]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool DestroyIcon(IntPtr hIcon);

        [SecurityCritical]
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int IntGetWindowLong(HandleRef hWnd, int nIndex);

        [SecurityCritical]
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr IntGetWindowLongPtr(HandleRef hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int IntSetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        [SecurityCritical]
        public static int GetWindowLong(HandleRef hWnd, WindowLongValue nIndex)
        {
            int result;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                result = IntGetWindowLong(hWnd, (int)nIndex);
                Marshal.GetLastWin32Error();
            }
            else
            {
                IntPtr resultPtr = IntGetWindowLongPtr(hWnd, (int)nIndex);
                Marshal.GetLastWin32Error();
                result = IntPtrToInt32(resultPtr);
            }
            return result;
        }

        [SecurityCritical]
        public static IntPtr SetWindowLong(HandleRef hWnd, WindowLongValue nIndex, IntPtr dwNewLong)
        {
            IntPtr result;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                int intResult = IntSetWindowLong(hWnd, (int)nIndex, IntPtrToInt32(dwNewLong));
                Marshal.GetLastWin32Error();
                result = new IntPtr(intResult);
            }
            else
            {
                result = IntSetWindowLongPtr(hWnd, (int)nIndex, dwNewLong);
                Marshal.GetLastWin32Error();
            }
            return result;
        }

        [SecurityCritical]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool EnableMenuItem(HandleRef handleMenu, SystemMenu idEnabledItem, int enable);

        [SecurityCritical]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetSystemMenu(HandleRef hWnd, bool bRevert);

        [SecurityCritical]
        public static void SetSystemMenuItems(HandleRef hwnd, bool isEnabled, params SystemMenu[] menus)
        {
            if (menus != null && menus.Length > 0)
            {
                HandleRef hMenu = new HandleRef(null, GetSystemMenu(hwnd, false));

                foreach (SystemMenu menu in menus)
                    SetMenuItem(hMenu, menu, isEnabled);
            }
        }

        [SecurityCritical]
        public static void SetMenuItem(HandleRef hMenu, SystemMenu menu, bool isEnabled)
        {
            EnableMenuItem(hMenu, menu, (isEnabled) ? ~1 : 1);
        }

        [DllImport("user32.dll")]
        internal static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            Byte[] lpKeyState,
            [Out] [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)] StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        internal static extern bool GetKeyboardState(Byte[] lpKeyState);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetSystemMetrics(int nIndex);

        [SecurityCritical]
        [DllImport("shell32.dll", EntryPoint = "SHGetMalloc")]
        internal static extern int ShellGetMalloc([Out] [MarshalAs(UnmanagedType.LPArray)] IMalloc[] ppMalloc);

        [SecurityCritical]
        [DllImport("shell32.dll", EntryPoint = "SHGetFolderLocation")]
        internal static extern int ShellGetFolderLocation(IntPtr hwndOwner, Int32 nFolder, IntPtr hToken, uint dwReserved, out IntPtr ppidl);

        [SecurityCritical]
        [DllImport("shell32.dll", EntryPoint = "SHParseDisplayName")]
        internal static extern int ShellParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string pszName, IntPtr pbc, out IntPtr ppidl, uint sfgaoIn, out uint psfgaoOut);

        [SecurityCritical]
        [DllImport("shell32.dll", EntryPoint = "SHBrowseForFolder")]
        internal static extern IntPtr ShellBrowseForFolder(ref BrowseInfo lbpi);

        [SecurityCritical]
        [DllImport("shell32.dll", CharSet = CharSet.Auto, EntryPoint = "SHGetPathFromIDList")]
        internal static extern bool ShellGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

        [SecurityCritical]
        [DllImport("shell32.dll", CharSet = CharSet.Auto, EntryPoint = "Shell_NotifyIcon")]
        internal static extern int ShellNotifyIcon(int message, NotifyIconData pnid);

        [DllImport("mapi32.dll", SetLastError = true)]
        internal static extern int MapiSendMail(IntPtr sess, IntPtr hwnd, MapiMessage message, int flg, int rsv);

        /// <summary>
        /// Преобразование кода последней ошибки в строку
        /// </summary>
        public static String LastErrorToString(int err)
        {
            StringBuilder strLastErrorMessage = new StringBuilder(255);
            int ret2 = err;
            int dwFlags = 4096;

            FormatMessage(dwFlags,
                          null,
                          ret2,
                          0,
                          strLastErrorMessage,
                          strLastErrorMessage.Capacity,
                          null);

            return strLastErrorMessage.ToString();
        }

        /// <summary>
        /// Генерация исключения, соответсвующего коду GetLastError
        /// </summary>
        private static void RaiseSystemException()
        {
            int err = GetLastError();
            String message = "(#" + err + ") " + LastErrorToString(err);

            switch (err)
            {
                case ERROR_INVALID_HANDLE:
                    throw new IOException(message);
                case ERROR_FILE_NOT_FOUND:
                    throw new FileNotFoundException(message);
                case ERROR_ACCESS_DENIED:
                    throw new UnauthorizedAccessException(message);
                default:
                    throw new Exception(message);
            }
        }

        /// <summary>
        /// Handle check for INVALID_HANDLE
        /// </summary>
        public static void Check(UInt32 result)
        {
            if (result == 0)
                RaiseSystemException();
        }

        /// <summary>
        /// Handle check for INVALID_HANDLE
        /// </summary>
        public static void Check(IntPtr handle)
        {
            if (handle == InvalidHandle)
                RaiseSystemException();
        }

        /// <summary>
        /// Чтение кода последней ошибки
        /// </summary>
        /// <returns></returns>
        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }

        /// <summary>
        /// Преобразование виртуального кода в клавишу
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static char GetCharFromKey(Key key)
        {
            char ch = ' ';

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            Byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }

        /// <summary>
        /// Выделение памяти (используется в диалогах)
        /// </summary>
        /// <returns></returns>
        [SecurityCritical]
        public static IMalloc GetShMalloc()
        {
            IMalloc[] ppMalloc = new IMalloc[1];
            ShellGetMalloc(ppMalloc);
            return ppMalloc[0];
        }

        /// <summary>
        /// Преобразование IntPtr в int
        /// </summary>
        /// <param name="intPtr"></param>
        /// <returns></returns>
        public static int IntPtrToInt32(IntPtr intPtr)
        {
            return (int)intPtr.ToInt64();
        }

        /// <summary>
        /// Позиционирование окна
        /// </summary>
        public static void MoveWindow(IntPtr hWnd, RECT rect, bool bRepaint)
        {
            MoveWindow(hWnd, rect.Left, rect.Top, rect.Width, rect.Height, bRepaint);
        }

        /// <summary>
        /// Центрирование окна относительно родительского
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="hwndParent"></param>
        public static void CenterTo(IntPtr hwnd, IntPtr hwndParent)
        {
            if (hwndParent == IntPtr.Zero)
            {
                // Центрирование относительно рабочего стола
                hwndParent = GetDesktopWindow();
            }

            NativeMethods.RECT rcWindow;
            NativeMethods.GetWindowRect(hwnd, out rcWindow);

            NativeMethods.RECT rcParent;
            NativeMethods.GetWindowRect(hwndParent, out rcParent);

            int cx = (rcParent.Width - rcWindow.Width) / 2;
            int cy = (rcParent.Height - rcWindow.Height) / 2;

            // rcWindow.Center = rcParent.Center;

            rcWindow = new NativeMethods.RECT(rcParent.Left + cx, rcParent.Top + cy, rcWindow.Width, rcWindow.Height);

            NativeMethods.MoveWindow(hwnd, rcWindow, true);
        }

        #endregion

        #region Класс Exception

        public class Exception : System.SystemException
        {
            #region Конструктор

            public Exception()
            {
            }

            public Exception(String text) : base(text)
            {
            }

            public Exception(String text, Exception innerException) : base(text, innerException)
            {
            }

            #endregion
        }

        #endregion Класс Exception

        #region Класс ResourceWinVer

        public abstract class ResourceWinVer
        {
            #region Константы

            public const UInt32 VS_FFI_FILEFLAGSMASK = 0x0000003F;

            public const UInt32 VS_FFI_SIGNATURE = 0xFEEF04BD;

            public const UInt32 VS_FFI_STRUCVERSION = 0x00010000;

            #endregion

            #region Перечисление FileFlags

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

            #endregion Перечисление FileFlags

            #region Перечисление FileOs

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
                /// 
                /// </summary>
                VOS_WINCE = 0x00050000,

                /// <summary>
                /// The file was designed to run under the 16-bit Windows API. 
                /// </summary>
                VOS__WINDOWS16 = 0x00000001,

                /// <summary>
                /// The file was designed to be run under a 16-bit version of Presentation Manager. 
                /// </summary>
                VOS__PM16 = 0x00000002,

                /// <summary>
                /// The file was designed to be run under a 32-bit version of Presentation Manager.
                /// </summary>
                VOS__PM32 = 0x00000003,

                /// <summary>
                /// The file was designed to run under the 32-bit Windows API. 
                /// </summary>
                VOS__WINDOWS32 = 0x00000004,

                /// <summary>
                /// 
                /// </summary>
                VOS_DOS_WINDOWS16 = 0x00010001,

                /// <summary>
                /// 
                /// </summary>
                VOS_DOS_WINDOWS32 = 0x00010004,

                /// <summary>
                /// 
                /// </summary>
                VOS_OS216_PM16 = 0x00020002,

                /// <summary>
                /// 
                /// </summary>
                VOS_OS232_PM32 = 0x00030003,

                /// <summary>
                /// 
                /// </summary>
                VOS_NT_WINDOWS32 = 0x00040004
            }

            #endregion Перечисление FileOs

            #region Перечисление FileType

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

            #endregion Перечисление FileType

            #region Перечисление FileSubType

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

            #endregion Перечисление FileSubType
        }

        #endregion Класс ResourceWinVer

        #region Работа с ресурсами

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int SizeofResource(IntPtr hInstance, IntPtr hResInfo);

        [DllImport("kernel32.dll", EntryPoint = "FindResourceExW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, UInt16 wLanguage);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", EntryPoint = "BeginUpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        [DllImport("kernel32.dll", EntryPoint = "UpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, IntPtr lpName, UInt16 wLanguage, Byte[] lpData, UInt32 cbData);

        [DllImport("kernel32.dll", EntryPoint = "EndUpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        #endregion Работа с ресурсами

        #region Работа с блокировками

        [DllImport("kernel32.dll", EntryPoint = "AcquireSRWLockExclusive")]
        internal static extern void AcquireSrwLockExclusive(ref IntPtr srw);

        [DllImport("kernel32.dll", EntryPoint = "AcquireSRWLockShared")]
        internal static extern void AcquireSrwLockShared(ref IntPtr srw);

        [DllImport("kernel32.dll", EntryPoint = "InitializeSRWLock")]
        internal static extern void InitializeSrwLock(out IntPtr srw);

        [DllImport("kernel32.dll", EntryPoint = "ReleaseSRWLockExclusive")]
        internal static extern void ReleaseSrwLockExclusive(ref IntPtr srw);

        [DllImport("kernel32.dll", EntryPoint = "ReleaseSRWLockShared")]
        internal static extern void ReleaseSrwLockShared(ref IntPtr srw);

        #endregion Работа с блокировками

        #region Вложенный класс: SpDeviInfoData

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SpDeviInfoData
        {
            public int Size;

            public Guid ClassGuid;

            public int DevInst;

            private IntPtr _reserved;
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

    #endregion Класс NativeMethods
}