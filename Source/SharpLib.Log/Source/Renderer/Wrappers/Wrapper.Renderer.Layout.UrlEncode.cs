namespace SharpLib.Log
{
    [LayoutRenderer("url-encode")]
    [ThreadAgnostic]
    public sealed class UrlEncodeLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Свойства

        public bool SpaceAsPlus { get; set; }

        #endregion

        #region Конструктор

        public UrlEncodeLayoutRendererWrapper()
        {
            SpaceAsPlus = true;
        }

        #endregion

        #region Методы

        protected override string Transform(string text)
        {
            return UrlHelper.UrlEncode(text, SpaceAsPlus);
        }

        #endregion
    }
}