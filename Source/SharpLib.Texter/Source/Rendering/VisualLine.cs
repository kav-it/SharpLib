using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;

using SharpLib.Texter.Document;
using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Rendering
{
    public sealed class VisualLine
    {
        #region Перечисления

        private enum LifetimePhase : byte
        {
            Generating,

            Transforming,

            Live,

            Disposed
        }

        #endregion

        #region Поля

        private readonly TextView textView;

        private List<VisualLineElement> elements;

        internal bool hasInlineObjects;

        private LifetimePhase phase;

        private ReadOnlyCollection<TextLine> textLines;

        private VisualLineDrawingVisual visual;

        #endregion

        #region Свойства

        public TextDocument Document { get; private set; }

        public DocumentLine FirstDocumentLine { get; private set; }

        public DocumentLine LastDocumentLine { get; private set; }

        public ReadOnlyCollection<VisualLineElement> Elements { get; private set; }

        public ReadOnlyCollection<TextLine> TextLines
        {
            get
            {
                if (phase < LifetimePhase.Live)
                {
                    throw new InvalidOperationException();
                }
                return textLines;
            }
        }

        public int StartOffset
        {
            get { return FirstDocumentLine.Offset; }
        }

        public int VisualLength { get; private set; }

        public int VisualLengthWithEndOfLineMarker
        {
            get
            {
                int length = VisualLength;
                if (textView.Options.ShowEndOfLine && LastDocumentLine.NextLine != null)
                {
                    length++;
                }
                return length;
            }
        }

        public double Height { get; private set; }

        public double VisualTop { get; internal set; }

        public bool IsDisposed
        {
            get { return phase == LifetimePhase.Disposed; }
        }

        #endregion

        #region Конструктор

        internal VisualLine(TextView textView, DocumentLine firstDocumentLine)
        {
            Debug.Assert(textView != null);
            Debug.Assert(firstDocumentLine != null);
            this.textView = textView;
            Document = textView.Document;
            FirstDocumentLine = firstDocumentLine;
        }

        #endregion

        #region Методы

        internal void ConstructVisualElements(ITextRunConstructionContext context, VisualLineElementGenerator[] generators)
        {
            Debug.Assert(phase == LifetimePhase.Generating);
            foreach (VisualLineElementGenerator g in generators)
            {
                g.StartGeneration(context);
            }
            elements = new List<VisualLineElement>();
            PerformVisualElementConstruction(generators);
            foreach (VisualLineElementGenerator g in generators)
            {
                g.FinishGeneration();
            }

            var globalTextRunProperties = context.GlobalTextRunProperties;
            foreach (var element in elements)
            {
                element.SetTextRunProperties(new VisualLineElementTextRunProperties(globalTextRunProperties));
            }
            Elements = elements.AsReadOnly();
            CalculateOffsets();
            phase = LifetimePhase.Transforming;
        }

        private void PerformVisualElementConstruction(VisualLineElementGenerator[] generators)
        {
            var document = Document;
            int offset = FirstDocumentLine.Offset;
            int currentLineEnd = offset + FirstDocumentLine.Length;
            LastDocumentLine = FirstDocumentLine;
            int askInterestOffset = 0;
            while (offset + askInterestOffset <= currentLineEnd)
            {
                int textPieceEndOffset = currentLineEnd;
                foreach (VisualLineElementGenerator g in generators)
                {
                    g.cachedInterest = g.GetFirstInterestedOffset(offset + askInterestOffset);
                    if (g.cachedInterest != -1)
                    {
                        if (g.cachedInterest < offset)
                        {
                            throw new ArgumentOutOfRangeException(g.GetType().Name + ".GetFirstInterestedOffset",
                                g.cachedInterest,
                                "GetFirstInterestedOffset must not return an offset less than startOffset. Return -1 to signal no interest.");
                        }
                        if (g.cachedInterest < textPieceEndOffset)
                        {
                            textPieceEndOffset = g.cachedInterest;
                        }
                    }
                }
                Debug.Assert(textPieceEndOffset >= offset);
                if (textPieceEndOffset > offset)
                {
                    int textPieceLength = textPieceEndOffset - offset;
                    elements.Add(new VisualLineText(this, textPieceLength));
                    offset = textPieceEndOffset;
                }

                askInterestOffset = 1;
                foreach (VisualLineElementGenerator g in generators)
                {
                    if (g.cachedInterest == offset)
                    {
                        var element = g.ConstructElement(offset);
                        if (element != null)
                        {
                            elements.Add(element);
                            if (element.DocumentLength > 0)
                            {
                                askInterestOffset = 0;
                                offset += element.DocumentLength;
                                if (offset > currentLineEnd)
                                {
                                    var newEndLine = document.GetLineByOffset(offset);
                                    currentLineEnd = newEndLine.Offset + newEndLine.Length;
                                    LastDocumentLine = newEndLine;
                                    if (currentLineEnd < offset)
                                    {
                                        throw new InvalidOperationException(
                                            "The VisualLineElementGenerator " + g.GetType().Name +
                                            " produced an element which ends within the line delimiter");
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void CalculateOffsets()
        {
            int visualOffset = 0;
            int textOffset = 0;
            foreach (VisualLineElement element in elements)
            {
                element.VisualColumn = visualOffset;
                element.RelativeTextOffset = textOffset;
                visualOffset += element.VisualLength;
                textOffset += element.DocumentLength;
            }
            VisualLength = visualOffset;
            Debug.Assert(textOffset == LastDocumentLine.EndOffset - FirstDocumentLine.Offset);
        }

        internal void RunTransformers(ITextRunConstructionContext context, IVisualLineTransformer[] transformers)
        {
            Debug.Assert(phase == LifetimePhase.Transforming);
            foreach (IVisualLineTransformer transformer in transformers)
            {
                transformer.Transform(context, elements);
            }

            if (elements.Any(e => e.TextRunProperties.TypographyProperties != null))
            {
                foreach (VisualLineElement element in elements)
                {
                    if (element.TextRunProperties.TypographyProperties == null)
                    {
                        element.TextRunProperties.SetTypographyProperties(new DefaultTextRunTypographyProperties());
                    }
                }
            }
            phase = LifetimePhase.Live;
        }

        public void ReplaceElement(int elementIndex, params VisualLineElement[] newElements)
        {
            ReplaceElement(elementIndex, 1, newElements);
        }

        public void ReplaceElement(int elementIndex, int count, params VisualLineElement[] newElements)
        {
            if (phase != LifetimePhase.Transforming)
            {
                throw new InvalidOperationException("This method may only be called by line transformers.");
            }
            int oldDocumentLength = 0;
            for (int i = elementIndex; i < elementIndex + count; i++)
            {
                oldDocumentLength += elements[i].DocumentLength;
            }
            int newDocumentLength = 0;
            foreach (var newElement in newElements)
            {
                newDocumentLength += newElement.DocumentLength;
            }
            if (oldDocumentLength != newDocumentLength)
            {
                throw new InvalidOperationException("Old elements have document length " + oldDocumentLength + ", but new elements have length " + newDocumentLength);
            }
            elements.RemoveRange(elementIndex, count);
            elements.InsertRange(elementIndex, newElements);
            CalculateOffsets();
        }

        internal void SetTextLines(List<TextLine> textLines)
        {
            this.textLines = textLines.AsReadOnly();
            Height = 0;
            foreach (TextLine line in textLines)
            {
                Height += line.Height;
            }
        }

        public int GetVisualColumn(int relativeTextOffset)
        {
            ThrowUtil.CheckNotNegative(relativeTextOffset, "relativeTextOffset");
            foreach (VisualLineElement element in elements)
            {
                if (element.RelativeTextOffset <= relativeTextOffset
                    && element.RelativeTextOffset + element.DocumentLength >= relativeTextOffset)
                {
                    return element.GetVisualColumn(relativeTextOffset);
                }
            }
            return VisualLength;
        }

        public int GetRelativeOffset(int visualColumn)
        {
            ThrowUtil.CheckNotNegative(visualColumn, "visualColumn");
            int documentLength = 0;
            foreach (VisualLineElement element in elements)
            {
                if (element.VisualColumn <= visualColumn
                    && element.VisualColumn + element.VisualLength > visualColumn)
                {
                    return element.GetRelativeOffset(visualColumn);
                }
                documentLength += element.DocumentLength;
            }
            return documentLength;
        }

        public TextLine GetTextLine(int visualColumn)
        {
            return GetTextLine(visualColumn, false);
        }

        public TextLine GetTextLine(int visualColumn, bool isAtEndOfLine)
        {
            if (visualColumn < 0)
            {
                throw new ArgumentOutOfRangeException("visualColumn");
            }
            if (visualColumn >= VisualLengthWithEndOfLineMarker)
            {
                return TextLines[TextLines.Count - 1];
            }
            foreach (TextLine line in TextLines)
            {
                if (isAtEndOfLine ? visualColumn <= line.Length : visualColumn < line.Length)
                {
                    return line;
                }
                visualColumn -= line.Length;
            }
            throw new InvalidOperationException("Shouldn't happen (VisualLength incorrect?)");
        }

        public double GetTextLineVisualYPosition(TextLine textLine, VisualYPosition yPositionMode)
        {
            if (textLine == null)
            {
                throw new ArgumentNullException("textLine");
            }
            double pos = VisualTop;
            foreach (TextLine tl in TextLines)
            {
                if (tl == textLine)
                {
                    switch (yPositionMode)
                    {
                        case VisualYPosition.LineTop:
                            return pos;
                        case VisualYPosition.LineMiddle:
                            return pos + tl.Height / 2;
                        case VisualYPosition.LineBottom:
                            return pos + tl.Height;
                        case VisualYPosition.TextTop:
                            return pos + tl.Baseline - textView.DefaultBaseline;
                        case VisualYPosition.TextBottom:
                            return pos + tl.Baseline - textView.DefaultBaseline + textView.DefaultLineHeight;
                        case VisualYPosition.TextMiddle:
                            return pos + tl.Baseline - textView.DefaultBaseline + textView.DefaultLineHeight / 2;
                        case VisualYPosition.Baseline:
                            return pos + tl.Baseline;
                        default:
                            throw new ArgumentException("Invalid yPositionMode:" + yPositionMode);
                    }
                }
                pos += tl.Height;
            }
            throw new ArgumentException("textLine is not a line in this VisualLine");
        }

        public int GetTextLineVisualStartColumn(TextLine textLine)
        {
            if (!TextLines.Contains(textLine))
            {
                throw new ArgumentException("textLine is not a line in this VisualLine");
            }
            int col = 0;
            foreach (TextLine tl in TextLines)
            {
                if (tl == textLine)
                {
                    break;
                }
                col += tl.Length;
            }
            return col;
        }

        public TextLine GetTextLineByVisualYPosition(double visualTop)
        {
            const double epsilon = 0.0001;
            double pos = VisualTop;
            foreach (TextLine tl in TextLines)
            {
                pos += tl.Height;
                if (visualTop + epsilon < pos)
                {
                    return tl;
                }
            }
            return TextLines[TextLines.Count - 1];
        }

        public Point GetVisualPosition(int visualColumn, VisualYPosition yPositionMode)
        {
            var textLine = GetTextLine(visualColumn);
            double xPos = GetTextLineVisualXPosition(textLine, visualColumn);
            double yPos = GetTextLineVisualYPosition(textLine, yPositionMode);
            return new Point(xPos, yPos);
        }

        internal Point GetVisualPosition(int visualColumn, bool isAtEndOfLine, VisualYPosition yPositionMode)
        {
            var textLine = GetTextLine(visualColumn, isAtEndOfLine);
            double xPos = GetTextLineVisualXPosition(textLine, visualColumn);
            double yPos = GetTextLineVisualYPosition(textLine, yPositionMode);
            return new Point(xPos, yPos);
        }

        public double GetTextLineVisualXPosition(TextLine textLine, int visualColumn)
        {
            if (textLine == null)
            {
                throw new ArgumentNullException("textLine");
            }
            double xPos = textLine.GetDistanceFromCharacterHit(
                new CharacterHit(Math.Min(visualColumn, VisualLengthWithEndOfLineMarker), 0));
            if (visualColumn > VisualLengthWithEndOfLineMarker)
            {
                xPos += (visualColumn - VisualLengthWithEndOfLineMarker) * textView.WideSpaceWidth;
            }
            return xPos;
        }

        public int GetVisualColumn(Point point)
        {
            return GetVisualColumn(point, textView.Options.EnableVirtualSpace);
        }

        public int GetVisualColumn(Point point, bool allowVirtualSpace)
        {
            return GetVisualColumn(GetTextLineByVisualYPosition(point.Y), point.X, allowVirtualSpace);
        }

        internal int GetVisualColumn(Point point, bool allowVirtualSpace, out bool isAtEndOfLine)
        {
            var textLine = GetTextLineByVisualYPosition(point.Y);
            int vc = GetVisualColumn(textLine, point.X, allowVirtualSpace);
            isAtEndOfLine = (vc >= GetTextLineVisualStartColumn(textLine) + textLine.Length);
            return vc;
        }

        public int GetVisualColumn(TextLine textLine, double xPos, bool allowVirtualSpace)
        {
            if (xPos > textLine.WidthIncludingTrailingWhitespace)
            {
                if (allowVirtualSpace && textLine == TextLines[TextLines.Count - 1])
                {
                    int virtualX = (int)Math.Round((xPos - textLine.WidthIncludingTrailingWhitespace) / textView.WideSpaceWidth);
                    return VisualLengthWithEndOfLineMarker + virtualX;
                }
            }
            var ch = textLine.GetCharacterHitFromDistance(xPos);
            return ch.FirstCharacterIndex + ch.TrailingLength;
        }

        public int ValidateVisualColumn(TextViewPosition position, bool allowVirtualSpace)
        {
            return ValidateVisualColumn(Document.GetOffset(position.Location), position.VisualColumn, allowVirtualSpace);
        }

        public int ValidateVisualColumn(int offset, int visualColumn, bool allowVirtualSpace)
        {
            int firstDocumentLineOffset = FirstDocumentLine.Offset;
            if (visualColumn < 0)
            {
                return GetVisualColumn(offset - firstDocumentLineOffset);
            }
            int offsetFromVisualColumn = GetRelativeOffset(visualColumn);
            offsetFromVisualColumn += firstDocumentLineOffset;
            if (offsetFromVisualColumn != offset)
            {
                return GetVisualColumn(offset - firstDocumentLineOffset);
            }
            if (visualColumn > VisualLength && !allowVirtualSpace)
            {
                return VisualLength;
            }
            return visualColumn;
        }

        public int GetVisualColumnFloor(Point point)
        {
            return GetVisualColumnFloor(point, textView.Options.EnableVirtualSpace);
        }

        public int GetVisualColumnFloor(Point point, bool allowVirtualSpace)
        {
            bool tmp;
            return GetVisualColumnFloor(point, allowVirtualSpace, out tmp);
        }

        internal int GetVisualColumnFloor(Point point, bool allowVirtualSpace, out bool isAtEndOfLine)
        {
            var textLine = GetTextLineByVisualYPosition(point.Y);
            if (point.X > textLine.WidthIncludingTrailingWhitespace)
            {
                isAtEndOfLine = true;
                if (allowVirtualSpace && textLine == TextLines[TextLines.Count - 1])
                {
                    int virtualX = (int)((point.X - textLine.WidthIncludingTrailingWhitespace) / textView.WideSpaceWidth);
                    return VisualLengthWithEndOfLineMarker + virtualX;
                }
                return GetTextLineVisualStartColumn(textLine) + textLine.Length;
            }
            isAtEndOfLine = false;
            var ch = textLine.GetCharacterHitFromDistance(point.X);
            return ch.FirstCharacterIndex;
        }

        public TextViewPosition GetTextViewPosition(int visualColumn)
        {
            int documentOffset = GetRelativeOffset(visualColumn) + FirstDocumentLine.Offset;
            return new TextViewPosition(Document.GetLocation(documentOffset), visualColumn);
        }

        public TextViewPosition GetTextViewPosition(Point visualPosition, bool allowVirtualSpace)
        {
            bool isAtEndOfLine;
            int visualColumn = GetVisualColumn(visualPosition, allowVirtualSpace, out isAtEndOfLine);
            int documentOffset = GetRelativeOffset(visualColumn) + FirstDocumentLine.Offset;
            var pos = new TextViewPosition(Document.GetLocation(documentOffset), visualColumn);
            pos.IsAtEndOfLine = isAtEndOfLine;
            return pos;
        }

        public TextViewPosition GetTextViewPositionFloor(Point visualPosition, bool allowVirtualSpace)
        {
            bool isAtEndOfLine;
            int visualColumn = GetVisualColumnFloor(visualPosition, allowVirtualSpace, out isAtEndOfLine);
            int documentOffset = GetRelativeOffset(visualColumn) + FirstDocumentLine.Offset;
            var pos = new TextViewPosition(Document.GetLocation(documentOffset), visualColumn);
            pos.IsAtEndOfLine = isAtEndOfLine;
            return pos;
        }

        internal void Dispose()
        {
            if (phase == LifetimePhase.Disposed)
            {
                return;
            }
            Debug.Assert(phase == LifetimePhase.Live);
            phase = LifetimePhase.Disposed;
            foreach (TextLine textLine in TextLines)
            {
                textLine.Dispose();
            }
        }

        public int GetNextCaretPosition(int visualColumn, LogicalDirection direction, CaretPositioningMode mode, bool allowVirtualSpace)
        {
            if (!HasStopsInVirtualSpace(mode))
            {
                allowVirtualSpace = false;
            }

            if (elements.Count == 0)
            {
                if (allowVirtualSpace)
                {
                    if (direction == LogicalDirection.Forward)
                    {
                        return Math.Max(0, visualColumn + 1);
                    }
                    if (visualColumn > 0)
                    {
                        return visualColumn - 1;
                    }
                    return -1;
                }
                if (visualColumn < 0 && direction == LogicalDirection.Forward)
                {
                    return 0;
                }
                if (visualColumn > 0 && direction == LogicalDirection.Backward)
                {
                    return 0;
                }
                return -1;
            }

            int i;
            if (direction == LogicalDirection.Backward)
            {
                if (visualColumn > VisualLength && !elements[elements.Count - 1].HandlesLineBorders && HasImplicitStopAtLineEnd(mode))
                {
                    if (allowVirtualSpace)
                    {
                        return visualColumn - 1;
                    }
                    return VisualLength;
                }

                for (i = elements.Count - 1; i >= 0; i--)
                {
                    if (elements[i].VisualColumn < visualColumn)
                    {
                        break;
                    }
                }

                for (; i >= 0; i--)
                {
                    int pos = elements[i].GetNextCaretPosition(
                        Math.Min(visualColumn, elements[i].VisualColumn + elements[i].VisualLength + 1),
                        direction, mode);
                    if (pos >= 0)
                    {
                        return pos;
                    }
                }

                if (visualColumn > 0 && !elements[0].HandlesLineBorders && HasImplicitStopAtLineStart(mode))
                {
                    return 0;
                }
            }
            else
            {
                if (visualColumn < 0 && !elements[0].HandlesLineBorders && HasImplicitStopAtLineStart(mode))
                {
                    return 0;
                }

                for (i = 0; i < elements.Count; i++)
                {
                    if (elements[i].VisualColumn + elements[i].VisualLength > visualColumn)
                    {
                        break;
                    }
                }

                for (; i < elements.Count; i++)
                {
                    int pos = elements[i].GetNextCaretPosition(
                        Math.Max(visualColumn, elements[i].VisualColumn - 1),
                        direction, mode);
                    if (pos >= 0)
                    {
                        return pos;
                    }
                }

                if ((allowVirtualSpace || !elements[elements.Count - 1].HandlesLineBorders) && HasImplicitStopAtLineEnd(mode))
                {
                    if (visualColumn < VisualLength)
                    {
                        return VisualLength;
                    }
                    if (allowVirtualSpace)
                    {
                        return visualColumn + 1;
                    }
                }
            }

            return -1;
        }

        private static bool HasStopsInVirtualSpace(CaretPositioningMode mode)
        {
            return mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint;
        }

        private static bool HasImplicitStopAtLineStart(CaretPositioningMode mode)
        {
            return mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "mode",
            Justification = "make method consistent with HasImplicitStopAtLineStart; might depend on mode in the future")]
        private static bool HasImplicitStopAtLineEnd(CaretPositioningMode mode)
        {
            return true;
        }

        internal VisualLineDrawingVisual Render()
        {
            Debug.Assert(phase == LifetimePhase.Live);
            if (visual == null)
            {
                visual = new VisualLineDrawingVisual(this);
            }
            return visual;
        }

        #endregion
    }
}