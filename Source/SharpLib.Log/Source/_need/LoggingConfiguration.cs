using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using NLog.Common;
using NLog.Internal;
using NLog.Targets;

namespace NLog.Config
{
    public class LoggingConfiguration
    {
        #region Поля

        private readonly IDictionary<string, Target> _targets;

        private object[] _configItems;

        #endregion

        #region Свойства

        public ReadOnlyCollection<Target> ConfiguredNamedTargets
        {
            get { return new List<Target>(_targets.Values).AsReadOnly(); }
        }

        public virtual IEnumerable<string> FileNamesToWatch
        {
            get { return new string[0]; }
        }

        public IList<LoggingRule> LoggingRules { get; private set; }

        public CultureInfo DefaultCultureInfo
        {
            get { return LogManager.Instance.DefaultCultureInfo(); }
            set { LogManager.Instance.DefaultCultureInfo = () => value; }
        }

        public ReadOnlyCollection<Target> AllTargets
        {
            get { return _configItems.OfType<Target>().ToList().AsReadOnly(); }
        }

        #endregion

        #region Конструктор

        public LoggingConfiguration()
        {
            _targets = new Dictionary<string, Target>(StringComparer.OrdinalIgnoreCase);
            LoggingRules = new List<LoggingRule>();
        }

        #endregion

        #region Методы

        public void AddTarget(string name, Target target)
        {
            if (name == null)
            {
                throw new ArgumentException("Target name cannot be null", "name");
            }

            InternalLogger.Debug("Registering target {0}: {1}", name, target.GetType().FullName);
            _targets[name] = target;
        }

        public Target FindTargetByName(string name)
        {
            Target value;

            if (!_targets.TryGetValue(name, out value))
            {
                return null;
            }

            return value;
        }

        public virtual LoggingConfiguration Reload()
        {
            return this;
        }

        public void RemoveTarget(string name)
        {
            _targets.Remove(name);
        }

        public void Install(InstallationContext installationContext)
        {
            if (installationContext == null)
            {
                throw new ArgumentNullException("installationContext");
            }

            InitializeAll();
            foreach (IInstallable installable in _configItems.OfType<IInstallable>())
            {
                installationContext.Info("Installing '{0}'", installable);

                try
                {
                    installable.Install(installationContext);
                    installationContext.Info("Finished installing '{0}'.", installable);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    installationContext.Error("'{0}' installation failed: {1}.", installable, exception);
                }
            }
        }

        public void Uninstall(InstallationContext installationContext)
        {
            if (installationContext == null)
            {
                throw new ArgumentNullException("installationContext");
            }

            InitializeAll();

            foreach (IInstallable installable in _configItems.OfType<IInstallable>())
            {
                installationContext.Info("Uninstalling '{0}'", installable);

                try
                {
                    installable.Uninstall(installationContext);
                    installationContext.Info("Finished uninstalling '{0}'.", installable);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    installationContext.Error("Uninstallation of '{0}' failed: {1}.", installable, exception);
                }
            }
        }

        internal void Close()
        {
            InternalLogger.Debug("Closing logging configuration...");
            foreach (ISupportsInitialize initialize in _configItems.OfType<ISupportsInitialize>())
            {
                InternalLogger.Trace("Closing {0}", initialize);
                try
                {
                    initialize.Close();
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    InternalLogger.Warn("Exception while closing {0}", exception);
                }
            }

            InternalLogger.Debug("Finished closing logging configuration.");
        }

        internal void Dump()
        {
            InternalLogger.Debug("--- NLog configuration dump. ---");
            InternalLogger.Debug("Targets:");
            foreach (Target target in _targets.Values)
            {
                InternalLogger.Info("{0}", target);
            }

            InternalLogger.Debug("Rules:");
            foreach (LoggingRule rule in LoggingRules)
            {
                InternalLogger.Info("{0}", rule);
            }

            InternalLogger.Debug("--- End of NLog configuration dump ---");
        }

        internal void FlushAllTargets(AsyncContinuation asyncContinuation)
        {
            var uniqueTargets = new List<Target>();
            foreach (var rule in LoggingRules)
            {
                foreach (var t in rule.Targets)
                {
                    if (!uniqueTargets.Contains(t))
                    {
                        uniqueTargets.Add(t);
                    }
                }
            }

            AsyncHelpers.ForEachItemInParallel(uniqueTargets, asyncContinuation, (target, cont) => target.Flush(cont));
        }

        internal void ValidateConfig()
        {
            var roots = new List<object>();
            foreach (LoggingRule r in LoggingRules)
            {
                roots.Add(r);
            }

            foreach (Target target in _targets.Values)
            {
                roots.Add(target);
            }

            _configItems = ObjectGraphScanner.FindReachableObjects<object>(roots.ToArray());

            InternalLogger.Info("Found {0} configuration items", _configItems.Length);

            foreach (object o in _configItems)
            {
                PropertyHelper.CheckRequiredParameters(o);
            }
        }

        internal void InitializeAll()
        {
            ValidateConfig();

            foreach (ISupportsInitialize initialize in _configItems.OfType<ISupportsInitialize>().Reverse())
            {
                InternalLogger.Trace("Initializing {0}", initialize);

                try
                {
                    initialize.Initialize(this);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    if (LogManager.Instance.ThrowExceptions)
                    {
                        throw new NLogConfigurationException("Error during initialization of " + initialize, exception);
                    }
                }
            }
        }

        internal void EnsureInitialized()
        {
            InitializeAll();
        }

        #endregion
    }
}