using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Internal.Fakeables;
using NLog.Targets;

namespace NLog
{
    public class LogFactory : IDisposable
    {
        #region Константы

        private const int RECONFIG_AFTER_FILE_CHANGED_TIMEOUT = 1000;

        #endregion

        #region Поля

        private static readonly TimeSpan _defaultFlushTimeout = TimeSpan.FromSeconds(15);

        private static IAppDomain _currentAppDomain;

        private readonly Dictionary<LoggerCacheKey, WeakReference> _loggerCache;

        private readonly MultiFileWatcher _watcher;

        private LoggingConfiguration _config;

        private bool _configLoaded;

        private LogLevel _globalThreshold;

        private int _logsEnabled;

        private Timer _reloadTimer;

        #endregion

        #region Свойства

        public static IAppDomain CurrentAppDomain
        {
            get { return _currentAppDomain ?? (_currentAppDomain = AppDomainWrapper.CurrentDomain); }
            set { _currentAppDomain = value; }
        }

        public bool ThrowExceptions { get; set; }

        public LoggingConfiguration Configuration
        {
            get
            {
                lock (this)
                {
                    if (_configLoaded)
                    {
                        return _config;
                    }

                    _configLoaded = true;

                    if (_config == null)
                    {
                        _config = XmlLoggingConfiguration.AppConfig;
                    }

                    if (_config == null)
                    {
                        foreach (string configFile in GetCandidateFileNames())
                        {
                            if (File.Exists(configFile))
                            {
                                InternalLogger.Debug("Attempting to load config from {0}", configFile);
                                _config = new XmlLoggingConfiguration(configFile);
                                break;
                            }
                        }
                    }

                    if (_config != null)
                    {
                        Dump(_config);
                        try
                        {
                            _watcher.Watch(_config.FileNamesToWatch);
                        }
                        catch (Exception exception)
                        {
                            InternalLogger.Warn("Cannot start file watching: {0}. File watching is disabled", exception);
                        }
                    }
                    if (_config != null)
                    {
                        _config.InitializeAll();
                    }

                    return _config;
                }
            }

            set
            {
                try
                {
                    _watcher.StopWatching();
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    InternalLogger.Error("Cannot stop file watching: {0}", exception);
                }

                lock (this)
                {
                    LoggingConfiguration oldConfig = _config;
                    if (oldConfig != null)
                    {
                        InternalLogger.Info("Closing old configuration.");
                        Flush();
                        oldConfig.Close();
                    }

                    _config = value;
                    _configLoaded = true;

                    if (_config != null)
                    {
                        Dump(_config);

                        _config.InitializeAll();
                        ReconfigExistingLoggers(_config);
                        try
                        {
                            _watcher.Watch(_config.FileNamesToWatch);
                        }
                        catch (Exception exception)
                        {
                            if (exception.MustBeRethrown())
                            {
                                throw;
                            }

                            InternalLogger.Warn("Cannot start file watching: {0}", exception);
                        }
                    }

                    var configurationChangedDelegate = ConfigurationChanged;

                    if (configurationChangedDelegate != null)
                    {
                        configurationChangedDelegate(this, new LoggingConfigurationChangedEventArgs(oldConfig, value));
                    }
                }
            }
        }

        public LogLevel GlobalThreshold
        {
            get { return _globalThreshold; }

            set
            {
                lock (this)
                {
                    _globalThreshold = value;
                    ReconfigExistingLoggers();
                }
            }
        }

        #endregion

        #region События

        public event EventHandler<LoggingConfigurationChangedEventArgs> ConfigurationChanged;

        public event EventHandler<LoggingConfigurationReloadedEventArgs> ConfigurationReloaded;

        #endregion

        #region Конструктор

        public LogFactory()
        {
            _globalThreshold = LogLevel.MinLevel;
            _loggerCache = new Dictionary<LoggerCacheKey, WeakReference>();
            _watcher = new MultiFileWatcher();
            _watcher.OnChange += ConfigFileChanged;
        }

        public LogFactory(LoggingConfiguration config)
            : this()
        {
            Configuration = config;
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Logger CreateNullLogger()
        {
            TargetWithFilterChain[] targetsByLevel = new TargetWithFilterChain[LogLevel.MaxLevel.Ordinal + 1];
            Logger newLogger = new Logger();
            newLogger.Initialize(string.Empty, new LoggerConfiguration(targetsByLevel), this);
            return newLogger;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Logger GetCurrentClassLogger()
        {
            var frame = new StackFrame(1, false);

            return GetLogger(frame.GetMethod().DeclaringType.FullName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Logger GetCurrentClassLogger(Type loggerType)
        {
            var frame = new StackFrame(1, false);

            return GetLogger(frame.GetMethod().DeclaringType.FullName, loggerType);
        }

        public Logger GetLogger(string name)
        {
            return GetLogger(new LoggerCacheKey(typeof(Logger), name));
        }

        public Logger GetLogger(string name, Type loggerType)
        {
            return GetLogger(new LoggerCacheKey(loggerType, name));
        }

        public void ReconfigExistingLoggers()
        {
            ReconfigExistingLoggers(_config);
        }

        public void Flush()
        {
            Flush(_defaultFlushTimeout);
        }

        public void Flush(TimeSpan timeout)
        {
            try
            {
                AsyncHelpers.RunSynchronously(cb => Flush(cb, timeout));
            }
            catch (Exception e)
            {
                if (ThrowExceptions)
                {
                    throw;
                }

                InternalLogger.Error(e.ToString());
            }
        }

        public void Flush(int timeoutMilliseconds)
        {
            Flush(TimeSpan.FromMilliseconds(timeoutMilliseconds));
        }

        public void Flush(AsyncContinuation asyncContinuation)
        {
            Flush(asyncContinuation, TimeSpan.MaxValue);
        }

        public void Flush(AsyncContinuation asyncContinuation, int timeoutMilliseconds)
        {
            Flush(asyncContinuation, TimeSpan.FromMilliseconds(timeoutMilliseconds));
        }

        public void Flush(AsyncContinuation asyncContinuation, TimeSpan timeout)
        {
            try
            {
                InternalLogger.Trace("LogFactory.Flush({0})", timeout);

                var loggingConfiguration = Configuration;
                if (loggingConfiguration != null)
                {
                    InternalLogger.Trace("Flushing all targets...");
                    loggingConfiguration.FlushAllTargets(AsyncHelpers.WithTimeout(asyncContinuation, timeout));
                }
                else
                {
                    asyncContinuation(null);
                }
            }
            catch (Exception e)
            {
                if (ThrowExceptions)
                {
                    throw;
                }

                InternalLogger.Error(e.ToString());
            }
        }

        public IDisposable DisableLogging()
        {
            lock (this)
            {
                _logsEnabled--;
                if (_logsEnabled == -1)
                {
                    ReconfigExistingLoggers();
                }
            }

            return new LogEnabler(this);
        }

        public void EnableLogging()
        {
            lock (this)
            {
                _logsEnabled++;
                if (_logsEnabled == 0)
                {
                    ReconfigExistingLoggers();
                }
            }
        }

        public bool IsLoggingEnabled()
        {
            return _logsEnabled >= 0;
        }

        internal void ReloadConfigOnTimer(object state)
        {
            LoggingConfiguration configurationToReload = (LoggingConfiguration)state;

            InternalLogger.Info("Reloading configuration...");
            lock (this)
            {
                if (_reloadTimer != null)
                {
                    _reloadTimer.Dispose();
                    _reloadTimer = null;
                }

                _watcher.StopWatching();
                try
                {
                    if (Configuration != configurationToReload)
                    {
                        throw new NLogConfigurationException("Config changed in between. Not reloading.");
                    }

                    LoggingConfiguration newConfig = configurationToReload.Reload();
                    if (newConfig != null)
                    {
                        Configuration = newConfig;
                        if (ConfigurationReloaded != null)
                        {
                            ConfigurationReloaded(this, new LoggingConfigurationReloadedEventArgs(true, null));
                        }
                    }
                    else
                    {
                        throw new NLogConfigurationException("Configuration.Reload() returned null. Not reloading.");
                    }
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    _watcher.Watch(configurationToReload.FileNamesToWatch);

                    var configurationReloadedDelegate = ConfigurationReloaded;
                    if (configurationReloadedDelegate != null)
                    {
                        configurationReloadedDelegate(this, new LoggingConfigurationReloadedEventArgs(false, exception));
                    }
                }
            }
        }

        internal void ReconfigExistingLoggers(LoggingConfiguration configuration)
        {
            if (configuration != null)
            {
                configuration.EnsureInitialized();
            }

            foreach (var loggerWrapper in _loggerCache.Values.ToList())
            {
                Logger logger = loggerWrapper.Target as Logger;
                if (logger != null)
                {
                    logger.SetConfiguration(GetConfigurationForLogger(logger.Name, configuration));
                }
            }
        }

        internal void GetTargetsByLevelForLogger(string name, IList<LoggingRule> rules, TargetWithFilterChain[] targetsByLevel, TargetWithFilterChain[] lastTargetsByLevel)
        {
            foreach (LoggingRule rule in rules)
            {
                if (!rule.NameMatches(name))
                {
                    continue;
                }

                for (int i = 0; i <= LogLevel.MaxLevel.Ordinal; ++i)
                {
                    if (i < GlobalThreshold.Ordinal || !rule.IsLoggingEnabledForLevel(LogLevel.FromOrdinal(i)))
                    {
                        continue;
                    }

                    foreach (Target target in rule.Targets)
                    {
                        var awf = new TargetWithFilterChain(target, rule.Filters);
                        if (lastTargetsByLevel[i] != null)
                        {
                            lastTargetsByLevel[i].NextInChain = awf;
                        }
                        else
                        {
                            targetsByLevel[i] = awf;
                        }

                        lastTargetsByLevel[i] = awf;
                    }
                }

                GetTargetsByLevelForLogger(name, rule.ChildRules, targetsByLevel, lastTargetsByLevel);

                if (rule.Final)
                {
                    break;
                }
            }

            for (int i = 0; i <= LogLevel.MaxLevel.Ordinal; ++i)
            {
                TargetWithFilterChain tfc = targetsByLevel[i];
                if (tfc != null)
                {
                    tfc.PrecalculateStackTraceUsage();
                }
            }
        }

        internal LoggerConfiguration GetConfigurationForLogger(string name, LoggingConfiguration configuration)
        {
            TargetWithFilterChain[] targetsByLevel = new TargetWithFilterChain[LogLevel.MaxLevel.Ordinal + 1];
            TargetWithFilterChain[] lastTargetsByLevel = new TargetWithFilterChain[LogLevel.MaxLevel.Ordinal + 1];

            if (configuration != null && IsLoggingEnabled())
            {
                GetTargetsByLevelForLogger(name, configuration.LoggingRules, targetsByLevel, lastTargetsByLevel);
            }

            InternalLogger.Debug("Targets for {0} by level:", name);
            for (int i = 0; i <= LogLevel.MaxLevel.Ordinal; ++i)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0} =>", LogLevel.FromOrdinal(i));
                for (TargetWithFilterChain afc = targetsByLevel[i]; afc != null; afc = afc.NextInChain)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, " {0}", afc.Target.Name);
                    if (afc.FilterChain.Count > 0)
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, " ({0} filters)", afc.FilterChain.Count);
                    }
                }

                InternalLogger.Debug(sb.ToString());
            }

            return new LoggerConfiguration(targetsByLevel);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _watcher.Dispose();

                if (_reloadTimer != null)
                {
                    _reloadTimer.Dispose();
                    _reloadTimer = null;
                }
            }
        }

        private static IEnumerable<string> GetCandidateFileNames()
        {
            if (CurrentAppDomain.BaseDirectory != null)
            {
                yield return Path.Combine(CurrentAppDomain.BaseDirectory, "NLog.config");
            }

            string cf = CurrentAppDomain.ConfigurationFile;
            if (cf != null)
            {
                yield return Path.ChangeExtension(cf, ".nlog");

                const string VSHOST_SUB_STR = ".vshost.";
                if (cf.Contains(VSHOST_SUB_STR))
                {
                    yield return Path.ChangeExtension(cf.Replace(VSHOST_SUB_STR, "."), ".nlog");
                }

                IEnumerable<string> privateBinPaths = CurrentAppDomain.PrivateBinPath;
                if (privateBinPaths != null)
                {
                    foreach (var path in privateBinPaths)
                    {
                        if (path != null)
                        {
                            yield return Path.Combine(path, "NLog.config");
                        }
                    }
                }
            }

            var nlogAssembly = typeof(LogFactory).Assembly;
            if (!nlogAssembly.GlobalAssemblyCache)
            {
                if (!string.IsNullOrEmpty(nlogAssembly.Location))
                {
                    yield return nlogAssembly.Location + ".nlog";
                }
            }
        }

        private static void Dump(LoggingConfiguration config)
        {
            if (!InternalLogger.IsDebugEnabled)
            {
                return;
            }

            config.Dump();
        }

        private Logger GetLogger(LoggerCacheKey cacheKey)
        {
            lock (this)
            {
                WeakReference l;

                if (_loggerCache.TryGetValue(cacheKey, out l))
                {
                    Logger existingLogger = l.Target as Logger;
                    if (existingLogger != null)
                    {
                        return existingLogger;
                    }
                }

                Logger newLogger;

                if (cacheKey.ConcreteType != null && cacheKey.ConcreteType != typeof(Logger))
                {
                    try
                    {
                        newLogger = (Logger)FactoryHelper.CreateInstance(cacheKey.ConcreteType);
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        if (ThrowExceptions)
                        {
                            throw;
                        }

                        InternalLogger.Error("Cannot create instance of specified type. Proceeding with default type instance. Exception : {0}", exception);

                        cacheKey = new LoggerCacheKey(typeof(Logger), cacheKey.Name);

                        newLogger = new Logger();
                    }
                }
                else
                {
                    newLogger = new Logger();
                }

                if (cacheKey.ConcreteType != null)
                {
                    newLogger.Initialize(cacheKey.Name, GetConfigurationForLogger(cacheKey.Name, Configuration), this);
                }

                _loggerCache[cacheKey] = new WeakReference(newLogger);
                return newLogger;
            }
        }

        private void ConfigFileChanged(object sender, EventArgs args)
        {
            InternalLogger.Info("Configuration file change detected! Reloading in {0}ms...", RECONFIG_AFTER_FILE_CHANGED_TIMEOUT);

            lock (this)
            {
                if (_reloadTimer == null)
                {
                    _reloadTimer = new Timer(
                        ReloadConfigOnTimer,
                        Configuration,
                        RECONFIG_AFTER_FILE_CHANGED_TIMEOUT,
                        Timeout.Infinite);
                }
                else
                {
                    _reloadTimer.Change(RECONFIG_AFTER_FILE_CHANGED_TIMEOUT, Timeout.Infinite);
                }
            }
        }

        #endregion

        #region Вложенный класс: LogEnabler

        private class LogEnabler : IDisposable
        {
            #region Поля

            private readonly LogFactory _factory;

            #endregion

            #region Конструктор

            public LogEnabler(LogFactory factory)
            {
                _factory = factory;
            }

            #endregion

            #region Методы

            void IDisposable.Dispose()
            {
                _factory.EnableLogging();
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: LoggerCacheKey

        internal class LoggerCacheKey
        {
            #region Свойства

            internal Type ConcreteType { get; private set; }

            internal string Name { get; private set; }

            #endregion

            #region Конструктор

            internal LoggerCacheKey(Type loggerConcreteType, string name)
            {
                ConcreteType = loggerConcreteType;
                Name = name;
            }

            #endregion

            #region Методы

            public override int GetHashCode()
            {
                return ConcreteType.GetHashCode() ^ Name.GetHashCode();
            }

            public override bool Equals(object o)
            {
                var key = o as LoggerCacheKey;
                if (ReferenceEquals(key, null))
                {
                    return false;
                }

                return (ConcreteType == key.ConcreteType) && (key.Name == Name);
            }

            #endregion
        }

        #endregion
    }
}