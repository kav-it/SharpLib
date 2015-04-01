using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Utils;
#if DOTNET4
using System.Net;
#else
using System.Web;
#endif

namespace ICSharpCode.AvalonEdit.Highlighting
{
    internal class HtmlRichTextWriter : RichTextWriter
    {
        #region Поля

        private static readonly char[] specialChars = { ' ', '\t', '\r', '\n' };

        private readonly Stack<string> endTagStack = new Stack<string>();

        private readonly TextWriter htmlWriter;

        private readonly HtmlOptions options;

        private bool hasSpace;

        private int indentationLevel;

        private bool needIndentation = true;

        private bool spaceNeedsEscaping = true;

        #endregion

        #region Свойства

        public override Encoding Encoding
        {
            get { return htmlWriter.Encoding; }
        }

        #endregion

        #region Конструктор

        public HtmlRichTextWriter(TextWriter htmlWriter, HtmlOptions options = null)
        {
            if (htmlWriter == null)
            {
                throw new ArgumentNullException("htmlWriter");
            }
            this.htmlWriter = htmlWriter;
            this.options = options ?? new HtmlOptions();
        }

        #endregion

        #region Методы

        public override void Flush()
        {
            FlushSpace(true);
            htmlWriter.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                FlushSpace(true);
            }
            base.Dispose(disposing);
        }

        private void FlushSpace(bool nextIsWhitespace)
        {
            if (hasSpace)
            {
                if (spaceNeedsEscaping || nextIsWhitespace)
                {
                    htmlWriter.Write("&nbsp;");
                }
                else
                {
                    htmlWriter.Write(' ');
                }
                hasSpace = false;
                spaceNeedsEscaping = true;
            }
        }

        private void WriteIndentation()
        {
            if (needIndentation)
            {
                for (int i = 0; i < indentationLevel; i++)
                {
                    WriteChar('\t');
                }
                needIndentation = false;
            }
        }

        public override void Write(char value)
        {
            WriteIndentation();
            WriteChar(value);
        }

        private void WriteChar(char c)
        {
            bool isWhitespace = char.IsWhiteSpace(c);
            FlushSpace(isWhitespace);
            switch (c)
            {
                case ' ':
                    if (spaceNeedsEscaping)
                    {
                        htmlWriter.Write("&nbsp;");
                    }
                    else
                    {
                        hasSpace = true;
                    }
                    break;
                case '\t':
                    for (int i = 0; i < options.TabSize; i++)
                    {
                        htmlWriter.Write("&nbsp;");
                    }
                    break;
                case '\r':
                    break;
                case '\n':
                    htmlWriter.Write("<br/>");
                    needIndentation = true;
                    break;
                default:
#if DOTNET4
                    WebUtility.HtmlEncode(c.ToString(), htmlWriter);
#else
					HttpUtility.HtmlEncode(c.ToString(), htmlWriter);
#endif
                    break;
            }

            if (c != ' ')
            {
                spaceNeedsEscaping = isWhitespace;
            }
        }

        public override void Write(string value)
        {
            int pos = 0;
            do
            {
                int endPos = value.IndexOfAny(specialChars, pos);
                if (endPos < 0)
                {
                    WriteSimpleString(value.Substring(pos));
                    return;
                }
                if (endPos > pos)
                {
                    WriteSimpleString(value.Substring(pos, endPos - pos));
                }
                WriteChar(value[pos]);
                pos = endPos + 1;
            } while (pos < value.Length);
        }

        private void WriteIndentationAndSpace()
        {
            WriteIndentation();
            FlushSpace(false);
        }

        private void WriteSimpleString(string value)
        {
            if (value.Length == 0)
            {
                return;
            }
            WriteIndentationAndSpace();
#if DOTNET4
            WebUtility.HtmlEncode(value, htmlWriter);
#else
			HttpUtility.HtmlEncode(value, htmlWriter);
#endif
        }

        public override void Indent()
        {
            indentationLevel++;
        }

        public override void Unindent()
        {
            if (indentationLevel == 0)
            {
                throw new NotSupportedException();
            }
            indentationLevel--;
        }

        protected override void BeginUnhandledSpan()
        {
            endTagStack.Push(null);
        }

        public override void EndSpan()
        {
            htmlWriter.Write(endTagStack.Pop());
        }

        public override void BeginSpan(Color foregroundColor)
        {
            BeginSpan(new HighlightingColor
            {
                Foreground = new SimpleHighlightingBrush(foregroundColor)
            });
        }

        public override void BeginSpan(FontFamily fontFamily)
        {
            BeginUnhandledSpan();
        }

        public override void BeginSpan(FontStyle fontStyle)
        {
            BeginSpan(new HighlightingColor
            {
                FontStyle = fontStyle
            });
        }

        public override void BeginSpan(FontWeight fontWeight)
        {
            BeginSpan(new HighlightingColor
            {
                FontWeight = fontWeight
            });
        }

        public override void BeginSpan(HighlightingColor highlightingColor)
        {
            WriteIndentationAndSpace();
            if (options.ColorNeedsSpanForStyling(highlightingColor))
            {
                htmlWriter.Write("<span");
                options.WriteStyleAttributeForColor(htmlWriter, highlightingColor);
                htmlWriter.Write('>');
                endTagStack.Push("</span>");
            }
            else
            {
                endTagStack.Push(null);
            }
        }

        public override void BeginHyperlinkSpan(Uri uri)
        {
            WriteIndentationAndSpace();
#if DOTNET4
            string link = WebUtility.HtmlEncode(uri.ToString());
#else
			string link = HttpUtility.HtmlEncode(uri.ToString());
#endif
            htmlWriter.Write("<a href=\"" + link + "\">");
            endTagStack.Push("</a>");
        }

        #endregion
    }
}