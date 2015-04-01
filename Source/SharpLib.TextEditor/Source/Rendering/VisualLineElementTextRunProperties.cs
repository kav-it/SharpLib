using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering
{
    public class VisualLineElementTextRunProperties : TextRunProperties, ICloneable
    {
        #region Поля

        private Brush backgroundBrush;

        private BaselineAlignment baselineAlignment;

        private CultureInfo cultureInfo;

        private double fontHintingEmSize;

        private double fontRenderingEmSize;

        private Brush foregroundBrush;

        private NumberSubstitution numberSubstitution;

        private TextDecorationCollection textDecorations;

        private TextEffectCollection textEffects;

        private Typeface typeface;

        private TextRunTypographyProperties typographyProperties;

        #endregion

        #region Свойства

        public override Brush BackgroundBrush
        {
            get { return backgroundBrush; }
        }

        public override BaselineAlignment BaselineAlignment
        {
            get { return baselineAlignment; }
        }

        public override CultureInfo CultureInfo
        {
            get { return cultureInfo; }
        }

        public override double FontHintingEmSize
        {
            get { return fontHintingEmSize; }
        }

        public override double FontRenderingEmSize
        {
            get { return fontRenderingEmSize; }
        }

        public override Brush ForegroundBrush
        {
            get { return foregroundBrush; }
        }

        public override Typeface Typeface
        {
            get { return typeface; }
        }

        public override TextDecorationCollection TextDecorations
        {
            get { return textDecorations; }
        }

        public override TextEffectCollection TextEffects
        {
            get { return textEffects; }
        }

        public override TextRunTypographyProperties TypographyProperties
        {
            get { return typographyProperties; }
        }

        public override NumberSubstitution NumberSubstitution
        {
            get { return numberSubstitution; }
        }

        #endregion

        #region Конструктор

        public VisualLineElementTextRunProperties(TextRunProperties textRunProperties)
        {
            if (textRunProperties == null)
            {
                throw new ArgumentNullException("textRunProperties");
            }
            backgroundBrush = textRunProperties.BackgroundBrush;
            baselineAlignment = textRunProperties.BaselineAlignment;
            cultureInfo = textRunProperties.CultureInfo;
            fontHintingEmSize = textRunProperties.FontHintingEmSize;
            fontRenderingEmSize = textRunProperties.FontRenderingEmSize;
            foregroundBrush = textRunProperties.ForegroundBrush;
            typeface = textRunProperties.Typeface;
            textDecorations = textRunProperties.TextDecorations;
            if (textDecorations != null && !textDecorations.IsFrozen)
            {
                textDecorations = textDecorations.Clone();
            }
            textEffects = textRunProperties.TextEffects;
            if (textEffects != null && !textEffects.IsFrozen)
            {
                textEffects = textEffects.Clone();
            }
            typographyProperties = textRunProperties.TypographyProperties;
            numberSubstitution = textRunProperties.NumberSubstitution;
        }

        #endregion

        #region Методы

        public virtual VisualLineElementTextRunProperties Clone()
        {
            return new VisualLineElementTextRunProperties(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public void SetBackgroundBrush(Brush value)
        {
            ExtensionMethods.CheckIsFrozen(value);
            backgroundBrush = value;
        }

        public void SetBaselineAlignment(BaselineAlignment value)
        {
            baselineAlignment = value;
        }

        public void SetCultureInfo(CultureInfo value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            cultureInfo = value;
        }

        public void SetFontHintingEmSize(double value)
        {
            fontHintingEmSize = value;
        }

        public void SetFontRenderingEmSize(double value)
        {
            fontRenderingEmSize = value;
        }

        public void SetForegroundBrush(Brush value)
        {
            ExtensionMethods.CheckIsFrozen(value);
            foregroundBrush = value;
        }

        public void SetTypeface(Typeface value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            typeface = value;
        }

        public void SetTextDecorations(TextDecorationCollection value)
        {
            ExtensionMethods.CheckIsFrozen(value);
            textDecorations = value;
        }

        public void SetTextEffects(TextEffectCollection value)
        {
            ExtensionMethods.CheckIsFrozen(value);
            textEffects = value;
        }

        public void SetTypographyProperties(TextRunTypographyProperties value)
        {
            typographyProperties = value;
        }

        public void SetNumberSubstitution(NumberSubstitution value)
        {
            numberSubstitution = value;
        }

        #endregion
    }
}