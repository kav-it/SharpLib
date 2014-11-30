using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SharpLib.Log
{
    public class LogFactory : IDisposable
    {
        #region Константы

        private const int FLUSH_TIMEOUT = 15000;

        private const int RECONFIG_AFTER_FILE_CHANGED_TIMEOUT = 1000;

        private const string CONFIG_EXTENSION = ".log.config";

        #endregion

        #region Поля

        private static readonly TimeSpan _defaultFlushTimeout;

        /// <summary>
        /// Текущий домен
        /// </summary>
        private static IAppDomain _currentAppDomain;

        /// <summary>
        /// Кеш объектов-логгеров
        /// </summary>
        private readonly Dictionary<LoggerCacheKey, WeakReference> _loggerCache;

        /// <summary>
        /// Модуль анализа изменения файлов конфигурации в файловой системе
        /// </summary>
        private readonly MultiFileWatcher _watcher;

        /// <summary>
        /// Конфигурация
        /// </summary>
        private LoggingConfiguration _config;

        private LogLevel _globalThreshold;

        private int _logsEnabled;

        /// <summary>
        /// Таймер перезагрузки конфигурации
        /// </summary>
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
                    if (_config != null)
                    {
                        return _config;
                    }

                    _config = GetConfiguration();

                    return _config;
                }
            }
            set
            {
                SetConfiguration(value);
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

        static LogFactory()
        {
            _defaultFlushTimeout = TimeSpan.FromMilliseconds(FLUSH_TIMEOUT);
        }

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
            catch
            {
                if (ThrowExceptions)
                {
                    throw;
                }
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
                var loggingConfiguration = Configuration;
                if (loggingConfiguration != null)
                {
                    loggingConfiguration.FlushAllTargets(AsyncHelpers.WithTimeout(asyncContinuation, timeout));
                }
                else
                {
                    asyncContinuation(null);
                }
            }
            catch
            {
                if (ThrowExceptions)
                {
                    throw;
                }
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
                        throw new Exception("Config changed in between. Not reloading.");
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
                        throw new Exception("Configuration.Reload() returned null. Not reloading.");
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
            var targetsByLevel = new TargetWithFilterChain[LogLevel.MaxLevel.Ordinal + 1];
            var lastTargetsByLevel = new TargetWithFilterChain[LogLevel.MaxLevel.Ordinal + 1];

            if (configuration != null && IsLoggingEnabled())
            {
                GetTargetsByLevelForLogger(name, configuration.LoggingRules, targetsByLevel, lastTargetsByLevel);
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

        private static string GetConfigFilename()
        {
            const string VSHOST_SUB_STR = ".vshost.";

            var rootDir = CurrentAppDomain.BaseDirectory;
            var exeName = Path.Combine(rootDir, CurrentAppDomain.FriendlyName);
            var configName = Path.ChangeExtension(exeName, CONFIG_EXTENSION);

            if (configName.Contains(VSHOST_SUB_STR))
            {
                configName = configName.Replace(VSHOST_SUB_STR, ".");
            }

            return configName;
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
                    var configuration = GetConfigurationForLogger(cacheKey.Name, Configuration);
                    newLogger.Initialize(cacheKey.Name, configuration, this);
                }

                _loggerCache[cacheKey] = new WeakReference(newLogger);
                return newLogger;
            }
        }

        private void ConfigFileChanged(object sender, EventArgs args)
        {
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

        private LoggingConfiguration GetConfiguration()
        {
            LoggingConfiguration config = null;

            var configFile = GetConfigFilename();

            if (!File.Exists(configFile))
            {
                var context = XmlLoggingConfiguration.GetDefaultConfigAsText();
                File.WriteAllText(configFile, context);
            }

            config = new XmlLoggingConfiguration(configFile);

            if (config == null)
            {
                throw new Exception("Не найдено файла конфигурации для логгера");
            }

            try
            {
                _watcher.Watch(_config.FileNamesToWatch);
            }
            catch (Exception)
            {
            }

            config.InitializeAll();

            return config;
        }

        private void SetConfiguration(LoggingConfiguration newValue)
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
            }

            lock (this)
            {
                LoggingConfiguration oldConfig = _config;
                if (oldConfig != null)
                {
                    Flush();
                    oldConfig.Close();
                }

                _config = newValue;

                if (_config != null)
                {
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
                    }
                }

                var configurationChangedDelegate = ConfigurationChanged;

                if (configurationChangedDelegate != null)
                {
                    configurationChangedDelegate(this, new LoggingConfigurationChangedEventArgs(oldConfig, newValue));
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
