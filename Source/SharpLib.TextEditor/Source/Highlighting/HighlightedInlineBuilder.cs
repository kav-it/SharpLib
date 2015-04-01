using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting
{
    [Obsolete("Use RichText / RichTextModel instead")]
    public sealed class HighlightedInlineBuilder
    {
        #region Поля

        private readonly List<int> stateChangeOffsets = new List<int>();

        private readonly List<HighlightingColor> stateChanges = new List<HighlightingColor>();

        private readonly string text;

        #endregion

        #region Свойства

        public string Text
        {
            get { return text; }
        }

        #endregion

        #region Конструктор

        public HighlightedInlineBuilder(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            this.text = text;
            stateChangeOffsets.Add(0);
            stateChanges.Add(new HighlightingColor());
        }

        public HighlightedInlineBuilder(RichText text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            this.text = text.Text;
            stateChangeOffsets.AddRange(text.stateChangeOffsets);
            stateChanges.AddRange(text.stateChanges);
        }

        private HighlightedInlineBuilder(string text, List<int> offsets, List<HighlightingColor> states)
        {
            this.text = text;
            stateChangeOffsets = offsets;
            stateChanges = states;
        }

        #endregion

        #region Методы

        private static HighlightingBrush MakeBrush(Brush b)
        {
            var scb = b as SolidColorBrush;
            if (scb != null)
            {
                return new SimpleHighlightingBrush(scb);
            }
            return null;
        }

        private int GetIndexForOffset(int offset)
        {
            if (offset < 0 || offset > text.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            int index = stateChangeOffsets.BinarySearch(offset);
            if (index < 0)
            {
                index = ~index;
                if (offset < text.Length)
                {
                    stateChanges.Insert(index, stateChanges[index - 1].Clone());
                    stateChangeOffsets.Insert(index, offset);
                }
            }
            return index;
        }

        public void SetHighlighting(int offset, int length, HighlightingColor color)
        {
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            if (color.Foreground == null && color.Background == null && color.FontStyle == null && color.FontWeight == null && color.Underline == null)
            {
                return;
            }
            int startIndex = GetIndexForOffset(offset);
            int endIndex = GetIndexForOffset(offset + length);
            for (int i = startIndex; i < endIndex; i++)
            {
                stateChanges[i].MergeWith(color);
            }
        }

        public void SetForeground(int offset, int length, Brush brush)
        {
            int startIndex = GetIndexForOffset(offset);
            int endIndex = GetIndexForOffset(offset + length);
            var hbrush = MakeBrush(brush);
            for (int i = startIndex; i < endIndex; i++)
            {
                stateChanges[i].Foreground = hbrush;
            }
        }

        public void SetBackground(int offset, int length, Brush brush)
        {
            int startIndex = GetIndexForOffset(offset);
            int endIndex = GetIndexForOffset(offset + length);
            var hbrush = MakeBrush(brush);
            for (int i = startIndex; i < endIndex; i++)
            {
                stateChanges[i].Background = hbrush;
            }
        }

        public void SetFontWeight(int offset, int length, FontWeight weight)
        {
            int startIndex = GetIndexForOffset(offset);
            int endIndex = GetIndexForOffset(offset + length);
            for (int i = startIndex; i < endIndex; i++)
            {
                stateChanges[i].FontWeight = weight;
            }
        }

        public void SetFontStyle(int offset, int length, FontStyle style)
        {
            int startIndex = GetIndexForOffset(offset);
            int endIndex = GetIndexForOffset(offset + length);
            for (int i = startIndex; i < endIndex; i++)
            {
                stateChanges[i].FontStyle = style;
            }
        }

        public Run[] CreateRuns()
        {
            return ToRichText().CreateRuns();
        }

        public RichText ToRichText()
        {
            return new RichText(text, stateChangeOffsets.ToArray(), stateChanges.Select(FreezableHelper.GetFrozenClone).ToArray());
        }

        public HighlightedInlineBuilder Clone()
        {
            return new HighlightedInlineBuilder(text,
                stateChangeOffsets.ToList(),
                stateChanges.Select(sc => sc.Clone()).ToList());
        }

        #endregion
    }
}