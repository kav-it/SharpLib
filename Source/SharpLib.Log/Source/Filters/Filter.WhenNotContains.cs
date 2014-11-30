using System;
using System.ComponentModel;

namespace SharpLib.Log
{
    [Filter("whenNotContains")]
    public class WhenNotContainsFilter : LayoutBasedFilter
    {
        #region Ñגמיסעגא

        [RequiredParameter]
        public string Substring { get; set; }

        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        #endregion

        #region Ìועמהû

        protected override FilterResult Check(LogEventInfo logEvent)
        {
            StringComparison comparison = IgnoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            string result = Layout.Render(logEvent);

            if (result.IndexOf(Substring, comparison) < 0)
            {
                return Action;
            }

            return FilterResult.Neutral;
        }

        #endregion
    }
}