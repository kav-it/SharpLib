using System;
using System.ComponentModel;

namespace SharpLib.Log
{
    [Filter("whenNotEqual")]
    public class WhenNotEqualFilter : LayoutBasedFilter
    {
        #region ��������

        [RequiredParameter]
        public string CompareTo { get; set; }

        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        #endregion

        #region ������

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