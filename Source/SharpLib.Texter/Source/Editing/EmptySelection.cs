using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using SharpLib.Texter.Document;
using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Editing
{
    internal sealed class EmptySelection : Selection
    {
        #region Свойства

        public override TextViewPosition StartPosition
        {
            get { return new TextViewPosition(TextLocation.Empty); }
        }

        public override TextViewPosition EndPosition
        {
            get { return new TextViewPosition(TextLocation.Empty); }
        }

        public override ISegment SurroundingSegment
        {
            get { return null; }
        }

        public override IEnumerable<SelectionSegment> Segments
        {
            get { return Empty<SelectionSegment>.Array; }
        }

        public override int Length
        {
            get { return 0; }
        }

        #endregion

        #region Конструктор

        public EmptySelection(TextArea textArea)
            : base(textArea)
        {
        }

        #endregion

        #region Методы

        public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
        {
            return this;
        }

        public override Selection SetEndpoint(TextViewPosition endPosition)
        {
            throw new NotSupportedException();
        }

        public override Selection StartSelectionOrSetEndpoint(TextViewPosition startPosition, TextViewPosition endPosition)
        {
            var document = textArea.Document;
            if (document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            return Create(textArea, startPosition, endPosition);
        }

        public override string GetText()
        {
            return string.Empty;
        }

        public override void ReplaceSelectionWithText(string newText)
        {
            if (newText == null)
            {
                throw new ArgumentNullException("newText");
            }
            newText = AddSpacesIfRequired(newText, textArea.Caret.Position, textArea.Caret.Position);
            if (newText.Length > 0)
            {
                if (textArea.ReadOnlySectionProvider.CanInsert(textArea.Caret.Offset))
                {
                    textArea.Document.Insert(textArea.Caret.Offset, newText);
                }
            }
            textArea.Caret.VisualColumn = -1;
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        public override bool Equals(object obj)
        {
            return this == obj;
        }

        #endregion
    }
}