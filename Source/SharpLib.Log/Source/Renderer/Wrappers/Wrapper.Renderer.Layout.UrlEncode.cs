namespace SharpLib.Log
{
    [LayoutRenderer("url-encode")]
    [ThreadAgnostic]
    public sealed class UrlEncodeLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region ��������

        public bool SpaceAsPlus { get; set; }

        #endregion

        #region �����������

        public UrlEncodeLayoutRendererWrapper()
        {
            SpaceAsPlus = true;
        }

        #endregion

        #region ������

        protected override string Transform(string text)
        {
            return UrlHelper.UrlEncode(text, SpaceAsPlus);
        }

        #endregion
    }
}