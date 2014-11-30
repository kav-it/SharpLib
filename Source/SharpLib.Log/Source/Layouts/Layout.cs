using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    [LogConfigurationItem]
    public abstract class Layout : ISupportsInitialize, IRenderable
    {
        #region Поля

        private bool isInitialized;

        private bool threadAgnostic;

        #endregion

        #region Свойства

        internal bool IsThreadAgnostic
        {
            get { return threadAgnostic; }
        }

        protected LoggingConfiguration LoggingConfiguration { get; private set; }

        #endregion

        #region Методы

        public static Layout FromString(string layoutText)
        {
            return FromString(layoutText, ConfigurationItemFactory.Default);
        }

        public static Layout FromString(string layoutText, ConfigurationItemFactory configurationItemFactory)
        {
            return new SimpleLayout(layoutText, configurationItemFactory);
        }

        public virtual void Precalculate(LogEventInfo logEvent)
        {
            if (!threadAgnostic)
            {
                Render(logEvent);
            }
        }

        public string Render(LogEventInfo logEvent)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                InitializeLayout();
            }

            return GetFormattedMessage(logEvent);
        }

        void ISupportsInitialize.Initialize(LoggingConfiguration configuration)
        {
            Initialize(configuration);
        }

        void ISupportsInitialize.Close()
        {
            Close();
        }

        internal void Initialize(LoggingConfiguration configuration)
        {
            if (!isInitialized)
            {
                LoggingConfiguration = configuration;
                isInitialized = true;

                threadAgnostic = true;
                foreach (object item in ObjectGraphScanner.FindReachableObjects<object>(this))
                {
                    if (!item.GetType().IsDefined(typeof(ThreadAgnosticAttribute), true))
                    {
                        threadAgnostic = false;
                        break;
                    }
                }

                InitializeLayout();
            }
        }

        internal void Close()
        {
            if (isInitialized)
            {
                LoggingConfiguration = null;
                isInitialized = false;
                CloseLayout();
            }
        }

        protected virtual void InitializeLayout()
        {
        }

        protected virtual void CloseLayout()
        {
        }

        protected abstract string GetFormattedMessage(LogEventInfo logEvent);

        #endregion

        public static implicit operator Layout([Localizable(false)] string text)
        {
            return FromString(text);
        }
    }
}
