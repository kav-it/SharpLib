using System.ComponentModel;
using System.Globalization;

namespace SharpLib.Log
{
    [LayoutRenderer("lowercase")]
    [AmbientProperty("Lowercase")]
    [ThreadAgnostic]
    public sealed class LowercaseLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Свойства

        [DefaultValue(true)]
        public bool Lowercase { get; set; }

        public CultureInfo Culture { get; set; }

        #endregion

        #region Конструктор

        public LowercaseLayoutRendererWrapper()
        {
            Culture = CultureInfo.InvariantCulture;
            Lowercase = true;
        }

        #endregion

        #region Методы

        protected override string Transform(string text)
        {
            return Lowercase ? text.ToLower(Culture) : text;
        }

        #endregion
    }
}