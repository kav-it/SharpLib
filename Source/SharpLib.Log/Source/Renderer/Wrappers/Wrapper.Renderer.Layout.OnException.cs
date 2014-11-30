namespace SharpLib.Log
{
    [LayoutRenderer("onexception")]
    [ThreadAgnostic]
    public sealed class OnExceptionLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Ìועמהû

        protected override string Transform(string text)
        {
            return text;
        }

        protected override string RenderInner(LogEventInfo logEvent)
        {
            if (logEvent.Exception != null)
            {
                return base.RenderInner(logEvent);
            }

            return string.Empty;
        }

        #endregion
    }
}