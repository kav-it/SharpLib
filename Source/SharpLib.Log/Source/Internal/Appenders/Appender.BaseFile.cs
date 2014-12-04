using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpLib.Log
{
    [SecuritySafeCritical]
    internal abstract class BaseFileAppender : IDisposable
    {
        #region Поля

        private readonly Random _random;

        #endregion

        #region Свойства

        public string FileName { get; private set; }

        public DateTime LastWriteTime { get; private set; }

        public DateTime OpenTime { get; private set; }

        public ICreateFileParameters CreateFileParameters { get; private set; }

        #endregion

        #region Конструктор

        protected BaseFileAppender(string fileName, ICreateFileParameters createParameters)
        {
            _random = new Random();
            CreateFileParameters = createParameters;
            FileName = fileName;
            OpenTime = TimeSource.Current.Time.ToLocalTime();
            LastWriteTime = DateTime.MinValue;
        }

        #endregion

        #region Методы

        public abstract void Write(byte[] bytes);

        public abstract void Flush();

        public abstract void Close();

        public abstract bool GetFileInfo(out DateTime lastWriteTime, out long fileLength);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        protected void FileTouched()
        {
            LastWriteTime = TimeSource.Current.Time.ToLocalTime();
        }

        protected void FileTouched(DateTime dateTime)
        {
            LastWriteTime = dateTime;
        }

        protected FileStream CreateFileStream(bool allowConcurrentWrite)
        {
            int currentDelay = CreateFileParameters.ConcurrentWriteAttemptDelay;

            for (int i = 0; i < CreateFileParameters.ConcurrentWriteAttempts; ++i)
            {
                try
                {
                    try
                    {
                        return TryCreateFileStream(allowConcurrentWrite);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        if (!CreateFileParameters.CreateDirs)
                        {
                            throw;
                        }

                        Directory.CreateDirectory(Path.GetDirectoryName(FileName));
                        return TryCreateFileStream(allowConcurrentWrite);
                    }
                }
                catch (IOException)
                {
                    if (!CreateFileParameters.ConcurrentWrites || !allowConcurrentWrite || i + 1 == CreateFileParameters.ConcurrentWriteAttempts)
                    {
                        throw;
                    }

                    int actualDelay = _random.Next(currentDelay);
                    currentDelay *= 2;
                    System.Threading.Thread.Sleep(actualDelay);
                }
            }

            throw new InvalidOperationException("Should not be reached.");
        }

        private FileStream WindowsCreateFile(string fileName, bool allowConcurrentWrite)
        {
            int fileShare = Win32FileNativeMethods.FILE_SHARE_READ;

            if (allowConcurrentWrite)
            {
                fileShare |= Win32FileNativeMethods.FILE_SHARE_WRITE;
            }

            if (CreateFileParameters.EnableFileDelete && PlatformDetector.CurrentOS != RuntimeOS.Windows)
            {
                fileShare |= Win32FileNativeMethods.FILE_SHARE_DELETE;
            }

            IntPtr handle = Win32FileNativeMethods.CreateFile(
                fileName,
                Win32FileNativeMethods.FileAccess.GenericWrite,
                fileShare,
                IntPtr.Zero,
                Win32FileNativeMethods.CreationDisposition.OpenAlways,
                CreateFileParameters.FileAttributes,
                IntPtr.Zero);

            if (handle.ToInt32() == -1)
            {
                var result = Marshal.GetHRForLastWin32Error();
                Marshal.ThrowExceptionForHR(result);
            }

            var safeHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(handle, true);
            var returnValue = new FileStream(safeHandle, FileAccess.Write, CreateFileParameters.BufferSize);
            returnValue.Seek(0, SeekOrigin.End);
            return returnValue;
        }

        private FileStream TryCreateFileStream(bool allowConcurrentWrite)
        {
            FileShare fileShare = FileShare.Read;

            if (allowConcurrentWrite)
            {
                fileShare = FileShare.ReadWrite;
            }

            if (CreateFileParameters.EnableFileDelete && PlatformDetector.CurrentOS != RuntimeOS.Windows)
            {
                fileShare |= FileShare.Delete;
            }

            try
            {
                if (!CreateFileParameters.ForceManaged && PlatformDetector.IsDesktopWin32)
                {
                    return WindowsCreateFile(FileName, allowConcurrentWrite);
                }
            }
            catch (SecurityException)
            {
            }

            return new FileStream(
                FileName,
                FileMode.Append,
                FileAccess.Write,
                fileShare,
                CreateFileParameters.BufferSize);
        }

        #endregion
    }
}