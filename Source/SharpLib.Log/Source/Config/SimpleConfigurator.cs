using NLog.Targets;

namespace NLog.Config
{
    public static class SimpleConfigurator
    {
        #region Ìועמהû

        public static void ConfigureForConsoleLogging()
        {
            ConfigureForConsoleLogging(LogLevel.Info);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Target is disposed elsewhere.")]
        public static void ConfigureForConsoleLogging(LogLevel minLevel)
        {
            ConsoleTarget consoleTarget = new ConsoleTarget();

            LoggingConfiguration config = new LoggingConfiguration();
            LoggingRule rule = new LoggingRule("*", minLevel, consoleTarget);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
        }

        public static void ConfigureForTargetLogging(Target target)
        {
            ConfigureForTargetLogging(target, LogLevel.Info);
        }

        public static void ConfigureForTargetLogging(Target target, LogLevel minLevel)
        {
            LoggingConfiguration config = new LoggingConfiguration();
            LoggingRule rule = new LoggingRule("*", minLevel, target);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
        }

        public static void ConfigureForFileLogging(string fileName)
        {
            ConfigureForFileLogging(fileName, LogLevel.Info);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Target is disposed elsewhere.")]
        public static void ConfigureForFileLogging(string fileName, LogLevel minLevel)
        {
            FileTarget target = new FileTarget();
            target.FileName = fileName;
            ConfigureForTargetLogging(target, minLevel);
        }

        #endregion
    }
}