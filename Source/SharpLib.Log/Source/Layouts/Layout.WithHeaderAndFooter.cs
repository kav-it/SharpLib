namespace SharpLib.Log
{
    [Layout("LayoutWithHeaderAndFooter")]
    [ThreadAgnostic]
    public class LayoutWithHeaderAndFooter : Layout
    {
        #region ��������

        public Layout Layout { get; set; }

        public Layout Header { get; set; }

        public Layout Footer { get; set; }

        #endregion

        #region ������

        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            return Layout.Render(logEvent);
        }

        #endregion
    }
}