using NLog.Layouts;

namespace NLog.Conditions
{
    internal sealed class ConditionLayoutExpression : ConditionExpression
    {
        #region ��������

        public Layout Layout { get; private set; }

        #endregion

        #region �����������

        public ConditionLayoutExpression(Layout layout)
        {
            Layout = layout;
        }

        #endregion

        #region ������

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