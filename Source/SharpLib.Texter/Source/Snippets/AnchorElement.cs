using SharpLib.Texter.Document;

namespace SharpLib.Texter.Snippets
{
    public sealed class AnchorElement : IActiveElement
    {
        #region Поля

        private readonly InsertionContext context;

        private AnchorSegment segment;

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

        public string Text
        {
            get { return context.Document.GetText(segment); }
            set
            {
                int offset = segment.Offset;
                int length = segment.Length;
                context.Document.Replace(offset, length, value);
                if (length == 0)
                {
                    segment = new AnchorSegment(context.Document, offset, value.Length);
                }
            }
        }

        public string Name { get; private set; }

        #endregion

        #region Конструктор

        public AnchorElement(AnchorSegment segment, string name, InsertionContext context)
        {
            this.segment = segment;
            this.context = context;
            Name = name;
        }

        #endregion

        #region Методы

        public void OnInsertionCompleted()
        {
        }

        public void Deactivate(SnippetEventArgs e)
        {
        }

        #endregion
    }
}