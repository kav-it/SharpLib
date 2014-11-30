using System;
using System.ComponentModel;

namespace SharpLib.Log
{
    [Filter("whenContains")]
    public class WhenContainsFilter : LayoutBasedFilter
    {
        #region Ñגמיסעגא

        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        [RequiredParameter]
        public string Substring { get; set; }

        #endregion

        #region Ìועמהû

        protected override FilterResult Check(LogEventInfo logEvent)
        {
            StringComparison comparisonType = IgnoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            if (Layout.Render(logEvent).IndexOf(Substring, comparisonType) >= 0)
            {
                return Action;
            }

            return FilterResult.Neutral;
        }

        #endregion
    }
}