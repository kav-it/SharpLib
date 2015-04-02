using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using SharpLib.Texter.Document;
using SharpLib.Texter.Utils;

using SpanStack = SharpLib.Texter.Utils.ImmutableStack<SharpLib.Texter.Highlighting.HighlightingSpan>;

namespace SharpLib.Texter.Highlighting
{
    public class DocumentHighlighter : ILineTracker, IHighlighter
    {
        private readonly CompressingTreeList<SpanStack> storedSpanStacks = new CompressingTreeList<SpanStack>(object.ReferenceEquals);

        private readonly CompressingTreeList<bool> isValid = new CompressingTreeList<bool>((a, b) => a == b);

        private readonly IDocument document;

        private readonly IHighlightingDefinition definition;

        private readonly HighlightingEngine engine;

        private readonly WeakLineTracker weakLineTracker;

        private bool isHighlighting;

        private bool isInHighlightingGroup;

        private bool isDisposed;

        public IDocument Document
        {
            get { return document; }
        }

        public DocumentHighlighter(TextDocument document, IHighlightingDefinition definition)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
            this.document = document;
            this.definition = definition;
            engine = new HighlightingEngine(definition.MainRuleSet);
            document.VerifyAccess();
            weakLineTracker = WeakLineTracker.Register(document, this);
            InvalidateSpanStacks();
        }

        public void Dispose()
        {
            if (weakLineTracker != null)
            {
                weakLineTracker.Deregister();
            }
            isDisposed = true;
        }

        void ILineTracker.BeforeRemoveLine(DocumentLine line)
        {
            CheckIsHighlighting();
            int number = line.LineNumber;
            storedSpanStacks.RemoveAt(number);
            isValid.RemoveAt(number);
            if (number < isValid.Count)
            {
                isValid[number] = false;
                if (number < firstInvalidLine)
                {
                    firstInvalidLine = number;
                }
            }
        }

        void ILineTracker.SetLineLength(DocumentLine line, int newTotalLength)
        {
            CheckIsHighlighting();
            int number = line.LineNumber;
            isValid[number] = false;
            if (number < firstInvalidLine)
            {
                firstInvalidLine = number;
            }
        }

        void ILineTracker.LineInserted(DocumentLine insertionPos, DocumentLine newLine)
        {
            CheckIsHighlighting();
            Debug.Assert(insertionPos.LineNumber + 1 == newLine.LineNumber);
            int lineNumber = newLine.LineNumber;
            storedSpanStacks.Insert(lineNumber, null);
            isValid.Insert(lineNumber, false);
            if (lineNumber < firstInvalidLine)
            {
                firstInvalidLine = lineNumber;
            }
        }

        void ILineTracker.RebuildDocument()
        {
            InvalidateSpanStacks();
        }

        void ILineTracker.ChangeComplete(DocumentChangeEventArgs e)
        {
        }

        private ImmutableStack<HighlightingSpan> initialSpanStack = SpanStack.Empty;

        public ImmutableStack<HighlightingSpan> InitialSpanStack
        {
            get { return initialSpanStack; }
            set
            {
                initialSpanStack = value ?? SpanStack.Empty;
                InvalidateHighlighting();
            }
        }

        public void InvalidateHighlighting()
        {
            InvalidateSpanStacks();
            OnHighlightStateChanged(1, document.LineCount);
        }

        private void InvalidateSpanStacks()
        {
            CheckIsHighlighting();
            storedSpanStacks.Clear();
            storedSpanStacks.Add(initialSpanStack);
            storedSpanStacks.InsertRange(1, document.LineCount, null);
            isValid.Clear();
            isValid.Add(true);
            isValid.InsertRange(1, document.LineCount, false);
            firstInvalidLine = 1;
        }

        private int firstInvalidLine;

        public HighlightedLine HighlightLine(int lineNumber)
        {
            ThrowUtil.CheckInRangeInclusive(lineNumber, "lineNumber", 1, document.LineCount);
            CheckIsHighlighting();
            isHighlighting = true;
            try
            {
                HighlightUpTo(lineNumber - 1);
                var line = document.GetLineByNumber(lineNumber);
                var result = engine.HighlightLine(document, line);
                UpdateTreeList(lineNumber);
                return result;
            }
            finally
            {
                isHighlighting = false;
            }
        }

        public SpanStack GetSpanStack(int lineNumber)
        {
            ThrowUtil.CheckInRangeInclusive(lineNumber, "lineNumber", 0, document.LineCount);
            if (firstInvalidLine <= lineNumber)
            {
                UpdateHighlightingState(lineNumber);
            }
            return storedSpanStacks[lineNumber];
        }

        public IEnumerable<HighlightingColor> GetColorStack(int lineNumber)
        {
            return GetSpanStack(lineNumber).Select(s => s.SpanColor).Where(s => s != null);
        }

        private void CheckIsHighlighting()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("DocumentHighlighter");
            }
            if (isHighlighting)
            {
                throw new InvalidOperationException("Invalid call - a highlighting operation is currently running.");
            }
        }

        public void UpdateHighlightingState(int lineNumber)
        {
            CheckIsHighlighting();
            isHighlighting = true;
            try
            {
                HighlightUpTo(lineNumber);
            }
            finally
            {
                isHighlighting = false;
            }
        }

        private void HighlightUpTo(int targetLineNumber)
        {
            for (int currentLine = 0; currentLine <= targetLineNumber; currentLine++)
            {
                if (firstInvalidLine > currentLine)
                {
                    if (firstInvalidLine <= targetLineNumber)
                    {
                        engine.CurrentSpanStack = storedSpanStacks[firstInvalidLine - 1];
                        currentLine = firstInvalidLine;
                    }
                    else
                    {
                        engine.CurrentSpanStack = storedSpanStacks[targetLineNumber];
                        break;
                    }
                }
                Debug.Assert(EqualSpanStacks(engine.CurrentSpanStack, storedSpanStacks[currentLine - 1]));
                engine.ScanLine(document, document.GetLineByNumber(currentLine));
                UpdateTreeList(currentLine);
            }
            Debug.Assert(EqualSpanStacks(engine.CurrentSpanStack, storedSpanStacks[targetLineNumber]));
        }

        private void UpdateTreeList(int lineNumber)
        {
            if (!EqualSpanStacks(engine.CurrentSpanStack, storedSpanStacks[lineNumber]))
            {
                isValid[lineNumber] = true;

                storedSpanStacks[lineNumber] = engine.CurrentSpanStack;
                if (lineNumber + 1 < isValid.Count)
                {
                    isValid[lineNumber + 1] = false;
                    firstInvalidLine = lineNumber + 1;
                }
                else
                {
                    firstInvalidLine = int.MaxValue;
                }
                if (lineNumber + 1 < document.LineCount)
                {
                    OnHighlightStateChanged(lineNumber + 1, lineNumber + 1);
                }
            }
            else if (firstInvalidLine == lineNumber)
            {
                isValid[lineNumber] = true;
                firstInvalidLine = isValid.IndexOf(false);
                if (firstInvalidLine < 0)
                {
                    firstInvalidLine = int.MaxValue;
                }
            }
        }

        private static bool EqualSpanStacks(SpanStack a, SpanStack b)
        {
            if (a == b)
            {
                return true;
            }
            if (a == null || b == null)
            {
                return false;
            }
            while (!a.IsEmpty && !b.IsEmpty)
            {
                if (a.Peek() != b.Peek())
                {
                    return false;
                }
                a = a.Pop();
                b = b.Pop();
                if (a == b)
                {
                    return true;
                }
            }
            return a.IsEmpty && b.IsEmpty;
        }

        public event HighlightingStateChangedEventHandler HighlightingStateChanged;

        protected virtual void OnHighlightStateChanged(int fromLineNumber, int toLineNumber)
        {
            if (HighlightingStateChanged != null)
            {
                HighlightingStateChanged(fromLineNumber, toLineNumber);
            }
        }

        public HighlightingColor DefaultTextColor
        {
            get { return null; }
        }

        public void BeginHighlighting()
        {
            if (isInHighlightingGroup)
            {
                throw new InvalidOperationException("Highlighting group is already open");
            }
            isInHighlightingGroup = true;
        }

        public void EndHighlighting()
        {
            if (!isInHighlightingGroup)
            {
                throw new InvalidOperationException("Highlighting group is not open");
            }
            isInHighlightingGroup = false;
        }

        public HighlightingColor GetNamedColor(string name)
        {
            return definition.GetNamedColor(name);
        }
    }
}