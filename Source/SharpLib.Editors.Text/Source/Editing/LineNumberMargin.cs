using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using SharpLib.Notepad.Document;
using SharpLib.Notepad.Rendering;
using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Editing
{
    public class LineNumberMargin : AbstractMargin, IWeakEventListener
    {
        #region Поля

        protected double emSize;

        protected int maxLineNumberLength = 1;

        private bool selecting;

        private AnchorSegment selectionStart;

        private TextArea textArea;

        protected Typeface typeface;

        #endregion

        #region Конструктор

        static LineNumberMargin()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineNumberMargin),
                new FrameworkPropertyMetadata(typeof(LineNumberMargin)));
        }

        #endregion

        #region Методы

        protected override Size MeasureOverride(Size availableSize)
        {
            typeface = this.CreateTypeface();
            emSize = (double)GetValue(TextBlock.FontSizeProperty);

            var text = TextFormatterFactory.CreateFormattedText(
                this,
                new string('9', maxLineNumberLength),
                typeface,
                emSize,
                (Brush)GetValue(Control.ForegroundProperty)
                );
            return new Size(text.Width, 0);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var textView = TextView;
            var renderSize = RenderSize;
            if (textView != null && textView.VisualLinesValid)
            {
                var foreground = (Brush)GetValue(Control.ForegroundProperty);
                foreach (VisualLine line in textView.VisualLines)
                {
                    int lineNumber = line.FirstDocumentLine.LineNumber;
                    var text = TextFormatterFactory.CreateFormattedText(
                        this,
                        lineNumber.ToString(CultureInfo.CurrentCulture),
                        typeface, emSize, foreground
                        );
                    double y = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextTop);
                    drawingContext.DrawText(text, new Point(renderSize.Width - text.Width, y - textView.VerticalOffset));
                }
            }
        }

        protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
        {
            if (oldTextView != null)
            {
                oldTextView.VisualLinesChanged -= TextViewVisualLinesChanged;
            }
            base.OnTextViewChanged(oldTextView, newTextView);
            if (newTextView != null)
            {
                newTextView.VisualLinesChanged += TextViewVisualLinesChanged;

                textArea = newTextView.GetService(typeof(TextArea)) as TextArea;
            }
            else
            {
                textArea = null;
            }
            InvalidateVisual();
        }

        protected override void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
        {
            if (oldDocument != null)
            {
                PropertyChangedEventManager.RemoveListener(oldDocument, this, "LineCount");
            }
            base.OnDocumentChanged(oldDocument, newDocument);
            if (newDocument != null)
            {
                PropertyChangedEventManager.AddListener(newDocument, this, "LineCount");
            }
            OnDocumentLineCountChanged();
        }

        protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedEventManager))
            {
                OnDocumentLineCountChanged();
                return true;
            }
            return false;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return ReceiveWeakEvent(managerType, sender, e);
        }

        private void OnDocumentLineCountChanged()
        {
            int documentLineCount = Document != null ? Document.LineCount : 1;
            int newLength = documentLineCount.ToString(CultureInfo.CurrentCulture).Length;

            if (newLength < 2)
            {
                newLength = 2;
            }

            if (newLength != maxLineNumberLength)
            {
                maxLineNumberLength = newLength;
                InvalidateMeasure();
            }
        }

        private void TextViewVisualLinesChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!e.Handled && TextView != null && textArea != null)
            {
                e.Handled = true;
                textArea.Focus();

                var currentSeg = GetTextLineSegment(e);
                if (currentSeg == SimpleSegment.Invalid)
                {
                    return;
                }
                textArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
                if (CaptureMouse())
                {
                    selecting = true;
                    selectionStart = new AnchorSegment(Document, currentSeg.Offset, currentSeg.Length);
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        var simpleSelection = textArea.Selection as SimpleSelection;
                        if (simpleSelection != null)
                        {
                            selectionStart = new AnchorSegment(Document, simpleSelection.SurroundingSegment);
                        }
                    }
                    textArea.Selection = Selection.Create(textArea, selectionStart);
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        ExtendSelection(currentSeg);
                    }
                    textArea.Caret.BringCaretToView(5.0);
                }
            }
        }

        private SimpleSegment GetTextLineSegment(MouseEventArgs e)
        {
            var pos = e.GetPosition(TextView);
            pos.X = 0;
            pos.Y = pos.Y.CoerceValue(0, TextView.ActualHeight);
            pos.Y += TextView.VerticalOffset;
            var vl = TextView.GetVisualLineFromVisualTop(pos.Y);
            if (vl == null)
            {
                return SimpleSegment.Invalid;
            }
            var tl = vl.GetTextLineByVisualYPosition(pos.Y);
            int visualStartColumn = vl.GetTextLineVisualStartColumn(tl);
            int visualEndColumn = visualStartColumn + tl.Length;
            int relStart = vl.FirstDocumentLine.Offset;
            int startOffset = vl.GetRelativeOffset(visualStartColumn) + relStart;
            int endOffset = vl.GetRelativeOffset(visualEndColumn) + relStart;
            if (endOffset == vl.LastDocumentLine.Offset + vl.LastDocumentLine.Length)
            {
                endOffset += vl.LastDocumentLine.DelimiterLength;
            }
            return new SimpleSegment(startOffset, endOffset - startOffset);
        }

        private void ExtendSelection(SimpleSegment currentSeg)
        {
            if (currentSeg.Offset < selectionStart.Offset)
            {
                textArea.Caret.Offset = currentSeg.Offset;
                textArea.Selection = Selection.Create(textArea, currentSeg.Offset, selectionStart.Offset + selectionStart.Length);
            }
            else
            {
                textArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
                textArea.Selection = Selection.Create(textArea, selectionStart.Offset, currentSeg.Offset + currentSeg.Length);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (selecting && textArea != null && TextView != null)
            {
                e.Handled = true;
                var currentSeg = GetTextLineSegment(e);
                if (currentSeg == SimpleSegment.Invalid)
                {
                    return;
                }
                ExtendSelection(currentSeg);
                textArea.Caret.BringCaretToView(5.0);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (selecting)
            {
                selecting = false;
                selectionStart = null;
                ReleaseMouseCapture();
                e.Handled = true;
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        #endregion
    }
}