using System;
using System.ComponentModel;

using NLog.Config;

namespace NLog.Filters
{
    [Filter("whenEqual")]
    public class WhenEqualFilter : LayoutBasedFilter
    {
        #region ��������

        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        [RequiredParameter]
        public string CompareTo { get; set; }

        #endregion

        #region ������

        protected override FilterResult Check(LogEventInfo logEvent)
        {
            StringComparison comparisonType = IgnoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            if (Layout.Render(logEvent).Equals(CompareTo, comparisonType))
            {
                return Action;
            }

            return FilterResult.Neutral;
        }

        #endregion
    }
}