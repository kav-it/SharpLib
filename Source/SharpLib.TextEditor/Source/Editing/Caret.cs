using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

#if NREFACTORY
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
#endif

namespace ICSharpCode.AvalonEdit.Editing
{
    public sealed class Caret
    {
        #region Константы

        internal const double MinimumDistanceToViewBorder = 30;

        #endregion

        #region Поля

        private readonly CaretLayer caretAdorner;

        private readonly TextArea textArea;

        private readonly TextView textView;

        private double desiredXPos = double.NaN;

        private bool hasWin32Caret;

        private bool isInVirtualSpace;

        private TextViewPosition position;

        private bool raisePositionChangedOnUpdateFinished;

        private bool showScheduled;

        private int storedCaretOffset;

        private bool visible;

        private bool visualColumnValid;

        #endregion

        #region Свойства

        public TextViewPosition Position
        {
            get
            {
                ValidateVisualColumn();
                return position;
            }
            set
            {
                if (position != value)
                {
                    position = value;

                    storedCaretOffset = -1;

                    ValidatePosition();
                    InvalidateVisualColumn();
                    RaisePositionChanged();
                    Log("Caret position changed to " + value);
                    if (visible)
                    {
                        Show();
                    }
                }
            }
        }

        internal TextViewPosition NonValidatedPosition
        {
            get { return position; }
        }

        public TextLocation Location
        {
            get { return position.Location; }
            set { Position = new TextViewPosition(value); }
        }

        public int Line
        {
            get { return position.Line; }
            set { Position = new TextViewPosition(value, position.Column); }
        }

        public int Column
        {
            get { return position.Column; }
            set { Position = new TextViewPosition(position.Line, value); }
        }

        public int VisualColumn
        {
            get
            {
                ValidateVisualColumn();
                return position.VisualColumn;
            }
            set { Position = new TextViewPosition(position.Line, position.Column, value); }
        }

        public bool IsInVirtualSpace
        {
            get
            {
                ValidateVisualColumn();
                return isInVirtualSpace;
            }
        }

        public int Offset
        {
            get
            {
                var document = textArea.Document;
                if (document == null)
                {
                    return 0;
                }
                return document.GetOffset(position.Location);
            }
            set
            {
                var document = textArea.Document;
                if (document != null)
                {
                    Position = new TextViewPosition(document.GetLocation(value));
                    DesiredXPos = double.NaN;
                }
            }
        }

        public double DesiredXPos
        {
            get { return desiredXPos; }
            set { desiredXPos = value; }
        }

        public Brush CaretBrush
        {
            get { return caretAdorner.CaretBrush; }
            set { caretAdorner.CaretBrush = value; }
        }

        #endregion

        #region События

        public event EventHandler PositionChanged;

        #endregion

        #region Конструктор

        internal Caret(TextArea textArea)
        {
            this.textArea = textArea;
            textView = textArea.TextView;
            position = new TextViewPosition(1, 1, 0);

            caretAdorner = new CaretLayer(textArea);
            textView.InsertLayer(caretAdorner, KnownLayer.Caret, LayerInsertionPosition.Replace);
            textView.VisualLinesChanged += TextView_VisualLinesChanged;
            textView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;
        }

        #endregion

        #region Методы

        internal void UpdateIfVisible()
        {
            if (visible)
            {
                Show();
            }
        }

        private void TextView_VisualLinesChanged(object sender, EventArgs e)
        {
            if (visible)
            {
                Show();
            }

            InvalidateVisualColumn();
        }

        private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            if (caretAdorner != null)
            {
                caretAdorner.InvalidateVisual();
            }
        }

        internal void OnDocumentChanging()
        {
            storedCaretOffset = Offset;
            InvalidateVisualColumn();
        }

        internal void OnDocumentChanged(DocumentChangeEventArgs e)
        {
            InvalidateVisualColumn();
            if (storedCaretOffset >= 0)
            {
                AnchorMovementType caretMovementType;
                if (!textArea.Selection.IsEmpty && storedCaretOffset == textArea.Selection.SurroundingSegment.EndOffset)
                {
                    caretMovementType = AnchorMovementType.BeforeInsertion;
                }
                else
                {
                    caretMovementType = AnchorMovementType.Default;
                }
                int newCaretOffset = e.GetNewOffset(storedCaretOffset, caretMovementType);
                var document = textArea.Document;
                if (document != null)
                {
                    Position = new TextViewPosition(document.GetLocation(newCaretOffset), position.VisualColumn);
                }
            }
            storedCaretOffset = -1;
        }

        private void ValidatePosition()
        {
            if (position.Line < 1)
            {
                position.Line = 1;
            }
            if (position.Column < 1)
            {
                position.Column = 1;
            }
            if (position.VisualColumn < -1)
            {
                position.VisualColumn = -1;
            }
            var document = textArea.Document;
            if (document != null)
            {
                if (position.Line > document.LineCount)
                {
                    position.Line = document.LineCount;
                    position.Column = document.GetLineByNumber(position.Line).Length + 1;
                    position.VisualColumn = -1;
                }
                else
                {
                    var line = document.GetLineByNumber(position.Line);
                    if (position.Column > line.Length + 1)
                    {
                        position.Column = line.Length + 1;
                        position.VisualColumn = -1;
                    }
                }
            }
        }

        private void RaisePositionChanged()
        {
            if (textArea.Document != null && textArea.Document.IsInUpdate)
            {
                raisePositionChangedOnUpdateFinished = true;
            }
            else
            {
                if (PositionChanged != null)
                {
                    PositionChanged(this, EventArgs.Empty);
                }
            }
        }

        internal void OnDocumentUpdateFinished()
        {
            if (raisePositionChangedOnUpdateFinished)
            {
                if (PositionChanged != null)
                {
                    PositionChanged(this, EventArgs.Empty);
                }
            }
        }

        private void ValidateVisualColumn()
        {
            if (!visualColumnValid)
            {
                var document = textArea.Document;
                if (document != null)
                {
                    Debug.WriteLine("Explicit validation of caret column");
                    var documentLine = document.GetLineByNumber(position.Line);
                    RevalidateVisualColumn(textView.GetOrConstructVisualLine(documentLine));
                }
            }
        }

        private void InvalidateVisualColumn()
        {
            visualColumnValid = false;
        }

        private void RevalidateVisualColumn(VisualLine visualLine)
        {
            if (visualLine == null)
            {
                throw new ArgumentNullException("visualLine");
            }

            visualColumnValid = true;

            int caretOffset = textView.Document.GetOffset(position.Location);
            int firstDocumentLineOffset = visualLine.FirstDocumentLine.Offset;
            position.VisualColumn = visualLine.ValidateVisualColumn(position, textArea.Selection.EnableVirtualSpace);

            int newVisualColumnForwards = visualLine.GetNextCaretPosition(position.VisualColumn - 1, LogicalDirection.Forward, CaretPositioningMode.Normal, textArea.Selection.EnableVirtualSpace);

            if (newVisualColumnForwards != position.VisualColumn)
            {
                int newVisualColumnBackwards = visualLine.GetNextCaretPosition(position.VisualColumn + 1, LogicalDirection.Backward, CaretPositioningMode.Normal, textArea.Selection.EnableVirtualSpace);

                if (newVisualColumnForwards < 0 && newVisualColumnBackwards < 0)
                {
                    throw ThrowUtil.NoValidCaretPosition();
                }

                int newOffsetForwards;
                if (newVisualColumnForwards >= 0)
                {
                    newOffsetForwards = visualLine.GetRelativeOffset(newVisualColumnForwards) + firstDocumentLineOffset;
                }
                else
                {
                    newOffsetForwards = -1;
                }
                int newOffsetBackwards;
                if (newVisualColumnBackwards >= 0)
                {
                    newOffsetBackwards = visualLine.GetRelativeOffset(newVisualColumnBackwards) + firstDocumentLineOffset;
                }
                else
                {
                    newOffsetBackwards = -1;
                }

                int newVisualColumn, newOffset;

                if (newVisualColumnForwards < 0)
                {
                    newVisualColumn = newVisualColumnBackwards;
                    newOffset = newOffsetBackwards;
                }
                else if (newVisualColumnBackwards < 0)
                {
                    newVisualColumn = newVisualColumnForwards;
                    newOffset = newOffsetForwards;
                }
                else
                {
                    if (Math.Abs(newOffsetBackwards - caretOffset) < Math.Abs(newOffsetForwards - caretOffset))
                    {
                        newVisualColumn = newVisualColumnBackwards;
                        newOffset = newOffsetBackwards;
                    }
                    else
                    {
                        newVisualColumn = newVisualColumnForwards;
                        newOffset = newOffsetForwards;
                    }
                }
                Position = new TextViewPosition(textView.Document.GetLocation(newOffset), newVisualColumn);
            }
            isInVirtualSpace = (position.VisualColumn > visualLine.VisualLength);
        }

        private Rect CalcCaretRectangle(VisualLine visualLine)
        {
            if (!visualColumnValid)
            {
                RevalidateVisualColumn(visualLine);
            }

            var textLine = visualLine.GetTextLine(position.VisualColumn, position.IsAtEndOfLine);
            double xPos = visualLine.GetTextLineVisualXPosition(textLine, position.VisualColumn);
            double lineTop = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextTop);
            double lineBottom = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextBottom);

            return new Rect(xPos,
                lineTop,
                SystemParameters.CaretWidth,
                lineBottom - lineTop);
        }

        private Rect CalcCaretOverstrikeRectangle(VisualLine visualLine)
        {
            if (!visualColumnValid)
            {
                RevalidateVisualColumn(visualLine);
            }

            int currentPos = position.VisualColumn;

            int nextPos = visualLine.GetNextCaretPosition(currentPos, LogicalDirection.Forward, CaretPositioningMode.Normal, true);
            var textLine = visualLine.GetTextLine(currentPos);

            Rect r;
            if (currentPos < visualLine.VisualLength)
            {
                var textBounds = textLine.GetTextBounds(currentPos, nextPos - currentPos)[0];
                r = textBounds.Rectangle;
                r.Y += visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.LineTop);
            }
            else
            {
                double xPos = visualLine.GetTextLineVisualXPosition(textLine, currentPos);
                double xPos2 = visualLine.GetTextLineVisualXPosition(textLine, nextPos);
                double lineTop = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextTop);
                double lineBottom = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.TextBottom);
                r = new Rect(xPos, lineTop, xPos2 - xPos, lineBottom - lineTop);
            }

            if (r.Width < SystemParameters.CaretWidth)
            {
                r.Width = SystemParameters.CaretWidth;
            }
            return r;
        }

        public Rect CalculateCaretRectangle()
        {
            if (textView != null && textView.Document != null)
            {
                var visualLine = textView.GetOrConstructVisualLine(textView.Document.GetLineByNumber(position.Line));
                return textArea.OverstrikeMode ? CalcCaretOverstrikeRectangle(visualLine) : CalcCaretRectangle(visualLine);
            }
            return Rect.Empty;
        }

        public void BringCaretToView()
        {
            BringCaretToView(MinimumDistanceToViewBorder);
        }

        internal void BringCaretToView(double border)
        {
            var caretRectangle = CalculateCaretRectangle();
            if (!caretRectangle.IsEmpty)
            {
                caretRectangle.Inflate(border, border);
                textView.MakeVisible(caretRectangle);
            }
        }

        public void Show()
        {
            Log("Caret.Show()");
            visible = true;
            if (!showScheduled)
            {
                showScheduled = true;
                textArea.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(ShowInternal));
            }
        }

        private void ShowInternal()
        {
            showScheduled = false;

            if (!visible)
            {
                return;
            }

            if (caretAdorner != null && textView != null)
            {
                var visualLine = textView.GetVisualLine(position.Line);
                if (visualLine != null)
                {
                    var caretRect = textArea.OverstrikeMode ? CalcCaretOverstrikeRectangle(visualLine) : CalcCaretRectangle(visualLine);

                    if (!hasWin32Caret)
                    {
                        hasWin32Caret = Win32.CreateCaret(textView, caretRect.Size);
                    }
                    if (hasWin32Caret)
                    {
                        Win32.SetCaretPosition(textView, caretRect.Location - textView.ScrollOffset);
                    }
                    caretAdorner.Show(caretRect);
                    textArea.ime.UpdateCompositionWindow();
                }
                else
                {
                    caretAdorner.Hide();
                }
            }
        }

        public void Hide()
        {
            Log("Caret.Hide()");
            visible = false;
            if (hasWin32Caret)
            {
                Win32.DestroyCaret();
                hasWin32Caret = false;
            }
            if (caretAdorner != null)
            {
                caretAdorner.Hide();
            }
        }

        [Conditional("DEBUG")]
        private static void Log(string text)
        {
        }

        #endregion
    }
}