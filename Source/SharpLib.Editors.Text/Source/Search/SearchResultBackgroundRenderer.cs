using System;
using System.Linq;
using System.Windows.Media;

using SharpLib.Notepad.Document;
using SharpLib.Notepad.Rendering;

namespace SharpLib.Notepad.Search
{
    internal class SearchResultBackgroundRenderer : IBackgroundRenderer
    {
        #region Поля

        private readonly TextSegmentCollection<SearchResult> currentResults = new TextSegmentCollection<SearchResult>();

        private Brush markerBrush;

        private Pen markerPen;

        #endregion

        #region Свойства

        public TextSegmentCollection<SearchResult> CurrentResults
        {
            get { return currentResults; }
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Selection; }
        }

        public Brush MarkerBrush
        {
            get { return markerBrush; }
            set
            {
                markerBrush = value;
                markerPen = new Pen(markerBrush, 1);
            }
        }

        #endregion

        #region Конструктор

        public SearchResultBackgroundRenderer()
        {
            markerBrush = Brushes.LightGreen;
            markerPen = new Pen(markerBrush, 1);
        }

        #endregion

        #region Методы

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }
            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }

            if (currentResults == null || !textView.VisualLinesValid)
            {
                return;
            }

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
            {
                return;
            }

            int viewStart = visualLines.First().FirstDocumentLine.Offset;
            int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;

            foreach (SearchResult result in currentResults.FindOverlappingSegments(viewStart, viewEnd - viewStart))
            {
                var geoBuilder = new BackgroundGeometryBuilder();
                geoBuilder.AlignToMiddleOfPixels = true;
                geoBuilder.CornerRadius = 3;
                geoBuilder.AddSegment(textView, result);
                var geometry = geoBuilder.CreateGeometry();
                if (geometry != null)
                {
                    drawingContext.DrawGeometry(markerBrush, markerPen, geometry);
                }
            }
        }

        #endregion
    }
}