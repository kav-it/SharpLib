using System;
using System.IO;
using System.Security;

namespace SharpLib.Log
{
    [SecuritySafeCritical]
    internal class SingleProcessFileAppender : BaseFileAppender
    {
        #region Поля

        public static readonly IFileAppenderFactory TheFactory = new Factory();

        private FileStream _file;

        #endregion

        #region Конструктор

        public SingleProcessFileAppender(string fileName, ICreateFileParameters parameters)
            : base(fileName, parameters)
        {
            var fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                FileTouched(fi.LastWriteTime);
            }
            else
            {
                FileTouched();
            }
            _file = CreateFileStream(false);
        }

        #endregion

        #region Методы

        public override void Write(byte[] bytes)
        {
            if (_file == null)
            {
                return;
            }

            _file.Write(bytes, 0, bytes.Length);
            FileTouched();
        }

        public override void Flush()
        {
            if (_file == null)
            {
                return;
            }

            _file.Flush();
            FileTouched();
        }

        public override void Close()
        {
            if (_file == null)
            {
                return;
            }

            _file.Close();
            _file = null;
        }

        public override bool GetFileInfo(out DateTime lastWriteTime, out long fileLength)
        {
            if (_file != null)
            {
                lastWriteTime = LastWriteTime;
                fileLength = _file.Length;
                return true;
            }
            lastWriteTime = new DateTime();
            fileLength = 0;
            return false;
        }

        #endregion

        #region Вложенный класс: Factory

        private class Factory : IFileAppenderFactory
        {
            #region Методы

            BaseFileAppender IFileAppenderFactory.Open(string fileName, ICreateFileParameters parameters)
            {
                return new SingleProcessFileAppender(fileName, parameters);
            }

            #endregion
        }

        #endregion
    }
}