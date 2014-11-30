using System;
using System.IO;
using System.Security;

namespace SharpLib.Log
{
    [SecuritySafeCritical]
    internal class RetryingMultiProcessFileAppender : BaseFileAppender
    {
        #region Поля

        public static readonly IFileAppenderFactory TheFactory = new Factory();

        #endregion

        #region Конструктор

        public RetryingMultiProcessFileAppender(string fileName, ICreateFileParameters parameters)
            : base(fileName, parameters)
        {
        }

        #endregion

        #region Методы

        public override void Write(byte[] bytes)
        {
            using (FileStream fileStream = CreateFileStream(false))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }

            FileTouched();
        }

        public override void Flush()
        {
        }

        public override void Close()
        {
        }

        public override bool GetFileInfo(out DateTime lastWriteTime, out long fileLength)
        {
            FileInfo fi = new FileInfo(FileName);
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

        #region Вложенный класс: Factory

        private class Factory : IFileAppenderFactory
        {
            #region Методы

            BaseFileAppender IFileAppenderFactory.Open(string fileName, ICreateFileParameters parameters)
            {
                return new RetryingMultiProcessFileAppender(fileName, parameters);
            }

            #endregion
        }

        #endregion
    }
}