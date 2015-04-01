using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Folding
{
    internal sealed class FoldingMarginMarker : UIElement
    {
        #region Константы

        private const double MarginSizeFactor = 0.7;

        #endregion

        #region Поля

        internal FoldingSection FoldingSection;

        internal VisualLine VisualLine;

        private bool isExpanded;

        #endregion

        #region Свойства

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    InvalidateVisual();
                }
                if (FoldingSection != null)
                {
                    FoldingSection.IsFolded = !value;
                }
            }
        }

        #endregion

        #region Методы

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!e.Handled)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    IsExpanded = !IsExpanded;
                    e.Handled = true;
                }
            }
        }

        protected override Size MeasureCore(Size availableSize)
        {
            double size = MarginSizeFactor * FoldingMargin.SizeFactor * (double)GetValue(TextBlock.FontSizeProperty);
            size = PixelSnapHelpers.RoundToOdd(size, PixelSnapHelpers.GetPixelSize(this).Width);
            return new Size(size, size);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var margin = VisualParent as FoldingMargin;
            var activePen = new Pen(margin.SelectedFoldingMarkerBrush, 1);
            var inactivePen = new Pen(margin.FoldingMarkerBrush, 1);
            activePen.StartLineCap = inactivePen.StartLineCap = PenLineCap.Square;
            activePen.EndLineCap = inactivePen.EndLineCap = PenLineCap.Square;
            var pixelSize = PixelSnapHelpers.GetPixelSize(this);
            var rect = new Rect(pixelSize.Width / 2,
                pixelSize.Height / 2,
                RenderSize.Width - pixelSize.Width,
                RenderSize.Height - pixelSize.Height);
            drawingContext.DrawRectangle(
                IsMouseDirectlyOver ? margin.SelectedFoldingMarkerBackgroundBrush : margin.FoldingMarkerBackgroundBrush,
                IsMouseDirectlyOver ? activePen : inactivePen, rect);
            double middleX = rect.Left + rect.Width / 2;
            double middleY = rect.Top + rect.Height / 2;
            double space = PixelSnapHelpers.Round(rect.Width / 8, pixelSize.Width) + pixelSize.Width;
            drawingContext.DrawLine(activePen,
                new Point(rect.Left + space, middleY),
                new Point(rect.Right - space, middleY));
            if (!isExpanded)
            {
                drawingContext.DrawLine(activePen,
                    new Point(middleX, rect.Top + space),
                    new Point(middleX, rect.Bottom - space));
            }
        }

        protected override void OnIsMouseDirectlyOverChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsMouseDirectlyOverChanged(e);
            InvalidateVisual();
        }

        #endregion
    }
}