namespace SharpLib.Log
{
    [LayoutRenderer("when")]
    [AmbientProperty("When")]
    [ThreadAgnostic]
    public sealed class WhenLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Ñגמיסעגא

        [RequiredParameter]
        public ConditionExpression When { get; set; }

        #endregion

        #region Ìועמהû

        protected override string Transform(string text)
        {
            return text;
        }

        protected override string RenderInner(LogEventInfo logEvent)
        {
            if (true.Equals(When.Evaluate(logEvent)))
            {
                return base.RenderInner(logEvent);
            }

            return string.Empty;
        }

        #endregion
    }
}