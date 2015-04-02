using System;
using System.Globalization;
using System.Text;
using System.Windows.Documents;

namespace SharpLib.Notepad.Document
{
    public static class TextUtilities
    {
        #region Поля

        private static readonly string[] c0Table =
        {
            "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS", "HT",
            "LF", "VT", "FF", "CR", "SO", "SI", "DLE", "DC1", "DC2", "DC3",
            "DC4", "NAK", "SYN", "ETB", "CAN", "EM", "SUB", "ESC", "FS", "GS",
            "RS", "US"
        };

        private static readonly string[] delAndC1Table =
        {
            "DEL",
            "PAD", "HOP", "BPH", "NBH", "IND", "NEL", "SSA", "ESA", "HTS", "HTJ",
            "VTS", "PLD", "PLU", "RI", "SS2", "SS3", "DCS", "PU1", "PU2", "STS",
            "CCH", "MW", "SPA", "EPA", "SOS", "SGCI", "SCI", "CSI", "ST", "OSC",
            "PM", "APC"
        };

        #endregion

        #region Методы

        public static int FindNextNewLine(ITextSource text, int offset, out string newLineType)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (offset < 0 || offset > text.TextLength)
            {
                throw new ArgumentOutOfRangeException("offset", offset, "offset is outside of text source");
            }
            var s = NewLineFinder.NextNewLine(text, offset);
            if (s == SimpleSegment.Invalid)
            {
                newLineType = null;
                return -1;
            }
            if (s.Length == 2)
            {
                newLineType = "\r\n";
            }
            else if (text.GetCharAt(s.Offset) == '\n')
            {
                newLineType = "\n";
            }
            else
            {
                newLineType = "\r";
            }
            return s.Offset;
        }

        public static bool IsNewLine(string newLine)
        {
            return newLine == "\r\n" || newLine == "\n" || newLine == "\r";
        }

        public static string NormalizeNewLines(string input, string newLine)
        {
            if (input == null)
            {
                return null;
            }
            if (!IsNewLine(newLine))
            {
                throw new ArgumentException("newLine must be one of the known newline sequences");
            }
            var ds = NewLineFinder.NextNewLine(input, 0);
            if (ds == SimpleSegment.Invalid)
            {
                return input;
            }
            var b = new StringBuilder(input.Length);
            int lastEndOffset = 0;
            do
            {
                b.Append(input, lastEndOffset, ds.Offset - lastEndOffset);
                b.Append(newLine);
                lastEndOffset = ds.EndOffset;
                ds = NewLineFinder.NextNewLine(input, lastEndOffset);
            } while (ds != SimpleSegment.Invalid);

            b.Append(input, lastEndOffset, input.Length - lastEndOffset);
            return b.ToString();
        }

        public static string GetNewLineFromDocument(IDocument document, int lineNumber)
        {
            var line = document.GetLineByNumber(lineNumber);
            if (line.DelimiterLength == 0)
            {
                line = line.PreviousLine;
                if (line == null)
                {
                    return Environment.NewLine;
                }
            }
            return document.GetText(line.Offset + line.Length, line.DelimiterLength);
        }

        public static string GetControlCharacterName(char controlCharacter)
        {
            int num = controlCharacter;
            if (num < c0Table.Length)
            {
                return c0Table[num];
            }
            if (num >= 127 && num <= 159)
            {
                return delAndC1Table[num - 127];
            }
            return num.ToString("x4", CultureInfo.InvariantCulture);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace",
            Justification = "WPF uses 'Whitespace'")]
        public static ISegment GetWhitespaceAfter(ITextSource textSource, int offset)
        {
            if (textSource == null)
            {
                throw new ArgumentNullException("textSource");
            }
            int pos;
            for (pos = offset; pos < textSource.TextLength; pos++)
            {
                char c = textSource.GetCharAt(pos);
                if (c != ' ' && c != '\t')
                {
                    break;
                }
            }
            return new SimpleSegment(offset, pos - offset);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace",
            Justification = "WPF uses 'Whitespace'")]
        public static ISegment GetWhitespaceBefore(ITextSource textSource, int offset)
        {
            if (textSource == null)
            {
                throw new ArgumentNullException("textSource");
            }
            int pos;
            for (pos = offset - 1; pos >= 0; pos--)
            {
                char c = textSource.GetCharAt(pos);
                if (c != ' ' && c != '\t')
                {
                    break;
                }
            }
            pos++;
            return new SimpleSegment(pos, offset - pos);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace",
            Justification = "WPF uses 'Whitespace'")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Parameter cannot be ITextSource because it must belong to the DocumentLine")]
        public static ISegment GetLeadingWhitespace(TextDocument document, DocumentLine documentLine)
        {
            if (documentLine == null)
            {
                throw new ArgumentNullException("documentLine");
            }
            return GetWhitespaceAfter(document, documentLine.Offset);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace",
            Justification = "WPF uses 'Whitespace'")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Parameter cannot be ITextSource because it must belong to the DocumentLine")]
        public static ISegment GetTrailingWhitespace(TextDocument document, DocumentLine documentLine)
        {
            if (documentLine == null)
            {
                throw new ArgumentNullException("documentLine");
            }
            var segment = GetWhitespaceBefore(document, documentLine.EndOffset);

            if (segment.Offset == documentLine.Offset)
            {
                return new SimpleSegment(documentLine.EndOffset, 0);
            }
            return segment;
        }

        public static ISegment GetSingleIndentationSegment(ITextSource textSource, int offset, int indentationSize)
        {
            if (textSource == null)
            {
                throw new ArgumentNullException("textSource");
            }
            int pos = offset;
            while (pos < textSource.TextLength)
            {
                char c = textSource.GetCharAt(pos);
                if (c == '\t')
                {
                    if (pos == offset)
                    {
                        return new SimpleSegment(offset, 1);
                    }
                    break;
                }
                if (c == ' ')
                {
                    if (pos - offset >= indentationSize)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }

                pos++;
            }
            return new SimpleSegment(offset, pos - offset);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
        public static CharacterClass GetCharacterClass(char c)
        {
            if (c == '\r' || c == '\n')
            {
                return CharacterClass.LineTerminator;
            }
            if (c == '_')
            {
                return CharacterClass.IdentifierPart;
            }
            return GetCharacterClass(char.GetUnicodeCategory(c));
        }

        private static CharacterClass GetCharacterClass(char highSurrogate, char lowSurrogate)
        {
            if (char.IsSurrogatePair(highSurrogate, lowSurrogate))
            {
                return GetCharacterClass(char.GetUnicodeCategory(highSurrogate.ToString(CultureInfo.InvariantCulture) + lowSurrogate.ToString(CultureInfo.InvariantCulture), 0));
            }
            return CharacterClass.Other;
        }

        private static CharacterClass GetCharacterClass(UnicodeCategory c)
        {
            switch (c)
            {
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.Control:
                    return CharacterClass.Whitespace;
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    return CharacterClass.IdentifierPart;
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.EnclosingMark:
                    return CharacterClass.CombiningMark;
                default:
                    return CharacterClass.Other;
            }
        }

        public static int GetNextCaretPosition(ITextSource textSource, int offset, LogicalDirection direction, CaretPositioningMode mode)
        {
            if (textSource == null)
            {
                throw new ArgumentNullException("textSource");
            }
            switch (mode)
            {
                case CaretPositioningMode.Normal:
                case CaretPositioningMode.EveryCodepoint:
                case CaretPositioningMode.WordBorder:
                case CaretPositioningMode.WordBorderOrSymbol:
                case CaretPositioningMode.WordStart:
                case CaretPositioningMode.WordStartOrSymbol:
                    break;
                default:
                    throw new ArgumentException("Unsupported CaretPositioningMode: " + mode, "mode");
            }
            if (direction != LogicalDirection.Backward
                && direction != LogicalDirection.Forward)
            {
                throw new ArgumentException("Invalid LogicalDirection: " + direction, "direction");
            }
            int textLength = textSource.TextLength;
            if (textLength <= 0)
            {
                if (IsNormal(mode))
                {
                    if (offset > 0 && direction == LogicalDirection.Backward)
                    {
                        return 0;
                    }
                    if (offset < 0 && direction == LogicalDirection.Forward)
                    {
                        return 0;
                    }
                }
                return -1;
            }
            while (true)
            {
                int nextPos = (direction == LogicalDirection.Backward) ? offset - 1 : offset + 1;

                if (nextPos < 0 || nextPos > textLength)
                {
                    return -1;
                }

                if (nextPos == 0)
                {
                    if (IsNormal(mode) || !char.IsWhiteSpace(textSource.GetCharAt(0)))
                    {
                        return nextPos;
                    }
                }
                else if (nextPos == textLength)
                {
                    if (mode != CaretPositioningMode.WordStart && mode != CaretPositioningMode.WordStartOrSymbol)
                    {
                        if (IsNormal(mode) || !char.IsWhiteSpace(textSource.GetCharAt(textLength - 1)))
                        {
                            return nextPos;
                        }
                    }
                }
                else
                {
                    char charBefore = textSource.GetCharAt(nextPos - 1);
                    char charAfter = textSource.GetCharAt(nextPos);

                    if (!char.IsSurrogatePair(charBefore, charAfter))
                    {
                        var classBefore = GetCharacterClass(charBefore);
                        var classAfter = GetCharacterClass(charAfter);

                        if (char.IsLowSurrogate(charBefore) && nextPos >= 2)
                        {
                            classBefore = GetCharacterClass(textSource.GetCharAt(nextPos - 2), charBefore);
                        }
                        if (char.IsHighSurrogate(charAfter) && nextPos + 1 < textLength)
                        {
                            classAfter = GetCharacterClass(charAfter, textSource.GetCharAt(nextPos + 1));
                        }
                        if (StopBetweenCharacters(mode, classBefore, classAfter))
                        {
                            return nextPos;
                        }
                    }
                }

                offset = nextPos;
            }
        }

        private static bool IsNormal(CaretPositioningMode mode)
        {
            return mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint;
        }

        private static bool StopBetweenCharacters(CaretPositioningMode mode, CharacterClass charBefore, CharacterClass charAfter)
        {
            if (mode == CaretPositioningMode.EveryCodepoint)
            {
                return true;
            }

            if (charAfter == CharacterClass.CombiningMark)
            {
                return false;
            }

            if (mode == CaretPositioningMode.Normal)
            {
                return true;
            }
            if (charBefore == charAfter)
            {
                if (charBefore == CharacterClass.Other &&
                    (mode == CaretPositioningMode.WordBorderOrSymbol || mode == CaretPositioningMode.WordStartOrSymbol))
                {
                    return true;
                }
            }
            else
            {
                if (!((mode == CaretPositioningMode.WordStart || mode == CaretPositioningMode.WordStartOrSymbol)
                      && (charAfter == CharacterClass.Whitespace || charAfter == CharacterClass.LineTerminator)))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}