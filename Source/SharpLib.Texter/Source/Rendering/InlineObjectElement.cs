using System;
using System.Windows;
using System.Windows.Media.TextFormatting;

namespace SharpLib.Texter.Rendering
{
    public class InlineObjectElement : VisualLineElement
    {
        #region Свойства

        public UIElement Element { get; private set; }

        #endregion

        #region Конструктор

        public InlineObjectElement(int documentLength, UIElement element)
            : base(1, documentLength)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            Element = element;
        }

        #endregion

        #region Методы

        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return new InlineObjectRun(1, TextRunProperties, Element);
        }

        #endregion
    }
}