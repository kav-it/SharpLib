using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// </summary>
        public virtual IEnumerable<string> FileNamesToWatch
        {
            get { return new string[0]; }
        }

        /// <summary>
        /// Правила вывода/фильтрации сообщений
        /// </summary>
        public IList<LoggingRule> Rules { get; private set; }

        /// <summary>
        /// Объекты логгирования
        /// </summary>
        public ReadOnlyCollection<Target> Targets
        {
            get { return _configItems.OfType<Target>().ToList().AsReadOnly(); }
        }

        #endregion

        #region Конструктор

        public LoggingConfiguration()
        {
            _targets = new Dictionary<string, Target>(StringComparer.OrdinalIgnoreCase);
            Rules = new List<LoggingRule>();
        }

        #endregion

        #region Методы

        public void AddTarget(Target target)
        {
            if (target.Name.IsNotValid())
            {
                throw new ArgumentException("Target name cannot be null", "name");
            }

            _targets[target.Name] = target;
        }

        public void AddTarget(Target target, LogLevel minLevel)
        {
            var rule = new LoggingRule("*", minLevel, target);
            AddTarget(target);
            Rules.Add(rule);

            LogManager.Instance.Reconfig();
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
            foreach (var rule in Rules)
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
            var roots = Rules.Cast<object>().ToList();

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