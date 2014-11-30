using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    [SecuritySafeCritical]
    internal class Win32ThreadIdHelper : ThreadIdHelper
    {
        #region Поля

        private readonly string _currentProcessBaseName;

        private readonly int _currentProcessId;

        private readonly string _currentProcessName;

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
            get { return _currentProcessName; }
        }

        public override string CurrentProcessBaseName
        {
            get { return _currentProcessBaseName; }
        }

        #endregion

        #region Конструктор

        public Win32ThreadIdHelper()
        {
            _currentProcessId = NativeMethods.GetCurrentProcessId();

            var sb = new StringBuilder(512);
            if (0 == NativeMethods.GetModuleFileName(IntPtr.Zero, sb, sb.Capacity))
            {
                throw new InvalidOperationException("Cannot determine program name.");
            }

            _currentProcessName = sb.ToString();
            _currentProcessBaseName = Path.GetFileNameWithoutExtension(_currentProcessName);
        }

        #endregion
    }
}