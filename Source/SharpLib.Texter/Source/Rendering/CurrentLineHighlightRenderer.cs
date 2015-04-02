using System;
using System.Windows;
using System.Windows.Media;

namespace SharpLib.Texter.Rendering
{
    internal sealed class CurrentLineHighlightRenderer : IBackgroundRenderer
    {
        #region Поля

        public static readonly Color DefaultBackground = Color.FromArgb(22, 20, 220, 224);

        public static readonly Color DefaultBorder = Color.FromArgb(52, 0, 255, 110);

        private readonly TextView textView;

        private int line;

        #endregion

        #region Свойства

        public int Line
        {
            get { return line; }
            set
            {
                if (line != value)
                {
                    line = value;
                    textView.InvalidateLayer(Layer);
                }
            }
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Selection; }
        }

        public Brush BackgroundBrush { get; set; }

        public Pen BorderPen { get; set; }

        #endregion

        #region Конструктор

        public CurrentLineHighlightRenderer(TextView textView)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }

            BorderPen = new Pen(new SolidColorBrush(DefaultBorder), 1);
            BorderPen.Freeze();

            BackgroundBrush = new SolidColorBrush(DefaultBackground);
            BackgroundBrush.Freeze();

            this.textView = textView;
            this.textView.BackgroundRenderers.Add(this);

            line = 0;
        }

        #endregion

        #region Методы

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (!this.textView.Options.HighlightCurrentLine)
            {
                return;
            }

            var builder = new BackgroundGeometryBuilder();

            var visualLine = this.textView.GetVisualLine(line);
            if (visualLine == null)
            {
                return;
            }

            var linePosY = visualLine.VisualTop - this.textView.ScrollOffset.Y;

            builder.AddRectangle(textView, new Rect(0, linePosY, textView.ActualWidth, visualLine.Height));

            var geometry = builder.CreateGeometry();
            if (geometry != null)
            {
                drawingContext.DrawGeometry(BackgroundBrush, BorderPen, geometry);
            }
        }

        #endregion
    }
}