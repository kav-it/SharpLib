using System;
using System.ComponentModel;

using NLog.Config;

namespace NLog.Filters
{
    [Filter("whenNotEqual")]
    public class WhenNotEqualFilter : LayoutBasedFilter
    {
        #region Ñגמיסעגא

        [RequiredParameter]
        public string CompareTo { get; set; }

        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        #endregion

        #region Ìועמהû

        protected override FilterResult Check(LogEventInfo logEvent)
        {
            StringComparison comparisonType = IgnoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            if (!Layout.Render(logEvent).Equals(CompareTo, comparisonType))
            {
                return Action;
            }

            return FilterResult.Neutral;
        }

        #endregion
    }
}