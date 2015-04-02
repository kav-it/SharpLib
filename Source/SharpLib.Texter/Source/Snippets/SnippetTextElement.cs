using System;
using System.Windows.Documents;

namespace SharpLib.Texter.Snippets
{
    [Serializable]
    public class SnippetTextElement : SnippetElement
    {
        #region Поля

        #endregion

        #region Свойства

        public string Text { get; set; }

        #endregion

        #region Методы

        public override void Insert(InsertionContext context)
        {
            if (Text != null)
            {
                context.InsertText(Text);
            }
        }

        public override Inline ToTextRun()
        {
            return new Run(Text ?? string.Empty);
        }

        #endregion
    }
}