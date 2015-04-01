using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing
{
    public class TextArea : Control, IScrollInfo, IWeakEventListener, ITextEditorComponent, IServiceProvider
    {
        #region Поля

        public static readonly DependencyProperty DocumentProperty
            = TextView.DocumentProperty.AddOwner(typeof(TextArea), new FrameworkPropertyMetadata(OnDocumentChanged));

        public static readonly DependencyProperty IndentationStrategyProperty =
            DependencyProperty.Register("IndentationStrategy", typeof(IIndentationStrategy), typeof(TextArea),
                new FrameworkPropertyMetadata(new DefaultIndentationStrategy()));

        public static readonly DependencyProperty OptionsProperty
            = TextView.OptionsProperty.AddOwner(typeof(TextArea), new FrameworkPropertyMetadata(OnOptionsChanged));

        public static readonly DependencyProperty OverstrikeModeProperty =
            DependencyProperty.Register("OverstrikeMode", typeof(bool), typeof(TextArea),
                new FrameworkPropertyMetadata(Boxes.False));

        public static readonly DependencyProperty SelectionBorderProperty =
            DependencyProperty.Register("SelectionBorder", typeof(Pen), typeof(TextArea));

        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register("SelectionBrush", typeof(Brush), typeof(TextArea));

        public static readonly DependencyProperty SelectionCornerRadiusProperty =
            DependencyProperty.Register("SelectionCornerRadius", typeof(double), typeof(TextArea),
                new FrameworkPropertyMetadata(3.0));

        public static readonly DependencyProperty SelectionForegroundProperty =
            DependencyProperty.Register("SelectionForeground", typeof(Brush), typeof(TextArea));

        private readonly Caret caret;

        internal readonly Selection emptySelection;

        internal readonly ImeSupport ime;

        private readonly ObservableCollection<UIElement> leftMargins = new ObservableCollection<UIElement>();

        private readonly TextView textView;

        private ITextAreaInputHandler activeInputHandler;

        private int allowCaretOutsideSelection;

        private bool canHorizontallyScroll;

        private bool canVerticallyScroll;

        private bool ensureSelectionValidRequested;

        private bool isChangingInputHandler;

        private bool isMouseCursorHidden;

        private IReadOnlySectionProvider readOnlySectionProvider = NoReadOnlySections.Instance;

        private IScrollInfo scrollInfo;

        private ScrollViewer scrollOwner;

        private Selection selection;

        private ImmutableStack<TextAreaStackedInputHandler> stackedInputHandlers = ImmutableStack<TextAreaStackedInputHandler>.Empty;

        #endregion

        #region Свойства

        public TextAreaDefaultInputHandler DefaultInputHandler { get; private set; }

        public ITextAreaInputHandler ActiveInputHandler
        {
            get { return activeInputHandler; }
            set
            {
                if (value != null && value.TextArea != this)
                {
                    throw new ArgumentException("The input handler was created for a different text area than this one.");
                }
                if (isChangingInputHandler)
                {
                    throw new InvalidOperationException("Cannot set ActiveInputHandler recursively");
                }
                if (activeInputHandler != value)
                {
                    isChangingInputHandler = true;
                    try
                    {
                        PopStackedInputHandler(stackedInputHandlers.LastOrDefault());
                        Debug.Assert(stackedInputHandlers.IsEmpty);

                        if (activeInputHandler != null)
                        {
                            activeInputHandler.Detach();
                        }
                        activeInputHandler = value;
                        if (value != null)
                        {
                            value.Attach();
                        }
                    }
                    finally
                    {
                        isChangingInputHandler = false;
                    }
                    if (ActiveInputHandlerChanged != null)
                    {
                        ActiveInputHandlerChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public ImmutableStack<TextAreaStackedInputHandler> StackedInputHandlers
        {
            get { return stackedInputHandlers; }
        }

        public TextDocument Document
        {
            get { return (TextDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public TextEditorOptions Options
        {
            get { return (TextEditorOptions)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public TextView TextView
        {
            get { return textView; }
        }

        public Selection Selection
        {
            get { return selection; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (value.textArea != this)
                {
                    throw new ArgumentException("Cannot use a Selection instance that belongs to another text area.");
                }
                if (!object.Equals(selection, value))
                {
                    if (textView != null)
                    {
                        var oldSegment = selection.SurroundingSegment;
                        var newSegment = value.SurroundingSegment;
                        if (!Selection.EnableVirtualSpace && (selection is SimpleSelection && value is SimpleSelection && oldSegment != null && newSegment != null))
                        {
                            int oldSegmentOffset = oldSegment.Offset;
                            int newSegmentOffset = newSegment.Offset;
                            if (oldSegmentOffset != newSegmentOffset)
                            {
                                textView.Redraw(Math.Min(oldSegmentOffset, newSegmentOffset),
                                    Math.Abs(oldSegmentOffset - newSegmentOffset),
                                    DispatcherPriority.Background);
                            }
                            int oldSegmentEndOffset = oldSegment.EndOffset;
                            int newSegmentEndOffset = newSegment.EndOffset;
                            if (oldSegmentEndOffset != newSegmentEndOffset)
                            {
                                textView.Redraw(Math.Min(oldSegmentEndOffset, newSegmentEndOffset),
                                    Math.Abs(oldSegmentEndOffset - newSegmentEndOffset),
                                    DispatcherPriority.Background);
                            }
                        }
                        else
                        {
                            textView.Redraw(oldSegment, DispatcherPriority.Background);
                            textView.Redraw(newSegment, DispatcherPriority.Background);
                        }
                    }
                    selection = value;
                    if (SelectionChanged != null)
                    {
                        SelectionChanged(this, EventArgs.Empty);
                    }

                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public Brush SelectionBrush
        {
            get { return (Brush)GetValue(SelectionBrushProperty); }
            set { SetValue(SelectionBrushProperty, value); }
        }

        public Brush SelectionForeground
        {
            get { return (Brush)GetValue(SelectionForegroundProperty); }
            set { SetValue(SelectionForegroundProperty, value); }
        }

        public Pen SelectionBorder
        {
            get { return (Pen)GetValue(SelectionBorderProperty); }
            set { SetValue(SelectionBorderProperty, value); }
        }

        public double SelectionCornerRadius
        {
            get { return (double)GetValue(SelectionCornerRadiusProperty); }
            set { SetValue(SelectionCornerRadiusProperty, value); }
        }

        public Caret Caret
        {
            get { return caret; }
        }

        public ObservableCollection<UIElement> LeftMargins
        {
            get { return leftMargins; }
        }

        public IReadOnlySectionProvider ReadOnlySectionProvider
        {
            get { return readOnlySectionProvider; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                readOnlySectionProvider = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        bool IScrollInfo.CanVerticallyScroll
        {
            get { return scrollInfo != null ? scrollInfo.CanVerticallyScroll : false; }
            set
            {
                canVerticallyScroll = value;
                if (scrollInfo != null)
                {
                    scrollInfo.CanVerticallyScroll = value;
                }
            }
        }

        bool IScrollInfo.CanHorizontallyScroll
        {
            get { return scrollInfo != null ? scrollInfo.CanHorizontallyScroll : false; }
            set
            {
                canHorizontallyScroll = value;
                if (scrollInfo != null)
                {
                    scrollInfo.CanHorizontallyScroll = value;
                }
            }
        }

        double IScrollInfo.ExtentWidth
        {
            get { return scrollInfo != null ? scrollInfo.ExtentWidth : 0; }
        }

        double IScrollInfo.ExtentHeight
        {
            get { return scrollInfo != null ? scrollInfo.ExtentHeight : 0; }
        }

        double IScrollInfo.ViewportWidth
        {
            get { return scrollInfo != null ? scrollInfo.ViewportWidth : 0; }
        }

        double IScrollInfo.ViewportHeight
        {
            get { return scrollInfo != null ? scrollInfo.ViewportHeight : 0; }
        }

        double IScrollInfo.HorizontalOffset
        {
            get { return scrollInfo != null ? scrollInfo.HorizontalOffset : 0; }
        }

        double IScrollInfo.VerticalOffset
        {
            get { return scrollInfo != null ? scrollInfo.VerticalOffset : 0; }
        }

        ScrollViewer IScrollInfo.ScrollOwner
        {
            get { return scrollInfo != null ? scrollInfo.ScrollOwner : null; }
            set
            {
                if (scrollInfo != null)
                {
                    scrollInfo.ScrollOwner = value;
                }
                else
                {
                    scrollOwner = value;
                }
            }
        }

        public IIndentationStrategy IndentationStrategy
        {
            get { return (IIndentationStrategy)GetValue(IndentationStrategyProperty); }
            set { SetValue(IndentationStrategyProperty, value); }
        }

        public bool OverstrikeMode
        {
            get { return (bool)GetValue(OverstrikeModeProperty); }
            set { SetValue(OverstrikeModeProperty, value); }
        }

        #endregion

        #region События

        public event EventHandler ActiveInputHandlerChanged;

        public event EventHandler DocumentChanged;

        public event PropertyChangedEventHandler OptionChanged;

        public event EventHandler SelectionChanged;

        public event EventHandler<TextEventArgs> TextCopied;

        public event TextCompositionEventHandler TextEntered;

        public event TextCompositionEventHandler TextEntering;

        #endregion

        #region Конструктор

        static TextArea()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextArea),
                new FrameworkPropertyMetadata(typeof(TextArea)));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(
                typeof(TextArea), new FrameworkPropertyMetadata(Boxes.True));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(
                typeof(TextArea), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
            FocusableProperty.OverrideMetadata(
                typeof(TextArea), new FrameworkPropertyMetadata(Boxes.True));
        }

        public TextArea()
            : this(new TextView())
        {
        }

        protected TextArea(TextView textView)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }
            this.textView = textView;
            Options = textView.Options;

            selection = emptySelection = new EmptySelection(this);

            textView.Services.AddService(typeof(TextArea), this);

            textView.LineTransformers.Add(new SelectionColorizer(this));
            textView.InsertLayer(new SelectionLayer(this), KnownLayer.Selection, LayerInsertionPosition.Replace);

            caret = new Caret(this);
            caret.PositionChanged += (sender, e) => RequestSelectionValidation();
            caret.PositionChanged += CaretPositionChanged;
            AttachTypingEvents();
            ime = new ImeSupport(this);

            leftMargins.CollectionChanged += leftMargins_CollectionChanged;

            DefaultInputHandler = new TextAreaDefaultInputHandler(this);
            ActiveInputHandler = DefaultInputHandler;
        }

        #endregion

        #region Методы

        public void PushStackedInputHandler(TextAreaStackedInputHandler inputHandler)
        {
            if (inputHandler == null)
            {
                throw new ArgumentNullException("inputHandler");
            }
            stackedInputHandlers = stackedInputHandlers.Push(inputHandler);
            inputHandler.Attach();
        }

        public void PopStackedInputHandler(TextAreaStackedInputHandler inputHandler)
        {
            if (stackedInputHandlers.Any(i => i == inputHandler))
            {
                ITextAreaInputHandler oldHandler;
                do
                {
                    oldHandler = stackedInputHandlers.Peek();
                    stackedInputHandlers = stackedInputHandlers.Pop();
                    oldHandler.Detach();
                } while (oldHandler != inputHandler);
            }
        }

        private static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            ((TextArea)dp).OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
        }

        private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
        {
            if (oldValue != null)
            {
                TextDocumentWeakEventManager.Changing.RemoveListener(oldValue, this);
                TextDocumentWeakEventManager.Changed.RemoveListener(oldValue, this);
                TextDocumentWeakEventManager.UpdateStarted.RemoveListener(oldValue, this);
                TextDocumentWeakEventManager.UpdateFinished.RemoveListener(oldValue, this);
            }
            textView.Document = newValue;
            if (newValue != null)
            {
                TextDocumentWeakEventManager.Changing.AddListener(newValue, this);
                TextDocumentWeakEventManager.Changed.AddListener(newValue, this);
                TextDocumentWeakEventManager.UpdateStarted.AddListener(newValue, this);
                TextDocumentWeakEventManager.UpdateFinished.AddListener(newValue, this);
            }

            caret.Location = new TextLocation(1, 1);
            ClearSelection();
            if (DocumentChanged != null)
            {
                DocumentChanged(this, EventArgs.Empty);
            }
            CommandManager.InvalidateRequerySuggested();
        }

        protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
        {
            if (OptionChanged != null)
            {
                OptionChanged(this, e);
            }
        }

        private static void OnOptionsChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            ((TextArea)dp).OnOptionsChanged((TextEditorOptions)e.OldValue, (TextEditorOptions)e.NewValue);
        }

        private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
        {
            if (oldValue != null)
            {
                PropertyChangedWeakEventManager.RemoveListener(oldValue, this);
            }
            textView.Options = newValue;
            if (newValue != null)
            {
                PropertyChangedWeakEventManager.AddListener(newValue, this);
            }
            OnOptionChanged(new PropertyChangedEventArgs(null));
        }

        protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(TextDocumentWeakEventManager.Changing))
            {
                OnDocumentChanging();
                return true;
            }
            if (managerType == typeof(TextDocumentWeakEventManager.Changed))
            {
                OnDocumentChanged((DocumentChangeEventArgs)e);
                return true;
            }
            if (managerType == typeof(TextDocumentWeakEventManager.UpdateStarted))
            {
                OnUpdateStarted();
                return true;
            }
            if (managerType == typeof(TextDocumentWeakEventManager.UpdateFinished))
            {
                OnUpdateFinished();
                return true;
            }
            if (managerType == typeof(PropertyChangedWeakEventManager))
            {
                OnOptionChanged((PropertyChangedEventArgs)e);
                return true;
            }
            return false;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return ReceiveWeakEvent(managerType, sender, e);
        }

        private void OnDocumentChanging()
        {
            caret.OnDocumentChanging();
        }

        private void OnDocumentChanged(DocumentChangeEventArgs e)
        {
            caret.OnDocumentChanged(e);
            Selection = selection.UpdateOnDocumentChange(e);
        }

        private void OnUpdateStarted()
        {
            Document.UndoStack.PushOptional(new RestoreCaretAndSelectionUndoAction(this));
        }

        private void OnUpdateFinished()
        {
            caret.OnDocumentUpdateFinished();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scrollInfo = textView;
            ApplyScrollInfo();
        }

        public void ClearSelection()
        {
            Selection = emptySelection;
        }

        private void RequestSelectionValidation()
        {
            if (!ensureSelectionValidRequested && allowCaretOutsideSelection == 0)
            {
                ensureSelectionValidRequested = true;
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(EnsureSelectionValid));
            }
        }

        private void EnsureSelectionValid()
        {
            ensureSelectionValidRequested = false;
            if (allowCaretOutsideSelection == 0)
            {
                if (!selection.IsEmpty && !selection.Contains(caret.Offset))
                {
                    Debug.WriteLine("Resetting selection because caret is outside");
                    ClearSelection();
                }
            }
        }

        public IDisposable AllowCaretOutsideSelection()
        {
            VerifyAccess();
            allowCaretOutsideSelection++;
            return new CallbackOnDispose(
                delegate
                {
                    VerifyAccess();
                    allowCaretOutsideSelection--;
                    RequestSelectionValidation();
                });
        }

        private void CaretPositionChanged(object sender, EventArgs e)
        {
            if (textView == null)
            {
                return;
            }

            textView.HighlightedLine = Caret.Line;
        }

        private void leftMargins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ITextViewConnect c in e.OldItems.OfType<ITextViewConnect>())
                {
                    c.RemoveFromTextView(textView);
                }
            }
            if (e.NewItems != null)
            {
                foreach (ITextViewConnect c in e.NewItems.OfType<ITextViewConnect>())
                {
                    c.AddToTextView(textView);
                }
            }
        }

        private void ApplyScrollInfo()
        {
            if (scrollInfo != null)
            {
                scrollInfo.ScrollOwner = scrollOwner;
                scrollInfo.CanVerticallyScroll = canVerticallyScroll;
                scrollInfo.CanHorizontallyScroll = canHorizontallyScroll;
                scrollOwner = null;
            }
        }

        void IScrollInfo.LineUp()
        {
            if (scrollInfo != null)
            {
                scrollInfo.LineUp();
            }
        }

        void IScrollInfo.LineDown()
        {
            if (scrollInfo != null)
            {
                scrollInfo.LineDown();
            }
        }

        void IScrollInfo.LineLeft()
        {
            if (scrollInfo != null)
            {
                scrollInfo.LineLeft();
            }
        }

        void IScrollInfo.LineRight()
        {
            if (scrollInfo != null)
            {
                scrollInfo.LineRight();
            }
        }

        void IScrollInfo.PageUp()
        {
            if (scrollInfo != null)
            {
                scrollInfo.PageUp();
            }
        }

        void IScrollInfo.PageDown()
        {
            if (scrollInfo != null)
            {
                scrollInfo.PageDown();
            }
        }

        void IScrollInfo.PageLeft()
        {
            if (scrollInfo != null)
            {
                scrollInfo.PageLeft();
            }
        }

        void IScrollInfo.PageRight()
        {
            if (scrollInfo != null)
            {
                scrollInfo.PageRight();
            }
        }

        void IScrollInfo.MouseWheelUp()
        {
            if (scrollInfo != null)
            {
                scrollInfo.MouseWheelUp();
            }
        }

        void IScrollInfo.MouseWheelDown()
        {
            if (scrollInfo != null)
            {
                scrollInfo.MouseWheelDown();
            }
        }

        void IScrollInfo.MouseWheelLeft()
        {
            if (scrollInfo != null)
            {
                scrollInfo.MouseWheelLeft();
            }
        }

        void IScrollInfo.MouseWheelRight()
        {
            if (scrollInfo != null)
            {
                scrollInfo.MouseWheelRight();
            }
        }

        void IScrollInfo.SetHorizontalOffset(double offset)
        {
            if (scrollInfo != null)
            {
                scrollInfo.SetHorizontalOffset(offset);
            }
        }

        void IScrollInfo.SetVerticalOffset(double offset)
        {
            if (scrollInfo != null)
            {
                scrollInfo.SetVerticalOffset(offset);
            }
        }

        Rect IScrollInfo.MakeVisible(System.Windows.Media.Visual visual, Rect rectangle)
        {
            if (scrollInfo != null)
            {
                return scrollInfo.MakeVisible(visual, rectangle);
            }
            return Rect.Empty;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            ime.OnGotKeyboardFocus(e);
            caret.Show();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            caret.Hide();
            ime.OnLostKeyboardFocus(e);
        }

        protected virtual void OnTextEntering(TextCompositionEventArgs e)
        {
            if (TextEntering != null)
            {
                TextEntering(this, e);
            }
        }

        protected virtual void OnTextEntered(TextCompositionEventArgs e)
        {
            if (TextEntered != null)
            {
                TextEntered(this, e);
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
            if (!e.Handled && Document != null)
            {
                if (string.IsNullOrEmpty(e.Text) || e.Text == "\x1b" || e.Text == "\b")
                {
                    return;
                }
                HideMouseCursor();
                PerformTextInput(e);
                e.Handled = true;
            }
        }

        public void PerformTextInput(string text)
        {
            var textComposition = new TextComposition(InputManager.Current, this, text);
            var e = new TextCompositionEventArgs(Keyboard.PrimaryDevice, textComposition);
            e.RoutedEvent = TextInputEvent;
            PerformTextInput(e);
        }

        public void PerformTextInput(TextCompositionEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (Document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            OnTextEntering(e);
            if (!e.Handled)
            {
                if (e.Text == "\n" || e.Text == "\r" || e.Text == "\r\n")
                {
                    ReplaceSelectionWithNewLine();
                }
                else
                {
                    if (OverstrikeMode && Selection.IsEmpty && Document.GetLineByNumber(Caret.Line).EndOffset > Caret.Offset)
                    {
                        EditingCommands.SelectRightByCharacter.Execute(null, this);
                    }
                    ReplaceSelectionWithText(e.Text);
                }
                OnTextEntered(e);
                caret.BringCaretToView();
            }
        }

        private void ReplaceSelectionWithNewLine()
        {
            string newLine = TextUtilities.GetNewLineFromDocument(Document, Caret.Line);
            using (Document.RunUpdate())
            {
                ReplaceSelectionWithText(newLine);
                if (IndentationStrategy != null)
                {
                    var line = Document.GetLineByNumber(Caret.Line);
                    var deletable = GetDeletableSegments(line);
                    if (deletable.Length == 1 && deletable[0].Offset == line.Offset && deletable[0].Length == line.Length)
                    {
                        IndentationStrategy.IndentLine(Document, line);
                    }
                }
            }
        }

        internal void RemoveSelectedText()
        {
            if (Document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            selection.ReplaceSelectionWithText(string.Empty);
#if DEBUG
            if (!selection.IsEmpty)
            {
                foreach (ISegment s in selection.Segments)
                {
                    Debug.Assert(ReadOnlySectionProvider.GetDeletableSegments(s).Count() == 0);
                }
            }
#endif
        }

        internal void ReplaceSelectionWithText(string newText)
        {
            if (newText == null)
            {
                throw new ArgumentNullException("newText");
            }
            if (Document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            selection.ReplaceSelectionWithText(newText);
        }

        internal ISegment[] GetDeletableSegments(ISegment segment)
        {
            var deletableSegments = ReadOnlySectionProvider.GetDeletableSegments(segment);
            if (deletableSegments == null)
            {
                throw new InvalidOperationException("ReadOnlySectionProvider.GetDeletableSegments returned null");
            }
            var array = deletableSegments.ToArray();
            int lastIndex = segment.Offset;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Offset < lastIndex)
                {
                    throw new InvalidOperationException("ReadOnlySectionProvider returned incorrect segments (outside of input segment / wrong order)");
                }
                lastIndex = array[i].EndOffset;
            }
            if (lastIndex > segment.EndOffset)
            {
                throw new InvalidOperationException("ReadOnlySectionProvider returned incorrect segments (outside of input segment / wrong order)");
            }
            return array;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (!e.Handled && e.Key == Key.Insert && Options.AllowToggleOverstrikeMode)
            {
                OverstrikeMode = !OverstrikeMode;
                e.Handled = true;
                return;
            }

            foreach (TextAreaStackedInputHandler h in stackedInputHandlers)
            {
                if (e.Handled)
                {
                    break;
                }
                h.OnPreviewKeyDown(e);
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            foreach (TextAreaStackedInputHandler h in stackedInputHandlers)
            {
                if (e.Handled)
                {
                    break;
                }
                h.OnPreviewKeyUp(e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            TextView.InvalidateCursorIfMouseWithinTextView();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            TextView.InvalidateCursorIfMouseWithinTextView();
        }

        private void AttachTypingEvents()
        {
            MouseEnter += delegate { ShowMouseCursor(); };
            MouseLeave += delegate { ShowMouseCursor(); };
            PreviewMouseMove += delegate { ShowMouseCursor(); };
#if DOTNET4
            TouchEnter += delegate { ShowMouseCursor(); };
            TouchLeave += delegate { ShowMouseCursor(); };
            PreviewTouchMove += delegate { ShowMouseCursor(); };
#endif
        }

        private void ShowMouseCursor()
        {
            if (isMouseCursorHidden)
            {
                System.Windows.Forms.Cursor.Show();
                isMouseCursorHidden = false;
            }
        }

        private void HideMouseCursor()
        {
            if (Options.HideCursorWhileTyping && !isMouseCursorHidden && IsMouseOver)
            {
                isMouseCursorHidden = true;
                System.Windows.Forms.Cursor.Hide();
            }
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == SelectionBrushProperty
                || e.Property == SelectionBorderProperty
                || e.Property == SelectionForegroundProperty
                || e.Property == SelectionCornerRadiusProperty)
            {
                textView.Redraw();
            }
            else if (e.Property == OverstrikeModeProperty)
            {
                caret.UpdateIfVisible();
            }
        }

        public virtual object GetService(Type serviceType)
        {
            return textView.GetService(serviceType);
        }

        internal void OnTextCopied(TextEventArgs e)
        {
            if (TextCopied != null)
            {
                TextCopied(this, e);
            }
        }

        #endregion

        #region Вложенный класс: RestoreCaretAndSelectionUndoAction

        private sealed class RestoreCaretAndSelectionUndoAction : IUndoableOperation
        {
            #region Поля

            private readonly TextViewPosition caretPosition;

            private readonly Selection selection;

            private readonly WeakReference textAreaReference;

            #endregion

            #region Конструктор

            public RestoreCaretAndSelectionUndoAction(TextArea textArea)
            {
                textAreaReference = new WeakReference(textArea);

                caretPosition = textArea.Caret.NonValidatedPosition;
                selection = textArea.Selection;
            }

            #endregion

            #region Методы

            public void Undo()
            {
                var textArea = (TextArea)textAreaReference.Target;
                if (textArea != null)
                {
                    textArea.Caret.Position = caretPosition;
                    textArea.Selection = selection;
                }
            }

            public void Redo()
            {
                Undo();
            }

            #endregion
        }

        #endregion
    }
}