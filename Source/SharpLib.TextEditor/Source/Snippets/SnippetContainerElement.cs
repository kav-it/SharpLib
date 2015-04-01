using System;
using System.Collections.Generic;
using System.Windows.Documents;

using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Snippets
{
    [Serializable]
    public class SnippetContainerElement : SnippetElement
    {
        #region Поля

        private readonly NullSafeCollection<SnippetElement> elements = new NullSafeCollection<SnippetElement>();

        #endregion

        #region Свойства

        public IList<SnippetElement> Elements
        {
            get { return elements; }
        }

        #endregion

        #region Методы

        public override void Insert(InsertionContext context)
        {
            foreach (SnippetElement e in Elements)
            {
                e.Insert(context);
            }
        }

        public override Inline ToTextRun()
        {
            var span = new Span();
            foreach (SnippetElement e in Elements)
            {
                var r = e.ToTextRun();
                if (r != null)
                {
                    span.Inlines.Add(r);
                }
            }
            return span;
        }

        #endregion
    }
}