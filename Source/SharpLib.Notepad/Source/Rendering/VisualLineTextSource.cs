﻿using System;
using System.Diagnostics;
using System.Windows.Media.TextFormatting;

using SharpLib.Notepad.Document;
using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Rendering
{
    internal sealed class VisualLineTextSource : TextSource, ITextRunConstructionContext
    {
        #region Поля

        private string cachedString;

        private int cachedStringOffset;

        #endregion

        #region Свойства

        public VisualLine VisualLine { get; private set; }

        public TextView TextView { get; set; }

        public TextDocument Document { get; set; }

        public TextRunProperties GlobalTextRunProperties { get; set; }

        #endregion

        #region Конструктор

        public VisualLineTextSource(VisualLine visualLine)
        {
            VisualLine = visualLine;
        }

        #endregion

        #region Методы

        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            try
            {
                foreach (VisualLineElement element in VisualLine.Elements)
                {
                    if (textSourceCharacterIndex >= element.VisualColumn
                        && textSourceCharacterIndex < element.VisualColumn + element.VisualLength)
                    {
                        int relativeOffset = textSourceCharacterIndex - element.VisualColumn;
                        var run = element.CreateTextRun(textSourceCharacterIndex, this);
                        if (run == null)
                        {
                            throw new ArgumentNullException(element.GetType().Name + ".CreateTextRun");
                        }
                        if (run.Length == 0)
                        {
                            throw new ArgumentException("The returned TextRun must not have length 0.", element.GetType().Name + ".Length");
                        }
                        if (relativeOffset + run.Length > element.VisualLength)
                        {
                            throw new ArgumentException("The returned TextRun is too long.", element.GetType().Name + ".CreateTextRun");
                        }
                        var inlineRun = run as InlineObjectRun;
                        if (inlineRun != null)
                        {
                            inlineRun.VisualLine = VisualLine;
                            VisualLine.hasInlineObjects = true;
                            TextView.AddInlineObject(inlineRun);
                        }
                        return run;
                    }
                }
                if (TextView.Options.ShowEndOfLine && textSourceCharacterIndex == VisualLine.VisualLength)
                {
                    return CreateTextRunForNewLine();
                }
                return new TextEndOfParagraph(1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        private TextRun CreateTextRunForNewLine()
        {
            string newlineText = "";
            var lastDocumentLine = VisualLine.LastDocumentLine;
            if (lastDocumentLine.DelimiterLength == 2)
            {
                newlineText = "¶";
            }
            else if (lastDocumentLine.DelimiterLength == 1)
            {
                char newlineChar = Document.GetCharAt(lastDocumentLine.Offset + lastDocumentLine.Length);
                if (newlineChar == '\r')
                {
                    newlineText = "\\r";
                }
                else if (newlineChar == '\n')
                {
                    newlineText = "\\n";
                }
                else
                {
                    newlineText = "?";
                }
            }
            return new FormattedTextRun(new FormattedTextElement(TextView.cachedElements.GetTextForNonPrintableCharacter(newlineText, this), 0), GlobalTextRunProperties);
        }

        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            try
            {
                foreach (VisualLineElement element in VisualLine.Elements)
                {
                    if (textSourceCharacterIndexLimit > element.VisualColumn
                        && textSourceCharacterIndexLimit <= element.VisualColumn + element.VisualLength)
                    {
                        var span = element.GetPrecedingText(textSourceCharacterIndexLimit, this);
                        if (span == null)
                        {
                            break;
                        }
                        int relativeOffset = textSourceCharacterIndexLimit - element.VisualColumn;
                        if (span.Length > relativeOffset)
                        {
                            throw new ArgumentException("The returned TextSpan is too long.", element.GetType().Name + ".GetPrecedingText");
                        }
                        return span;
                    }
                }
                var empty = CharacterBufferRange.Empty;
                return new TextSpan<CultureSpecificCharacterBufferRange>(empty.Length, new CultureSpecificCharacterBufferRange(null, empty));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            throw new NotSupportedException();
        }

        public StringSegment GetText(int offset, int length)
        {
            if (cachedString != null)
            {
                if (offset >= cachedStringOffset && offset + length <= cachedStringOffset + cachedString.Length)
                {
                    return new StringSegment(cachedString, offset - cachedStringOffset, length);
                }
            }
            cachedStringOffset = offset;
            return new StringSegment(cachedString = Document.GetText(offset, length));
        }

        #endregion
    }
}