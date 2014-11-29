using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Internal.Fakeables;

namespace NLog
{
    public sealed class LogManager
    {
        #region Делегаты

        public delegate CultureInfo GetCultureInfo();

        #endregion

        #region Поля

        private readonly LogFactory _globalFactory;

        private static readonly Lazy<LogManager> _instance = new Lazy<LogManager>(() => new LogManager());

        private IAppDomain _currentAppDomain;

        #endregion

        #region Свойства

        public static LogManager Instance
        {
            get { return _instance.Value; }
        }

        public bool ThrowExceptions
        {
            get { return _globalFactory.ThrowExceptions; }
            set { _globalFactory.ThrowExceptions = value; }
        }

        internal IAppDomain CurrentAppDomain
        {
            get { return _currentAppDomain ?? (_currentAppDomain = AppDomainWrapper.CurrentDomain); }
            set
            {
#if !SILVERLIGHT
                _currentAppDomain.DomainUnload -= TurnOffLogging;
                _currentAppDomain.ProcessExit -= TurnOffLogging;
#endif
                _currentAppDomain = value;
            }
        }

        public LoggingConfiguration Configuration
        {
            get { return _globalFactory.Configuration; }
            set { _globalFactory.Configuration = value; }
        }

        public LogLevel GlobalThreshold
        {
            get { return _globalFactory.GlobalThreshold; }
            set { _globalFactory.GlobalThreshold = value; }
        }

        public GetCultureInfo DefaultCultureInfo { get; set; }

        #endregion

        #region События

        public event EventHandler<LoggingConfigurationChangedEventArgs> ConfigurationChanged
        {
            add { _globalFactory.ConfigurationChanged += value; }
            remove { _globalFactory.ConfigurationChanged -= value; }
        }

        public event EventHandler<LoggingConfigurationReloadedEventArgs> ConfigurationReloaded
        {
            add { _globalFactory.ConfigurationReloaded += value; }
            remove { _globalFactory.ConfigurationReloaded -= value; }
        }

        #endregion

        #region Конструктор

        private LogManager()
        {
            DefaultCultureInfo = () => CultureInfo.CurrentCulture;
            _globalFactory = new LogFactory();
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

                InternalLogger.Warn("Error setting up termiation events: {0}", exception);
            }
        }

        #endregion

        #region Методы

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Logger GetCurrentClassLogger()
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

            return _globalFactory.GetLogger(loggerName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Logger GetCurrentClassLogger(Type loggerType)
        {
            Type declaringType;
            int framesToSkip = 1;
            do
            {
                StackFrame frame = new StackFrame(framesToSkip, false);
                declaringType = frame.GetMethod().DeclaringType;
                framesToSkip++;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return _globalFactory.GetLogger(declaringType.FullName, loggerType);
        }

        public Logger CreateNullLogger()
        {
            return _globalFactory.CreateNullLogger();
        }

        public Logger GetLogger(string name)
        {
            return _globalFactory.GetLogger(name);
        }

        public Logger GetLogger(string name, Type loggerType)
        {
            return _globalFactory.GetLogger(name, loggerType);
        }

        public void ReconfigExistingLoggers()
        {
            _globalFactory.ReconfigExistingLoggers();
        }

        public void Flush()
        {
            _globalFactory.Flush();
        }

        public void Flush(TimeSpan timeout)
        {
            _globalFactory.Flush(timeout);
        }

        public void Flush(int timeoutMilliseconds)
        {
            _globalFactory.Flush(timeoutMilliseconds);
        }

        public void Flush(AsyncContinuation asyncContinuation)
        {
            _globalFactory.Flush(asyncContinuation);
        }

        public void Flush(AsyncContinuation asyncContinuation, TimeSpan timeout)
        {
            _globalFactory.Flush(asyncContinuation, timeout);
        }

        public void Flush(AsyncContinuation asyncContinuation, int timeoutMilliseconds)
        {
            _globalFactory.Flush(asyncContinuation, timeoutMilliseconds);
        }

        public IDisposable DisableLogging()
        {
            return _globalFactory.DisableLogging();
        }

        public void EnableLogging()
        {
            _globalFactory.EnableLogging();
        }

        public bool IsLoggingEnabled()
        {
            return _globalFactory.IsLoggingEnabled();
        }

        public void Shutdown()
        {
            foreach (var target in Configuration.AllTargets)
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
            InternalLogger.Info("Shutting down logging...");
            Configuration = null;
            InternalLogger.Info("Logger has been shut down.");
        }

        #endregion
    }
}