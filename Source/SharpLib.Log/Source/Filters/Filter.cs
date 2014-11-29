using NLog.Config;

namespace NLog.Filters
{
    [NLogConfigurationItem]
    public abstract class Filter
    {
        #region ��������

        [RequiredParameter]
        public FilterResult Action { get; set; }

        #endregion

        #region �����������

        protected Filter()
        {
            Action = FilterResult.Neutral;
        }

        #endregion

        #region ������

        internal FilterResult GetFilterResult(LogEventInfo logEvent)
        {
            return Check(logEvent);
        }

        protected abstract FilterResult Check(LogEventInfo logEvent);

        #endregion
    }
}