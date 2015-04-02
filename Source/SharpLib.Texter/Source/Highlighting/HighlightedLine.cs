using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using SharpLib.Texter.Document;
using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Highlighting
{
    public class HighlightedLine
    {
        #region Свойства

        public IDocument Document { get; private set; }

        public IDocumentLine DocumentLine { get; private set; }

        public IList<HighlightedSection> Sections { get; private set; }

        #endregion

        #region Конструктор

        public HighlightedLine(IDocument document, IDocumentLine documentLine)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            Document = document;
            DocumentLine = documentLine;
            Sections = new NullSafeCollection<HighlightedSection>();
        }

        #endregion

        #region Методы

        public void ValidateInvariants()
        {
            var line = this;
            int lineStartOffset = line.DocumentLine.Offset;
            int lineEndOffset = line.DocumentLine.EndOffset;
            for (int i = 0; i < line.Sections.Count; i++)
            {
                var s1 = line.Sections[i];
                if (s1.Offset < lineStartOffset || s1.Length < 0 || s1.Offset + s1.Length > lineEndOffset)
                {
                    throw new InvalidOperationException("Section is outside line bounds");
                }
                for (int j = i + 1; j < line.Sections.Count; j++)
                {
                    var s2 = line.Sections[j];
                    if (s2.Offset >= s1.Offset + s1.Length)
                    {
                    }
                    else if (s2.Offset >= s1.Offset && s2.Offset + s2.Length <= s1.Offset + s1.Length)
                    {
                    }
                    else
                    {
                        throw new InvalidOperationException("Sections are overlapping or incorrectly sorted.");
                    }
                }
            }
        }

        public void MergeWith(HighlightedLine additionalLine)
        {
            if (additionalLine == null)
            {
                return;
            }
#if DEBUG
            ValidateInvariants();
            additionalLine.ValidateInvariants();
#endif

            int pos = 0;
            var activeSectionEndOffsets = new Stack<int>();
            int lineEndOffset = DocumentLine.EndOffset;
            activeSectionEndOffsets.Push(lineEndOffset);
            foreach (HighlightedSection newSection in additionalLine.Sections)
            {
                int newSectionStart = newSection.Offset;

                while (pos < Sections.Count)
                {
                    var s = Sections[pos];
                    if (newSection.Offset < s.Offset)
                    {
                        break;
                    }
                    while (s.Offset > activeSectionEndOffsets.Peek())
                    {
                        activeSectionEndOffsets.Pop();
                    }
                    activeSectionEndOffsets.Push(s.Offset + s.Length);
                    pos++;
                }

                var insertionStack = new Stack<int>(activeSectionEndOffsets.Reverse());

                int i;
                for (i = pos; i < Sections.Count; i++)
                {
                    var s = Sections[i];
                    if (newSection.Offset + newSection.Length <= s.Offset)
                    {
                        break;
                    }

                    Insert(ref i, ref newSectionStart, s.Offset, newSection.Color, insertionStack);

                    while (s.Offset > insertionStack.Peek())
                    {
                        insertionStack.Pop();
                    }
                    insertionStack.Push(s.Offset + s.Length);
                }
                Insert(ref i, ref newSectionStart, newSection.Offset + newSection.Length, newSection.Color, insertionStack);
            }

#if DEBUG
            ValidateInvariants();
#endif
        }

        private void Insert(ref int pos, ref int newSectionStart, int insertionEndPos, HighlightingColor color, Stack<int> insertionStack)
        {
            if (newSectionStart >= insertionEndPos)
            {
                return;
            }

            while (insertionStack.Peek() <= newSectionStart)
            {
                insertionStack.Pop();
            }
            while (insertionStack.Peek() < insertionEndPos)
            {
                int end = insertionStack.Pop();

                if (end > newSectionStart)
                {
                    Sections.Insert(pos++, new HighlightedSection
                    {
                        Offset = newSectionStart,
                        Length = end - newSectionStart,
                        Color = color
                    });
                    newSectionStart = end;
                }
            }
            if (insertionEndPos > newSectionStart)
            {
                Sections.Insert(pos++, new HighlightedSection
                {
                    Offset = newSectionStart,
                    Length = insertionEndPos - newSectionStart,
                    Color = color
                });
                newSectionStart = insertionEndPos;
            }
        }

        internal void WriteTo(RichTextWriter writer)
        {
            int startOffset = DocumentLine.Offset;
            WriteTo(writer, startOffset, startOffset + DocumentLine.Length);
        }

        internal void WriteTo(RichTextWriter writer, int startOffset, int endOffset)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            int documentLineStartOffset = DocumentLine.Offset;
            int documentLineEndOffset = documentLineStartOffset + DocumentLine.Length;
            if (startOffset < documentLineStartOffset || startOffset > documentLineEndOffset)
            {
                throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between " + documentLineStartOffset + " and " + documentLineEndOffset);
            }
            if (endOffset < startOffset || endOffset > documentLineEndOffset)
            {
                throw new ArgumentOutOfRangeException("endOffset", endOffset, "Value must be between startOffset and " + documentLineEndOffset);
            }
            ISegment requestedSegment = new SimpleSegment(startOffset, endOffset - startOffset);

            var elements = new List<HtmlElement>();
            for (int i = 0; i < Sections.Count; i++)
            {
                var s = Sections[i];
                if (SimpleSegment.GetOverlap(s, requestedSegment).Length > 0)
                {
                    elements.Add(new HtmlElement(s.Offset, i, false, s.Color));
                    elements.Add(new HtmlElement(s.Offset + s.Length, i, true, s.Color));
                }
            }
            elements.Sort();

            var document = Document;
            int textOffset = startOffset;
            foreach (HtmlElement e in elements)
            {
                int newOffset = Math.Min(e.Offset, endOffset);
                if (newOffset > startOffset)
                {
                    document.WriteTextTo(writer, textOffset, newOffset - textOffset);
                }
                textOffset = Math.Max(textOffset, newOffset);
                if (e.IsEnd)
                {
                    writer.EndSpan();
                }
                else
                {
                    writer.BeginSpan(e.Color);
                }
            }
            document.WriteTextTo(writer, textOffset, endOffset - textOffset);
        }

        public string ToHtml(HtmlOptions options = null)
        {
            var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
            using (var htmlWriter = new HtmlRichTextWriter(stringWriter, options))
            {
                WriteTo(htmlWriter);
            }
            return stringWriter.ToString();
        }

        public string ToHtml(int startOffset, int endOffset, HtmlOptions options = null)
        {
            var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
            using (var htmlWriter = new HtmlRichTextWriter(stringWriter, options))
            {
                WriteTo(htmlWriter, startOffset, endOffset);
            }
            return stringWriter.ToString();
        }

        public override string ToString()
        {
            return "[" + GetType().Name + " " + ToHtml() + "]";
        }

        [Obsolete("Use ToRichText() instead")]
        public HighlightedInlineBuilder ToInlineBuilder()
        {
            var builder = new HighlightedInlineBuilder(Document.GetText(DocumentLine));
            int startOffset = DocumentLine.Offset;
            foreach (HighlightedSection section in Sections)
            {
                builder.SetHighlighting(section.Offset - startOffset, section.Length, section.Color);
            }
            return builder;
        }

        public RichTextModel ToRichTextModel()
        {
            var builder = new RichTextModel();
            int startOffset = DocumentLine.Offset;
            foreach (HighlightedSection section in Sections)
            {
                builder.ApplyHighlighting(section.Offset - startOffset, section.Length, section.Color);
            }
            return builder;
        }

        public RichText ToRichText()
        {
            return new RichText(Document.GetText(DocumentLine), ToRichTextModel());
        }

        #endregion

        #region Вложенный класс: HtmlElement

        private sealed class HtmlElement : IComparable<HtmlElement>
        {
            #region Поля

            internal readonly HighlightingColor Color;

            internal readonly bool IsEnd;

            internal readonly int Nesting;

            internal readonly int Offset;

            #endregion

            #region Конструктор

            public HtmlElement(int offset, int nesting, bool isEnd, HighlightingColor color)
            {
                Offset = offset;
                Nesting = nesting;
                IsEnd = isEnd;
                Color = color;
            }

            #endregion

            #region Методы

            public int CompareTo(HtmlElement other)
            {
                int r = Offset.CompareTo(other.Offset);
                if (r != 0)
                {
                    return r;
                }
                if (IsEnd != other.IsEnd)
                {
                    if (IsEnd)
                    {
                        return -1;
                    }
                    return 1;
                }
                if (IsEnd)
                {
                    return other.Nesting.CompareTo(Nesting);
                }
                return Nesting.CompareTo(other.Nesting);
            }

            #endregion
        }

        #endregion
    }
}