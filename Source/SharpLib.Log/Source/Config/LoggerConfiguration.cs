namespace SharpLib.Log
{
    internal class LoggerConfiguration
    {
        #region Поля

        private readonly TargetWithFilterChain[] _targetsByLevel;

        #endregion

        #region Конструктор

        public LoggerConfiguration(TargetWithFilterChain[] targetsByLevel)
        {
            _targetsByLevel = targetsByLevel;
        }

        #endregion

        #region Методы

        public TargetWithFilterChain GetTargetsForLevel(LogLevel level)
        {
            return _targetsByLevel[level.Ordinal];
        }

        public bool IsEnabled(LogLevel level)
        {
            return _targetsByLevel[level.Ordinal] != null;
        }

        #endregion
    }
}