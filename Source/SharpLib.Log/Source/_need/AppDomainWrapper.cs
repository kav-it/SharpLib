using System;
using System.Collections.Generic;

namespace NLog.Internal.Fakeables
{
    public class AppDomainWrapper : IAppDomain
    {
        #region ��������

        public static AppDomainWrapper CurrentDomain
        {
            get { return new AppDomainWrapper(AppDomain.CurrentDomain); }
        }

        public string BaseDirectory { get; private set; }

        public string ConfigurationFile { get; private set; }

        public IEnumerable<string> PrivateBinPath { get; private set; }

        public string FriendlyName { get; private set; }

        #endregion

        #region �������

        public event EventHandler<EventArgs> DomainUnload;

        public event EventHandler<EventArgs> ProcessExit;

        #endregion

        #region �����������

        public AppDomainWrapper(AppDomain appDomain)
        {
            BaseDirectory = appDomain.BaseDirectory;
            ConfigurationFile = appDomain.SetupInformation.ConfigurationFile;

            string privateBinPath = appDomain.SetupInformation.PrivateBinPath;
            PrivateBinPath = string.IsNullOrEmpty(privateBinPath)
                ? new string[] { }
                : appDomain.SetupInformation.PrivateBinPath.Split(new[] { ';' },
                    StringSplitOptions.RemoveEmptyEntries);
            FriendlyName = appDomain.FriendlyName;
            appDomain.ProcessExit += OnProcessExit;
            appDomain.DomainUnload += OnDomainUnload;
        }

        #endregion

        #region ������

        private void OnDomainUnload(object sender, EventArgs e)
        {
            var handler = DomainUnload;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private void OnProcessExit(object sender, EventArgs eventArgs)
        {
            var handler = ProcessExit;
            if (handler != null)
            {
                handler(sender, eventArgs);
            }
        }

        #endregion
    }
}