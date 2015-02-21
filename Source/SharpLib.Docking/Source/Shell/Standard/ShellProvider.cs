using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Standard
{
    internal enum SIATTRIBFLAGS
    {
        AND = 0x00000001,

        OR = 0x00000002,

        APPCOMPAT = 0x00000003,
    }

    internal enum APPDOCLISTTYPE
    {
        ADLT_RECENT = 0,

        ADLT_FREQUENT,
    }

    [Flags]
    internal enum STPF
    {
        NONE = 0x00000000,

        USEAPPTHUMBNAILALWAYS = 0x00000001,

        USEAPPTHUMBNAILWHENACTIVE = 0x00000002,

        USEAPPPEEKALWAYS = 0x00000004,

        USEAPPPEEKWHENACTIVE = 0x00000008,
    }

    internal enum TBPF
    {
        NOPROGRESS = 0x00000000,

        INDETERMINATE = 0x00000001,

        NORMAL = 0x00000002,

        ERROR = 0x00000004,

        PAUSED = 0x00000008,
    }

    [Flags]
    internal enum THB : uint
    {
        BITMAP = 0x0001,

        ICON = 0x0002,

        TOOLTIP = 0x0004,

        FLAGS = 0x0008,
    }

    [Flags]
    internal enum THBF : uint
    {
        ENABLED = 0x0000,

        DISABLED = 0x0001,

        DISMISSONCLICK = 0x0002,

        NOBACKGROUND = 0x0004,

        HIDDEN = 0x0008,

        NONINTERACTIVE = 0x0010,
    }

    internal enum GPS
    {
        DEFAULT = 0x00000000,

        HANDLERPROPERTIESONLY = 0x00000001,

        READWRITE = 0x00000002,

        TEMPORARY = 0x00000004,

        FASTPROPERTIESONLY = 0x00000008,

        OPENSLOWITEM = 0x00000010,

        DELAYCREATION = 0x00000020,

        BESTEFFORT = 0x00000040,

        NO_OPLOCK = 0x00000080,

        MASK_VALID = 0x000000FF,
    }

    internal enum KDC
    {
        FREQUENT = 1,

        RECENT,
    }

    [Flags]
    internal enum SFGAO : uint
    {
        CANCOPY = 0x1,

        CANMOVE = 0x2,

        CANLINK = 0x4,

        STORAGE = 0x00000008,

        CANRENAME = 0x00000010,

        CANDELETE = 0x00000020,

        HASPROPSHEET = 0x00000040,

        DROPTARGET = 0x00000100,

        CAPABILITYMASK = 0x00000177,

        ENCRYPTED = 0x00002000,

        ISSLOW = 0x00004000,

        GHOSTED = 0x00008000,

        LINK = 0x00010000,

        SHARE = 0x00020000,

        READONLY = 0x00040000,

        HIDDEN = 0x00080000,

        DISPLAYATTRMASK = 0x000FC000,

        FILESYSANCESTOR = 0x10000000,

        FOLDER = 0x20000000,

        FILESYSTEM = 0x40000000,

        HASSUBFOLDER = 0x80000000,

        CONTENTSMASK = 0x80000000,

        VALIDATE = 0x01000000,

        REMOVABLE = 0x02000000,

        COMPRESSED = 0x04000000,

        BROWSABLE = 0x08000000,

        NONENUMERATED = 0x00100000,

        NEWCONTENT = 0x00200000,

        CANMONIKER = 0x00400000,

        HASSTORAGE = 0x00400000,

        STREAM = 0x00400000,

        STORAGEANCESTOR = 0x00800000,

        STORAGECAPMASK = 0x70C50008,

        PKEYSFGAOMASK = 0x81044000,
    }

    internal enum SHCONTF
    {
        CHECKING_FOR_CHILDREN = 0x0010,

        FOLDERS = 0x0020,

        NONFOLDERS = 0x0040,

        INCLUDEHIDDEN = 0x0080,

        INIT_ON_FIRST_NEXT = 0x0100,

        NETPRINTERSRCH = 0x0200,

        SHAREABLE = 0x0400,

        STORAGE = 0x0800,

        NAVIGATION_ENUM = 0x1000,

        FASTITEMS = 0x2000,

        FLATLIST = 0x4000,

        ENABLE_ASYNC = 0x8000,
    }

    [Flags]
    internal enum SHGDN
    {
        SHGDN_NORMAL = 0x0000,

        SHGDN_INFOLDER = 0x0001,

        SHGDN_FOREDITING = 0x1000,

        SHGDN_FORADDRESSBAR = 0x4000,

        SHGDN_FORPARSING = 0x8000,
    }

    internal enum SICHINT : uint
    {
        DISPLAY = 0x00000000,

        ALLFIELDS = 0x80000000,

        CANONICAL = 0x10000000,

        TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
    };

    internal enum SIGDN : uint
    {
        NORMALDISPLAY = 0x00000000,

        PARENTRELATIVEPARSING = 0x80018001,

        DESKTOPABSOLUTEPARSING = 0x80028000,

        PARENTRELATIVEEDITING = 0x80031001,

        DESKTOPABSOLUTEEDITING = 0x8004c000,

        FILESYSPATH = 0x80058000,

        URL = 0x80068000,

        PARENTRELATIVEFORADDRESSBAR = 0x8007c001,

        PARENTRELATIVE = 0x80080001,
    }

    internal static class STR_GPS
    {
        #region Константы

        public const string BESTEFFORT = "GPS_BESTEFFORT";

        public const string DELAYCREATION = "GPS_DELAYCREATION";

        public const string FASTPROPERTIESONLY = "GPS_FASTPROPERTIESONLY";

        public const string HANDLERPROPERTIESONLY = "GPS_HANDLERPROPERTIESONLY";

        public const string NO_OPLOCK = "GPS_NO_OPLOCK";

        public const string OPENSLOWITEM = "GPS_OPENSLOWITEM";

        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
    internal struct THUMBBUTTON
    {
        public const int THBN_CLICKED = 0x1800;

        public THB dwMask;

        public uint iId;

        public uint iBitmap;

        public IntPtr hIcon;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szTip;

        public THBF dwFlags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PKEY
    {
        private readonly Guid _fmtid;

        private readonly uint _pid;

        public PKEY(Guid fmtid, uint pid)
        {
            _fmtid = fmtid;
            _pid = pid;
        }

        public static readonly PKEY Title = new PKEY(new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9"), 2);

        public static readonly PKEY AppUserModel_ID = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);

        public static readonly PKEY AppUserModel_IsDestListSeparator = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 6);

        public static readonly PKEY AppUserModel_RelaunchCommand = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 2);

        public static readonly PKEY AppUserModel_RelaunchDisplayNameResource = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 4);

        public static readonly PKEY AppUserModel_RelaunchIconResource = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 3);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.EnumIdList),
    ]
    internal interface IEnumIDList
    {
        [PreserveSig]
        HRESULT Next(uint celt, out IntPtr rgelt, out int pceltFetched);

        [PreserveSig]
        HRESULT Skip(uint celt);

        void Reset();

        void Clone([Out, MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenum);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.EnumObjects),
    ]
    internal interface IEnumObjects
    {
        void Next(uint celt,
            [In] ref Guid riid,
            [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.IUnknown, IidParameterIndex = 1, SizeParamIndex = 0)] object[] rgelt,
            [Out] out uint pceltFetched);

        void Skip(uint celt);

        void Reset();

        IEnumObjects Clone();
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.ObjectArray),
    ]
    internal interface IObjectArray
    {
        uint GetCount();

        [return: MarshalAs(UnmanagedType.IUnknown)]
        object GetAt([In] uint uiIndex, [In] ref Guid riid);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.ObjectArray),
    ]
    internal interface IObjectCollection : IObjectArray
    {
        #region IObjectArray redeclarations

        new uint GetCount();

        [return: MarshalAs(UnmanagedType.IUnknown)]
        new object GetAt([In] uint uiIndex, [In] ref Guid riid);

        #endregion

        void AddObject([MarshalAs(UnmanagedType.IUnknown)] object punk);

        void AddFromArray(IObjectArray poaSource);

        void RemoveObjectAt(uint uiIndex);

        void Clear();
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.PropertyStore)
    ]
    internal interface IPropertyStore
    {
        uint GetCount();

        PKEY GetAt(uint iProp);

        void GetValue([In] ref PKEY pkey, [In, Out] PROPVARIANT pv);

        void SetValue([In] ref PKEY pkey, PROPVARIANT pv);

        void Commit();
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.ShellFolder),
    ]
    internal interface IShellFolder
    {
        void ParseDisplayName(
            [In] IntPtr hwnd,
            [In] IBindCtx pbc,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName,
            [In, Out] ref int pchEaten,
            [Out] out IntPtr ppidl,
            [In, Out] ref uint pdwAttributes);

        IEnumIDList EnumObjects(
            [In] IntPtr hwnd,
            [In] SHCONTF grfFlags);

        [return: MarshalAs(UnmanagedType.Interface)]
        object BindToObject(
            [In] IntPtr pidl,
            [In] IBindCtx pbc,
            [In] ref Guid riid);

        [return: MarshalAs(UnmanagedType.Interface)]
        object BindToStorage([In] IntPtr pidl, [In] IBindCtx pbc, [In] ref Guid riid);

        [PreserveSig]
        HRESULT CompareIDs([In] IntPtr lParam, [In] IntPtr pidl1, [In] IntPtr pidl2);

        [return: MarshalAs(UnmanagedType.Interface)]
        object CreateViewObject([In] IntPtr hwndOwner, [In] ref Guid riid);

        void GetAttributesOf(
            [In] uint cidl,
            [In] IntPtr apidl,
            [In, Out] ref SFGAO rgfInOut);

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetUIObjectOf(
            [In] IntPtr hwndOwner,
            [In] uint cidl,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.SysInt, SizeParamIndex = 2)] IntPtr apidl,
            [In] ref Guid riid,
            [In, Out] ref uint rgfReserved);

        void GetDisplayNameOf([In] IntPtr pidl, [In] SHGDN uFlags, [Out] out IntPtr pName);

        void SetNameOf([In] IntPtr hwnd,
            [In] IntPtr pidl,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszName,
            [In] SHGDN uFlags,
            [Out] out IntPtr ppidlOut);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.ShellItem),
    ]
    internal interface IShellItem
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object BindToHandler(IBindCtx pbc, [In] ref Guid bhid, [In] ref Guid riid);

        IShellItem GetParent();

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetDisplayName(SIGDN sigdnName);

        SFGAO GetAttributes(SFGAO sfgaoMask);

        int Compare(IShellItem psi, SICHINT hint);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.ShellItemArray),
    ]
    internal interface IShellItemArray
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object BindToHandler(IBindCtx pbc, [In] ref Guid rbhid, [In] ref Guid riid);

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetPropertyStore(int flags, [In] ref Guid riid);

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetPropertyDescriptionList([In] ref PKEY keyType, [In] ref Guid riid);

        uint GetAttributes(SIATTRIBFLAGS dwAttribFlags, uint sfgaoMask);

        uint GetCount();

        IShellItem GetItemAt(uint dwIndex);

        [return: MarshalAs(UnmanagedType.Interface)]
        object EnumItems();
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.ShellItem2),
    ]
    internal interface IShellItem2 : IShellItem
    {
        #region IShellItem redeclarations

        [return: MarshalAs(UnmanagedType.Interface)]
        new object BindToHandler([In] IBindCtx pbc, [In] ref Guid bhid, [In] ref Guid riid);

        new IShellItem GetParent();

        [return: MarshalAs(UnmanagedType.LPWStr)]
        new string GetDisplayName(SIGDN sigdnName);

        new SFGAO GetAttributes(SFGAO sfgaoMask);

        new int Compare(IShellItem psi, SICHINT hint);

        #endregion

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetPropertyStore(
            GPS flags,
            [In] ref Guid riid);

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetPropertyStoreWithCreateObject(
            GPS flags,
            [MarshalAs(UnmanagedType.IUnknown)] object punkCreateObject,
            [In] ref Guid riid);

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetPropertyStoreForKeys(
            IntPtr rgKeys,
            uint cKeys,
            GPS flags,
            [In] ref Guid riid);

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetPropertyDescriptionList(
            IntPtr keyType,
            [In] ref Guid riid);

        void Update(IBindCtx pbc);

        PROPVARIANT GetProperty(IntPtr key);

        Guid GetCLSID(IntPtr key);

        FILETIME GetFileTime(IntPtr key);

        int GetInt32(IntPtr key);

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetString(IntPtr key);

        uint GetUInt32(IntPtr key);

        ulong GetUInt64(IntPtr key);

        [return: MarshalAs(UnmanagedType.Bool)]
        void GetBool(IntPtr key);
    }

    [
        ComImport,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid(IID.ShellLink),
    ]
    internal interface IShellLinkW
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, [In, Out] WIN32_FIND_DATAW pfd, SLGP fFlags);

        void GetIDList(out IntPtr ppidl);

        void SetIDList(IntPtr pidl);

        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxName);

        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

        short GetHotKey();

        void SetHotKey(short wHotKey);

        uint GetShowCmd();

        void SetShowCmd(uint iShowCmd);

        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

        void Resolve(IntPtr hwnd, uint fFlags);

        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid(IID.TaskbarList)]
    internal interface ITaskbarList
    {
        void HrInit();

        void AddTab(IntPtr hwnd);

        void DeleteTab(IntPtr hwnd);

        void ActivateTab(IntPtr hwnd);

        void SetActiveAlt(IntPtr hwnd);
    }

    [ComImport,InterfaceType(ComInterfaceType.InterfaceIsIUnknown),Guid(IID.TaskbarList2)]
    internal interface ITaskbarList2 : ITaskbarList
    {
        #region ITaskbarList redeclaration

        new void HrInit();

        new void AddTab(IntPtr hwnd);

        new void DeleteTab(IntPtr hwnd);

        new void ActivateTab(IntPtr hwnd);

        new void SetActiveAlt(IntPtr hwnd);

        #endregion

        void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);
    }

    [ComImport,InterfaceType(ComInterfaceType.InterfaceIsIUnknown),Guid(IID.ApplicationDestinations)]
    internal interface IApplicationDestinations
    {
        void SetAppID([In, MarshalAs(UnmanagedType.LPWStr)] string pszAppId);

        void RemoveDestination([MarshalAs(UnmanagedType.IUnknown)] object punk);

        void RemoveAllDestinations();
    }

    [ComImport,InterfaceType(ComInterfaceType.InterfaceIsIUnknown),Guid(IID.ApplicationDocumentLists)]
    internal interface IApplicationDocumentLists
    {
        void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);

        [return: MarshalAs(UnmanagedType.IUnknown)]
        object GetList([In] APPDOCLISTTYPE listtype, [In] uint cItemsDesired, [In] ref Guid riid);
    }

    [ComImport,InterfaceType(ComInterfaceType.InterfaceIsIUnknown),Guid(IID.CustomDestinationList)]
    internal interface ICustomDestinationList
    {
        void SetAppID([In, MarshalAs(UnmanagedType.LPWStr)] string pszAppID);

        [return: MarshalAs(UnmanagedType.Interface)]
        object BeginList(out uint pcMaxSlots, [In] ref Guid riid);

        [PreserveSig]
        HRESULT AppendCategory([MarshalAs(UnmanagedType.LPWStr)] string pszCategory, IObjectArray poa);

        void AppendKnownCategory(KDC category);

        [PreserveSig]
        HRESULT AddUserTasks(IObjectArray poa);

        void CommitList();

        [return: MarshalAs(UnmanagedType.Interface)]
        object GetRemovedDestinations([In] ref Guid riid);

        void DeleteList([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);

        void AbortList();
    }

    [ComImport,InterfaceType(ComInterfaceType.InterfaceIsIUnknown),Guid(IID.ObjectWithAppUserModelId)]
    internal interface IObjectWithAppUserModelId
    {
        void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetAppID();
    };

    [ComImport,InterfaceType(ComInterfaceType.InterfaceIsIUnknown),Guid(IID.ObjectWithProgId)]
    internal interface IObjectWithProgId
    {
        void SetProgID([MarshalAs(UnmanagedType.LPWStr)] string pszProgID);

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetProgID();
    };

    [ComImport,InterfaceType(ComInterfaceType.InterfaceIsIUnknown),Guid(IID.TaskbarList3),]
    internal interface ITaskbarList3 : ITaskbarList2
    {
        #region ITaskbarList2 redeclaration

        #region ITaskbarList redeclaration

        new void HrInit();

        new void AddTab(IntPtr hwnd);

        new void DeleteTab(IntPtr hwnd);

        new void ActivateTab(IntPtr hwnd);

        new void SetActiveAlt(IntPtr hwnd);

        #endregion

        new void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        #endregion

        [PreserveSig]
        HRESULT SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);

        [PreserveSig]
        HRESULT SetProgressState(IntPtr hwnd, TBPF tbpFlags);

        [PreserveSig]
        HRESULT RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);

        [PreserveSig]
        HRESULT UnregisterTab(IntPtr hwndTab);

        [PreserveSig]
        HRESULT SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);

        [PreserveSig]
        HRESULT SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, uint dwReserved);

        [PreserveSig]
        HRESULT ThumbBarAddButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] THUMBBUTTON[] pButtons);

        [PreserveSig]
        HRESULT ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] THUMBBUTTON[] pButtons);

        [PreserveSig]
        HRESULT ThumbBarSetImageList(IntPtr hwnd, [MarshalAs(UnmanagedType.IUnknown)] object himl);

        [PreserveSig]
        HRESULT SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

        [PreserveSig]
        HRESULT SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);

        [PreserveSig]
        HRESULT SetThumbnailClip(IntPtr hwnd, RefRECT prcClip);
    }

    [ComImport,InterfaceType(ComInterfaceType.InterfaceIsIUnknown),Guid(IID.TaskbarList3),]
    internal interface ITaskbarList4 : ITaskbarList3
    {
        #region ITaskbarList3 redeclaration

        #region ITaskbarList2 redeclaration

        #region ITaskbarList redeclaration

        new void HrInit();

        new void AddTab(IntPtr hwnd);

        new void DeleteTab(IntPtr hwnd);

        new void ActivateTab(IntPtr hwnd);

        new void SetActiveAlt(IntPtr hwnd);

        #endregion

        new void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        #endregion

        [PreserveSig]
        new HRESULT SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);

        [PreserveSig]
        new HRESULT SetProgressState(IntPtr hwnd, TBPF tbpFlags);

        [PreserveSig]
        new HRESULT RegisterTab(IntPtr hwndTab, IntPtr hwndMdi);

        [PreserveSig]
        new HRESULT UnregisterTab(IntPtr hwndTab);

        [PreserveSig]
        new HRESULT SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);

        [PreserveSig]
        new HRESULT SetTabActive(IntPtr hwndTab, IntPtr hwndMdi, uint dwReserved);

        [PreserveSig]
        new HRESULT ThumbBarAddButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] THUMBBUTTON[] pButtons);

        [PreserveSig]
        new HRESULT ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] THUMBBUTTON[] pButtons);

        [PreserveSig]
        new HRESULT ThumbBarSetImageList(IntPtr hwnd, [MarshalAs(UnmanagedType.IUnknown)] object himl);

        [PreserveSig]
        new HRESULT SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

        [PreserveSig]
        new HRESULT SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);

        [PreserveSig]
        new HRESULT SetThumbnailClip(IntPtr hwnd, RefRECT prcClip);

        #endregion

        void SetTabProperties(IntPtr hwndTab, STPF stpFlags);
    }
}