using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;

using SharpLib.Wpf.Dialogs.Interop;

namespace SharpLib.Wpf.Dialogs
{
    internal static class DialogNativeMethods
    {
        internal const int ERROR_FILE_NOT_FOUND = 2;

        #region LoadLibrary

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeModuleHandle LoadLibraryEx(string lpFileName, IntPtr hFile, LoadLibraryExFlags dwFlags);

        [DllImport("kernel32", SetLastError = true), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [Flags]
        internal enum LoadLibraryExFlags : uint
        {
            DontResolveDllReferences = 0x00000001,

            LoadLibraryAsDatafile = 0x00000002,

            LoadWithAlteredSearchPath = 0x00000008,

            LoadIgnoreCodeAuthzLevel = 0x00000010
        }

        #endregion

        #region Task Dialogs

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern int GetCurrentThreadId();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist"), DllImport("comctl32.dll", PreserveSig = false)]
        internal static extern void TaskDialogIndirect([In] ref TASKDIALOGCONFIG pTaskConfig, out int pnButton, out int pnRadioButton, [MarshalAs(UnmanagedType.Bool)] out bool pfVerificationFlagChecked);

        internal delegate uint TaskDialogCallback(IntPtr hwnd, uint uNotification, IntPtr wParam, IntPtr lParam, IntPtr dwRefData);

        internal const int WM_USER = 0x400;

        internal const int WM_GETICON = 0x007F;

        internal const int WM_SETICON = 0x0080;

        internal const int ICON_SMALL = 0;

        internal enum TaskDialogNotifications
        {
            Created = 0,

            Navigated = 1,

            ButtonClicked = 2,

            HyperlinkClicked = 3,

            Timer = 4,

            Destroyed = 5,

            RadioButtonClicked = 6,

            DialogConstructed = 7,

            VerificationClicked = 8,

            Help = 9,

            ExpandoButtonClicked = 10
        }

        [Flags]
        internal enum TaskDialogCommonButtonFlags
        {
            OkButton = 0x0001,

            YesButton = 0x0002,

            NoButton = 0x0004,

            CancelButton = 0x0008,

            RetryButton = 0x0010,

            CloseButton = 0x0020
        };

        [Flags]
        internal enum TaskDialogFlags
        {
            EnableHyperLinks = 0x0001,

            UseHIconMain = 0x0002,

            UseHIconFooter = 0x0004,

            AllowDialogCancellation = 0x0008,

            UseCommandLinks = 0x0010,

            UseCommandLinksNoIcon = 0x0020,

            ExpandFooterArea = 0x0040,

            ExpandedByDefault = 0x0080,

            VerificationFlagChecked = 0x0100,

            ShowProgressBar = 0x0200,

            ShowMarqueeProgressBar = 0x0400,

            CallbackTimer = 0x0800,

            PositionRelativeToWindow = 0x1000,

            RtlLayout = 0x2000,

            NoDefaultRadioButton = 0x4000,

            CanBeMinimized = 0x8000
        };

        internal enum TaskDialogMessages
        {
            NavigatePage = WM_USER + 101,

            ClickButton = WM_USER + 102,

            SetMarqueeProgressBar = WM_USER + 103,

            SetProgressBarState = WM_USER + 104,

            SetProgressBarRange = WM_USER + 105,

            SetProgressBarPos = WM_USER + 106,

            SetProgressBarMarquee = WM_USER + 107,

            SetElementText = WM_USER + 108,

            ClickRadioButton = WM_USER + 110,

            EnableButton = WM_USER + 111,

            EnableRadioButton = WM_USER + 112,

            ClickVerification = WM_USER + 113,

            UpdateElementText = WM_USER + 114,

            SetButtonElevationRequiredState = WM_USER + 115,

            UpdateIcon = WM_USER + 116
        }

        internal enum TaskDialogElements
        {
            Content,

            ExpandedInformation,

            Footer,

            MainInstruction
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct TASKDIALOG_BUTTON
        {
            internal int nButtonID;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszButtonText;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable"), StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct TASKDIALOGCONFIG
        {
            internal uint cbSize;

            internal IntPtr hwndParent;

            internal IntPtr hInstance;

            internal TaskDialogFlags dwFlags;

            internal TaskDialogCommonButtonFlags dwCommonButtons;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszWindowTitle;

            internal IntPtr hMainIcon;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszMainInstruction;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszContent;

            internal uint cButtons;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            internal IntPtr pButtons;

            internal int nDefaultButton;

            internal uint cRadioButtons;

            internal IntPtr pRadioButtons;

            internal int nDefaultRadioButton;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszVerificationText;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszExpandedInformation;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszExpandedControlText;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszCollapsedControlText;

            internal IntPtr hFooterIcon;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszFooterText;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            internal TaskDialogCallback pfCallback;

            internal IntPtr lpCallbackData;

            internal uint cxWidth;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Activation Context

        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern ActivationContextSafeHandle CreateActCtx(ref ACTCTX actctx);

        [DllImport("kernel32.dll"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal static extern void ReleaseActCtx(IntPtr hActCtx);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ActivateActCtx(ActivationContextSafeHandle hActCtx, out IntPtr lpCookie);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeactivateActCtx(uint dwFlags, IntPtr lpCookie);

        internal const int ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID = 0x004;

        internal struct ACTCTX
        {
            #region Поля

            internal int _cbSize;

            internal uint _dwFlags;

            internal string _lpApplicationName;

            internal string _lpAssemblyDirectory;

            internal string _lpResourceName;

            internal string _lpSource;

            internal ushort _wLangId;

            internal ushort _wProcessorArchitecture;

            #endregion
        }

        #endregion

        #region File Operations Definitions

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSpec;
        }

        internal enum FDAP
        {
            FDAP_BOTTOM = 0x00000000,

            FDAP_TOP = 0x00000001,
        }

        internal enum FDE_SHAREVIOLATION_RESPONSE
        {
            FDESVR_DEFAULT = 0x00000000,

            FDESVR_ACCEPT = 0x00000001,

            FDESVR_REFUSE = 0x00000002
        }

        internal enum FDE_OVERWRITE_RESPONSE
        {
            FDEOR_DEFAULT = 0x00000000,

            FDEOR_ACCEPT = 0x00000001,

            FDEOR_REFUSE = 0x00000002
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

        internal enum CDCONTROLSTATE
        {
            CDCS_INACTIVE = 0x00000000,

            CDCS_ENABLED = 0x00000001,

            CDCS_VISIBLE = 0x00000002
        }

        #endregion

        #region KnownFolder Definitions

        internal enum FFFP_MODE
        {
            FFFP_EXACTMATCH,

            FFFP_NEARESTPARENTMATCH
        }

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

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct PROPERTYKEY
        {
            internal Guid fmtid;

            internal uint pid;
        }

        #endregion

        #region Shell Parsing Names

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

        #endregion

        #region String Resources

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

        #endregion

        #region Credentials

        internal const int CREDUI_MAX_USERNAME_LENGTH = 256 + 1 + 256;

        internal const int CREDUI_MAX_PASSWORD_LENGTH = 256;

        [Flags]
        internal enum CREDUI_FLAGS
        {
            INCORRECT_PASSWORD = 0x1,

            DO_NOT_PERSIST = 0x2,

            REQUEST_ADMINISTRATOR = 0x4,

            EXCLUDE_CERTIFICATES = 0x8,

            REQUIRE_CERTIFICATE = 0x10,

            SHOW_SAVE_CHECK_BOX = 0x40,

            ALWAYS_SHOW_UI = 0x80,

            REQUIRE_SMARTCARD = 0x100,

            PASSWORD_ONLY_OK = 0x200,

            VALIDATE_USERNAME = 0x400,

            COMPLETE_USERNAME = 0x800,

            PERSIST = 0x1000,

            SERVER_CREDENTIAL = 0x4000,

            EXPECT_CONFIRMATION = 0x20000,

            GENERIC_CREDENTIALS = 0x40000,

            USERNAME_TARGET_CREDENTIALS = 0x80000,

            KEEP_USERNAME = 0x100000
        }

        [Flags]
        internal enum CredUIWinFlags
        {
            Generic = 0x1,

            Checkbox = 0x2,

            AutoPackageOnly = 0x10,

            InCredOnly = 0x20,

            EnumerateAdmins = 0x100,

            EnumerateCurrentUser = 0x200,

            SecurePrompt = 0x1000,

            Pack32Wow = 0x10000000
        }

        internal enum CredUIReturnCodes
        {
            NO_ERROR = 0,

            ERROR_CANCELLED = 1223,

            ERROR_NO_SUCH_LOGON_SESSION = 1312,

            ERROR_NOT_FOUND = 1168,

            ERROR_INVALID_ACCOUNT_NAME = 1315,

            ERROR_INSUFFICIENT_BUFFER = 122,

            ERROR_INVALID_PARAMETER = 87,

            ERROR_INVALID_FLAGS = 1004
        }

        internal enum CredTypes
        {
            CRED_TYPE_GENERIC = 1,

            CRED_TYPE_DOMAIN_PASSWORD = 2,

            CRED_TYPE_DOMAIN_CERTIFICATE = 3,

            CRED_TYPE_DOMAIN_VISIBLE_PASSWORD = 4
        }

        internal enum CredPersist
        {
            Session = 1,

            LocalMachine = 2,

            Enterprise = 3
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        internal struct CREDUI_INFO
        {
            #region Поля

            internal int _cbSize;

            internal IntPtr _hbmBanner;

            internal IntPtr _hwndParent;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string _pszCaptionText;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string _pszMessageText;

            #endregion
        }

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        internal static extern CredUIReturnCodes CredUIPromptForCredentials(
            ref CREDUI_INFO pUiInfo,
            string targetName,
            IntPtr reserved,
            int dwAuthError,
            StringBuilder pszUserName,
            uint ulUserNameMaxChars,
            StringBuilder pszPassword,
            uint ulPaswordMaxChars,
            [MarshalAs(UnmanagedType.Bool), In, Out] ref bool pfSave,
            CREDUI_FLAGS dwFlags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        internal static extern CredUIReturnCodes CredUIPromptForWindowsCredentials(
            ref CREDUI_INFO pUiInfo,
            uint dwAuthError,
            ref uint pulAuthPackage,
            IntPtr pvInAuthBuffer,
            uint ulInAuthBufferSize,
            out IntPtr ppvOutAuthBuffer,
            out uint pulOutAuthBufferSize,
            [MarshalAs(UnmanagedType.Bool)] ref bool pfSave,
            CredUIWinFlags dwFlags);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredReadW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredRead(string targetName, CredTypes type, int flags, out IntPtr credential);

        [DllImport("advapi32.dll"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern void CredFree(IntPtr buffer);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredDeleteW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredDelete(string targetName, CredTypes type, int flags);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredWriteW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredWrite(ref CREDENTIAL credential, int flags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredPackAuthenticationBuffer(uint dwFlags, string pszUserName, string pszPassword, IntPtr pPackedCredentials, ref uint pcbPackedCredentials);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredUnPackAuthenticationBuffer(uint dwFlags,
            IntPtr pAuthBuffer,
            uint cbAuthBuffer,
            StringBuilder pszUserName,
            ref uint pcchMaxUserName,
            StringBuilder pszDomainName,
            ref uint pcchMaxDomainName,
            StringBuilder pszPassword,
            ref uint pcchMaxPassword);

#pragma warning disable 649

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
        internal struct CREDENTIAL
        {
            #region Поля

            internal int AttributeCount;

            internal IntPtr Attributes;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string Comment;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
            internal IntPtr CredentialBlob;

            internal uint CredentialBlobSize;

            internal int Flags;

            internal long LastWritten;

            [MarshalAs(UnmanagedType.U4)]
            internal CredPersist Persist;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string TargetAlias;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string TargetName;

            internal CredTypes Type;

            [MarshalAs(UnmanagedType.LPWStr)]
            internal string UserName;

            #endregion
        }
#pragma warning restore 649

        #endregion

        #region Downlevel folder browser dialog

        internal enum FolderBrowserDialogMessage
        {
            Initialized = 1,

            SelChanged = 2,

            ValidateFailedA = 3,

            ValidateFailedW = 4,

            EnableOk = 0x465,

            SetSelection = 0x467
        }

        internal delegate int BrowseCallbackProc(IntPtr hwnd, FolderBrowserDialogMessage msg, IntPtr lParam, IntPtr wParam);

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

        #endregion
    }
}