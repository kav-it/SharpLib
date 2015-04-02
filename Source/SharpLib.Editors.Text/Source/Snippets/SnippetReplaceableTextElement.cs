using System;
using System.Windows.Documents;

namespace SharpLib.Notepad.Snippets
{
    [Serializable]
    public class SnippetReplaceableTextElement : SnippetTextElement
    {
        #region Методы

        public override void Insert(InsertionContext context)
        {
            int start = context.InsertionPosition;
            base.Insert(context);
            int end = context.InsertionPosition;
            context.RegisterActiveElement(this, new ReplaceableActiveElement(context, start, end));
        }

        public override Inline ToTextRun()
        {
            return new Italic(base.ToTextRun());
        }

        #endregion
    }
}