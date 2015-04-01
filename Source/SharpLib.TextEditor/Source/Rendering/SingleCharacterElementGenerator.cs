using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace")]
    internal sealed class SingleCharacterElementGenerator : VisualLineElementGenerator, IBuiltinElementGenerator
    {
        #region Свойства

        public bool ShowSpaces { get; set; }

        public bool ShowTabs { get; set; }

        public bool ShowBoxForControlCharacters { get; set; }

        #endregion

        #region Конструктор

        public SingleCharacterElementGenerator()
        {
            ShowSpaces = true;
            ShowTabs = true;
            ShowBoxForControlCharacters = true;
        }

        #endregion

        #region Методы

        void IBuiltinElementGenerator.FetchOptions(TextEditorOptions options)
        {
            ShowSpaces = options.ShowSpaces;
            ShowTabs = options.ShowTabs;
            ShowBoxForControlCharacters = options.ShowBoxForControlCharacters;
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            var endLine = CurrentContext.VisualLine.LastDocumentLine;
            var relevantText = CurrentContext.GetText(startOffset, endLine.EndOffset - startOffset);

            for (int i = 0; i < relevantText.Count; i++)
            {
                char c = relevantText.Text[relevantText.Offset + i];
                switch (c)
                {
                    case ' ':
                        if (ShowSpaces)
                        {
                            return startOffset + i;
                        }
                        break;
                    case '\t':
                        if (ShowTabs)
                        {
                            return startOffset + i;
                        }
                        break;
                    default:
                        if (ShowBoxForControlCharacters && char.IsControl(c))
                        {
                            return startOffset + i;
                        }
                        break;
                }
            }
            return -1;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            char c = CurrentContext.Document.GetCharAt(offset);
            if (ShowSpaces && c == ' ')
            {
                return new SpaceTextElement(CurrentContext.TextView.cachedElements.GetTextForNonPrintableCharacter("\u00B7", CurrentContext));
            }
            if (ShowTabs && c == '\t')
            {
                return new TabTextElement(CurrentContext.TextView.cachedElements.GetTextForNonPrintableCharacter("\u00BB", CurrentContext));
            }
            if (ShowBoxForControlCharacters && char.IsControl(c))
            {
                var p = new VisualLineElementTextRunProperties(CurrentContext.GlobalTextRunProperties);
                p.SetForegroundBrush(Brushes.White);
                var textFormatter = TextFormatterFactory.Create(CurrentContext.TextView);
                var text = FormattedTextElement.PrepareText(textFormatter,
                    TextUtilities.GetControlCharacterName(c), p);
                return new SpecialCharacterBoxElement(text);
            }
            return null;
        }

        #endregion

        #region Вложенный класс: SpaceTextElement

        private sealed class SpaceTextElement : FormattedTextElement
        {
            #region Конструктор

            public SpaceTextElement(TextLine textLine)
                : base(textLine, 1)
            {
                BreakBefore = LineBreakCondition.BreakPossible;
                BreakAfter = LineBreakCondition.BreakDesired;
            }

            #endregion

            #region Методы

            public override int GetNextCaretPosition(int visualColumn, LogicalDirection direction, CaretPositioningMode mode)
            {
                if (mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint)
                {
                    return base.GetNextCaretPosition(visualColumn, direction, mode);
                }
                return -1;
            }

            public override bool IsWhitespace(int visualColumn)
            {
                return true;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: SpecialCharacterBoxElement

        private sealed class SpecialCharacterBoxElement : FormattedTextElement
        {
            #region Конструктор

            public SpecialCharacterBoxElement(TextLine text)
                : base(text, 1)
            {
            }

            #endregion

            #region Методы

            public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
            {
                return new SpecialCharacterTextRun(this, TextRunProperties);
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: SpecialCharacterTextRun

        private sealed class SpecialCharacterTextRun : FormattedTextRun
        {
            #region Поля

            private static readonly SolidColorBrush darkGrayBrush;

            #endregion

            #region Конструктор

            static SpecialCharacterTextRun()
            {
                darkGrayBrush = new SolidColorBrush(Color.FromArgb(200, 128, 128, 128));
                darkGrayBrush.Freeze();
            }

            public SpecialCharacterTextRun(FormattedTextElement element, TextRunProperties properties)
                : base(element, properties)
            {
            }

            #endregion

            #region Методы

            public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
            {
                var newOrigin = new Point(origin.X + 1.5, origin.Y);
                var metrics = base.Format(double.PositiveInfinity);
                var r = new Rect(newOrigin.X - 0.5, newOrigin.Y - metrics.Baseline, metrics.Width + 2, metrics.Height);
                drawingContext.DrawRoundedRectangle(darkGrayBrush, null, r, 2.5, 2.5);
                base.Draw(drawingContext, newOrigin, rightToLeft, sideways);
            }

            public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
            {
                var metrics = base.Format(remainingParagraphWidth);
                return new TextEmbeddedObjectMetrics(metrics.Width + 3,
                    metrics.Height, metrics.Baseline);
            }

            public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
            {
                var r = base.ComputeBoundingBox(rightToLeft, sideways);
                r.Width += 3;
                return r;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: TabGlyphRun

        private sealed class TabGlyphRun : TextEmbeddedObject
        {
            #region Поля

            private readonly TabTextElement element;

            private readonly TextRunProperties properties;

            #endregion

            #region Свойства

            public override LineBreakCondition BreakBefore
            {
                get { return LineBreakCondition.BreakPossible; }
            }

            public override LineBreakCondition BreakAfter
            {
                get { return LineBreakCondition.BreakRestrained; }
            }

            public override bool HasFixedSize
            {
                get { return true; }
            }

            public override CharacterBufferReference CharacterBufferReference
            {
                get { return new CharacterBufferReference(); }
            }

            public override int Length
            {
                get { return 1; }
            }

            public override TextRunProperties Properties
            {
                get { return properties; }
            }

            #endregion

            #region Конструктор

            public TabGlyphRun(TabTextElement element, TextRunProperties properties)
            {
                if (properties == null)
                {
                    throw new ArgumentNullException("properties");
                }
                this.properties = properties;
                this.element = element;
            }

            #endregion

            #region Методы

            public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
            {
                double width = Math.Min(0, element.text.WidthIncludingTrailingWhitespace - 1);
                return new TextEmbeddedObjectMetrics(width, element.text.Height, element.text.Baseline);
            }

            public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
            {
                double width = Math.Min(0, element.text.WidthIncludingTrailingWhitespace - 1);
                return new Rect(0, 0, width, element.text.Height);
            }

            public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
            {
                origin.Y -= element.text.Baseline;
                element.text.Draw(drawingContext, origin, InvertAxes.None);
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: TabTextElement

        private sealed class TabTextElement : VisualLineElement
        {
            #region Поля

            internal readonly TextLine text;

            #endregion

            #region Конструктор

            public TabTextElement(TextLine text)
                : base(2, 1)
            {
                this.text = text;
            }

            #endregion

            #region Методы

            public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
            {
                if (startVisualColumn == VisualColumn)
                {
                    return new TabGlyphRun(this, TextRunProperties);
                }
                if (startVisualColumn == VisualColumn + 1)
                {
                    return new TextCharacters("\t", 0, 1, TextRunProperties);
                }
                throw new ArgumentOutOfRangeException("startVisualColumn");
            }

            public override int GetNextCaretPosition(int visualColumn, LogicalDirection direction, CaretPositioningMode mode)
            {
                if (mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint)
                {
                    return base.GetNextCaretPosition(visualColumn, direction, mode);
                }
                return -1;
            }

            public override bool IsWhitespace(int visualColumn)
            {
                return true;
            }

            #endregion
        }

        #endregion
    }
}