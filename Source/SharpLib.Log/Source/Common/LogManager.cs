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
        private static readonly LogFactory globalFactory = new LogFactory();

        private static IAppDomain _currentAppDomain;

        private static GetCultureInfo _defaultCultureInfo = () => CultureInfo.CurrentCulture;

        public delegate CultureInfo GetCultureInfo();

#if !SILVERLIGHT && !MONO

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Significant logic in .cctor()")]
        static LogManager()
        {
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
#endif

        private LogManager()
        {
        }

        public static event EventHandler<LoggingConfigurationChangedEventArgs> ConfigurationChanged
        {
            add { globalFactory.ConfigurationChanged += value; }
            remove { globalFactory.ConfigurationChanged -= value; }
        }

#if !SILVERLIGHT

        public static event EventHandler<LoggingConfigurationReloadedEventArgs> ConfigurationReloaded
        {
            add { globalFactory.ConfigurationReloaded += value; }
            remove { globalFactory.ConfigurationReloaded -= value; }
        }
#endif

        public static bool ThrowExceptions
        {
            get { return globalFactory.ThrowExceptions; }
            set { globalFactory.ThrowExceptions = value; }
        }

        internal static IAppDomain CurrentAppDomain
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

        public static LoggingConfiguration Configuration
        {
            get { return globalFactory.Configuration; }
            set { globalFactory.Configuration = value; }
        }

        public static LogLevel GlobalThreshold
        {
            get { return globalFactory.GlobalThreshold; }
            set { globalFactory.GlobalThreshold = value; }
        }

        public static GetCultureInfo DefaultCultureInfo
        {
            get { return _defaultCultureInfo; }
            set { _defaultCultureInfo = value; }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Logger GetCurrentClassLogger()
        {
            string loggerName;
            Type declaringType;
            int framesToSkip = 1;
            do
            {
#if SILVERLIGHT
                StackFrame frame = new StackTrace().GetFrame(framesToSkip);
#else
                StackFrame frame = new StackFrame(framesToSkip, false);
#endif
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

            return globalFactory.GetLogger(loggerName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Logger GetCurrentClassLogger(Type loggerType)
        {
            Type declaringType;
            int framesToSkip = 1;
            do
            {
#if SILVERLIGHT
                StackFrame frame = new StackTrace().GetFrame(framesToSkip);
#else
                StackFrame frame = new StackFrame(framesToSkip, false);
#endif
                declaringType = frame.GetMethod().DeclaringType;
                framesToSkip++;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return globalFactory.GetLogger(declaringType.FullName, loggerType);
        }

        public static Logger CreateNullLogger()
        {
            return globalFactory.CreateNullLogger();
        }

        public static Logger GetLogger(string name)
        {
            return globalFactory.GetLogger(name);
        }

        public static Logger GetLogger(string name, Type loggerType)
        {
            return globalFactory.GetLogger(name, loggerType);
        }

        public static void ReconfigExistingLoggers()
        {
            globalFactory.ReconfigExistingLoggers();
        }

#if !SILVERLIGHT

        public static void Flush()
        {
            globalFactory.Flush();
        }

        public static void Flush(TimeSpan timeout)
        {
            globalFactory.Flush(timeout);
        }

        public static void Flush(int timeoutMilliseconds)
        {
            globalFactory.Flush(timeoutMilliseconds);
        }
#endif

        public static void Flush(AsyncContinuation asyncContinuation)
        {
            globalFactory.Flush(asyncContinuation);
        }

        public static void Flush(AsyncContinuation asyncContinuation, TimeSpan timeout)
        {
            globalFactory.Flush(asyncContinuation, timeout);
        }

        public static void Flush(AsyncContinuation asyncContinuation, int timeoutMilliseconds)
        {
            globalFactory.Flush(asyncContinuation, timeoutMilliseconds);
        }

        public static IDisposable DisableLogging()
        {
            return globalFactory.DisableLogging();
        }

        public static void EnableLogging()
        {
            globalFactory.EnableLogging();
        }

        public static bool IsLoggingEnabled()
        {
            return globalFactory.IsLoggingEnabled();
        }

        public static void Shutdown()
        {
            foreach (var target in Configuration.AllTargets)
            {
                target.Dispose();
            }
        }

#if !SILVERLIGHT && !MONO
        private static void SetupTerminationEvents()
        {
            CurrentAppDomain.ProcessExit += TurnOffLogging;
            CurrentAppDomain.DomainUnload += TurnOffLogging;
        }

        private static void TurnOffLogging(object sender, EventArgs args)
        {
            InternalLogger.Info("Shutting down logging...");
            Configuration = null;
            InternalLogger.Info("Logger has been shut down.");
        }
#endif
    }
}