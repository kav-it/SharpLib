using System;
using System.Collections.Generic;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering
{
    internal sealed class TextViewCachedElements : IDisposable
    {
        #region Поля

        private TextFormatter formatter;

        private Dictionary<string, TextLine> nonPrintableCharacterTexts;

        #endregion

        #region Методы

        public TextLine GetTextForNonPrintableCharacter(string text, ITextRunConstructionContext context)
        {
            if (nonPrintableCharacterTexts == null)
            {
                nonPrintableCharacterTexts = new Dictionary<string, TextLine>();
            }
            TextLine textLine;
            if (!nonPrintableCharacterTexts.TryGetValue(text, out textLine))
            {
                var p = new VisualLineElementTextRunProperties(context.GlobalTextRunProperties);
                p.SetForegroundBrush(context.TextView.NonPrintableCharacterBrush);
                if (formatter == null)
                {
                    formatter = TextFormatterFactory.Create(context.TextView);
                }
                textLine = FormattedTextElement.PrepareText(formatter, text, p);
                nonPrintableCharacterTexts[text] = textLine;
            }
            return textLine;
        }

        public void Dispose()
        {
            if (nonPrintableCharacterTexts != null)
            {
                foreach (TextLine line in nonPrintableCharacterTexts.Values)
                {
                    line.Dispose();
                }
            }
            if (formatter != null)
            {
                formatter.Dispose();
            }
        }

        #endregion
    }
}