using System;

namespace SharpLib.Log
{
    internal class Win32FileInfoHelper : FileInfoHelper
    {
        #region Методы

        public override bool GetFileInfo(string fileName, IntPtr fileHandle, out DateTime lastWriteTime, out long fileLength)
        {
            Win32FileNativeMethods.BY_HANDLE_FILE_INFORMATION fi;

            if (Win32FileNativeMethods.GetFileInformationByHandle(fileHandle, out fi))
            {
                lastWriteTime = DateTime.FromFileTime(fi.ftLastWriteTime);
                fileLength = fi.nFileSizeLow + (((long)fi.nFileSizeHigh) << 32);
                return true;
            }
            lastWriteTime = DateTime.MinValue;
            fileLength = -1;
            return false;
        }

        #endregion
    }
}