﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.AvalonEdit.Utils
{
    public static class DocumentPrinter
    {
#if NREFACTORY
		
		
		
		public static Block ConvertTextDocumentToBlock(ReadOnlyDocument document, IHighlightingDefinition highlightingDefinition)
		{
			IHighlighter highlighter;
			if (highlightingDefinition != null)
				highlighter = new DocumentHighlighter(document, highlightingDefinition);
			else
				highlighter = null;
			return ConvertTextDocumentToBlock(document, highlighter);
		}
#endif

        public static Block ConvertTextDocumentToBlock(IDocument document, IHighlighter highlighter)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            var p = new Paragraph();
            p.TextAlignment = TextAlignment.Left;
            for (int lineNumber = 1; lineNumber <= document.LineCount; lineNumber++)
            {
                if (lineNumber > 1)
                {
                    p.Inlines.Add(new LineBreak());
                }
                var line = document.GetLineByNumber(lineNumber);
                if (highlighter != null)
                {
                    var highlightedLine = highlighter.HighlightLine(lineNumber);
                    p.Inlines.AddRange(highlightedLine.ToRichText().CreateRuns());
                }
                else
                {
                    p.Inlines.Add(document.GetText(line));
                }
            }
            return p;
        }

#if NREFACTORY
		
		
		
		public static RichText ConvertTextDocumentToRichText(ReadOnlyDocument document, IHighlightingDefinition highlightingDefinition)
		{
			IHighlighter highlighter;
			if (highlightingDefinition != null)
				highlighter = new DocumentHighlighter(document, highlightingDefinition);
			else
				highlighter = null;
			return ConvertTextDocumentToRichText(document, highlighter);
		}
#endif

        public static RichText ConvertTextDocumentToRichText(IDocument document, IHighlighter highlighter)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            var texts = new List<RichText>();
            for (int lineNumber = 1; lineNumber <= document.LineCount; lineNumber++)
            {
                var line = document.GetLineByNumber(lineNumber);
                if (lineNumber > 1)
                {
                    texts.Add(line.PreviousLine.DelimiterLength == 2 ? "\r\n" : "\n");
                }
                if (highlighter != null)
                {
                    var highlightedLine = highlighter.HighlightLine(lineNumber);
                    texts.Add(highlightedLine.ToRichText());
                }
                else
                {
                    texts.Add(document.GetText(line));
                }
            }
            return RichText.Concat(texts.ToArray());
        }

        public static FlowDocument CreateFlowDocumentForEditor(TextEditor editor)
        {
            var highlighter = editor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;
            var doc = new FlowDocument(ConvertTextDocumentToBlock(editor.Document, highlighter));
            doc.FontFamily = editor.FontFamily;
            doc.FontSize = editor.FontSize;
            return doc;
        }
    }
}