using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

using SharpLib.Wpf.Dialogs.Interop;

namespace SharpLib.Wpf.Dialogs
{
    internal static class DialogNativeMethods
    {
        #region Делегаты

        internal delegate int BrowseCallbackProc(IntPtr hwnd, FolderBrowserDialogMessage msg, IntPtr lParam, IntPtr wParam);

        #endregion

        #region Перечисления

        [Flags]
        internal enum BrowseInfoFlags
        {
            ReturnOnlyFsDirs = 0x00000001,

            DontGoBelowDomain = 0x00000002,

            StatusText = 0x00000004,

            ReturnFsAncestors = 0x00000008,

            EditBox = 0x00000010,

            Validate = 0x00000020,

            NewDialogStyle = 0x00000040,

            UseNewUI = NewDialogStyle | EditBox,

            BrowseIncludeUrls = 0x00000080,

            UaHint = 0x00000100,

            NoNewFolderButton = 0x00000200,

            NoTranslateTargets = 0x00000400,

            BrowseForComputer = 0x00001000,

            BrowseForPrinter = 0x00002000,

            BrowseIncludeFiles = 0x00004000,

            Shareable = 0x00008000,

            BrowseFileJunctions = 0x00010000
        }

        internal enum CDCONTROLSTATE
        {
            CDCS_INACTIVE = 0x00000000,

            CDCS_ENABLED = 0x00000001,

            CDCS_VISIBLE = 0x00000002
        }

        internal enum FDAP
        {
            FDAP_BOTTOM = 0x00000000,

            FDAP_TOP = 0x00000001,
        }

        internal enum FDE_OVERWRITE_RESPONSE
        {
            FDEOR_DEFAULT = 0x00000000,

            FDEOR_ACCEPT = 0x00000001,

            FDEOR_REFUSE = 0x00000002
        }

        internal enum FDE_SHAREVIOLATION_RESPONSE
        {
            FDESVR_DEFAULT = 0x00000000,

            FDESVR_ACCEPT = 0x00000001,

            FDESVR_REFUSE = 0x00000002
        }

        internal enum FFFP_MODE
        {
            FFFP_EXACTMATCH,

            FFFP_NEARESTPARENTMATCH
        }

        [Flags]
        internal enum FOS : uint
        {
            FOS_OVERWRITEPROMPT = 0x00000002,

            FOS_STRICTFILETYPES = 0x00000004,

            FOS_NOCHANGEDIR = 0x00000008,

            FOS_PICKFOLDERS = 0x00000020,

            FOS_FORCEFILESYSTEM = 0x00000040,

            FOS_ALLNONSTORAGEITEMS = 0x00000080,

            FOS_NOVALIDATE = 0x00000100,

            FOS_ALLOWMULTISELECT = 0x00000200,

            FOS_PATHMUSTEXIST = 0x00000800,

            FOS_FILEMUSTEXIST = 0x00001000,

            FOS_CREATEPROMPT = 0x00002000,

            FOS_SHAREAWARE = 0x00004000,

            FOS_NOREADONLYRETURN = 0x00008000,

            FOS_NOTESTFILECREATE = 0x00010000,

            FOS_HIDEMRUPLACES = 0x00020000,

            FOS_HIDEPINNEDPLACES = 0x00040000,

            FOS_NODEREFERENCELINKS = 0x00100000,

            FOS_DONTADDTORECENT = 0x02000000,

            FOS_FORCESHOWHIDDEN = 0x10000000,

            FOS_DEFAULTNOMINIMODE = 0x20000000
        }

        internal enum FolderBrowserDialogMessage
        {
            Initialized = 1,

            SelChanged = 2,

            ValidateFailedA = 3,

            ValidateFailedW = 4,

            EnableOk = 0x465,

            SetSelection = 0x467
        }

        [Flags]
        internal enum FormatMessageFlags
        {
            FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,

            FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,

            FORMAT_MESSAGE_FROM_STRING = 0x00000400,

            FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,

            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,

            FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000
        }

        internal enum KF_CATEGORY
        {
            KF_CATEGORY_VIRTUAL = 0x00000001,

            KF_CATEGORY_FIXED = 0x00000002,

            KF_CATEGORY_COMMON = 0x00000003,

            KF_CATEGORY_PERUSER = 0x00000004
        }

        [Flags]
        internal enum KF_DEFINITION_FLAGS
        {
            KFDF_PERSONALIZE = 0x00000001,

            KFDF_LOCAL_REDIRECT_ONLY = 0x00000002,

            KFDF_ROAMABLE = 0x00000004,
        }

        [Flags]
        internal enum LoadLibraryExFlags : uint
        {
            DontResolveDllReferences = 0x00000001,

            LoadLibraryAsDatafile = 0x00000002,

            LoadWithAlteredSearchPath = 0x00000008,

            LoadIgnoreCodeAuthzLevel = 0x00000010
        }

        internal enum SIATTRIBFLAGS
        {
            SIATTRIBFLAGS_AND = 0x00000001,

            SIATTRIBFLAGS_OR = 0x00000002,

            SIATTRIBFLAGS_APPCOMPAT = 0x00000003,
        }

        internal enum SIGDN : uint
        {
            SIGDN_NORMALDISPLAY = 0x00000000,

            SIGDN_PARENTRELATIVEPARSING = 0x80018001,

            SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,

            SIGDN_PARENTRELATIVEEDITING = 0x80031001,

            SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,

            SIGDN_FILESYSPATH = 0x80058000,

            SIGDN_URL = 0x80068000,

            SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,

            SIGDN_PARENTRELATIVE = 0x80080001
        }

        #endregion

        #region Константы

        internal const int ERROR_FILE_NOT_FOUND = 2;

        #endregion

        #region Методы

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeModuleHandle LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryExFlags dwFlags);

        [DllImport("kernel32", SetLastError = true), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);

        internal static IShellItem CreateItemFromParsingName(string path)
        {
            object item;
            Guid guid = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe");
            int hr = SHCreateItemFromParsingName(path, IntPtr.Zero, ref guid, out item);
            if (hr != 0)
            {
                throw new System.ComponentModel.Win32Exception(hr);
            }
            return (IShellItem)item;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int LoadString(SafeModuleHandle hInstance, uint uId, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint FormatMessage([MarshalAs(UnmanagedType.U4)] FormatMessageFlags dwFlags,
            IntPtr lpSource,
            uint dwMessageId,
            uint dwLanguageId,
            ref IntPtr lpBuffer,
            uint nSize,
            string[] arguments);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

        [DllImport("shell32.dll", SetLastError = true)]
        internal static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, Environment.SpecialFolder nFolder, ref IntPtr ppidl);

        [DllImport("shell32.dll", PreserveSig = false)]
        internal static extern IMalloc SHGetMalloc();

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SHGetPathFromIDList(IntPtr pidl, StringBuilder pszPath);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, FolderBrowserDialogMessage msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, FolderBrowserDialogMessage msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetActiveWindow();

        #endregion

        #region Вложенный класс: BROWSEINFO

        internal struct BROWSEINFO
        {
            #region Поля

            internal IntPtr _hwndOwner;

            internal int _iImage;

            internal IntPtr _lParam;

            internal BrowseCallbackProc _lpfn;

            internal string _lpszTitle;

            internal IntPtr _pidlRoot;

            internal string _pszDisplayName;

            internal BrowseInfoFlags _ulFlags;

            #endregion
        }

        #endregion

        #region Вложенный класс: COMDLG_FILTERSPEC

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSpec;
        }

        #endregion

        #region Вложенный класс: KNOWNFOLDER_DEFINITION

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct KNOWNFOLDER_DEFINITION
        {
            internal KF_CATEGORY category;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszCreator;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszDescription;

            internal Guid fidParent;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszRelativePath;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszParsingName;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszToolTip;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszLocalizedName;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszIcon;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSecurity;

            internal uint dwAttributes;

            internal KF_DEFINITION_FLAGS kfdFlags;

            internal Guid ftidType;
        }

        #endregion

        #region Вложенный класс: PROPERTYKEY

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct PROPERTYKEY
        {
            internal Guid fmtid;

            internal uint pid;
        }

        #endregion
    }
}