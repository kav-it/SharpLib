﻿namespace SharpLib.Log
{
    public interface IInstallable
    {
        #region Методы

        void Install(InstallationContext installationContext);

        void Uninstall(InstallationContext installationContext);

        bool? IsInstalled(InstallationContext installationContext);

        #endregion
    }
}