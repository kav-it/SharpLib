using System;
using System.Collections.Generic;
using System.Windows;

using SharpLib.Texter.Document;
using SharpLib.Texter.Editing;

namespace SharpLib.Texter.Snippets
{
    public class InsertionContext : IWeakEventListener
    {
        #region Перечисления

        private enum Status
        {
            Insertion,

            RaisingInsertionCompleted,

            Interactive,

            RaisingDeactivated,

            Deactivated
        }

        #endregion

        #region Поля

        private readonly Dictionary<SnippetElement, IActiveElement> elementMap = new Dictionary<SnippetElement, IActiveElement>();

        private readonly List<IActiveElement> registeredElements = new List<IActiveElement>();

        private readonly int startPosition;

        private Status currentStatus = Status.Insertion;

        private bool deactivateIfSnippetEmpty;

        private SnippetInputHandler myInputHandler;

        private SharpLib.Texter.Document.AnchorSegment wholeSnippetAnchor;

        #endregion

        #region Свойства

        public TextArea TextArea { get; private set; }

        public SharpLib.Texter.Document.TextDocument Document { get; private set; }

        public string SelectedText { get; private set; }

        public string Indentation { get; private set; }

        public string Tab { get; private set; }

        public string LineTerminator { get; private set; }

        public int InsertionPosition { get; set; }

        public int StartPosition
        {
            get
            {
                if (wholeSnippetAnchor != null)
                {
                    return wholeSnippetAnchor.Offset;
                }
                return startPosition;
            }
        }

        public IEnumerable<IActiveElement> ActiveElements
        {
            get { return registeredElements; }
        }

        #endregion

        #region События

        public event EventHandler<SnippetEventArgs> Deactivated;

        public event EventHandler InsertionCompleted;

        #endregion

        #region Конструктор

        public InsertionContext(TextArea textArea, int insertionPosition)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            TextArea = textArea;
            Document = textArea.Document;
            SelectedText = textArea.Selection.GetText();
            InsertionPosition = insertionPosition;
            startPosition = insertionPosition;

            var startLine = Document.GetLineByOffset(insertionPosition);
            var indentation = TextUtilities.GetWhitespaceAfter(Document, startLine.Offset);
            Indentation = Document.GetText(indentation.Offset, Math.Min(indentation.EndOffset, insertionPosition) - indentation.Offset);
            Tab = textArea.Options.IndentationString;

            LineTerminator = TextUtilities.GetNewLineFromDocument(Document, startLine.LineNumber);
        }

        #endregion

        #region Методы

        public void InsertText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (currentStatus != Status.Insertion)
            {
                throw new InvalidOperationException();
            }

            text = text.Replace("\t", Tab);

            using (Document.RunUpdate())
            {
                int textOffset = 0;
                SimpleSegment segment;
                while ((segment = NewLineFinder.NextNewLine(text, textOffset)) != SimpleSegment.Invalid)
                {
                    string insertString = text.Substring(textOffset, segment.Offset - textOffset)
                                          + LineTerminator + Indentation;
                    Document.Insert(InsertionPosition, insertString);
                    InsertionPosition += insertString.Length;
                    textOffset = segment.EndOffset;
                }
                string remainingInsertString = text.Substring(textOffset);
                Document.Insert(InsertionPosition, remainingInsertString);
                InsertionPosition += remainingInsertString.Length;
            }
        }

        public void RegisterActiveElement(SnippetElement owner, IActiveElement element)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (currentStatus != Status.Insertion)
            {
                throw new InvalidOperationException();
            }
            elementMap.Add(owner, element);
            registeredElements.Add(element);
        }

        public IActiveElement GetActiveElement(SnippetElement owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            IActiveElement element;
            if (elementMap.TryGetValue(owner, out element))
            {
                return element;
            }
            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "There is an event and this method is raising it.")]
        public void RaiseInsertionCompleted(EventArgs e)
        {
            if (currentStatus != Status.Insertion)
            {
                throw new InvalidOperationException();
            }
            if (e == null)
            {
                e = EventArgs.Empty;
            }

            currentStatus = Status.RaisingInsertionCompleted;
            int endPosition = InsertionPosition;
            wholeSnippetAnchor = new AnchorSegment(Document, startPosition, endPosition - startPosition);
            TextDocumentWeakEventManager.UpdateFinished.AddListener(Document, this);
            deactivateIfSnippetEmpty = (endPosition != startPosition);

            foreach (IActiveElement element in registeredElements)
            {
                element.OnInsertionCompleted();
            }
            if (InsertionCompleted != null)
            {
                InsertionCompleted(this, e);
            }
            currentStatus = Status.Interactive;
            if (registeredElements.Count == 0)
            {
                Deactivate(new SnippetEventArgs(DeactivateReason.NoActiveElements));
            }
            else
            {
                myInputHandler = new SnippetInputHandler(this);

                foreach (TextAreaStackedInputHandler h in TextArea.StackedInputHandlers)
                {
                    if (h is SnippetInputHandler)
                    {
                        TextArea.PopStackedInputHandler(h);
                    }
                }
                TextArea.PushStackedInputHandler(myInputHandler);
            }
        }

        public void Deactivate(SnippetEventArgs e)
        {
            if (currentStatus == Status.Deactivated || currentStatus == Status.RaisingDeactivated)
            {
                return;
            }
            if (currentStatus != Status.Interactive)
            {
                throw new InvalidOperationException("Cannot call Deactivate() until RaiseInsertionCompleted() has finished.");
            }
            if (e == null)
            {
                e = new SnippetEventArgs(DeactivateReason.Unknown);
            }

            TextDocumentWeakEventManager.UpdateFinished.RemoveListener(Document, this);
            currentStatus = Status.RaisingDeactivated;
            TextArea.PopStackedInputHandler(myInputHandler);
            foreach (IActiveElement element in registeredElements)
            {
                element.Deactivate(e);
            }
            if (Deactivated != null)
            {
                Deactivated(this, e);
            }
            currentStatus = Status.Deactivated;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return ReceiveWeakEvent(managerType, sender, e);
        }

        protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(TextDocumentWeakEventManager.UpdateFinished))
            {
                if (wholeSnippetAnchor.Length == 0 && deactivateIfSnippetEmpty)
                {
                    Deactivate(new SnippetEventArgs(DeactivateReason.Deleted));
                }
                return true;
            }
            return false;
        }

        public void Link(ISegment mainElement, ISegment[] boundElements)
        {
            var main = new SnippetReplaceableTextElement
            {
                Text = Document.GetText(mainElement)
            };
            RegisterActiveElement(main, new ReplaceableActiveElement(this, mainElement.Offset, mainElement.EndOffset));
            foreach (var boundElement in boundElements)
            {
                var bound = new SnippetBoundElement
                {
                    TargetElement = main
                };
                var start = Document.CreateAnchor(boundElement.Offset);
                start.MovementType = AnchorMovementType.BeforeInsertion;
                start.SurviveDeletion = true;
                var end = Document.CreateAnchor(boundElement.EndOffset);
                end.MovementType = AnchorMovementType.BeforeInsertion;
                end.SurviveDeletion = true;

                RegisterActiveElement(bound, new BoundActiveElement(this, main, bound, new AnchorSegment(start, end)));
            }
        }

        #endregion
    }
}