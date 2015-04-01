﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;

using SharpLib.Notepad.Document;
using SharpLib.Notepad.Rendering;
using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Editing
{
    internal static class CaretNavigationCommandHandler
    {
        #region Поля

        private static readonly List<CommandBinding> CommandBindings = new List<CommandBinding>();

        private static readonly List<InputBinding> InputBindings = new List<InputBinding>();

        #endregion

        #region Конструктор

        static CaretNavigationCommandHandler()
        {
            const ModifierKeys None = ModifierKeys.None;
            const ModifierKeys Ctrl = ModifierKeys.Control;
            const ModifierKeys Shift = ModifierKeys.Shift;
            const ModifierKeys Alt = ModifierKeys.Alt;

            AddBinding(EditingCommands.MoveLeftByCharacter, None, Key.Left, OnMoveCaret(CaretMovementType.CharLeft));
            AddBinding(EditingCommands.SelectLeftByCharacter, Shift, Key.Left, OnMoveCaretExtendSelection(CaretMovementType.CharLeft));
            AddBinding(RectangleSelection.BoxSelectLeftByCharacter, Alt | Shift, Key.Left, OnMoveCaretBoxSelection(CaretMovementType.CharLeft));
            AddBinding(EditingCommands.MoveRightByCharacter, None, Key.Right, OnMoveCaret(CaretMovementType.CharRight));
            AddBinding(EditingCommands.SelectRightByCharacter, Shift, Key.Right, OnMoveCaretExtendSelection(CaretMovementType.CharRight));
            AddBinding(RectangleSelection.BoxSelectRightByCharacter, Alt | Shift, Key.Right, OnMoveCaretBoxSelection(CaretMovementType.CharRight));

            AddBinding(EditingCommands.MoveLeftByWord, Ctrl, Key.Left, OnMoveCaret(CaretMovementType.WordLeft));
            AddBinding(EditingCommands.SelectLeftByWord, Ctrl | Shift, Key.Left, OnMoveCaretExtendSelection(CaretMovementType.WordLeft));
            AddBinding(RectangleSelection.BoxSelectLeftByWord, Ctrl | Alt | Shift, Key.Left, OnMoveCaretBoxSelection(CaretMovementType.WordLeft));
            AddBinding(EditingCommands.MoveRightByWord, Ctrl, Key.Right, OnMoveCaret(CaretMovementType.WordRight));
            AddBinding(EditingCommands.SelectRightByWord, Ctrl | Shift, Key.Right, OnMoveCaretExtendSelection(CaretMovementType.WordRight));
            AddBinding(RectangleSelection.BoxSelectRightByWord, Ctrl | Alt | Shift, Key.Right, OnMoveCaretBoxSelection(CaretMovementType.WordRight));

            AddBinding(EditingCommands.MoveUpByLine, None, Key.Up, OnMoveCaret(CaretMovementType.LineUp));
            AddBinding(EditingCommands.SelectUpByLine, Shift, Key.Up, OnMoveCaretExtendSelection(CaretMovementType.LineUp));
            AddBinding(RectangleSelection.BoxSelectUpByLine, Alt | Shift, Key.Up, OnMoveCaretBoxSelection(CaretMovementType.LineUp));
            AddBinding(EditingCommands.MoveDownByLine, None, Key.Down, OnMoveCaret(CaretMovementType.LineDown));
            AddBinding(EditingCommands.SelectDownByLine, Shift, Key.Down, OnMoveCaretExtendSelection(CaretMovementType.LineDown));
            AddBinding(RectangleSelection.BoxSelectDownByLine, Alt | Shift, Key.Down, OnMoveCaretBoxSelection(CaretMovementType.LineDown));

            AddBinding(EditingCommands.MoveDownByPage, None, Key.PageDown, OnMoveCaret(CaretMovementType.PageDown));
            AddBinding(EditingCommands.SelectDownByPage, Shift, Key.PageDown, OnMoveCaretExtendSelection(CaretMovementType.PageDown));
            AddBinding(EditingCommands.MoveUpByPage, None, Key.PageUp, OnMoveCaret(CaretMovementType.PageUp));
            AddBinding(EditingCommands.SelectUpByPage, Shift, Key.PageUp, OnMoveCaretExtendSelection(CaretMovementType.PageUp));

            AddBinding(EditingCommands.MoveToLineStart, None, Key.Home, OnMoveCaret(CaretMovementType.LineStart));
            AddBinding(EditingCommands.SelectToLineStart, Shift, Key.Home, OnMoveCaretExtendSelection(CaretMovementType.LineStart));
            AddBinding(RectangleSelection.BoxSelectToLineStart, Alt | Shift, Key.Home, OnMoveCaretBoxSelection(CaretMovementType.LineStart));
            AddBinding(EditingCommands.MoveToLineEnd, None, Key.End, OnMoveCaret(CaretMovementType.LineEnd));
            AddBinding(EditingCommands.SelectToLineEnd, Shift, Key.End, OnMoveCaretExtendSelection(CaretMovementType.LineEnd));
            AddBinding(RectangleSelection.BoxSelectToLineEnd, Alt | Shift, Key.End, OnMoveCaretBoxSelection(CaretMovementType.LineEnd));

            AddBinding(EditingCommands.MoveToDocumentStart, Ctrl, Key.Home, OnMoveCaret(CaretMovementType.DocumentStart));
            AddBinding(EditingCommands.SelectToDocumentStart, Ctrl | Shift, Key.Home, OnMoveCaretExtendSelection(CaretMovementType.DocumentStart));
            AddBinding(EditingCommands.MoveToDocumentEnd, Ctrl, Key.End, OnMoveCaret(CaretMovementType.DocumentEnd));
            AddBinding(EditingCommands.SelectToDocumentEnd, Ctrl | Shift, Key.End, OnMoveCaretExtendSelection(CaretMovementType.DocumentEnd));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, OnSelectAll));

            TextAreaDefaultInputHandler.WorkaroundWPFMemoryLeak(InputBindings);
        }

        #endregion

        #region Методы

        public static TextAreaInputHandler Create(TextArea textArea)
        {
            var handler = new TextAreaInputHandler(textArea);
            handler.CommandBindings.AddRange(CommandBindings);
            handler.InputBindings.AddRange(InputBindings);
            return handler;
        }

        private static void AddBinding(ICommand command, ModifierKeys modifiers, Key key, ExecutedRoutedEventHandler handler)
        {
            CommandBindings.Add(new CommandBinding(command, handler));
            InputBindings.Add(TextAreaDefaultInputHandler.CreateFrozenKeyBinding(command, modifiers, key));
        }

        private static void OnSelectAll(object target, ExecutedRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                args.Handled = true;
                textArea.Caret.Offset = textArea.Document.TextLength;
                textArea.Selection = Selection.Create(textArea, 0, textArea.Document.TextLength);
            }
        }

        private static TextArea GetTextArea(object target)
        {
            return target as TextArea;
        }

        private static ExecutedRoutedEventHandler OnMoveCaret(CaretMovementType direction)
        {
            return (target, args) =>
            {
                var textArea = GetTextArea(target);
                if (textArea != null && textArea.Document != null)
                {
                    args.Handled = true;
                    textArea.ClearSelection();
                    MoveCaret(textArea, direction);
                    textArea.Caret.BringCaretToView();
                }
            };
        }

        private static ExecutedRoutedEventHandler OnMoveCaretExtendSelection(CaretMovementType direction)
        {
            return (target, args) =>
            {
                var textArea = GetTextArea(target);
                if (textArea != null && textArea.Document != null)
                {
                    args.Handled = true;
                    var oldPosition = textArea.Caret.Position;
                    MoveCaret(textArea, direction);
                    textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(oldPosition, textArea.Caret.Position);
                    textArea.Caret.BringCaretToView();
                }
            };
        }

        private static ExecutedRoutedEventHandler OnMoveCaretBoxSelection(CaretMovementType direction)
        {
            return (target, args) =>
            {
                var textArea = GetTextArea(target);
                if (textArea != null && textArea.Document != null)
                {
                    args.Handled = true;

                    if (textArea.Options.EnableRectangularSelection && !(textArea.Selection is RectangleSelection))
                    {
                        if (textArea.Selection.IsEmpty)
                        {
                            textArea.Selection = new RectangleSelection(textArea, textArea.Caret.Position, textArea.Caret.Position);
                        }
                        else
                        {
                            textArea.Selection = new RectangleSelection(textArea, textArea.Selection.StartPosition, textArea.Caret.Position);
                        }
                    }

                    var oldPosition = textArea.Caret.Position;
                    MoveCaret(textArea, direction);
                    textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(oldPosition, textArea.Caret.Position);
                    textArea.Caret.BringCaretToView();
                }
            };
        }

        internal static void MoveCaret(TextArea textArea, CaretMovementType direction)
        {
            double desiredXPos = textArea.Caret.DesiredXPos;
            textArea.Caret.Position = GetNewCaretPosition(textArea.TextView, textArea.Caret.Position, direction, textArea.Selection.EnableVirtualSpace, ref desiredXPos);
            textArea.Caret.DesiredXPos = desiredXPos;
        }

        internal static TextViewPosition GetNewCaretPosition(TextView textView, TextViewPosition caretPosition, CaretMovementType direction, bool enableVirtualSpace, ref double desiredXPos)
        {
            switch (direction)
            {
                case CaretMovementType.None:
                    return caretPosition;
                case CaretMovementType.DocumentStart:
                    desiredXPos = double.NaN;
                    return new TextViewPosition(0, 0);
                case CaretMovementType.DocumentEnd:
                    desiredXPos = double.NaN;
                    return new TextViewPosition(textView.Document.GetLocation(textView.Document.TextLength));
            }
            var caretLine = textView.Document.GetLineByNumber(caretPosition.Line);
            var visualLine = textView.GetOrConstructVisualLine(caretLine);
            var textLine = visualLine.GetTextLine(caretPosition.VisualColumn, caretPosition.IsAtEndOfLine);
            switch (direction)
            {
                case CaretMovementType.CharLeft:
                    desiredXPos = double.NaN;
                    return GetPrevCaretPosition(textView, caretPosition, visualLine, CaretPositioningMode.Normal, enableVirtualSpace);
                case CaretMovementType.Backspace:
                    desiredXPos = double.NaN;
                    return GetPrevCaretPosition(textView, caretPosition, visualLine, CaretPositioningMode.EveryCodepoint, enableVirtualSpace);
                case CaretMovementType.CharRight:
                    desiredXPos = double.NaN;
                    return GetNextCaretPosition(textView, caretPosition, visualLine, CaretPositioningMode.Normal, enableVirtualSpace);
                case CaretMovementType.WordLeft:
                    desiredXPos = double.NaN;
                    return GetPrevCaretPosition(textView, caretPosition, visualLine, CaretPositioningMode.WordStart, enableVirtualSpace);
                case CaretMovementType.WordRight:
                    desiredXPos = double.NaN;
                    return GetNextCaretPosition(textView, caretPosition, visualLine, CaretPositioningMode.WordStart, enableVirtualSpace);
                case CaretMovementType.LineUp:
                case CaretMovementType.LineDown:
                case CaretMovementType.PageUp:
                case CaretMovementType.PageDown:
                    return GetUpDownCaretPosition(textView, caretPosition, direction, visualLine, textLine, enableVirtualSpace, ref desiredXPos);
                case CaretMovementType.LineStart:
                    desiredXPos = double.NaN;
                    return GetStartOfLineCaretPosition(caretPosition.VisualColumn, visualLine, textLine, enableVirtualSpace);
                case CaretMovementType.LineEnd:
                    desiredXPos = double.NaN;
                    return GetEndOfLineCaretPosition(visualLine, textLine);
                default:
                    throw new NotSupportedException(direction.ToString());
            }
        }

        private static TextViewPosition GetStartOfLineCaretPosition(int oldVC, VisualLine visualLine, TextLine textLine, bool enableVirtualSpace)
        {
            int newVC = visualLine.GetTextLineVisualStartColumn(textLine);
            if (newVC == 0)
            {
                newVC = visualLine.GetNextCaretPosition(newVC - 1, LogicalDirection.Forward, CaretPositioningMode.WordStart, enableVirtualSpace);
            }
            if (newVC < 0)
            {
                throw ThrowUtil.NoValidCaretPosition();
            }

            if (newVC == oldVC)
            {
                newVC = 0;
            }
            return visualLine.GetTextViewPosition(newVC);
        }

        private static TextViewPosition GetEndOfLineCaretPosition(VisualLine visualLine, TextLine textLine)
        {
            int newVC = visualLine.GetTextLineVisualStartColumn(textLine) + textLine.Length - textLine.TrailingWhitespaceLength;
            var pos = visualLine.GetTextViewPosition(newVC);
            pos.IsAtEndOfLine = true;
            return pos;
        }

        private static TextViewPosition GetNextCaretPosition(TextView textView, TextViewPosition caretPosition, VisualLine visualLine, CaretPositioningMode mode, bool enableVirtualSpace)
        {
            int pos = visualLine.GetNextCaretPosition(caretPosition.VisualColumn, LogicalDirection.Forward, mode, enableVirtualSpace);
            if (pos >= 0)
            {
                return visualLine.GetTextViewPosition(pos);
            }
            var nextDocumentLine = visualLine.LastDocumentLine.NextLine;
            if (nextDocumentLine != null)
            {
                var nextLine = textView.GetOrConstructVisualLine(nextDocumentLine);
                pos = nextLine.GetNextCaretPosition(-1, LogicalDirection.Forward, mode, enableVirtualSpace);
                if (pos < 0)
                {
                    throw ThrowUtil.NoValidCaretPosition();
                }
                return nextLine.GetTextViewPosition(pos);
            }
            Debug.Assert(visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.TotalLength == textView.Document.TextLength);
            return new TextViewPosition(textView.Document.GetLocation(textView.Document.TextLength));
        }

        private static TextViewPosition GetPrevCaretPosition(TextView textView, TextViewPosition caretPosition, VisualLine visualLine, CaretPositioningMode mode, bool enableVirtualSpace)
        {
            int pos = visualLine.GetNextCaretPosition(caretPosition.VisualColumn, LogicalDirection.Backward, mode, enableVirtualSpace);
            if (pos >= 0)
            {
                return visualLine.GetTextViewPosition(pos);
            }
            var previousDocumentLine = visualLine.FirstDocumentLine.PreviousLine;
            if (previousDocumentLine != null)
            {
                var previousLine = textView.GetOrConstructVisualLine(previousDocumentLine);
                pos = previousLine.GetNextCaretPosition(previousLine.VisualLength + 1, LogicalDirection.Backward, mode, enableVirtualSpace);
                if (pos < 0)
                {
                    throw ThrowUtil.NoValidCaretPosition();
                }
                return previousLine.GetTextViewPosition(pos);
            }
            Debug.Assert(visualLine.FirstDocumentLine.Offset == 0);
            return new TextViewPosition(0, 0);
        }

        private static TextViewPosition GetUpDownCaretPosition(TextView textView,
            TextViewPosition caretPosition,
            CaretMovementType direction,
            VisualLine visualLine,
            TextLine textLine,
            bool enableVirtualSpace,
            ref double xPos)
        {
            if (double.IsNaN(xPos))
            {
                xPos = visualLine.GetTextLineVisualXPosition(textLine, caretPosition.VisualColumn);
            }

            var targetVisualLine = visualLine;
            TextLine targetLine;
            int textLineIndex = visualLine.TextLines.IndexOf(textLine);
            switch (direction)
            {
                case CaretMovementType.LineUp:
                    {
                        int prevLineNumber = visualLine.FirstDocumentLine.LineNumber - 1;
                        if (textLineIndex > 0)
                        {
                            targetLine = visualLine.TextLines[textLineIndex - 1];
                        }
                        else if (prevLineNumber >= 1)
                        {
                            var prevLine = textView.Document.GetLineByNumber(prevLineNumber);
                            targetVisualLine = textView.GetOrConstructVisualLine(prevLine);
                            targetLine = targetVisualLine.TextLines[targetVisualLine.TextLines.Count - 1];
                        }
                        else
                        {
                            targetLine = null;
                        }
                        break;
                    }
                case CaretMovementType.LineDown:
                    {
                        int nextLineNumber = visualLine.LastDocumentLine.LineNumber + 1;
                        if (textLineIndex < visualLine.TextLines.Count - 1)
                        {
                            targetLine = visualLine.TextLines[textLineIndex + 1];
                        }
                        else if (nextLineNumber <= textView.Document.LineCount)
                        {
                            var nextLine = textView.Document.GetLineByNumber(nextLineNumber);
                            targetVisualLine = textView.GetOrConstructVisualLine(nextLine);
                            targetLine = targetVisualLine.TextLines[0];
                        }
                        else
                        {
                            targetLine = null;
                        }
                        break;
                    }
                case CaretMovementType.PageUp:
                case CaretMovementType.PageDown:
                    {
                        double yPos = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.LineMiddle);
                        if (direction == CaretMovementType.PageUp)
                        {
                            yPos -= textView.RenderSize.Height;
                        }
                        else
                        {
                            yPos += textView.RenderSize.Height;
                        }
                        var newLine = textView.GetDocumentLineByVisualTop(yPos);
                        targetVisualLine = textView.GetOrConstructVisualLine(newLine);
                        targetLine = targetVisualLine.GetTextLineByVisualYPosition(yPos);
                        break;
                    }
                default:
                    throw new NotSupportedException(direction.ToString());
            }
            if (targetLine != null)
            {
                double yPos = targetVisualLine.GetTextLineVisualYPosition(targetLine, VisualYPosition.LineMiddle);
                int newVisualColumn = targetVisualLine.GetVisualColumn(new Point(xPos, yPos), enableVirtualSpace);

                int targetLineStartCol = targetVisualLine.GetTextLineVisualStartColumn(targetLine);
                if (newVisualColumn >= targetLineStartCol + targetLine.Length)
                {
                    if (newVisualColumn <= targetVisualLine.VisualLength)
                    {
                        newVisualColumn = targetLineStartCol + targetLine.Length - 1;
                    }
                }
                return targetVisualLine.GetTextViewPosition(newVisualColumn);
            }
            return caretPosition;
        }

        #endregion
    }
}