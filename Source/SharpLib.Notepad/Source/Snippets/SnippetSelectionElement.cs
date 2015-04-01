using System;
using System.Text;

namespace SharpLib.Notepad.Snippets
{
    [Serializable]
    public class SnippetSelectionElement : SnippetElement
    {
        #region Свойства

        public int Indentation { get; set; }

        #endregion

        #region Методы

        public override void Insert(InsertionContext context)
        {
            var tabString = new StringBuilder();

            for (int i = 0; i < Indentation; i++)
            {
                tabString.Append(context.Tab);
            }

            string indent = tabString.ToString();

            string text = context.SelectedText.TrimStart(' ', '\t');

            text = text.Replace(context.LineTerminator, context.LineTerminator + indent);

            context.Document.Insert(context.InsertionPosition, text);
            context.InsertionPosition += text.Length;

            if (string.IsNullOrEmpty(context.SelectedText))
            {
                SnippetCaretElement.SetCaret(context);
            }
        }

        #endregion
    }
}