﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Highlighting
{
    public static class HtmlClipboard
    {
        #region Методы

        private static string BuildHeader(int startHTML, int endHTML, int startFragment, int endFragment)
        {
            var b = new StringBuilder();
            b.AppendLine("Version:0.9");
            b.AppendLine("StartHTML:" + startHTML.ToString("d8", CultureInfo.InvariantCulture));
            b.AppendLine("EndHTML:" + endHTML.ToString("d8", CultureInfo.InvariantCulture));
            b.AppendLine("StartFragment:" + startFragment.ToString("d8", CultureInfo.InvariantCulture));
            b.AppendLine("EndFragment:" + endFragment.ToString("d8", CultureInfo.InvariantCulture));
            return b.ToString();
        }

        public static void SetHtml(DataObject dataObject, string htmlFragment)
        {
            if (dataObject == null)
            {
                throw new ArgumentNullException("dataObject");
            }
            if (htmlFragment == null)
            {
                throw new ArgumentNullException("htmlFragment");
            }

            string htmlStart = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">" + Environment.NewLine
                               + "<HTML>" + Environment.NewLine
                               + "<BODY>" + Environment.NewLine
                               + "<!--StartFragment-->" + Environment.NewLine;
            string htmlEnd = "<!--EndFragment-->" + Environment.NewLine + "</BODY>" + Environment.NewLine + "</HTML>" + Environment.NewLine;
            string dummyHeader = BuildHeader(0, 0, 0, 0);

            int startHTML = dummyHeader.Length;
            int startFragment = startHTML + htmlStart.Length;
            int endFragment = startFragment + Encoding.UTF8.GetByteCount(htmlFragment);
            int endHTML = endFragment + htmlEnd.Length;
            string cf_html = BuildHeader(startHTML, endHTML, startFragment, endFragment) + htmlStart + htmlFragment + htmlEnd;
            Debug.WriteLine(cf_html);
            dataObject.SetText(cf_html, TextDataFormat.Html);
        }

        public static string CreateHtmlFragment(IDocument document, IHighlighter highlighter, ISegment segment, HtmlOptions options)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (highlighter != null && highlighter.Document != document)
            {
                throw new ArgumentException("Highlighter does not belong to the specified document.");
            }
            if (segment == null)
            {
                segment = new SimpleSegment(0, document.TextLength);
            }

            var html = new StringBuilder();
            int segmentEndOffset = segment.EndOffset;
            var line = document.GetLineByOffset(segment.Offset);
            while (line != null && line.Offset < segmentEndOffset)
            {
                HighlightedLine highlightedLine;
                if (highlighter != null)
                {
                    highlightedLine = highlighter.HighlightLine(line.LineNumber);
                }
                else
                {
                    highlightedLine = new HighlightedLine(document, line);
                }
                var s = SimpleSegment.GetOverlap(segment, line);
                if (html.Length > 0)
                {
                    html.AppendLine("<br>");
                }
                html.Append(highlightedLine.ToHtml(s.Offset, s.EndOffset, options));
                line = line.NextLine;
            }
            return html.ToString();
        }

        #endregion
    }
}