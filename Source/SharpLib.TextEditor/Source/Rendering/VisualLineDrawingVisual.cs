using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering
{
    sealed class VisualLineDrawingVisual : DrawingVisual
    {
        public readonly VisualLine VisualLine;
        public readonly double Height;
        internal bool IsAdded;
		
        public VisualLineDrawingVisual(VisualLine visualLine)
        {
            this.VisualLine = visualLine;
            var drawingContext = RenderOpen();
            double pos = 0;
            foreach (TextLine textLine in visualLine.TextLines) {
                textLine.Draw(drawingContext, new Point(0, pos), InvertAxes.None);
                pos += textLine.Height;
            }
            this.Height = pos;
            drawingContext.Close();
        }
		
        protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        {
            return null;
        }
		
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return null;
        }
    }
}