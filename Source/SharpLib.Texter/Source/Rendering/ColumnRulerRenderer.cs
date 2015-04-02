using System;
using System.Windows;
using System.Windows.Media;

using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Rendering
{
    internal sealed class ColumnRulerRenderer : IBackgroundRenderer
    {
        #region Поля

        public static readonly Color DefaultForeground = Colors.LightGray;

        private readonly TextView textView;

        private int column;

        private Pen pen;

        #endregion

        #region Свойства

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        #endregion

        #region Конструктор

        public ColumnRulerRenderer(TextView textView)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }

            pen = new Pen(new SolidColorBrush(DefaultForeground), 1);
            pen.Freeze();
            this.textView = textView;
            this.textView.BackgroundRenderers.Add(this);
        }

        #endregion

        #region Методы

        public void SetRuler(int column, Pen pen)
        {
            if (this.column != column)
            {
                this.column = column;
                textView.InvalidateLayer(Layer);
            }
            if (this.pen != pen)
            {
                this.pen = pen;
                textView.InvalidateLayer(Layer);
            }
        }

        public void Draw(TextView textView, System.Windows.Media.DrawingContext drawingContext)
        {
            if (column < 1)
            {
                return;
            }
            double offset = textView.WideSpaceWidth * column;
            var pixelSize = PixelSnapHelpers.GetPixelSize(textView);
            double markerXPos = PixelSnapHelpers.PixelAlign(offset, pixelSize.Width);
            markerXPos -= textView.ScrollOffset.X;
            var start = new Point(markerXPos, 0);
            var end = new Point(markerXPos, Math.Max(textView.DocumentHeight, textView.ActualHeight));

            drawingContext.DrawLine(pen, start, end);
        }

        #endregion
    }
}