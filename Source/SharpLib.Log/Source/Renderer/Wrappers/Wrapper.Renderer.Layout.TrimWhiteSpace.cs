using System.ComponentModel;

namespace SharpLib.Log
{
    [LayoutRenderer("trim-whitespace")]
    [AmbientProperty("TrimWhiteSpace")]
    [ThreadAgnostic]
    public sealed class TrimWhiteSpaceLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region ��������

        [DefaultValue(true)]
        public bool TrimWhiteSpace { get; set; }

        #endregion

        #region �����������

        public TrimWhiteSpaceLayoutRendererWrapper()
        {
            TrimWhiteSpace = true;
        }

        #endregion

        #region ������

        protected override string Transform(string text)
        {
            return TrimWhiteSpace ? text.Trim() : text;
        }

        #endregion
    }
}