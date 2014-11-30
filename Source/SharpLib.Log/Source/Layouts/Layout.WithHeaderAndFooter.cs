namespace SharpLib.Log
{
    [Layout("LayoutWithHeaderAndFooter")]
    [ThreadAgnostic]
    public class LayoutWithHeaderAndFooter : Layout
    {
        #region Ñגמיסעגא

        public Layout Layout { get; set; }

        public Layout Header { get; set; }

        public Layout Footer { get; set; }

        #endregion

        #region Ìועמהû

        protected override string GetFormattedMessage(LogEventInfo logEvent)
        {
            return Layout.Render(logEvent);
        }

        #endregion
    }
}