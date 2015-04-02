namespace SharpLib.Notepad.Document
{
    internal static class NewLineFinder
    {
        #region Поля

        internal static readonly string[] NewlineStrings = { "\r\n", "\r", "\n" };

        private static readonly char[] newline = { '\r', '\n' };

        #endregion

        #region Методы

        internal static SimpleSegment NextNewLine(string text, int offset)
        {
            int pos = text.IndexOfAny(newline, offset);
            if (pos >= 0)
            {
                if (text[pos] == '\r')
                {
                    if (pos + 1 < text.Length && text[pos + 1] == '\n')
                    {
                        return new SimpleSegment(pos, 2);
                    }
                }
                return new SimpleSegment(pos, 1);
            }
            return SimpleSegment.Invalid;
        }

        internal static SimpleSegment NextNewLine(ITextSource text, int offset)
        {
            int textLength = text.TextLength;
            int pos = text.IndexOfAny(newline, offset, textLength - offset);
            if (pos >= 0)
            {
                if (text.GetCharAt(pos) == '\r')
                {
                    if (pos + 1 < textLength && text.GetCharAt(pos + 1) == '\n')
                    {
                        return new SimpleSegment(pos, 2);
                    }
                }
                return new SimpleSegment(pos, 1);
            }
            return SimpleSegment.Invalid;
        }

        #endregion
    }
}