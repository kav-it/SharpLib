using System.ComponentModel;

namespace SharpLib.Log
{
    [LayoutRenderer("trim-whitespace")]
    [AmbientProperty("TrimWhiteSpace")]
    [ThreadAgnostic]
    public sealed class TrimWhiteSpaceLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Свойства

        [DefaultValue(true)]
        public bool TrimWhiteSpace { get; set; }

        #endregion

        #region Конструктор

        public TrimWhiteSpaceLayoutRendererWrapper()
        {
            TrimWhiteSpace = true;
        }

        #endregion

        #region Методы

        protected override string Transform(string text)
        {
            return TrimWhiteSpace ? text.Trim() : text;
        }

        #endregion
    }
}