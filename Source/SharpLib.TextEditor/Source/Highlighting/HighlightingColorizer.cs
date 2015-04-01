using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Highlighting
{
    public class HighlightingColorizer : DocumentColorizingTransformer
    {
        #region Поля

        private readonly IHighlightingDefinition definition;

        private readonly bool isFixedHighlighter;

        private IHighlighter highlighter;

        private bool isInHighlightingGroup;

        private DocumentLine lastColorizedLine;

        private int lineNumberBeingColorized;

        private TextView textView;

        #endregion

        #region Конструктор

        public HighlightingColorizer(IHighlightingDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
            this.definition = definition;
        }

        public HighlightingColorizer(IHighlighter highlighter)
        {
            if (highlighter == null)
            {
                throw new ArgumentNullException("highlighter");
            }
            this.highlighter = highlighter;
            isFixedHighlighter = true;
        }

        protected HighlightingColorizer()
        {
        }

        #endregion

        #region Методы

        private void textView_DocumentChanged(object sender, EventArgs e)
        {
            var textView = (TextView)sender;
            DeregisterServices(textView);
            RegisterServices(textView);
        }

        protected virtual void DeregisterServices(TextView textView)
        {
            if (highlighter != null)
            {
                if (isInHighlightingGroup)
                {
                    highlighter.EndHighlighting();
                    isInHighlightingGroup = false;
                }
                highlighter.HighlightingStateChanged -= OnHighlightStateChanged;

                if (textView.Services.GetService(typeof(IHighlighter)) == highlighter)
                {
                    textView.Services.RemoveService(typeof(IHighlighter));
                }
                if (!isFixedHighlighter)
                {
                    if (highlighter != null)
                    {
                        highlighter.Dispose();
                    }
                    highlighter = null;
                }
            }
        }

        protected virtual void RegisterServices(TextView textView)
        {
            if (textView.Document != null)
            {
                if (!isFixedHighlighter)
                {
                    highlighter = textView.Document != null ? CreateHighlighter(textView, textView.Document) : null;
                }
                if (highlighter != null && highlighter.Document == textView.Document)
                {
                    if (textView.Services.GetService(typeof(IHighlighter)) == null)
                    {
                        textView.Services.AddService(typeof(IHighlighter), highlighter);
                    }
                    highlighter.HighlightingStateChanged += OnHighlightStateChanged;
                }
            }
        }

        protected virtual IHighlighter CreateHighlighter(TextView textView, TextDocument document)
        {
            if (definition != null)
            {
                return new DocumentHighlighter(document, definition);
            }
            throw new NotSupportedException("Cannot create a highlighter because no IHighlightingDefinition was specified, and the CreateHighlighter() method was not overridden.");
        }

        protected override void OnAddToTextView(TextView textView)
        {
            if (this.textView != null)
            {
                throw new InvalidOperationException("Cannot use a HighlightingColorizer instance in multiple text views. Please create a separate instance for each text view.");
            }
            base.OnAddToTextView(textView);
            this.textView = textView;
            textView.DocumentChanged += textView_DocumentChanged;
            textView.VisualLineConstructionStarting += textView_VisualLineConstructionStarting;
            textView.VisualLinesChanged += textView_VisualLinesChanged;
            RegisterServices(textView);
        }

        protected override void OnRemoveFromTextView(TextView textView)
        {
            DeregisterServices(textView);
            textView.DocumentChanged -= textView_DocumentChanged;
            textView.VisualLineConstructionStarting -= textView_VisualLineConstructionStarting;
            textView.VisualLinesChanged -= textView_VisualLinesChanged;
            base.OnRemoveFromTextView(textView);
            this.textView = null;
        }

        private void textView_VisualLineConstructionStarting(object sender, VisualLineConstructionStartEventArgs e)
        {
            if (highlighter != null)
            {
                lineNumberBeingColorized = e.FirstLineInView.LineNumber - 1;
                if (!isInHighlightingGroup)
                {
                    highlighter.BeginHighlighting();
                    isInHighlightingGroup = true;
                }
                highlighter.UpdateHighlightingState(lineNumberBeingColorized);
                lineNumberBeingColorized = 0;
            }
        }

        private void textView_VisualLinesChanged(object sender, EventArgs e)
        {
            if (highlighter != null && isInHighlightingGroup)
            {
                highlighter.EndHighlighting();
                isInHighlightingGroup = false;
            }
        }

        protected override void Colorize(ITextRunConstructionContext context)
        {
            lastColorizedLine = null;
            base.Colorize(context);
            if (lastColorizedLine != context.VisualLine.LastDocumentLine)
            {
                if (highlighter != null)
                {
                    lineNumberBeingColorized = context.VisualLine.LastDocumentLine.LineNumber;
                    highlighter.UpdateHighlightingState(lineNumberBeingColorized);
                    lineNumberBeingColorized = 0;
                }
            }
            lastColorizedLine = null;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (highlighter != null)
            {
                lineNumberBeingColorized = line.LineNumber;
                var hl = highlighter.HighlightLine(lineNumberBeingColorized);
                lineNumberBeingColorized = 0;
                foreach (HighlightedSection section in hl.Sections)
                {
                    if (IsEmptyColor(section.Color))
                    {
                        continue;
                    }
                    ChangeLinePart(section.Offset, section.Offset + section.Length,
                        visualLineElement => ApplyColorToElement(visualLineElement, section.Color));
                }
            }
            lastColorizedLine = line;
        }

        internal static bool IsEmptyColor(HighlightingColor color)
        {
            if (color == null)
            {
                return true;
            }
            return color.Background == null && color.Foreground == null
                   && color.FontStyle == null && color.FontWeight == null
                   && color.Underline == null;
        }

        protected virtual void ApplyColorToElement(VisualLineElement element, HighlightingColor color)
        {
            ApplyColorToElement(element, color, CurrentContext);
        }

        internal static void ApplyColorToElement(VisualLineElement element, HighlightingColor color, ITextRunConstructionContext context)
        {
            if (color.Foreground != null)
            {
                var b = color.Foreground.GetBrush(context);
                if (b != null)
                {
                    element.TextRunProperties.SetForegroundBrush(b);
                }
            }
            if (color.Background != null)
            {
                var b = color.Background.GetBrush(context);
                if (b != null)
                {
                    element.BackgroundBrush = b;
                }
            }
            if (color.FontStyle != null || color.FontWeight != null)
            {
                var tf = element.TextRunProperties.Typeface;
                element.TextRunProperties.SetTypeface(new Typeface(
                    tf.FontFamily,
                    color.FontStyle ?? tf.Style,
                    color.FontWeight ?? tf.Weight,
                    tf.Stretch
                    ));
            }
            if (color.Underline ?? false)
            {
                element.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
            }
        }

        private void OnHighlightStateChanged(int fromLineNumber, int toLineNumber)
        {
            if (lineNumberBeingColorized != 0)
            {
                if (toLineNumber <= lineNumberBeingColorized)
                {
                    return;
                }
            }

            Debug.WriteLine("OnHighlightStateChanged forces redraw of lines {0} to {1}", fromLineNumber, toLineNumber);

            if (fromLineNumber == toLineNumber)
            {
                textView.Redraw(textView.Document.GetLineByNumber(fromLineNumber));
            }
            else
            {
                var fromLine = textView.Document.GetLineByNumber(fromLineNumber);
                var toLine = textView.Document.GetLineByNumber(toLineNumber);
                int startOffset = fromLine.Offset;
                textView.Redraw(startOffset, toLine.EndOffset - startOffset);
            }
        }

        #endregion
    }
}