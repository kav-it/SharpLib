using NLog.Conditions;
using NLog.Config;

namespace NLog.Filters
{
    [Filter("when")]
    public class ConditionBasedFilter : Filter
    {
        #region ����

        private static readonly object boxedTrue = true;

        #endregion

        #region ��������

        [RequiredParameter]
        public ConditionExpression Condition { get; set; }

        #endregion

        #region ������

        protected override FilterResult Check(LogEventInfo logEvent)
        {
            object val = Condition.Evaluate(logEvent);
            if (boxedTrue.Equals(val))
            {
                return Action;
            }

            return FilterResult.Neutral;
        }

        #endregion
    }
}