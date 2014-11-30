using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SharpLib.Log
{
    internal class PortableThreadIdHelper : ThreadIdHelper
    {
        #region Константы

        private const string UNKNOWN_PROCESS_NAME = "<unknown>";

        #endregion

        #region Поля

        private readonly int _currentProcessId;

        private string _currentProcessBaseName;

        private string _currentProcessName;

        #endregion

        #region Свойства

        public override int CurrentThreadId
        {
            get { return Thread.CurrentThread.ManagedThreadId; }
        }

        public override int CurrentProcessId
        {
            get { return _currentProcessId; }
        }

        public override string CurrentProcessName
        {
            get
            {
                GetProcessName();
                return _currentProcessName;
            }
        }

        public override string CurrentProcessBaseName
        {
            get
            {
                GetProcessName();
                return _currentProcessBaseName;
            }
        }

        #endregion

        #region Конструктор

        public PortableThreadIdHelper()
        {
            _currentProcessId = Process.GetCurrentProcess().Id;
        }

        #endregion

        #region Методы

        private void GetProcessName()
        {
            if (_currentProcessName == null)
            {
                try
                {
                    _currentProcessName = Process.GetCurrentProcess().MainModule.FileName;
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    _currentProcessName = UNKNOWN_PROCESS_NAME;
                }

                _currentProcessBaseName = Path.GetFileNameWithoutExtension(_currentProcessName);
            }
        }

        #endregion
    }
}