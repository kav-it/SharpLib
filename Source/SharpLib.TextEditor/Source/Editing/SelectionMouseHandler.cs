using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#endif

namespace ICSharpCode.AvalonEdit.Editing
{
    internal sealed class SelectionMouseHandler : ITextAreaInputHandler
    {
        #region Перечисления

        private enum SelectionMode
        {
            None,

            PossibleDragStart,

            Drag,

            Normal,

            WholeWord,

            WholeLine,

            Rectangular
        }

        #endregion

        #region Поля

        private readonly TextArea textArea;

        private object currentDragDescriptor;

        private bool enableTextDragDrop;

        private SelectionMode mode;

        private Point possibleDragStartMousePos;

        private AnchorSegment startWord;

        #endregion

        #region Свойства

        public TextArea TextArea
        {
            get { return textArea; }
        }

        #endregion

        #region Конструктор

        public SelectionMouseHandler(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            this.textArea = textArea;
        }

        #endregion

        #region Методы

        public void Attach()
        {
            textArea.MouseLeftButtonDown += textArea_MouseLeftButtonDown;
            textArea.MouseMove += textArea_MouseMove;
            textArea.MouseLeftButtonUp += textArea_MouseLeftButtonUp;
            textArea.QueryCursor += textArea_QueryCursor;
            textArea.OptionChanged += textArea_OptionChanged;

            enableTextDragDrop = textArea.Options.EnableTextDragDrop;
            if (enableTextDragDrop)
            {
                AttachDragDrop();
            }
        }

        public void Detach()
        {
            mode = SelectionMode.None;
            textArea.MouseLeftButtonDown -= textArea_MouseLeftButtonDown;
            textArea.MouseMove -= textArea_MouseMove;
            textArea.MouseLeftButtonUp -= textArea_MouseLeftButtonUp;
            textArea.QueryCursor -= textArea_QueryCursor;
            textArea.OptionChanged -= textArea_OptionChanged;
            if (enableTextDragDrop)
            {
                DetachDragDrop();
            }
        }

        private void AttachDragDrop()
        {
            textArea.AllowDrop = true;
            textArea.GiveFeedback += textArea_GiveFeedback;
            textArea.QueryContinueDrag += textArea_QueryContinueDrag;
            textArea.DragEnter += textArea_DragEnter;
            textArea.DragOver += textArea_DragOver;
            textArea.DragLeave += textArea_DragLeave;
            textArea.Drop += textArea_Drop;
        }

        private void DetachDragDrop()
        {
            textArea.AllowDrop = false;
            textArea.GiveFeedback -= textArea_GiveFeedback;
            textArea.QueryContinueDrag -= textArea_QueryContinueDrag;
            textArea.DragEnter -= textArea_DragEnter;
            textArea.DragOver -= textArea_DragOver;
            textArea.DragLeave -= textArea_DragLeave;
            textArea.Drop -= textArea_Drop;
        }

        private void textArea_OptionChanged(object sender, PropertyChangedEventArgs e)
        {
            bool newEnableTextDragDrop = textArea.Options.EnableTextDragDrop;
            if (newEnableTextDragDrop != enableTextDragDrop)
            {
                enableTextDragDrop = newEnableTextDragDrop;
                if (newEnableTextDragDrop)
                {
                    AttachDragDrop();
                }
                else
                {
                    DetachDragDrop();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void textArea_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = GetEffect(e);
                textArea.Caret.Show();
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void textArea_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = GetEffect(e);
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }

        private DragDropEffects GetEffect(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.UnicodeText, true))
            {
                e.Handled = true;
                int visualColumn;
                bool isAtEndOfLine;
                int offset = GetOffsetFromMousePosition(e.GetPosition(textArea.TextView), out visualColumn, out isAtEndOfLine);
                if (offset >= 0)
                {
                    textArea.Caret.Position = new TextViewPosition(textArea.Document.GetLocation(offset), visualColumn)
                    {
                        IsAtEndOfLine = isAtEndOfLine
                    };
                    textArea.Caret.DesiredXPos = double.NaN;
                    if (textArea.ReadOnlySectionProvider.CanInsert(offset))
                    {
                        if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move
                            && (e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey)
                        {
                            return DragDropEffects.Move;
                        }
                        return e.AllowedEffects & DragDropEffects.Copy;
                    }
                }
            }
            return DragDropEffects.None;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void textArea_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                e.Handled = true;
                if (!textArea.IsKeyboardFocusWithin)
                {
                    textArea.Caret.Hide();
                }
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void textArea_Drop(object sender, DragEventArgs e)
        {
            try
            {
                var effect = GetEffect(e);
                e.Effects = effect;
                if (effect != DragDropEffects.None)
                {
                    string text = e.Data.GetData(DataFormats.UnicodeText, true) as string;
                    if (text != null)
                    {
                        int start = textArea.Caret.Offset;
                        if (mode == SelectionMode.Drag && textArea.Selection.Contains(start))
                        {
                            Debug.WriteLine("Drop: did not drop: drop target is inside selection");
                            e.Effects = DragDropEffects.None;
                        }
                        else
                        {
                            Debug.WriteLine("Drop: insert at " + start);

                            bool rectangular = e.Data.GetDataPresent(RectangleSelection.RectangularSelectionDataType);

                            string newLine = TextUtilities.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line);
                            text = TextUtilities.NormalizeNewLines(text, newLine);

                            string pasteFormat;

                            if (rectangular)
                            {
                                pasteFormat = RectangleSelection.RectangularSelectionDataType;
                            }
                            else
                            {
                                pasteFormat = DataFormats.UnicodeText;
                            }

                            var pastingEventArgs = new DataObjectPastingEventArgs(e.Data, true, pasteFormat);
                            textArea.RaiseEvent(pastingEventArgs);
                            if (pastingEventArgs.CommandCancelled)
                            {
                                return;
                            }

                            rectangular = pastingEventArgs.FormatToApply == RectangleSelection.RectangularSelectionDataType;

                            textArea.Document.UndoStack.StartUndoGroup(currentDragDescriptor);
                            try
                            {
                                if (rectangular && RectangleSelection.PerformRectangularPaste(textArea, textArea.Caret.Position, text, true))
                                {
                                }
                                else
                                {
                                    textArea.Document.Insert(start, text);
                                    textArea.Selection = Selection.Create(textArea, start, start + text.Length);
                                }
                            }
                            finally
                            {
                                textArea.Document.UndoStack.EndUndoGroup();
                            }
                        }
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }

        private void OnDragException(Exception ex)
        {
            textArea.Dispatcher.BeginInvoke(
                DispatcherPriority.Send,
                new Action(delegate { throw new DragDropException("Exception during drag'n'drop", ex); }));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void textArea_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            try
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void textArea_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            try
            {
                if (e.EscapePressed)
                {
                    e.Action = DragAction.Cancel;
                }
                else if ((e.KeyStates & DragDropKeyStates.LeftMouseButton) != DragDropKeyStates.LeftMouseButton)
                {
                    e.Action = DragAction.Drop;
                }
                else
                {
                    e.Action = DragAction.Continue;
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                OnDragException(ex);
            }
        }

        private void StartDrag()
        {
            mode = SelectionMode.Drag;

            textArea.ReleaseMouseCapture();

            var dataObject = textArea.Selection.CreateDataObject(textArea);

            var allowedEffects = DragDropEffects.All;
            var deleteOnMove = textArea.Selection.Segments.Select(s => new AnchorSegment(textArea.Document, s)).ToList();
            foreach (ISegment s in deleteOnMove)
            {
                var result = textArea.GetDeletableSegments(s);
                if (result.Length != 1 || result[0].Offset != s.Offset || result[0].EndOffset != s.EndOffset)
                {
                    allowedEffects &= ~DragDropEffects.Move;
                }
            }

            var copyingEventArgs = new DataObjectCopyingEventArgs(dataObject, true);
            textArea.RaiseEvent(copyingEventArgs);
            if (copyingEventArgs.CommandCancelled)
            {
                return;
            }

            var dragDescriptor = new object();
            currentDragDescriptor = dragDescriptor;

            DragDropEffects resultEffect;
            using (textArea.AllowCaretOutsideSelection())
            {
                var oldCaretPosition = textArea.Caret.Position;
                try
                {
                    Debug.WriteLine("DoDragDrop with allowedEffects=" + allowedEffects);
                    resultEffect = DragDrop.DoDragDrop(textArea, dataObject, allowedEffects);
                    Debug.WriteLine("DoDragDrop done, resultEffect=" + resultEffect);
                }
                catch (COMException ex)
                {
                    Debug.WriteLine("DoDragDrop failed: " + ex);
                    return;
                }
                if (resultEffect == DragDropEffects.None)
                {
                    textArea.Caret.Position = oldCaretPosition;
                }
            }

            currentDragDescriptor = null;

            if (deleteOnMove != null && resultEffect == DragDropEffects.Move && (allowedEffects & DragDropEffects.Move) == DragDropEffects.Move)
            {
                bool draggedInsideSingleDocument = (dragDescriptor == textArea.Document.UndoStack.LastGroupDescriptor);
                if (draggedInsideSingleDocument)
                {
                    textArea.Document.UndoStack.StartContinuedUndoGroup(null);
                }
                textArea.Document.BeginUpdate();
                try
                {
                    foreach (ISegment s in deleteOnMove)
                    {
                        textArea.Document.Remove(s.Offset, s.Length);
                    }
                }
                finally
                {
                    textArea.Document.EndUpdate();
                    if (draggedInsideSingleDocument)
                    {
                        textArea.Document.UndoStack.EndUndoGroup();
                    }
                }
            }
        }

        private void textArea_QueryCursor(object sender, QueryCursorEventArgs e)
        {
            if (!e.Handled)
            {
                if (mode != SelectionMode.None || !enableTextDragDrop)
                {
                    e.Cursor = Cursors.IBeam;
                    e.Handled = true;
                }
                else if (textArea.TextView.VisualLinesValid)
                {
                    var p = e.GetPosition(textArea.TextView);
                    if (p.X >= 0 && p.Y >= 0 && p.X <= textArea.TextView.ActualWidth && p.Y <= textArea.TextView.ActualHeight)
                    {
                        int visualColumn;
                        bool isAtEndOfLine;
                        int offset = GetOffsetFromMousePosition(e, out visualColumn, out isAtEndOfLine);
                        if (textArea.Selection.Contains(offset))
                        {
                            e.Cursor = Cursors.Arrow;
                        }
                        else
                        {
                            e.Cursor = Cursors.IBeam;
                        }
                        e.Handled = true;
                    }
                }
            }
        }

        private void textArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mode = SelectionMode.None;
            if (!e.Handled && e.ChangedButton == MouseButton.Left)
            {
                var modifiers = Keyboard.Modifiers;
                bool shift = (modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
                if (enableTextDragDrop && e.ClickCount == 1 && !shift)
                {
                    int visualColumn;
                    bool isAtEndOfLine;
                    int offset = GetOffsetFromMousePosition(e, out visualColumn, out isAtEndOfLine);
                    if (textArea.Selection.Contains(offset))
                    {
                        if (textArea.CaptureMouse())
                        {
                            mode = SelectionMode.PossibleDragStart;
                            possibleDragStartMousePos = e.GetPosition(textArea);
                        }
                        e.Handled = true;
                        return;
                    }
                }

                var oldPosition = textArea.Caret.Position;
                SetCaretOffsetToMousePosition(e);

                if (!shift)
                {
                    textArea.ClearSelection();
                }
                if (textArea.CaptureMouse())
                {
                    if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && textArea.Options.EnableRectangularSelection)
                    {
                        mode = SelectionMode.Rectangular;
                        if (shift && textArea.Selection is RectangleSelection)
                        {
                            textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(oldPosition, textArea.Caret.Position);
                        }
                    }
                    else if (e.ClickCount == 1 && ((modifiers & ModifierKeys.Control) == 0))
                    {
                        mode = SelectionMode.Normal;
                        if (shift && !(textArea.Selection is RectangleSelection))
                        {
                            textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(oldPosition, textArea.Caret.Position);
                        }
                    }
                    else
                    {
                        SimpleSegment startWord;
                        if (e.ClickCount == 3)
                        {
                            mode = SelectionMode.WholeLine;
                            startWord = GetLineAtMousePosition(e);
                        }
                        else
                        {
                            mode = SelectionMode.WholeWord;
                            startWord = GetWordAtMousePosition(e);
                        }
                        if (startWord == SimpleSegment.Invalid)
                        {
                            mode = SelectionMode.None;
                            textArea.ReleaseMouseCapture();
                            return;
                        }
                        if (shift && !textArea.Selection.IsEmpty)
                        {
                            if (startWord.Offset < textArea.Selection.SurroundingSegment.Offset)
                            {
                                textArea.Selection = textArea.Selection.SetEndpoint(new TextViewPosition(textArea.Document.GetLocation(startWord.Offset)));
                            }
                            else if (startWord.EndOffset > textArea.Selection.SurroundingSegment.EndOffset)
                            {
                                textArea.Selection = textArea.Selection.SetEndpoint(new TextViewPosition(textArea.Document.GetLocation(startWord.EndOffset)));
                            }
                            this.startWord = new AnchorSegment(textArea.Document, textArea.Selection.SurroundingSegment);
                        }
                        else
                        {
                            textArea.Selection = Selection.Create(textArea, startWord.Offset, startWord.EndOffset);
                            this.startWord = new AnchorSegment(textArea.Document, startWord.Offset, startWord.Length);
                        }
                    }
                }
            }
            e.Handled = true;
        }

        private SimpleSegment GetWordAtMousePosition(MouseEventArgs e)
        {
            var textView = textArea.TextView;
            if (textView == null)
            {
                return SimpleSegment.Invalid;
            }
            var pos = e.GetPosition(textView);
            if (pos.Y < 0)
            {
                pos.Y = 0;
            }
            if (pos.Y > textView.ActualHeight)
            {
                pos.Y = textView.ActualHeight;
            }
            pos += textView.ScrollOffset;
            var line = textView.GetVisualLineFromVisualTop(pos.Y);
            if (line != null)
            {
                int visualColumn = line.GetVisualColumn(pos, textArea.Selection.EnableVirtualSpace);
                int wordStartVC = line.GetNextCaretPosition(visualColumn + 1, LogicalDirection.Backward, CaretPositioningMode.WordStartOrSymbol, textArea.Selection.EnableVirtualSpace);
                if (wordStartVC == -1)
                {
                    wordStartVC = 0;
                }
                int wordEndVC = line.GetNextCaretPosition(wordStartVC, LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol, textArea.Selection.EnableVirtualSpace);
                if (wordEndVC == -1)
                {
                    wordEndVC = line.VisualLength;
                }
                int relOffset = line.FirstDocumentLine.Offset;
                int wordStartOffset = line.GetRelativeOffset(wordStartVC) + relOffset;
                int wordEndOffset = line.GetRelativeOffset(wordEndVC) + relOffset;
                return new SimpleSegment(wordStartOffset, wordEndOffset - wordStartOffset);
            }
            return SimpleSegment.Invalid;
        }

        private SimpleSegment GetLineAtMousePosition(MouseEventArgs e)
        {
            var textView = textArea.TextView;
            if (textView == null)
            {
                return SimpleSegment.Invalid;
            }
            var pos = e.GetPosition(textView);
            if (pos.Y < 0)
            {
                pos.Y = 0;
            }
            if (pos.Y > textView.ActualHeight)
            {
                pos.Y = textView.ActualHeight;
            }
            pos += textView.ScrollOffset;
            var line = textView.GetVisualLineFromVisualTop(pos.Y);
            if (line != null)
            {
                return new SimpleSegment(line.StartOffset, line.LastDocumentLine.EndOffset - line.StartOffset);
            }
            return SimpleSegment.Invalid;
        }

        private int GetOffsetFromMousePosition(MouseEventArgs e, out int visualColumn, out bool isAtEndOfLine)
        {
            return GetOffsetFromMousePosition(e.GetPosition(textArea.TextView), out visualColumn, out isAtEndOfLine);
        }

        private int GetOffsetFromMousePosition(Point positionRelativeToTextView, out int visualColumn, out bool isAtEndOfLine)
        {
            visualColumn = 0;
            var textView = textArea.TextView;
            var pos = positionRelativeToTextView;
            if (pos.Y < 0)
            {
                pos.Y = 0;
            }
            if (pos.Y > textView.ActualHeight)
            {
                pos.Y = textView.ActualHeight;
            }
            pos += textView.ScrollOffset;
            if (pos.Y > textView.DocumentHeight)
            {
                pos.Y = textView.DocumentHeight - ExtensionMethods.EPSILON;
            }
            var line = textView.GetVisualLineFromVisualTop(pos.Y);
            if (line != null)
            {
                visualColumn = line.GetVisualColumn(pos, textArea.Selection.EnableVirtualSpace, out isAtEndOfLine);
                return line.GetRelativeOffset(visualColumn) + line.FirstDocumentLine.Offset;
            }
            isAtEndOfLine = false;
            return -1;
        }

        private int GetOffsetFromMousePositionFirstTextLineOnly(Point positionRelativeToTextView, out int visualColumn)
        {
            visualColumn = 0;
            var textView = textArea.TextView;
            var pos = positionRelativeToTextView;
            if (pos.Y < 0)
            {
                pos.Y = 0;
            }
            if (pos.Y > textView.ActualHeight)
            {
                pos.Y = textView.ActualHeight;
            }
            pos += textView.ScrollOffset;
            if (pos.Y > textView.DocumentHeight)
            {
                pos.Y = textView.DocumentHeight - ExtensionMethods.EPSILON;
            }
            var line = textView.GetVisualLineFromVisualTop(pos.Y);
            if (line != null)
            {
                visualColumn = line.GetVisualColumn(line.TextLines.First(), pos.X, textArea.Selection.EnableVirtualSpace);
                return line.GetRelativeOffset(visualColumn) + line.FirstDocumentLine.Offset;
            }
            return -1;
        }

        private void textArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            if (mode == SelectionMode.Normal || mode == SelectionMode.WholeWord || mode == SelectionMode.WholeLine || mode == SelectionMode.Rectangular)
            {
                e.Handled = true;
                if (textArea.TextView.VisualLinesValid)
                {
                    ExtendSelectionToMouse(e);
                }
            }
            else if (mode == SelectionMode.PossibleDragStart)
            {
                e.Handled = true;
                var mouseMovement = e.GetPosition(textArea) - possibleDragStartMousePos;
                if (Math.Abs(mouseMovement.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(mouseMovement.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag();
                }
            }
        }

        private void SetCaretOffsetToMousePosition(MouseEventArgs e)
        {
            SetCaretOffsetToMousePosition(e, null);
        }

        private void SetCaretOffsetToMousePosition(MouseEventArgs e, ISegment allowedSegment)
        {
            int visualColumn;
            bool isAtEndOfLine;
            int offset;
            if (mode == SelectionMode.Rectangular)
            {
                offset = GetOffsetFromMousePositionFirstTextLineOnly(e.GetPosition(textArea.TextView), out visualColumn);
                isAtEndOfLine = true;
            }
            else
            {
                offset = GetOffsetFromMousePosition(e, out visualColumn, out isAtEndOfLine);
            }
            if (allowedSegment != null)
            {
                offset = offset.CoerceValue(allowedSegment.Offset, allowedSegment.EndOffset);
            }
            if (offset >= 0)
            {
                textArea.Caret.Position = new TextViewPosition(textArea.Document.GetLocation(offset), visualColumn)
                {
                    IsAtEndOfLine = isAtEndOfLine
                };
                textArea.Caret.DesiredXPos = double.NaN;
            }
        }

        private void ExtendSelectionToMouse(MouseEventArgs e)
        {
            var oldPosition = textArea.Caret.Position;
            if (mode == SelectionMode.Normal || mode == SelectionMode.Rectangular)
            {
                SetCaretOffsetToMousePosition(e);
                if (mode == SelectionMode.Normal && textArea.Selection is RectangleSelection)
                {
                    textArea.Selection = new SimpleSelection(textArea, oldPosition, textArea.Caret.Position);
                }
                else if (mode == SelectionMode.Rectangular && !(textArea.Selection is RectangleSelection))
                {
                    textArea.Selection = new RectangleSelection(textArea, oldPosition, textArea.Caret.Position);
                }
                else
                {
                    textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(oldPosition, textArea.Caret.Position);
                }
            }
            else if (mode == SelectionMode.WholeWord || mode == SelectionMode.WholeLine)
            {
                var newWord = (mode == SelectionMode.WholeLine) ? GetLineAtMousePosition(e) : GetWordAtMousePosition(e);
                if (newWord != SimpleSegment.Invalid)
                {
                    textArea.Selection = Selection.Create(textArea,
                        Math.Min(newWord.Offset, startWord.Offset),
                        Math.Max(newWord.EndOffset, startWord.EndOffset));

                    SetCaretOffsetToMousePosition(e, textArea.Selection.SurroundingSegment);
                }
            }
            textArea.Caret.BringCaretToView(5.0);
        }

        private void textArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mode == SelectionMode.None || e.Handled)
            {
                return;
            }
            e.Handled = true;
            if (mode == SelectionMode.PossibleDragStart)
            {
                SetCaretOffsetToMousePosition(e);
                textArea.ClearSelection();
            }
            else if (mode == SelectionMode.Normal || mode == SelectionMode.WholeWord || mode == SelectionMode.WholeLine || mode == SelectionMode.Rectangular)
            {
                ExtendSelectionToMouse(e);
            }
            mode = SelectionMode.None;
            textArea.ReleaseMouseCapture();
        }

        #endregion
    }
}