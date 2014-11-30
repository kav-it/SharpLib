namespace SharpLib.Log
{
    [LogConfigurationItem]
    public abstract class Filter
    {
        #region Свойства

        [RequiredParameter]
        public FilterResult Action { get; set; }

        #endregion

        #region Конструктор

        protected Filter()
        {
            Action = FilterResult.Neutral;
        }

        #endregion

        #region Методы

        internal FilterResult GetFilterResult(LogEventInfo logEvent)
        {
            return Check(logEvent);
        }

        protected abstract FilterResult Check(LogEventInfo logEvent);

        #endregion
    }
}