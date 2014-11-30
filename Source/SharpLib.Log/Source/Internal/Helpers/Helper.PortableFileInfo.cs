using System;
using System.IO;

namespace SharpLib.Log
{
    internal class PortableFileInfoHelper : FileInfoHelper
    {
        #region Методы

        public override bool GetFileInfo(string fileName, IntPtr fileHandle, out DateTime lastWriteTime, out long fileLength)
        {
            var fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                fileLength = fi.Length;
                lastWriteTime = fi.LastWriteTime;
                return true;
            }
            fileLength = -1;
            lastWriteTime = DateTime.MinValue;
            return false;
        }

        #endregion
    }
}