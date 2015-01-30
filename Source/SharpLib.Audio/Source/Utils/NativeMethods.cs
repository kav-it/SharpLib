﻿using System;
using System.Runtime.InteropServices;

namespace SharpLib.Audio.Utils
{
    internal class NativeMethods
    {
#if !NETFX_CORE
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);
#endif

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }
}