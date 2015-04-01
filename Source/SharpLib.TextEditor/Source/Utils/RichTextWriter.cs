using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.AvalonEdit.Utils
{
    internal abstract class RichTextWriter : TextWriter
    {
        #region Методы

        protected abstract void BeginUnhandledSpan();

        public void Write(RichText richText)
        {
            Write(richText, 0, richText.Length);
        }

        public virtual void Write(RichText richText, int offset, int length)
        {
            foreach (var section in richText.GetHighlightedSections(offset, length))
            {
                BeginSpan(section.Color);
                Write(richText.Text.Substring(section.Offset, section.Length));
                EndSpan();
            }
        }

        public virtual void BeginSpan(Color foregroundColor)
        {
            BeginUnhandledSpan();
        }

        public virtual void BeginSpan(FontWeight fontWeight)
        {
            BeginUnhandledSpan();
        }

        public virtual void BeginSpan(FontStyle fontStyle)
        {
            BeginUnhandledSpan();
        }

        public virtual void BeginSpan(FontFamily fontFamily)
        {
            BeginUnhandledSpan();
        }

        public virtual void BeginSpan(Highlighting.HighlightingColor highlightingColor)
        {
            BeginUnhandledSpan();
        }

        public virtual void BeginHyperlinkSpan(Uri uri)
        {
            BeginUnhandledSpan();
        }

        public abstract void EndSpan();

        public abstract void Indent();

        public abstract void Unindent();

        #endregion
    }
}