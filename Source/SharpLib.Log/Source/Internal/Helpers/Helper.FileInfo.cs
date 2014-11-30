using System;

namespace SharpLib.Log
{
    internal abstract class FileInfoHelper
    {
        #region Свойства

        internal static FileInfoHelper Helper { get; private set; }

        #endregion

        #region Конструктор

        static FileInfoHelper()
        {
            if (PlatformDetector.IsDesktopWin32)
            {
                Helper = new Win32FileInfoHelper();
            }
            else
            {
                Helper = new PortableFileInfoHelper();
            }
        }

        #endregion

        #region Методы

        public abstract bool GetFileInfo(string fileName, IntPtr fileHandle, out DateTime lastWriteTime, out long fileLength);

        #endregion
    }
}