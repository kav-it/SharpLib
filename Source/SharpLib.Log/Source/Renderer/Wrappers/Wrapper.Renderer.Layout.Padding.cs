using System.ComponentModel;

namespace SharpLib.Log
{
    [LayoutRenderer("pad")]
    [AmbientProperty("Padding")]
    [AmbientProperty("PadCharacter")]
    [AmbientProperty("FixedLength")]
    [ThreadAgnostic]
    public sealed class PaddingLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Свойства

        public int Padding { get; set; }

        [DefaultValue(' ')]
        public char PadCharacter { get; set; }

        [DefaultValue(false)]
        public bool FixedLength { get; set; }

        #endregion

        #region Конструктор

        public PaddingLayoutRendererWrapper()
        {
            PadCharacter = ' ';
        }

        #endregion

        #region Методы

        protected override string Transform(string text)
        {
            string s = text ?? string.Empty;

            if (Padding != 0)
            {
                if (Padding > 0)
                {
                    s = s.PadLeft(Padding, PadCharacter);
                }
                else
                {
                    s = s.PadRight(-Padding, PadCharacter);
                }

                int absolutePadding = Padding;
                if (absolutePadding < 0)
                {
                    absolutePadding = -absolutePadding;
                }

                if (FixedLength && s.Length > absolutePadding)
                {
                    s = s.Substring(0, absolutePadding);
                }
            }

            return s;
        }

        #endregion
    }
}