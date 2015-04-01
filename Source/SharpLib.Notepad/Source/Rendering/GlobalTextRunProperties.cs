using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace SharpLib.Notepad.Rendering
{
    internal sealed class GlobalTextRunProperties : TextRunProperties
    {
        #region Поля

        internal Brush backgroundBrush;

        internal System.Globalization.CultureInfo cultureInfo;

        internal double fontRenderingEmSize;

        internal Brush foregroundBrush;

        internal Typeface typeface;

        #endregion

        #region Свойства

        public override Typeface Typeface
        {
            get { return typeface; }
        }

        public override double FontRenderingEmSize
        {
            get { return fontRenderingEmSize; }
        }

        public override double FontHintingEmSize
        {
            get { return fontRenderingEmSize; }
        }

        public override TextDecorationCollection TextDecorations
        {
            get { return null; }
        }

        public override Brush ForegroundBrush
        {
            get { return foregroundBrush; }
        }

        public override Brush BackgroundBrush
        {
            get { return backgroundBrush; }
        }

        public override System.Globalization.CultureInfo CultureInfo
        {
            get { return cultureInfo; }
        }

        public override TextEffectCollection TextEffects
        {
            get { return null; }
        }

        #endregion
    }
}