using System;
using System.ComponentModel;

namespace SharpLib.Log
{
    [Filter("whenEqual")]
    public class WhenEqualFilter : LayoutBasedFilter
    {
        #region Ñגמיסעגא

        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        [RequiredParameter]
        public string CompareTo { get; set; }

        #endregion

        #region Ìועמהû

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