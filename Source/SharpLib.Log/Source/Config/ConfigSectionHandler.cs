using System;
using System.Configuration;
using System.Xml;

using NLog.Common;
using NLog.Internal;
using NLog.Internal.Fakeables;

namespace NLog.Config
{
    public sealed class ConfigSectionHandler : IConfigurationSectionHandler
    {
        #region Ìועמהû

        private object Create(XmlNode section, IAppDomain appDomain)
        {
            try
            {
                string configFileName = appDomain.ConfigurationFile;

                return new XmlLoggingConfiguration((XmlElement)section, configFileName);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("ConfigSectionHandler error: {0}", exception);
                throw;
            }
        }

        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            return Create(section, AppDomainWrapper.CurrentDomain);
        }

        #endregion
    }
}
