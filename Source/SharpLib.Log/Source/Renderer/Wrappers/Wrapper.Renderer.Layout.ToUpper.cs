using System.ComponentModel;
using System.Globalization;

namespace SharpLib.Log
{
    [LayoutRenderer("uppercase")]
    [AmbientProperty("Uppercase")]
    [ThreadAgnostic]
    public sealed class UppercaseLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Свойства

        [DefaultValue(true)]
        public bool Uppercase { get; set; }

        public CultureInfo Culture { get; set; }

        #endregion

        #region Конструктор

        public UppercaseLayoutRendererWrapper()
        {
            Culture = CultureInfo.InvariantCulture;
            Uppercase = true;
        }

        #endregion

        #region Методы

        protected override string Transform(string text)
        {
            return Uppercase ? text.ToUpper(Culture) : text;
        }

        #endregion
    }
}