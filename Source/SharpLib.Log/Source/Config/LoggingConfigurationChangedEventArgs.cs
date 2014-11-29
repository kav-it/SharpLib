﻿using System;

namespace NLog.Config
{
    public class LoggingConfigurationChangedEventArgs : EventArgs
    {
        #region Свойства

        public LoggingConfiguration OldConfiguration { get; private set; }

        public LoggingConfiguration NewConfiguration { get; private set; }

        #endregion

        #region Конструктор

        internal LoggingConfigurationChangedEventArgs(LoggingConfiguration oldConfiguration, LoggingConfiguration newConfiguration)
        {
            OldConfiguration = oldConfiguration;
            NewConfiguration = newConfiguration;
        }

        #endregion
    }
}