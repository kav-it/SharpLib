using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace SharpLib.Notepad.Rendering
{
    public class FormattedTextRun : TextEmbeddedObject
    {
        #region Поля

        private readonly FormattedTextElement element;

        private readonly TextRunProperties properties;

        #endregion

        #region Свойства

        public FormattedTextElement Element
        {
            get { return element; }
        }

        public override LineBreakCondition BreakBefore
        {
            get { return element.BreakBefore; }
        }

        public override LineBreakCondition BreakAfter
        {
            get { return element.BreakAfter; }
        }

        public override bool HasFixedSize
        {
            get { return true; }
        }

        public override CharacterBufferReference CharacterBufferReference
        {
            get { return new CharacterBufferReference(); }
        }

        public override int Length
        {
            get { return element.VisualLength; }
        }

        public override TextRunProperties Properties
        {
            get { return properties; }
        }

        #endregion

        #region Конструктор

        public FormattedTextRun(FormattedTextElement element, TextRunProperties properties)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            this.properties = properties;
            this.element = element;
        }

        #endregion

        #region Методы

        public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
        {
            var formattedText = element.formattedText;
            if (formattedText != null)
            {
                return new TextEmbeddedObjectMetrics(formattedText.WidthIncludingTrailingWhitespace,
                    formattedText.Height,
                    formattedText.Baseline);
            }
            var text = element.textLine;
            return new TextEmbeddedObjectMetrics(text.WidthIncludingTrailingWhitespace,
                text.Height,
                text.Baseline);
        }

        public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
        {
            var formattedText = element.formattedText;
            if (formattedText != null)
            {
                return new Rect(0, 0, formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
            }
            var text = element.textLine;
            return new Rect(0, 0, text.WidthIncludingTrailingWhitespace, text.Height);
        }

        public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
        {
            if (element.formattedText != null)
            {
                origin.Y -= element.formattedText.Baseline;
                drawingContext.DrawText(element.formattedText, origin);
            }
            else
            {
                origin.Y -= element.textLine.Baseline;
                element.textLine.Draw(drawingContext, origin, InvertAxes.None);
            }
        }

        #endregion
    }
}