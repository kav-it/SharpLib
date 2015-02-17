using System;
using System.Runtime.InteropServices;

namespace SharpLib.Native.Windows
{
    public partial class NativeMethods
    {
        private const uint SHGFI_ICON = 0x100;

        private const uint SHGFI_LARGEICON = 0x0;

        private const uint SHGFI_SMALLICON = 0x1;

        #region Перечисления

        [Flags]
        public enum NotifyIconFlags
        {
            Message = 0x01,

            Icon = 0x02,

            ToolTip = 0x04,

            State = 0x08,

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

        #endregion

        #region Вложенный класс: BrowseInfo

        [StructLayout(LayoutKind.Sequential)]
        public struct BrowseInfo
        {
            /// <summary>
            /// Handle to the owner window for the dialog box.
            /// </summary>
            public IntPtr HwndOwner;

            /// <summary>
            /// Pointer to an item identifier list (PIDL) specifying the
            /// location of the root folder from which to start browsing.
            /// </summary>
            public IntPtr Root;

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

        #region Вложенный класс: SHFILEINFO

        [StructLayout(LayoutKind.Sequential)]
        internal struct SHFILEINFO
        {
            internal readonly IntPtr hIcon;

            internal readonly IntPtr iIcon;

            internal readonly uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal readonly string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            internal readonly string szTypeName;
        };

        #endregion
    }
}