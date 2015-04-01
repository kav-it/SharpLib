using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Snippets
{
    public sealed class SnippetAnchorElement : SnippetElement
    {
        #region Свойства

        public string Name { get; private set; }

        #endregion

        #region Конструктор

        public SnippetAnchorElement(string name)
        {
            Name = name;
        }

        #endregion

        #region Методы

        public override void Insert(InsertionContext context)
        {
            var start = context.Document.CreateAnchor(context.InsertionPosition);
            start.MovementType = AnchorMovementType.BeforeInsertion;
            start.SurviveDeletion = true;
            var segment = new AnchorSegment(start, start);
            context.RegisterActiveElement(this, new AnchorElement(segment, Name, context));
        }

        #endregion
    }
}