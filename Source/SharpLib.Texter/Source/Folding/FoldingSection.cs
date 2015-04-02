using System;
using System.Diagnostics;
using System.Text;

using SharpLib.Texter.Document;
using SharpLib.Texter.Rendering;
using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Folding
{
    internal sealed class FoldingSection : TextSegment
    {
        #region Поля

        private readonly FoldingManager manager;

        internal CollapsedLineSection[] collapsedSections;

        private bool isFolded;

        private string title;

        #endregion

        #region Свойства

        public bool IsFolded
        {
            get { return isFolded; }
            set
            {
                if (isFolded != value)
                {
                    isFolded = value;
                    ValidateCollapsedLineSections();
                    manager.Redraw(this);
                }
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    if (IsFolded)
                    {
                        manager.Redraw(this);
                    }
                }
            }
        }

        public string TextContent
        {
            get { return manager.document.GetText(StartOffset, EndOffset - StartOffset); }
        }

        [Obsolete]
        public string TooltipText
        {
            get
            {
                var startLine = manager.document.GetLineByOffset(StartOffset);
                var endLine = manager.document.GetLineByOffset(EndOffset);
                var builder = new StringBuilder();

                var current = startLine;
                var startIndent = TextUtilities.GetLeadingWhitespace(manager.document, startLine);

                while (current != endLine.NextLine)
                {
                    var currentIndent = TextUtilities.GetLeadingWhitespace(manager.document, current);

                    if (current == startLine && current == endLine)
                    {
                        builder.Append(manager.document.GetText(StartOffset, EndOffset - StartOffset));
                    }
                    else if (current == startLine)
                    {
                        if (current.EndOffset - StartOffset > 0)
                        {
                            builder.AppendLine(manager.document.GetText(StartOffset, current.EndOffset - StartOffset).TrimStart());
                        }
                    }
                    else if (current == endLine)
                    {
                        if (startIndent.Length <= currentIndent.Length)
                        {
                            builder.Append(manager.document.GetText(current.Offset + startIndent.Length, EndOffset - current.Offset - startIndent.Length));
                        }
                        else
                        {
                            builder.Append(manager.document.GetText(current.Offset + currentIndent.Length, EndOffset - current.Offset - currentIndent.Length));
                        }
                    }
                    else
                    {
                        if (startIndent.Length <= currentIndent.Length)
                        {
                            builder.AppendLine(manager.document.GetText(current.Offset + startIndent.Length, current.Length - startIndent.Length));
                        }
                        else
                        {
                            builder.AppendLine(manager.document.GetText(current.Offset + currentIndent.Length, current.Length - currentIndent.Length));
                        }
                    }

                    current = current.NextLine;
                }

                return builder.ToString();
            }
        }

        public object Tag { get; set; }

        #endregion

        #region Конструктор

        internal FoldingSection(FoldingManager manager, int startOffset, int endOffset)
        {
            Debug.Assert(manager != null);
            this.manager = manager;
            StartOffset = startOffset;
            Length = endOffset - startOffset;
        }

        #endregion

        #region Методы

        internal void ValidateCollapsedLineSections()
        {
            if (!isFolded)
            {
                RemoveCollapsedLineSection();
                return;
            }

            var startLine = manager.document.GetLineByOffset(StartOffset.CoerceValue(0, manager.document.TextLength));
            var endLine = manager.document.GetLineByOffset(EndOffset.CoerceValue(0, manager.document.TextLength));
            if (startLine == endLine)
            {
                RemoveCollapsedLineSection();
            }
            else
            {
                if (collapsedSections == null)
                {
                    collapsedSections = new CollapsedLineSection[manager.textViews.Count];
                }

                var startLinePlusOne = startLine.NextLine;
                for (int i = 0; i < collapsedSections.Length; i++)
                {
                    var collapsedSection = collapsedSections[i];
                    if (collapsedSection == null || collapsedSection.Start != startLinePlusOne || collapsedSection.End != endLine)
                    {
                        if (collapsedSection != null)
                        {
                            Debug.WriteLine("CollapsedLineSection validation - recreate collapsed section from " + startLinePlusOne + " to " + endLine);
                            collapsedSection.Uncollapse();
                        }
                        collapsedSections[i] = manager.textViews[i].CollapseLines(startLinePlusOne, endLine);
                    }
                }
            }
        }

        protected override void OnSegmentChanged()
        {
            ValidateCollapsedLineSections();
            base.OnSegmentChanged();

            if (IsConnectedToCollection)
            {
                manager.Redraw(this);
            }
        }

        private void RemoveCollapsedLineSection()
        {
            if (collapsedSections != null)
            {
                foreach (var collapsedSection in collapsedSections)
                {
                    if (collapsedSection != null && collapsedSection.Start != null)
                    {
                        collapsedSection.Uncollapse();
                    }
                }
                collapsedSections = null;
            }
        }

        #endregion
    }
}