using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SharpLib.OpenGL
{
    public class Glut
    {
        #region Делегаты

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DisplayCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MouseCallback(int button, int state, int x, int y);

        #endregion

        #region Константы

        private const CallingConvention CALLING_CONVENTION = CallingConvention.Winapi;

        private const string FREEGLUT_NATIVE_LIBRARY = "glut32.dll";

        public const int GLUT_RGB = 0;

        public const int GLUT_SINGLE = 0;

        #endregion

        #region Поля

        private static DisplayCallback _displayCallback;

        #endregion

        #region Методы

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

        public static void glutDisplayFunc([In] DisplayCallback func)
        {
            _displayCallback = func;
            __glutDisplayFunc(_displayCallback);
        }

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutInit(ref int argcp, StringBuilder[] argv);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutInitDisplayMode(int mode);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutInitWindowSize(int width, int height);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutInitWindowPosition(int x, int y);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern int glutCreateWindow(string name);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutDisplayFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutDisplayFunc(DisplayCallback func);

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION), SuppressUnmanagedCodeSecurity]
        public static extern void glutMainLoop();

        [DllImport(FREEGLUT_NATIVE_LIBRARY, CallingConvention = CALLING_CONVENTION, EntryPoint = "glutMouseFunc"), SuppressUnmanagedCodeSecurity]
        private static extern void __glutMouseFunc(MouseCallback func);


        #endregion
    }
}