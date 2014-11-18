using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SharpLib.OpenGL
{
    public static class Glut
    {
        #region Делегаты

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ButtonBoxCallback(int button, int state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CloseCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CreateMenuCallback(int val);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DialsCallback(int dial, int val);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DisplayCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void EntryCallback(int state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLUTprocDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void IdleCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void JoystickCallback(int buttonMask, int x, int y, int z);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void KeyboardCallback(byte key, int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void KeyboardUpCallback(byte key, int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MenuDestroyCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MenuStateCallback(int state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MenuStatusCallback(int status, int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MotionCallback(int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MouseCallback(int button, int state, int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MouseWheelCallback(int wheel, int direction, int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OverlayDisplayCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PassiveMotionCallback(int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReshapeCallback(int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SpaceballButtonCallback(int button, int state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SpaceballMotionCallback(int x, int y, int z);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SpaceballRotateCallback(int x, int y, int z);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SpecialCallback(int key, int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SpecialUpCallback(int key, int x, int y);

        public delegate void TabletButtonCallback(int button, int state, int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void TabletMotionCallback(int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void TimerCallback(int val);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void VisibilityCallback(int state);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WindowCloseCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WindowStatusCallback(int state);

        #endregion

        #region Константы

        private const CallingConvention CALLING_CONVENTION = CallingConvention.Winapi;

        public const int FREEGLUT = 1;

        private const string FREEGLUT_NATIVE_LIBRARY = "freeglut.dll";

        public const int FREEGLUT_VERSION_2_0 = 1;

        public const int GLUT_ACCUM = 4;

        public const int GLUT_ACTION_CONTINUE_EXECUTION = 2;

        public const int GLUT_ACTION_EXIT = 0;

        public const int GLUT_ACTION_GLUTMAINLOOP_RETURNS = 1;

        public const int GLUT_ACTION_ON_WINDOW_CLOSE = 0x01F9;

        public const int GLUT_ACTIVE_ALT = 4;

        public const int GLUT_ACTIVE_CTRL = 2;

        public const int GLUT_ACTIVE_SHIFT = 1;

        public const int GLUT_ALLOW_DIRECT_CONTEXT = 1;

        public const int GLUT_ALPHA = 8;

        public const int GLUT_API_VERSION = 4;

        public const int GLUT_AUX1 = 0x1000;

        public const int GLUT_AUX2 = 0x2000;

        public const int GLUT_AUX3 = 0x4000;

        public const int GLUT_AUX4 = 0x8000;

        public const int GLUT_BLUE = 2;

        public const int GLUT_CREATE_NEW_CONTEXT = 0;

        public const int GLUT_CURSOR_BOTTOM_LEFT_CORNER = 19;

        public const int GLUT_CURSOR_BOTTOM_RIGHT_CORNER = 18;

        public const int GLUT_CURSOR_BOTTOM_SIDE = 13;

        public const int GLUT_CURSOR_CROSSHAIR = 9;

        public const int GLUT_CURSOR_CYCLE = 5;

        public const int GLUT_CURSOR_DESTROY = 3;

        public const int GLUT_CURSOR_FULL_CROSSHAIR = 102;

        public const int GLUT_CURSOR_HELP = 4;

        public const int GLUT_CURSOR_INFO = 2;

        public const int GLUT_CURSOR_INHERIT = 100;

        public const int GLUT_CURSOR_LEFT_ARROW = 1;

        public const int GLUT_CURSOR_LEFT_RIGHT = 11;

        public const int GLUT_CURSOR_LEFT_SIDE = 14;

        public const int GLUT_CURSOR_NONE = 101;

        public const int GLUT_CURSOR_RIGHT_ARROW = 0;

        public const int GLUT_CURSOR_RIGHT_SIDE = 15;

        public const int GLUT_CURSOR_SPRAY = 6;

        public const int GLUT_CURSOR_TEXT = 8;

        public const int GLUT_CURSOR_TOP_LEFT_CORNER = 16;

        public const int GLUT_CURSOR_TOP_RIGHT_CORNER = 17;

        public const int GLUT_CURSOR_TOP_SIDE = 12;

        public const int GLUT_CURSOR_UP_DOWN = 10;

        public const int GLUT_CURSOR_WAIT = 7;

        public const int GLUT_DEPTH = 16;

        public const int GLUT_DEVICE_IGNORE_KEY_REPEAT = 610;

        public const int GLUT_DEVICE_KEY_REPEAT = 611;

        public const int GLUT_DIRECT_RENDERING = 0x01FE;

        public const int GLUT_DISPLAY_MODE_POSSIBLE = 400;

        public const int GLUT_DOUBLE = 2;

        public const int GLUT_DOWN = 0;

        public const int GLUT_ELAPSED_TIME = 700;

        public const int GLUT_ENTERED = 1;

        public const int GLUT_FORCE_DIRECT_CONTEXT = 3;

        public const int GLUT_FORCE_INDIRECT_CONTEXT = 0;

        public const int GLUT_FULLY_COVERED = 3;

        public const int GLUT_FULLY_RETAINED = 1;

        public const int GLUT_GAME_MODE_ACTIVE = 0;

        public const int GLUT_GAME_MODE_DISPLAY_CHANGED = 6;

        public const int GLUT_GAME_MODE_HEIGHT = 3;

        public const int GLUT_GAME_MODE_PIXEL_DEPTH = 4;

        public const int GLUT_GAME_MODE_POSSIBLE = 1;

        public const int GLUT_GAME_MODE_REFRESH_RATE = 5;

        public const int GLUT_GAME_MODE_WIDTH = 2;

        public const int GLUT_GREEN = 1;

        public const int GLUT_HAS_DIAL_AND_BUTTON_BOX = 603;

        public const int GLUT_HAS_JOYSTICK = 612;

        public const int GLUT_HAS_KEYBOARD = 600;

        public const int GLUT_HAS_MOUSE = 601;

        public const int GLUT_HAS_OVERLAY = 802;

        public const int GLUT_HAS_SPACEBALL = 602;

        public const int GLUT_HAS_TABLET = 604;

        public const int GLUT_HIDDEN = 0;

        public const int GLUT_INDEX = 1;

        public const int GLUT_INIT_DISPLAY_MODE = 504;

        public const int GLUT_INIT_STATE = 124;

        public const int GLUT_INIT_WINDOW_HEIGHT = 503;

        public const int GLUT_INIT_WINDOW_WIDTH = 502;

        public const int GLUT_INIT_WINDOW_X = 500;

        public const int GLUT_INIT_WINDOW_Y = 501;

        public const int GLUT_JOYSTICK_AXES = 615;

        public const int GLUT_JOYSTICK_BUTTONS = 614;

        public const int GLUT_JOYSTICK_BUTTON_A = 1;

        public const int GLUT_JOYSTICK_BUTTON_B = 2;

        public const int GLUT_JOYSTICK_BUTTON_C = 4;

        public const int GLUT_JOYSTICK_BUTTON_D = 8;

        public const int GLUT_JOYSTICK_POLL_RATE = 616;

        public const int GLUT_KEY_DOWN = 103;

        public const int GLUT_KEY_END = 107;

        public const int GLUT_KEY_F1 = 1;

        public const int GLUT_KEY_F10 = 10;

        public const int GLUT_KEY_F11 = 11;

        public const int GLUT_KEY_F12 = 12;

        public const int GLUT_KEY_F2 = 2;

        public const int GLUT_KEY_F3 = 3;

        public const int GLUT_KEY_F4 = 4;

        public const int GLUT_KEY_F5 = 5;

        public const int GLUT_KEY_F6 = 6;

        public const int GLUT_KEY_F7 = 7;

        public const int GLUT_KEY_F8 = 8;

        public const int GLUT_KEY_F9 = 9;

        public const int GLUT_KEY_HOME = 106;

        public const int GLUT_KEY_INSERT = 108;

        public const int GLUT_KEY_LEFT = 100;

        public const int GLUT_KEY_PAGE_DOWN = 105;

        public const int GLUT_KEY_PAGE_UP = 104;

        public const int GLUT_KEY_REPEAT_DEFAULT = 2;

        public const int GLUT_KEY_REPEAT_OFF = 0;

        public const int GLUT_KEY_REPEAT_ON = 1;

        public const int GLUT_KEY_RIGHT = 102;

        public const int GLUT_KEY_UP = 101;

        public const int GLUT_LAYER_IN_USE = 801;

        public const int GLUT_LEFT = 0;

        public const int GLUT_LEFT_BUTTON = 0;

        public const int GLUT_LUMINANCE = 512;

        public const int GLUT_MENU_IN_USE = 1;

        public const int GLUT_MENU_NOT_IN_USE = 0;

        public const int GLUT_MENU_NUM_ITEMS = 300;

        public const int GLUT_MIDDLE_BUTTON = 1;

        public const int GLUT_MULTISAMPLE = 128;

        public const int GLUT_NORMAL = 0;

        public const int GLUT_NORMAL_DAMAGED = 804;

        public const int GLUT_NOT_VISIBLE = 0;

        public const int GLUT_NUM_BUTTON_BOX_BUTTONS = 607;

        public const int GLUT_NUM_DIALS = 608;

        public const int GLUT_NUM_MOUSE_BUTTONS = 605;

        public const int GLUT_NUM_SPACEBALL_BUTTONS = 606;

        public const int GLUT_NUM_TABLET_BUTTONS = 609;

        public const int GLUT_OVERLAY = 1;

        public const int GLUT_OVERLAY_DAMAGED = 805;

        public const int GLUT_OVERLAY_POSSIBLE = 800;

        public const int GLUT_OWNS_JOYSTICK = 613;

        public const int GLUT_PARTIALLY_RETAINED = 2;

        public const int GLUT_RED = 0;

        public const int GLUT_RENDERING_CONTEXT = 0x01FD;

        public const int GLUT_RGB = 0;

        public const int GLUT_RGBA = GLUT_RGB;

        public const int GLUT_RIGHT_BUTTON = 2;

        public const int GLUT_SCREEN_HEIGHT = 201;

        public const int GLUT_SCREEN_HEIGHT_MM = 203;

        public const int GLUT_SCREEN_WIDTH = 200;

        public const int GLUT_SCREEN_WIDTH_MM = 202;

        public const int GLUT_SINGLE = 0;

        public const int GLUT_STENCIL = 32;

        public const int GLUT_STEREO = 256;

        public const int GLUT_TRANSPARENT_INDEX = 803;

        public const int GLUT_TRY_DIRECT_CONTEXT = 2;

        public const int GLUT_UP = 1;

        public const int GLUT_USE_CURRENT_CONTEXT = 1;

        public const int GLUT_VERSION = 0x01FC;

        public const int GLUT_VIDEO_RESIZE_HEIGHT = 909;

        public const int GLUT_VIDEO_RESIZE_HEIGHT_DELTA = 905;

        public const int GLUT_VIDEO_RESIZE_IN_USE = 901;

        public const int GLUT_VIDEO_RESIZE_POSSIBLE = 900;

        public const int GLUT_VIDEO_RESIZE_WIDTH = 908;

        public const int GLUT_VIDEO_RESIZE_WIDTH_DELTA = 904;

        public const int GLUT_VIDEO_RESIZE_X = 906;

        public const int GLUT_VIDEO_RESIZE_X_DELTA = 902;

        public const int GLUT_VIDEO_RESIZE_Y = 907;

        public const int GLUT_VIDEO_RESIZE_Y_DELTA = 903;

        public const int GLUT_VISIBLE = 1;

        public const int GLUT_WINDOW_ACCUM_ALPHA_SIZE = 114;

        public const int GLUT_WINDOW_ACCUM_BLUE_SIZE = 113;

        public const int GLUT_WINDOW_ACCUM_GREEN_SIZE = 112;

        public const int GLUT_WINDOW_ACCUM_RED_SIZE = 111;

        public const int GLUT_WINDOW_ALPHA_SIZE = 110;

        public const int GLUT_WINDOW_BLUE_SIZE = 109;

        public const int GLUT_WINDOW_BORDER_WIDTH = 0x01FA;

        public const int GLUT_WINDOW_BUFFER_SIZE = 104;

        public const int GLUT_WINDOW_COLORMAP_SIZE = 119;

        public const int GLUT_WINDOW_CURSOR = 122;

        public const int GLUT_WINDOW_DEPTH_SIZE = 106;

        public const int GLUT_WINDOW_DOUBLEBUFFER = 115;

        public const int GLUT_WINDOW_FORMAT_ID = 123;

        public const int GLUT_WINDOW_GREEN_SIZE = 108;

        public const int GLUT_WINDOW_HEADER_HEIGHT = 0x01FB;

        public const int GLUT_WINDOW_HEIGHT = 103;

        public const int GLUT_WINDOW_NUM_CHILDREN = 118;

        public const int GLUT_WINDOW_NUM_SAMPLES = 120;

        public const int GLUT_WINDOW_PARENT = 117;

        public const int GLUT_WINDOW_RED_SIZE = 107;

        public const int GLUT_WINDOW_RGBA = 116;

        public const int GLUT_WINDOW_STENCIL_SIZE = 105;

        public const int GLUT_WINDOW_STEREO = 121;

        public const int GLUT_WINDOW_WIDTH = 102;

        public const int GLUT_WINDOW_X = 100;

        public const int GLUT_WINDOW_Y = 101;

        public const int GLUT_XLIB_IMPLEMENTATION = 13;

        #endregion

        #region Поля

        public static readonly IntPtr GLUT_BITMAP_8_BY_13;

        public static readonly IntPtr GLUT_BITMAP_9_BY_15;

        public static readonly IntPtr GLUT_BITMAP_HELVETICA_10;

        public static readonly IntPtr GLUT_BITMAP_HELVETICA_12;

        public static readonly IntPtr GLUT_BITMAP_HELVETICA_18;

        public static readonly IntPtr GLUT_BITMAP_TIMES_ROMAN_10;

        public static readonly IntPtr GLUT_BITMAP_TIMES_ROMAN_24;

        public static readonly IntPtr GLUT_STROKE_MONO_ROMAN;

        public static readonly IntPtr GLUT_STROKE_ROMAN;

        private static ButtonBoxCallback buttonBoxCallback;

        private static CloseCallback closeCallback;

        private static CreateMenuCallback createMenuCallback;

        private static DialsCallback dialsCallback;

        private static DisplayCallback displayCallback;

        private static EntryCallback entryCallback;

        private static IdleCallback idleCallback;

        private static JoystickCallback joystickCallback;

        private static KeyboardCallback keyboardCallback;

        private static KeyboardUpCallback keyboardUpCallback;

        private static MenuDestroyCallback menuDestroyCallback;

        private static MenuStateCallback menuStateCallback;

        private static MenuStatusCallback menuStatusCallback;

        private static MotionCallback motionCallback;

        private static MouseCallback mouseCallback;

        private static MouseWheelCallback mouseWheelCallback;

        private static OverlayDisplayCallback overlayDisplayCallback;

        private static PassiveMotionCallback passiveMotionCallback;

        private static ReshapeCallback reshapeCallback;

        private static SpaceballButtonCallback spaceballButtonCallback;

        private static SpaceballMotionCallback spaceballMotionCallback;

        private static SpaceballRotateCallback spaceballRotateCallback;

        private static SpecialCallback specialCallback;

        private static SpecialUpCallback specialUpCallback;

        private static TabletButtonCallback tabletButtonCallback;

        private static TabletMotionCallback tabletMotionCallback;

        private static TimerCallback timerCallback;

        private static VisibilityCallback visibilityCallback;

        private static WindowCloseCallback windowCloseCallback;

        private static WindowStatusCallback windowStatusCallback;

        #endregion

        #region Конструктор

        static Glut()
        {
            unsafe
            {
                GLUT_STROKE_ROMAN = new IntPtr((void*)0);
                GLUT_STROKE_MONO_ROMAN = new IntPtr((void*)1);
                GLUT_BITMAP_9_BY_15 = new IntPtr((void*)2);
                GLUT_BITMAP_8_BY_13 = new IntPtr((void*)3);
                GLUT_BITMAP_TIMES_ROMAN_10 = new IntPtr((void*)4);
                GLUT_BITMAP_TIMES_ROMAN_24 = new IntPtr((void*)5);
                GLUT_BITMAP_HELVETICA_10 = new IntPtr((void*)6);
                GLUT_BITMAP_HELVETICA_12 = new IntPtr((void*)7);
                GLUT_BITMAP_HELVETICA_18 = new IntPtr((void*)8);
            }
        }

        #endregion

        #region Методы

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutCreateMenu"), SuppressUnmanagedCodeSecurity]
        private static extern int __glutCreateMenu([In] CreateMenuCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutDisplayFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutDisplayFunc(DisplayCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutReshapeFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutReshapeFunc(ReshapeCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutKeyboardFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutKeyboardFunc(KeyboardCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutMouseFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutMouseFunc(MouseCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutMotionFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutMotionFunc(MotionCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutPassiveMotionFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutPassiveMotionFunc(PassiveMotionCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutEntryFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutEntryFunc(EntryCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutVisibilityFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutVisibilityFunc(VisibilityCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutIdleFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutIdleFunc(IdleCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutTimerFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutTimerFunc(int msecs, TimerCallback func, int val);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutMenuStateFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutMenuStateFunc(MenuStateCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutSpecialFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutSpecialFunc(SpecialCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutSpaceballMotionFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutSpaceballMotionFunc(SpaceballMotionCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutSpaceballRotateFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutSpaceballRotateFunc(SpaceballRotateCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutSpaceballButtonFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutSpaceballButtonFunc(SpaceballButtonCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutButtonBoxFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutButtonBoxFunc(ButtonBoxCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutDialsFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutDialsFunc(DialsCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutTabletMotionFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutTabletMotionFunc(TabletMotionCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutTabletButtonFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutTabletButtonFunc(TabletButtonCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutMenuStatusFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutMenuStatusFunc(MenuStatusCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutOverlayDisplayFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutOverlayDisplayFunc(OverlayDisplayCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutWindowStatusFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutWindowStatusFunc(WindowStatusCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutKeyboardUpFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutKeyboardUpFunc(KeyboardUpCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutSpecialUpFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutSpecialUpFunc(SpecialUpCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutJoystickFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutJoystickFunc(JoystickCallback func, int pollInterval);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutMouseWheelFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutMouseWheelFunc(MouseWheelCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutCloseFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutCloseFunc(CloseCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutWMCloseFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutWMCloseFunc(WindowCloseCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutMenuDestroyFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutMenuDestroyFunc(MenuDestroyCallback func);

        public static void glutInit()
        {
            string[] argsArray = Environment.GetCommandLineArgs();
            StringBuilder[] args = new StringBuilder[argsArray.Length];
            int argsLength = args.Length;

            for (int i = 0; i < argsArray.Length; i++)
            {
                args[i] = new StringBuilder(argsArray[i], argsArray[i].Length);
            }

            glutInit(ref argsLength, args);
        }

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutInit(ref int argcp, StringBuilder[] argv);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutInitDisplayMode(int mode);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutInitDisplayString(string str);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutInitWindowPosition(int x, int y);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutInitWindowSize(int width, int height);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutMainLoop();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutCreateWindow(string name);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutCreateSubWindow(int win, int x, int y, int width, int height);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutDestroyWindow(int win);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutPostRedisplay();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutPostWindowRedisplay(int win);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSwapBuffers();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutGetWindow();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetWindow(int win);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetWindowTitle(string name);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetIconTitle(string name);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutPositionWindow(int x, int y);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutReshapeWindow(int width, int height);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutPopWindow();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutPushWindow();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutIconifyWindow();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutShowWindow();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutHideWindow();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutFullScreen();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetCursor(int cursor);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWarpPointer(int x, int y);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutEstablishOverlay();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutRemoveOverlay();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutUseLayer(int layer);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutPostOverlayRedisplay();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutPostWindowOverlayRedisplay(int win);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutShowOverlay();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutHideOverlay();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutDestroyMenu(int menu);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutGetMenu();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetMenu(int menu);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutAddMenuEntry(string name, int val);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutAddSubMenu(string name, int menu);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutChangeToMenuEntry(int entry, string name, int val);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutChangeToSubMenu(int entry, string name, int menu);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutRemoveMenuItem(int entry);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutAttachMenu(int button);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutDetachMenu(int button);

        public static int glutCreateMenu([In] CreateMenuCallback func)
        {
            createMenuCallback = func;
            return __glutCreateMenu(createMenuCallback);
        }

        public static void glutDisplayFunc([In] DisplayCallback func)
        {
            displayCallback = func;
            __glutDisplayFunc(displayCallback);
        }

        public static void glutReshapeFunc([In] ReshapeCallback func)
        {
            reshapeCallback = func;
            __glutReshapeFunc(reshapeCallback);
        }

        public static void glutKeyboardFunc([In] KeyboardCallback func)
        {
            keyboardCallback = func;
            __glutKeyboardFunc(keyboardCallback);
        }

        public static void glutMouseFunc([In] MouseCallback func)
        {
            mouseCallback = func;
            __glutMouseFunc(mouseCallback);
        }

        public static void glutMotionFunc([In] MotionCallback func)
        {
            motionCallback = func;
            __glutMotionFunc(motionCallback);
        }

        public static void glutPassiveMotionFunc([In] PassiveMotionCallback func)
        {
            passiveMotionCallback = func;
            __glutPassiveMotionFunc(passiveMotionCallback);
        }

        public static void glutEntryFunc([In] EntryCallback func)
        {
            entryCallback = func;
            __glutEntryFunc(entryCallback);
        }

        public static void glutVisibilityFunc([In] VisibilityCallback func)
        {
            visibilityCallback = func;
            __glutVisibilityFunc(visibilityCallback);
        }

        public static void glutIdleFunc([In] IdleCallback func)
        {
            idleCallback = func;
            __glutIdleFunc(idleCallback);
        }

        public static void glutTimerFunc(int msecs, [In] TimerCallback func, int val)
        {
            timerCallback = func;
            __glutTimerFunc(msecs, timerCallback, val);
        }

        public static void glutMenuStateFunc([In] MenuStateCallback func)
        {
            menuStateCallback = func;
            __glutMenuStateFunc(menuStateCallback);
        }

        public static void glutSpecialFunc([In] SpecialCallback func)
        {
            specialCallback = func;
            __glutSpecialFunc(specialCallback);
        }

        public static void glutSpaceballMotionFunc([In] SpaceballMotionCallback func)
        {
            spaceballMotionCallback = func;
            __glutSpaceballMotionFunc(spaceballMotionCallback);
        }

        public static void glutSpaceballRotateFunc([In] SpaceballRotateCallback func)
        {
            spaceballRotateCallback = func;
            __glutSpaceballRotateFunc(spaceballRotateCallback);
        }

        public static void glutSpaceballButtonFunc([In] SpaceballButtonCallback func)
        {
            spaceballButtonCallback = func;
            __glutSpaceballButtonFunc(spaceballButtonCallback);
        }

        public static void glutButtonBoxFunc([In] ButtonBoxCallback func)
        {
            buttonBoxCallback = func;
            __glutButtonBoxFunc(buttonBoxCallback);
        }

        public static void glutDialsFunc([In] DialsCallback func)
        {
            dialsCallback = func;
            __glutDialsFunc(dialsCallback);
        }

        public static void glutTabletMotionFunc([In] TabletMotionCallback func)
        {
            tabletMotionCallback = func;
            __glutTabletMotionFunc(tabletMotionCallback);
        }

        public static void glutTabletButtonFunc([In] TabletButtonCallback func)
        {
            tabletButtonCallback = func;
            __glutTabletButtonFunc(tabletButtonCallback);
        }

        public static void glutMenuStatusFunc([In] MenuStatusCallback func)
        {
            menuStatusCallback = func;
            __glutMenuStatusFunc(menuStatusCallback);
        }

        public static void glutOverlayDisplayFunc([In] OverlayDisplayCallback func)
        {
            overlayDisplayCallback = func;
            __glutOverlayDisplayFunc(overlayDisplayCallback);
        }

        public static void glutWindowStatusFunc([In] WindowStatusCallback func)
        {
            windowStatusCallback = func;
            __glutWindowStatusFunc(windowStatusCallback);
        }

        public static void glutKeyboardUpFunc([In] KeyboardUpCallback func)
        {
            keyboardUpCallback = func;
            __glutKeyboardUpFunc(keyboardUpCallback);
        }

        public static void glutSpecialUpFunc([In] SpecialUpCallback func)
        {
            specialUpCallback = func;
            __glutSpecialUpFunc(specialUpCallback);
        }

        public static void glutJoystickFunc([In] JoystickCallback func, int pollInterval)
        {
            joystickCallback = func;
            __glutJoystickFunc(joystickCallback, pollInterval);
        }

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetColor(int cell, float red, float green, float blue);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern float glutGetColor(int cell, int component);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutCopyColormap(int win);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutGet(int state);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutDeviceGet(int info);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutExtensionSupported(string extension);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutGetModifiers();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutLayerGet(int info);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutBitmapCharacter([In] IntPtr font, int character);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutBitmapWidth([In] IntPtr font, int character);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutStrokeCharacter([In] IntPtr font, int character);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutStrokeWidth([In] IntPtr font, int character);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutBitmapLength([In] IntPtr font, string text);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutStrokeLength(IntPtr font, string text);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireSphere(double radius, int slices, int stacks);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidSphere(double radius, int slices, int stacks);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireCone(double baseRadius, double height, int slices, int stacks);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidCone(double baseRadius, double height, int slices, int stacks);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireCube(double size);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidCube(double size);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireTorus(double innerRadius, double outerRadius, int sides, int rings);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidTorus(double innerRadius, double outerRadius, int sides, int rings);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireDodecahedron();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidDodecahedron();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireTeapot(double size);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidTeapot(double size);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireOctahedron();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidOctahedron();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireTetrahedron();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidTetrahedron();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireIcosahedron();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidIcosahedron();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutVideoResizeGet(int param);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetupVideoResizing();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutStopVideoResizing();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutVideoResize(int x, int y, int width, int height);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutVideoPan(int x, int y, int width, int height);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutReportErrors();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutIgnoreKeyRepeat(int ignore);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetKeyRepeat(int repeatMode);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutForceJoystickFunc();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutGameModeString(string str);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutEnterGameMode();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutLeaveGameMode();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutGameModeGet(int mode);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutMainLoopEvent();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutLeaveMainLoop();

        public static void glutMouseWheelFunc([In] MouseWheelCallback func)
        {
            mouseWheelCallback = func;
            __glutMouseWheelFunc(mouseWheelCallback);
        }

        public static void glutCloseFunc([In] CloseCallback func)
        {
            closeCallback = func;
            __glutCloseFunc(closeCallback);
        }

        public static void glutWMCloseFunc([In] WindowCloseCallback func)
        {
            windowCloseCallback = func;
            __glutWMCloseFunc(windowCloseCallback);
        }

        public static void glutMenuDestroyFunc([In] MenuDestroyCallback func)
        {
            menuDestroyCallback = func;
            __glutMenuDestroyFunc(menuDestroyCallback);
        }

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetOption(int optionFlag, int value);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr glutGetWindowData();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetupWindowData(IntPtr data);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr glutGetMenuData();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSetMenuData(IntPtr data);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutBitmapHeight(IntPtr font);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern float glutStrokeHeight(IntPtr font);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutBitmapString(IntPtr font, string str);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutStrokeString(IntPtr font, string str);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireRhombicDodecahedron();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidRhombicDodecahedron();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireSierpinskiSponge(int levels, double[] offset, double scale);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidSierpinskiSponge(int levels, double[] offset, double scale);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutWireCylinder(double radius, double height, int slices, int stacks);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutSolidCylinder(double radius, double height, int slices, int stacks);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr glutGetProcAddress(string procName);

        #endregion
    }
}