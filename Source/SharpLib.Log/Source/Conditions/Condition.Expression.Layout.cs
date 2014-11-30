
using System;

namespace SharpLib.Log
{
    internal sealed class ConditionLayoutExpression : ConditionExpression
    {
        #region Свойства

        public Layout Layout { get; private set; }

        #endregion

        #region Конструктор

        public ConditionLayoutExpression(Layout layout)
        {
            Layout = layout;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return Layout.ToString();
        }

        protected override object EvaluateNode(LogEventInfo context)
        {
            return Layout.Render(context);
        }

        #endregion
    }
}
