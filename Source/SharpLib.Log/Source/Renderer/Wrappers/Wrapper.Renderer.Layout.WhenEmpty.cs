namespace SharpLib.Log
{
    [LayoutRenderer("whenEmpty")]
    [AmbientProperty("WhenEmpty")]
    [ThreadAgnostic]
    public sealed class WhenEmptyLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region ��������

        [RequiredParameter]
        public Layout WhenEmpty { get; set; }

        #endregion

        #region ������

        protected override string Transform(string text)
        {
            return text;
        }

        protected override string RenderInner(LogEventInfo logEvent)
        {
            string inner = base.RenderInner(logEvent);
            if (!string.IsNullOrEmpty(inner))
            {
                return inner;
            }

            return WhenEmpty.Render(logEvent);
        }

        #endregion
    }
}