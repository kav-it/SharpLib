﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using SharpLib.Texter.Document;
using SharpLib.Texter.Editing;
using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Rendering
{
    public sealed class BackgroundGeometryBuilder
    {
        #region Поля

        private readonly PathFigureCollection figures = new PathFigureCollection();

        private double cornerRadius;

        private PathFigure figure;

        private int insertionIndex;

        private double lastBottom;

        private double lastLeft, lastRight;

        private double lastTop;

        #endregion

        #region Свойства

        public double CornerRadius
        {
            get { return cornerRadius; }
            set { cornerRadius = value; }
        }

        public bool AlignToWholePixels { get; set; }

        public bool AlignToMiddleOfPixels { get; set; }

        public bool ExtendToFullWidthAtLineEnd { get; set; }

        #endregion

        #region Методы

        public void AddSegment(TextView textView, ISegment segment)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }
            var pixelSize = PixelSnapHelpers.GetPixelSize(textView);
            foreach (Rect r in GetRectsForSegment(textView, segment, ExtendToFullWidthAtLineEnd))
            {
                AddRectangle(pixelSize, r);
            }
        }

        public void AddRectangle(TextView textView, Rect rectangle)
        {
            AddRectangle(PixelSnapHelpers.GetPixelSize(textView), rectangle);
        }

        private void AddRectangle(Size pixelSize, Rect r)
        {
            if (AlignToWholePixels)
            {
                AddRectangle(PixelSnapHelpers.Round(r.Left, pixelSize.Width),
                    PixelSnapHelpers.Round(r.Top + 1, pixelSize.Height),
                    PixelSnapHelpers.Round(r.Right, pixelSize.Width),
                    PixelSnapHelpers.Round(r.Bottom + 1, pixelSize.Height));
            }
            else if (AlignToMiddleOfPixels)
            {
                AddRectangle(PixelSnapHelpers.PixelAlign(r.Left, pixelSize.Width),
                    PixelSnapHelpers.PixelAlign(r.Top + 1, pixelSize.Height),
                    PixelSnapHelpers.PixelAlign(r.Right, pixelSize.Width),
                    PixelSnapHelpers.PixelAlign(r.Bottom + 1, pixelSize.Height));
            }
            else
            {
                AddRectangle(r.Left, r.Top + 1, r.Right, r.Bottom + 1);
            }
        }

        public static IEnumerable<Rect> GetRectsForSegment(TextView textView, ISegment segment, bool extendToFullWidthAtLineEnd = false)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }
            if (segment == null)
            {
                throw new ArgumentNullException("segment");
            }
            return GetRectsForSegmentImpl(textView, segment, extendToFullWidthAtLineEnd);
        }

        private static IEnumerable<Rect> GetRectsForSegmentImpl(TextView textView, ISegment segment, bool extendToFullWidthAtLineEnd)
        {
            int segmentStart = segment.Offset;
            int segmentEnd = segment.Offset + segment.Length;

            segmentStart = segmentStart.CoerceValue(0, textView.Document.TextLength);
            segmentEnd = segmentEnd.CoerceValue(0, textView.Document.TextLength);

            TextViewPosition start;
            TextViewPosition end;

            if (segment is SelectionSegment)
            {
                var sel = (SelectionSegment)segment;
                start = new TextViewPosition(textView.Document.GetLocation(sel.StartOffset), sel.StartVisualColumn);
                end = new TextViewPosition(textView.Document.GetLocation(sel.EndOffset), sel.EndVisualColumn);
            }
            else
            {
                start = new TextViewPosition(textView.Document.GetLocation(segmentStart));
                end = new TextViewPosition(textView.Document.GetLocation(segmentEnd));
            }

            foreach (VisualLine vl in textView.VisualLines)
            {
                int vlStartOffset = vl.FirstDocumentLine.Offset;
                if (vlStartOffset > segmentEnd)
                {
                    break;
                }
                int vlEndOffset = vl.LastDocumentLine.Offset + vl.LastDocumentLine.Length;
                if (vlEndOffset < segmentStart)
                {
                    continue;
                }

                int segmentStartVC;
                if (segmentStart < vlStartOffset)
                {
                    segmentStartVC = 0;
                }
                else
                {
                    segmentStartVC = vl.ValidateVisualColumn(start, extendToFullWidthAtLineEnd);
                }

                int segmentEndVC;
                if (segmentEnd > vlEndOffset)
                {
                    segmentEndVC = extendToFullWidthAtLineEnd ? int.MaxValue : vl.VisualLengthWithEndOfLineMarker;
                }
                else
                {
                    segmentEndVC = vl.ValidateVisualColumn(end, extendToFullWidthAtLineEnd);
                }

                foreach (var rect in ProcessTextLines(textView, vl, segmentStartVC, segmentEndVC))
                {
                    yield return rect;
                }
            }
        }

        public static IEnumerable<Rect> GetRectsFromVisualSegment(TextView textView, VisualLine line, int startVC, int endVC)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }
            return ProcessTextLines(textView, line, startVC, endVC);
        }

        private static IEnumerable<Rect> ProcessTextLines(TextView textView, VisualLine visualLine, int segmentStartVC, int segmentEndVC)
        {
            var lastTextLine = visualLine.TextLines.Last();
            var scrollOffset = textView.ScrollOffset;

            for (int i = 0; i < visualLine.TextLines.Count; i++)
            {
                var line = visualLine.TextLines[i];
                double y = visualLine.GetTextLineVisualYPosition(line, VisualYPosition.LineTop);
                int visualStartCol = visualLine.GetTextLineVisualStartColumn(line);
                int visualEndCol = visualStartCol + line.Length;
                if (line != lastTextLine)
                {
                    visualEndCol -= line.TrailingWhitespaceLength;
                }

                if (segmentEndVC < visualStartCol)
                {
                    break;
                }
                if (lastTextLine != line && segmentStartVC > visualEndCol)
                {
                    continue;
                }
                int segmentStartVCInLine = Math.Max(segmentStartVC, visualStartCol);
                int segmentEndVCInLine = Math.Min(segmentEndVC, visualEndCol);
                y -= scrollOffset.Y;
                if (segmentStartVCInLine == segmentEndVCInLine)
                {
                    double pos = visualLine.GetTextLineVisualXPosition(line, segmentStartVCInLine);
                    pos -= scrollOffset.X;

                    if (segmentEndVCInLine == visualEndCol && i < visualLine.TextLines.Count - 1 && segmentEndVC > segmentEndVCInLine && line.TrailingWhitespaceLength == 0)
                    {
                        continue;
                    }
                    if (segmentStartVCInLine == visualStartCol && i > 0 && segmentStartVC < segmentStartVCInLine && visualLine.TextLines[i - 1].TrailingWhitespaceLength == 0)
                    {
                        continue;
                    }
                    yield return new Rect(pos, y, 1, line.Height);
                }
                else
                {
                    var lastRect = Rect.Empty;
                    if (segmentStartVCInLine <= visualEndCol)
                    {
                        foreach (TextBounds b in line.GetTextBounds(segmentStartVCInLine, segmentEndVCInLine - segmentStartVCInLine))
                        {
                            double left = b.Rectangle.Left - scrollOffset.X;
                            double right = b.Rectangle.Right - scrollOffset.X;
                            if (!lastRect.IsEmpty)
                            {
                                yield return lastRect;
                            }

                            lastRect = new Rect(Math.Min(left, right), y, Math.Abs(right - left), line.Height);
                        }
                    }
                    if (segmentEndVC >= visualLine.VisualLengthWithEndOfLineMarker)
                    {
                        double left = (segmentStartVC > visualLine.VisualLengthWithEndOfLineMarker ? visualLine.GetTextLineVisualXPosition(lastTextLine, segmentStartVC) : line.Width) - scrollOffset.X;
                        double right = ((segmentEndVC == int.MaxValue || line != lastTextLine)
                            ? Math.Max(((IScrollInfo)textView).ExtentWidth, ((IScrollInfo)textView).ViewportWidth)
                            : visualLine.GetTextLineVisualXPosition(lastTextLine, segmentEndVC)) - scrollOffset.X;
                        var extendSelection = new Rect(Math.Min(left, right), y, Math.Abs(right - left), line.Height);
                        if (!lastRect.IsEmpty)
                        {
                            if (extendSelection.IntersectsWith(lastRect))
                            {
                                lastRect.Union(extendSelection);
                                yield return lastRect;
                            }
                            else
                            {
                                yield return lastRect;
                                yield return extendSelection;
                            }
                        }
                        else
                        {
                            yield return extendSelection;
                        }
                    }
                    else
                    {
                        yield return lastRect;
                    }
                }
            }
        }

        public void AddRectangle(double left, double top, double right, double bottom)
        {
            if (!top.IsClose(lastBottom))
            {
                CloseFigure();
            }
            if (figure == null)
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(left, top + cornerRadius);
                if (Math.Abs(left - right) > cornerRadius)
                {
                    figure.Segments.Add(MakeArc(left + cornerRadius, top, SweepDirection.Clockwise));
                    figure.Segments.Add(MakeLineSegment(right - cornerRadius, top));
                    figure.Segments.Add(MakeArc(right, top + cornerRadius, SweepDirection.Clockwise));
                }
                figure.Segments.Add(MakeLineSegment(right, bottom - cornerRadius));
                insertionIndex = figure.Segments.Count;
            }
            else
            {
                if (!lastRight.IsClose(right))
                {
                    double cr = right < lastRight ? -cornerRadius : cornerRadius;
                    var dir1 = right < lastRight ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                    var dir2 = right < lastRight ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
                    figure.Segments.Insert(insertionIndex++, MakeArc(lastRight + cr, lastBottom, dir1));
                    figure.Segments.Insert(insertionIndex++, MakeLineSegment(right - cr, top));
                    figure.Segments.Insert(insertionIndex++, MakeArc(right, top + cornerRadius, dir2));
                }
                figure.Segments.Insert(insertionIndex++, MakeLineSegment(right, bottom - cornerRadius));
                figure.Segments.Insert(insertionIndex, MakeLineSegment(lastLeft, lastTop + cornerRadius));
                if (!lastLeft.IsClose(left))
                {
                    double cr = left < lastLeft ? cornerRadius : -cornerRadius;
                    var dir1 = left < lastLeft ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
                    var dir2 = left < lastLeft ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                    figure.Segments.Insert(insertionIndex, MakeArc(lastLeft, lastBottom - cornerRadius, dir1));
                    figure.Segments.Insert(insertionIndex, MakeLineSegment(lastLeft - cr, lastBottom));
                    figure.Segments.Insert(insertionIndex, MakeArc(left + cr, lastBottom, dir2));
                }
            }
            lastTop = top;
            lastBottom = bottom;
            lastLeft = left;
            lastRight = right;
        }

        private ArcSegment MakeArc(double x, double y, SweepDirection dir)
        {
            var arc = new ArcSegment(
                new Point(x, y),
                new Size(cornerRadius, cornerRadius),
                0, false, dir, true);
            arc.Freeze();
            return arc;
        }

        private static LineSegment MakeLineSegment(double x, double y)
        {
            var ls = new LineSegment(new Point(x, y), true);
            ls.Freeze();
            return ls;
        }

        public void CloseFigure()
        {
            if (figure != null)
            {
                figure.Segments.Insert(insertionIndex, MakeLineSegment(lastLeft, lastTop + cornerRadius));
                if (Math.Abs(lastLeft - lastRight) > cornerRadius)
                {
                    figure.Segments.Insert(insertionIndex, MakeArc(lastLeft, lastBottom - cornerRadius, SweepDirection.Clockwise));
                    figure.Segments.Insert(insertionIndex, MakeLineSegment(lastLeft + cornerRadius, lastBottom));
                    figure.Segments.Insert(insertionIndex, MakeArc(lastRight - cornerRadius, lastBottom, SweepDirection.Clockwise));
                }

                figure.IsClosed = true;
                figures.Add(figure);
                figure = null;
            }
        }

        public Geometry CreateGeometry()
        {
            CloseFigure();
            if (figures.Count != 0)
            {
                var g = new PathGeometry(figures);
                g.Freeze();
                return g;
            }
            return null;
        }

        #endregion
    }
}