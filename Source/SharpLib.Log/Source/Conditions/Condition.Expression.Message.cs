
using System;
using System.Globalization;

namespace SharpLib.Log
{
    internal sealed class ConditionMessageExpression : ConditionExpression
    {
        #region Методы

        public override string ToString()
        {
            return "message";
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            return context.FormattedMessage;
        }

        #endregion
    }
}
