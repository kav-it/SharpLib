using System;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering
{
    internal sealed class SimpleTextSource : TextSource
    {
        #region Поля

        private readonly TextRunProperties properties;

        private readonly string text;

        #endregion

        #region Конструктор

        public SimpleTextSource(string text, TextRunProperties properties)
        {
            this.text = text;
            this.properties = properties;
        }

        #endregion

        #region Методы

        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            if (textSourceCharacterIndex < text.Length)
            {
                return new TextCharacters(text, textSourceCharacterIndex, text.Length - textSourceCharacterIndex, properties);
            }
            return new TextEndOfParagraph(1);
        }

        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            throw new NotImplementedException();
        }

        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}