using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SharpLib.Log
{
    /// <summary>
    /// Менеджер логов (основной класс создания логгеров)
    /// </summary>
    public class LogManager : IDisposable
    {
        #region Константы

        /// <summary>
        /// Расширение файлов конфигурации
        /// </summary>
        private const string CONFIG_EXTENSION = ".log.xml";

        /// <summary>
        /// Время паузы перед сохранением событий (мс)
        /// </summary>
        private const int FLUSH_TIMEOUT = 15000;

        /// <summary>
        /// Время перед загрузкой конфигурации, после изменения пользователем (мс)
        /// </summary>
        private const int RECONFIG_AFTER_FILE_CHANGED_TIMEOUT = 1000;

        #endregion

        #region Поля

        private static readonly Lazy<LogManager> _instance = new Lazy<LogManager>(() => new LogManager());

        /// <summary>
        /// Объект интервала времени <see cref="FLUSH_TIMEOUT" />
        /// </summary>
        private readonly TimeSpan _defaultFlushTimeout;

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

        /// <summary>
        /// Текущий домен
        /// </summary>
        private IAppDomain _currentAppDomain;

        /// <summary>
        /// Общий уровень отладки
        /// </summary>
        private LogLevel _globalThreshold;

        /// <summary>
        /// ХЗ
        /// </summary>
        private int _logsEnabled;

        /// <summary>
        /// Таймер перезагрузки конфигурации
        /// </summary>
        private Timer _reloadTimer;

        #endregion

        #region Свойства

        /// <summary>
        /// Текущая конфигурация
        /// </summary>
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

                    _config = GetConfigurationFromFile();

                    return _config;
                }
            }
            set { SetConfiguration(value); }
        }

        /// <summary>
        /// Глобальный уровень отладки
        /// </summary>
        public LogLevel GlobalThreshold
        {
            get { return _globalThreshold; }

            set
            {
                lock (this)
                {
                    _globalThreshold = value;
                    Reconfig();
                }
            }
        }

        /// <summary>
        /// Экземпляр класса
        /// </summary>
        public static LogManager Instance
        {
            get { return _instance.Value; }
        }

        internal IAppDomain CurrentAppDomain
        {
            get { return _currentAppDomain ?? (_currentAppDomain = AppDomainWrapper.CurrentDomain); }
            set
            {
                _currentAppDomain.DomainUnload -= TurnOffLogging;
                _currentAppDomain.ProcessExit -= TurnOffLogging;
                _currentAppDomain = value;
            }
        }

        #endregion

        #region События


        #endregion

        #region Конструктор

        private LogManager()
        {
            _defaultFlushTimeout = TimeSpan.FromMilliseconds(FLUSH_TIMEOUT);
            _globalThreshold = LogLevel.MinLevel;
            _loggerCache = new Dictionary<LoggerCacheKey, WeakReference>();
            _watcher = new MultiFileWatcher();
            _watcher.OnChange += ConfigFileChanged;

            try
            {
                SetupTerminationEvents();
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }
            }
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ILogger GetLogger(Type loggerType)
        {
            Type declaringType;
            int framesToSkip = 1;
            do
            {
                StackFrame frame = new StackFrame(framesToSkip, false);
                declaringType = frame.GetMethod().DeclaringType;
                framesToSkip++;
            } while (declaringType != null && declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return declaringType != null
                ? GetLoggerByTypeAndName(declaringType.FullName, loggerType)
                : null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ILogger GetLogger()
        {
            string loggerName;
            Type declaringType;
            int framesToSkip = 1;
            do
            {
                StackFrame frame = new StackFrame(framesToSkip, false);
                var method = frame.GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    loggerName = method.Name;
                    break;
                }

                framesToSkip++;
                loggerName = declaringType.FullName;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return GetLoggerByName(loggerName);
        }

        public ILogger GetLogger(string name)
        {
            return GetLoggerByName(name);
        }

        public ILogger GetLogger(string name, Type loggerType)
        {
            return GetLoggerByTypeAndName(name, loggerType);
        }

        private ILogger GetLoggerByName(string name)
        {
            return GetLogger(new LoggerCacheKey(typeof(Logger), name));
        }

        private ILogger GetLoggerByTypeAndName(string name, Type loggerType)
        {
            return GetLogger(new LoggerCacheKey(loggerType, name));
        }

        public void LoadConfigFromResource(Assembly assembly, string pathInResources)
        {
            pathInResources = string.Format("{0}.{1}", assembly.GetName().Name, pathInResources);

            using (var stream = assembly.GetManifestResourceStream(pathInResources))
            {
                if (stream != null)
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var result = sr.ReadToEnd();

                        var config = new XmlLoggingConfiguration(null, result);

                        config.InitializeAll();

                        _config = config;
                    }
                }
            }
        }

        private LoggingConfiguration GetConfigurationFromFile()
        {
            var configFile = GetConfigFilename();

            if (!File.Exists(configFile))
            {
                var context = XmlLoggingConfiguration.GetDefaultConfigAsText();
                File.WriteAllText(configFile, context);
            }

            var config = new XmlLoggingConfiguration(configFile, null);

            if (config == null)
            {
                throw new Exception("Не найдено файла конфигурации для логгера");
            }

            try
            {
                _watcher.Watch(config.FileNamesToWatch);
            }
            catch (Exception)
            {
            }

            config.InitializeAll();

            return config;
        }

        /// <summary>
        /// Переконфигурирования логгеров
        /// </summary>
        /// <remarks>
        /// Используется после изменения логгеров в коде
        /// </remarks>
        public void Reconfig()
        {
            ReconfigExistingLoggers(_config);
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

        public void Flush()
        {
            Flush(_defaultFlushTimeout);
        }

        public void Flush(int timeoutMilliseconds)
        {
            Flush(TimeSpan.FromMilliseconds(timeoutMilliseconds));
        }

        public void Flush(TimeSpan timeout)
        {
            try
            {
                AsyncHelpers.RunSynchronously(cb => Flush(cb, timeout));
            }
            catch
            {
            }
        }

        private void Flush(AsyncContinuation asyncContinuation, TimeSpan timeout)
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
            }
        }

        public IDisposable DisableLogging()
        {
            lock (this)
            {
                _logsEnabled--;
                if (_logsEnabled == -1)
                {
                    Reconfig();
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
                    Reconfig();
                }
            }
        }

        public bool IsLoggingEnabled()
        {
            return _logsEnabled >= 0;
        }

        public void Shutdown()
        {
            foreach (var target in Configuration.Targets)
            {
                target.Dispose();
            }
        }

        private void SetupTerminationEvents()
        {
            CurrentAppDomain.ProcessExit += TurnOffLogging;
            CurrentAppDomain.DomainUnload += TurnOffLogging;
        }

        private void TurnOffLogging(object sender, EventArgs args)
        {
            Configuration = null;
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

        private void ReloadConfigOnTimer(object state)
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
                }
            }
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
                    newLogger.Initialize(cacheKey.Name, configuration);
                }

                _loggerCache[cacheKey] = new WeakReference(newLogger);
                return newLogger;
            }
        }

        internal LoggerConfiguration GetConfigurationForLogger(string name, LoggingConfiguration configuration)
        {
            var targetsByLevel = new TargetWithFilterChain[LogLevel.MaxLevel.Ordinal + 1];
            var lastTargetsByLevel = new TargetWithFilterChain[LogLevel.MaxLevel.Ordinal + 1];

            if (configuration != null && IsLoggingEnabled())
            {
                GetTargetsByLevelForLogger(name, configuration.Rules, targetsByLevel, lastTargetsByLevel);
            }

            return new LoggerConfiguration(targetsByLevel);
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
            }
        }

        private string GetConfigFilename()
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

        #endregion

        #region Вложенный класс: LogEnabler

        private class LogEnabler : IDisposable
        {
            #region Поля

            private readonly LogManager _manager;

            #endregion

            #region Конструктор

            public LogEnabler(LogManager manager)
            {
                _manager = manager;
            }

            #endregion

            #region Методы

            void IDisposable.Dispose()
            {
                _manager.EnableLogging();
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