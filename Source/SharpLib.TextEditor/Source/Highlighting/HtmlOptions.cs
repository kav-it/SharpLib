using System;
using System.IO;
#if DOTNET4
using System.Net;

#else
using System.Web;
#endif

namespace ICSharpCode.AvalonEdit.Highlighting
{
    public class HtmlOptions
    {
        #region Свойства

        public int TabSize { get; set; }

        #endregion

        #region Конструктор

        public HtmlOptions()
        {
            TabSize = 4;
        }

        public HtmlOptions(TextEditorOptions options)
            : this()
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            TabSize = options.IndentationSize;
        }

        #endregion

        #region Методы

        public virtual void WriteStyleAttributeForColor(TextWriter writer, HighlightingColor color)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            writer.Write(" style=\"");
#if DOTNET4
            WebUtility.HtmlEncode(color.ToCss(), writer);
#else
			HttpUtility.HtmlEncode(color.ToCss(), writer);
#endif
            writer.Write('"');
        }

        public virtual bool ColorNeedsSpanForStyling(HighlightingColor color)
        {
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            return !string.IsNullOrEmpty(color.ToCss());
        }

        #endregion
    }
}