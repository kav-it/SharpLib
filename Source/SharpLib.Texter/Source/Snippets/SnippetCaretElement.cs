using System;
using System.Runtime.Serialization;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Snippets
{
    [Serializable]
    public class SnippetCaretElement : SnippetElement
    {
        #region Поля

        [OptionalField]
        private readonly bool setCaretOnlyIfTextIsSelected;

        #endregion

        #region Конструктор

        public SnippetCaretElement()
        {
        }

        public SnippetCaretElement(bool setCaretOnlyIfTextIsSelected)
        {
            this.setCaretOnlyIfTextIsSelected = setCaretOnlyIfTextIsSelected;
        }

        #endregion

        #region Методы

        public override void Insert(InsertionContext context)
        {
            if (!setCaretOnlyIfTextIsSelected || !string.IsNullOrEmpty(context.SelectedText))
            {
                SetCaret(context);
            }
        }

        internal static void SetCaret(InsertionContext context)
        {
            var pos = context.Document.CreateAnchor(context.InsertionPosition);
            pos.MovementType = AnchorMovementType.BeforeInsertion;
            pos.SurviveDeletion = true;
            context.Deactivated += (sender, e) =>
            {
                if (e.Reason == DeactivateReason.ReturnPressed || e.Reason == DeactivateReason.NoActiveElements)
                {
                    context.TextArea.Caret.Offset = pos.Offset;
                }
            };
        }

        #endregion
    }
}