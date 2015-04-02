using System;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Snippets
{
    internal sealed class BoundActiveElement : IActiveElement
    {
        #region Поля

        private readonly SnippetBoundElement boundElement;

        private readonly InsertionContext context;

        private readonly SnippetReplaceableTextElement targetSnippetElement;

        private AnchorSegment segment;

        internal IReplaceableActiveElement targetElement;

        #endregion

        #region Свойства

        public bool IsEditable
        {
            get { return false; }
        }

        public ISegment Segment
        {
            get { return segment; }
        }

        #endregion

        #region Конструктор

        public BoundActiveElement(InsertionContext context, SnippetReplaceableTextElement targetSnippetElement, SnippetBoundElement boundElement, AnchorSegment segment)
        {
            this.context = context;
            this.targetSnippetElement = targetSnippetElement;
            this.boundElement = boundElement;
            this.segment = segment;
        }

        #endregion

        #region Методы

        public void OnInsertionCompleted()
        {
            targetElement = context.GetActiveElement(targetSnippetElement) as IReplaceableActiveElement;
            if (targetElement != null)
            {
                targetElement.TextChanged += targetElement_TextChanged;
            }
        }

        private void targetElement_TextChanged(object sender, EventArgs e)
        {
            if (SimpleSegment.GetOverlap(segment, targetElement.Segment) == SimpleSegment.Invalid)
            {
                int offset = segment.Offset;
                int length = segment.Length;
                string text = boundElement.ConvertText(targetElement.Text);
                if (length != text.Length || text != context.Document.GetText(offset, length))
                {
                    context.Document.Replace(offset, length, text);
                    if (length == 0)
                    {
                        segment = new AnchorSegment(context.Document, offset, text.Length);
                    }
                }
            }
        }

        public void Deactivate(SnippetEventArgs e)
        {
        }

        #endregion
    }
}