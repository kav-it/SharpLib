using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit
{
    [Localizability(LocalizationCategory.Text), ContentProperty("Text")]
    public class TextEditor : Control, ITextEditorComponent, IServiceProvider, IWeakEventListener
    {
        #region Constructors

        static TextEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEditor), new FrameworkPropertyMetadata(typeof(TextEditor)));
            FocusableProperty.OverrideMetadata(typeof(TextEditor), new FrameworkPropertyMetadata(Boxes.True));
        }

        public TextEditor()
            : this(new TextArea())
        {
        }

        protected TextEditor(TextArea textArea)
        {
            if (textArea == null)
            {
                throw new ArgumentNullException("textArea");
            }
            this.textArea = textArea;

            textArea.TextView.Services.AddService(typeof(TextEditor), this);

            SetCurrentValue(OptionsProperty, textArea.Options);
            SetCurrentValue(DocumentProperty, new TextDocument());
        }

        #endregion

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new TextEditorAutomationPeer(this);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            if (e.NewFocus == this)
            {
                Keyboard.Focus(TextArea);
                e.Handled = true;
            }
        }

        #region Document property

        public static readonly DependencyProperty DocumentProperty
            = TextView.DocumentProperty.AddOwner(
                typeof(TextEditor), new FrameworkPropertyMetadata(OnDocumentChanged));

        public TextDocument Document
        {
            get { return (TextDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public event EventHandler DocumentChanged;

        protected virtual void OnDocumentChanged(EventArgs e)
        {
            if (DocumentChanged != null)
            {
                DocumentChanged(this, e);
            }
        }

        private static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            ((TextEditor)dp).OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
        }

        private void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
        {
            if (oldValue != null)
            {
                TextDocumentWeakEventManager.TextChanged.RemoveListener(oldValue, this);
                PropertyChangedEventManager.RemoveListener(oldValue.UndoStack, this, "IsOriginalFile");
            }
            textArea.Document = newValue;
            if (newValue != null)
            {
                TextDocumentWeakEventManager.TextChanged.AddListener(newValue, this);
                PropertyChangedEventManager.AddListener(newValue.UndoStack, this, "IsOriginalFile");
            }
            OnDocumentChanged(EventArgs.Empty);
            OnTextChanged(EventArgs.Empty);
        }

        #endregion

        #region Options property

        public static readonly DependencyProperty OptionsProperty
            = TextView.OptionsProperty.AddOwner(typeof(TextEditor), new FrameworkPropertyMetadata(OnOptionsChanged));

        public TextEditorOptions Options
        {
            get { return (TextEditorOptions)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public event PropertyChangedEventHandler OptionChanged;

        protected virtual void OnOptionChanged(PropertyChangedEventArgs e)
        {
            if (OptionChanged != null)
            {
                OptionChanged(this, e);
            }
        }

        private static void OnOptionsChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            ((TextEditor)dp).OnOptionsChanged((TextEditorOptions)e.OldValue, (TextEditorOptions)e.NewValue);
        }

        private void OnOptionsChanged(TextEditorOptions oldValue, TextEditorOptions newValue)
        {
            if (oldValue != null)
            {
                PropertyChangedWeakEventManager.RemoveListener(oldValue, this);
            }
            textArea.Options = newValue;
            if (newValue != null)
            {
                PropertyChangedWeakEventManager.AddListener(newValue, this);
            }
            OnOptionChanged(new PropertyChangedEventArgs(null));
        }

        protected virtual bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedWeakEventManager))
            {
                OnOptionChanged((PropertyChangedEventArgs)e);
                return true;
            }
            if (managerType == typeof(TextDocumentWeakEventManager.TextChanged))
            {
                OnTextChanged(e);
                return true;
            }
            if (managerType == typeof(PropertyChangedEventManager))
            {
                return HandleIsOriginalChanged((PropertyChangedEventArgs)e);
            }
            return false;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return ReceiveWeakEvent(managerType, sender, e);
        }

        #endregion

        #region Text property

        [Localizability(LocalizationCategory.Text), DefaultValue("")]
        public string Text
        {
            get
            {
                var document = Document;
                return document != null ? document.Text : string.Empty;
            }
            set
            {
                var document = GetDocument();
                document.Text = value ?? string.Empty;

                CaretOffset = 0;
                document.UndoStack.ClearAll();
            }
        }

        private TextDocument GetDocument()
        {
            var document = Document;
            if (document == null)
            {
                throw ThrowUtil.NoDocumentAssigned();
            }
            return document;
        }

        public event EventHandler TextChanged;

        protected virtual void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }
        }

        #endregion

        #region TextArea / ScrollViewer properties

        private readonly TextArea textArea;

        private ScrollViewer scrollViewer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scrollViewer = (ScrollViewer)Template.FindName("PART_ScrollViewer", this);
        }

        public TextArea TextArea
        {
            get { return textArea; }
        }

        internal ScrollViewer ScrollViewer
        {
            get { return scrollViewer; }
        }

        private bool CanExecute(RoutedUICommand command)
        {
            var textArea = TextArea;
            if (textArea == null)
            {
                return false;
            }
            return command.CanExecute(null, textArea);
        }

        private void Execute(RoutedUICommand command)
        {
            var textArea = TextArea;
            if (textArea != null)
            {
                command.Execute(null, textArea);
            }
        }

        #endregion

        #region Syntax highlighting

        public static readonly DependencyProperty SyntaxHighlightingProperty =
            DependencyProperty.Register("SyntaxHighlighting", typeof(IHighlightingDefinition), typeof(TextEditor),
                new FrameworkPropertyMetadata(OnSyntaxHighlightingChanged));

        public IHighlightingDefinition SyntaxHighlighting
        {
            get { return (IHighlightingDefinition)GetValue(SyntaxHighlightingProperty); }
            set { SetValue(SyntaxHighlightingProperty, value); }
        }

        private IVisualLineTransformer colorizer;

        private static void OnSyntaxHighlightingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextEditor)d).OnSyntaxHighlightingChanged(e.NewValue as IHighlightingDefinition);
        }

        private void OnSyntaxHighlightingChanged(IHighlightingDefinition newValue)
        {
            if (colorizer != null)
            {
                TextArea.TextView.LineTransformers.Remove(colorizer);
                colorizer = null;
            }
            if (newValue != null)
            {
                colorizer = CreateColorizer(newValue);
                if (colorizer != null)
                {
                    TextArea.TextView.LineTransformers.Insert(0, colorizer);
                }
            }
        }

        protected virtual IVisualLineTransformer CreateColorizer(IHighlightingDefinition highlightingDefinition)
        {
            if (highlightingDefinition == null)
            {
                throw new ArgumentNullException("highlightingDefinition");
            }
            return new HighlightingColorizer(highlightingDefinition);
        }

        #endregion

        #region WordWrap

        public static readonly DependencyProperty WordWrapProperty =
            DependencyProperty.Register("WordWrap", typeof(bool), typeof(TextEditor),
                new FrameworkPropertyMetadata(Boxes.False));

        public bool WordWrap
        {
            get { return (bool)GetValue(WordWrapProperty); }
            set { SetValue(WordWrapProperty, Boxes.Box(value)); }
        }

        #endregion

        #region IsReadOnly

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TextEditor),
                new FrameworkPropertyMetadata(Boxes.False, OnIsReadOnlyChanged));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, Boxes.Box(value)); }
        }

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as TextEditor;
            if (editor != null)
            {
                if ((bool)e.NewValue)
                {
                    editor.TextArea.ReadOnlySectionProvider = ReadOnlySectionDocument.Instance;
                }
                else
                {
                    editor.TextArea.ReadOnlySectionProvider = NoReadOnlySections.Instance;
                }

                var peer = UIElementAutomationPeer.FromElement(editor) as TextEditorAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseIsReadOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
                }
            }
        }

        #endregion

        #region IsModified

        public static readonly DependencyProperty IsModifiedProperty =
            DependencyProperty.Register("IsModified", typeof(bool), typeof(TextEditor),
                new FrameworkPropertyMetadata(Boxes.False, OnIsModifiedChanged));

        public bool IsModified
        {
            get { return (bool)GetValue(IsModifiedProperty); }
            set { SetValue(IsModifiedProperty, Boxes.Box(value)); }
        }

        private static void OnIsModifiedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as TextEditor;
            if (editor != null)
            {
                var document = editor.Document;
                if (document != null)
                {
                    var undoStack = document.UndoStack;
                    if ((bool)e.NewValue)
                    {
                        if (undoStack.IsOriginalFile)
                        {
                            undoStack.DiscardOriginalFileMarker();
                        }
                    }
                    else
                    {
                        undoStack.MarkAsOriginalFile();
                    }
                }
            }
        }

        private bool HandleIsOriginalChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsOriginalFile")
            {
                var document = Document;
                if (document != null)
                {
                    SetCurrentValue(IsModifiedProperty, Boxes.Box(!document.UndoStack.IsOriginalFile));
                }
                return true;
            }
            return false;
        }

        #endregion

        #region ShowLineNumbers

        public static readonly DependencyProperty ShowLineNumbersProperty =
            DependencyProperty.Register("ShowLineNumbers", typeof(bool), typeof(TextEditor),
                new FrameworkPropertyMetadata(Boxes.False, OnShowLineNumbersChanged));

        public bool ShowLineNumbers
        {
            get { return (bool)GetValue(ShowLineNumbersProperty); }
            set { SetValue(ShowLineNumbersProperty, Boxes.Box(value)); }
        }

        private static void OnShowLineNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TextEditor)d;
            var leftMargins = editor.TextArea.LeftMargins;
            if ((bool)e.NewValue)
            {
                var lineNumbers = new LineNumberMargin();
                var line = (Line)DottedLineMargin.Create();
                leftMargins.Insert(0, lineNumbers);
                leftMargins.Insert(1, line);
                var lineNumbersForeground = new Binding("LineNumbersForeground")
                {
                    Source = editor
                };
                line.SetBinding(Shape.StrokeProperty, lineNumbersForeground);
                lineNumbers.SetBinding(Control.ForegroundProperty, lineNumbersForeground);
            }
            else
            {
                for (int i = 0; i < leftMargins.Count; i++)
                {
                    if (leftMargins[i] is LineNumberMargin)
                    {
                        leftMargins.RemoveAt(i);
                        if (i < leftMargins.Count && DottedLineMargin.IsDottedLineMargin(leftMargins[i]))
                        {
                            leftMargins.RemoveAt(i);
                        }
                        break;
                    }
                }
            }
        }

        #endregion

        #region LineNumbersForeground

        public static readonly DependencyProperty LineNumbersForegroundProperty =
            DependencyProperty.Register("LineNumbersForeground", typeof(Brush), typeof(TextEditor),
                new FrameworkPropertyMetadata(Brushes.Gray, OnLineNumbersForegroundChanged));

        public Brush LineNumbersForeground
        {
            get { return (Brush)GetValue(LineNumbersForegroundProperty); }
            set { SetValue(LineNumbersForegroundProperty, value); }
        }

        private static void OnLineNumbersForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (TextEditor)d;
            var lineNumberMargin = editor.TextArea.LeftMargins.FirstOrDefault(margin => margin is LineNumberMargin) as LineNumberMargin;
            ;

            if (lineNumberMargin != null)
            {
                lineNumberMargin.SetValue(Control.ForegroundProperty, e.NewValue);
            }
        }

        #endregion

        #region TextBoxBase-like methods

        public void AppendText(string textData)
        {
            var document = GetDocument();
            document.Insert(document.TextLength, textData);
        }

        public void BeginChange()
        {
            GetDocument().BeginUpdate();
        }

        public void Copy()
        {
            Execute(ApplicationCommands.Copy);
        }

        public void Cut()
        {
            Execute(ApplicationCommands.Cut);
        }

        public IDisposable DeclareChangeBlock()
        {
            return GetDocument().RunUpdate();
        }

        public void EndChange()
        {
            GetDocument().EndUpdate();
        }

        public void LineDown()
        {
            if (scrollViewer != null)
            {
                scrollViewer.LineDown();
            }
        }

        public void LineLeft()
        {
            if (scrollViewer != null)
            {
                scrollViewer.LineLeft();
            }
        }

        public void LineRight()
        {
            if (scrollViewer != null)
            {
                scrollViewer.LineRight();
            }
        }

        public void LineUp()
        {
            if (scrollViewer != null)
            {
                scrollViewer.LineUp();
            }
        }

        public void PageDown()
        {
            if (scrollViewer != null)
            {
                scrollViewer.PageDown();
            }
        }

        public void PageUp()
        {
            if (scrollViewer != null)
            {
                scrollViewer.PageUp();
            }
        }

        public void PageLeft()
        {
            if (scrollViewer != null)
            {
                scrollViewer.PageLeft();
            }
        }

        public void PageRight()
        {
            if (scrollViewer != null)
            {
                scrollViewer.PageRight();
            }
        }

        public void Paste()
        {
            Execute(ApplicationCommands.Paste);
        }

        public bool Redo()
        {
            if (CanExecute(ApplicationCommands.Redo))
            {
                Execute(ApplicationCommands.Redo);
                return true;
            }
            return false;
        }

        public void ScrollToEnd()
        {
            ApplyTemplate();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToEnd();
            }
        }

        public void ScrollToHome()
        {
            ApplyTemplate();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHome();
            }
        }

        public void ScrollToHorizontalOffset(double offset)
        {
            ApplyTemplate();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset(offset);
            }
        }

        public void ScrollToVerticalOffset(double offset)
        {
            ApplyTemplate();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(offset);
            }
        }

        public void SelectAll()
        {
            Execute(ApplicationCommands.SelectAll);
        }

        public bool Undo()
        {
            if (CanExecute(ApplicationCommands.Undo))
            {
                Execute(ApplicationCommands.Undo);
                return true;
            }
            return false;
        }

        public bool CanRedo
        {
            get { return CanExecute(ApplicationCommands.Redo); }
        }

        public bool CanUndo
        {
            get { return CanExecute(ApplicationCommands.Undo); }
        }

        public double ExtentHeight
        {
            get { return scrollViewer != null ? scrollViewer.ExtentHeight : 0; }
        }

        public double ExtentWidth
        {
            get { return scrollViewer != null ? scrollViewer.ExtentWidth : 0; }
        }

        public double ViewportHeight
        {
            get { return scrollViewer != null ? scrollViewer.ViewportHeight : 0; }
        }

        public double ViewportWidth
        {
            get { return scrollViewer != null ? scrollViewer.ViewportWidth : 0; }
        }

        public double VerticalOffset
        {
            get { return scrollViewer != null ? scrollViewer.VerticalOffset : 0; }
        }

        public double HorizontalOffset
        {
            get { return scrollViewer != null ? scrollViewer.HorizontalOffset : 0; }
        }

        #endregion

        #region TextBox methods

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedText
        {
            get
            {
                var textArea = TextArea;

                if (textArea != null && textArea.Document != null && !textArea.Selection.IsEmpty)
                {
                    return textArea.Document.GetText(textArea.Selection.SurroundingSegment);
                }
                return string.Empty;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                var textArea = TextArea;
                if (textArea != null && textArea.Document != null)
                {
                    int offset = SelectionStart;
                    int length = SelectionLength;
                    textArea.Document.Replace(offset, length, value);

                    textArea.Selection = Selection.Create(textArea, offset, offset + value.Length);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CaretOffset
        {
            get
            {
                var textArea = TextArea;
                if (textArea != null)
                {
                    return textArea.Caret.Offset;
                }
                return 0;
            }
            set
            {
                var textArea = TextArea;
                if (textArea != null)
                {
                    textArea.Caret.Offset = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionStart
        {
            get
            {
                var textArea = TextArea;
                if (textArea != null)
                {
                    if (textArea.Selection.IsEmpty)
                    {
                        return textArea.Caret.Offset;
                    }
                    return textArea.Selection.SurroundingSegment.Offset;
                }
                return 0;
            }
            set { Select(value, SelectionLength); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionLength
        {
            get
            {
                var textArea = TextArea;
                if (textArea != null && !textArea.Selection.IsEmpty)
                {
                    return textArea.Selection.SurroundingSegment.Length;
                }
                return 0;
            }
            set { Select(SelectionStart, value); }
        }

        public void Select(int start, int length)
        {
            int documentLength = Document != null ? Document.TextLength : 0;
            if (start < 0 || start > documentLength)
            {
                throw new ArgumentOutOfRangeException("start", start, "Value must be between 0 and " + documentLength);
            }
            if (length < 0 || start + length > documentLength)
            {
                throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + (documentLength - start));
            }
            textArea.Selection = Selection.Create(textArea, start, start + length);
            textArea.Caret.Offset = start + length;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LineCount
        {
            get
            {
                var document = Document;
                if (document != null)
                {
                    return document.LineCount;
                }
                return 1;
            }
        }

        public void Clear()
        {
            Text = string.Empty;
        }

        #endregion

        #region Loading from stream

        public void Load(Stream stream)
        {
            using (var reader = FileReader.OpenStream(stream, Encoding ?? Encoding.UTF8))
            {
                Text = reader.ReadToEnd();
                SetCurrentValue(EncodingProperty, reader.CurrentEncoding);
            }
            SetCurrentValue(IsModifiedProperty, Boxes.False);
        }

        public void Load(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Load(fs);
            }
        }

        public static readonly DependencyProperty EncodingProperty =
            DependencyProperty.Register("Encoding", typeof(Encoding), typeof(TextEditor));

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Encoding Encoding
        {
            get { return (Encoding)GetValue(EncodingProperty); }
            set { SetValue(EncodingProperty, value); }
        }

        public void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            var encoding = Encoding;
            var document = Document;
            var writer = encoding != null ? new StreamWriter(stream, encoding) : new StreamWriter(stream);
            if (document != null)
            {
                document.WriteTextTo(writer);
            }
            writer.Flush();

            SetCurrentValue(IsModifiedProperty, Boxes.False);
        }

        public void Save(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(fs);
            }
        }

        #endregion

        #region MouseHover events

        public static readonly RoutedEvent PreviewMouseHoverEvent =
            TextView.PreviewMouseHoverEvent.AddOwner(typeof(TextEditor));

        public static readonly RoutedEvent MouseHoverEvent =
            TextView.MouseHoverEvent.AddOwner(typeof(TextEditor));

        public static readonly RoutedEvent PreviewMouseHoverStoppedEvent =
            TextView.PreviewMouseHoverStoppedEvent.AddOwner(typeof(TextEditor));

        public static readonly RoutedEvent MouseHoverStoppedEvent =
            TextView.MouseHoverStoppedEvent.AddOwner(typeof(TextEditor));

        public event MouseEventHandler PreviewMouseHover
        {
            add { AddHandler(PreviewMouseHoverEvent, value); }
            remove { RemoveHandler(PreviewMouseHoverEvent, value); }
        }

        public event MouseEventHandler MouseHover
        {
            add { AddHandler(MouseHoverEvent, value); }
            remove { RemoveHandler(MouseHoverEvent, value); }
        }

        public event MouseEventHandler PreviewMouseHoverStopped
        {
            add { AddHandler(PreviewMouseHoverStoppedEvent, value); }
            remove { RemoveHandler(PreviewMouseHoverStoppedEvent, value); }
        }

        public event MouseEventHandler MouseHoverStopped
        {
            add { AddHandler(MouseHoverStoppedEvent, value); }
            remove { RemoveHandler(MouseHoverStoppedEvent, value); }
        }

        #endregion

        #region ScrollBarVisibility

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty.AddOwner(typeof(TextEditor),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Visible));

        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty.AddOwner(typeof(TextEditor),
            new FrameworkPropertyMetadata(ScrollBarVisibility.Visible));

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        #endregion

        object IServiceProvider.GetService(Type serviceType)
        {
            return textArea.GetService(serviceType);
        }

        public TextViewPosition? GetPositionFromPoint(Point point)
        {
            if (Document == null)
            {
                return null;
            }
            var textView = TextArea.TextView;
            return textView.GetPosition(TranslatePoint(point, textView) + textView.ScrollOffset);
        }

        public void ScrollToLine(int line)
        {
            ScrollTo(line, -1);
        }

        public void ScrollTo(int line, int column)
        {
            const double MinimumScrollPercentage = 0.3;

            var textView = textArea.TextView;
            var document = textView.Document;
            if (scrollViewer != null && document != null)
            {
                if (line < 1)
                {
                    line = 1;
                }
                if (line > document.LineCount)
                {
                    line = document.LineCount;
                }

                IScrollInfo scrollInfo = textView;
                if (!scrollInfo.CanHorizontallyScroll)
                {
                    var vl = textView.GetOrConstructVisualLine(document.GetLineByNumber(line));
                    double remainingHeight = scrollViewer.ViewportHeight / 2;
                    while (remainingHeight > 0)
                    {
                        var prevLine = vl.FirstDocumentLine.PreviousLine;
                        if (prevLine == null)
                        {
                            break;
                        }
                        vl = textView.GetOrConstructVisualLine(prevLine);
                        remainingHeight -= vl.Height;
                    }
                }

                var p = textArea.TextView.GetVisualPosition(new TextViewPosition(line, Math.Max(1, column)), VisualYPosition.LineMiddle);
                double verticalPos = p.Y - scrollViewer.ViewportHeight / 2;
                if (Math.Abs(verticalPos - scrollViewer.VerticalOffset) > MinimumScrollPercentage * scrollViewer.ViewportHeight)
                {
                    scrollViewer.ScrollToVerticalOffset(Math.Max(0, verticalPos));
                }
                if (column > 0)
                {
                    if (p.X > scrollViewer.ViewportWidth - Caret.MinimumDistanceToViewBorder * 2)
                    {
                        double horizontalPos = Math.Max(0, p.X - scrollViewer.ViewportWidth / 2);
                        if (Math.Abs(horizontalPos - scrollViewer.HorizontalOffset) > MinimumScrollPercentage * scrollViewer.ViewportWidth)
                        {
                            scrollViewer.ScrollToHorizontalOffset(horizontalPos);
                        }
                    }
                    else
                    {
                        scrollViewer.ScrollToHorizontalOffset(0);
                    }
                }
            }
        }
    }
}