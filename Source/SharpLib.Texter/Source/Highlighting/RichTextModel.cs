using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Highlighting
{
    public sealed class RichTextModel
    {
        #region Поля

        private readonly List<int> stateChangeOffsets = new List<int>();

        private readonly List<HighlightingColor> stateChanges = new List<HighlightingColor>();

        #endregion

        #region Конструктор

        public RichTextModel()
        {
            stateChangeOffsets.Add(0);
            stateChanges.Add(new HighlightingColor());
        }

        internal RichTextModel(int[] stateChangeOffsets, HighlightingColor[] stateChanges)
        {
            Debug.Assert(stateChangeOffsets[0] == 0);
            this.stateChangeOffsets.AddRange(stateChangeOffsets);
            this.stateChanges.AddRange(stateChanges);
        }

        #endregion

        #region Методы

        private int GetIndexForOffset(int offset)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            int index = stateChangeOffsets.BinarySearch(offset);
            if (index < 0)
            {
                index = ~index;
                stateChanges.Insert(index, stateChanges[index - 1].Clone());
                stateChangeOffsets.Insert(index, offset);
            }
            return index;
        }

        private int GetIndexForOffsetUseExistingSegment(int offset)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            int index = stateChangeOffsets.BinarySearch(offset);
            if (index < 0)
            {
                index = ~index - 1;
            }
            return index;
        }

        private int GetEnd(int index)
        {
            if (index + 1 < stateChangeOffsets.Count)
            {
                return stateChangeOffsets[index + 1];
            }
            return int.MaxValue;
        }

        public void UpdateOffsets(TextChangeEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            UpdateOffsets(e.GetNewOffset);
        }

        public void UpdateOffsets(OffsetChangeMap change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }
            UpdateOffsets(change.GetNewOffset);
        }

        public void UpdateOffsets(OffsetChangeMapEntry change)
        {
            UpdateOffsets(change.GetNewOffset);
        }

        private void UpdateOffsets(Func<int, AnchorMovementType, int> updateOffset)
        {
            int readPos = 1;
            int writePos = 1;
            while (readPos < stateChangeOffsets.Count)
            {
                Debug.Assert(writePos <= readPos);
                int newOffset = updateOffset(stateChangeOffsets[readPos], AnchorMovementType.Default);
                if (newOffset == stateChangeOffsets[writePos - 1])
                {
                    stateChanges[writePos - 1] = stateChanges[readPos];
                }
                else
                {
                    stateChangeOffsets[writePos] = newOffset;
                    stateChanges[writePos] = stateChanges[readPos];
                    writePos++;
                }
                readPos++;
            }

            stateChangeOffsets.RemoveRange(writePos, stateChangeOffsets.Count - writePos);
            stateChanges.RemoveRange(writePos, stateChanges.Count - writePos);
        }

        internal void Append(int offset, int[] newOffsets, HighlightingColor[] newColors)
        {
            Debug.Assert(newOffsets.Length == newColors.Length);
            Debug.Assert(newOffsets[0] == 0);

            while (stateChangeOffsets.Count > 0 && stateChangeOffsets.Last() <= offset)
            {
                stateChangeOffsets.RemoveAt(stateChangeOffsets.Count - 1);
                stateChanges.RemoveAt(stateChanges.Count - 1);
            }

            for (int i = 0; i < newOffsets.Length; i++)
            {
                stateChangeOffsets.Add(offset + newOffsets[i]);
                stateChanges.Add(newColors[i]);
            }
        }

        public HighlightingColor GetHighlightingAt(int offset)
        {
            return stateChanges[GetIndexForOffsetUseExistingSegment(offset)].Clone();
        }

        public void ApplyHighlighting(int offset, int length, HighlightingColor color)
        {
            if (color == null || color.IsEmptyForMerge)
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

        public void SetHighlighting(int offset, int length, HighlightingColor color)
        {
            if (length <= 0)
            {
                return;
            }
            int startIndex = GetIndexForOffset(offset);
            int endIndex = GetIndexForOffset(offset + length);
            stateChanges[startIndex] = color != null ? color.Clone() : new HighlightingColor();
            stateChanges.RemoveRange(startIndex + 1, endIndex - (startIndex + 1));
            stateChangeOffsets.RemoveRange(startIndex + 1, endIndex - (startIndex + 1));
        }

        public void SetForeground(int offset, int length, HighlightingBrush brush)
        {
            int startIndex = GetIndexForOffset(offset);
            int endIndex = GetIndexForOffset(offset + length);
            for (int i = startIndex; i < endIndex; i++)
            {
                stateChanges[i].Foreground = brush;
            }
        }

        public void SetBackground(int offset, int length, HighlightingBrush brush)
        {
            int startIndex = GetIndexForOffset(offset);
            int endIndex = GetIndexForOffset(offset + length);
            for (int i = startIndex; i < endIndex; i++)
            {
                stateChanges[i].Background = brush;
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

        public IEnumerable<HighlightedSection> GetHighlightedSections(int offset, int length)
        {
            int index = GetIndexForOffsetUseExistingSegment(offset);
            int pos = offset;
            int endOffset = offset + length;
            while (pos < endOffset)
            {
                int endPos = Math.Min(endOffset, GetEnd(index));
                yield return new HighlightedSection
                {
                    Offset = pos,
                    Length = endPos - pos,
                    Color = stateChanges[index].Clone()
                };
                pos = endPos;
                index++;
            }
        }

        public Run[] CreateRuns(ITextSource textSource)
        {
            var runs = new Run[stateChanges.Count];
            for (int i = 0; i < runs.Length; i++)
            {
                int startOffset = stateChangeOffsets[i];
                int endOffset = i + 1 < stateChangeOffsets.Count ? stateChangeOffsets[i + 1] : textSource.TextLength;
                var r = new Run(textSource.GetText(startOffset, endOffset - startOffset));
                var state = stateChanges[i];
                RichText.ApplyColorToTextElement(r, state);
                runs[i] = r;
            }
            return runs;
        }

        #endregion
    }
}