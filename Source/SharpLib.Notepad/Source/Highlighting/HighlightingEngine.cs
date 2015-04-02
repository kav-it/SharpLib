using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using SharpLib.Notepad.Utils;
using SharpLib.Notepad.Document;

using SpanStack = SharpLib.Notepad.Utils.ImmutableStack<SharpLib.Notepad.Highlighting.HighlightingSpan>;

namespace SharpLib.Notepad.Highlighting
{
    public class HighlightingEngine
    {
        #region Поля

        private static readonly HighlightingRuleSet emptyRuleSet = new HighlightingRuleSet
        {
            Name = "EmptyRuleSet"
        };

        private readonly HighlightingRuleSet mainRuleSet;

        private HighlightedLine highlightedLine;

        private Stack<HighlightedSection> highlightedSectionStack;

        private HighlightedSection lastPoppedSection;

        private int lineStartOffset;

        private string lineText;

        private int position;

        private SpanStack spanStack = SpanStack.Empty;

        #endregion

        #region Свойства

        public SpanStack CurrentSpanStack
        {
            get { return spanStack; }
            set { spanStack = value ?? SpanStack.Empty; }
        }

        private HighlightingRuleSet CurrentRuleSet
        {
            get
            {
                if (spanStack.IsEmpty)
                {
                    return mainRuleSet;
                }
                return spanStack.Peek().RuleSet ?? emptyRuleSet;
            }
        }

        #endregion

        #region Конструктор

        public HighlightingEngine(HighlightingRuleSet mainRuleSet)
        {
            if (mainRuleSet == null)
            {
                throw new ArgumentNullException("mainRuleSet");
            }
            this.mainRuleSet = mainRuleSet;
        }

        #endregion

        #region Методы

        public HighlightedLine HighlightLine(IDocument document, IDocumentLine line)
        {
            lineStartOffset = line.Offset;
            lineText = document.GetText(line);
            try
            {
                highlightedLine = new HighlightedLine(document, line);
                HighlightLineInternal();
                return highlightedLine;
            }
            finally
            {
                highlightedLine = null;
                lineText = null;
                lineStartOffset = 0;
            }
        }

        public void ScanLine(IDocument document, IDocumentLine line)
        {
            lineText = document.GetText(line);
            try
            {
                Debug.Assert(highlightedLine == null);
                HighlightLineInternal();
            }
            finally
            {
                lineText = null;
            }
        }

        private void HighlightLineInternal()
        {
            position = 0;
            ResetColorStack();
            var currentRuleSet = CurrentRuleSet;
            var storedMatchArrays = new Stack<Match[]>();
            var matches = AllocateMatchArray(currentRuleSet.Spans.Count);
            Match endSpanMatch = null;

            while (true)
            {
                for (int i = 0; i < matches.Length; i++)
                {
                    if (matches[i] == null || (matches[i].Success && matches[i].Index < position))
                    {
                        matches[i] = currentRuleSet.Spans[i].StartExpression.Match(lineText, position);
                    }
                }
                if (endSpanMatch == null && !spanStack.IsEmpty)
                {
                    endSpanMatch = spanStack.Peek().EndExpression.Match(lineText, position);
                }

                var firstMatch = Minimum(matches, endSpanMatch);
                if (firstMatch == null)
                {
                    break;
                }

                HighlightNonSpans(firstMatch.Index);

                Debug.Assert(position == firstMatch.Index);

                if (firstMatch == endSpanMatch)
                {
                    var poppedSpan = spanStack.Peek();
                    if (!poppedSpan.SpanColorIncludesEnd)
                    {
                        PopColor();
                    }
                    PushColor(poppedSpan.EndColor);
                    position = firstMatch.Index + firstMatch.Length;
                    PopColor();
                    if (poppedSpan.SpanColorIncludesEnd)
                    {
                        PopColor();
                    }
                    spanStack = spanStack.Pop();
                    currentRuleSet = CurrentRuleSet;

                    if (storedMatchArrays.Count > 0)
                    {
                        matches = storedMatchArrays.Pop();
                        int index = currentRuleSet.Spans.IndexOf(poppedSpan);
                        Debug.Assert(index >= 0 && index < matches.Length);
                        if (matches[index].Index == position)
                        {
                            throw new InvalidOperationException(
                                "A highlighting span matched 0 characters, which would cause an endless loop.\n" +
                                "Change the highlighting definition so that either the start or the end regex matches at least one character.\n" +
                                "Start regex: " + poppedSpan.StartExpression + "\n" +
                                "End regex: " + poppedSpan.EndExpression);
                        }
                    }
                    else
                    {
                        matches = AllocateMatchArray(currentRuleSet.Spans.Count);
                    }
                }
                else
                {
                    int index = Array.IndexOf(matches, firstMatch);
                    Debug.Assert(index >= 0);
                    var newSpan = currentRuleSet.Spans[index];
                    spanStack = spanStack.Push(newSpan);
                    currentRuleSet = CurrentRuleSet;
                    storedMatchArrays.Push(matches);
                    matches = AllocateMatchArray(currentRuleSet.Spans.Count);
                    if (newSpan.SpanColorIncludesStart)
                    {
                        PushColor(newSpan.SpanColor);
                    }
                    PushColor(newSpan.StartColor);
                    position = firstMatch.Index + firstMatch.Length;
                    PopColor();
                    if (!newSpan.SpanColorIncludesStart)
                    {
                        PushColor(newSpan.SpanColor);
                    }
                }
                endSpanMatch = null;
            }
            HighlightNonSpans(lineText.Length);

            PopAllColors();
        }

        private void HighlightNonSpans(int until)
        {
            Debug.Assert(position <= until);
            if (position == until)
            {
                return;
            }
            if (highlightedLine != null)
            {
                var rules = CurrentRuleSet.Rules;
                var matches = AllocateMatchArray(rules.Count);
                while (true)
                {
                    for (int i = 0; i < matches.Length; i++)
                    {
                        if (matches[i] == null || (matches[i].Success && matches[i].Index < position))
                        {
                            matches[i] = rules[i].Regex.Match(lineText, position, until - position);
                        }
                    }
                    var firstMatch = Minimum(matches, null);
                    if (firstMatch == null)
                    {
                        break;
                    }

                    position = firstMatch.Index;
                    int ruleIndex = Array.IndexOf(matches, firstMatch);
                    if (firstMatch.Length == 0)
                    {
                        throw new InvalidOperationException(
                            "A highlighting rule matched 0 characters, which would cause an endless loop.\n" +
                            "Change the highlighting definition so that the rule matches at least one character.\n" +
                            "Regex: " + rules[ruleIndex].Regex);
                    }
                    PushColor(rules[ruleIndex].Color);
                    position = firstMatch.Index + firstMatch.Length;
                    PopColor();
                }
            }
            position = until;
        }

        private void ResetColorStack()
        {
            Debug.Assert(position == 0);
            lastPoppedSection = null;
            if (highlightedLine == null)
            {
                highlightedSectionStack = null;
            }
            else
            {
                highlightedSectionStack = new Stack<HighlightedSection>();
                foreach (HighlightingSpan span in spanStack.Reverse())
                {
                    PushColor(span.SpanColor);
                }
            }
        }

        private void PushColor(HighlightingColor color)
        {
            if (highlightedLine == null)
            {
                return;
            }
            if (color == null)
            {
                highlightedSectionStack.Push(null);
            }
            else if (lastPoppedSection != null && lastPoppedSection.Color == color
                     && lastPoppedSection.Offset + lastPoppedSection.Length == position + lineStartOffset)
            {
                highlightedSectionStack.Push(lastPoppedSection);
                lastPoppedSection = null;
            }
            else
            {
                var hs = new HighlightedSection
                {
                    Offset = position + lineStartOffset,
                    Color = color
                };
                highlightedLine.Sections.Add(hs);
                highlightedSectionStack.Push(hs);
                lastPoppedSection = null;
            }
        }

        private void PopColor()
        {
            if (highlightedLine == null)
            {
                return;
            }
            var s = highlightedSectionStack.Pop();
            if (s != null)
            {
                s.Length = (position + lineStartOffset) - s.Offset;
                if (s.Length == 0)
                {
                    highlightedLine.Sections.Remove(s);
                }
                else
                {
                    lastPoppedSection = s;
                }
            }
        }

        private void PopAllColors()
        {
            if (highlightedSectionStack != null)
            {
                while (highlightedSectionStack.Count > 0)
                {
                    PopColor();
                }
            }
        }

        private static Match Minimum(Match[] arr, Match endSpanMatch)
        {
            Match min = null;
            foreach (Match v in arr)
            {
                if (v.Success && (min == null || v.Index < min.Index))
                {
                    min = v;
                }
            }
            if (endSpanMatch != null && endSpanMatch.Success && (min == null || endSpanMatch.Index < min.Index))
            {
                return endSpanMatch;
            }
            return min;
        }

        private static Match[] AllocateMatchArray(int count)
        {
            if (count == 0)
            {
                return Empty<Match>.Array;
            }
            return new Match[count];
        }

        #endregion
    }
}