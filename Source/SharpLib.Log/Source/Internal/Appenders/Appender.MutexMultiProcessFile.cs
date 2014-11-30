using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    [SecuritySafeCritical]
    internal class MutexMultiProcessFileAppender : BaseFileAppender
    {
        #region Поля

        public static readonly IFileAppenderFactory TheFactory = new Factory();

        private FileStream _file;

        private Mutex _mutex;

        #endregion

        #region Конструктор

        public MutexMultiProcessFileAppender(string fileName, ICreateFileParameters parameters)
            : base(fileName, parameters)
        {
            try
            {
                _mutex = CreateSharableMutex(GetMutexName(fileName));
                _file = CreateFileStream(true);
            }
            catch
            {
                if (_mutex != null)
                {
                    _mutex.Close();
                    _mutex = null;
                }

                if (_file != null)
                {
                    _file.Close();
                    _file = null;
                }

                throw;
            }
        }

        #endregion

        #region Методы

        public override void Write(byte[] bytes)
        {
            if (_mutex == null)
            {
                return;
            }

            try
            {
                _mutex.WaitOne();
            }
            catch (AbandonedMutexException)
            {
            }

            try
            {
                _file.Seek(0, SeekOrigin.End);
                _file.Write(bytes, 0, bytes.Length);
                _file.Flush();
                FileTouched();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public override void Close()
        {
            if (_mutex != null)
            {
                _mutex.Close();
            }

            if (_file != null)
            {
                _file.Close();
            }

            _mutex = null;
            _file = null;
            FileTouched();
        }

        public override void Flush()
        {
        }

        public override bool GetFileInfo(out DateTime lastWriteTime, out long fileLength)
        {
            return FileInfoHelper.Helper.GetFileInfo(FileName, _file.SafeFileHandle.DangerousGetHandle(), out lastWriteTime, out fileLength);
        }

        private static Mutex CreateSharableMutex(string name)
        {
            var mutexSecurity = new MutexSecurity();
            var everyoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            mutexSecurity.AddAccessRule(new MutexAccessRule(everyoneSid, MutexRights.FullControl, AccessControlType.Allow));

            bool createdNew;
            return new Mutex(false, name, out createdNew, mutexSecurity);
        }

        private static string GetMutexName(string fileName)
        {
            const string MUTEX_NAME_PREFIX = @"Global\Log-FileLock-";
            const int MAX_MUTEX_NAME_LENGTH = 260;

            string canonicalName = Path.GetFullPath(fileName).ToLowerInvariant();

            canonicalName = canonicalName.Replace('\\', '/');

            if (MUTEX_NAME_PREFIX.Length + canonicalName.Length <= MAX_MUTEX_NAME_LENGTH)
            {
                return MUTEX_NAME_PREFIX + canonicalName;
            }

            string hash;
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(canonicalName));
                hash = Convert.ToBase64String(bytes);
            }

            int cutOffIndex = canonicalName.Length - (MAX_MUTEX_NAME_LENGTH - MUTEX_NAME_PREFIX.Length - hash.Length);
            return MUTEX_NAME_PREFIX + hash + canonicalName.Substring(cutOffIndex);
        }

        #endregion

        #region Вложенный класс: Factory

        private class Factory : IFileAppenderFactory
        {
            #region Методы

            BaseFileAppender IFileAppenderFactory.Open(string fileName, ICreateFileParameters parameters)
            {
                return new MutexMultiProcessFileAppender(fileName, parameters);
            }

            #endregion
        }

        #endregion
    }
}