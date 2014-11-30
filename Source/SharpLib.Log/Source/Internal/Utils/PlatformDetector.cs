using System;
using System.Collections.Generic;
using System.IO;

namespace SharpLib.Log
{
    internal static class PlatformDetector
    {
        #region Поля

        private static readonly RuntimeOS _currentOs = GetCurrentRuntimeOS();

        #endregion

        #region Свойства

        public static RuntimeOS CurrentOS
        {
            get { return _currentOs; }
        }

        public static bool IsDesktopWin32
        {
            get { return _currentOs == RuntimeOS.Windows || _currentOs == RuntimeOS.WindowsNT; }
        }

        public static bool IsWin32
        {
            get { return _currentOs == RuntimeOS.Windows || _currentOs == RuntimeOS.WindowsNT || _currentOs == RuntimeOS.WindowsCE; }
        }

        public static bool IsUnix
        {
            get { return _currentOs == RuntimeOS.Unix; }
        }

        #endregion

        #region Методы

        private static RuntimeOS GetCurrentRuntimeOS()
        {
            PlatformID platformID = Environment.OSVersion.Platform;
            if ((int)platformID == 4 || (int)platformID == 128)
            {
                return RuntimeOS.Unix;
            }

            if ((int)platformID == 3)
            {
                return RuntimeOS.WindowsCE;
            }

            if (platformID == PlatformID.Win32Windows)
            {
                return RuntimeOS.Windows;
            }

            if (platformID == PlatformID.Win32NT)
            {
                return RuntimeOS.WindowsNT;
            }

            return RuntimeOS.Unknown;
        }

        #endregion
    }
}