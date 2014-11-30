using System.ComponentModel;

namespace SharpLib.Log
{
    [LayoutRenderer("cached")]
    [AmbientProperty("Cached")]
    [ThreadAgnostic]
    public sealed class CachedLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region ����

        private string _cachedValue;

        #endregion

        #region ��������

        [DefaultValue(true)]
        public bool Cached { get; set; }

        #endregion

        #region �����������

        public CachedLayoutRendererWrapper()
        {
            Cached = true;
        }

        #endregion

        #region ������

        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();
            _cachedValue = null;
        }

        protected override void CloseLayoutRenderer()
        {
            base.CloseLayoutRenderer();
            _cachedValue = null;
        }

        protected override string Transform(string text)
        {
            return text;
        }

        protected override string RenderInner(LogEventInfo logEvent)
        {
            if (Cached)
            {
                return _cachedValue ?? (_cachedValue = base.RenderInner(logEvent));
            }
            return base.RenderInner(logEvent);
        }

        #endregion
    }
}