using System.ComponentModel;

namespace SharpLib.Log
{
    [LogConfigurationItem]
    public class ConsoleRowHighlightingRule
    {
        #region ��������

        public static ConsoleRowHighlightingRule Default { get; private set; }

        [RequiredParameter]
        public ConditionExpression Condition { get; set; }

        [DefaultValue("NoChange")]
        public ConsoleOutputColor ForegroundColor { get; set; }

        [DefaultValue("NoChange")]
        public ConsoleOutputColor BackgroundColor { get; set; }

        #endregion

        #region �����������

        static ConsoleRowHighlightingRule()
        {
            Default = new ConsoleRowHighlightingRule(null, ConsoleOutputColor.NoChange, ConsoleOutputColor.NoChange);
        }

        public ConsoleRowHighlightingRule()
            : this(null, ConsoleOutputColor.NoChange, ConsoleOutputColor.NoChange)
        {
        }

        public ConsoleRowHighlightingRule(ConditionExpression condition, ConsoleOutputColor foregroundColor, ConsoleOutputColor backgroundColor)
        {
            Condition = condition;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }

        #endregion

        #region ������

        public bool CheckCondition(LogEventInfo logEvent)
        {
            if (Condition == null)
            {
                return true;
            }

            return true.Equals(Condition.Evaluate(logEvent));
        }

        #endregion
    }
}