using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace SharpLib.Texter.Rendering
{
    public class InlineObjectRun : TextEmbeddedObject
    {
        #region Поля

        private readonly UIElement element;

        private readonly int length;

        private readonly TextRunProperties properties;

        internal Size desiredSize;

        #endregion

        #region Свойства

        public UIElement Element
        {
            get { return element; }
        }

        public VisualLine VisualLine { get; internal set; }

        public override LineBreakCondition BreakBefore
        {
            get { return LineBreakCondition.BreakDesired; }
        }

        public override LineBreakCondition BreakAfter
        {
            get { return LineBreakCondition.BreakDesired; }
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
            get { return length; }
        }

        public override TextRunProperties Properties
        {
            get { return properties; }
        }

        #endregion

        #region Конструктор

        public InlineObjectRun(int length, TextRunProperties properties, UIElement element)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Value must be positive");
            }
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            this.length = length;
            this.properties = properties;
            this.element = element;
        }

        #endregion

        #region Методы

        public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
        {
            double baseline = TextBlock.GetBaselineOffset(element);
            if (double.IsNaN(baseline))
            {
                baseline = desiredSize.Height;
            }
            return new TextEmbeddedObjectMetrics(desiredSize.Width, desiredSize.Height, baseline);
        }

        public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
        {
            if (element.IsArrangeValid)
            {
                double baseline = TextBlock.GetBaselineOffset(element);
                if (double.IsNaN(baseline))
                {
                    baseline = desiredSize.Height;
                }
                return new Rect(new Point(0, -baseline), desiredSize);
            }
            return Rect.Empty;
        }

        public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
        {
        }

        #endregion
    }
}