
using System;

namespace SharpLib.Log
{
    internal sealed class ConditionLevelExpression : ConditionExpression
    {
        #region Методы

        public override string ToString()
        {
            return "level";
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            return context.Level;
        }

        #endregion
    }
}
