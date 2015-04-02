using System;

using SharpLib.Texter.Document;
using SharpLib.Texter.Rendering;

namespace SharpLib.Texter.Highlighting
{
    public class RichTextColorizer : DocumentColorizingTransformer
    {
        #region Поля

        private readonly RichTextModel richTextModel;

        #endregion

        #region Конструктор

        public RichTextColorizer(RichTextModel richTextModel)
        {
            if (richTextModel == null)
            {
                throw new ArgumentNullException("richTextModel");
            }
            this.richTextModel = richTextModel;
        }

        #endregion

        #region Методы

        protected override void ColorizeLine(DocumentLine line)
        {
            var sections = richTextModel.GetHighlightedSections(line.Offset, line.Length);
            foreach (HighlightedSection section in sections)
            {
                if (HighlightingColorizer.IsEmptyColor(section.Color))
                {
                    continue;
                }
                ChangeLinePart(section.Offset, section.Offset + section.Length,
                    visualLineElement => HighlightingColorizer.ApplyColorToElement(visualLineElement, section.Color, CurrentContext));
            }
        }

        #endregion
    }
}