using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using SharpLib.Texter.Document;
using SharpLib.Texter.Editing;
using SharpLib.Texter.Rendering;
using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Folding
{
    internal class FoldingManager : IWeakEventListener
    {
        #region Поля

        internal readonly TextDocument document;

        private readonly TextSegmentCollection<FoldingSection> foldings;

        internal readonly List<TextView> textViews = new List<TextView>();

        private bool isFirstUpdate = true;

        #endregion

        #region Свойства

        public IEnumerable<FoldingSection> AllFoldings
        {
            get { return foldings; }
        }

        #endregion

        #region Конструктор

        public FoldingManager(TextDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            this.document = document;
            foldings = new TextSegmentCollection<FoldingSection>();
            document.VerifyAccess();
            TextDocumentWeakEventManager.Changed.AddListener(document, this);
        }

        #endregion

        #region Методы

        protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(TextDocumentWeakEventManager.Changed))
            {
                OnDocumentChanged((DocumentChangeEventArgs)e);
                return true;
            }
            return false;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return ReceiveWeakEvent(managerType, sender, e);
        }

        private void OnDocumentChanged(DocumentChangeEventArgs e)
        {
            foldings.UpdateOffsets(e);
            int newEndOffset = e.Offset + e.InsertionLength;

            var endLine = document.GetLineByOffset(newEndOffset);
            newEndOffset = endLine.Offset + endLine.TotalLength;
            foreach (var affectedFolding in foldings.FindOverlappingSegments(e.Offset, newEndOffset - e.Offset))
            {
                if (affectedFolding.Length == 0)
                {
                    RemoveFolding(affectedFolding);
                }
                else
                {
                    affectedFolding.ValidateCollapsedLineSections();
                }
            }
        }

        internal void AddToTextView(TextView textView)
        {
            if (textView == null || textViews.Contains(textView))
            {
                throw new ArgumentException();
            }
            textViews.Add(textView);
            foreach (FoldingSection fs in foldings)
            {
                if (fs.collapsedSections != null)
                {
                    Array.Resize(ref fs.collapsedSections, textViews.Count);
                    fs.ValidateCollapsedLineSections();
                }
            }
        }

        internal void RemoveFromTextView(TextView textView)
        {
            int pos = textViews.IndexOf(textView);
            if (pos < 0)
            {
                throw new ArgumentException();
            }
            textViews.RemoveAt(pos);
            foreach (FoldingSection fs in foldings)
            {
                if (fs.collapsedSections != null)
                {
                    var c = new CollapsedLineSection[textViews.Count];
                    Array.Copy(fs.collapsedSections, 0, c, 0, pos);
                    fs.collapsedSections[pos].Uncollapse();
                    Array.Copy(fs.collapsedSections, pos + 1, c, pos, c.Length - pos);
                    fs.collapsedSections = c;
                }
            }
        }

        internal void Redraw()
        {
            foreach (TextView textView in textViews)
            {
                textView.Redraw();
            }
        }

        internal void Redraw(FoldingSection fs)
        {
            foreach (TextView textView in textViews)
            {
                textView.Redraw(fs);
            }
        }

        public FoldingSection CreateFolding(int startOffset, int endOffset)
        {
            if (startOffset >= endOffset)
            {
                throw new ArgumentException("startOffset must be less than endOffset");
            }
            if (startOffset < 0 || endOffset > document.TextLength)
            {
                throw new ArgumentException("Folding must be within document boundary");
            }
            var fs = new FoldingSection(this, startOffset, endOffset);
            foldings.Add(fs);
            Redraw(fs);
            return fs;
        }

        public void RemoveFolding(FoldingSection fs)
        {
            if (fs == null)
            {
                throw new ArgumentNullException("fs");
            }
            fs.IsFolded = false;
            foldings.Remove(fs);
            Redraw(fs);
        }

        public void Clear()
        {
            document.VerifyAccess();
            foreach (FoldingSection s in foldings)
            {
                s.IsFolded = false;
            }
            foldings.Clear();
            Redraw();
        }

        public int GetNextFoldedFoldingStart(int startOffset)
        {
            var fs = foldings.FindFirstSegmentWithStartAfter(startOffset);
            while (fs != null && !fs.IsFolded)
            {
                fs = foldings.GetNextSegment(fs);
            }
            return fs != null ? fs.StartOffset : -1;
        }

        public FoldingSection GetNextFolding(int startOffset)
        {
            return foldings.FindFirstSegmentWithStartAfter(startOffset);
        }

        public ReadOnlyCollection<FoldingSection> GetFoldingsAt(int startOffset)
        {
            var result = new List<FoldingSection>();
            var fs = foldings.FindFirstSegmentWithStartAfter(startOffset);
            while (fs != null && fs.StartOffset == startOffset)
            {
                result.Add(fs);
                fs = foldings.GetNextSegment(fs);
            }
            return result.AsReadOnly();
        }

        public ReadOnlyCollection<FoldingSection> GetFoldingsContaining(int offset)
        {
            return foldings.FindSegmentsContaining(offset);
        }

        public void UpdateFoldings(IEnumerable<NewFolding> newFoldings, int firstErrorOffset)
        {
            if (newFoldings == null)
            {
                throw new ArgumentNullException("newFoldings");
            }

            if (firstErrorOffset < 0)
            {
                firstErrorOffset = int.MaxValue;
            }

            var oldFoldings = AllFoldings.ToArray();
            int oldFoldingIndex = 0;
            int previousStartOffset = 0;

            foreach (NewFolding newFolding in newFoldings)
            {
                if (newFolding.StartOffset < previousStartOffset)
                {
                    throw new ArgumentException("newFoldings must be sorted by start offset");
                }
                previousStartOffset = newFolding.StartOffset;

                int startOffset = newFolding.StartOffset.CoerceValue(0, document.TextLength);
                int endOffset = newFolding.EndOffset.CoerceValue(0, document.TextLength);

                if (newFolding.StartOffset == newFolding.EndOffset)
                {
                    continue;
                }

                while (oldFoldingIndex < oldFoldings.Length && newFolding.StartOffset > oldFoldings[oldFoldingIndex].StartOffset)
                {
                    RemoveFolding(oldFoldings[oldFoldingIndex++]);
                }
                FoldingSection section;

                if (oldFoldingIndex < oldFoldings.Length && newFolding.StartOffset == oldFoldings[oldFoldingIndex].StartOffset)
                {
                    section = oldFoldings[oldFoldingIndex++];
                    section.Length = newFolding.EndOffset - newFolding.StartOffset;
                }
                else
                {
                    section = CreateFolding(newFolding.StartOffset, newFolding.EndOffset);

                    if (isFirstUpdate)
                    {
                        section.IsFolded = newFolding.DefaultClosed;
                        isFirstUpdate = false;
                    }
                    section.Tag = newFolding;
                }
                section.Title = newFolding.Name;
            }

            while (oldFoldingIndex < oldFoldings.Length)
            {
                var oldSection = oldFoldings[oldFoldingIndex++];
                if (oldSection.StartOffset >= firstErrorOffset)
                {
                    break;
                }
                RemoveFolding(oldSection);
            }
        }

        public static FoldingManager Install(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            return new FoldingManagerInstallation(textArea);
        }

        public static void Uninstall(FoldingManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            var installation = manager as FoldingManagerInstallation;
            if (installation != null)
            {
                installation.Uninstall();
            }
            else
            {
                throw new ArgumentException("FoldingManager was not created using FoldingManager.Install");
            }
        }

        #endregion

        #region Вложенный класс: FoldingManagerInstallation

        private sealed class FoldingManagerInstallation : FoldingManager
        {
            #region Поля

            private FoldingElementGenerator generator;

            private FoldingMargin margin;

            private TextArea textArea;

            #endregion

            #region Конструктор

            public FoldingManagerInstallation(TextArea textArea)
                : base(textArea.Document)
            {
                this.textArea = textArea;
                margin = new FoldingMargin
                {
                    FoldingManager = this
                };
                generator = new FoldingElementGenerator
                {
                    FoldingManager = this
                };
                textArea.LeftMargins.Add(margin);
                textArea.TextView.Services.AddService(typeof(FoldingManager), this);

                textArea.TextView.ElementGenerators.Insert(0, generator);
                textArea.Caret.PositionChanged += textArea_Caret_PositionChanged;
            }

            #endregion

            #region Методы

            public void Uninstall()
            {
                Clear();
                if (textArea != null)
                {
                    textArea.Caret.PositionChanged -= textArea_Caret_PositionChanged;
                    textArea.LeftMargins.Remove(margin);
                    textArea.TextView.ElementGenerators.Remove(generator);
                    textArea.TextView.Services.RemoveService(typeof(FoldingManager));
                    margin = null;
                    generator = null;
                    textArea = null;
                }
            }

            private void textArea_Caret_PositionChanged(object sender, EventArgs e)
            {
                int caretOffset = textArea.Caret.Offset;
                foreach (FoldingSection s in GetFoldingsContaining(caretOffset))
                {
                    if (s.IsFolded && s.StartOffset < caretOffset && caretOffset < s.EndOffset)
                    {
                        s.IsFolded = false;
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}