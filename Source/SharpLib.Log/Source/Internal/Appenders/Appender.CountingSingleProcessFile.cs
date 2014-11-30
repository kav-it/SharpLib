using System;
using System.IO;
using System.Security;

namespace SharpLib.Log
{
    [SecuritySafeCritical]
    internal class CountingSingleProcessFileAppender : BaseFileAppender
    {
        #region Поля

        public static readonly IFileAppenderFactory TheFactory = new Factory();

        private long _currentFileLength;

        private FileStream _file;

        #endregion

        #region Конструктор

        public CountingSingleProcessFileAppender(string fileName, ICreateFileParameters parameters)
            : base(fileName, parameters)
        {
            var fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                FileTouched(fi.LastWriteTime);
                _currentFileLength = fi.Length;
            }
            else
            {
                FileTouched();
                _currentFileLength = 0;
            }

            _file = CreateFileStream(false);
        }

        #endregion

        #region Методы

        public override void Close()
        {
            if (_file != null)
            {
                _file.Close();
                _file = null;
            }
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

        public override bool GetFileInfo(out DateTime lastWriteTime, out long fileLength)
        {
            lastWriteTime = LastWriteTime;
            fileLength = _currentFileLength;
            return true;
        }

        public override void Write(byte[] bytes)
        {
            if (_file == null)
            {
                return;
            }

            _currentFileLength += bytes.Length;
            _file.Write(bytes, 0, bytes.Length);
            FileTouched();
        }

        #endregion

        #region Вложенный класс: Factory

        private class Factory : IFileAppenderFactory
        {
            #region Методы

            BaseFileAppender IFileAppenderFactory.Open(string fileName, ICreateFileParameters parameters)
            {
                return new CountingSingleProcessFileAppender(fileName, parameters);
            }

            #endregion
        }

        #endregion
    }
}