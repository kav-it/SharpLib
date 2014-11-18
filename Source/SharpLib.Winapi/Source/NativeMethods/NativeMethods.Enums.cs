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
using System.Security;

namespace SharpLib.Winapi
{
    [SuppressUnmanagedCodeSecurity]
    public sealed partial class NativeMethods
    {
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
            /// Use a balloon ToolTip instead of a standard ToolTip. The szInfo, uTimeout, szInfoTitle, and dwInfoFlags members are
            /// valid.
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
    }
}