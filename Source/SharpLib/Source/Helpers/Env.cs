using System;

namespace SharpLib
{
    /// <summary>
    /// Информация об окружении приложения
    /// </summary>
    public class Env
    {
        #region Свойства

        public static bool IsWindowsVistaOrLater
        {
            get { return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 0, 6000); }
        }

        public static bool IsWindowsXpOrLater
        {
            get { return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(5, 1, 2600); }
        }

        #endregion
    }
}