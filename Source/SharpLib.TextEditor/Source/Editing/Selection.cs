using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;

#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#endif

namespace ICSharpCode.AvalonEdit.Editing
{
    public abstract class Selection
    {
        #region Поля

        internal readonly TextArea textArea;

        #endregion

        #region Свойства

        public abstract TextViewPosition StartPosition { get; }

        public abstract TextViewPosition EndPosition { get; }

        public abstract IEnumerable<SelectionSegment> Segments { get; }

        public abstract ISegment SurroundingSegment { get; }

        public virtual bool IsEmpty
        {
            get { return Length == 0; }
        }

        public virtual bool EnableVirtualSpace
        {
            get { return textArea.Options.EnableVirtualSpace; }
        }

        public abstract int Length { get; }

        public virtual bool IsMultiline
        {
            get
            {
                var surroundingSegment = SurroundingSegment;
                if (surroundingSegment == null)
                {
                    return false;
                }
                int start = surroundingSegment.Offset;
                int end = start + surroundingSegment.Length;
                var document = textArea.Document;
                if (document == null)
                {
                    throw ThrowUtil.NoDocumentAssigned();
                }
                return document.GetLineByOffset(start) != document.GetLineByOffset(end);
            }
        }

        #endregion

        #region Конструктор

        protected Selection(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            this.textArea = textArea;
        }

        #endregion

        #region Методы

        public static Selection Create(TextArea textArea, int startOffset, int endOffset)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            if (startOffset == endOffset)
            {
                return textArea.emptySelection;
            }
            return new SimpleSelection(textArea,
                new TextViewPosition(textArea.Document.GetLocation(startOffset)),
                new TextViewPosition(textArea.Document.GetLocation(endOffset)));
        }

        internal static Selection Create(TextArea textArea, TextViewPosition start, TextViewPosition end)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            if (textArea.Document.GetOffset(start.Location) == textArea.Document.GetOffset(end.Location) && start.VisualColumn == end.VisualColumn)
            {
                return textArea.emptySelection;
            }
            return new SimpleSelection(textArea, start, end);
        }

        public static Selection Create(TextArea textArea, ISegment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException("segment");
            }
            return Create(textArea, segment.Offset, segment.EndOffset);
        }

        public abstract void ReplaceSelectionWithText(string newText);

        internal string AddSpacesIfRequired(string newText, TextViewPosition start, TextViewPosition end)
        {
            if (EnableVirtualSpace && InsertVirtualSpaces(newText, start, end))
            {
                var line = textArea.Document.GetLineByNumber(start.Line);
                string lineText = textArea.Document.GetText(line);
                var vLine = textArea.TextView.GetOrConstructVisualLine(line);
                int colDiff = start.VisualColumn - vLine.VisualLengthWithEndOfLineMarker;
                if (colDiff > 0)
                {
                    string additionalSpaces = "";
                    if (!textArea.Options.ConvertTabsToSpaces && lineText.Trim('\t').Length == 0)
                    {
                        int tabCount = colDiff / textArea.Options.IndentationSize;
                        additionalSpaces = new string('\t', tabCount);
                        colDiff -= tabCount * textArea.Options.IndentationSize;
                    }
                    additionalSpaces += new string(' ', colDiff);
                    return additionalSpaces + newText;
                }
            }
            return newText;
        }

        private bool InsertVirtualSpaces(string newText, TextViewPosition start, TextViewPosition end)
        {
            return (!string.IsNullOrEmpty(newText) || !(IsInVirtualSpace(start) && IsInVirtualSpace(end)))
                   && newText != "\r\n"
                   && newText != "\n"
                   && newText != "\r";
        }

        private bool IsInVirtualSpace(TextViewPosition pos)
        {
            return pos.VisualColumn > textArea.TextView.GetOrConstructVisualLine(textArea.Document.GetLineByNumber(pos.Line)).VisualLength;
        }

        public abstract Selection UpdateOnDocumentChange(DocumentChangeEventArgs e);

        public abstract Selection SetEndpoint(TextViewPosition endPosition);

        public abstract Selection StartSelectionOrSetEndpoint(TextViewPosition startPosition, TextViewPosition endPosition);

        public virtual string GetText()
        {
            var document = textArea.Document;
            if (document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            StringBuilder b = null;
            string text = null;
            foreach (ISegment s in Segments)
            {
                if (text != null)
                {
                    if (b == null)
                    {
                        b = new StringBuilder(text);
                    }
                    else
                    {
                        b.Append(text);
                    }
                }
                text = document.GetText(s);
            }
            if (b != null)
            {
                if (text != null)
                {
                    b.Append(text);
                }
                return b.ToString();
            }
            return text ?? string.Empty;
        }

        public string CreateHtmlFragment(HtmlOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            var highlighter = textArea.GetService(typeof(IHighlighter)) as IHighlighter;
            var html = new StringBuilder();
            bool first = true;
            foreach (ISegment selectedSegment in Segments)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    html.AppendLine("<br>");
                }
                html.Append(HtmlClipboard.CreateHtmlFragment(textArea.Document, highlighter, selectedSegment, options));
            }
            return html.ToString();
        }

        public abstract override bool Equals(object obj);

        public abstract override int GetHashCode();

        public virtual bool Contains(int offset)
        {
            if (IsEmpty)
            {
                return false;
            }
            if (SurroundingSegment.Contains(offset, 0))
            {
                foreach (ISegment s in Segments)
                {
                    if (s.Contains(offset, 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual DataObject CreateDataObject(TextArea textArea)
        {
            var data = new DataObject();

            string text = TextUtilities.NormalizeNewLines(GetText(), Environment.NewLine);

            if (EditingCommandHandler.ConfirmDataFormat(textArea, data, DataFormats.UnicodeText))
            {
                data.SetText(text);
            }

            if (EditingCommandHandler.ConfirmDataFormat(textArea, data, typeof(string).FullName))
            {
                data.SetData(typeof(string).FullName, text);
            }

            if (EditingCommandHandler.ConfirmDataFormat(textArea, data, DataFormats.Html))
            {
                HtmlClipboard.SetHtml(data, CreateHtmlFragment(new HtmlOptions(textArea.Options)));
            }
            return data;
        }

        #endregion
    }
}