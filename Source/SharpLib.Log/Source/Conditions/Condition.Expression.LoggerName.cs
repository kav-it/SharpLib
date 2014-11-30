
using System;

namespace SharpLib.Log
{
    internal sealed class ConditionLoggerNameExpression : ConditionExpression
    {
        #region Методы

        public override string ToString()
        {
            return "logger";
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            return context.LoggerName;
        }

        #endregion
    }
}
