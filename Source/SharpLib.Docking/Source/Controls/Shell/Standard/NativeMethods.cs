﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;
using System.Text;

using Microsoft.Win32.SafeHandles;

namespace Standard
{
    internal static class Win32Value
    {
        #region Константы

        public const uint FALSE = 0;

        public const uint INFOTIPSIZE = 1024;

        public const uint MAX_PATH = 260;

        public const uint TRUE = 1;

        public const uint SIZEOF_BOOL = 4;

        public const uint SIZEOF_CHAR = 1;

        public const uint SIZEOF_WCHAR = 2;

        #endregion
    }

    [Flags]
    internal enum HCF
    {
        HIGHCONTRASTON = 0x00000001,

        AVAILABLE = 0x00000002,

        HOTKEYACTIVE = 0x00000004,

        CONFIRMHOTKEY = 0x00000008,

        HOTKEYSOUND = 0x00000010,

        INDICATOR = 0x00000020,

        HOTKEYAVAILABLE = 0x00000040,
    }

    internal enum BI
    {
        RGB = 0,
    }

    internal enum RGN
    {
        AND = 1,

        OR = 2,

        XOR = 3,

        DIFF = 4,

        COPY = 5,
    }

    internal enum CombineRgnResult
    {
        ERROR = 0,

        NULLREGION = 1,

        SIMPLEREGION = 2,

        COMPLEXREGION = 3,
    }

    internal enum OLECMDEXECOPT
    {
        DODEFAULT = 0,

        PROMPTUSER = 1,

        DONTPROMPTUSER = 2,

        SHOWHELP = 3
    }

    internal enum OLECMDF
    {
        SUPPORTED = 1,

        ENABLED = 2,

        LATCHED = 4,

        NINCHED = 8,

        INVISIBLE = 16,

        DEFHIDEONCTXTMENU = 32
    }

    internal enum OLECMDID
    {
        OPEN = 1,

        NEW = 2,

        SAVE = 3,

        SAVEAS = 4,

        SAVECOPYAS = 5,

        PRINT = 6,

        PRINTPREVIEW = 7,

        PAGESETUP = 8,

        SPELL = 9,

        PROPERTIES = 10,

        CUT = 11,

        COPY = 12,

        PASTE = 13,

        PASTESPECIAL = 14,

        UNDO = 15,

        REDO = 16,

        SELECTALL = 17,

        CLEARSELECTION = 18,

        ZOOM = 19,

        GETZOOMRANGE = 20,

        UPDATECOMMANDS = 21,

        REFRESH = 22,

        STOP = 23,

        HIDETOOLBARS = 24,

        SETPROGRESSMAX = 25,

        SETPROGRESSPOS = 26,

        SETPROGRESSTEXT = 27,

        SETTITLE = 28,

        SETDOWNLOADSTATE = 29,

        STOPDOWNLOAD = 30,

        ONTOOLBARACTIVATED = 31,

        FIND = 32,

        DELETE = 33,

        HTTPEQUIV = 34,

        HTTPEQUIV_DONE = 35,

        ENABLE_INTERACTION = 36,

        ONUNLOAD = 37,

        PROPERTYBAG2 = 38,

        PREREFRESH = 39,

        SHOWSCRIPTERROR = 40,

        SHOWMESSAGE = 41,

        SHOWFIND = 42,

        SHOWPAGESETUP = 43,

        SHOWPRINT = 44,

        CLOSE = 45,

        ALLOWUILESSSAVEAS = 46,

        DONTDOWNLOADCSS = 47,

        UPDATEPAGESTATUS = 48,

        PRINT2 = 49,

        PRINTPREVIEW2 = 50,

        SETPRINTTEMPLATE = 51,

        GETPRINTTEMPLATE = 52,

        PAGEACTIONBLOCKED = 55,

        PAGEACTIONUIQUERY = 56,

        FOCUSVIEWCONTROLS = 57,

        FOCUSVIEWCONTROLSQUERY = 58,

        SHOWPAGEACTIONMENU = 59
    }

    internal enum READYSTATE
    {
        UNINITIALIZED = 0,

        LOADING = 1,

        LOADED = 2,

        INTERACTIVE = 3,

        COMPLETE = 4
    }

    internal enum DOGIF
    {
        DEFAULT = 0x0000,

        TRAVERSE_LINK = 0x0001,

        NO_HDROP = 0x0002,

        NO_URL = 0x0004,

        ONLY_IF_ONE = 0x0008,
    }

    internal enum DWM_SIT
    {
        None,

        DISPLAYFRAME = 1,
    }

    [Flags]
    internal enum ErrorModes
    {
        Default = 0x0,

        FailCriticalErrors = 0x1,

        NoGpFaultErrorBox = 0x2,

        NoAlignmentFaultExcept = 0x4,

        NoOpenFileErrorBox = 0x8000
    }

    internal enum HT
    {
        ERROR = -2,

        TRANSPARENT = -1,

        NOWHERE = 0,

        CLIENT = 1,

        CAPTION = 2,

        SYSMENU = 3,

        GROWBOX = 4,

        SIZE = GROWBOX,

        MENU = 5,

        HSCROLL = 6,

        VSCROLL = 7,

        MINBUTTON = 8,

        MAXBUTTON = 9,

        LEFT = 10,

        RIGHT = 11,

        TOP = 12,

        TOPLEFT = 13,

        TOPRIGHT = 14,

        BOTTOM = 15,

        BOTTOMLEFT = 16,

        BOTTOMRIGHT = 17,

        BORDER = 18,

        REDUCE = MINBUTTON,

        ZOOM = MAXBUTTON,

        SIZEFIRST = LEFT,

        SIZELAST = BOTTOMRIGHT,

        OBJECT = 19,

        CLOSE = 20,

        HELP = 21
    }

    internal enum GCLP
    {
        HBRBACKGROUND = -10,
    }

    internal enum GWL
    {
        WNDPROC = (-4),

        HINSTANCE = (-6),

        HWNDPARENT = (-8),

        STYLE = (-16),

        EXSTYLE = (-20),

        USERDATA = (-21),

        ID = (-12)
    }

    internal enum SM
    {
        CXSCREEN = 0,

        CYSCREEN = 1,

        CXVSCROLL = 2,

        CYHSCROLL = 3,

        CYCAPTION = 4,

        CXBORDER = 5,

        CYBORDER = 6,

        CXFIXEDFRAME = 7,

        CYFIXEDFRAME = 8,

        CYVTHUMB = 9,

        CXHTHUMB = 10,

        CXICON = 11,

        CYICON = 12,

        CXCURSOR = 13,

        CYCURSOR = 14,

        CYMENU = 15,

        CXFULLSCREEN = 16,

        CYFULLSCREEN = 17,

        CYKANJIWINDOW = 18,

        MOUSEPRESENT = 19,

        CYVSCROLL = 20,

        CXHSCROLL = 21,

        DEBUG = 22,

        SWAPBUTTON = 23,

        CXMIN = 28,

        CYMIN = 29,

        CXSIZE = 30,

        CYSIZE = 31,

        CXFRAME = 32,

        CXSIZEFRAME = CXFRAME,

        CYFRAME = 33,

        CYSIZEFRAME = CYFRAME,

        CXMINTRACK = 34,

        CYMINTRACK = 35,

        CXDOUBLECLK = 36,

        CYDOUBLECLK = 37,

        CXICONSPACING = 38,

        CYICONSPACING = 39,

        MENUDROPALIGNMENT = 40,

        PENWINDOWS = 41,

        DBCSENABLED = 42,

        CMOUSEBUTTONS = 43,

        SECURE = 44,

        CXEDGE = 45,

        CYEDGE = 46,

        CXMINSPACING = 47,

        CYMINSPACING = 48,

        CXSMICON = 49,

        CYSMICON = 50,

        CYSMCAPTION = 51,

        CXSMSIZE = 52,

        CYSMSIZE = 53,

        CXMENUSIZE = 54,

        CYMENUSIZE = 55,

        ARRANGE = 56,

        CXMINIMIZED = 57,

        CYMINIMIZED = 58,

        CXMAXTRACK = 59,

        CYMAXTRACK = 60,

        CXMAXIMIZED = 61,

        CYMAXIMIZED = 62,

        NETWORK = 63,

        CLEANBOOT = 67,

        CXDRAG = 68,

        CYDRAG = 69,

        SHOWSOUNDS = 70,

        CXMENUCHECK = 71,

        CYMENUCHECK = 72,

        SLOWMACHINE = 73,

        MIDEASTENABLED = 74,

        MOUSEWHEELPRESENT = 75,

        XVIRTUALSCREEN = 76,

        YVIRTUALSCREEN = 77,

        CXVIRTUALSCREEN = 78,

        CYVIRTUALSCREEN = 79,

        CMONITORS = 80,

        SAMEDISPLAYFORMAT = 81,

        IMMENABLED = 82,

        CXFOCUSBORDER = 83,

        CYFOCUSBORDER = 84,

        TABLETPC = 86,

        MEDIACENTER = 87,

        REMOTESESSION = 0x1000,

        REMOTECONTROL = 0x2001,
    }

    internal enum SPI
    {
        GETBEEP = 0x0001,

        SETBEEP = 0x0002,

        GETMOUSE = 0x0003,

        SETMOUSE = 0x0004,

        GETBORDER = 0x0005,

        SETBORDER = 0x0006,

        GETKEYBOARDSPEED = 0x000A,

        SETKEYBOARDSPEED = 0x000B,

        LANGDRIVER = 0x000C,

        ICONHORIZONTALSPACING = 0x000D,

        GETSCREENSAVETIMEOUT = 0x000E,

        SETSCREENSAVETIMEOUT = 0x000F,

        GETSCREENSAVEACTIVE = 0x0010,

        SETSCREENSAVEACTIVE = 0x0011,

        GETGRIDGRANULARITY = 0x0012,

        SETGRIDGRANULARITY = 0x0013,

        SETDESKWALLPAPER = 0x0014,

        SETDESKPATTERN = 0x0015,

        GETKEYBOARDDELAY = 0x0016,

        SETKEYBOARDDELAY = 0x0017,

        ICONVERTICALSPACING = 0x0018,

        GETICONTITLEWRAP = 0x0019,

        SETICONTITLEWRAP = 0x001A,

        GETMENUDROPALIGNMENT = 0x001B,

        SETMENUDROPALIGNMENT = 0x001C,

        SETDOUBLECLKWIDTH = 0x001D,

        SETDOUBLECLKHEIGHT = 0x001E,

        GETICONTITLELOGFONT = 0x001F,

        SETDOUBLECLICKTIME = 0x0020,

        SETMOUSEBUTTONSWAP = 0x0021,

        SETICONTITLELOGFONT = 0x0022,

        GETFASTTASKSWITCH = 0x0023,

        SETFASTTASKSWITCH = 0x0024,

        SETDRAGFULLWINDOWS = 0x0025,

        GETDRAGFULLWINDOWS = 0x0026,

        GETNONCLIENTMETRICS = 0x0029,

        SETNONCLIENTMETRICS = 0x002A,

        GETMINIMIZEDMETRICS = 0x002B,

        SETMINIMIZEDMETRICS = 0x002C,

        GETICONMETRICS = 0x002D,

        SETICONMETRICS = 0x002E,

        SETWORKAREA = 0x002F,

        GETWORKAREA = 0x0030,

        SETPENWINDOWS = 0x0031,

        GETHIGHCONTRAST = 0x0042,

        SETHIGHCONTRAST = 0x0043,

        GETKEYBOARDPREF = 0x0044,

        SETKEYBOARDPREF = 0x0045,

        GETSCREENREADER = 0x0046,

        SETSCREENREADER = 0x0047,

        GETANIMATION = 0x0048,

        SETANIMATION = 0x0049,

        GETFONTSMOOTHING = 0x004A,

        SETFONTSMOOTHING = 0x004B,

        SETDRAGWIDTH = 0x004C,

        SETDRAGHEIGHT = 0x004D,

        SETHANDHELD = 0x004E,

        GETLOWPOWERTIMEOUT = 0x004F,

        GETPOWEROFFTIMEOUT = 0x0050,

        SETLOWPOWERTIMEOUT = 0x0051,

        SETPOWEROFFTIMEOUT = 0x0052,

        GETLOWPOWERACTIVE = 0x0053,

        GETPOWEROFFACTIVE = 0x0054,

        SETLOWPOWERACTIVE = 0x0055,

        SETPOWEROFFACTIVE = 0x0056,

        SETCURSORS = 0x0057,

        SETICONS = 0x0058,

        GETDEFAULTINPUTLANG = 0x0059,

        SETDEFAULTINPUTLANG = 0x005A,

        SETLANGTOGGLE = 0x005B,

        GETWINDOWSEXTENSION = 0x005C,

        SETMOUSETRAILS = 0x005D,

        GETMOUSETRAILS = 0x005E,

        SETSCREENSAVERRUNNING = 0x0061,

        SCREENSAVERRUNNING = SETSCREENSAVERRUNNING,

        GETFILTERKEYS = 0x0032,

        SETFILTERKEYS = 0x0033,

        GETTOGGLEKEYS = 0x0034,

        SETTOGGLEKEYS = 0x0035,

        GETMOUSEKEYS = 0x0036,

        SETMOUSEKEYS = 0x0037,

        GETSHOWSOUNDS = 0x0038,

        SETSHOWSOUNDS = 0x0039,

        GETSTICKYKEYS = 0x003A,

        SETSTICKYKEYS = 0x003B,

        GETACCESSTIMEOUT = 0x003C,

        SETACCESSTIMEOUT = 0x003D,

        GETSERIALKEYS = 0x003E,

        SETSERIALKEYS = 0x003F,

        GETSOUNDSENTRY = 0x0040,

        SETSOUNDSENTRY = 0x0041,

        GETSNAPTODEFBUTTON = 0x005F,

        SETSNAPTODEFBUTTON = 0x0060,

        GETMOUSEHOVERWIDTH = 0x0062,

        SETMOUSEHOVERWIDTH = 0x0063,

        GETMOUSEHOVERHEIGHT = 0x0064,

        SETMOUSEHOVERHEIGHT = 0x0065,

        GETMOUSEHOVERTIME = 0x0066,

        SETMOUSEHOVERTIME = 0x0067,

        GETWHEELSCROLLLINES = 0x0068,

        SETWHEELSCROLLLINES = 0x0069,

        GETMENUSHOWDELAY = 0x006A,

        SETMENUSHOWDELAY = 0x006B,

        GETWHEELSCROLLCHARS = 0x006C,

        SETWHEELSCROLLCHARS = 0x006D,

        GETSHOWIMEUI = 0x006E,

        SETSHOWIMEUI = 0x006F,

        GETMOUSESPEED = 0x0070,

        SETMOUSESPEED = 0x0071,

        GETSCREENSAVERRUNNING = 0x0072,

        GETDESKWALLPAPER = 0x0073,

        GETAUDIODESCRIPTION = 0x0074,

        SETAUDIODESCRIPTION = 0x0075,

        GETSCREENSAVESECURE = 0x0076,

        SETSCREENSAVESECURE = 0x0077,

        GETHUNGAPPTIMEOUT = 0x0078,

        SETHUNGAPPTIMEOUT = 0x0079,

        GETWAITTOKILLTIMEOUT = 0x007A,

        SETWAITTOKILLTIMEOUT = 0x007B,

        GETWAITTOKILLSERVICETIMEOUT = 0x007C,

        SETWAITTOKILLSERVICETIMEOUT = 0x007D,

        GETMOUSEDOCKTHRESHOLD = 0x007E,

        SETMOUSEDOCKTHRESHOLD = 0x007F,

        GETPENDOCKTHRESHOLD = 0x0080,

        SETPENDOCKTHRESHOLD = 0x0081,

        GETWINARRANGING = 0x0082,

        SETWINARRANGING = 0x0083,

        GETMOUSEDRAGOUTTHRESHOLD = 0x0084,

        SETMOUSEDRAGOUTTHRESHOLD = 0x0085,

        GETPENDRAGOUTTHRESHOLD = 0x0086,

        SETPENDRAGOUTTHRESHOLD = 0x0087,

        GETMOUSESIDEMOVETHRESHOLD = 0x0088,

        SETMOUSESIDEMOVETHRESHOLD = 0x0089,

        GETPENSIDEMOVETHRESHOLD = 0x008A,

        SETPENSIDEMOVETHRESHOLD = 0x008B,

        GETDRAGFROMMAXIMIZE = 0x008C,

        SETDRAGFROMMAXIMIZE = 0x008D,

        GETSNAPSIZING = 0x008E,

        SETSNAPSIZING = 0x008F,

        GETDOCKMOVING = 0x0090,

        SETDOCKMOVING = 0x0091,

        GETACTIVEWINDOWTRACKING = 0x1000,

        SETACTIVEWINDOWTRACKING = 0x1001,

        GETMENUANIMATION = 0x1002,

        SETMENUANIMATION = 0x1003,

        GETCOMBOBOXANIMATION = 0x1004,

        SETCOMBOBOXANIMATION = 0x1005,

        GETLISTBOXSMOOTHSCROLLING = 0x1006,

        SETLISTBOXSMOOTHSCROLLING = 0x1007,

        GETGRADIENTCAPTIONS = 0x1008,

        SETGRADIENTCAPTIONS = 0x1009,

        GETKEYBOARDCUES = 0x100A,

        SETKEYBOARDCUES = 0x100B,

        GETMENUUNDERLINES = GETKEYBOARDCUES,

        SETMENUUNDERLINES = SETKEYBOARDCUES,

        GETACTIVEWNDTRKZORDER = 0x100C,

        SETACTIVEWNDTRKZORDER = 0x100D,

        GETHOTTRACKING = 0x100E,

        SETHOTTRACKING = 0x100F,

        GETMENUFADE = 0x1012,

        SETMENUFADE = 0x1013,

        GETSELECTIONFADE = 0x1014,

        SETSELECTIONFADE = 0x1015,

        GETTOOLTIPANIMATION = 0x1016,

        SETTOOLTIPANIMATION = 0x1017,

        GETTOOLTIPFADE = 0x1018,

        SETTOOLTIPFADE = 0x1019,

        GETCURSORSHADOW = 0x101A,

        SETCURSORSHADOW = 0x101B,

        GETMOUSESONAR = 0x101C,

        SETMOUSESONAR = 0x101D,

        GETMOUSECLICKLOCK = 0x101E,

        SETMOUSECLICKLOCK = 0x101F,

        GETMOUSEVANISH = 0x1020,

        SETMOUSEVANISH = 0x1021,

        GETFLATMENU = 0x1022,

        SETFLATMENU = 0x1023,

        GETDROPSHADOW = 0x1024,

        SETDROPSHADOW = 0x1025,

        GETBLOCKSENDINPUTRESETS = 0x1026,

        SETBLOCKSENDINPUTRESETS = 0x1027,

        GETUIEFFECTS = 0x103E,

        SETUIEFFECTS = 0x103F,

        GETDISABLEOVERLAPPEDCONTENT = 0x1040,

        SETDISABLEOVERLAPPEDCONTENT = 0x1041,

        GETCLIENTAREAANIMATION = 0x1042,

        SETCLIENTAREAANIMATION = 0x1043,

        GETCLEARTYPE = 0x1048,

        SETCLEARTYPE = 0x1049,

        GETSPEECHRECOGNITION = 0x104A,

        SETSPEECHRECOGNITION = 0x104B,

        GETFOREGROUNDLOCKTIMEOUT = 0x2000,

        SETFOREGROUNDLOCKTIMEOUT = 0x2001,

        GETACTIVEWNDTRKTIMEOUT = 0x2002,

        SETACTIVEWNDTRKTIMEOUT = 0x2003,

        GETFOREGROUNDFLASHCOUNT = 0x2004,

        SETFOREGROUNDFLASHCOUNT = 0x2005,

        GETCARETWIDTH = 0x2006,

        SETCARETWIDTH = 0x2007,

        GETMOUSECLICKLOCKTIME = 0x2008,

        SETMOUSECLICKLOCKTIME = 0x2009,

        GETFONTSMOOTHINGTYPE = 0x200A,

        SETFONTSMOOTHINGTYPE = 0x200B,

        GETFONTSMOOTHINGCONTRAST = 0x200C,

        SETFONTSMOOTHINGCONTRAST = 0x200D,

        GETFOCUSBORDERWIDTH = 0x200E,

        SETFOCUSBORDERWIDTH = 0x200F,

        GETFOCUSBORDERHEIGHT = 0x2010,

        SETFOCUSBORDERHEIGHT = 0x2011,

        GETFONTSMOOTHINGORIENTATION = 0x2012,

        SETFONTSMOOTHINGORIENTATION = 0x2013,

        GETMINIMUMHITRADIUS = 0x2014,

        SETMINIMUMHITRADIUS = 0x2015,

        GETMESSAGEDURATION = 0x2016,

        SETMESSAGEDURATION = 0x2017,
    }

    [Flags]
    internal enum SPIF
    {
        None = 0,

        UPDATEINIFILE = 0x01,

        SENDCHANGE = 0x02,

        SENDWININICHANGE = SENDCHANGE,
    }

    [Flags]
    internal enum STATE_SYSTEM
    {
        UNAVAILABLE = 0x00000001,

        SELECTED = 0x00000002,

        FOCUSED = 0x00000004,

        PRESSED = 0x00000008,

        CHECKED = 0x00000010,

        MIXED = 0x00000020,

        INDETERMINATE = MIXED,

        READONLY = 0x00000040,

        HOTTRACKED = 0x00000080,

        DEFAULT = 0x00000100,

        EXPANDED = 0x00000200,

        COLLAPSED = 0x00000400,

        BUSY = 0x00000800,

        FLOATING = 0x00001000,

        MARQUEED = 0x00002000,

        ANIMATED = 0x00004000,

        INVISIBLE = 0x00008000,

        OFFSCREEN = 0x00010000,

        SIZEABLE = 0x00020000,

        MOVEABLE = 0x00040000,

        SELFVOICING = 0x00080000,

        FOCUSABLE = 0x00100000,

        SELECTABLE = 0x00200000,

        LINKED = 0x00400000,

        TRAVERSED = 0x00800000,

        MULTISELECTABLE = 0x01000000,

        EXTSELECTABLE = 0x02000000,

        ALERT_LOW = 0x04000000,

        ALERT_MEDIUM = 0x08000000,

        ALERT_HIGH = 0x10000000,

        PROTECTED = 0x20000000,

        VALID = 0x3FFFFFFF,
    }

    internal enum StockObject
    {
        WHITE_BRUSH = 0,

        LTGRAY_BRUSH = 1,

        GRAY_BRUSH = 2,

        DKGRAY_BRUSH = 3,

        BLACK_BRUSH = 4,

        NULL_BRUSH = 5,

        HOLLOW_BRUSH = NULL_BRUSH,

        WHITE_PEN = 6,

        BLACK_PEN = 7,

        NULL_PEN = 8,

        SYSTEM_FONT = 13,

        DEFAULT_PALETTE = 15,
    }

    [Flags]
    internal enum CS : uint
    {
        VREDRAW = 0x0001,

        HREDRAW = 0x0002,

        DBLCLKS = 0x0008,

        OWNDC = 0x0020,

        CLASSDC = 0x0040,

        PARENTDC = 0x0080,

        NOCLOSE = 0x0200,

        SAVEBITS = 0x0800,

        BYTEALIGNCLIENT = 0x1000,

        BYTEALIGNWINDOW = 0x2000,

        GLOBALCLASS = 0x4000,

        IME = 0x00010000,

        DROPSHADOW = 0x00020000
    }

    [Flags]
    internal enum WS : uint
    {
        OVERLAPPED = 0x00000000,

        POPUP = 0x80000000,

        CHILD = 0x40000000,

        MINIMIZE = 0x20000000,

        VISIBLE = 0x10000000,

        DISABLED = 0x08000000,

        CLIPSIBLINGS = 0x04000000,

        CLIPCHILDREN = 0x02000000,

        MAXIMIZE = 0x01000000,

        BORDER = 0x00800000,

        DLGFRAME = 0x00400000,

        VSCROLL = 0x00200000,

        HSCROLL = 0x00100000,

        SYSMENU = 0x00080000,

        THICKFRAME = 0x00040000,

        GROUP = 0x00020000,

        TABSTOP = 0x00010000,

        MINIMIZEBOX = 0x00020000,

        MAXIMIZEBOX = 0x00010000,

        CAPTION = BORDER | DLGFRAME,

        TILED = OVERLAPPED,

        ICONIC = MINIMIZE,

        SIZEBOX = THICKFRAME,

        TILEDWINDOW = OVERLAPPEDWINDOW,

        OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,

        POPUPWINDOW = POPUP | BORDER | SYSMENU,

        CHILDWINDOW = CHILD,
    }

    internal enum WM
    {
        NULL = 0x0000,

        CREATE = 0x0001,

        DESTROY = 0x0002,

        MOVE = 0x0003,

        SIZE = 0x0005,

        ACTIVATE = 0x0006,

        SETFOCUS = 0x0007,

        KILLFOCUS = 0x0008,

        ENABLE = 0x000A,

        SETREDRAW = 0x000B,

        SETTEXT = 0x000C,

        GETTEXT = 0x000D,

        GETTEXTLENGTH = 0x000E,

        PAINT = 0x000F,

        CLOSE = 0x0010,

        QUERYENDSESSION = 0x0011,

        QUIT = 0x0012,

        QUERYOPEN = 0x0013,

        ERASEBKGND = 0x0014,

        SYSCOLORCHANGE = 0x0015,

        SHOWWINDOW = 0x0018,

        CTLCOLOR = 0x0019,

        WININICHANGE = 0x001A,

        SETTINGCHANGE = 0x001A,

        ACTIVATEAPP = 0x001C,

        SETCURSOR = 0x0020,

        MOUSEACTIVATE = 0x0021,

        CHILDACTIVATE = 0x0022,

        QUEUESYNC = 0x0023,

        GETMINMAXINFO = 0x0024,

        WINDOWPOSCHANGING = 0x0046,

        WINDOWPOSCHANGED = 0x0047,

        CONTEXTMENU = 0x007B,

        STYLECHANGING = 0x007C,

        STYLECHANGED = 0x007D,

        DISPLAYCHANGE = 0x007E,

        GETICON = 0x007F,

        SETICON = 0x0080,

        NCCREATE = 0x0081,

        NCDESTROY = 0x0082,

        NCCALCSIZE = 0x0083,

        NCHITTEST = 0x0084,

        NCPAINT = 0x0085,

        NCACTIVATE = 0x0086,

        GETDLGCODE = 0x0087,

        SYNCPAINT = 0x0088,

        NCMOUSEMOVE = 0x00A0,

        NCLBUTTONDOWN = 0x00A1,

        NCLBUTTONUP = 0x00A2,

        NCLBUTTONDBLCLK = 0x00A3,

        NCRBUTTONDOWN = 0x00A4,

        NCRBUTTONUP = 0x00A5,

        NCRBUTTONDBLCLK = 0x00A6,

        NCMBUTTONDOWN = 0x00A7,

        NCMBUTTONUP = 0x00A8,

        NCMBUTTONDBLCLK = 0x00A9,

        SYSKEYDOWN = 0x0104,

        SYSKEYUP = 0x0105,

        SYSCHAR = 0x0106,

        SYSDEADCHAR = 0x0107,

        COMMAND = 0x0111,

        SYSCOMMAND = 0x0112,

        MOUSEMOVE = 0x0200,

        LBUTTONDOWN = 0x0201,

        LBUTTONUP = 0x0202,

        LBUTTONDBLCLK = 0x0203,

        RBUTTONDOWN = 0x0204,

        RBUTTONUP = 0x0205,

        RBUTTONDBLCLK = 0x0206,

        MBUTTONDOWN = 0x0207,

        MBUTTONUP = 0x0208,

        MBUTTONDBLCLK = 0x0209,

        MOUSEWHEEL = 0x020A,

        XBUTTONDOWN = 0x020B,

        XBUTTONUP = 0x020C,

        XBUTTONDBLCLK = 0x020D,

        MOUSEHWHEEL = 0x020E,

        PARENTNOTIFY = 0x0210,

        CAPTURECHANGED = 0x0215,

        POWERBROADCAST = 0x0218,

        DEVICECHANGE = 0x0219,

        ENTERSIZEMOVE = 0x0231,

        EXITSIZEMOVE = 0x0232,

        IME_SETCONTEXT = 0x0281,

        IME_NOTIFY = 0x0282,

        IME_CONTROL = 0x0283,

        IME_COMPOSITIONFULL = 0x0284,

        IME_SELECT = 0x0285,

        IME_CHAR = 0x0286,

        IME_REQUEST = 0x0288,

        IME_KEYDOWN = 0x0290,

        IME_KEYUP = 0x0291,

        NCMOUSELEAVE = 0x02A2,

        TABLET_DEFBASE = 0x02C0,

        TABLET_ADDED = TABLET_DEFBASE + 8,

        TABLET_DELETED = TABLET_DEFBASE + 9,

        TABLET_FLICK = TABLET_DEFBASE + 11,

        TABLET_QUERYSYSTEMGESTURESTATUS = TABLET_DEFBASE + 12,

        CUT = 0x0300,

        COPY = 0x0301,

        PASTE = 0x0302,

        CLEAR = 0x0303,

        UNDO = 0x0304,

        RENDERFORMAT = 0x0305,

        RENDERALLFORMATS = 0x0306,

        DESTROYCLIPBOARD = 0x0307,

        DRAWCLIPBOARD = 0x0308,

        PAINTCLIPBOARD = 0x0309,

        VSCROLLCLIPBOARD = 0x030A,

        SIZECLIPBOARD = 0x030B,

        ASKCBFORMATNAME = 0x030C,

        CHANGECBCHAIN = 0x030D,

        HSCROLLCLIPBOARD = 0x030E,

        QUERYNEWPALETTE = 0x030F,

        PALETTEISCHANGING = 0x0310,

        PALETTECHANGED = 0x0311,

        HOTKEY = 0x0312,

        PRINT = 0x0317,

        PRINTCLIENT = 0x0318,

        APPCOMMAND = 0x0319,

        THEMECHANGED = 0x031A,

        DWMCOMPOSITIONCHANGED = 0x031E,

        DWMNCRENDERINGCHANGED = 0x031F,

        DWMCOLORIZATIONCOLORCHANGED = 0x0320,

        DWMWINDOWMAXIMIZEDCHANGE = 0x0321,

        GETTITLEBARINFOEX = 0x033F,

        #region Windows 7

        DWMSENDICONICTHUMBNAIL = 0x0323,

        DWMSENDICONICLIVEPREVIEWBITMAP = 0x0326,

        #endregion

        USER = 0x0400,

        TRAYMOUSEMESSAGE = 0x800,

        APP = 0x8000,
    }

    [Flags]
    internal enum WS_EX : uint
    {
        None = 0,

        DLGMODALFRAME = 0x00000001,

        NOPARENTNOTIFY = 0x00000004,

        TOPMOST = 0x00000008,

        ACCEPTFILES = 0x00000010,

        TRANSPARENT = 0x00000020,

        MDICHILD = 0x00000040,

        TOOLWINDOW = 0x00000080,

        WINDOWEDGE = 0x00000100,

        CLIENTEDGE = 0x00000200,

        CONTEXTHELP = 0x00000400,

        RIGHT = 0x00001000,

        LEFT = 0x00000000,

        RTLREADING = 0x00002000,

        LTRREADING = 0x00000000,

        LEFTSCROLLBAR = 0x00004000,

        RIGHTSCROLLBAR = 0x00000000,

        CONTROLPARENT = 0x00010000,

        STATICEDGE = 0x00020000,

        APPWINDOW = 0x00040000,

        LAYERED = 0x00080000,

        NOINHERITLAYOUT = 0x00100000,

        LAYOUTRTL = 0x00400000,

        COMPOSITED = 0x02000000,

        NOACTIVATE = 0x08000000,

        OVERLAPPEDWINDOW = (WINDOWEDGE | CLIENTEDGE),

        PALETTEWINDOW = (WINDOWEDGE | TOOLWINDOW | TOPMOST),
    }

    internal enum DeviceCap
    {
        BITSPIXEL = 12,

        PLANES = 14,

        LOGPIXELSX = 88,

        LOGPIXELSY = 90,
    }

    internal enum FO
    {
        MOVE = 0x0001,

        COPY = 0x0002,

        DELETE = 0x0003,

        RENAME = 0x0004,
    }

    internal enum FOF : ushort
    {
        MULTIDESTFILES = 0x0001,

        CONFIRMMOUSE = 0x0002,

        SILENT = 0x0004,

        RENAMEONCOLLISION = 0x0008,

        NOCONFIRMATION = 0x0010,

        WANTMAPPINGHANDLE = 0x0020,

        ALLOWUNDO = 0x0040,

        FILESONLY = 0x0080,

        SIMPLEPROGRESS = 0x0100,

        NOCONFIRMMKDIR = 0x0200,

        NOERRORUI = 0x0400,

        NOCOPYSECURITYATTRIBS = 0x0800,

        NORECURSION = 0x1000,

        NO_CONNECTED_ELEMENTS = 0x2000,

        WANTNUKEWARNING = 0x4000,

        NORECURSEREPARSE = 0x8000,
    }

    [Flags]
    internal enum MF : uint
    {
        DOES_NOT_EXIST = unchecked((uint)-1),

        ENABLED = 0,

        BYCOMMAND = 0,

        GRAYED = 1,

        DISABLED = 2,
    }

    internal enum WINDOWTHEMEATTRIBUTETYPE : uint
    {
        WTA_NONCLIENT = 1,
    }

    internal enum DWMFLIP3D
    {
        DEFAULT,

        EXCLUDEBELOW,

        EXCLUDEABOVE,
    }

    internal enum DWMNCRP
    {
        USEWINDOWSTYLE,

        DISABLED,

        ENABLED,
    }

    internal enum DWMWA
    {
        NCRENDERING_ENABLED = 1,

        NCRENDERING_POLICY,

        TRANSITIONS_FORCEDISABLED,

        ALLOW_NCPAINT,

        CAPTION_BUTTON_BOUNDS,

        NONCLIENT_RTL_LAYOUT,

        FORCE_ICONIC_REPRESENTATION,

        FLIP3D_POLICY,

        EXTENDED_FRAME_BOUNDS,

        HAS_ICONIC_BITMAP,

        DISALLOW_PEEK,

        EXCLUDED_FROM_PEEK,
    }

    [Flags]
    internal enum WTNCA : uint
    {
        NODRAWCAPTION = 0x00000001,

        NODRAWICON = 0x00000002,

        NOSYSMENU = 0x00000004,

        NOMIRRORHELP = 0x00000008,

        VALIDBITS = NODRAWCAPTION | NODRAWICON | NOMIRRORHELP | NOSYSMENU,
    }

    [Flags]
    internal enum SWP
    {
        ASYNCWINDOWPOS = 0x4000,

        DEFERERASE = 0x2000,

        DRAWFRAME = 0x0020,

        FRAMECHANGED = 0x0020,

        HIDEWINDOW = 0x0080,

        NOACTIVATE = 0x0010,

        NOCOPYBITS = 0x0100,

        NOMOVE = 0x0002,

        NOOWNERZORDER = 0x0200,

        NOREDRAW = 0x0008,

        NOREPOSITION = 0x0200,

        NOSENDCHANGING = 0x0400,

        NOSIZE = 0x0001,

        NOZORDER = 0x0004,

        SHOWWINDOW = 0x0040,
    }

    internal enum SW
    {
        HIDE = 0,

        SHOWNORMAL = 1,

        NORMAL = 1,

        SHOWMINIMIZED = 2,

        SHOWMAXIMIZED = 3,

        MAXIMIZE = 3,

        SHOWNOACTIVATE = 4,

        SHOW = 5,

        MINIMIZE = 6,

        SHOWMINNOACTIVE = 7,

        SHOWNA = 8,

        RESTORE = 9,

        SHOWDEFAULT = 10,

        FORCEMINIMIZE = 11,
    }

    internal enum SC
    {
        SIZE = 0xF000,

        MOVE = 0xF010,

        MINIMIZE = 0xF020,

        MAXIMIZE = 0xF030,

        NEXTWINDOW = 0xF040,

        PREVWINDOW = 0xF050,

        CLOSE = 0xF060,

        VSCROLL = 0xF070,

        HSCROLL = 0xF080,

        MOUSEMENU = 0xF090,

        KEYMENU = 0xF100,

        ARRANGE = 0xF110,

        RESTORE = 0xF120,

        TASKLIST = 0xF130,

        SCREENSAVE = 0xF140,

        HOTKEY = 0xF150,

        DEFAULT = 0xF160,

        MONITORPOWER = 0xF170,

        CONTEXTHELP = 0xF180,

        SEPARATOR = 0xF00F,

        F_ISSECURE = 0x00000001,

        ICON = MINIMIZE,

        ZOOM = MAXIMIZE,
    }

    internal enum Status
    {
        Ok = 0,

        GenericError = 1,

        InvalidParameter = 2,

        OutOfMemory = 3,

        ObjectBusy = 4,

        InsufficientBuffer = 5,

        NotImplemented = 6,

        Win32Error = 7,

        WrongState = 8,

        Aborted = 9,

        FileNotFound = 10,

        ValueOverflow = 11,

        AccessDenied = 12,

        UnknownImageFormat = 13,

        FontFamilyNotFound = 14,

        FontStyleNotFound = 15,

        NotTrueTypeFont = 16,

        UnsupportedGdiplusVersion = 17,

        GdiplusNotInitialized = 18,

        PropertyNotFound = 19,

        PropertyNotSupported = 20,

        ProfileNotFound = 21,
    }

    internal enum MOUSEEVENTF
    {
        LEFTDOWN = 2,

        LEFTUP = 4
    }

    internal enum MSGFLT
    {
        RESET = 0,

        ALLOW = 1,

        DISALLOW = 2,
    }

    internal enum MSGFLTINFO
    {
        NONE = 0,

        ALREADYALLOWED_FORWND = 1,

        ALREADYDISALLOWED_FORWND = 2,

        ALLOWED_HIGHER = 3,
    }

    internal enum INPUT_TYPE : uint
    {
        MOUSE = 0,
    }

    internal enum NIM : uint
    {
        ADD = 0,

        MODIFY = 1,

        DELETE = 2,

        SETFOCUS = 3,

        SETVERSION = 4,
    }

    internal enum SHARD
    {
        PIDL = 0x00000001,

        PATHA = 0x00000002,

        PATHW = 0x00000003,

        APPIDINFO = 0x00000004,

        APPIDINFOIDLIST = 0x00000005,

        LINK = 0x00000006,

        APPIDINFOLINK = 0x00000007,
    }

    [Flags]
    internal enum SLGP
    {
        SHORTPATH = 0x1,

        UNCPRIORITY = 0x2,

        RAWPATH = 0x4
    }

    [Flags]
    internal enum NIF : uint
    {
        MESSAGE = 0x0001,

        ICON = 0x0002,

        TIP = 0x0004,

        STATE = 0x0008,

        INFO = 0x0010,

        GUID = 0x0020,

        REALTIME = 0x0040,

        SHOWTIP = 0x0080,

        XP_MASK = MESSAGE | ICON | STATE | INFO | GUID,

        VISTA_MASK = XP_MASK | REALTIME | SHOWTIP,
    }

    internal enum NIIF
    {
        NONE = 0x00000000,

        INFO = 0x00000001,

        WARNING = 0x00000002,

        ERROR = 0x00000003,

        USER = 0x00000004,

        NOSOUND = 0x00000010,

        LARGE_ICON = 0x00000020,

        NIIF_RESPECT_QUIET_TIME = 0x00000080,

        XP_ICON_MASK = 0x0000000F,
    }

    internal enum AC : byte
    {
        SRC_OVER = 0,

        SRC_ALPHA = 1,
    }

    internal enum ULW
    {
        ALPHA = 2,

        COLORKEY = 1,

        OPAQUE = 4,
    }

    internal enum WVR
    {
        ALIGNTOP = 0x0010,

        ALIGNLEFT = 0x0020,

        ALIGNBOTTOM = 0x0040,

        ALIGNRIGHT = 0x0080,

        HREDRAW = 0x0100,

        VREDRAW = 0x0200,

        VALIDRECTS = 0x0400,

        REDRAW = HREDRAW | VREDRAW,
    }

    internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region Конструктор

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        private SafeFindHandle() : base(true)
        {
        }

        #endregion

        #region Методы

        protected override bool ReleaseHandle()
        {
            return NativeMethods.FindClose(handle);
        }

        #endregion
    }

    internal sealed class SafeDC : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region Поля

        private bool _created;

        private IntPtr? _hwnd;

        #endregion

        #region Свойства

        public IntPtr Hwnd
        {
            set
            {
                Assert.NullableIsNull(_hwnd);
                _hwnd = value;
            }
        }

        #endregion

        #region Конструктор

        private SafeDC() : base(true)
        {
        }

        #endregion

        #region Методы

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            if (_created)
            {
                return NativeMethods.DeleteDC(handle);
            }

            if (!_hwnd.HasValue || _hwnd.Value == IntPtr.Zero)
            {
                return true;
            }

            return NativeMethods.ReleaseDC(_hwnd.Value, handle) == 1;
        }

        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static SafeDC CreateDC(string deviceName)
        {
            SafeDC dc = null;
            try
            {
                dc = NativeMethods.CreateDC(deviceName, null, IntPtr.Zero, IntPtr.Zero);
            }
            finally
            {
                if (dc != null)
                {
                    dc._created = true;
                }
            }

            if (dc.IsInvalid)
            {
                dc.Dispose();
                throw new SystemException("Unable to create a device context from the specified device information.");
            }

            return dc;
        }

        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static SafeDC CreateCompatibleDC(SafeDC hdc)
        {
            SafeDC dc = null;
            try
            {
                var hPtr = IntPtr.Zero;
                if (hdc != null)
                {
                    hPtr = hdc.handle;
                }
                dc = NativeMethods.CreateCompatibleDC(hPtr);
                if (dc == null)
                {
                    HRESULT.ThrowLastError();
                }
            }
            finally
            {
                if (dc != null)
                {
                    dc._created = true;
                }
            }

            if (dc.IsInvalid)
            {
                dc.Dispose();
                throw new SystemException("Unable to create a device context from the specified device information.");
            }

            return dc;
        }

        public static SafeDC GetDC(IntPtr hwnd)
        {
            SafeDC dc = null;
            try
            {
                dc = NativeMethods.GetDC(hwnd);
            }
            finally
            {
                if (dc != null)
                {
                    dc.Hwnd = hwnd;
                }
            }

            if (dc.IsInvalid)
            {
                HRESULT.E_FAIL.ThrowIfFailed();
            }

            return dc;
        }

        public static SafeDC GetDesktop()
        {
            return GetDC(IntPtr.Zero);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static SafeDC WrapDC(IntPtr hdc)
        {
            return new SafeDC
            {
                handle = hdc,
                _created = false,
                _hwnd = IntPtr.Zero,
            };
        }

        #endregion

        #region Вложенный класс: NativeMethods

        private static class NativeMethods
        {
            #region Методы

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [DllImport("user32.dll")]
            public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [DllImport("user32.dll")]
            public static extern SafeDC GetDC(IntPtr hwnd);

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
            public static extern SafeDC CreateDC([MarshalAs(UnmanagedType.LPWStr)] string lpszDriver, [MarshalAs(UnmanagedType.LPWStr)] string lpszDevice, IntPtr lpszOutput, IntPtr lpInitData);

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern SafeDC CreateCompatibleDC(IntPtr hdc);

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteDC(IntPtr hdc);

            #endregion
        }

        #endregion
    }

    internal sealed class SafeHBITMAP : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region Конструктор

        private SafeHBITMAP() : base(true)
        {
        }

        #endregion

        #region Методы

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.DeleteObject(handle);
        }

        #endregion
    }

    internal sealed class SafeGdiplusStartupToken : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region Конструктор

        private SafeGdiplusStartupToken() : base(true)
        {
        }

        #endregion

        #region Методы

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            var s = NativeMethods.GdiplusShutdown(handle);
            return s == Status.Ok;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        public static SafeGdiplusStartupToken Startup()
        {
            var safeHandle = new SafeGdiplusStartupToken();
            IntPtr unsafeHandle;
            StartupOutput output;
            var s = NativeMethods.GdiplusStartup(out unsafeHandle, new StartupInput(), out output);
            if (s == Status.Ok)
            {
                safeHandle.handle = unsafeHandle;
                return safeHandle;
            }
            safeHandle.Dispose();
            throw new Exception("Unable to initialize GDI+");
        }

        #endregion
    }

    internal sealed class SafeConnectionPointCookie : SafeHandleZeroOrMinusOneIsInvalid
    {
        #region Поля

        private IConnectionPoint _cp;

        #endregion

        #region Конструктор

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IConnectionPoint")]
        public SafeConnectionPointCookie(IConnectionPointContainer target, object sink, Guid eventId)
            : base(true)
        {
            Verify.IsNotNull(target, "target");
            Verify.IsNotNull(sink, "sink");
            Verify.IsNotDefault(eventId, "eventId");

            handle = IntPtr.Zero;

            IConnectionPoint cp = null;
            try
            {
                int dwCookie;
                target.FindConnectionPoint(ref eventId, out cp);
                cp.Advise(sink, out dwCookie);
                if (dwCookie == 0)
                {
                    throw new InvalidOperationException("IConnectionPoint::Advise returned an invalid cookie.");
                }
                handle = new IntPtr(dwCookie);
                _cp = cp;
                cp = null;
            }
            finally
            {
                Utility.SafeRelease(ref cp);
            }
        }

        #endregion

        #region Методы

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void Disconnect()
        {
            ReleaseHandle();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            try
            {
                if (!IsInvalid)
                {
                    int dwCookie = handle.ToInt32();
                    handle = IntPtr.Zero;

                    Assert.IsNotNull(_cp);
                    try
                    {
                        _cp.Unadvise(dwCookie);
                    }
                    finally
                    {
                        Utility.SafeRelease(ref _cp);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BLENDFUNCTION
    {
        public AC BlendOp;

        public byte BlendFlags;

        public byte SourceConstantAlpha;

        public AC AlphaFormat;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct HIGHCONTRAST
    {
        public int cbSize;

        public HCF dwFlags;

        public IntPtr lpszDefaultScheme;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RGBQUAD
    {
        public byte rgbBlue;

        public byte rgbGreen;

        public byte rgbRed;

        public byte rgbReserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct BITMAPINFOHEADER
    {
        public int biSize;

        public int biWidth;

        public int biHeight;

        public short biPlanes;

        public short biBitCount;

        public BI biCompression;

        public int biSizeImage;

        public int biXPelsPerMeter;

        public int biYPelsPerMeter;

        public int biClrUsed;

        public int biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;

        public RGBQUAD bmiColors;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CHANGEFILTERSTRUCT
    {
        public uint cbSize;

        public MSGFLTINFO ExtStatus;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CREATESTRUCT
    {
        public IntPtr lpCreateParams;

        public IntPtr hInstance;

        public IntPtr hMenu;

        public IntPtr hwndParent;

        public int cy;

        public int cx;

        public int y;

        public int x;

        public WS style;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClass;

        public WS_EX dwExStyle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    internal struct SHFILEOPSTRUCT
    {
        public IntPtr hwnd;

        [MarshalAs(UnmanagedType.U4)]
        public FO wFunc;

        public string pFrom;

        public string pTo;

        [MarshalAs(UnmanagedType.U2)]
        public FOF fFlags;

        [MarshalAs(UnmanagedType.Bool)]
        public int fAnyOperationsAborted;

        public IntPtr hNameMappings;

        public string lpszProgressTitle;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TITLEBARINFO
    {
        public int cbSize;

        public RECT rcTitleBar;

        public STATE_SYSTEM rgstate_TitleBar;

        public STATE_SYSTEM rgstate_Reserved;

        public STATE_SYSTEM rgstate_MinimizeButton;

        public STATE_SYSTEM rgstate_MaximizeButton;

        public STATE_SYSTEM rgstate_HelpButton;

        public STATE_SYSTEM rgstate_CloseButton;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TITLEBARINFOEX
    {
        public int cbSize;

        public RECT rcTitleBar;

        public STATE_SYSTEM rgstate_TitleBar;

        public STATE_SYSTEM rgstate_Reserved;

        public STATE_SYSTEM rgstate_MinimizeButton;

        public STATE_SYSTEM rgstate_MaximizeButton;

        public STATE_SYSTEM rgstate_HelpButton;

        public STATE_SYSTEM rgstate_CloseButton;

        public RECT rgrect_TitleBar;

        public RECT rgrect_Reserved;

        public RECT rgrect_MinimizeButton;

        public RECT rgrect_MaximizeButton;

        public RECT rgrect_HelpButton;

        public RECT rgrect_CloseButton;
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [StructLayout(LayoutKind.Sequential)]
    internal class NOTIFYICONDATA
    {
        public int cbSize;

        public IntPtr hWnd;

        public int uID;

        public NIF uFlags;

        public int uCallbackMessage;

        public IntPtr hIcon;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public char[] szTip = new char[128];

        public uint dwState;

        public uint dwStateMask;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public char[] szInfo = new char[256];

        public uint uVersion;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] szInfoTitle = new char[64];

        public uint dwInfoFlags;

        public Guid guidItem;

        private IntPtr hBalloonIcon;
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [StructLayout(LayoutKind.Explicit)]
    internal class PROPVARIANT : IDisposable
    {
        private static class NativeMethods
        {
            #region Методы

            [DllImport("ole32.dll")]
            internal static extern HRESULT PropVariantClear(PROPVARIANT pvar);

            #endregion
        }

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [FieldOffset(0)]
        private ushort vt;

        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [FieldOffset(8)]
        private IntPtr pointerVal;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [FieldOffset(8)]
        private byte byteVal;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [FieldOffset(8)]
        private long longVal;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        [FieldOffset(8)]
        private short boolVal;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public VarEnum VarType
        {
            get { return (VarEnum)vt; }
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public string GetValue()
        {
            if (vt == (ushort)VarEnum.VT_LPWSTR)
            {
                return Marshal.PtrToStringUni(pointerVal);
            }

            return null;
        }

        public void SetValue(bool f)
        {
            Clear();
            vt = (ushort)VarEnum.VT_BOOL;
            boolVal = (short)(f ? -1 : 0);
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void SetValue(string val)
        {
            Clear();
            vt = (ushort)VarEnum.VT_LPWSTR;
            pointerVal = Marshal.StringToCoTaskMemUni(val);
        }

        public void Clear()
        {
            var hr = NativeMethods.PropVariantClear(this);
            Assert.IsTrue(hr.Succeeded);
        }

        #region IDisposable Pattern

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PROPVARIANT()
        {
            Dispose(false);
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing")]
        private void Dispose(bool disposing)
        {
            Clear();
        }

        #endregion
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal class SHARDAPPIDINFO
    {
        [MarshalAs(UnmanagedType.Interface)]
        private object psi;

        [MarshalAs(UnmanagedType.LPWStr)]
        private string pszAppID;
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal class SHARDAPPIDINFOIDLIST
    {
        private IntPtr pidl;

        [MarshalAs(UnmanagedType.LPWStr)]
        private string pszAppID;
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal class SHARDAPPIDINFOLINK
    {
        private IntPtr psl;

        [MarshalAs(UnmanagedType.LPWStr)]
        private string pszAppID;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct LOGFONT
    {
        public int lfHeight;

        public int lfWidth;

        public int lfEscapement;

        public int lfOrientation;

        public int lfWeight;

        public byte lfItalic;

        public byte lfUnderline;

        public byte lfStrikeOut;

        public byte lfCharSet;

        public byte lfOutPrecision;

        public byte lfClipPrecision;

        public byte lfQuality;

        public byte lfPitchAndFamily;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string lfFaceName;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MINMAXINFO
    {
        public POINT ptReserved;

        public POINT ptMaxSize;

        public POINT ptMaxPosition;

        public POINT ptMinTrackSize;

        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NONCLIENTMETRICS
    {
        public int cbSize;

        public int iBorderWidth;

        public int iScrollWidth;

        public int iScrollHeight;

        public int iCaptionWidth;

        public int iCaptionHeight;

        public LOGFONT lfCaptionFont;

        public int iSmCaptionWidth;

        public int iSmCaptionHeight;

        public LOGFONT lfSmCaptionFont;

        public int iMenuWidth;

        public int iMenuHeight;

        public LOGFONT lfMenuFont;

        public LOGFONT lfStatusFont;

        public LOGFONT lfMessageFont;

        public int iPaddedBorderWidth;

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static NONCLIENTMETRICS VistaMetricsStruct
        {
            get
            {
                var ncm = new NONCLIENTMETRICS();
                ncm.cbSize = Marshal.SizeOf(typeof(NONCLIENTMETRICS));
                return ncm;
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static NONCLIENTMETRICS XPMetricsStruct
        {
            get
            {
                var ncm = new NONCLIENTMETRICS();

                ncm.cbSize = Marshal.SizeOf(typeof(NONCLIENTMETRICS)) - sizeof (int);
                return ncm;
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct WTA_OPTIONS
    {
        public const uint Size = 8;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used by native code.")]
        [FieldOffset(0)]
        public WTNCA dwFlags;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Used by native code.")]
        [FieldOffset(4)]
        public WTNCA dwMask;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MARGINS
    {
        public int cxLeftWidth;

        public int cxRightWidth;

        public int cyTopHeight;

        public int cyBottomHeight;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

        public RECT rcMonitor;

        public RECT rcWork;

        public int dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;

        public int y;
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [StructLayout(LayoutKind.Sequential)]
    internal class RefPOINT
    {
        public int x;

        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        private int _left;

        private int _top;

        private int _right;

        private int _bottom;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void Offset(int dx, int dy)
        {
            _left += dx;
            _top += dy;
            _right += dx;
            _bottom += dy;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Right
        {
            get { return _right; }
            set { _right = value; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Top
        {
            get { return _top; }
            set { _top = value; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Width
        {
            get { return _right - _left; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Height
        {
            get { return _bottom - _top; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public POINT Position
        {
            get
            {
                return new POINT
                {
                    x = _left,
                    y = _top
                };
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public SIZE Size
        {
            get
            {
                return new SIZE
                {
                    cx = Width,
                    cy = Height
                };
            }
        }

        public static RECT Union(RECT rect1, RECT rect2)
        {
            return new RECT
            {
                Left = Math.Min(rect1.Left, rect2.Left),
                Top = Math.Min(rect1.Top, rect2.Top),
                Right = Math.Max(rect1.Right, rect2.Right),
                Bottom = Math.Max(rect1.Bottom, rect2.Bottom),
            };
        }

        public override bool Equals(object obj)
        {
            try
            {
                var rc = (RECT)obj;
                return rc._bottom == _bottom
                       && rc._left == _left
                       && rc._right == _right
                       && rc._top == _top;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (_left << 16 | Utility.Loword(_right)) ^ (_top << 16 | Utility.Loword(_bottom));
        }
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [StructLayout(LayoutKind.Sequential)]
    internal class RefRECT
    {
        private int _left;

        private int _top;

        private int _right;

        private int _bottom;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public RefRECT(int left, int top, int right, int bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Width
        {
            get { return _right - _left; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Height
        {
            get { return _bottom - _top; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Right
        {
            get { return _right; }
            set { _right = value; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Top
        {
            get { return _top; }
            set { _top = value; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public int Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void Offset(int dx, int dy)
        {
            _left += dx;
            _top += dy;
            _right += dx;
            _bottom += dy;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SIZE
    {
        public int cx;

        public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StartupOutput
    {
        public IntPtr hook;

        public IntPtr unhook;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class StartupInput
    {
        public int GdiplusVersion = 1;

        public IntPtr DebugEventCallback;

        public bool SuppressBackgroundThread;

        public bool SuppressExternalCodecs;
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [BestFitMapping(false)]
    internal class WIN32_FIND_DATAW
    {
        public FileAttributes dwFileAttributes;

        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;

        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;

        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;

        public int nFileSizeHigh;

        public int nFileSizeLow;

        public int dwReserved0;

        public int dwReserved1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class WINDOWPLACEMENT
    {
        public int length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));

        public int flags;

        public SW showCmd;

        public POINT ptMinPosition;

        public POINT ptMaxPosition;

        public RECT rcNormalPosition;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWPOS
    {
        public IntPtr hwnd;

        public IntPtr hwndInsertAfter;

        public int x;

        public int y;

        public int cx;

        public int cy;

        public int flags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WNDCLASSEX
    {
        public int cbSize;

        public CS style;

        public WndProc lpfnWndProc;

        public int cbClsExtra;

        public int cbWndExtra;

        public IntPtr hInstance;

        public IntPtr hIcon;

        public IntPtr hCursor;

        public IntPtr hbrBackground;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;

        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT
    {
        public int dx;

        public int dy;

        public int mouseData;

        public int dwFlags;

        public int time;

        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct INPUT
    {
        public uint type;

        public MOUSEINPUT mi;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct UNSIGNED_RATIO
    {
        public uint uiNumerator;

        public uint uiDenominator;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DWM_TIMING_INFO
    {
        public int cbSize;

        public UNSIGNED_RATIO rateRefresh;

        public ulong qpcRefreshPeriod;

        public UNSIGNED_RATIO rateCompose;

        public ulong qpcVBlank;

        public ulong cRefresh;

        public uint cDXRefresh;

        public ulong qpcCompose;

        public ulong cFrame;

        public uint cDXPresent;

        public ulong cRefreshFrame;

        public ulong cFrameSubmitted;

        public uint cDXPresentSubmitted;

        public ulong cFrameConfirmed;

        public uint cDXPresentConfirmed;

        public ulong cRefreshConfirmed;

        public uint cDXRefreshConfirmed;

        public ulong cFramesLate;

        public uint cFramesOutstanding;

        public ulong cFrameDisplayed;

        public ulong qpcFrameDisplayed;

        public ulong cRefreshFrameDisplayed;

        public ulong cFrameComplete;

        public ulong qpcFrameComplete;

        public ulong cFramePending;

        public ulong qpcFramePending;

        public ulong cFramesDisplayed;

        public ulong cFramesComplete;

        public ulong cFramesPending;

        public ulong cFramesAvailable;

        public ulong cFramesDropped;

        public ulong cFramesMissed;

        public ulong cRefreshNextDisplayed;

        public ulong cRefreshNextPresented;

        public ulong cRefreshesDisplayed;

        public ulong cRefreshesPresented;

        public ulong cRefreshStarted;

        public ulong cPixelsReceived;

        public ulong cPixelsDrawn;

        public ulong cBuffersEmpty;
    }

    internal delegate IntPtr WndProc(IntPtr hwnd, WM uMsg, IntPtr wParam, IntPtr lParam);

    internal delegate IntPtr WndProcHook(IntPtr hwnd, WM uMsg, IntPtr wParam, IntPtr lParam, ref bool handled);

    internal delegate IntPtr MessageHandler(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled);

    internal static class NativeMethods
    {
        #region Методы

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "AdjustWindowRectEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _AdjustWindowRectEx(ref RECT lpRect, WS dwStyle, [MarshalAs(UnmanagedType.Bool)] bool bMenu, WS_EX dwExStyle);

        public static RECT AdjustWindowRectEx(RECT lpRect, WS dwStyle, bool bMenu, WS_EX dwExStyle)
        {
            if (!_AdjustWindowRectEx(ref lpRect, dwStyle, bMenu, dwExStyle))
            {
                HRESULT.ThrowLastError();
            }

            return lpRect;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "ChangeWindowMessageFilter", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _ChangeWindowMessageFilter(WM message, MSGFLT dwFlag);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "ChangeWindowMessageFilterEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _ChangeWindowMessageFilterEx(IntPtr hwnd, WM message, MSGFLT action, [In, Out, Optional] ref CHANGEFILTERSTRUCT pChangeFilterStruct);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static HRESULT ChangeWindowMessageFilterEx(IntPtr hwnd, WM message, MSGFLT action, out MSGFLTINFO filterInfo)
        {
            filterInfo = MSGFLTINFO.NONE;

            bool ret;

            if (!Utility.IsOsVistaOrNewer)
            {
                return HRESULT.S_FALSE;
            }

            if (!Utility.IsOsWindows7OrNewer)
            {
                ret = _ChangeWindowMessageFilter(message, action);
                if (!ret)
                {
                    return (HRESULT)Win32Error.GetLastError();
                }
                return HRESULT.S_OK;
            }

            var filterstruct = new CHANGEFILTERSTRUCT
            {
                cbSize = (uint)Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT))
            };
            ret = _ChangeWindowMessageFilterEx(hwnd, message, action, ref filterstruct);
            if (!ret)
            {
                return (HRESULT)Win32Error.GetLastError();
            }

            filterInfo = filterstruct.ExtStatus;
            return HRESULT.S_OK;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll")]
        public static extern CombineRgnResult CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, RGN fnCombineMode);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("shell32.dll", EntryPoint = "CommandLineToArgvW", CharSet = CharSet.Unicode)]
        private static extern IntPtr _CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string cmdLine, out int numArgs);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string[] CommandLineToArgvW(string cmdLine)
        {
            var argv = IntPtr.Zero;
            try
            {
                int numArgs = 0;

                argv = _CommandLineToArgvW(cmdLine, out numArgs);
                if (argv == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
                var result = new string[numArgs];

                for (int i = 0; i < numArgs; i++)
                {
                    var currArg = Marshal.ReadIntPtr(argv, i * Marshal.SizeOf(typeof(IntPtr)));
                    result[i] = Marshal.PtrToStringUni(currArg);
                }

                return result;
            }
            finally
            {
                var p = _LocalFree(argv);

                Assert.AreEqual(IntPtr.Zero, p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll", EntryPoint = "CreateDIBSection", SetLastError = true)]
        private static extern SafeHBITMAP _CreateDIBSection(SafeDC hdc, [In] ref BITMAPINFO bitmapInfo, int iUsage, [Out] out IntPtr ppvBits, IntPtr hSection, int dwOffset);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll", EntryPoint = "CreateDIBSection", SetLastError = true)]
        private static extern SafeHBITMAP _CreateDIBSectionIntPtr(IntPtr hdc, [In] ref BITMAPINFO bitmapInfo, int iUsage, [Out] out IntPtr ppvBits, IntPtr hSection, int dwOffset);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static SafeHBITMAP CreateDIBSection(SafeDC hdc, ref BITMAPINFO bitmapInfo, out IntPtr ppvBits, IntPtr hSection, int dwOffset)
        {
            const int DIB_RGB_COLORS = 0;
            SafeHBITMAP hBitmap = null;
            if (hdc == null)
            {
                hBitmap = _CreateDIBSectionIntPtr(IntPtr.Zero, ref bitmapInfo, DIB_RGB_COLORS, out ppvBits, hSection, dwOffset);
            }
            else
            {
                hBitmap = _CreateDIBSection(hdc, ref bitmapInfo, DIB_RGB_COLORS, out ppvBits, hSection, dwOffset);
            }

            if (hBitmap.IsInvalid)
            {
                HRESULT.ThrowLastError();
            }

            return hBitmap;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn", SetLastError = true)]
        private static extern IntPtr _CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse)
        {
            var ret = _CreateRoundRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect, nWidthEllipse, nHeightEllipse);
            if (IntPtr.Zero == ret)
            {
                throw new Win32Exception();
            }
            return ret;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll", EntryPoint = "CreateRectRgn", SetLastError = true)]
        private static extern IntPtr _CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect)
        {
            var ret = _CreateRectRgn(nLeftRect, nTopRect, nRightRect, nBottomRect);
            if (IntPtr.Zero == ret)
            {
                throw new Win32Exception();
            }
            return ret;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll", EntryPoint = "CreateRectRgnIndirect", SetLastError = true)]
        private static extern IntPtr _CreateRectRgnIndirect([In] ref RECT lprc);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr CreateRectRgnIndirect(RECT lprc)
        {
            var ret = _CreateRectRgnIndirect(ref lprc);
            if (IntPtr.Zero == ret)
            {
                throw new Win32Exception();
            }
            return ret;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(int crColor);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW")]
        private static extern IntPtr _CreateWindowEx(
            WS_EX dwExStyle,
            [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            WS dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr CreateWindowEx(
            WS_EX dwExStyle,
            string lpClassName,
            string lpWindowName,
            WS dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam)
        {
            var ret = _CreateWindowEx(dwExStyle, lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
            if (IntPtr.Zero == ret)
            {
                HRESULT.ThrowLastError();
            }

            return ret;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "DefWindowProcW")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr handle);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hwnd);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll", EntryPoint = "DwmIsCompositionEnabled", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _DwmIsCompositionEnabled();

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll", EntryPoint = "DwmGetColorizationColor", PreserveSig = true)]
        private static extern HRESULT _DwmGetColorizationColor(out uint pcrColorization, [Out, MarshalAs(UnmanagedType.Bool)] out bool pfOpaqueBlend);

        public static bool DwmGetColorizationColor(out uint pcrColorization, out bool pfOpaqueBlend)
        {
            if (Utility.IsOsVistaOrNewer && IsThemeActive())
            {
                var hr = _DwmGetColorizationColor(out pcrColorization, out pfOpaqueBlend);
                if (hr.Succeeded)
                {
                    return true;
                }
            }

            pcrColorization = 0xFF000000;
            pfOpaqueBlend = true;

            return false;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool DwmIsCompositionEnabled()
        {
            if (!Utility.IsOsVistaOrNewer)
            {
                return false;
            }
            return _DwmIsCompositionEnabled();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DwmDefWindowProc(IntPtr hwnd, WM msg, IntPtr wParam, IntPtr lParam, out IntPtr plResult);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
        private static extern void _DwmSetWindowAttribute(IntPtr hwnd, DWMWA dwAttribute, ref int pvAttribute, int cbAttribute);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void DwmSetWindowAttributeFlip3DPolicy(IntPtr hwnd, DWMFLIP3D flip3dPolicy)
        {
            Assert.IsTrue(Utility.IsOsVistaOrNewer);
            var dwPolicy = (int)flip3dPolicy;
            _DwmSetWindowAttribute(hwnd, DWMWA.FLIP3D_POLICY, ref dwPolicy, sizeof (int));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void DwmSetWindowAttributeDisallowPeek(IntPtr hwnd, bool disallowPeek)
        {
            Assert.IsTrue(Utility.IsOsWindows7OrNewer);
            int dwDisallow = (int)(disallowPeek ? Win32Value.TRUE : Win32Value.FALSE);
            _DwmSetWindowAttribute(hwnd, DWMWA.DISALLOW_PEEK, ref dwDisallow, sizeof (int));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "EnableMenuItem")]
        private static extern int _EnableMenuItem(IntPtr hMenu, SC uIDEnableItem, MF uEnable);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static MF EnableMenuItem(IntPtr hMenu, SC uIDEnableItem, MF uEnable)
        {
            int iRet = _EnableMenuItem(hMenu, uIDEnableItem, uEnable);
            return (MF)iRet;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "RemoveMenu", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void RemoveMenu(IntPtr hMenu, SC uPosition, MF uFlags)
        {
            if (!_RemoveMenu(hMenu, (uint)uPosition, (uint)uFlags))
            {
                throw new Win32Exception();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "DrawMenuBar", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _DrawMenuBar(IntPtr hWnd);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void DrawMenuBar(IntPtr hWnd)
        {
            if (!_DrawMenuBar(hWnd))
            {
                throw new Win32Exception();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindClose(IntPtr handle);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFindHandle FindFirstFileW(string lpFileName, [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_DATAW lpFindFileData);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindNextFileW(SafeFindHandle hndFindFile, [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_DATAW lpFindFileData);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetClientRect", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _GetClientRect(IntPtr hwnd, [Out] out RECT lpRect);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static RECT GetClientRect(IntPtr hwnd)
        {
            RECT rc;
            if (!_GetClientRect(hwnd, out rc))
            {
                HRESULT.ThrowLastError();
            }
            return rc;
        }

        [DllImport("uxtheme.dll", EntryPoint = "GetCurrentThemeName", CharSet = CharSet.Unicode)]
        private static extern HRESULT _GetCurrentThemeName(
            StringBuilder pszThemeFileName,
            int dwMaxNameChars,
            StringBuilder pszColorBuff,
            int cchMaxColorChars,
            StringBuilder pszSizeBuff,
            int cchMaxSizeChars);

        public static void GetCurrentThemeName(out string themeFileName, out string color, out string size)
        {
            var fileNameBuilder = new StringBuilder((int)Win32Value.MAX_PATH);
            var colorBuilder = new StringBuilder((int)Win32Value.MAX_PATH);
            var sizeBuilder = new StringBuilder((int)Win32Value.MAX_PATH);

            _GetCurrentThemeName(fileNameBuilder, fileNameBuilder.Capacity,
                colorBuilder, colorBuilder.Capacity,
                sizeBuilder, sizeBuilder.Capacity)
                .ThrowIfFailed();

            themeFileName = fileNameBuilder.ToString();
            color = colorBuilder.ToString();
            size = sizeBuilder.ToString();
        }

        [DllImport("uxtheme.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsThemeActive();

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [Obsolete("Use SafeDC.GetDC instead.", true)]
        public static void GetDC()
        {
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(SafeDC hdc, DeviceCap nIndex);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("kernel32.dll", EntryPoint = "GetModuleFileName", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int _GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string GetModuleFileName(IntPtr hModule)
        {
            var buffer = new StringBuilder((int)Win32Value.MAX_PATH);
            while (true)
            {
                int size = _GetModuleFileName(hModule, buffer, buffer.Capacity);
                if (size == 0)
                {
                    HRESULT.ThrowLastError();
                }

                if (size == buffer.Capacity)
                {
                    buffer.EnsureCapacity(buffer.Capacity * 2);
                    continue;
                }

                return buffer.ToString();
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr _GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        public static IntPtr GetModuleHandle(string lpModuleName)
        {
            var retPtr = _GetModuleHandle(lpModuleName);
            if (retPtr == IntPtr.Zero)
            {
                HRESULT.ThrowLastError();
            }
            return retPtr;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetMonitorInfo", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _GetMonitorInfo(IntPtr hMonitor, [In, Out] MONITORINFO lpmi);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static MONITORINFO GetMonitorInfo(IntPtr hMonitor)
        {
            var mi = new MONITORINFO();
            if (!_GetMonitorInfo(hMonitor, mi))
            {
                throw new Win32Exception();
            }
            return mi;
        }

        [DllImport("gdi32.dll", EntryPoint = "GetStockObject", SetLastError = true)]
        private static extern IntPtr _GetStockObject(StockObject fnObject);

        public static IntPtr GetStockObject(StockObject fnObject)
        {
            var retPtr = _GetStockObject(fnObject);
            if (retPtr == null)
            {
                HRESULT.ThrowLastError();
            }
            return retPtr;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SM nIndex);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr GetWindowLongPtr(IntPtr hwnd, GWL nIndex)
        {
            var ret = IntPtr.Zero;
            if (8 == IntPtr.Size)
            {
                ret = GetWindowLongPtr64(hwnd, nIndex);
            }
            else
            {
                ret = new IntPtr(GetWindowLongPtr32(hwnd, nIndex));
            }
            if (IntPtr.Zero == ret)
            {
                throw new Win32Exception();
            }
            return ret;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("uxtheme.dll", PreserveSig = false)]
        public static extern void SetWindowThemeAttribute([In] IntPtr hwnd, [In] WINDOWTHEMEATTRIBUTETYPE eAttribute, [In] ref WTA_OPTIONS pvAttribute, [In] uint cbAttribute);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        private static extern int GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hwnd, WINDOWPLACEMENT lpwndpl);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
        {
            var wndpl = new WINDOWPLACEMENT();
            if (GetWindowPlacement(hwnd, wndpl))
            {
                return wndpl;
            }
            throw new Win32Exception();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static RECT GetWindowRect(IntPtr hwnd)
        {
            RECT rc;
            if (!_GetWindowRect(hwnd, out rc))
            {
                HRESULT.ThrowLastError();
            }
            return rc;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdiplus.dll")]
        public static extern Status GdipCreateBitmapFromStream(IStream stream, out IntPtr bitmap);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdiplus.dll")]
        public static extern Status GdipCreateHBITMAPFromBitmap(IntPtr bitmap, out IntPtr hbmReturn, Int32 background);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdiplus.dll")]
        public static extern Status GdipCreateHICONFromBitmap(IntPtr bitmap, out IntPtr hbmReturn);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdiplus.dll")]
        public static extern Status GdipDisposeImage(IntPtr image);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdiplus.dll")]
        public static extern Status GdipImageForceValidation(IntPtr image);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdiplus.dll")]
        public static extern Status GdiplusStartup(out IntPtr token, StartupInput input, out StartupOutput output);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdiplus.dll")]
        public static extern Status GdiplusShutdown(IntPtr token);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("kernel32.dll", EntryPoint = "LocalFree", SetLastError = true)]
        private static extern IntPtr _LocalFree(IntPtr hMem);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "PostMessage", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam)
        {
            if (!_PostMessage(hWnd, Msg, wParam, lParam))
            {
                throw new Win32Exception();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterClassExW")]
        private static extern short _RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static short RegisterClassEx(ref WNDCLASSEX lpwcx)
        {
            short ret = _RegisterClassEx(ref lpwcx);
            if (ret == 0)
            {
                HRESULT.ThrowLastError();
            }

            return ret;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessage", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint _RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static WM RegisterWindowMessage(string lpString)
        {
            uint iRet = _RegisterWindowMessage(lpString);
            if (iRet == 0)
            {
                HRESULT.ThrowLastError();
            }
            return (WM)iRet;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetActiveWindow", SetLastError = true)]
        private static extern IntPtr _SetActiveWindow(IntPtr hWnd);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr SetActiveWindow(IntPtr hwnd)
        {
            Verify.IsNotDefault(hwnd, "hwnd");
            var ret = _SetActiveWindow(hwnd);
            if (ret == IntPtr.Zero)
            {
                HRESULT.ThrowLastError();
            }

            return ret;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr SetClassLongPtr(IntPtr hwnd, GCLP nIndex, IntPtr dwNewLong)
        {
            if (8 == IntPtr.Size)
            {
                return SetClassLongPtr64(hwnd, nIndex, dwNewLong);
            }
            return new IntPtr(SetClassLongPtr32(hwnd, nIndex, dwNewLong.ToInt32()));
        }

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetClassLong", SetLastError = true)]
        private static extern int SetClassLongPtr32(IntPtr hWnd, GCLP nIndex, int dwNewLong);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetClassLongPtr", SetLastError = true)]
        private static extern IntPtr SetClassLongPtr64(IntPtr hWnd, GCLP nIndex, IntPtr dwNewLong);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern ErrorModes SetErrorMode(ErrorModes newMode);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "SetProcessWorkingSetSize")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _SetProcessWorkingSetSize(IntPtr hProcess, IntPtr dwMinimiumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize)
        {
            if (!_SetProcessWorkingSetSize(hProcess, new IntPtr(dwMinimumWorkingSetSize), new IntPtr(dwMaximumWorkingSetSize)))
            {
                throw new Win32Exception();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr SetWindowLongPtr(IntPtr hwnd, GWL nIndex, IntPtr dwNewLong)
        {
            if (8 == IntPtr.Size)
            {
                return SetWindowLongPtr64(hwnd, nIndex, dwNewLong);
            }
            return new IntPtr(SetWindowLongPtr32(hwnd, nIndex, dwNewLong.ToInt32()));
        }

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int SetWindowLongPtr32(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetWindowRgn", SetLastError = true)]
        private static extern int _SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool bRedraw);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw)
        {
            int err = _SetWindowRgn(hWnd, hRgn, bRedraw);
            if (0 == err)
            {
                throw new Win32Exception();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags)
        {
            if (!_SetWindowPos(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags))
            {
                return false;
            }

            return true;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("shell32.dll", SetLastError = false)]
        public static extern Win32Error SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hwnd, SW nCmdShow);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _SystemParametersInfo_String(SPI uiAction, int uiParam, [MarshalAs(UnmanagedType.LPWStr)] string pvParam, SPIF fWinIni);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _SystemParametersInfo_NONCLIENTMETRICS(SPI uiAction, int uiParam, [In, Out] ref NONCLIENTMETRICS pvParam, SPIF fWinIni);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _SystemParametersInfo_HIGHCONTRAST(SPI uiAction, int uiParam, [In, Out] ref HIGHCONTRAST pvParam, SPIF fWinIni);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SystemParametersInfo(SPI uiAction, int uiParam, string pvParam, SPIF fWinIni)
        {
            if (!_SystemParametersInfo_String(uiAction, uiParam, pvParam, fWinIni))
            {
                HRESULT.ThrowLastError();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static NONCLIENTMETRICS SystemParameterInfo_GetNONCLIENTMETRICS()
        {
            var metrics = Utility.IsOsVistaOrNewer
                ? NONCLIENTMETRICS.VistaMetricsStruct
                : NONCLIENTMETRICS.XPMetricsStruct;

            if (!_SystemParametersInfo_NONCLIENTMETRICS(SPI.GETNONCLIENTMETRICS, metrics.cbSize, ref metrics, SPIF.None))
            {
                HRESULT.ThrowLastError();
            }

            return metrics;
        }

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static HIGHCONTRAST SystemParameterInfo_GetHIGHCONTRAST()
        {
            var hc = new HIGHCONTRAST
            {
                cbSize = Marshal.SizeOf(typeof(HIGHCONTRAST))
            };

            if (!_SystemParametersInfo_HIGHCONTRAST(SPI.GETHIGHCONTRAST, hc.cbSize, ref hc, SPIF.None))
            {
                HRESULT.ThrowLastError();
            }

            return hc;
        }

        [DllImport("user32.dll")]
        public static extern uint TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll", EntryPoint = "SelectObject", SetLastError = true)]
        private static extern IntPtr _SelectObject(SafeDC hdc, IntPtr hgdiobj);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr SelectObject(SafeDC hdc, IntPtr hgdiobj)
        {
            var ret = _SelectObject(hdc, hgdiobj);
            if (ret == IntPtr.Zero)
            {
                HRESULT.ThrowLastError();
            }
            return ret;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("gdi32.dll", EntryPoint = "SelectObject", SetLastError = true)]
        private static extern IntPtr _SelectObjectSafeHBITMAP(SafeDC hdc, SafeHBITMAP hgdiobj);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr SelectObject(SafeDC hdc, SafeHBITMAP hgdiobj)
        {
            var ret = _SelectObjectSafeHBITMAP(hdc, hgdiobj);
            if (ret == IntPtr.Zero)
            {
                HRESULT.ThrowLastError();
            }
            return ret;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "UnregisterClass", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _UnregisterClassAtom(IntPtr lpClassName, IntPtr hInstance);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", EntryPoint = "UnregisterClass", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _UnregisterClassName(string lpClassName, IntPtr hInstance);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void UnregisterClass(short atom, IntPtr hinstance)
        {
            if (!_UnregisterClassAtom(new IntPtr(atom), hinstance))
            {
                HRESULT.ThrowLastError();
            }
        }

        public static void UnregisterClass(string lpClassName, IntPtr hInstance)
        {
            if (!_UnregisterClassName(lpClassName, hInstance))
            {
                HRESULT.ThrowLastError();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "UpdateLayeredWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _UpdateLayeredWindow(
            IntPtr hwnd,
            SafeDC hdcDst,
            [In] ref POINT pptDst,
            [In] ref SIZE psize,
            SafeDC hdcSrc,
            [In] ref POINT pptSrc,
            int crKey,
            ref BLENDFUNCTION pblend,
            ULW dwFlags);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "UpdateLayeredWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool _UpdateLayeredWindowIntPtr(
            IntPtr hwnd,
            IntPtr hdcDst,
            IntPtr pptDst,
            IntPtr psize,
            IntPtr hdcSrc,
            IntPtr pptSrc,
            int crKey,
            ref BLENDFUNCTION pblend,
            ULW dwFlags);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void UpdateLayeredWindow(
            IntPtr hwnd,
            SafeDC hdcDst,
            ref POINT pptDst,
            ref SIZE psize,
            SafeDC hdcSrc,
            ref POINT pptSrc,
            int crKey,
            ref BLENDFUNCTION pblend,
            ULW dwFlags)
        {
            if (!_UpdateLayeredWindow(hwnd, hdcDst, ref pptDst, ref psize, hdcSrc, ref pptSrc, crKey, ref pblend, dwFlags))
            {
                HRESULT.ThrowLastError();
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void UpdateLayeredWindow(
            IntPtr hwnd,
            int crKey,
            ref BLENDFUNCTION pblend,
            ULW dwFlags)
        {
            if (!_UpdateLayeredWindowIntPtr(hwnd, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, crKey, ref pblend, dwFlags))
            {
                HRESULT.ThrowLastError();
            }
        }

        [DllImport("shell32.dll", EntryPoint = "SHAddToRecentDocs")]
        private static extern void _SHAddToRecentDocs_String(SHARD uFlags, [MarshalAs(UnmanagedType.LPWStr)] string pv);

        [DllImport("shell32.dll", EntryPoint = "SHAddToRecentDocs")]
        private static extern void _SHAddToRecentDocs_ShellLink(SHARD uFlags, IShellLinkW pv);

        public static void SHAddToRecentDocs(string path)
        {
            _SHAddToRecentDocs_String(SHARD.PATHW, path);
        }

        public static void SHAddToRecentDocs(IShellLinkW shellLink)
        {
            _SHAddToRecentDocs_ShellLink(SHARD.LINK, shellLink);
        }

        [DllImport("dwmapi.dll", EntryPoint = "DwmGetCompositionTimingInfo")]
        private static extern HRESULT _DwmGetCompositionTimingInfo(IntPtr hwnd, ref DWM_TIMING_INFO pTimingInfo);

        public static DWM_TIMING_INFO? DwmGetCompositionTimingInfo(IntPtr hwnd)
        {
            if (!Utility.IsOsVistaOrNewer)
            {
                return null;
            }

            var dti = new DWM_TIMING_INFO
            {
                cbSize = Marshal.SizeOf(typeof(DWM_TIMING_INFO))
            };
            var hr = _DwmGetCompositionTimingInfo(hwnd, ref dti);
            if (hr == HRESULT.E_PENDING)
            {
                return null;
            }
            hr.ThrowIfFailed();

            return dti;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmInvalidateIconicBitmaps(IntPtr hwnd);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmSetIconicThumbnail(IntPtr hwnd, IntPtr hbmp, DWM_SIT dwSITFlags);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmSetIconicLivePreviewBitmap(IntPtr hwnd, IntPtr hbmp, RefPOINT pptClient, DWM_SIT dwSITFlags);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("shell32.dll", PreserveSig = false)]
        public static extern void SHGetItemFromDataObject(IDataObject pdtobj, DOGIF dwFlags, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("shell32.dll", PreserveSig = false)]
        public static extern HRESULT SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            IBindCtx pbc,
            [In] ref Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)] out object ppv);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("shell32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Shell_NotifyIcon(NIM dwMessage, [In] NOTIFYICONDATA lpdata);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("shell32.dll", PreserveSig = false)]
        public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [DllImport("shell32.dll")]
        public static extern HRESULT GetCurrentProcessExplicitAppUserModelID([Out, MarshalAs(UnmanagedType.LPWStr)] out string AppID);

        #endregion
    }
}