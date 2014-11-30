using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace SharpLib.Log
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

            _targets[name] = target;
        }

        public Target FindTargetByName(string name)
        {
            Target value;

            return !_targets.TryGetValue(name, out value) ? null : value;
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
            foreach (ISupportsInitialize initialize in _configItems.OfType<ISupportsInitialize>())
            {
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
                }
            }
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
            var roots = LoggingRules.Cast<object>().ToList();

            roots.AddRange(_targets.Values);

            _configItems = ObjectGraphScanner.FindReachableObjects<object>(roots.ToArray());

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
                        throw new Exception("Error during initialization of " + initialize, exception);
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
