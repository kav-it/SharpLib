using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

using SharpLib.Notepad.Document;
using SharpLib.Notepad.Highlighting;
using SharpLib.Notepad.Utils;

#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#endif

namespace SharpLib.Notepad.Editing
{
    internal static class EditingCommandHandler
    {
        #region Перечисления

        private enum DefaultSegmentType
        {
            None,

            WholeDocument,

            CurrentLine
        }

        #endregion

        #region Константы

        private const string LineSelectedType = "MSDEVLineSelect";

        #endregion

        #region Поля

        private static readonly List<CommandBinding> CommandBindings = new List<CommandBinding>();

        private static readonly List<InputBinding> InputBindings = new List<InputBinding>();

        #endregion

        #region Конструктор

        static EditingCommandHandler()
        {
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, OnDelete(CaretMovementType.None), CanDelete));
            AddBinding(EditingCommands.Delete, ModifierKeys.None, Key.Delete, OnDelete(CaretMovementType.CharRight));
            AddBinding(EditingCommands.DeleteNextWord, ModifierKeys.Control, Key.Delete, OnDelete(CaretMovementType.WordRight));
            AddBinding(EditingCommands.Backspace, ModifierKeys.None, Key.Back, OnDelete(CaretMovementType.Backspace));
            InputBindings.Add(TextAreaDefaultInputHandler.CreateFrozenKeyBinding(EditingCommands.Backspace, ModifierKeys.Shift, Key.Back));
            AddBinding(EditingCommands.DeletePreviousWord, ModifierKeys.Control, Key.Back, OnDelete(CaretMovementType.WordLeft));
            AddBinding(EditingCommands.EnterParagraphBreak, ModifierKeys.None, Key.Enter, OnEnter);
            AddBinding(EditingCommands.EnterLineBreak, ModifierKeys.Shift, Key.Enter, OnEnter);
            AddBinding(EditingCommands.TabForward, ModifierKeys.None, Key.Tab, OnTab);
            AddBinding(EditingCommands.TabBackward, ModifierKeys.Shift, Key.Tab, OnShiftTab);

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, CanCutOrCopy));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, OnCut, CanCutOrCopy));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));

            CommandBindings.Add(new CommandBinding(AvalonEditCommands.DeleteLine, OnDeleteLine));

            CommandBindings.Add(new CommandBinding(AvalonEditCommands.RemoveLeadingWhitespace, OnRemoveLeadingWhitespace));
            CommandBindings.Add(new CommandBinding(AvalonEditCommands.RemoveTrailingWhitespace, OnRemoveTrailingWhitespace));
            CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertToUppercase, OnConvertToUpperCase));
            CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertToLowercase, OnConvertToLowerCase));
            CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertToTitleCase, OnConvertToTitleCase));
            CommandBindings.Add(new CommandBinding(AvalonEditCommands.InvertCase, OnInvertCase));
            CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertTabsToSpaces, OnConvertTabsToSpaces));
            CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertSpacesToTabs, OnConvertSpacesToTabs));
            CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertLeadingTabsToSpaces, OnConvertLeadingTabsToSpaces));
            CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertLeadingSpacesToTabs, OnConvertLeadingSpacesToTabs));
            CommandBindings.Add(new CommandBinding(AvalonEditCommands.IndentSelection, OnIndentSelection));

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

        private static TextArea GetTextArea(object target)
        {
            return target as TextArea;
        }

        private static void TransformSelectedLines(Action<TextArea, DocumentLine> transformLine, object target, ExecutedRoutedEventArgs args, DefaultSegmentType defaultSegmentType)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                using (textArea.Document.RunUpdate())
                {
                    DocumentLine start, end;
                    if (textArea.Selection.IsEmpty)
                    {
                        if (defaultSegmentType == DefaultSegmentType.CurrentLine)
                        {
                            start = end = textArea.Document.GetLineByNumber(textArea.Caret.Line);
                        }
                        else if (defaultSegmentType == DefaultSegmentType.WholeDocument)
                        {
                            start = textArea.Document.Lines.First();
                            end = textArea.Document.Lines.Last();
                        }
                        else
                        {
                            start = end = null;
                        }
                    }
                    else
                    {
                        var segment = textArea.Selection.SurroundingSegment;
                        start = textArea.Document.GetLineByOffset(segment.Offset);
                        end = textArea.Document.GetLineByOffset(segment.EndOffset);

                        if (start != end && end.Offset == segment.EndOffset)
                        {
                            end = end.PreviousLine;
                        }
                    }
                    if (start != null)
                    {
                        transformLine(textArea, start);
                        while (start != end)
                        {
                            start = start.NextLine;
                            transformLine(textArea, start);
                        }
                    }
                }
                textArea.Caret.BringCaretToView();
                args.Handled = true;
            }
        }

        private static void TransformSelectedSegments(Action<TextArea, ISegment> transformSegment, object target, ExecutedRoutedEventArgs args, DefaultSegmentType defaultSegmentType)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                using (textArea.Document.RunUpdate())
                {
                    IEnumerable<ISegment> segments;
                    if (textArea.Selection.IsEmpty)
                    {
                        if (defaultSegmentType == DefaultSegmentType.CurrentLine)
                        {
                            segments = new ISegment[] { textArea.Document.GetLineByNumber(textArea.Caret.Line) };
                        }
                        else if (defaultSegmentType == DefaultSegmentType.WholeDocument)
                        {
                            segments = textArea.Document.Lines.Cast<ISegment>();
                        }
                        else
                        {
                            segments = null;
                        }
                    }
                    else
                    {
                        segments = textArea.Selection.Segments.Cast<ISegment>();
                    }
                    if (segments != null)
                    {
                        foreach (ISegment segment in segments.Reverse())
                        {
                            foreach (ISegment writableSegment in textArea.GetDeletableSegments(segment).Reverse())
                            {
                                transformSegment(textArea, writableSegment);
                            }
                        }
                    }
                }
                textArea.Caret.BringCaretToView();
                args.Handled = true;
            }
        }

        private static void OnEnter(object target, ExecutedRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.IsKeyboardFocused)
            {
                textArea.PerformTextInput("\n");
                args.Handled = true;
            }
        }

        private static void OnTab(object target, ExecutedRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                using (textArea.Document.RunUpdate())
                {
                    if (textArea.Selection.IsMultiline)
                    {
                        var segment = textArea.Selection.SurroundingSegment;
                        var start = textArea.Document.GetLineByOffset(segment.Offset);
                        var end = textArea.Document.GetLineByOffset(segment.EndOffset);

                        if (start != end && end.Offset == segment.EndOffset)
                        {
                            end = end.PreviousLine;
                        }
                        var current = start;
                        while (true)
                        {
                            int offset = current.Offset;
                            if (textArea.ReadOnlySectionProvider.CanInsert(offset))
                            {
                                textArea.Document.Replace(offset, 0, textArea.Options.IndentationString, OffsetChangeMappingType.KeepAnchorBeforeInsertion);
                            }
                            if (current == end)
                            {
                                break;
                            }
                            current = current.NextLine;
                        }
                    }
                    else
                    {
                        string indentationString = textArea.Options.GetIndentationString(textArea.Caret.Column);
                        textArea.ReplaceSelectionWithText(indentationString);
                    }
                }
                textArea.Caret.BringCaretToView();
                args.Handled = true;
            }
        }

        private static void OnShiftTab(object target, ExecutedRoutedEventArgs args)
        {
            TransformSelectedLines(
                delegate(TextArea textArea, DocumentLine line)
                {
                    int offset = line.Offset;
                    var s = TextUtilities.GetSingleIndentationSegment(textArea.Document, offset, textArea.Options.IndentationSize);
                    if (s.Length > 0)
                    {
                        s = textArea.GetDeletableSegments(s).FirstOrDefault();
                        if (s != null && s.Length > 0)
                        {
                            textArea.Document.Remove(s.Offset, s.Length);
                        }
                    }
                }, target, args, DefaultSegmentType.CurrentLine);
        }

        private static ExecutedRoutedEventHandler OnDelete(CaretMovementType caretMovement)
        {
            return (target, args) =>
            {
                var textArea = GetTextArea(target);
                if (textArea != null && textArea.Document != null)
                {
                    if (textArea.Selection.IsEmpty)
                    {
                        var startPos = textArea.Caret.Position;
                        bool enableVirtualSpace = textArea.Options.EnableVirtualSpace;

                        if (caretMovement == CaretMovementType.CharRight)
                        {
                            enableVirtualSpace = false;
                        }
                        double desiredXPos = textArea.Caret.DesiredXPos;
                        var endPos = CaretNavigationCommandHandler.GetNewCaretPosition(
                            textArea.TextView, startPos, caretMovement, enableVirtualSpace, ref desiredXPos);

                        if (endPos.Line < 1 || endPos.Column < 1)
                        {
                            endPos = new TextViewPosition(Math.Max(endPos.Line, 1), Math.Max(endPos.Column, 1));
                        }

                        var sel = new SimpleSelection(textArea, startPos, endPos);
                        sel.ReplaceSelectionWithText(string.Empty);
                    }
                    else
                    {
                        textArea.RemoveSelectedText();
                    }
                    textArea.Caret.BringCaretToView();
                    args.Handled = true;
                }
            };
        }

        private static void CanDelete(object target, CanExecuteRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                args.CanExecute = !textArea.Selection.IsEmpty;
                args.Handled = true;
            }
        }

        private static void CanCutOrCopy(object target, CanExecuteRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                args.CanExecute = textArea.Options.CutCopyWholeLine || !textArea.Selection.IsEmpty;
                args.Handled = true;
            }
        }

        private static void OnCopy(object target, ExecutedRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                if (textArea.Selection.IsEmpty && textArea.Options.CutCopyWholeLine)
                {
                    var currentLine = textArea.Document.GetLineByNumber(textArea.Caret.Line);
                    CopyWholeLine(textArea, currentLine);
                }
                else
                {
                    CopySelectedText(textArea);
                }
                args.Handled = true;
            }
        }

        private static void OnCut(object target, ExecutedRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                if (textArea.Selection.IsEmpty && textArea.Options.CutCopyWholeLine)
                {
                    var currentLine = textArea.Document.GetLineByNumber(textArea.Caret.Line);
                    if (CopyWholeLine(textArea, currentLine))
                    {
                        var segmentsToDelete = textArea.GetDeletableSegments(new SimpleSegment(currentLine.Offset, currentLine.TotalLength));
                        for (int i = segmentsToDelete.Length - 1; i >= 0; i--)
                        {
                            textArea.Document.Remove(segmentsToDelete[i]);
                        }
                    }
                }
                else
                {
                    if (CopySelectedText(textArea))
                    {
                        textArea.RemoveSelectedText();
                    }
                }
                textArea.Caret.BringCaretToView();
                args.Handled = true;
            }
        }

        private static bool CopySelectedText(TextArea textArea)
        {
            var data = textArea.Selection.CreateDataObject(textArea);
            var copyingEventArgs = new DataObjectCopyingEventArgs(data, false);
            textArea.RaiseEvent(copyingEventArgs);
            if (copyingEventArgs.CommandCancelled)
            {
                return false;
            }

            try
            {
                Clipboard.SetDataObject(data, true);
            }
            catch (ExternalException)
            {
            }

            string text = textArea.Selection.GetText();
            text = TextUtilities.NormalizeNewLines(text, Environment.NewLine);
            textArea.OnTextCopied(new TextEventArgs(text));
            return true;
        }

        public static bool ConfirmDataFormat(TextArea textArea, DataObject dataObject, string format)
        {
            var e = new DataObjectSettingDataEventArgs(dataObject, format);
            textArea.RaiseEvent(e);
            return !e.CommandCancelled;
        }

        private static bool CopyWholeLine(TextArea textArea, DocumentLine line)
        {
            ISegment wholeLine = new SimpleSegment(line.Offset, line.TotalLength);
            string text = textArea.Document.GetText(wholeLine);

            text = TextUtilities.NormalizeNewLines(text, Environment.NewLine);
            var data = new DataObject();
            if (ConfirmDataFormat(textArea, data, DataFormats.UnicodeText))
            {
                data.SetText(text);
            }

            if (ConfirmDataFormat(textArea, data, DataFormats.Html))
            {
                var highlighter = textArea.GetService(typeof(IHighlighter)) as IHighlighter;
                HtmlClipboard.SetHtml(data, HtmlClipboard.CreateHtmlFragment(textArea.Document, highlighter, wholeLine, new HtmlOptions(textArea.Options)));
            }

            if (ConfirmDataFormat(textArea, data, LineSelectedType))
            {
                var lineSelected = new MemoryStream(1);
                lineSelected.WriteByte(1);
                data.SetData(LineSelectedType, lineSelected, false);
            }

            var copyingEventArgs = new DataObjectCopyingEventArgs(data, false);
            textArea.RaiseEvent(copyingEventArgs);
            if (copyingEventArgs.CommandCancelled)
            {
                return false;
            }

            try
            {
                Clipboard.SetDataObject(data, true);
            }
            catch (ExternalException)
            {
                return false;
            }
            textArea.OnTextCopied(new TextEventArgs(text));
            return true;
        }

        private static void CanPaste(object target, CanExecuteRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                args.CanExecute = textArea.ReadOnlySectionProvider.CanInsert(textArea.Caret.Offset)
                                  && Clipboard.ContainsText();

                args.Handled = true;
            }
        }

        private static void OnPaste(object target, ExecutedRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                IDataObject dataObject;
                try
                {
                    dataObject = Clipboard.GetDataObject();
                }
                catch (ExternalException)
                {
                    return;
                }
                if (dataObject == null)
                {
                    return;
                }

                var pastingEventArgs = new DataObjectPastingEventArgs(dataObject, false, DataFormats.UnicodeText);
                textArea.RaiseEvent(pastingEventArgs);
                if (pastingEventArgs.CommandCancelled)
                {
                    return;
                }

                dataObject = pastingEventArgs.DataObject;
                if (dataObject == null)
                {
                    return;
                }

                string newLine = TextUtilities.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line);
                string text;
                try
                {
                    if (pastingEventArgs.FormatToApply != null && dataObject.GetDataPresent(pastingEventArgs.FormatToApply))
                    {
                        text = (string)dataObject.GetData(pastingEventArgs.FormatToApply);
                    }
                    else if (pastingEventArgs.FormatToApply != DataFormats.UnicodeText && dataObject.GetDataPresent(DataFormats.UnicodeText))
                    {
                        text = (string)dataObject.GetData(DataFormats.UnicodeText);
                    }
                    else if (pastingEventArgs.FormatToApply != DataFormats.Text && dataObject.GetDataPresent(DataFormats.Text))
                    {
                        text = (string)dataObject.GetData(DataFormats.Text);
                    }
                    else
                    {
                        return;
                    }
                    text = TextUtilities.NormalizeNewLines(text, newLine);
                    text = textArea.Options.ConvertTabsToSpaces ? text.Replace("\t", new String(' ', textArea.Options.IndentationSize)) : text;
                }
                catch (OutOfMemoryException)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(text))
                {
                    bool fullLine = textArea.Options.CutCopyWholeLine && dataObject.GetDataPresent(LineSelectedType);
                    bool rectangular = dataObject.GetDataPresent(RectangleSelection.RectangularSelectionDataType);

                    if (fullLine)
                    {
                        var currentLine = textArea.Document.GetLineByNumber(textArea.Caret.Line);
                        if (textArea.ReadOnlySectionProvider.CanInsert(currentLine.Offset))
                        {
                            textArea.Document.Insert(currentLine.Offset, text);
                        }
                    }
                    else if (rectangular && textArea.Selection.IsEmpty && !(textArea.Selection is RectangleSelection))
                    {
                        if (!RectangleSelection.PerformRectangularPaste(textArea, textArea.Caret.Position, text, false))
                        {
                            textArea.ReplaceSelectionWithText(text);
                        }
                    }
                    else
                    {
                        textArea.ReplaceSelectionWithText(text);
                    }
                }
                textArea.Caret.BringCaretToView();
                args.Handled = true;
            }
        }

        private static void OnDeleteLine(object target, ExecutedRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                int firstLineIndex, lastLineIndex;
                if (textArea.Selection.Length == 0)
                {
                    firstLineIndex = lastLineIndex = textArea.Caret.Line;
                }
                else
                {
                    firstLineIndex = Math.Min(textArea.Selection.StartPosition.Line, textArea.Selection.EndPosition.Line);
                    lastLineIndex = Math.Max(textArea.Selection.StartPosition.Line, textArea.Selection.EndPosition.Line);
                }
                var startLine = textArea.Document.GetLineByNumber(firstLineIndex);
                var endLine = textArea.Document.GetLineByNumber(lastLineIndex);
                textArea.Selection = Selection.Create(textArea, startLine.Offset, endLine.Offset + endLine.TotalLength);
                textArea.RemoveSelectedText();
                args.Handled = true;
            }
        }

        private static void OnRemoveLeadingWhitespace(object target, ExecutedRoutedEventArgs args)
        {
            TransformSelectedLines(
                delegate(TextArea textArea, DocumentLine line) { textArea.Document.Remove(TextUtilities.GetLeadingWhitespace(textArea.Document, line)); }, target, args,
                DefaultSegmentType.WholeDocument);
        }

        private static void OnRemoveTrailingWhitespace(object target, ExecutedRoutedEventArgs args)
        {
            TransformSelectedLines(
                delegate(TextArea textArea, DocumentLine line) { textArea.Document.Remove(TextUtilities.GetTrailingWhitespace(textArea.Document, line)); }, target, args,
                DefaultSegmentType.WholeDocument);
        }

        private static void OnConvertTabsToSpaces(object target, ExecutedRoutedEventArgs args)
        {
            TransformSelectedSegments(ConvertTabsToSpaces, target, args, DefaultSegmentType.WholeDocument);
        }

        private static void OnConvertLeadingTabsToSpaces(object target, ExecutedRoutedEventArgs args)
        {
            TransformSelectedLines(
                delegate(TextArea textArea, DocumentLine line) { ConvertTabsToSpaces(textArea, TextUtilities.GetLeadingWhitespace(textArea.Document, line)); }, target, args,
                DefaultSegmentType.WholeDocument);
        }

        private static void ConvertTabsToSpaces(TextArea textArea, ISegment segment)
        {
            var document = textArea.Document;
            int endOffset = segment.EndOffset;
            string indentationString = new string(' ', textArea.Options.IndentationSize);
            for (int offset = segment.Offset; offset < endOffset; offset++)
            {
                if (document.GetCharAt(offset) == '\t')
                {
                    document.Replace(offset, 1, indentationString, OffsetChangeMappingType.CharacterReplace);
                    endOffset += indentationString.Length - 1;
                }
            }
        }

        private static void OnConvertSpacesToTabs(object target, ExecutedRoutedEventArgs args)
        {
            TransformSelectedSegments(ConvertSpacesToTabs, target, args, DefaultSegmentType.WholeDocument);
        }

        private static void OnConvertLeadingSpacesToTabs(object target, ExecutedRoutedEventArgs args)
        {
            TransformSelectedLines(
                delegate(TextArea textArea, DocumentLine line) { ConvertSpacesToTabs(textArea, TextUtilities.GetLeadingWhitespace(textArea.Document, line)); }, target, args,
                DefaultSegmentType.WholeDocument);
        }

        private static void ConvertSpacesToTabs(TextArea textArea, ISegment segment)
        {
            var document = textArea.Document;
            int endOffset = segment.EndOffset;
            int indentationSize = textArea.Options.IndentationSize;
            int spacesCount = 0;
            for (int offset = segment.Offset; offset < endOffset; offset++)
            {
                if (document.GetCharAt(offset) == ' ')
                {
                    spacesCount++;
                    if (spacesCount == indentationSize)
                    {
                        document.Replace(offset - (indentationSize - 1), indentationSize, "\t", OffsetChangeMappingType.CharacterReplace);
                        spacesCount = 0;
                        offset -= indentationSize - 1;
                        endOffset -= indentationSize - 1;
                    }
                }
                else
                {
                    spacesCount = 0;
                }
            }
        }

        private static void ConvertCase(Func<string, string> transformText, object target, ExecutedRoutedEventArgs args)
        {
            TransformSelectedSegments(
                delegate(TextArea textArea, ISegment segment)
                {
                    string oldText = textArea.Document.GetText(segment);
                    string newText = transformText(oldText);
                    textArea.Document.Replace(segment.Offset, segment.Length, newText, OffsetChangeMappingType.CharacterReplace);
                }, target, args, DefaultSegmentType.WholeDocument);
        }

        private static void OnConvertToUpperCase(object target, ExecutedRoutedEventArgs args)
        {
            ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToUpper, target, args);
        }

        private static void OnConvertToLowerCase(object target, ExecutedRoutedEventArgs args)
        {
            ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToLower, target, args);
        }

        private static void OnConvertToTitleCase(object target, ExecutedRoutedEventArgs args)
        {
            ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToTitleCase, target, args);
        }

        private static void OnInvertCase(object target, ExecutedRoutedEventArgs args)
        {
            ConvertCase(InvertCase, target, args);
        }

        private static string InvertCase(string text)
        {
            var culture = CultureInfo.CurrentCulture;
            var buffer = text.ToCharArray();
            for (int i = 0; i < buffer.Length; ++i)
            {
                char c = buffer[i];
                buffer[i] = char.IsUpper(c) ? char.ToLower(c, culture) : char.ToUpper(c, culture);
            }
            return new string(buffer);
        }

        private static void OnIndentSelection(object target, ExecutedRoutedEventArgs args)
        {
            var textArea = GetTextArea(target);
            if (textArea != null && textArea.Document != null)
            {
                using (textArea.Document.RunUpdate())
                {
                    int start, end;
                    if (textArea.Selection.IsEmpty)
                    {
                        start = 1;
                        end = textArea.Document.LineCount;
                    }
                    else
                    {
                        start = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.Offset).LineNumber;
                        end = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.EndOffset).LineNumber;
                    }
                    textArea.IndentationStrategy.IndentLines(textArea.Document, start, end);
                }
                textArea.Caret.BringCaretToView();
                args.Handled = true;
            }
        }

        #endregion
    }
}