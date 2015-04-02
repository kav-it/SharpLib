using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace SharpLib.Notepad.Rendering
{
    internal sealed class GlobalTextRunProperties : TextRunProperties
    {
        #region Поля

        // internal Brush _backgroundBrush;

        internal CultureInfo _cultureInfo;

        internal double _fontRenderingEmSize;

        internal Brush _foregroundBrush;

        internal Typeface _typeface;

        #endregion

        #region Свойства

        public override Typeface Typeface
        {
            get { return _typeface; }
        }

        public override double FontRenderingEmSize
        {
            get { return _fontRenderingEmSize; }
        }

        public override double FontHintingEmSize
        {
            get { return _fontRenderingEmSize; }
        }

        public override TextDecorationCollection TextDecorations
        {
            get { return null; }
        }

        public override Brush ForegroundBrush
        {
            get { return _foregroundBrush; }
        }

        public override Brush BackgroundBrush
        {
            get { return null; }
        }

        public override System.Globalization.CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
        }

        public override TextEffectCollection TextEffects
        {
            get { return null; }
        }

        #endregion
    }
}