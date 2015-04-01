using System;
using System.Windows.Documents;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets
{
    [Serializable]
    public class SnippetBoundElement : SnippetElement
    {
        #region Поля

        private SnippetReplaceableTextElement targetElement;

        #endregion

        #region Свойства

        public SnippetReplaceableTextElement TargetElement
        {
            get { return targetElement; }
            set { targetElement = value; }
        }

        #endregion

        #region Методы

        public virtual string ConvertText(string input)
        {
            return input;
        }

        public override void Insert(InsertionContext context)
        {
            if (targetElement != null)
            {
                var start = context.Document.CreateAnchor(context.InsertionPosition);
                start.MovementType = AnchorMovementType.BeforeInsertion;
                start.SurviveDeletion = true;
                string inputText = targetElement.Text;
                if (inputText != null)
                {
                    context.InsertText(ConvertText(inputText));
                }
                var end = context.Document.CreateAnchor(context.InsertionPosition);
                end.MovementType = AnchorMovementType.BeforeInsertion;
                end.SurviveDeletion = true;
                var segment = new AnchorSegment(start, end);
                context.RegisterActiveElement(this, new BoundActiveElement(context, targetElement, this, segment));
            }
        }

        public override Inline ToTextRun()
        {
            if (targetElement != null)
            {
                string inputText = targetElement.Text;
                if (inputText != null)
                {
                    return new Italic(new Run(ConvertText(inputText)));
                }
            }
            return base.ToTextRun();
        }

        #endregion
    }
}